using System.Text.Json;
using System.IO;

namespace RustAI
{
    internal static class JSONConfigHandler
    {
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        public static async Task AddFavoritePlayerAsync(string playerId, string identifier)
        {
            var jsonString = await File.ReadAllTextAsync(JSONConfig.PathToConfig);
            var config = JsonSerializer.Deserialize<Data>(jsonString);

            config.FavoritePlayers.Add($"{playerId} | {identifier}");
            JSONConfig.FavoritePlayers = config.FavoritePlayers;

            await File.WriteAllTextAsync(JSONConfig.PathToConfig, JsonSerializer.Serialize(config, _jsonOptions));
        }

        public static async Task AddFavoriteServerAsync(string serverId, string identifier)
        {
            var jsonString = await File.ReadAllTextAsync(JSONConfig.PathToConfig);
            var config = JsonSerializer.Deserialize<Data>(jsonString);

            config.FavoriteServers.Add($"{identifier} | {serverId}");
            JSONConfig.FavoriteServers = config.FavoriteServers;

            await File.WriteAllTextAsync(JSONConfig.PathToConfig, JsonSerializer.Serialize(config, _jsonOptions));
        }

        public static async Task AddTrackedPlayerAsync(string playerId, string name, string server)
        {
            var jsonString = await File.ReadAllTextAsync(JSONConfig.PathToConfig);
            var config = JsonSerializer.Deserialize<Data>(jsonString);

            config.TrackedPlayers.Add($"{playerId} | {name} | {server}");
            JSONConfig.TrackedPlayers = config.TrackedPlayers;

            await File.WriteAllTextAsync(JSONConfig.PathToConfig, JsonSerializer.Serialize(config, _jsonOptions));
        }

        public static async Task RemoveTrackedPlayerAsync(string playerId)
        {
            var jsonString = await File.ReadAllTextAsync(JSONConfig.PathToConfig);
            var config = JsonSerializer.Deserialize<Data>(jsonString);

            config.TrackedPlayers.RemoveAll(p => p.StartsWith($"{playerId} |"));
            JSONConfig.TrackedPlayers = config.TrackedPlayers;

            await File.WriteAllTextAsync(JSONConfig.PathToConfig, JsonSerializer.Serialize(config, _jsonOptions));
        }

        public static async Task UpdateTrackedPlayerAsync(string playerId, string name, string newServer)
        {
            var jsonString = await File.ReadAllTextAsync(JSONConfig.PathToConfig);
            var config = JsonSerializer.Deserialize<Data>(jsonString);

            config.TrackedPlayers.RemoveAll(p => p.StartsWith($"{playerId} |"));
            config.TrackedPlayers.Add($"{playerId} | {name} | {newServer}");
            JSONConfig.TrackedPlayers = config.TrackedPlayers;

            await File.WriteAllTextAsync(JSONConfig.PathToConfig, JsonSerializer.Serialize(config, _jsonOptions));
        }
    }
}
