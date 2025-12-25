namespace RustAI
{
    internal class MonitorServerStatus
    {
        private TelegramBot _bot { get; set; }
        private CancellationTokenSource _cancellation { get; set; }

        public MonitorServerStatus(TelegramBot bot, CancellationTokenSource cts)
        {
            _bot = bot;
            _cancellation = cts;
        }

        public async Task MonitorServerStatusAsync(string serverID)
        {
            while (!_cancellation.IsCancellationRequested)
            {
                var json = await ServerHandler.GetJson(serverID);
                var status = await ServerHandler.GetStatus(json);
                var name = await ServerHandler.GetName(json);

                if (status == "online")
                {
                    await _bot.SendMessageAsync(Messages.ServerOnline(name));
                    var rustService = new RustService(_bot, _cancellation);
                    await rustService.ConnectRightNowAsync(serverID);
                    break;
                }

                await Task.Delay(Constants.ShortDelayMs);
            }
        }

    }
}
