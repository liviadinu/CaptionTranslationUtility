using System.Text.RegularExpressions;

namespace CaptionTranslationUtility.Base
{    
    public class LanguagePair
    {        
        public string Source { get; private set; }
        public string SourceCode { get; private set; }
        public string Target { get; private set; }
        public string TargetCode { get; private set; }
        public bool IsValid { get; private set; }

        private string validatorPattern = @"^[a-zA-Z]{2}[-|\|][a-zA-Z]{2}$";
        private string languagePattern = "[a-zA-Z]{2}";

        public const string LANGUAGE_PAIR_SEPARATOR = "-";

        public LanguagePair(string languagePair)
        {
            Validate(languagePair);

            if (!IsValid) return;

            var languageMatches = Regex.Matches(languagePair, languagePattern);
            Source = languageMatches[0].Value;
            Target = languageMatches[1].Value;

            SourceCode = GetLanguageCode(Source);
            TargetCode = GetLanguageCode(Target);
        }

        public string GetLanguagePair(string delimiter = LANGUAGE_PAIR_SEPARATOR)
        {
            return IsValid ? string.Format("{0}{1}{2}", Source, delimiter, Target) : string.Empty;
        }

        private void Validate(string languagePair)
        {
            IsValid = !string.IsNullOrWhiteSpace(languagePair) && Regex.IsMatch(languagePair, validatorPattern);
        }

        private string GetLanguageCode(string language)
        {
            var languageCode = string.Format("{0}{1}{2}", language, "-", (language == "en" ? "us" : language));
            return languageCode;
        }
    }
}
