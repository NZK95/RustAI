namespace RustAI
{ 
    internal static class Formatters
    {
        public static string GetCheckmark(bool value)
        {
            return value ? "✅ Enabled" : "❌ Disabled";
        }

        public static string GetEmoji(bool value)
        {
            return value ? "✅" : "❌";
        }
    }
}
