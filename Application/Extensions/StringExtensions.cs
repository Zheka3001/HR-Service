using System.Text.RegularExpressions;

namespace Application.Extensions
{
    public static class StringExtensions
    {
        public static string DefaultIfNullOrEmpty(this string value, string defaultValue)
        {
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        public static string RemoveExtraSpaces(this string value)
        {
            var regex = new Regex("[ ]{2,}", RegexOptions.None);
            return string.IsNullOrEmpty(value)
                ? value
                : regex.Replace(value, " ").Trim();
        }
    }
}
