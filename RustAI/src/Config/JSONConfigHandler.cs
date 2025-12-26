using System.IO;
using System.Numerics;
using System.Text.Json;
using System.Xml.Linq;

namespace RustAI
{
    internal static class JSONConfigHandler
    {
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        public static async Task UpdateConfig()
        {
            var config= new Data
            {
                TokenBot = JSONConfig.TokenBot,
                ChatID = JSONConfig.ChatID,
                FavoriteServers = JSONConfig.FavoriteServers,
                FavoritePlayers = JSONConfig.FavoritePlayers,
                GetListOfPlayerNames = JSONConfig.GetListOfPlayerNames,
                GetListOfPlayerServers = JSONConfig.GetListOfPlayerServers,
                GetServerDescription = JSONConfig.GetServerDescription,
                RustLaunchDelaySeconds = JSONConfig.RustLaunchDelaySeconds,
                BattlemetricsID = JSONConfig.BattlemetricsID,
                SendScreenshotWhenJoined = JSONConfig.SendScreenshotWhenJoined,
                TrackedPlayers = JSONConfig.TrackedPlayers,
                QueueLimit = JSONConfig.QueueLimit,
                ConnectTimerMinutes = JSONConfig.ConnectTimerMinutes,
                GetServerPlayers = JSONConfig.GetServerPlayers
            };

            await File.WriteAllTextAsync(JSONConfig.PathToConfig, JsonSerializer.Serialize(config, _jsonOptions));
        }

        public static async Task AddFavoritePlayerAsync(string playerId, string name)
        {
            JSONConfig.FavoritePlayers.Add(new FavoritePlayer { Id = playerId, Name = name });
            await UpdateConfig();
        }

        public static async Task RemoveFavoritePlayerAsync(string playerId)
        {
            JSONConfig.FavoritePlayers.RemoveAll(p => p.Id == playerId);
            await UpdateConfig();
        }

        public static async Task AddFavoriteServerAsync(string serverId, string identifier)
        {
            JSONConfig.FavoriteServers.Add(new FavoriteServer { Id = serverId, Name = identifier});
            await UpdateConfig();
        }

        public static async Task RemoveFavoriteServerAsync(string serverId)
        {
            JSONConfig.FavoriteServers.RemoveAll(p => p.Id == serverId);
            await UpdateConfig();
        }

        public static async Task AddTrackedPlayerAsync(TrackedPlayer player)
        {
            JSONConfig.TrackedPlayers.Add(player);
            await UpdateConfig();
        }

        public static async Task RemoveTrackedPlayerAsync(TrackedPlayer player)
        {
            JSONConfig.TrackedPlayers.RemoveAll(p => p.Id == player.Id);
            await UpdateConfig();
        }

        public static async Task UpdateTrackedPlayerAsync(TrackedPlayer oldPlayer, TrackedPlayer newPlayer)
        {
            JSONConfig.TrackedPlayers.RemoveAll(p => oldPlayer.Id == p.Id);
            JSONConfig.TrackedPlayers.Add(newPlayer);
            await UpdateConfig();
        }

        public static async Task<bool> IsPlayerTrackedAsync(TrackedPlayer player)
        {
            return player == null ? false : JSONConfig.TrackedPlayers.Contains(player);
        }

        public static bool IsPlayerAlreadyFavorited(string playerId)
        {
            if (JSONConfig.FavoritePlayers.Any(p => p.Id == playerId))
                return true;

            return false;
        }
        public static bool IsServerAlreadyFavorited(string serverId)
        {
            if (JSONConfig.FavoriteServers.Any(s => s.Id == serverId))
                return true;

            return false;
        }
    }
}
