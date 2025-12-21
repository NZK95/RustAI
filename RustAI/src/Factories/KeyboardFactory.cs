using Telegram.Bot.Types.ReplyMarkups;

namespace RustAI
{
    internal class KeyboardFactory
    {
        public InlineKeyboardMarkup Servers { get; private set; }
        public InlineKeyboardMarkup Connects { get; private set; }
        public InlineKeyboardMarkup Players { get; private set; }
        public InlineKeyboardMarkup Tracking { get; private set; }
        public InlineKeyboardMarkup TrackingRemove { get; private set; }
        public InlineKeyboardMarkup FavoritePlayerAdd { get; private set; }
        public InlineKeyboardMarkup FavoriteServerAdd { get; private set; }
        public InlineKeyboardMarkup FavoriteServerRemove { get; private set; }
        public InlineKeyboardMarkup FavoritePlayerRemove { get; private set; }

        public KeyboardFactory()
        {
            InitKeyboards();
        }

        public void InitKeyboards()
        {
            Servers = BuildServers();
            Connects = BuildConnects();
            Players = BuildPlayers();
            Tracking = BuildTracking();
            TrackingRemove = BuildTrackingRemove();
            FavoritePlayerAdd = BuildFavoritePlayer();
            FavoriteServerAdd = BuildFavoriteServer();
            FavoriteServerRemove = BuildRemoveServerFromFavorites();
            FavoritePlayerRemove = BuildRemovePlayerFromFavorites();
        }

        private InlineKeyboardMarkup BuildServers()
        {
            var rows = new List<InlineKeyboardButton[]>();

            foreach (var server in JSONConfig.FavoriteServers)
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData(server.Name, callbackData: $"ServersInfo@{server.Id}") });

            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("Your server id", "user_server_id") });
            return new InlineKeyboardMarkup(rows);
        }

        private InlineKeyboardMarkup BuildConnects()
        {
            var rows = new List<InlineKeyboardButton[]>();

            foreach (var server in JSONConfig.FavoriteServers)
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData(server.Name, $"Connects@{server.Id}") });

            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("Your server id", "user_server_id") });
            return new InlineKeyboardMarkup(rows);
        }

        private InlineKeyboardMarkup BuildPlayers()
        {
            var rows = new List<InlineKeyboardButton[]>();

            foreach (var player in JSONConfig.FavoritePlayers)
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData(player.Name, $"PlayersInfo@{player.Id}") });

            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("Your player id", "user_player_id") });
            return new InlineKeyboardMarkup(rows);
        }

        private InlineKeyboardMarkup BuildTracking()
        {
            var rows = new List<InlineKeyboardButton[]>();

            foreach (var player in JSONConfig.FavoritePlayers)
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData(player.Name, $"Tracking@{player.Id}") });

            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("Your player id", "user_player_id") });
            return new InlineKeyboardMarkup(rows);
        }

        private InlineKeyboardMarkup BuildTrackingRemove()
        {
            var rows = new List<InlineKeyboardButton[]>();

            foreach (var player in JSONConfig.TrackedPlayers)
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData(player.Name, $"TrackingRemove@{player.Id}") });

            return new InlineKeyboardMarkup(rows);
        }

        private InlineKeyboardMarkup BuildFavoritePlayer() =>
            new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("⭐ Add to favorite", "favorite_player") } });

        private InlineKeyboardMarkup BuildFavoriteServer() =>
            new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("⭐ Add to favorite", "favorite_server") } });

        private InlineKeyboardMarkup BuildRemoveServerFromFavorites() =>
          new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("🗑️ Remove from favorites", "favorite_server_remove") } });

        private InlineKeyboardMarkup BuildRemovePlayerFromFavorites() =>
         new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("🗑️ Remove from favorites", "favorite_player_remove") } });
    }
}