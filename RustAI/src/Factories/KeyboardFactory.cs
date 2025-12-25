using Telegram.Bot.Types.ReplyMarkups;

namespace RustAI
{
    internal class KeyboardFactory
    {
        public InlineKeyboardMarkup Servers { get; private set; }
        public InlineKeyboardMarkup Connects { get; private set; }
        public InlineKeyboardMarkup AutoConnects { get; private set; }
        public InlineKeyboardMarkup Players { get; private set; }
        public InlineKeyboardMarkup Tracking { get; private set; }
        public InlineKeyboardMarkup TrackingRemove { get; private set; }
        public InlineKeyboardMarkup FavoritePlayerAdd { get; private set; }
        public InlineKeyboardMarkup FavoriteServerAdd { get; private set; }
        public InlineKeyboardMarkup FavoriteServerRemove { get; private set; }
        public InlineKeyboardMarkup FavoritePlayerRemove { get; private set; }
        public InlineKeyboardMarkup Start { get; private set; }
        public InlineKeyboardMarkup Settings { get; private set; }

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
            AutoConnects = BuildAutoConnects();
            Start = BuildStart();
            Settings = BuildSettings();
        }

        public static InlineKeyboardMarkup BuildConnect(string serverID)
        {
            return new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData("Right now", $"{Constants.PrefixConnectNow}@{serverID}"),
                InlineKeyboardButton.WithCallbackData("When queue reaches a number",$"{Constants.PrefixConnectQueue}@{serverID}" ),
                InlineKeyboardButton.WithCallbackData("After a timer", $"{Constants.PrefixConnectTimer}@{serverID}")
            });
        }

        private static InlineKeyboardMarkup BuildStart()
        {
            var rows = new List<InlineKeyboardButton[]>();

            rows.Add(new[]
            {
            InlineKeyboardButton.WithCallbackData("👥 Players", Constants.PrefixPlayers),
            InlineKeyboardButton.WithCallbackData("🌐 Servers", Constants.PrefixServers)
            });

            rows.Add(new[]
            {
        InlineKeyboardButton.WithCallbackData("🚀 Launch", Constants.PrefixLaunch),
        InlineKeyboardButton.WithCallbackData("🛑 Quit", Constants.PrefixQuit)
            });

            rows.Add(new[]
            {
        InlineKeyboardButton.WithCallbackData("🔗 Connect", Constants.PrefixConnect),
        InlineKeyboardButton.WithCallbackData("⚡ AutoConnect", Constants.PrefixAutoConnect),
        InlineKeyboardButton.WithCallbackData("🔌 Disconnect", Constants.PrefixDisconnect),
        InlineKeyboardButton.WithCallbackData("📊 Status", Constants.PrefixStatus)
         });

            rows.Add(new[]
            {
        InlineKeyboardButton.WithCallbackData("➕ Add", Constants.PrefixAdd),
        InlineKeyboardButton.WithCallbackData("➖ Remove", Constants.PrefixRemove),
        InlineKeyboardButton.WithCallbackData("📋 List", Constants.PrefixList),
        InlineKeyboardButton.WithCallbackData("🗑️ Clear", Constants.PrefixClear)
            });

            rows.Add(new[]
            {
        InlineKeyboardButton.WithCallbackData("⚙️ Settings", Constants.PrefixSettings),
        InlineKeyboardButton.WithUrl("💻 GitHub", Builders.BuildRustAIProjectLink())
            });

            return new InlineKeyboardMarkup(rows);
        }

        public static InlineKeyboardMarkup BuildSettings(bool backButton = true)
        {
            var rows = new List<InlineKeyboardButton[]>();

            var playerNames = $"{Formatters.GetEmoji(JSONConfig.GetListOfPlayerNames)} Player Names History";
            var playerServers = $"{Formatters.GetEmoji(JSONConfig.GetListOfPlayerServers)} Player Servers History";

            rows.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData(playerNames, Constants.PrefixUpdatePNH),
                InlineKeyboardButton.WithCallbackData(playerServers, Constants.PrefixUpdatePSH)
            });


            var serverDescription = $"{Formatters.GetEmoji(JSONConfig.GetServerDescription)} Server Description";
            var screnenshotWhenJoined = $"{Formatters.GetEmoji(JSONConfig.SendScreenshotWhenJoined)} Screenshot When Joined";

            rows.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData(serverDescription, Constants.PrefixUpdateGSD),
                InlineKeyboardButton.WithCallbackData(screnenshotWhenJoined, Constants.PrefixUpdateSWJ)
            });

            rows.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData($"Rust Launch Delay: {JSONConfig.RustLaunchDelaySeconds} sec", Constants.PrefixBackSettings),
                InlineKeyboardButton.WithCallbackData($"Queue Limit: {JSONConfig.QueueLimit}", Constants.PrefixBackSettings)
            });

            rows.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData($"Connect Timer: {JSONConfig.ConnectTimerMinutes} min", Constants.PrefixBackSettings),
                InlineKeyboardButton.WithCallbackData($"User ID: {JSONConfig.BattlemetricsID}", Constants.PrefixBackSettings)
            });

            rows.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData("⬅️ Back", Constants.PrefixBackSettings)
            });

            return new InlineKeyboardMarkup(rows);
        }

        private InlineKeyboardMarkup BuildServers()
        {
            var rows = new List<InlineKeyboardButton[]>();

            foreach (var server in JSONConfig.FavoriteServers)
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData(server.Name, callbackData: $"{Constants.PrefixServersInfo}@{server.Id}") });

            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("Your server id", "user_server_id") });
            return new InlineKeyboardMarkup(rows);
        }

        private InlineKeyboardMarkup BuildConnects()
        {
            var rows = new List<InlineKeyboardButton[]>();

            foreach (var server in JSONConfig.FavoriteServers)
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData(server.Name, $"@{Constants.PrefixConnects}{server.Id}") });

            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("Your server id", "user_server_id") });
            return new InlineKeyboardMarkup(rows);
        }

        private InlineKeyboardMarkup BuildAutoConnects()
        {
            var rows = new List<InlineKeyboardButton[]>();

            foreach (var server in JSONConfig.FavoriteServers)
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData(server.Name, $"{Constants.PrefixAutoConnects}@{server.Id}") });

            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("Your server id", "user_server_id") });
            return new InlineKeyboardMarkup(rows);
        }

        private InlineKeyboardMarkup BuildPlayers()
        {
            var rows = new List<InlineKeyboardButton[]>();

            foreach (var player in JSONConfig.FavoritePlayers)
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData(player.Name, $"{Constants.PrefixPlayersInfo}@{player.Id}") });

            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("Your player id", "user_player_id") });
            return new InlineKeyboardMarkup(rows);
        }

        private InlineKeyboardMarkup BuildTracking()
        {
            var rows = new List<InlineKeyboardButton[]>();

            foreach (var player in JSONConfig.FavoritePlayers)
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData(player.Name, $"{Constants.PrefixTracking}@{player.Id}") });

            rows.Add(new[] { InlineKeyboardButton.WithCallbackData("Your player id", "user_player_id") });
            return new InlineKeyboardMarkup(rows);
        }

        private InlineKeyboardMarkup BuildTrackingRemove()
        {
            var rows = new List<InlineKeyboardButton[]>();

            foreach (var player in JSONConfig.TrackedPlayers)
                rows.Add(new[] { InlineKeyboardButton.WithCallbackData(player.Name, $"{Constants.PrefixTrackingRemove}@{player.Id}") });

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