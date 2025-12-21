namespace RustAI
{
    internal static class Date
    {
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
    }
}
