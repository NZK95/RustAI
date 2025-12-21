namespace RustAI
{
    internal static class Constants
    {
        // Limits
        public const int MaxTrackedPlayers = 20;

        // Timings (ms)
        public const int ConsoleDelayMs = 100;
        public const int ShortDelayMs = 1000;
        public const int TrackCheckIntervalMs = 30000;
        public static int RustLaunchDelayMs = (int)TimeSpan.FromSeconds(JSONConfig.RustLaunchDelaySeconds).TotalMilliseconds;
        public static int ConnectTimerDelayMs = (int)TimeSpan.FromMinutes(JSONConfig.ConnectTimerMinutes).TotalMilliseconds;

        // Process / Window names
        public const string RustProcessName = "RustClient";
        public const string RustWindowName = "Rust";
        public const string ProjectName = "RustAI";
        public const string GamePath = "steam://rungameid/252490";

        // Server/player offline markers
        public const string NotPlaying = "Not playing";
        public const string NA = "N/A";
        public const string Unknown = "Unknown";

        // Callback prefixes
        public const string PrefixPlayersInfo = "PlayersInfo";
        public const string PrefixServersInfo = "ServersInfo";
        public const string PrefixConnects = "Connects";
        public const string PrefixTracking = "Tracking";
        public const string PrefixTrackingRemove = "TrackingRemove";

        public const string PrefixConnectNow = "ConnectNow";
        public const string PrefixConnectQueue = "ConnectQueue";
        public const string PrefixConnectTimer = "ConnectTimer";

        // Commands
        public const string ClientConnectCommandPrefix = "client.connect ";
        public const string ClientDisconnectCommand = "disconnect";
    }
}