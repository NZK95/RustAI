namespace RustAI
{
    internal static class Validators
    {
        public static bool IsIDValid(string id)
        {
            return long.TryParse(id, out _);
        }

    }
}
