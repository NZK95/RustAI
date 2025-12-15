using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RustAI
{
    internal class Data
    {
        [JsonPropertyName("TelegramBotToken")]
        public string TokenBot { get; set; }

        [JsonPropertyName("TelegramChatID")]
        public long ChatID { get; set; }

        [JsonPropertyName("RustLaunchDelaySeconds")]
        public int RustLaunchDelaySeconds { get; set; }

        [JsonPropertyName("FavoriteServers")]
        public List<string> FavoriteServers { get; set; }

        [JsonPropertyName("FavoritePlayers")]
        public List<string> FavoritePlayers { get; set; }

        [JsonPropertyName("TrackedPlayers")]
        public List<string> TrackedPlayers { get; set; }

        [JsonPropertyName("GetPlayerNamesHistory")]
        public bool GetListOfPlayerNames { get; set; }

        [JsonPropertyName("GetPlayerServersHistory")]
        public bool GetListOfPlayerServers { get; set; }
    }

    internal static class JSONConfig
    {
        public static string BasePath { get; private set; }
        public static string PathToConfig { get; private set; }
        public static string PathToExportedPlayersNames { get; private set; }
        public static string PathToExportedPlayersServers { get; private set; }
        public static string TokenBot { get;  set; }
        public static long ChatID { get;  set; }
        public static int RustLaunchDelaySeconds { get; set; }
        public static List<string> FavoriteServers { get; set; }
        public static List<string> FavoritePlayers { get; set; }
        public static List<string> TrackedPlayers { get; set; }
        public static  bool GetListOfPlayerNames { get; private set; }
        public static bool GetListOfPlayerServers{ get; private set; }

        static JSONConfig()
        {
            BasePath = AppContext.BaseDirectory;
            PathToConfig = BasePath + @"Data\config.json";
            PathToExportedPlayersNames = BasePath + @"Exported-players-names";
            PathToExportedPlayersServers = BasePath + @"Exported-players-servers";

            DeserializeData();
        }
            
        private static void DeserializeData()
        {
            using var fs = new FileStream(PathToConfig, FileMode.Open, FileAccess.Read);

            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                PropertyNameCaseInsensitive = true
            };

            var deserializedBot = JsonSerializer.Deserialize<Data>(fs, options);

            TokenBot = deserializedBot.TokenBot;
            ChatID = deserializedBot.ChatID;
            FavoriteServers = deserializedBot.FavoriteServers;
            FavoritePlayers = deserializedBot.FavoritePlayers;
            GetListOfPlayerNames = deserializedBot.GetListOfPlayerNames;
            GetListOfPlayerServers = deserializedBot.GetListOfPlayerServers;
            TrackedPlayers = deserializedBot.TrackedPlayers;
            RustLaunchDelaySeconds = deserializedBot.RustLaunchDelaySeconds;
        }
    }
}
