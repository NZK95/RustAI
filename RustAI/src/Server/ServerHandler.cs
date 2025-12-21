using System.Net.Http;
using System.Text.Json;

namespace RustAI
{
    internal static class ServerHandler
    {
        public static async Task<JsonDocument?> GetJson(string serverId, string includeFlags = "")
        {
            try
            {
                var url = $"https://api.battlemetrics.com/servers/{serverId}?include={includeFlags}";
                using HttpClient client = new HttpClient();

                var response = await client.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();

                return JsonDocument.Parse(json);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<(bool, string)> RateLimitError(JsonDocument doc)
        {
            try
            {
                doc?.RootElement.GetProperty("errors");
                return (true, doc?.RootElement.GetProperty("errors")[0].GetProperty("detail").GetString().Replace("T", " "))!;
            }
            catch
            {
                return (false, string.Empty);
            }
        }

        public static async Task<string> GetName(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                    .GetProperty("data").GetProperty("attributes").GetProperty("name").GetString()
                    ?? "N/A";
            }
            catch { return "N/A"; }
        }

        public static async Task<string> GetStatus(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                    .GetProperty("data").GetProperty("attributes").GetProperty("status").GetString()
                    ?? "N/A";
            }
            catch { return "N/A"; }
        }

        public static async Task<string> GetAddress(JsonDocument doc)
        {
            try
            {
                var address = doc?.RootElement
                    .GetProperty("data").GetProperty("attributes").GetProperty("address").GetString()
                    ?? "N/A";

                if (address == "N/A")
                    address = await GetIP(doc);

                var port = doc?.RootElement
                    .GetProperty("data").GetProperty("attributes").GetProperty("port").GetInt32()
                    ?? 0;

                return $"{address}:{port}";
            }
            catch { return "N/A"; }
        }

        public static async Task<string> GetIP(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                     .GetProperty("data").GetProperty("attributes").GetProperty("ip").GetString()
                     ?? "N/A";
            }
            catch { return "N/A"; }
        }

        public static async Task<string> GetServerID(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                     .GetProperty("data").GetProperty("attributes").GetProperty("id").GetString()
                     ?? "N/A";
            }
            catch { return "N/A"; }
        }

        public static async Task<string> GetLastWipeDate(JsonDocument doc)
        {
            try
            {
                var lastWipe = doc?.RootElement
                     .GetProperty("data").GetProperty("attributes")
                     .GetProperty("details").GetProperty("rust_last_wipe").GetString()
                     ?? "N/A";

                return lastWipe.Split('T')[0];
            }
            catch { return "N/A"; }
        }

        public static async Task<string> GetCreationTime(JsonDocument doc)
        {
            try
            {
                var creationTime = doc?.RootElement
                     .GetProperty("data").GetProperty("attributes")
                     .GetProperty("createdAt").GetString()
                     ?? "N/A";

                return creationTime.Split('T')[0];
            }
            catch { return "N/A"; }
        }

        public static async Task<int> GetTeamLimit(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                    .GetProperty("data").GetProperty("attributes")
                    .GetProperty("details").GetProperty("rust_settings")
                    .GetProperty("teamUILimit").GetInt32()
                    ?? 0;
            }
            catch { return 0; }
        }

        public static async Task<int> GetGroupLimit(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                    .GetProperty("data").GetProperty("attributes")
                    .GetProperty("details").GetProperty("rust_settings")
                    .GetProperty("groupLimit").GetInt32()
                    ?? 0;
            }
            catch { return 0; }
        }

        public static async Task<string> GetWebsite(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                    .GetProperty("data").GetProperty("attributes")
                    .GetProperty("details").GetProperty("rust_url").GetString()
                    ?? "N/A";
            }
            catch { return "N/A"; }
        }

        public static async Task<string> GetDescription(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                    .GetProperty("data").GetProperty("attributes")
                    .GetProperty("details").GetProperty("rust_description").GetString().Replace("\\n", "\n").Replace("\\t", "")
                    ?? "N/A";
            }
            catch { return "N/A"; }
        }

        public static async Task<bool> GetOfficialServer(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                    .GetProperty("data").GetProperty("attributes")
                    .GetProperty("details").GetProperty("official").GetBoolean()
                    ?? false;
            }
            catch { return false; }
        }

        public static async Task<bool> GetPremiumServer(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                    .GetProperty("data").GetProperty("attributes")
                    .GetProperty("details").GetProperty("rust_premium").GetBoolean()
                    ?? false;
            }
            catch { return false; }
        }

        public static async Task<bool> GetModdedServer(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                    .GetProperty("data").GetProperty("attributes")
                    .GetProperty("details").GetProperty("rust_modded").GetBoolean()
                    ?? false;
            }
            catch { return false; }
        }

        public static async Task<string> GetMapDownloadLink(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                    .GetProperty("data").GetProperty("attributes")
                    .GetProperty("details").GetProperty("rust_maps")
                    .GetProperty("mapUrl").GetString()
                    ?? "N/A";
            }
            catch { return "N/A"; }
        }

        public static async Task<int> GetMapSeed(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                    .GetProperty("data").GetProperty("attributes")
                    .GetProperty("details").GetProperty("rust_maps")
                    .GetProperty("seed").GetInt32()
                    ?? 0;
            }
            catch { return 0; }
        }

        public static async Task<double> GetSnowPercentage(JsonDocument doc)
        {
            try
            {
                return Math.Round(
                    doc?.RootElement.GetProperty("data")
                    .GetProperty("attributes").GetProperty("details")
                    .GetProperty("rust_maps").GetProperty("biomePercentages")
                    .GetProperty("s").GetDouble()
                    ?? 0.0, 2
                );
            }
            catch { return 0.0; }
        }

        public static async Task<double> GetDesertPercentage(JsonDocument doc)
        {
            try
            {
                return Math.Round(
                    doc?.RootElement.GetProperty("data")
                    .GetProperty("attributes").GetProperty("details")
                    .GetProperty("rust_maps").GetProperty("biomePercentages")
                    .GetProperty("d").GetDouble()
                    ?? 0.0, 2
                );
            }
            catch { return 0.0; }
        }

        public static async Task<double> GetForestPercentage(JsonDocument doc)
        {
            try
            {
                return Math.Round(
                    doc?.RootElement.GetProperty("data")
                    .GetProperty("attributes").GetProperty("details")
                    .GetProperty("rust_maps").GetProperty("biomePercentages")
                    .GetProperty("f").GetDouble()
                    ?? 0.0, 2
                );
            }
            catch { return 0.0; }
        }

        public static async Task<double> GetTundraPercentage(JsonDocument doc)
        {
            try
            {
                return Math.Round(
                    doc?.RootElement.GetProperty("data")
                    .GetProperty("attributes").GetProperty("details")
                    .GetProperty("rust_maps").GetProperty("biomePercentages")
                    .GetProperty("t").GetDouble()
                    ?? 0.0, 2
                );
            }
            catch { return 0.0; }
        }

        public static async Task<double> GetJunglePercentage(JsonDocument doc)
        {
            try
            {
                return Math.Round(
                    doc?.RootElement.GetProperty("data")
                    .GetProperty("attributes").GetProperty("details")
                    .GetProperty("rust_maps").GetProperty("biomePercentages")
                    .GetProperty("j").GetDouble()
                    ?? 0.0, 2
                );
            }
            catch { return 0.0; }
        }

        public static async Task<bool> GetKitsStatus(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                    .GetProperty("data").GetProperty("attributes")
                    .GetProperty("details").GetProperty("rust_settings")
                    .GetProperty("kits").GetBoolean()
                    ?? false;
            }
            catch { return false; }
        }

        public static async Task<string> GetLocation(JsonDocument doc)
        {
            try
            {
                var arr = doc?.RootElement
                    .GetProperty("data").GetProperty("attributes")
                    .GetProperty("location")
                    .EnumerateArray()
                    .Select(x => x.GetDouble())
                    .ToArray();

                return arr != null ? "[" + string.Join(", ", arr) + "]" : "N/A";
            }
            catch { return "N/A"; }
        }

        public static async Task<string> GetUpcomingWipes(JsonDocument doc)
        {
            try
            {
                var wipes = doc?.RootElement
                    .GetProperty("data").GetProperty("attributes")
                    .GetProperty("details").GetProperty("rust_wipes")
                    .EnumerateArray()
                    .Select(x => new
                    {
                        Type = x.GetProperty("type").GetString() == "map" ? "Normal" : "Global" ?? "N/A",
                        Date = x.GetProperty("timestamp").GetString().Split('T')[0] ?? "N/A"
                    }).ToArray();

                if (wipes.Length <= 0)
                    return "N/A";

                var stringResult = string.Empty;

                foreach (var wipe in wipes)
                    stringResult += $"|  [Type: {wipe.Type} wipe    |    Date: {wipe.Date}]\n";

                return stringResult;
            }
            catch { return "N/A"; }
        }

        public static async Task<string> GetCountry(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                    .GetProperty("data").GetProperty("attributes")
                    .GetProperty("country").GetString() ?? "N/A";

            }
            catch { return "N/A"; }
        }

        public static List<string> GetPlayers(JsonDocument doc)
        {
            try
            {
                var players = doc?.RootElement.GetProperty("included")
                    .EnumerateArray()
                    .Select(x => x.GetProperty("relationships")
                                  .GetProperty("player")
                                  .GetProperty("data")
                                  .GetProperty("id")
                                  .GetString())
                    .ToList();

                return players ?? new List<string>();
            }
            catch { return new List<string>(); }
        }

        public static async Task<int> GetPlayersCount(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                    .GetProperty("data").GetProperty("attributes")
                    .GetProperty("players").GetInt32()
                    ?? 0;
            }
            catch { return 0; }
        }

        public static async Task<int> GetMaxPlayersCount(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                    .GetProperty("data").GetProperty("attributes")
                    .GetProperty("maxPlayers").GetInt32()
                    ?? 0;
            }
            catch { return 0; }
        }

        public static async Task<int> GetQueuedPlayers(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                    .GetProperty("data").GetProperty("attributes")
                    .GetProperty("details").GetProperty("rust_queued_players")
                    .GetInt32()
                    ?? 0;
            }
            catch { return 0; }
        }

        public static async Task<string> GetMapUrl(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                    .GetProperty("data").GetProperty("attributes")
                    .GetProperty("details").GetProperty("rust_maps")
                    .GetProperty("url").GetString()
                    ?? "N/A";
            }
            catch { return "N/A"; }
        }

        public static async Task<double> GetUpkeep(JsonDocument doc)
        {
            try
            {
                return
                    doc?.RootElement.GetProperty("data")
                    .GetProperty("attributes").GetProperty("details")
                    .GetProperty("rust_settings").GetProperty("upkeep").GetInt32() ?? 0;
            }
            catch { return 0.0; }
        }


        public static async Task<double> GetRates(JsonDocument doc)
        {
            try
            {
                return
                    doc?.RootElement.GetProperty("data")
                    .GetProperty("attributes").GetProperty("details")
                    .GetProperty("rust_settings").GetProperty("rates").GetProperty("craft").GetDouble() ?? 0.0;
            }
            catch { return 0.0; }
        }


        public static async Task<double> GetDecay(JsonDocument doc)
        {
            try
            {
                return
                    doc?.RootElement.GetProperty("data")
                    .GetProperty("attributes").GetProperty("details")
                    .GetProperty("rust_settings").GetProperty("decay").GetInt32() ?? 0;
            }
            catch { return 0.0; }
        }

        public static async Task<string> GetLastRestart(JsonDocument doc)
        {
            try
            {
                return
                    DateTime.Parse(doc?.RootElement.GetProperty("data").GetProperty("attributes")
                    .GetProperty("details").GetProperty("lastRestart")
                    .GetString()).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A";

            }
            catch { return "N/A"; }
        }

        public static async Task<string> GetServerFullInformation(JsonDocument doc)
        {
            return
"🌐 <b>Server Statistics</b>\n" +
   "───────────────\n" +
$"│ 🏷️ Name: {await GetName(doc)}\n" +
$"│ 🆔 Server ID: {await GetServerID(doc)}\n" +
$"│ 📡 Status: {await GetStatus(doc)}\n" +
$"│ 💥 Created at: {await GetCreationTime(doc)}\n" +
"\n" +
$"│ 🔗 Connect: <code>client.connect {await GetAddress(doc)}</code>\n" +
$"│ 🌐 IP: {await GetIP(doc)}\n" +
$"│ 📍 Location: {await GetLocation(doc)}\n" +
$"│ ♻ Last restart: {await GetLastRestart(doc)}\n" +
"\n" +
$"│ 🎮 Players: {await GetPlayersCount(doc)}/{await GetMaxPlayersCount(doc)}\n" +
$"│ ⏳ Queue: {(await GetQueuedPlayers(doc) == 0 ? "No queue" : await GetQueuedPlayers(doc))}\n" +
$"│ 👥 Team limit: {(await GetTeamLimit(doc) == 999999 ? "None" : await GetTeamLimit(doc))}\n" +
$"│ 👥 Group limit: {(await GetGroupLimit(doc) == 999999 ? "None" : await GetGroupLimit(doc))}\n" +
"\n" +
$"│ 🕒 Last wipe: {await GetLastWipeDate(doc)}\n" +
$"│ 🕒 Upcoming wipes: \n{await GetUpcomingWipes(doc)}" +
"\n" +
$"│ 🗺️ Map download: {(await GetMapDownloadLink(doc) == "N/A"
    ? "N/A"
    : $"<a href=\"{await GetMapDownloadLink(doc)}\">Download</a>")}\n" +
$"│ 🌱 Seed: {await GetMapSeed(doc)}\n" +
$"│ ❄️ Snow: {await GetSnowPercentage(doc)}%\n" +
$"│ 🏜️ Desert: {await GetDesertPercentage(doc)}%\n" +
$"│ 🌲 Forest: {await GetForestPercentage(doc)}%\n" +
$"│ 🧊 Tundra: {await GetTundraPercentage(doc)}%\n" +
$"│ 🌴 Jungle: {await GetJunglePercentage(doc)}%\n" +
"\n" +
$"│ ✔️ Rates: {await GetRates(doc)}x\n" +
$"│ ✔️ Decay: {await GetDecay(doc) * 100}%\n" +
$"│ ✔️ Upkeep: {await GetUpkeep(doc) * 100}%\n" +
"\n" +
$"│ ✔️ Official: {await GetOfficialServer(doc)}\n" +
$"│ ⭐ Premium: {await GetPremiumServer(doc)}\n" +
$"│ 🔧 Modded: {await GetModdedServer(doc)}\n" +
$"│ 🎁 Kits: {await GetKitsStatus(doc)}\n" +
"\n" +
$"| 🔗 Website: {(await GetWebsite(doc) == "N/A"
    ? "N/A"
    : $"<a href=\"{await GetWebsite(doc)}\">{await GetName(doc)}</a>")}\n" +
$"| 🔗 Battlemetrics: {(await GetServerID(doc) == "N/A"
    ? "N/A"
    : $"<a href=\"https://www.battlemetrics.com/servers/rust/{await GetServerID(doc)}\">{await GetName(doc)}</a>")}";
        }
    }
}
