using System.IO;
using System.Numerics;
using System.Text.Json;
using System.Xml.Linq;

namespace RustAI
{
    internal static class JSONConfigHandler
    {
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        public static async Task AddFavoritePlayerAsync(string playerId, string name)
        {
            var jsonString = await File.ReadAllTextAsync(JSONConfig.PathToConfig);
            var config = JsonSerializer.Deserialize<Data>(jsonString);

            config.FavoritePlayers.Add(new FavoritePlayer { Id = playerId, Name = name});
            JSONConfig.FavoritePlayers = config.FavoritePlayers;

            await File.WriteAllTextAsync(JSONConfig.PathToConfig, JsonSerializer.Serialize(config, _jsonOptions));
        }

        public static async Task RemoveFavoritePlayerAsync(string playerId)
        {
            var jsonString = await File.ReadAllTextAsync(JSONConfig.PathToConfig);
            var config = JsonSerializer.Deserialize<Data>(jsonString);

            config.FavoritePlayers.RemoveAll(p => p.Id == playerId);
            JSONConfig.FavoritePlayers = config.FavoritePlayers;

            await File.WriteAllTextAsync(JSONConfig.PathToConfig, JsonSerializer.Serialize(config, _jsonOptions));
        }

        public static async Task AddFavoriteServerAsync(string serverId, string identifier)
        {
            var jsonString = await File.ReadAllTextAsync(JSONConfig.PathToConfig);
            var config = JsonSerializer.Deserialize<Data>(jsonString);

            config.FavoriteServers.Add(new FavoriteServer { Id = serverId, Name = identifier});
            JSONConfig.FavoriteServers = config.FavoriteServers;

            await File.WriteAllTextAsync(JSONConfig.PathToConfig, JsonSerializer.Serialize(config, _jsonOptions));
        }

        public static async Task RemoveFavoriteServerAsync(string serverId)
        {
            var jsonString = await File.ReadAllTextAsync(JSONConfig.PathToConfig);
            var config = JsonSerializer.Deserialize<Data>(jsonString);

            config.FavoriteServers.RemoveAll(p => p.Id == serverId);
            JSONConfig.FavoriteServers = config.FavoriteServers;

            await File.WriteAllTextAsync(JSONConfig.PathToConfig, JsonSerializer.Serialize(config, _jsonOptions));
        }

        public static async Task AddTrackedPlayerAsync(TrackedPlayer player)
        {
            var jsonString = await File.ReadAllTextAsync(JSONConfig.PathToConfig);
            var config = JsonSerializer.Deserialize<Data>(jsonString);

            config.TrackedPlayers.Add(player);
            JSONConfig.TrackedPlayers = config.TrackedPlayers;

            await File.WriteAllTextAsync(JSONConfig.PathToConfig, JsonSerializer.Serialize(config, _jsonOptions));
        }

        public static async Task RemoveTrackedPlayerAsync(TrackedPlayer player)
        {
            var jsonString = await File.ReadAllTextAsync(JSONConfig.PathToConfig);
            var config = JsonSerializer.Deserialize<Data>(jsonString);

            config.TrackedPlayers.RemoveAll(p => p.Id == player.Id);
            JSONConfig.TrackedPlayers = config.TrackedPlayers;

            await File.WriteAllTextAsync(JSONConfig.PathToConfig, JsonSerializer.Serialize(config, _jsonOptions));
        }

        public static async Task UpdateTrackedPlayerAsync(TrackedPlayer oldPlayer, TrackedPlayer newPlayer)
        {
            var jsonString = await File.ReadAllTextAsync(JSONConfig.PathToConfig);
            var config = JsonSerializer.Deserialize<Data>(jsonString);

            config.TrackedPlayers.RemoveAll(p => oldPlayer.Id == p.Id);
            config.TrackedPlayers.Add(newPlayer);

            JSONConfig.TrackedPlayers = config.TrackedPlayers;

            await File.WriteAllTextAsync(JSONConfig.PathToConfig, JsonSerializer.Serialize(config, _jsonOptions));
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
