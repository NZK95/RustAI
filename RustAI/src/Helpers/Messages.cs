using System.Windows.Controls;

namespace RustAI
{
    internal static class Messages
    {
        public const string ProgramRunning = "▶️ The program is running.";
        public const string PreparingData = "✅ Preparing data...";
        public const string HttpRequestError = "🚫 HTTP request error.";
        public const string MapNotFound = "⚠️ Map image is not found!";
        public const string MaxTrackedReached = "🚫 You have reached the maximum number of tracked players (20). Remove some players from the tracking list to add new ones.";
        public const string EnterServerId = "Enter server ID (from Battlemetrics): ";
        public const string EnterPlayerId = "Enter player ID (from Battlemetrics): ";
        public const string EnterPlayerIdentifier = "Enter player identifier: ";
        public const string EnterServerIdentifier = "Enter server identifier: ";
        public const string NoTrackedPlayers = "🚫 There are no tracked players.";
        public const string TrackedPlayers = "📋 Currently tracked players:\n\n";
        public const string AllTrackedRemoved = "✅ All tracked players were removed.";
        public const string Connecting = "✅ Connecting..";
        public const string Applied = "✅ Successfully applied.";
        public const string RustNotLaunched = "ℹ️ Rust is not launched.";
        public const string RustAlreadyRunning = "ℹ️ Rust is already running.";
        public const string RustLaunching = "🚀 Launching Rust...";
        public const string RustQuitting = "❌ Quitted Rust...";
        public const string SwappingToRust = "🔄 Changing active window to RustClient.exe";
        public const string ServerData = "Choose a server to display data about:";
        public const string PlayerData = "Choose a player to display data about:";
        public const string ConnectToServer = "Choose a server to connect:";
        public const string Track = "Choose player to track:";
        public const string RemoveFromTracking = "Choose player to remove from tracking:";

        public static string PlayerAddedToFavorites(string name, string id) => $"✅ Player \"{name}\" ({id}) was added to favorites list.";
        public static string ServerAddedToFavorites(string name, string id) => $"✅ Server \"{name}\" ({id}) was added to favorites list.";
        public static string PlayerAlreadyTracked(string id) => $"🚫 Player {id} is already being tracked.";
        public static string PlayerAddedToTrack(string name, string id) => $"✅ Player \"{name}\" ({id}) was added to tracking list.";
        public static string PlayerNowPlaying(string name, string id, string server) => $"ℹ️ Player \"{name}\" ({id}) currently is playing on {server}.";
        public static string PlayerNowOffline(string name, string id) => $"ℹ️ Player \"{name}\" ({id}) currently is not online.";
        public static string PlayerRemovedFromTracking(string name, string id) => $"✅ Player \"{name}\" ({id}) was removed to tracking list.";
        public static string PlayerLoggedOff(string name, string id) => $"ℹ️ Player \"{name}\" ({id}) has just logged off.";
        public static string PlayerConnected(string name, string id, string server) => $"ℹ️ Player \"{name}\" ({id}) has just connected to {server}.";
        public static string PlayerServers(string name) => $"List of servers where \"{name}\" was spotted.";
        public static string PlayerHistoryNames(string name) => $"List of known names for player \"{name}\".";
    }
}