using System.Diagnostics;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace RustAI
{
    internal static class Utils
    {
        public static string BuildAboutMessage()
        {
            return $"🌐 Developer: {GetAuthorGitHubLink()}" +
                   $"\n🌐 RustAI: {GetRustAIProjectLink()}";
        }

        public static string GetRustAIProjectLink()
        {
            return "https://github.com/NZK95/RustAI";
        }

        public static string GetAuthorGitHubLink()
        {
            return "https://github.com/NZK95";
        }

        public static string BuildPlayerInfoWarningMessage()
        {
            return $"⚠️ Data Disclaimer:\r\nThe player information displayed may not always be fully accurate or up-to-date.\r\nIf certain parameters cannot be retrieved, they are automatically replaced with safe default values.";
        }

        public static string BuildServerfInfoWarningMessage()
        {
            return $"⚠️ Data Disclaimer:\r\nThe server information displayed may not always be fully accurate or up-to-date.\r\nIf certain parameters cannot be retrieved, they are automatically replaced with safe default values.";
        }

        public static async Task<string> BuildConnectWarningMessageAsync(JsonDocument json)
        {
            return $"There are currently {await ServerHandler.GetPlayersCount(json)} players on the server, and {await ServerHandler.GetQueuedPlayers(json)} in queue.\n";
        }

        public static string BuildPlayerNamesFileName(string name)
        {
            return $"{DateTime.Now.ToString("dd.MM.yyyy HH.mm")} {name} - Names.txt";
        }

        public static string BuildPlayerServersFileName(string name)
        {
            return $"{DateTime.Now.ToString("dd.MM.yyyy HH.mm")} {name} - Servers.txt";
        }

        public static string BuildAuthorFileWatermark()
        {
            return $"# This file was created by Rust AI.\n# For more information, visit:\n# Author GitHub: https://github.com/NZK95\n# Project GitHub: https://github.com/NZK95/RustAI\n\n";
        }

        public static string ConvertSecondsToTimeFormat(long totalSeconds)
        {
            var hours = totalSeconds / 3600;
            var minutes = (totalSeconds % 3600) / 60;
            var seconds = totalSeconds % 60;

            return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
        }

        public static string PrettyElapsed(double daysAgo)
        {
            if (daysAgo < 1)
            {
                int hours = (int)Math.Floor(daysAgo * 24);
                if (hours <= 0) return "just now";
                return $"{hours} hour{(hours == 1 ? "" : "s")} ago";
            }

            int days = (int)Math.Floor(daysAgo);

            if (days < 7)
                return $"{days} day{(days == 1 ? "" : "s")} ago";

            int weeks = (int)Math.Round(days / 7.0);
            if (weeks < 5)
                return $"{weeks} week{(weeks == 1 ? "" : "s")} ago";

            int months = (int)Math.Round(days / 30.4375);
            if (months < 12)
                return $"{months} month{(months == 1 ? "" : "s")} ago";

            int years = (int)Math.Round(days / 365.25);
            return $"{years} year{(years == 1 ? "" : "s")} ago";
        }

        public static bool IsPlayerAlreadyFavorited(string playerId)
        {
            if (JSONConfig.FavoritePlayers.Any(x => x.StartsWith(playerId)))
                return true;

            return false;

        }
        public static bool IsServerAlreadyFavorited(string serverId)
        {
            if (JSONConfig.FavoriteServers.Any(x => x.Split('|')[1].Trim().StartsWith(serverId)))
                return true;

            return false;
        }

        public static string GetSteamPath()
        {
            const string AppId = "252490";
            return $"steam://rungameid/{AppId}";
        }

        public static async Task<bool> IsPlayerTrackedAsync(string playerId)
        {
            return JSONConfig.TrackedPlayers.Any(p => p.StartsWith($"{playerId} |"));
        }
    }
}
