using System.Diagnostics;
using Telegram.Bot.Types.ReplyMarkups;
using WindowsInput;
using WindowsInput.Native;

namespace RustAI
{
    internal class RustService
    {
        private readonly TelegramBot _bot;
        private readonly CancellationTokenSource _cancellation;

        public RustService(TelegramBot bot, CancellationTokenSource cancellation)
        {
            _bot = bot;
            _cancellation = cancellation;
        }

        public async Task LaunchRustAsync()
        {
            if (SystemUtils.IsProcessRunning(Constants.RustProcessName))
            {
                await _bot.SendMessageAsync(Messages.RustAlreadyRunning);
                return;
            }

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = Constants.GamePath,
                UseShellExecute = true
            };

            Process.Start(psi);
            await _bot.SendMessageAsync(Messages.RustLaunching);
        }

        public async Task QuitRustAsync()
        {
            var processes = Process.GetProcessesByName(Constants.RustProcessName);

            if (processes.Length == 0)
            {
                await _bot.SendMessageAsync(Messages.RustNotLaunched);
                return;
            }

            foreach (var process in processes)
            {
                process.Kill();
                process.WaitForExit();
            }

            await _bot.SendMessageAsync(Messages.RustQuitting);
        }

        public async Task ConnectToServerAsync(string serverID)
        {
            var serverJson = await ServerHandler.GetJson(serverID);
            var playerJson = await PlayerHandler.GetJson(JSONConfig.BattlemetricsID, "server");
            var currentServer = await PlayerHandler.GetCurrentServer(playerJson);

            if (!SystemUtils.IsProcessRunning(Constants.RustProcessName))
            {
                var rustLauncher = new RustService(bot: _bot, cancellation: _cancellation);
                await rustLauncher.LaunchRustAsync();
                await Task.Delay(Constants.RustLaunchDelayMs);
            }

            if (currentServer != Constants.NotPlaying && currentServer != Constants.NA)
            {
                await _bot.SendMessageAsync(Messages.AlreadyConnected);
                return;
            }

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData("Right now", $"{Constants.PrefixConnectNow}@{serverID}"),
                InlineKeyboardButton.WithCallbackData("When queue reaches a number",$"{Constants.PrefixConnectQueue}@{serverID}" ),
                InlineKeyboardButton.WithCallbackData("After a timer", $"{Constants.PrefixConnectTimer}@{serverID}")
            });

            var name = await ServerHandler.GetName(serverJson);
            var playersCount = await ServerHandler.GetPlayersCount(serverJson);
            var queue = await ServerHandler.GetQueuedPlayers(serverJson);

            await _bot.SendMessageAsync(Messages.Connect(name, playersCount, queue), keyboard);
        }

        public async Task ConnectRightNowAsync(string serverID)
        {
            var json = await ServerHandler.GetJson(serverID);
            var connectToInsert = $"{Constants.ClientConnectCommandPrefix}{await ServerHandler.GetAddress(json)}";
            var message = await Builders.BuildConnectWarningMessageAsync(json);

            await _bot.SendMessageAsync(message);

            if (!SystemUtils.CheckActiveWindow(Constants.RustWindowName))
            {
                SystemUtils.SwapActiveWindow(Constants.RustProcessName);
                await Task.Delay(Constants.ShortDelayMs);
            }

            await PasteToConsole(connectToInsert);
            await _bot.SendMessageAsync(Messages.Connecting);

            var monitorConnection = new MonitorConnection(_bot, serverID, _cancellation);
            Task.Run(() => monitorConnection.MonitorConnectionAsync());
        }

        public async Task ConnectAfterQueueAsync(string serverID)
        {
            await _bot.SendMessageAsync(Messages.ConnectAfterQueue());
            var monitorQueue = new MonitorQueue(_bot, _cancellation);
            Task.Run(() => monitorQueue.MonitorQueueAsync(serverID));
        }

        public async Task ConnectAfterTimerAsync(string serverID)
        {
            await _bot.SendMessageAsync(Messages.ConnectAfterTimer());
            await Task.Delay(Constants.ConnectTimerDelayMs);
            Task.Run(() => ConnectRightNowAsync(serverID));
        }

        public async Task DisconnectAsync()
        {
            if (!SystemUtils.IsProcessRunning(Constants.RustProcessName))
            {
                await _bot.SendMessageAsync(Messages.RustNotLaunched);
                return;
            }

            var playerID = JSONConfig.BattlemetricsID;
            var json = await PlayerHandler.GetJson(playerID, "server");
            var server = await PlayerHandler.GetCurrentServer(json);

            if (server == Constants.NotPlaying || server == Constants.NA)
            {
                await _bot.SendMessageAsync(Messages.NotConnected);
                return;
            }

            if (!SystemUtils.CheckActiveWindow(Constants.RustWindowName))
            {
                SystemUtils.SwapActiveWindow(Constants.RustProcessName);
                await Task.Delay(Constants.ShortDelayMs);
            }

            await PasteToConsole(Constants.ClientDisconnectCommand);
            await _bot.SendMessageAsync(Messages.Disconnected);
        }

        private async Task PasteToConsole(string connectToInsert)
        {
            var simulator = new InputSimulator();
            simulator.Keyboard.KeyPress(VirtualKeyCode.F1);

            await Task.Delay(Constants.ConsoleDelayMs);

            foreach (char c in connectToInsert)
                simulator.Keyboard.TextEntry(c);


            simulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
            simulator.Keyboard.KeyPress(VirtualKeyCode.F1);
        }
    }
}
