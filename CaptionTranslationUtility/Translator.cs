using CaptionTranslationUtility.TerminologyService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CaptionTranslationUtility
{
    public class Translator
    {
        private MSApiTranslation apiTranslation;
        private PrivateDictionary dictionaryTranslation;

        private Products products;
        private static string fromXLang;
        private static string toXLang;
        private static string languagePair;

        public Translator(string languageCode)
        {
            // Create a collection to define the desired products and versions from which to get the translations 
            Product nav = new Product() { Name = "Dynamics NAV" };
            nav.Versions = new Versions() { new TerminologyService.Version() { Name = "2009" },
                                            new TerminologyService.Version() { Name = "2009 R2" },
                                            new TerminologyService.Version() { Name = "2009 SP1" },
                                            new TerminologyService.Version() { Name = "2013" } };
            products = new Products() { nav };
            apiTranslation = new MSApiTranslation();
            dictionaryTranslation = new PrivateDictionary();

            InitializeLanguages(languageCode);
        }

        private void InitializeLanguages(string languageCode)
        {
            languagePair = languageCode.Replace(" - ", "|");

            fromXLang = TextProcessing.GetLanguageCode(Regex.Replace(languageCode, "-.(\\w)", ""));
            toXLang = TextProcessing.GetLanguageCode(Regex.Replace(languageCode, ".(\\w)-", ""));
        }

        public string Translate(string caption)
        {
            if (string.IsNullOrWhiteSpace(caption)) return caption;

            string outputTranslation = dictionaryTranslation.ReturnDictionaryTranslation(dictionaryTranslation.LoadResourceFileContentInHashtable(dictionaryTranslation.ReturnResourcePath(languagePair)), caption);
            if (string.IsNullOrEmpty(outputTranslation))
            {
                string outputTranslation2 = apiTranslation.InitServiceCall(caption, fromXLang, toXLang, products);

                if (string.IsNullOrEmpty(outputTranslation2))
                {
                    outputTranslation2 = apiTranslation.TranslateText(caption, languagePair);
                    outputTranslation = outputTranslation2;
                }
                else
                {
                    outputTranslation = outputTranslation2;
                }
            }
            return outputTranslation;
        }

        public void TranslateCaptionsFromFile(string inputFilePath, string outputFilePath)
        {
            var input = Console.In;
            IEnumerable<string> fileLines;
            try
            {
                fileLines = File.ReadLines(inputFilePath, Encoding.Default);
            }
            catch (FileNotFoundException) { throw; }
            catch (FileLoadException) { throw; }
            catch (ArgumentNullException) { throw; }
            catch (ArgumentException) { throw; }

            var output = new StreamWriter(outputFilePath);

            foreach (var line in fileLines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var keyCaption = TextProcessing.GetCaption(line);
                    var translation = keyCaption.Key + Translate(keyCaption.Value);
                    output.WriteLine(translation);
                }
            }
            output.Close();
        }

        public void CloseTerminologyClientService()
        {
            apiTranslation.CloseTerminologyClientService();
        }
    }
}
