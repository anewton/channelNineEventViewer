using System.Text;

namespace ChannelNineEventFeed.WPF.Extensions
{
    public static class StringExtensions
    {
        public static string ProperCaseWithSpaces(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            var result = new StringBuilder(value);
            result[0] = char.ToUpper(result[0]);
            for (int i = 1; i < result.Length; ++i)
            {
                if (char.IsWhiteSpace(result[i - 1]))
                {
                    result[i] = char.ToUpper(result[i]);
                }
                else
                {
                    result[i] = char.ToLower(result[i]);
                }
            }
            return result.ToString();
        }
    }
}
