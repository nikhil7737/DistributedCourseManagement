namespace CommandService.ExtensionMethods
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this string stringToCheck)
        {
            return stringToCheck == null || stringToCheck.Equals(string.Empty);
        }
    }
}