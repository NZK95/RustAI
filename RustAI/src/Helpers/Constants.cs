namespace RustAI
{
    internal static class Constants
    {
        public const int MaxTrackedPlayers = 20;

        public const int ConsoleDelayMs = 100;
        public const int ShortDelayMs = 1000;
        public const int TrackCheckIntervalMs = 30000;
        public const int QueueCheckIntervalMs = 7000;
        public static int RustLaunchDelayMs = (int)TimeSpan.FromSeconds(JSONConfig.RustLaunchDelaySeconds).TotalMilliseconds;
        public static int ConnectTimerDelayMs = (int)TimeSpan.FromMinutes(JSONConfig.ConnectTimerMinutes).TotalMilliseconds;

        public const string RustProcessName = "RustClient";
        public const string RustWindowName = "Rust";
        public const string GamePath = "steam://rungameid/252490";

        public const string ProjectName = "RustAI";
        public static readonly string ProjectShortDescription = $"RustAI Bot - your personal Rust assistant\n{Builders.BuildRustAIProjectLink()}";
        public const string ProjectStartMessage = @"What can this bot do?

🤖 Manage Rust smartly.
RustAI Bot is your ultimate Rust companion — built for real-time 
server management and player tracking so you never miss a moment.

⚡ Real-Time Server Monitoring
🎮 Player Stats & Tracking
🔗 Quick Server Connect
📊 Advanced Analytics
⚙️ Smart Notifications

🌐 GitHub: www.yourwebsite.com
📖 Docs: docs.yoursite.io";

        public const string NotPlaying = "Not playing";
        public const string NA = "N/A";
        public const string NONE = "none";
        public const string Unknown = "Unknown";

        public const string PrefixPlayersInfo = "PlayersInfo";
        public const string PrefixServersInfo = "ServersInfo";
        public const string PrefixConnects = "Connects";
        public const string PrefixAutoConnects = "AutoConnects";
        public const string PrefixTracking = "Tracking";
        public const string PrefixTrackingRemove = "TrackingRemove";
        public const string PrefixConnectNow = "ConnectNow";
        public const string PrefixConnectQueue = "ConnectQueue";
        public const string PrefixConnectTimer = "ConnectTimer";

        public const string ClientConnectCommandPrefix = "client.connect ";
        public const string ClientDisconnectCommand = "client.disconnect";
    }
}