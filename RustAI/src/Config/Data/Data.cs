using System.Text.Json.Serialization;

namespace RustAI
{
    internal class Data
    {
        [JsonPropertyName("TelegramBotToken")]
        public string TokenBot { get; set; }

        [JsonPropertyName("TelegramChatID")]
        public long? ChatID { get; set; }
        [JsonPropertyName("BattlemetricsID")]
        public string BattlemetricsID { get; set; }

        [JsonPropertyName("CurrentVersion")]
        public string CurrentVersion { get; set; }

        [JsonPropertyName("RustLaunchDelaySeconds")]
        public int RustLaunchDelaySeconds { get; set; }

        [JsonPropertyName("QueueLimit")]
        public int QueueLimit { get; set; }
        [JsonPropertyName("ConnectTimerMinutes")]
        public double ConnectTimerMinutes { get; set; }

        [JsonPropertyName("FavoriteServers")]
        public List<FavoriteServer> FavoriteServers { get; set; }

        [JsonPropertyName("FavoritePlayers")]
        public List<FavoritePlayer> FavoritePlayers { get; set; }

        [JsonPropertyName("TrackedPlayers")]
        public List<TrackedPlayer> TrackedPlayers { get; set; }

        [JsonPropertyName("GetPlayerNamesHistory")]
        public bool GetListOfPlayerNames { get; set; }

        [JsonPropertyName("GetPlayerServersHistory")]
        public bool GetListOfPlayerServers { get; set; }

        [JsonPropertyName("GetServerDescription")]
        public bool GetServerDescription { get; set; }

        [JsonPropertyName("GetServerPlayers")]
        public bool GetServerPlayers { get; set; }

        [JsonPropertyName("SendScreenshotWhenJoined")]
        public bool SendScreenshotWhenJoined { get; set; }
    }
}
