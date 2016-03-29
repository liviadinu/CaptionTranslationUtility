using System;
using System.IO;
using System.Text.RegularExpressions;
using CaptionTranslationUtility.TerminologyService;

namespace CaptionTranslationUtility
{
    class Program
    {
        public static string fromXLang { get; set; }
        public static string toXLang { get; set; }
        public static string responseX { get; set; }
        public static string outputfile { get; set; }
        static TextReader input = Console.In;

        static void Main(string[] args)
        {
            
            string[] messages = { "Choose an action :", "1. Load a dictionary .txt file", 
                                     "2. Translate captions from .txt file", "3. Translate single word " };
            Console.Out.NewLine = "\r\n\r\n ";
            foreach (string message in messages)
            {
                Console.WriteLine(message);
            }

            
             int caseSwitch = Convert.ToInt32(Console.ReadLine());
            //add exception for not converting to int

            // Create a collection to define the desired sources of translations 
            TranslationSources translationSources = new TranslationSources() { TranslationSource.Terms, TranslationSource.UiStrings };

            // Create a collection to define the desired products and versions from which to get the translations 
            Product nav = new Product() { Name = "Dynamics NAV" };
            nav.Versions = new Versions() { new TerminologyService.Version() { Name = "2009" },
                                            new TerminologyService.Version() { Name = "2009 R2" },
                                            new TerminologyService.Version() { Name = "2009 SP1" },
                                            new TerminologyService.Version() { Name = "2013" } };
            Products products = new Products() { nav };
            SearchStringComparison sensitivity = new SearchStringComparison();

            // Create the proxy for the Terminology Service SOAP client 
            TerminologyClient service = new TerminologyService.TerminologyClient();

            switch (caseSwitch)
            {
                case 1:

                    return;

                case 2:
                    {
                        Console.WriteLine("Input language code (ex: en-de, fr-ro):....");
                        Console.Out.NewLine ="\r\n\r\n";
                        string response = Console.ReadLine().ToLower();
                        TextProcessing textprocessor = new TextProcessing();
                        string fromLang = textprocessor.ConvertText(Regex.Replace(response, "-.(\\w)", ""));
                        string toLang = textprocessor.ConvertText(Regex.Replace(response, ".(\\w)-", ""));

                  
                        Console.WriteLine("Input text full path...");
                        try {
                          
                            string path = textprocessor.CleanInput(Console.ReadLine());
                            input = File.OpenText(path);
                            string directory = Path.GetDirectoryName(path);
                            string filename = Path.GetFileName(path);
                       
                            string root = Path.GetPathRoot(path);
                             outputfile = directory+ "\\Translated_" + filename;

                         

                        }

                        catch (FileNotFoundException)
                        {

                            throw;
                        }

                        catch (FileLoadException)
                        {
                            throw;
                        }
                        catch (ArgumentNullException)
                        {
                            throw;

                        }
                        catch (ArgumentException)
                        {
                            throw;

                        }

                        StreamWriter output = new StreamWriter(outputfile);
                        string streamline;
                 
                            while ((streamline = input.ReadLine()) != null)
                        {
                            if (!string.IsNullOrEmpty(streamline))
                            {
                                string key = textprocessor.GetCaptionKey(streamline);
                                string linewithoutkey = textprocessor.ReturnTextLineWithoutKey(streamline);
                                MSApiTranslation newcall = new MSApiTranslation();
                                string outputTransl = key + newcall.InitServiceCall(service, translationSources, sensitivity, linewithoutkey, fromLang, toLang, products);
                                if (!string.IsNullOrEmpty(outputTransl))
                                {
                                    output.WriteLine(outputTransl);

                                }
                                else
                                {
                                    string key2 = textprocessor.GetCaptionKey(streamline);
                                    string linewithoutkey2 = textprocessor.ReturnTextLineWithoutKey(streamline);
                                    string outputTransl2 = key2 + newcall.TranslateText(linewithoutkey2, response.Replace(" - ", "|"));
                                    if (!string.IsNullOrEmpty(outputTransl2))
                                    output.WriteLine(outputTransl2);
                                }
                            }
                        }
                        output.Close();
                        service.Close();
                        return;
                    }

                case 3:
                    {
                        Console.WriteLine("Input language code (ex: en-de, fr-ro):....");
                        responseX = Console.ReadLine().ToLower();

                        TextProcessing textprocessor = new TextProcessing();

                        fromXLang = textprocessor.ConvertText(System.Text.RegularExpressions.Regex.Replace(responseX, "-.(\\w)", ""));
                        toXLang = textprocessor.ConvertText(System.Text.RegularExpressions.Regex.Replace(responseX, ".(\\w)-", ""));

                        goto case 4;
                    }
                case 4:
                    {
                        Console.WriteLine("Type word/phrase:....\r");
                        string word = Console.ReadLine();

                        if (!string.IsNullOrEmpty(word))
                            if (!string.IsNullOrEmpty(word))
                            {
                                MSApiTranslation newcall = new MSApiTranslation();
                                string outputTransl = newcall.InitServiceCall(service, translationSources, sensitivity, word, fromXLang, toXLang, products);
                                if (!string.IsNullOrEmpty(outputTransl))
                                {
                                    Console.WriteLine(outputTransl);
                                    Console.ReadLine();
                                    goto case 4;
                                }
                                else
                                {
                                    string outputTransl2 = newcall.TranslateText(word, responseX.Replace("-", "|"));
                                    if (!string.IsNullOrEmpty(outputTransl2))
                                        Console.WriteLine(outputTransl2);
                                        Console.ReadLine();
                                    goto case 4;
                                }
                            }
                            else
                                Console.WriteLine("fail");
                        service.Close();
                        return;
                    }
            }
        }
    }
}
