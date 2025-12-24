namespace RustAI
{
    internal class MonitorQueue
    {
        private TelegramBot _bot { get; set; }
        private CancellationTokenSource _cancellation { get; set; }

        public MonitorQueue(TelegramBot bot, CancellationTokenSource cts)
        {
            _bot = bot;
            _cancellation = cts;
        }

        public async Task MonitorQueueAsync(string serverID)
        {
            while (!_cancellation.IsCancellationRequested)
            {
                var json = await ServerHandler.GetJson(serverID);
                var queueCount = await ServerHandler.GetQueuedPlayers(json);

                if (queueCount >= JSONConfig.QueueLimit)
                {
                    await _bot.SendMessageAsync(Messages.QueueAlarm);
                    var rustService = new RustService(_bot, _cancellation);
                    await rustService.ConnectRightNowAsync(serverID);
                    break;
                }

                await Task.Delay(Constants.QueueCheckIntervalMs, _cancellation.Token);
            }
        }

    }
}
