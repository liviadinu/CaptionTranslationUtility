using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CaptionTranslationUtility
{
    public static class TextProcessing
    {
        private static readonly string pattern = @"(\w\d)*.{1,}-(\w\d)*.(-L999)\:";

        public static  KeyValuePair<string, string> GetCaption(string line)
        {
            KeyValuePair<string, string> result = default(KeyValuePair<string, string>);
            if (!Regex.IsMatch(line, pattern)) return result;

            var key = Regex.Match(line, pattern).Value;
            var value = line.Replace(key, "");
            value = value.Replace("&", "").Replace("\"", "");
            result = new KeyValuePair<string, string>(key, value);
            return result;
        }

        public static string CleanInput(string input)
        {
            var cleanedInput = input.Replace("\"", "");
            return cleanedInput;
        }

        public static string GetLanguageCode(string languageCode)
        {
            if (string.IsNullOrWhiteSpace(languageCode))
                throw new Exception("Null strings are not accepted!");

            languageCode += string.Format("{0}{1}", "-", (languageCode == "en" ? "us" : languageCode));

            return languageCode;
        }
    }
}
