using System.Windows.Controls;

namespace RustAI
{
    internal static class Messages
    {
        public const string ProgramRunning = "▶️ Program running";
        public const string ProgramShutdown = "❌ Shutting down";
        public const string PreparingData = "<i>Preparing data...</i>";

        public const string HttpRequestError = "🚫 HTTP request error";
        public const string MapNotFound = "⚠️ Map image not found!";
        public const string MaxTrackedReached = "🚫 Tracking limit reached.\nRemove players to add new ones (max 20)";
        public const string NoTrackedPlayers = "🚫 There are no tracked players";
        public const string ConnectionStatusError = $"🚫 Unable to retrieve connection status:\n        You are not playing";
        public const string NotConnected = "🚫 Not connected.";
        public const string InvalidID = "🚫 Invalid ID provided.";

        public const string EnterServerId = "Enter server ID (Battlemetrics)";
        public const string EnterPlayerId = "Enter player ID (Battlemetrics)";
        public const string EnterPlayerIdentifier = "Enter player identifier (or <b>\"None\"</b> to leave default): ";
        public const string EnterServerIdentifier = "Enter server identifier (or <b>\"None\"</b> to leave default): ";

        public const string TrackedPlayers = "<b>Tracked players:</b>\n\n";
        public const string AllTrackedRemoved = "✅ All tracked players were removed";
        public const string Connecting = "✅ Connecting...";
        public const string AlreadyConnected = "🚫 You are already connected to a server.\n        Please disconnect first.";
        public const string Disconnected = "✅ Disconnected";
        public const string ConnectedToServer = "🎮 Connected to the server!\n        Ready to play!";
        public const string Applied = "✅ Successfully applied";
        public const string RustLaunched = "✅ Rust launched";
        public const string RustNotLaunched = "Rust is not launched";
        public const string RustAlreadyRunning = "Rust is already running";
        public const string ConnectionStatus = "Current connection status";
        public const string RustLaunching = "🚀 Launching Rust...";
        public const string RustQuitting = "❌ Quitting Rust...";
        public const string QueueAlarm = "🚨 Queue limit reached!";

        public const string ServerData = "Select a server to view";
        public const string PlayerData = "Select a player to view";
        public const string ConnectToServer = "Select a server to connect";
        public const string Track = "Select a player to track";
        public const string RemoveFromTracking = "Select a player to remove";

        public const string InvalidInput = "❌ Invalid input";
        public const string UpdateRustLaunchDelay = "Enter new Rust launch delay in seconds:";
        public const string UpdatedRustLaunchDelay = "✅ Rust launch delay is successfully updated";
        public const string UpdateQueueLimit = "Enter new queue limit (number of players):";
        public const string UpdatedQueueLimit = "✅ Queue limit is successfully updated";
        public const string UpdateConnectTimer = "Enter new connect timer in minutes:";
        public const string UpdatedConnectTimer = "✅ Connect timer is successfully updated";
        

        public static async Task<string> BuildSettingsCaption()
        {
            return $@"⚙️ <b>Settings RustAI</b>

Here you can change various program settings that are stored in the <b>config.json</b> file, which you can also edit directly in the file.

<i>Use buttons below to modify settings</i>";
        }

        public static string ServerOnline(string name) =>
            $"Server <b>{name}</b> is now online. Connecting...";

        public static string Connect(string name, int playersCount, int queueCount) =>
            $"You selected \"{name}\"\n" +
            $"There are currently {playersCount} players on the server, and {queueCount} in queue\n" +
            $"Choose the moment to connect:\n";
       
        public static string ConnectAfterTimer() =>
                $"⏱️You will be connected after the timer expires in <b>{JSONConfig.ConnectTimerMinutes}</b> minutes.";

        public static string ConnectAfterQueue() =>
                $"👥 You will be connected when the queue reaches <b>{JSONConfig.QueueLimit}</b> users.";

        public static string PlayerAddedToFavorites(string name, string id) =>
            $"✅ Player \"{name}\" ({id}) was added to favorites list";

        public static string ServerAddedToFavorites(string name, string id) =>
            $"✅ Server \"{name}\" ({id}) was added to favorites list";

        public static string ServerRemovedFromFavorites(string name, string id) =>
            $"✅ Server \"{name}\" ({id}) was removed from favorites list";

        public static string PlayerRemovedFromFavorites(string name, string id) =>
            $"✅ Player \"{name}\" ({id}) was removed from favorites list";

        public static string PlayerAlreadyTracked(string id) =>
            $"🚫 Player {id} is already being tracked";

        public static string PlayerAddedToTrack(string name, string id) =>
            $"✅ Player \"{name}\" ({id}) was added to tracking list";

        public static string PlayerRemovedFromTracking(string name, string id) =>
            $"✅ Player \"{name}\" ({id}) was removed from tracking list";

        public static string PlayerNowPlaying(string name, string id, string server) =>
            $"ℹ️ Player \"{name}\" ({id}) currently is playing on {server}";

        public static string PlayerNowOffline(string name, string id) =>
            $"ℹ️ Player \"{name}\" ({id}) currently is not online";

        public static string PlayerLoggedOff(string name, string id) =>
            $"ℹ️ Player \"{name}\" ({id}) has just logged off";

        public static string PlayerConnected(string name, string id, string server) =>
            $"ℹ️ Player \"{name}\" ({id}) has just connected to {server}";

        public static string PlayerServers(string name) =>
            $"List of servers where \"{name}\" was spotted";

        public static string PlayerHistoryNames(string name) =>
            $"List of known names for player \"{name}\"";

        public static string ServerPlayers(string name) =>
            $"List of players currently on \"{name}\"";
    }
}