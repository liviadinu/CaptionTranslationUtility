using CaptionTranslationUtility.Enums;
using System;

namespace CaptionTranslationUtility.Translator
{
    public class BaseTranslator : ITranslator
    {
        public TranslatorType TranslatorType { get; protected set; }

        public virtual string Translate(string text)
        {
            throw new NotImplementedException();
        }

        public virtual void Close() { }
    }
}
