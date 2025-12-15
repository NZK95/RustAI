using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WindowsInput;
using WindowsInput.Native;

namespace RustAI
{
    public class TelegramBot
    {
        private Status _status { get; set; }
        private string _serverId { get; set; }
        private string _playerId { get; set; }
        private TelegramBotClient _telegramClient { get; set; }
        private CancellationTokenSource _cancellation { get; set; }
        private KeyboardFactory _keyboardFactory { get; set; }
        private readonly BotCommand[] _commands;

        private readonly string _botName;

        public TelegramBot()
        {
            _botName = Constants.ProjectName;
            _status = Status.NONE;
            _cancellation = new CancellationTokenSource();
            _keyboardFactory = new KeyboardFactory();

            _commands = new[]
            {
                    new BotCommand(command: "about", description: "Show author and project information."),
                    new BotCommand(command: "players", description: "Show player information."),
                    new BotCommand(command: "servers", description: "Show server information."),
                    new BotCommand(command: "trackadd", description: "Add player to track list."),
                    new BotCommand(command: "trackremove", description: "Remove player from track list."),
                    new BotCommand(command: "tracklist", description: "Show track list."),
                    new BotCommand(command: "trackclear", description: "Clear track list."),
                    new BotCommand(command: "connect", description: "Connect to server."),
                    new BotCommand(command: "launch", description: "Launch rust."),
                    new BotCommand(command: "quit", description: "Quit rust."),

            };

            _ = MonitorTrackedPlayersAsync();
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

            if (!CommandsAreEqual(currentCommands, _commands) || currentName.Name != _botName)
            {
                await _telegramClient.SetMyCommands(_commands);
                await _telegramClient.SetMyName(_botName);
            }

            var me = _telegramClient.GetMe();

            await SendMessageAsync(Messages.ProgramRunning);
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
            if (message == null)
                return false;

            switch (_status)
            {
                case Status.WAITING_FOR_SERVER_ID_INFO:
                    {
                        _serverId = message;
                        await SendServerInfoAsync();
                        return true;
                    }

                case Status.WAITING_FOR_SERVER_ID_CONNECT:
                    {
                        _serverId = message;
                        await ConnectToServerAsync();
                        return true;
                    }
                case Status.WAITING_FOR_PLAYER_ID:
                    {
                        _playerId = message;
                        await SendPlayersInfoAsync();
                        return true;
                    }
                case Status.WAITING_FOR_PLAYER_ID_TRACK:
                    {
                        _playerId = message;
                        await AddTrackedPlayerAsync();
                        return true;
                    }
                case Status.FAVORITE_PLAYER:
                    {
                        _status = Status.NONE;
                        await JSONConfigHandler.AddFavoritePlayerAsync(_playerId, message);
                        await SendMessageAsync(Messages.PlayerAddedToFavorites(message, _playerId));
                        _keyboardFactory.InitKeyboards();
                        return true;
                    }
                case Status.FAVORITE_SERVER:
                    {
                        _status = Status.NONE;
                        await JSONConfigHandler.AddFavoriteServerAsync(_serverId, message);
                        await SendMessageAsync(Messages.ServerAddedToFavorites(message, _serverId));
                        _keyboardFactory.InitKeyboards();
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

            await ProcessCallbackMainDataAsync(callbackData);
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

                default:
                    return false;
            }
        }

        private async Task ProcessCallbackMainDataAsync(string callbackData)
        {
            var splittedCallbackData = callbackData.Split('@');
            var prefix = splittedCallbackData[0];

            switch (prefix)
            {
                case Constants.PrefixPlayersInfo:
                    _playerId = splittedCallbackData[1];
                    await SendPlayersInfoAsync();
                    break;

                case Constants.PrefixServersInfo:
                    _serverId = splittedCallbackData[1];
                    await SendServerInfoAsync();
                    break;

                case Constants.PrefixConnects:
                    _serverId = splittedCallbackData[1];
                    await ConnectToServerAsync();
                    break;

                case Constants.PrefixTracking:
                    _playerId = splittedCallbackData[1];
                    await AddTrackedPlayerAsync();
                    break;

                case Constants.PrefixTrackingRemove:
                    _playerId = splittedCallbackData[1];
                    await RemoveTrackedPlayerAsync();
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

            if (await Utils.IsPlayerTrackedAsync(_playerId))
            {
                await SendMessageAsync(Messages.PlayerAlreadyTracked(_playerId));
                return;
            }

            var json = await PlayerHandler.GetBattlemetricsJson(_playerId, "server");
            var name = await PlayerHandler.GetName(json);
            var server = await PlayerHandler.GetCurrentServer(json);

            await JSONConfigHandler.AddTrackedPlayerAsync(_playerId, name, server);
            _keyboardFactory.InitKeyboards();

            await SendMessageAsync(Messages.PlayerAddedToTrack(name, _playerId));

            if (server == Constants.NotPlaying || server == Constants.NA)
                await SendMessageAsync(Messages.PlayerNowOffline(name, _playerId));
            else
                await SendMessageAsync(Messages.PlayerNowPlaying(name, _playerId, server));

            _status = Status.NONE;
        }

        private async Task RemoveTrackedPlayerAsync()
        {
            if (JSONConfig.TrackedPlayers.Count == 0)
            {
                await SendMessageAsync(Messages.NoTrackedPlayers);
                return;
            }

            var name = JSONConfig.TrackedPlayers
                .FirstOrDefault(p => p.StartsWith($"{_playerId} |"))?
                .Split('|')[1]
                .Trim() ?? Constants.Unknown;

            await JSONConfigHandler.RemoveTrackedPlayerAsync(_playerId);
            await SendMessageAsync(Messages.PlayerRemovedFromTracking(name, _playerId));
        }

        private async Task SendPlayersInfoAsync()
        {
            var warningMessage = Utils.BuildPlayerInfoWarningMessage();
            await SendMessageAsync(warningMessage);
            await SendMessageAsync(Messages.PreparingData);

            var json = await PlayerHandler.GetBattlemetricsJson(_playerId, "server");
            if (json is null)
            {
                _status = Status.NONE;
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
                await SendListOfServersAsync(json);

            if (JSONConfig.GetListOfPlayerNames)
                await SendListOfNamesAsync();

            _status = Status.NONE;

            var caption = await PlayerHandler.GetPlayerFullInformation(json);

            if (Utils.IsPlayerAlreadyFavorited(_playerId))
            {
                await SendMessageAsync(caption);
            }
            else
            {
                await _telegramClient.SendMessage(
                           chatId: JSONConfig.ChatID,
                           text: caption,
                           cancellationToken: _cancellation.Token,
                           replyMarkup: _keyboardFactory.FavoritePlayers);
            }
        }

        private async Task SendListOfServersAsync(JsonDocument doc)
        {
            var listOfServers = await PlayerHandler.GetListOfPlayedServersAndTime(doc);
            var fileWatermark = Utils.BuildAuthorFileWatermark();

            var fileName = Utils.BuildPlayerServersFileName(await PlayerHandler.GetName(doc));
            var path = JSONConfig.PathToExportedPlayersServers + $"\\{fileName}";

            await File.WriteAllTextAsync(path, fileWatermark + listOfServers);
            await using var stream = File.OpenRead(path);
            var inputFile = InputFile.FromStream(stream);

            await _telegramClient.SendDocument
                (chatId: JSONConfig.ChatID,
                cancellationToken: _cancellation.Token,
                caption: Messages.PlayerServers(await PlayerHandler.GetName(doc)),
                document: inputFile);
        }

        private async Task SendListOfNamesAsync()
        {
            var newJson = await PlayerHandler.GetBattlemetricsJson(_playerId, "identifier");
            var listOfNames = await PlayerHandler.GetListOfNames(newJson);
            var fileWatermark = Utils.BuildAuthorFileWatermark();

            var fileName = Utils.BuildPlayerNamesFileName(await PlayerHandler.GetName(newJson));
            var path = JSONConfig.PathToExportedPlayersNames + $"\\{fileName}";

            await File.WriteAllTextAsync(path, fileWatermark + listOfNames);
            await using var stream = File.OpenRead(path);
            var inputFile = InputFile.FromStream(stream);

            await _telegramClient.SendDocument
                (chatId: JSONConfig.ChatID,
                cancellationToken: _cancellation.Token,
                caption: Messages.PlayerHistoryNames(await PlayerHandler.GetName(newJson)),
                document: inputFile);
        }

        private async Task SendServerInfoAsync()
        {
            var warningMessage = Utils.BuildServerfInfoWarningMessage();
            await SendMessageAsync(warningMessage);
            await SendMessageAsync(Messages.PreparingData);

            var json = await ServerHandler.GetJson(_serverId);
            if (json is null)
            {
                _status = Status.NONE;
                await SendMessageAsync(Messages.HttpRequestError);
                return;
            }

            var error = await ServerHandler.RateLimitError(json);
            if (error.Item1)
            {
                await SendMessageAsync("🚫 " + error.Item2);
                return;
            }

            _status = Status.NONE;

            var mapUrl = await ServerHandler.GetMapUrl(json);
            var caption = await ServerHandler.GetServerFullInformation(json);
            var description = await ServerHandler.GetDescription(json);

            if (description != Constants.NA)
                await SendMessageAsync(description);

            bool isFavorited = Utils.IsServerAlreadyFavorited(_serverId);
            bool mapSent = false;

            try
            {
                await _telegramClient.SendPhoto(
                    chatId: JSONConfig.ChatID,
                    photo: mapUrl,
                    caption: caption,
                    replyMarkup: isFavorited ? null : _keyboardFactory.FavoriteServers,
                    cancellationToken: _cancellation.Token);

                mapSent = true;
            }
            catch { }

            if (!mapSent)
            {
                await SendMessageAsync(Messages.MapNotFound);

                await _telegramClient.SendMessage(
                    chatId: JSONConfig.ChatID,
                    text: caption,
                    replyMarkup: isFavorited ? null : _keyboardFactory.FavoriteServers,
                    cancellationToken: _cancellation.Token);
            }
        }

        private async Task ConnectToServerAsync()
        {
            var json = await ServerHandler.GetJson(_serverId);
            var warningMessage = await Utils.BuildConnectWarningMessageAsync(json);

            await SendMessageAsync(warningMessage);
            await Task.Delay(Constants.ShortDelayMs);

            var connectToInsert = $"{Constants.ClientConnectCommandPrefix}{await ServerHandler.GetAddress(json)}";

            if (ConnectHandler.IsProcessRunning(Constants.RustProcessName))
            {
                if (!ConnectHandler.CheckActiveWindow(Constants.RustWindowName))
                {
                    await SendMessageAsync(Messages.SwappingToRust);
                    ConnectHandler.SwapWindow(Constants.RustProcessName);
                }
            }
            else
            {
                await SendMessageAsync(Messages.RustNotLaunched);
                await LaunchRustAsync();
                await Task.Delay((int)TimeSpan.FromSeconds(JSONConfig.RustLaunchDelaySeconds).TotalMilliseconds);
            }

            var simulator = new InputSimulator();
            simulator.Keyboard.KeyPress(VirtualKeyCode.F1);

            await Task.Delay(Constants.ShortDelayMs);

            foreach (char c in connectToInsert)
                simulator.Keyboard.TextEntry(c);

            simulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);

            await SendMessageAsync(Messages.Connecting);
            _status = Status.NONE;
        }

        private async Task LaunchRustAsync()
        {
            if (ConnectHandler.IsProcessRunning(Constants.RustProcessName))
                await SendMessageAsync(Messages.RustAlreadyRunning);

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = Utils.GetSteamPath(),
                UseShellExecute = true
            };

            Process.Start(psi);

            await SendMessageAsync(Messages.RustLaunching);
        }

        private async Task QuitRustAsync()
        {
            var processes = Process.GetProcessesByName(Constants.RustProcessName);

            foreach (var process in processes)
            {
                process.Kill();
                process.WaitForExit();
            }

            await SendMessageAsync(Messages.RustQuitting);
        }

        private async Task HandleUserMessageAsync(string message)
        {
            switch (message)
            {
                case "/about":
                    await SendMessageAsync(Utils.BuildAboutMessage());
                    break;

                case "/servers":
                    {
                        _status = Status.WAITING_FOR_SERVER_ID_INFO;

                        await _telegramClient.SendMessage(
                        chatId: JSONConfig.ChatID,
                        text: Messages.ServerData,
                        cancellationToken: _cancellation.Token,
                        replyMarkup: _keyboardFactory.Servers);

                        break;
                    }
                case "/connect":
                    {
                        _status = Status.WAITING_FOR_SERVER_ID_CONNECT;

                        await _telegramClient.SendMessage(
                       chatId: JSONConfig.ChatID,
                       text: Messages.ConnectToServer,
                       cancellationToken: _cancellation.Token,
                       replyMarkup: _keyboardFactory.Connects);

                        break;
                    }
                case "/players":
                    {
                        _status = Status.WAITING_FOR_PLAYER_ID;

                        await _telegramClient.SendMessage(
                        chatId: JSONConfig.ChatID,
                        text: Messages.PlayerData,
                        cancellationToken: _cancellation.Token,
                        replyMarkup: _keyboardFactory.Players);

                        break;
                    }
                case "/trackadd":
                    {
                        _status = Status.WAITING_FOR_PLAYER_ID_TRACK;

                        await _telegramClient.SendMessage(
                      chatId: JSONConfig.ChatID,
                      text: Messages.Track,
                      cancellationToken: _cancellation.Token,
                      replyMarkup: _keyboardFactory.Tracking);

                        break;
                    }

                case "/trackremove":
                    await SendTrackListAsync();

                    if (JSONConfig.TrackedPlayers.Count == 0)
                        break;

                    await _telegramClient.SendMessage(
                  chatId: JSONConfig.ChatID,
                  text: Messages.RemoveFromTracking,
                  cancellationToken: _cancellation.Token,
                  replyMarkup: _keyboardFactory.TrackingRemove);

                    break;

                case "/tracklist":
                    await SendTrackListAsync();
                    break;

                case "/trackclear":

                    foreach (var tp in JSONConfig.TrackedPlayers.ToList())
                    {
                        var playerId = tp.Split('|')[0].Trim();
                        await JSONConfigHandler.RemoveTrackedPlayerAsync(playerId);
                    }

                    await SendMessageAsync(Messages.AllTrackedRemoved);
                    break;

                case "/quit":
                    await QuitRustAsync();
                    break;

                case "/launch":
                    await LaunchRustAsync();
                    break;
            }
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
            {
                var playerId = player.Split('|')[0].Trim();
                var name = player.Split('|')[1].Trim();
                var server = player.Split('|')[2].Trim();

                trackedList += $"• \"{name}\" ({playerId}) - {server}\n";
            }

            await SendMessageAsync(trackedList);
        }

        private async Task MonitorTrackedPlayersAsync()
        {
            while (!_cancellation.IsCancellationRequested)
            {
                if (JSONConfig.TrackedPlayers.Count == 0)
                {
                    await Task.Delay(Constants.ShortDelayMs, _cancellation.Token);
                    continue;
                }

                var tasks = JSONConfig.TrackedPlayers.Select(async player =>
                {
                    var playerId = player.Split('|')[0].Trim();
                    var name = player.Split('|')[1].Trim();
                    var oldServer = player.Split('|')[2].Trim();

                    var json = await PlayerHandler.GetBattlemetricsJson(playerId, "server");
                    var currentServer = await PlayerHandler.GetCurrentServer(json);

                    if (currentServer != oldServer)
                    {
                        if (currentServer == Constants.NotPlaying || currentServer == Constants.NA)
                            await SendMessageAsync(Messages.PlayerLoggedOff(name, playerId));
                        else
                            await SendMessageAsync(Messages.PlayerConnected(name, playerId, currentServer));

                        await JSONConfigHandler.UpdateTrackedPlayerAsync(playerId, name, currentServer);
                    }
                });

                await Task.WhenAll(tasks);
                await Task.Delay(Constants.TrackCheckIntervalMs, _cancellation.Token);
            }
        }

        private async Task SendMessageAsync(string message)
        {
            if (JSONConfig.ChatID != default)
            {
                try
                {
                    await _telegramClient.SendMessage(
                        chatId: JSONConfig.ChatID,
                        text: message,
                        cancellationToken: _cancellation.Token
                        );
                }
                catch (Exception) { }
            }
        }
    }
}