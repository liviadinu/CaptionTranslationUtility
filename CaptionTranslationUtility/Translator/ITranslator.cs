using CaptionTranslationUtility.Enums;

namespace CaptionTranslationUtility.Translator
{
    interface ITranslator
    {        
        string Translate(string text);
        void Close();
    }
}
