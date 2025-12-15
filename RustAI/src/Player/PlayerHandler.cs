using System.Net.Http;
using System.Text.Json;

namespace RustAI
{
    internal static class PlayerHandler
    {
        public static async Task<JsonDocument?> GetBattlemetricsJson(string playerId, string includeFlags = "")
        {
            try
            {
                var url = $"https://api.battlemetrics.com/players/{playerId}?include={includeFlags}";
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

        public static async Task<JsonDocument?> GetRustStatsJson(string playerId, string includeFlags = "")
        {
            try
            {
                var url = $"https://api.battlemetrics.com/players/{playerId}?include={includeFlags}";
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

        public static async Task<string> GetId(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                    .GetProperty("data")
                    .GetProperty("attributes")
                    .GetProperty("id")
                    .GetString() ?? "N/A";
            }
            catch { return "N/A"; }
        }

        public static async Task<string> GetName(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                    .GetProperty("data")
                    .GetProperty("attributes")
                    .GetProperty("name")
                    .GetString() ?? "N/A";
            }
            catch { return "N/A"; }
        }

        public static async Task<string> GetCreationDate(JsonDocument doc)
        {
            try
            {
                return doc?.RootElement
                    .GetProperty("data")
                    .GetProperty("attributes")
                    .GetProperty("createdAt")
                    .GetString().Split('T')[0] ?? "N/A";
            }
            catch { return "N/A"; }
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

        public static async Task<string> GetListOfNames(JsonDocument doc)
        {
            try
            {
                var listOfNames = doc?.RootElement
                     .GetProperty("included")
                     .EnumerateArray().Select(x => new
                     {
                         Name = x.GetProperty("attributes").GetProperty("identifier").GetString().Trim()
                     }).Select(x => x.Name);

                var result = $"# Found {listOfNames.Count()} names.\n______________________________________________\n\n";
                result += string.Join("\n", listOfNames);

                return result;
            }
            catch { return "N/A"; }
        }

        public static async Task<string> GetListOfPlayedServersAndTime(JsonDocument doc)
        {
            try
            {
                var listOfServers = doc?.RootElement
                     .GetProperty("included")
                     .EnumerateArray().Select(x => new
                     {
                         Name = x.GetProperty("attributes").GetProperty("name").GetString(),
                         TimePlayed = Utils.ConvertSecondsToTimeFormat(x.GetProperty("meta").GetProperty("timePlayed").GetInt64()),
                     });

                var result = $"# Found {listOfServers.Count()} servers.\n______________________________________________\n\n";

                foreach (var server in listOfServers)
                    result += $"{server.Name.Trim("\r")} ({server.TimePlayed})\n\n";

                return result;
            }
            catch { return "N/A"; }
        }

        public static async Task<string> GetTotalServerTime(JsonDocument doc)
        {
            try
            {
                long totalTime = 0;

                var listOfServers = doc?.RootElement
                     .GetProperty("included")
                     .EnumerateArray().Select(x => new
                     {
                         Time = x.GetProperty("meta").GetProperty("timePlayed").GetInt64()
                     });

                foreach (var server in listOfServers)
                    totalTime += server.Time;

                var result = $"{Utils.ConvertSecondsToTimeFormat(totalTime).Split(':')[0]} hours";
                return result;
            }
            catch { return "N/A"; }
        }

        public static async Task<string> GetCurrentServer(JsonDocument doc)
        {
            try
            {
                var currentServer = doc?.RootElement
                  .GetProperty("included")
                  .EnumerateArray()
                  .Where(x => x.GetProperty("meta")
                                         .GetProperty("online")
                                         .GetBoolean() is true)
                  .Select(x => new
                  {
                      Name = x.GetProperty("attributes")
                              .GetProperty("name")
                              .GetString()
                  });

                if (currentServer.Count() == 0)
                    return "Not playing";
                else
                    return currentServer.First().Name;
            }
            catch { return "N/A"; }
        }

        public static async Task<string> GetLastServer(JsonDocument doc)
        {
            try
            {
                var listOfServers = doc?.RootElement
                   .GetProperty("included")
                   .EnumerateArray().ToList();

                string latestDate = null;
                string latestName = null;

                foreach (var server in listOfServers)
                {
                    var lastSeen = server
                        .GetProperty("meta")
                        .GetProperty("lastSeen")
                        .GetString();

                    if (latestDate == null || lastSeen.CompareTo(latestDate) > 0)
                    {
                        latestDate = lastSeen;
                        latestName = server
                            .GetProperty("attributes")
                            .GetProperty("name")
                            .GetString();
                    }
                }

                var parsedDate = DateTime.Parse(latestDate);
                var time = (DateTime.Now - parsedDate).TotalDays;
                var formattedDate = Utils.PrettyElapsed(time);

                return $"{latestName} ({formattedDate})";
            }
            catch { return "N/A"; }
        }

        public static async Task<string> GetFirstServer(JsonDocument doc)
        {
            try
            {
                var listOfServers = doc?.RootElement
                   .GetProperty("included")
                   .EnumerateArray().ToList();

                string earliestDate = null;
                string earliestName = null;

                foreach (var server in listOfServers)
                {
                    var firstSeen = server
                        .GetProperty("meta")
                        .GetProperty("firstSeen")
                        .GetString();

                    if (earliestDate == null || firstSeen.CompareTo(earliestDate) < 0)
                    {
                        earliestDate = firstSeen;
                        earliestName = server
                            .GetProperty("attributes")
                            .GetProperty("name")
                            .GetString();
                    }
                }

                var parsedDate = DateTime.Parse(earliestDate);
                var time = (DateTime.Now - parsedDate).TotalDays;
                var formattedDate = Utils.PrettyElapsed(time);

                return $"{earliestName} ({formattedDate})";
            }
            catch { return "N/A"; }
        }

        public static async Task<string> GetMostPlayedServer(JsonDocument doc)
        {
            try
            {
                var listOfServers = doc?.RootElement
                   .GetProperty("included")
                   .EnumerateArray().ToList();

                long maxPlayedTime = 0;
                string serverName = string.Empty;

                foreach (var server in listOfServers)
                {
                    var timePlayed = server
                        .GetProperty("meta")
                        .GetProperty("timePlayed")
                        .GetInt64();

                    if (timePlayed > maxPlayedTime)
                    {
                        maxPlayedTime = timePlayed;
                        serverName = server.GetProperty("attributes").GetProperty("name").GetString();
                    }
                }

                return $"{serverName} ({Utils.ConvertSecondsToTimeFormat(maxPlayedTime).Split(':')[0]} hours)";
            }
            catch { return "N/A"; }
        }

        public static async Task<string> GetPlayerFullInformation(JsonDocument doc)
        {
            return
   "🎮 Player Statistics\n" +
   "───────────────\n" +
   $"👤 Name: {await GetName(doc)}\n" +
   $"🆔 Battlemetrics ID: {await GetId(doc)}\n" +
   $"📅 Account Created: {await GetCreationDate(doc)}\n" +
   $"⏱ Total Time: {await GetTotalServerTime(doc)}\n\n" +
   "🌐 Server History\n" +
   "───────────────\n" +
   $"🟢  Current: {await GetCurrentServer(doc)}\n\n" +
   $"⏮ Last Played: {await GetLastServer(doc)}\n\n" +
   $"🏁 First Played: {await GetFirstServer(doc)}\n\n" +
   $"🔥 Most Played: {await GetMostPlayedServer(doc)}";

        }
    }
}
