using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CaptionTranslationUtility.Helpers
{
    public static class TextProcessing
    {
        private static readonly string pattern = @"(\w\d)*.{1,}-(\w\d)*.(-L999)\:";

        public static KeyValuePair<string, string> Caption(this string line)
        {
            KeyValuePair<string, string> result = default(KeyValuePair<string, string>);
            if (!Regex.IsMatch(line, pattern)) return result;

            var key = Regex.Match(line, pattern).Value;
            var value = line.Clean(new List<string> { key, "&", "\"" }, "");
            result = new KeyValuePair<string, string>(key, value);
            return result;
        }

        public static string Clean(this string input, List<string> tokens, string replacement = "")
        {
            var cleanedInput = tokens.Aggregate(input, (item, token) => item.Replace(token, replacement));
            return cleanedInput;
        }

    }
}
