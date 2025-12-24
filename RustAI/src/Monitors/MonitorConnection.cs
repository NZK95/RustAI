namespace RustAI
{
    internal class MonitorConnection
    {
        private readonly TelegramBot _bot;
        private readonly CancellationTokenSource _cancellation;
        private readonly string _serverID;

        public MonitorConnection(TelegramBot bot, string serverID, CancellationTokenSource cancellation)
        {
            _bot = bot;
            _serverID = serverID;
            _cancellation = cancellation;
        }

        public async Task MonitorConnectionAsync()
        {
            while (!_cancellation.IsCancellationRequested)
            {
                var json = await ServerHandler.GetJson(_serverID, "session");
                var players = ServerHandler.GetPlayers(json);
                var isUserEntered = PlayerHandler.IsUserEntered(players, JSONConfig.BattlemetricsID);

                if (players.Count == 0 || players == null)
                {
                    await Task.Delay(Constants.ShortDelayMs, _cancellation.Token);
                    continue;
                }

                if (isUserEntered)
                {
                    if (JSONConfig.SendScreenshotWhenJoined)
                        await _bot.GetConnectionStatus(Messages.ConnectedToServer);
                    else
                        await _bot.SendMessageAsync(Messages.ConnectedToServer);

                    break;
                }

                await Task.Delay(Constants.TrackCheckIntervalMs, _cancellation.Token);
            }
        }
    }
}
