using System.IO;
using System.Text.Json;

namespace RustAI
{
    internal static class JSONConfig
    {
        public static string BasePath { get; private set; }
        public static string PathToConfig { get; private set; }
        public static string PathToScreenshots { get; private set; }
        public static string PathToExportedPlayersNames { get; private set; }
        public static string PathToExportedPlayersServers { get; private set; }
        public static string PathToExportedServerPlayers { get; private set; }
        public static string TokenBot { get; set; }
        public static long? ChatID { get; set; }
        public static string BattlemetricsID { get; set; }
        public static int RustLaunchDelaySeconds { get; set; }
        public static int QueueLimit { get; set; }
        public static double ConnectTimerMinutes { get; set; }
        public static List<FavoriteServer> FavoriteServers { get; set; }
        public static List<FavoritePlayer> FavoritePlayers { get; set; }
        public static List<TrackedPlayer> TrackedPlayers { get; set; }
        public static bool GetListOfPlayerNames { get; set; }
        public static bool GetServerDescription { get; set; }
        public static bool GetServerPlayers { get; set; }
        public static bool GetListOfPlayerServers { get; set; }
        public static bool SendScreenshotWhenJoined { get; set; }

        static JSONConfig()
        {
            BasePath = AppContext.BaseDirectory;
            PathToConfig = BasePath + @"data\config.json";
            PathToExportedPlayersNames = BasePath + @"exports\player-names";
            PathToExportedPlayersServers = BasePath + @"exports\player-servers";
            PathToExportedServerPlayers = BasePath + @"exports\server-players";
            PathToScreenshots = BasePath + @"screenshots";

            DeserializeData();
        }

        public static void DeserializeData()
        {
            using var fs = new FileStream(PathToConfig, FileMode.Open, FileAccess.Read);

            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                PropertyNameCaseInsensitive = true
            };

            var deserializedBot = JsonSerializer.Deserialize<Data>(fs, options);

            if (deserializedBot.ChatID == 0)
                ChatID = null;
            else
                ChatID = deserializedBot.ChatID;

            TokenBot = deserializedBot.TokenBot;
            FavoriteServers = deserializedBot.FavoriteServers;
            FavoritePlayers = deserializedBot.FavoritePlayers;
            GetListOfPlayerNames = deserializedBot.GetListOfPlayerNames;
            GetListOfPlayerServers = deserializedBot.GetListOfPlayerServers;
            GetServerDescription = deserializedBot.GetServerDescription;
            GetServerPlayers = deserializedBot.GetServerPlayers;
            RustLaunchDelaySeconds = deserializedBot.RustLaunchDelaySeconds;
            BattlemetricsID = deserializedBot.BattlemetricsID;
            SendScreenshotWhenJoined = deserializedBot.SendScreenshotWhenJoined;
            TrackedPlayers = deserializedBot.TrackedPlayers;
            FavoritePlayers = deserializedBot.FavoritePlayers;
            QueueLimit = deserializedBot.QueueLimit;
            ConnectTimerMinutes = deserializedBot.ConnectTimerMinutes;
        }
    }
}
