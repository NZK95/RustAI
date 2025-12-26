using System.Text.Json;
using Telegram.Bot.Types;

namespace RustAI
{
    internal static class Builders
    {
        public static string BuildRustAIProjectLink()
        {
            return "https://github.com/NZK95/RustAI";
        }

        public static string BuildAuthorGitHubLink()
        {
            return "https://github.com/NZK95";
        }

        public static string BuildPlayerNamesFileName(string name)
        {
            return $"{DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss")} - names.txt";
        }

        public static string BuildServerPlayersFileName(string name)
        {
            return $"{DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss")} - players.txt";
        }

        public static string BuildPlayerServersFileName(string name)
        {
            return $"{DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss")} - servers.txt";
        }

        public static string BuildPlayerNamesFilePath(string fileName)
        {
            return JSONConfig.PathToExportedPlayersNames + $"\\{fileName}";
        }

        public static string BuildServerPlayersFilePath(string fileName)
        {
            return JSONConfig.PathToExportedServerPlayers + $"\\{fileName}";
        }

        public static string BuildPlayerServersFilePath(string fileName)
        {
            return JSONConfig.PathToExportedPlayersServers + $"\\{fileName}";
        }
        
        public static string BuildScreenshotsFilePath(string fileName)
        {
            return JSONConfig.PathToScreenshots + $"\\{fileName}";
        }

        public static string BuildScreenshotFileName()
        {
            return $"{DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss")} - screenshot.png";
        }

        public static string BuildPlayerWarningMessage()
        {
            return $"⚠️ Disclaimer:\nThe player information displayed were collected from Battlemetrics API and may not always be fully accurate or up-to-date.\r\nIf certain parameters cannot be retrieved, they are automatically replaced with safe default values.\n\n{Messages.PreparingData}";
        }

        public static string BuildServerWarningMessage()
        {
            return $"⚠️ Disclaimer:\nThe server information displayed were collected from Battlemetrics API and may not always be fully accurate or up-to-date.\r\nIf certain parameters cannot be retrieved, they are automatically replaced with safe default values.\n\n{Messages.PreparingData}";
        }

        public static async Task<string> BuildConnectWarningMessageAsync(JsonDocument json)
        {
            return $"⚠️ Disclaimer:\nThis method works only if you set the right time Rust needs to start. If the time is wrong, it may act in unexpected ways.\n";
        }

        public static string BuildAuthorFileWatermark()
        {
            return $"# This file was created by Rust AI.\n# For more information, visit:\n# Author GitHub: https://github.com/NZK95\n# Project GitHub: https://github.com/NZK95/RustAI\n\n";
        }

    }
}
