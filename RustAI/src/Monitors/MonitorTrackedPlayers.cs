namespace RustAI
{
    internal class MonitorTrackedPlayers
    {
        private readonly TelegramBot _bot;
        private readonly CancellationTokenSource _cancellation;

        public MonitorTrackedPlayers(TelegramBot bot, CancellationTokenSource cancellation)
        {
            _bot = bot;
            _cancellation = cancellation;
        }

        public async Task MonitorTrackedPlayersAsync()
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
                    var json = await PlayerHandler.GetJson(player.Id, "server");
                    var currentServer = await PlayerHandler.GetCurrentServer(json);
                    var oldServer = player.CurrentServer;

                    if (currentServer != oldServer)
                    {
                        if (currentServer == Constants.NotPlaying || currentServer == Constants.NA)
                            await _bot.SendMessageAsync(Messages.PlayerLoggedOff(player.Name, player.Id));
                        else
                            await _bot.SendMessageAsync(Messages.PlayerConnected(player.Name, player.Id, currentServer));

                        var updatedPlayer = new TrackedPlayer
                        {
                            Id = player.Id,
                            Name = player.Name,
                            CurrentServer = currentServer
                        };

                        await JSONConfigHandler.UpdateTrackedPlayerAsync(player, updatedPlayer);
                    }
                });

                await Task.WhenAll(tasks);
                await Task.Delay(Constants.TrackCheckIntervalMs, _cancellation.Token);
            }
        }
    }
}
