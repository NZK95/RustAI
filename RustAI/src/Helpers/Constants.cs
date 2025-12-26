namespace RustAI
{
    internal static class Constants
    {
        public const int MaxTrackedPlayers = 20;

        public const int ConsoleDelayMs = 100;
        public const int ShortDelayMs = 1000;
        public const int TrackCheckIntervalMs = 30000;
        public const int QueueCheckIntervalMs = 7000;
        public const int ClearMessaggesIntervalMs = 3000;
        public static int RustLaunchDelayMs = (int)TimeSpan.FromSeconds(JSONConfig.RustLaunchDelaySeconds).TotalMilliseconds;
        public static int ConnectTimerDelayMs = (int)TimeSpan.FromMinutes(JSONConfig.ConnectTimerMinutes).TotalMilliseconds;

        public const string RustProcessName = "RustClient";
        public const string RustWindowName = "Rust";
        public const string GamePath = "steam://rungameid/252490";

        public const string ProjectName = "RustAI";
        public const string ProjectLogo = "https://raw.githubusercontent.com/NZK95/RustAI/master/RustAI/assets/images/logo.png";
        public static readonly string ProjectShortDescription = $"RustAI Bot - your personal Rust assistant\n{Builders.BuildRustAIProjectLink()}";
        public static readonly string ProjectStartMessage = $@"<b>What can this bot do?</b>

🤖 Manage Rust smartly.
<b>RustAI Bot</b> is your ultimate Rust companion — built for real-time 
server management and player tracking so you never miss a moment.

⚡ Real-Time Server Monitoring
🎮 Player Stats & Tracking
🔗 Quick Server Connect
📊 Advanced Analytics
⚙️ Smart Notifications

⚠️ If you encounter bugs/unexpected behavior/errors or just need help, please report them here: {Builders.BuildRustAIProjectLink()}/issues";

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
        public const string PrefixPlayers = "Start-Players";
        public const string PrefixServers = "Start-Servers";
        public const string PrefixLaunch = "Start-Launch";
        public const string PrefixQuit = "Start-Quit";
        public const string PrefixConnect = "Start-Connect";
        public const string PrefixAutoConnect = "Start-AutoConnect";
        public const string PrefixDisconnect = "Start-Disconnect";
        public const string PrefixStatus = "Start-Status";
        public const string PrefixAdd = "Start-Add";
        public const string PrefixRemove = "Start-Remove";
        public const string PrefixList = "Start-List";
        public const string PrefixClear = "Start-Clear";
        public const string PrefixSettings = "Start-Settings";
        public const string PrefixBackSettings = "Settings-Back";
        public const string PrefixUpdatePNH = "Settings-UpdatePNH";
        public const string PrefixUpdateGSD = "Settings-UpdateGSD";
        public const string PrefixUpdatePSH = "Settings-UpdatePSH";
        public const string PrefixUpdateSWJ = "Settings-UpdateSWJ";
        public const string PrefixUpdateSP = "Settings-UpdateSP";

        public const string PrefixUpdateRLD = "Settings-UpdateRLD";
        public const string PrefixUpdateQL = "Settings-UpdateQL";
        public const string PrefixUpdateCT = "Settings-UpdateCT";
        public const string PrefixUpdateUID = "Settings-UpdateUID";



        public const string ClientConnectCommandPrefix = "client.connect ";
        public const string ClientDisconnectCommand = "client.disconnect";
    }
}