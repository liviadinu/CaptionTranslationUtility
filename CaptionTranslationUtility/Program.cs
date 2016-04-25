using CaptionTranslationUtility.Base;
using CaptionTranslationUtility.Enums;
using CaptionTranslationUtility.Helpers;
using CaptionTranslationUtility.Translator;
using System;
using System.Collections.Generic;
using System.IO;

namespace CaptionTranslationUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            MenuOption selectedOption = default(MenuOption);
            while (selectedOption != MenuOption.Exit)
            {
                ShowMenu();
                selectedOption = ProcessSelectedOption();

            }
        }

        private static void ShowMenu()
        {
            List<string> messages = new List<string> {
                "Choose an action :",
                string.Format("{0}. Load a dictionary .txt file", (int)MenuOption.LoadDictionary),
                string.Format("{0}. Translate captions from .txt file", (int)MenuOption.TranslateFile),
                string.Format("{0}. Translate single word", (int)MenuOption.TranslateWord),
                string.Format("{0}. Exit", (int)MenuOption.Exit)
            };

            Console.Out.NewLine = "\r\n\r\n ";

            messages.ForEach(m => Console.WriteLine(m));
        }

        private static MenuOption ProcessSelectedOption()
        {
            var selectedOption = ReadSelectedOption();
            while (!Enum.IsDefined(typeof(MenuOption), selectedOption))
            {
                Console.WriteLine("Invalid option! Please select another option.");
                selectedOption = ReadSelectedOption();
            }

            var menuOption = (MenuOption)selectedOption;
            switch (menuOption)
            {
                case MenuOption.LoadDictionary:
                    LoadDictionary();
                    break;
                case MenuOption.TranslateFile:
                    Translate(TranslationEntity.File);
                    break;
                case MenuOption.TranslateWord:
                    Translate(TranslationEntity.Caption);
                    break;
                case MenuOption.Exit: break;
            }
            return menuOption;
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
            var languagePair = GetLanguagePair();
            var translator = new MasterTranslator(languagePair);

            switch (entity)
            {
                case TranslationEntity.Caption: TranslateCaption(translator); return;
                case TranslationEntity.File: TranslateCaptionsFromFile(translator); return;
            }

            translator.Close();
        }

        private static void LoadDictionary()
        {
            PrivateDictionary dictionary = new PrivateDictionary();
            dictionary.AddOrUpdateDictionary(ReadTranslationLanguage(), ReadInputFile());
        }

        private static void TranslateCaptionsFromFile(MasterTranslator translator)
        {
            string inputFilePath = ReadInputFile();
            string outputFilePath = GetOutputFilePath(inputFilePath, translator.LanguagePair);

            translator.TranslateFile(inputFilePath, outputFilePath);

            //Process.Start(outputFilePath);
        }

        private static void TranslateCaption(MasterTranslator translator)
        {
            if (translator == null) return;

            var caption = ReadCaption();
            if (string.IsNullOrWhiteSpace(caption)) return;

            var captionTranslation = translator.TranslateParallelized(caption);
            if (!string.IsNullOrWhiteSpace(captionTranslation))
                Console.WriteLine(captionTranslation);

            TranslateCaption(translator);
        }

        private static LanguagePair GetLanguagePair()
        {
            var languagePair = default(LanguagePair);
            var isRead = false;

            while (!isRead || !languagePair.IsValid)
            {
                if (isRead)
                    Console.WriteLine("Type as valid language code pair");

                var languageCode = ReadTranslationLanguage();
                isRead = true;
                languagePair = new LanguagePair(languageCode);
            }
            return languagePair;
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
            var filePath = Console.ReadLine();
            string inputFilePath = filePath.Clean(new List<string> { "\"" }, "");
            return inputFilePath;
        }

        private static string GetOutputFilePath(string inputFilePath, LanguagePair languagePair)
        {
            string directory = Path.GetDirectoryName(inputFilePath);
            string filename = Path.GetFileNameWithoutExtension(inputFilePath);
            string extension = Path.GetExtension(inputFilePath);

            var outputFilePath = Path.Combine(directory, string.Format("{0}_{1}_{2}{3}", filename, languagePair.GetLanguagePair(), DateTime.Now.ToString("ddmmyy_HHMM"), extension));
            return outputFilePath;
        }

    }
}
