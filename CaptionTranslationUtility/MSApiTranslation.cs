using System;
using System.Collections.Generic;
using System.Net;
using CaptionTranslationUtility.TerminologyService;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptionTranslationUtility
{
    class MSApiTranslation
    {
        public string TranslateText(string input, string languagePair)
        {
            // Summary : Translate Text using Google Translate API's
            // Google URL - http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}
            // </summary>
            // <param name="input">Input string</param>
            // <param name="languagePair">2 letter Language Pair, delimited by "|".
            // E.g. "ar|en" language pair means to translate from Arabic to English</param>
            string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input, languagePair);
            WebClient webClient = new WebClient();
            webClient.Encoding = System.Text.Encoding.UTF8;

            string result = webClient.DownloadString(url);//this throws 503 server unavailable at times
            string pattern1 = "<" + "\\s*" + "\\/?\\s*span title=" + '"' + input + '"' + "\\s*.*?>(<)";
            string pattern2 = ">(.*?)<";
            try
            {
                string parseresult = System.Text.RegularExpressions.Regex.Match(result, pattern1).Value;
                string finalresult = System.Text.RegularExpressions.Regex.Match(parseresult, pattern2).Value;
                finalresult = finalresult.Replace("<", "").Replace(">", "");
                return finalresult;
            }
            catch (Exception)
            {
                return "failed processing";
            }

        }

        public string InitServiceCall(TerminologyClient service, TranslationSources translationSources, SearchStringComparison sensitivity, string line, string fromLang, string toLang, Products products)
        {
        

            try
            {
                Matches results = service.GetTranslations(line, fromLang, toLang, sensitivity, SearchOperator.Exact, translationSources, false, 1, true, products);

                if (results.Count > 0)
                {
                    // Use the results 
                    foreach (Match match in results)
                    {
                        foreach (Translation translation in match.Translations)
                        {
                            Console.WriteLine("{0}: {1}", translation.Language, translation.TranslatedText);
                            return translation.TranslatedText;
                        }
                    }
                }
                else
                {   //aprox match
                    Matches results2 = service.GetTranslations(line, fromLang, toLang, sensitivity, SearchOperator.Contains, translationSources, false, 1, true, products);
                    if (results2.Count > 0)
                    {
                        // Use the results 
                        foreach (Match match in results2)
                        {
                            foreach (Translation translation in match.Translations)
                            {
                                Console.WriteLine("{0}: {1}", translation.Language, translation.TranslatedText);
                                return translation.TranslatedText;
                            }
                        }
                    }
                    else
                        return null;
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                Console.WriteLine("There was no endpoint listening at http://api.terminology.microsoft.com/Terminology.svc that could accept the message. This is often caused by internet connection problems, or an incorrect address/ SOAP action.");
                Console.ReadLine();
                return null;
            }

            return null;
        }
    }
}
