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
        public InlineKeyboardMarkup FavoritePlayers { get; private set; }
        public InlineKeyboardMarkup FavoriteServers { get; private set; }

        public KeyboardFactory()
        {
            InitKeyboards();
        }

        public void InitKeyboards()
        {
            Servers = BuildServersInfo();
            Connects = BuildConnects();
            Players = BuildPlayers();
            FavoritePlayers = FavoritePlayerMarkup();
            FavoriteServers = FavoriteServerMarkup();
            Tracking = BuildPlayersTrack();
            TrackingRemove = BuildPlayersTrackRemove();
        }

        private InlineKeyboardMarkup BuildServersInfo()
        {
            var rows = new List<InlineKeyboardButton[]>();

            foreach (var server in JSONConfig.FavoriteServers)
            {
                var parts = server.Split('|');
                var title = parts[0].Trim();
                var id = parts[1].Trim();
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData(text: title, callbackData: $"ServersInfo@{id}") });
            }

            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("Your server id", "user_server_id") });
            return new InlineKeyboardMarkup(rows);
        }

        private InlineKeyboardMarkup BuildConnects()
        {
            var rows = new List<InlineKeyboardButton[]>();

            foreach (var server in JSONConfig.FavoriteServers)
            {
                var parts = server.Split('|');
                var title = parts[0].Trim();
                var id = parts[1].Trim();
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData(title, $"Connects@{id}") });
            }

            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("Your server id", "user_server_id") });
            return new InlineKeyboardMarkup(rows);
        }

        private InlineKeyboardMarkup BuildPlayers()
        {
            var rows = new List<InlineKeyboardButton[]>();

            foreach (var player in JSONConfig.FavoritePlayers)
            {
                var parts = player.Split('|');
                var id = parts[0].Trim();
                var name = parts[1].Trim();
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData(name, $"PlayersInfo@{id}") });
            }

            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("Your player id", "user_player_id") });
            return new InlineKeyboardMarkup(rows);
        }

        private InlineKeyboardMarkup BuildPlayersTrack()
        {
            var rows = new List<InlineKeyboardButton[]>();

            foreach (var player in JSONConfig.FavoritePlayers)
            {
                var parts = player.Split('|');
                var id = parts[0].Trim();
                var name = parts[1].Trim();
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData(name, $"Tracking@{id}") });
            }

            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("Your player id", "user_player_id") });
            return new InlineKeyboardMarkup(rows);
        }

        private InlineKeyboardMarkup BuildPlayersTrackRemove()
        {
            var rows = new List<InlineKeyboardButton[]>();

            foreach (var player in JSONConfig.TrackedPlayers)
            {
                var parts = player.Split('|');

                var id = parts[0].Trim();
                var name = parts[1].Trim();
                var server = parts[2].Trim();

                rows.Add(new[] { InlineKeyboardButton.WithCallbackData(name, $"TrackingRemove@{id}") });
            }

            return new InlineKeyboardMarkup(rows);
        }

        private InlineKeyboardMarkup FavoritePlayerMarkup() =>
            new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("⭐ Add to favorite", "favorite_player") } });

        private InlineKeyboardMarkup FavoriteServerMarkup() =>
            new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("⭐ Add to favorite", "favorite_server") } });
    }
}