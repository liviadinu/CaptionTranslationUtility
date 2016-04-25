using CaptionTranslationUtility.Base;
using CaptionTranslationUtility.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptionTranslationUtility.Translator
{
    public class MasterTranslator
    {
        public LanguagePair LanguagePair { get; private set; }

        private List<BaseTranslator> translators = new List<BaseTranslator>();

        public MasterTranslator(LanguagePair languagePair)
        {
            LanguagePair = languagePair;
            translators = new List<BaseTranslator>() {
                new MSApiTranslator(languagePair),
                new GoogleTranslator(languagePair)
            };
        }

        public string TranslateParallelized(string caption)
        {
            if (string.IsNullOrWhiteSpace(caption)) return caption;
            Dictionary<int, string> results = new Dictionary<int, string>();

            Parallel.ForEach(translators, t =>
            {
                var translation = t.Translate(caption);
                if (!results.ContainsKey((int)t.TranslatorType))
                    results.Add((int)t.TranslatorType, translation);
                else
                    results[(int)t.TranslatorType] = translation;
            });

            var firstTranslation = results.OrderBy(r => r.Key).FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.Value));

            var outputTranslation = firstTranslation.Equals(default(KeyValuePair<int, string>)) ? string.Empty : firstTranslation.Value;
            return outputTranslation;
        }

        public void TranslateFile(string inputFilePath, string outputFilePath)
        {
            var input = Console.In;
            List<string> fileLines;
            try
            {
                fileLines = File.ReadLines(inputFilePath, Encoding.Default).ToList();
            }
            catch (FileNotFoundException) { throw; }
            catch (FileLoadException) { throw; }
            catch (ArgumentNullException) { throw; }
            catch (ArgumentException) { throw; }

            var output = new StreamWriter(outputFilePath);
            output.WriteLine("Started at:" + DateTime.Now);

            var concurrentDictionary = new ConcurrentDictionary<int, string>(fileLines.Select((l, index) => new KeyValuePair<int, string>(index, l)));
            var step = 100;
            var length = fileLines.Count();
            int processingCount = length / step;

            for (var i = 0; i < processingCount; i++)
            {
                TranslateRange(fileLines, i * step, step, output);
            }

            TranslateRange(fileLines, processingCount * step, length % step, output);

            output.WriteLine("Finished at:" + DateTime.Now);
            output.Close();
        }

        public void Close()
        {
            translators.ForEach(t => t.Close());
        }

        private string TranslateCaptionWithKey(string line)
        {
            var translation = string.Empty;
            if (string.IsNullOrWhiteSpace(line)) return translation;

            var keyCaption = line.Caption();
            translation = keyCaption.Key + TranslateParallelized(keyCaption.Value);
            return translation;
        }

        private void TranslateRange(List<string> fileLines, int index, int count, StreamWriter output)
        {
            var processedLines = fileLines.GetRange(index, count);
            var processedDictionary = new ConcurrentDictionary<int, string>(processedLines.Select((l, i) => new KeyValuePair<int, string>(i, l)));

            Translate(processedDictionary, output);
        }

        private void Translate(ConcurrentDictionary<int, string> processedDictionary, StreamWriter output)
        {
            Parallel.ForEach(processedDictionary, p =>
            {
                var translatedLine = TranslateCaptionWithKey(p.Value);
                processedDictionary[p.Key] = translatedLine;

            });

            foreach (var item in processedDictionary)
            {
                output.WriteLine(item.Value);
            }
        }

    }
}
