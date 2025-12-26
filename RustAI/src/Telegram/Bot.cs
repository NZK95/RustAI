using System.Drawing.Imaging;
using System.IO;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace RustAI
{
    public class TelegramBot
    {
        private Status _status;
        private string _serverId;
        private string _playerId;
        private TelegramBotClient _telegramClient;
        private CancellationTokenSource _cancellation;
        private KeyboardFactory _keyboardFactory;
        private readonly BotCommand[] _commands;
        private readonly string _botName;
        private readonly string _shortDescription;
        private readonly string _startMessage;

        public TelegramBot()
        {
            _status = Status.NONE;
            _botName = Constants.ProjectName;
            _shortDescription = Constants.ProjectShortDescription;
            _startMessage = Constants.ProjectStartMessage;
            _cancellation = new CancellationTokenSource();
            _keyboardFactory = new KeyboardFactory();

            _commands = new[]
            {
                    new BotCommand(command: "start", description: "Show RustAI main menu"),
                    new BotCommand(command: "players", description: "Show player information"),
                    new BotCommand(command: "servers", description: "Show server information"),
                    new BotCommand(command: "connect", description: "Connect to selected server. Launches Rust if necessary and switches active window to Rust"),
                    new BotCommand(command: "autoconnect", description: "Autoconnect to selected server when it is online. Use it when the server is turned off"),
                    new BotCommand(command: "status", description: "Show connection status in screenshot format. Switches active window to Rust if necessary"),
                    new BotCommand(command: "disconnect", description: "Disconnect from the server"),
                    new BotCommand(command: "launch", description: "Launch rust"),
                    new BotCommand(command: "quit", description: "Quit rust"),
                    new BotCommand(command: "add", description: "Add player to track list"),
                    new BotCommand(command: "remove", description: "Remove player from track list"),
                    new BotCommand(command: "list", description: "Show track list"),
                    new BotCommand(command: "clear", description: "Clear track list")
            };

            var monitorPlayers = new MonitorTrackedPlayers(this, _cancellation);
            _ = monitorPlayers.MonitorTrackedPlayersAsync();
        }

        public async Task InitAsync()
        {
            _telegramClient = new TelegramBotClient(JSONConfig.TokenBot);

            await DiscardOldUpdatesAsync();

            _telegramClient.StartReceiving(errorHandler: HandleErrorAsync,
                               updateHandler: HandleUpdateAsync,
                               cancellationToken: _cancellation.Token,
                               receiverOptions: new Telegram.Bot.Polling.ReceiverOptions() { AllowedUpdates = Array.Empty<UpdateType>() });

            var currentCommands = await _telegramClient.GetMyCommands();
            var currentName = await _telegramClient.GetMyName();
            var shortDescription = await _telegramClient.GetMyShortDescription();

            if (!CommandsAreEqual(currentCommands, _commands) ||
                currentName.Name != _botName ||
                shortDescription != _shortDescription)
            {
                await _telegramClient.SetMyCommands(_commands);
                await _telegramClient.SetMyName(_botName);
                await _telegramClient.SetMyShortDescription(_shortDescription);
            }

            var me = _telegramClient.GetMe();

            await SendMessageAsync(Messages.ProgramRunning);
            await HandleUserMessageAsync("/start");
        }

        public async Task ShutdownAsync()
        {
            await SendMessageAsync(Messages.ProgramShutdown);
            _cancellation.Cancel();
            await Task.Delay(500);
        }

        private async Task DiscardOldUpdatesAsync()
        {
            var oldUpdates = await _telegramClient.GetUpdates();

            if (oldUpdates.Length > 0)
            {
                var lastUpdateId = oldUpdates.Last().Id + 1;
                await _telegramClient.GetUpdates(offset: lastUpdateId);
            }
        }

        private bool CommandsAreEqual(BotCommand[] a, BotCommand[] b)
        {
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
                if (a[i].Command != b[i].Command || a[i].Description != b[i].Description)
                    return false;

            return true;
        }

        private Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var message = update?.Message?.Text;

            if (update.Type == UpdateType.CallbackQuery && update?.CallbackQuery != null)
            {
                await HandleCallbackQueryAsync(client, update);
                return;
            }

            if (await ProcessStateAsync(message))
                return;

            await HandleUserMessageAsync(message!);
        }

        private async Task<bool> ProcessStateAsync(string message)
        {
            if (message == null || IsMessageOneOfBasicCommands(message))
                return false;

            switch (_status)
            {
                case Status.WAITING_FOR_SERVER_ID_INFO:
                    {
                        while (!Validators.IsIDValid(message))
                        {
                            await SendMessageAsync(Messages.InvalidID);
                            await SendMessageAsync(Messages.EnterServerId);
                            return true;
                        }

                        _serverId = message;
                        await SendServerInfoAsync();
                        return true;
                    }

                case Status.WAITING_FOR_SERVER_ID_CONNECT:
                    {
                        while (!Validators.IsIDValid(message))
                        {
                            await SendMessageAsync(Messages.InvalidID);
                            await SendMessageAsync(Messages.EnterServerId);
                            return true;
                        }

                        _status = Status.NONE;
                        _serverId = message;

                        var rustService = new RustService(this, _cancellation);
                        _ = rustService.ConnectToServerAsync(_serverId);
                        return true;
                    }
                case Status.WAITING_FOR_PLAYER_ID:
                    {
                        while (!Validators.IsIDValid(message))
                        {
                            await SendMessageAsync(Messages.InvalidID);
                            await SendMessageAsync(Messages.EnterPlayerId);
                            return true;
                        }

                        _playerId = message;
                        await SendPlayerInfoAsync();
                        return true;
                    }
                case Status.WAITING_FOR_PLAYER_ID_TRACK:
                    {
                        while (!Validators.IsIDValid(message))
                        {
                            await SendMessageAsync(Messages.InvalidID);
                            await SendMessageAsync(Messages.EnterPlayerId);
                            return true;
                        }

                        _playerId = message;
                        await AddTrackedPlayerAsync();
                        return true;
                    }

                case Status.WAITING_FOR_SERVER_ID_AUTOCONNECT:
                    {
                        while (!Validators.IsIDValid(message))
                        {
                            await SendMessageAsync(Messages.InvalidID);
                            await SendMessageAsync(Messages.EnterServerId);
                            return true;
                        }

                        _serverId = message;
                        var monitorStatus = new MonitorServerStatus(this, _cancellation);
                        _ = monitorStatus.MonitorServerStatusAsync(_serverId);
                        return true;
                    }

                case Status.FAVORITE_PLAYER:
                    {
                        if (message.ToLower() == Constants.NONE)
                            message = await PlayerHandler.GetName(await PlayerHandler.GetJson(_playerId));

                        await JSONConfigHandler.AddFavoritePlayerAsync(_playerId, message);
                        await SendMessageAsync(Messages.PlayerAddedToFavorites(message, _playerId));

                        _keyboardFactory.InitKeyboards();
                        _status = Status.NONE;
                        return true;
                    }
                case Status.FAVORITE_SERVER:
                    {
                        if (message.ToLower() == Constants.NONE)
                            message = await ServerHandler.GetName(await ServerHandler.GetJson(_serverId));

                        await JSONConfigHandler.AddFavoriteServerAsync(_serverId, message);
                        await SendMessageAsync(Messages.ServerAddedToFavorites(message, _serverId));

                        _keyboardFactory.InitKeyboards();
                        _status = Status.NONE;
                        return true;
                    }

                default:
                    return false;
            }
        }

        private async Task HandleCallbackQueryAsync(ITelegramBotClient client, Update update)
        {
            var callbackData = update.CallbackQuery.Data;

            try
            {
                await client.AnswerCallbackQuery(update.CallbackQuery.Id, Messages.Applied);
            }
            catch { }

            if (await ProcessCallbackOtherDataAsync(callbackData))
                return;

            await ProcessCallbackMainDataAsync(callbackData, update.CallbackQuery);
        }

        private async Task<bool> ProcessCallbackOtherDataAsync(string callbackData)
        {
            switch (callbackData)
            {
                case "user_server_id":
                    await SendMessageAsync(Messages.EnterServerId);
                    return true;

                case "user_player_id":
                    await SendMessageAsync(Messages.EnterPlayerId);
                    return true;

                case "favorite_player":
                    _status = Status.FAVORITE_PLAYER;
                    await SendMessageAsync(Messages.EnterPlayerIdentifier);
                    return true;

                case "favorite_server":
                    _status = Status.FAVORITE_SERVER;
                    await SendMessageAsync(Messages.EnterServerIdentifier);
                    return true;

                case "favorite_server_remove":
                    var serverName = JSONConfig.FavoriteServers.FirstOrDefault(s => s.Id == _serverId)?.Name;

                    await JSONConfigHandler.RemoveFavoriteServerAsync(_serverId);
                    await SendMessageAsync(Messages.ServerRemovedFromFavorites(_serverId, serverName));
                    _keyboardFactory.InitKeyboards();
                    return true;

                case "favorite_player_remove":
                    var playerName = JSONConfig.FavoritePlayers.FirstOrDefault(p => p.Id == _playerId)?.Name;

                    await JSONConfigHandler.RemoveFavoritePlayerAsync(_playerId);
                    await SendMessageAsync(Messages.PlayerRemovedFromFavorites(_playerId, playerName));
                    _keyboardFactory.InitKeyboards();
                    return true;

                default:
                    return false;
            }
        }

        private async Task ProcessCallbackMainDataAsync(string callbackData, CallbackQuery cq)
        {
            var splittedCallbackData = callbackData.Split('@');
            var prefix = splittedCallbackData[0];

            switch (prefix)
            {
                case Constants.PrefixPlayersInfo:
                    _playerId = splittedCallbackData[1];
                    await SendPlayerInfoAsync();
                    break;

                case Constants.PrefixServersInfo:
                    _serverId = splittedCallbackData[1];
                    await SendServerInfoAsync();
                    break;

                case Constants.PrefixConnects:
                    _status = Status.NONE;
                    _serverId = splittedCallbackData[1];
                    var rustService = new RustService(this, _cancellation);
                    _ = rustService.ConnectToServerAsync(_serverId);
                    break;

                case Constants.PrefixAutoConnects:
                    _status = Status.NONE;
                    _serverId = splittedCallbackData[1];
                    var monitorStatus = new MonitorServerStatus(this, _cancellation);
                    _ = monitorStatus.MonitorServerStatusAsync(_serverId);
                    break;

                case Constants.PrefixTracking:
                    _playerId = splittedCallbackData[1];
                    await AddTrackedPlayerAsync();
                    break;

                case Constants.PrefixTrackingRemove:
                    _playerId = splittedCallbackData[1];
                    await RemoveTrackedPlayerAsync();
                    break;

                case Constants.PrefixConnectNow:
                    _serverId = splittedCallbackData[1];
                    var rustServiceNow = new RustService(this, _cancellation);
                    _ = rustServiceNow.ConnectRightNowAsync(_serverId);
                    break;

                case Constants.PrefixConnectQueue:
                    _serverId = splittedCallbackData[1];
                    var rustServiceQueue = new RustService(this, _cancellation);
                    await rustServiceQueue.ConnectAfterQueueAsync(_serverId);
                    break;

                case Constants.PrefixConnectTimer:
                    _serverId = splittedCallbackData[1];
                    var rustServiceTimer = new RustService(this, _cancellation);
                    _ = rustServiceTimer.ConnectAfterTimerAsync(_serverId);
                    break;

                case Constants.PrefixPlayers:
                    await HandleUserMessageAsync("/players");
                    break;

                case Constants.PrefixServers:
                    await HandleUserMessageAsync("/servers");
                    break;

                case Constants.PrefixLaunch:
                    await HandleUserMessageAsync("/launch");
                    break;

                case Constants.PrefixQuit:
                    await HandleUserMessageAsync("/quit");
                    break;

                case Constants.PrefixConnect:
                    await HandleUserMessageAsync("/connect");
                    break;

                case Constants.PrefixAutoConnect:
                    await HandleUserMessageAsync("/autoconnect");
                    break;

                case Constants.PrefixDisconnect:
                    await HandleUserMessageAsync("/disconnect");
                    break;

                case Constants.PrefixStatus:
                    await HandleUserMessageAsync("/status");
                    break;

                case Constants.PrefixAdd:
                    await HandleUserMessageAsync("/add");
                    break;

                case Constants.PrefixRemove:
                    await HandleUserMessageAsync("/remove");
                    break;

                case Constants.PrefixList:
                    await HandleUserMessageAsync("/list");
                    break;

                case Constants.PrefixClear:
                    await HandleUserMessageAsync("/clear");
                    break;

                case Constants.PrefixSettings:
                    await _telegramClient.EditMessageCaption(
                        chatId: JSONConfig.ChatID,
                        parseMode: ParseMode.Html,
                        caption: await Messages.BuildSettingsCaption(),
                        messageId: cq.Message.Id,
                        replyMarkup: KeyboardFactory.BuildSettings(),
                        cancellationToken: _cancellation.Token);

                    break;

                case Constants.PrefixBackSettings:
                    await _telegramClient.EditMessageCaption(
                        chatId: JSONConfig.ChatID,
                        caption: _startMessage,
                        parseMode: ParseMode.Html,
                        replyMarkup: _keyboardFactory.Start,
                        messageId: cq.Message.Id,
                        cancellationToken: _cancellation.Token);

                    break;

                case Constants.PrefixUpdatePNH:
                    JSONConfig.GetListOfPlayerNames = !JSONConfig.GetListOfPlayerNames;
                    await JSONConfigHandler.UpdateConfig();

                    await _telegramClient.EditMessageReplyMarkup(
                      chatId: JSONConfig.ChatID,
                      replyMarkup: KeyboardFactory.BuildSettings(),
                      messageId: cq.Message.Id,
                      cancellationToken: _cancellation.Token);
                    break;

                case Constants.PrefixUpdateGSD:
                    JSONConfig.GetServerDescription = !JSONConfig.GetServerDescription;
                    await JSONConfigHandler.UpdateConfig();

                    await _telegramClient.EditMessageReplyMarkup(
                      chatId: JSONConfig.ChatID,
                      replyMarkup: KeyboardFactory.BuildSettings(),
                      messageId: cq.Message.Id,
                      cancellationToken: _cancellation.Token);
                    break;

                case Constants.PrefixUpdatePSH:
                    JSONConfig.GetListOfPlayerServers = !JSONConfig.GetListOfPlayerServers;
                    await JSONConfigHandler.UpdateConfig();

                    await _telegramClient.EditMessageReplyMarkup(
                      chatId: JSONConfig.ChatID,
                      replyMarkup: KeyboardFactory.BuildSettings(),
                      messageId: cq.Message.Id,
                      cancellationToken: _cancellation.Token);
                    break;

                case Constants.PrefixUpdateSWJ:
                    JSONConfig.SendScreenshotWhenJoined = !JSONConfig.SendScreenshotWhenJoined;
                    await JSONConfigHandler.UpdateConfig();

                    await _telegramClient.EditMessageReplyMarkup(
                      chatId: JSONConfig.ChatID,
                      replyMarkup: KeyboardFactory.BuildSettings(),
                      messageId: cq.Message.Id,
                      cancellationToken: _cancellation.Token);
                    break;
            }
        }

        private async Task HandleUserMessageAsync(string message)
        {
            switch (message)
            {
                case "/start":
                    await _telegramClient.SendPhoto(
                       chatId: JSONConfig.ChatID,
                       photo: InputFile.FromUri(Constants.ProjectLogo),
                       caption: _startMessage,
                       parseMode: ParseMode.Html,
                       replyMarkup: _keyboardFactory.Start,
                       cancellationToken: _cancellation.Token);
                    break;

                case "/servers":
                    _status = Status.WAITING_FOR_SERVER_ID_INFO;
                    await SendMessageAsync(Messages.ServerData, _keyboardFactory.Servers);
                    break;

                case "/connect":
                    _status = Status.WAITING_FOR_SERVER_ID_CONNECT;
                    await SendMessageAsync(Messages.ConnectToServer, _keyboardFactory.Connects);
                    break;

                case "/players":
                    _status = Status.WAITING_FOR_PLAYER_ID;
                    await SendMessageAsync(Messages.PlayerData, _keyboardFactory.Players);
                    break;

                case "/add":
                    _status = Status.WAITING_FOR_PLAYER_ID_TRACK;
                    await SendMessageAsync(Messages.Track, _keyboardFactory.Tracking);
                    break;

                case "/remove":
                    await SendTrackListAsync();

                    if (JSONConfig.TrackedPlayers.Count == 0)
                        break;

                    await SendMessageAsync(Messages.RemoveFromTracking, _keyboardFactory.TrackingRemove);
                    break;

                case "/autoconnect":
                    _status = Status.WAITING_FOR_SERVER_ID_AUTOCONNECT;
                    await SendMessageAsync(Messages.ConnectToServer, _keyboardFactory.AutoConnects);
                    break;

                case "/list":
                    await SendTrackListAsync();
                    break;

                case "/clear":
                    await ClearTrackedPlayersAsync();
                    break;

                case "/disconnect":
                    var rustLauncherDisconnect = new RustService(this, _cancellation);
                    await rustLauncherDisconnect.DisconnectAsync();
                    break;

                case "/quit":
                    var rustLauncherQuit = new RustService(this, _cancellation);
                    await rustLauncherQuit.QuitRustAsync();
                    break;

                case "/launch":
                    var rustLauncherLaunch = new RustService(this, _cancellation);
                    await rustLauncherLaunch.LaunchRustAsync();
                    break;

                case "/status":
                    await GetConnectionStatus();
                    break;
            }
        }

        private async Task AddTrackedPlayerAsync()
        {
            if (JSONConfig.TrackedPlayers.Count >= Constants.MaxTrackedPlayers)
            {
                await SendMessageAsync(Messages.MaxTrackedReached);
                return;
            }

            var existedPlayer = JSONConfig.TrackedPlayers.FirstOrDefault(p => p.Id == _playerId);

            if (await JSONConfigHandler.IsPlayerTrackedAsync(existedPlayer))
            {
                await SendMessageAsync(Messages.PlayerAlreadyTracked(_playerId));
                return;
            }

            var json = await PlayerHandler.GetJson(_playerId, "server");
            var trackedPlayer = new TrackedPlayer
            {
                Id = _playerId,
                Name = await PlayerHandler.GetName(json),
                CurrentServer = await PlayerHandler.GetCurrentServer(json)
            };

            await JSONConfigHandler.AddTrackedPlayerAsync(trackedPlayer);
            _keyboardFactory.InitKeyboards();

            await SendMessageAsync(Messages.PlayerAddedToTrack(trackedPlayer.Name, trackedPlayer.Id));

            if (trackedPlayer.CurrentServer == Constants.NotPlaying || trackedPlayer.CurrentServer == Constants.NA)
                await SendMessageAsync(Messages.PlayerNowOffline(trackedPlayer.Name, trackedPlayer.Id));
            else
                await SendMessageAsync(Messages.PlayerNowPlaying(trackedPlayer.Name, trackedPlayer.Id, trackedPlayer.CurrentServer));

            _status = Status.NONE;
        }

        private async Task RemoveTrackedPlayerAsync()
        {
            if (JSONConfig.TrackedPlayers.Count == 0)
            {
                await SendMessageAsync(Messages.NoTrackedPlayers);
                return;
            }

            var player = JSONConfig.TrackedPlayers.FirstOrDefault(p => p.Id == _playerId);

            await JSONConfigHandler.RemoveTrackedPlayerAsync(player);
            await SendMessageAsync(Messages.PlayerRemovedFromTracking(player.Name, player.Id));
        }

        private async Task ClearTrackedPlayersAsync()
        {
            if (JSONConfig.TrackedPlayers.Count <= 0)
            {
                await SendMessageAsync(Messages.NoTrackedPlayers);
                return;
            }

            foreach (var tp in JSONConfig.TrackedPlayers)
                await JSONConfigHandler.RemoveTrackedPlayerAsync(tp);

            await SendMessageAsync(Messages.AllTrackedRemoved);
        }

        public async Task GetConnectionStatus(string message = Messages.ConnectionStatus)
        {
            if (!SystemUtils.IsProcessRunning(Constants.RustProcessName))
            {
                await SendMessageAsync(Messages.RustNotLaunched);
                return;
            }

            if (!SystemUtils.CheckActiveWindow(Constants.RustWindowName))
            {
                SystemUtils.SwapActiveWindow(Constants.RustProcessName);
                await Task.Delay(Constants.ShortDelayMs);
            }

            await SendScreenshotAsync(message);
        }

        private async Task SendPlayerInfoAsync()
        {
            _status = Status.NONE;

            var warningMessage = Builders.BuildPlayerWarningMessage();
            await SendMessageAsync(warningMessage);

            var json = await PlayerHandler.GetJson(_playerId, "server");
            if (json is null)
            {
                await SendMessageAsync(Messages.HttpRequestError);
                return;
            }

            var error = await PlayerHandler.RateLimitError(json);
            if (error.Item1)
            {
                await SendMessageAsync("🚫 " + error.Item2);
                return;
            }

            if (JSONConfig.GetListOfPlayerServers)
                await SendServersListAsync(json);

            if (JSONConfig.GetListOfPlayerNames)
                await SendNamesListAsync();

            var caption = await PlayerHandler.GetPlayerFullInformation(json);
            var isFavorited = JSONConfigHandler.IsPlayerAlreadyFavorited(_playerId);

            await SendMessageAsync(caption, isFavorited ? _keyboardFactory.FavoritePlayerRemove : _keyboardFactory.FavoritePlayerAdd);
        }

        private async Task SendServersListAsync(JsonDocument doc)
        {
            var listOfServers = await PlayerHandler.GetListOfPlayedServersAndTime(doc);
            var fileWatermark = Builders.BuildAuthorFileWatermark();

            var fileName = Builders.BuildPlayerServersFileName(await PlayerHandler.GetName(doc));
            var path = Builders.BuildPlayerServersFilePath(fileName);

            await File.WriteAllTextAsync(path, fileWatermark + listOfServers);
            await using var stream = File.OpenRead(path);
            var inputFile = InputFile.FromStream(stream);

            await _telegramClient.SendDocument(
                chatId: JSONConfig.ChatID,
                cancellationToken: _cancellation.Token,
                caption: Messages.PlayerServers(await PlayerHandler.GetName(doc)),
                document: inputFile);
        }

        private async Task SendNamesListAsync()
        {
            var newJson = await PlayerHandler.GetJson(_playerId, "identifier");
            var listOfNames = await PlayerHandler.GetListOfNames(newJson);
            var fileWatermark = Builders.BuildAuthorFileWatermark();

            var fileName = Builders.BuildPlayerNamesFileName(await PlayerHandler.GetName(newJson));
            var path = Builders.BuildPlayerNamesFilePath(fileName);

            await File.WriteAllTextAsync(path, fileWatermark + listOfNames);
            await using var stream = File.OpenRead(path);
            var inputFile = InputFile.FromStream(stream);

            await _telegramClient.SendDocument(
                chatId: JSONConfig.ChatID,
                cancellationToken: _cancellation.Token,
                caption: Messages.PlayerHistoryNames(await PlayerHandler.GetName(newJson)),
                document: inputFile);
        }

        private async Task SendServerInfoAsync()
        {
            _status = Status.NONE;

            var warningMessage = Builders.BuildServerWarningMessage();
            await SendMessageAsync(warningMessage);

            var json = await ServerHandler.GetJson(_serverId);
            if (json is null)
            {
                await SendMessageAsync(Messages.HttpRequestError);
                return;
            }

            var error = await ServerHandler.RateLimitError(json);
            if (error.Item1)
            {
                await SendMessageAsync("🚫 " + error.Item2);
                return;
            }

            var mapUrl = await ServerHandler.GetMapUrl(json);
            var caption = await ServerHandler.GetServerFullInformation(json);
            var description = await ServerHandler.GetDescription(json);

            if (description != Constants.NA && JSONConfig.GetServerDescription)
                await SendMessageAsync(description);

            var isFavorited = JSONConfigHandler.IsServerAlreadyFavorited(_serverId);
            var mapSent = false;

            try
            {
                await _telegramClient.SendPhoto(
                    chatId: JSONConfig.ChatID,
                    photo: mapUrl,
                    caption: caption,
                    parseMode: ParseMode.Html,
                    replyMarkup: isFavorited ? _keyboardFactory.FavoriteServerRemove : _keyboardFactory.FavoriteServerAdd,
                    cancellationToken: _cancellation.Token);

                mapSent = true;
            }
            catch { }

            if (!mapSent)
                await SendMessageAsync(caption, isFavorited ? _keyboardFactory.FavoriteServerRemove : _keyboardFactory.FavoriteServerAdd);
        }

        private async Task SendTrackListAsync()
        {
            if (JSONConfig.TrackedPlayers.Count == 0)
            {
                await SendMessageAsync(Messages.NoTrackedPlayers);
                return;
            }

            var trackedList = Messages.TrackedPlayers;

            foreach (var player in JSONConfig.TrackedPlayers)
                trackedList += $"• \"{player.Name}\" ({player.Id}) - {player.CurrentServer}\n";

            await SendMessageAsync(trackedList);
        }

        public async Task SendScreenshotAsync(string caption = "")
        {
            var resolution = SystemUtils.GetScreenResolution();

            using (Bitmap bmp = new Bitmap(width: resolution.width, height: resolution.height))
            {
                using Graphics g = Graphics.FromImage(bmp);
                g.CopyFromScreen(0, 0, 0, 0, bmp.Size);

                var currentScreenshotPath = Builders.BuildScreenshotsFilePath(Builders.BuildScreenshotFileName());
                bmp.Save(currentScreenshotPath, ImageFormat.Png);

                await SendImageAsync(
                    imagePath: currentScreenshotPath,
                    caption: caption);
            }
        }

        public async Task SendMessageAsync(string message, InlineKeyboardMarkup keyboard = null)
        {
            try
            {
                await _telegramClient.SendMessage(
                    chatId: JSONConfig.ChatID,
                    text: message,
                    parseMode: ParseMode.Html,
                    replyMarkup: keyboard,
                    cancellationToken: _cancellation.Token);
            }
            catch { }
        }

        private async Task SendImageAsync(string imagePath, string caption)
        {
            if (File.Exists(imagePath))
            {
                await using var stream = File.OpenRead(imagePath);
                var inputFile = InputFile.FromStream(stream);

                try
                {
                    await _telegramClient.SendDocument(
                        chatId: JSONConfig.ChatID,
                        caption: caption,
                        parseMode: ParseMode.Html,
                        document: inputFile,
                        cancellationToken: _cancellation.Token);
                }
                catch { }
            }
        }

        private bool IsMessageOneOfBasicCommands(string message)
        {
            message = message.ToLower().Replace("/", "");

            foreach (var command in _commands)
                if (command.Command == message)
                    return true;

            return false;
        }
    }
}