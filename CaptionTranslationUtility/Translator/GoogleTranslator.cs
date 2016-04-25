using CaptionTranslationUtility.Base;
using CaptionTranslationUtility.Enums;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace CaptionTranslationUtility.Translator
{
    class GoogleTranslator : BaseTranslator
    {
        /// <summary>
        /// 2 letter Language Pair, delimited by "|". e.g. "ar|en" language pair means to translate from Arabic to English
        /// </summary>
        private string languagePairValue;

        private static readonly string GOOGLE_TRANSLATE_URL_PATTERN = "http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}";

        public GoogleTranslator(LanguagePair languagePair)
        {
            TranslatorType = TranslatorType.Googgle;
            languagePairValue = languagePair.GetLanguagePair("|");
        }

        public override string Translate(string input)
        {
            var translation = string.Empty;

            string url = string.Format(GOOGLE_TRANSLATE_URL_PATTERN, input, languagePairValue);
            try
            {
                var webClient = new WebClient() { Encoding = System.Text.Encoding.Default };
                webClient.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";

                string result = webClient.DownloadString(url);
                string spanPattern = string.Format("<\\s*\\/?\\s*span title=\"{0}\"\\s*.*?>(<)", input);
                string translationPattern = ">(.*?)<";

                var spanMatch = Regex.Match(result, spanPattern);
                if (!spanMatch.Success) return translation;

                var translationMatch = Regex.Match(spanMatch.Value, translationPattern);
                if (!translationMatch.Success) return translation;
                translation = Regex.Replace(translationMatch.Value, "[<|>]", "");
            }
            catch (WebException ex)
            {
                var s = new StreamReader(ex.Response.GetResponseStream());
                var html = s.ReadToEnd();
                Console.WriteLine("{0},{1}", "GOOGLE WebException", "Failed Processing");
            }
            catch (Exception)
            {
                Console.WriteLine("{0},{1}", "GOOGLE", "Failed Processing");
            }
            return translation;
        }
    }
}