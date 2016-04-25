using CaptionTranslationUtility.Base;
using CaptionTranslationUtility.Enums;
using CaptionTranslationUtility.TerminologyService;
using System;
using System.Linq;

namespace CaptionTranslationUtility.Translator
{
    class MSApiTranslator : BaseTranslator
    {
        private TerminologyClient service;
        private TranslationSources translationSources;
        private SearchStringComparison sensitivity;
        Products products;

        LanguagePair languagePair;

        public MSApiTranslator(LanguagePair languagePair)
        {
            // Create a collection to define the desired products and versions from which to get the translations 
            var nav = new Product() { Name = "Dynamics NAV" };
            nav.Versions = new Versions() { new TerminologyService.Version() { Name = "2009" },
                                            new TerminologyService.Version() { Name = "2009 R2" },
                                            new TerminologyService.Version() { Name = "2009 SP1" },
                                            new TerminologyService.Version() { Name = "2013" } };
            products = new Products() { nav };

            TranslatorType = TranslatorType.MSApi;
            // Create a collection to define the desired sources of translations 
            translationSources = new TranslationSources() { TranslationSource.Terms, TranslationSource.UiStrings };

            sensitivity = new SearchStringComparison();

            // Create the proxy for the Terminology Service SOAP client 
            service = new TerminologyClient();

            this.languagePair = languagePair;
        }

        public override string Translate(string text)
        {
            var translation = GetTerminologyClientTranslation(text, products, SearchOperator.Exact);

            if (string.IsNullOrWhiteSpace(translation))
            {
                translation = GetTerminologyClientTranslation(text, products, SearchOperator.Contains);
            }

            return translation;
        }

        public override void Close()
        {
            base.Close();
            service.Close();
        }
        
        private string GetTerminologyClientTranslation(string line, Products products, SearchOperator searchOperator)
        {
            string translation = null;
            try
            {
                Matches results = service.GetTranslations(line, languagePair.SourceCode, languagePair.TargetCode, sensitivity, searchOperator, translationSources, false, 1, true, products);
                if (results.Count > 0)
                    translation = GetFirstMatchTranslatedText(results);
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                Console.WriteLine("There was no endpoint listening at http://api.terminology.microsoft.com/Terminology.svc that could accept the message. This is often caused by internet connection problems, or an incorrect address/ SOAP action.");
                Console.ReadLine();
                return null;
            }

            return translation;
        }

        private string GetFirstMatchTranslatedText(Matches matches)
        {
            string firstTranslatedText = null;
            var firstMatchTranslations = matches.Select(r => r.Translations).FirstOrDefault();
            if (firstMatchTranslations != default(Translations))
            {
                firstTranslatedText = firstMatchTranslations.Select(t => t.TranslatedText).FirstOrDefault();
            }
            return firstTranslatedText;
        }
    }
}
