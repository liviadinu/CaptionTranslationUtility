using CaptionTranslationUtility.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace CaptionTranslationUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            ShowMenu();

            var selectedOption = ReadSelectedOption();
            switch (selectedOption)
            {
                case 1:
                    {
                        PrivateDictionary dictionary = new PrivateDictionary();                        
                        dictionary.AddOrUpdateDictionary(ReadTranslationLanguage(), ReadInputFile());
                    }
                    return;
                case 2:
                    Translate(TranslationEntity.File);
                    return;
                case 3:
                    Translate(TranslationEntity.Caption);
                    return;
                case 4: return;
            }
        }

        private static void ShowMenu()
        {
            List<string> messages = new List<string>{
                "Choose an action :",
                "1. Load a dictionary .txt file",
                "2. Translate captions from .txt file",
                "3. Translate single word ",
                "4. Exit",
            };

            Console.Out.NewLine = "\r\n\r\n ";

            messages.ForEach(m => Console.WriteLine(m));
        }

        private static int ReadSelectedOption()
        {
            int selectedOption;
            var option = Console.ReadLine();
            while (!int.TryParse(option, out selectedOption))
            {
                Console.WriteLine("Please provide a valid option!");
                option = Console.ReadLine();
            }
            return selectedOption;
        }

        private static void Translate(TranslationEntity entity)
        {
            var languageCode = ReadTranslationLanguage();
            var translator = new Translator(languageCode);

            switch (entity)
            {
                case TranslationEntity.Caption: TranslateCaption(translator); return;
                case TranslationEntity.File: TranslateCaptionsFromFile(translator); return;
            }

            translator.CloseTerminologyClientService();
        }

        private static void TranslateCaptionsFromFile(Translator translator)
        {
            string inputFilePath = ReadInputFile();
            string outputFilePath = GetOutputFilePath(inputFilePath);

            translator.TranslateCaptionsFromFile(inputFilePath, outputFilePath);

            //Process.Start(outputFilePath);
        }

        private static void TranslateCaption(Translator translator)
        {
            if (translator == null) return;

            var caption = ReadCaption();
            if (string.IsNullOrWhiteSpace(caption)) return;

            var captionTranslation = translator.Translate(caption);
            if (!string.IsNullOrWhiteSpace(captionTranslation))
                Console.WriteLine(captionTranslation);

            TranslateCaption(translator);
        }

        private static string ReadTranslationLanguage()
        {
            //Regex for language ^[a-z]{2}-[a-z]{2}$
            Console.WriteLine("Type language code (ex: en-de, fr-ro): ...");
            var languageCode = Console.ReadLine().ToLower();
            return languageCode;
        }

        private static string ReadCaption()
        {
            Console.WriteLine("Type caption: ...");
            string expression = Console.ReadLine();
            return expression;

        }

        private static string ReadInputFile()
        {
            Console.WriteLine("Input text full path...");
            string inputFilePath = TextProcessing.CleanInput(Console.ReadLine());
            return inputFilePath;
        }

        private static string GetOutputFilePath(string inputFilePath)
        {
            string directory = Path.GetDirectoryName(inputFilePath);
            string filename = Path.GetFileName(inputFilePath);

            var outputFilePath = string.Format("{0}\\Translated_{1}", directory, filename);
            return outputFilePath;
        }

    }
}
