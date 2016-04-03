using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;

namespace CaptionTranslationUtility
{
    class PrivateDictionary
    {
        public void AddOrUpdateDictionary(string langCodeValue, string filePathValue)
        {

            string resourceFilePath = ReturnResourcePath(langCodeValue);
            if (File.Exists(resourceFilePath))
            {
                CreateNewResourceFile("temp", filePathValue, false);
                string tempResourceFilePath = ReturnResourcePath("temp");
                UpdateResourceFile(LoadResourceFileContentInHashtable(tempResourceFilePath), resourceFilePath);
                File.Delete(tempResourceFilePath);
            }
            else
            {
                CreateNewResourceFile(langCodeValue, filePathValue, true);
            }           
        }
        private void CreateNewResourceFile(string langCode, string filePath, bool showMessage)
        {
            string resourceName = ReturnResourcePath(langCode);

            IEnumerable<string> fileLines;
            try
            {
                fileLines = File.ReadLines(filePath, Encoding.Default);
            }
            catch (FileNotFoundException) { throw; }
            catch (FileLoadException) { throw; }
            catch (ArgumentNullException) { throw; }
            catch (ArgumentException) { throw; }
           
            using (ResXResourceWriter resourceWriter = new ResXResourceWriter(resourceName))
            {
                foreach (var line in fileLines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        try
                        {
                            // Create an IDictionaryEnumerator to read the data in the ResourceSet.
                            if (!line.Contains('='))
                            {
                                Console.WriteLine("Text input {0} is incorrectly formated. Your text needs to contain a '=' sign!", line);
                                continue;
                            }
                            else
                            {
                                string[] wordPair = line.Split('=');
                                string englishWord = wordPair.ElementAt(0).TrimEnd();
                                string foreignWord = wordPair.ElementAt(1).TrimStart();
                                resourceWriter.AddResource(englishWord, foreignWord);
                            }                       
                        }

                        //if the file path is wrong or dosn't found
                        catch (FileNotFoundException caught)
                        {
                            Console.WriteLine("Source: " + caught.Source + " Message: " + caught.Message);
                        }
                      }
                  }
                resourceWriter.Close();
            }

            if (showMessage)
            {
                DisplayFullResult(LoadResourceFileContentInHashtable(resourceName));
            }
   }      
        private void UpdateResourceFile(Hashtable data, String path)
    {
        Hashtable resourceEntries = new Hashtable();

        //Get existing resources
        ResXResourceReader reader = new ResXResourceReader(path);
        ResXResourceWriter resourceWriter = new ResXResourceWriter(path);

        if (reader != null)
        {
            IDictionaryEnumerator id = reader.GetEnumerator();
            foreach (DictionaryEntry d in reader)
            {
                //Read from file:
                string val = "";
                if (d.Value == null)
                    resourceEntries.Add(d.Key.ToString(), "");
                else
                {
                    resourceEntries.Add(d.Key.ToString(), d.Value.ToString());
                    val = d.Value.ToString();
                }
                //Write (with read to keep xml file order)
                resourceWriter.AddResource(d.Key.ToString(), val);
            }
            reader.Close();
        }

        //Add new data (at the end of the file):
        Hashtable newRes = new Hashtable();
        foreach (String key in data.Keys)
        {
            if (!resourceEntries.ContainsKey(key))
            {
                String value = data[key].ToString();
                if (value == null) value = "";
                resourceWriter.AddResource(key, value);
            }
        }

        //Write to file
        resourceWriter.Generate();
        resourceWriter.Close();
        DisplayFullResult(LoadResourceFileContentInHashtable(path));

    }
        public Hashtable LoadResourceFileContentInHashtable(string resourcePath)
        {
            var resourceEntries = new Hashtable();
            var reader = new ResXResourceReader(resourcePath);
            foreach (DictionaryEntry d in reader)
            {
                resourceEntries.Add(d.Key.ToString(), d.Value == null ? "" : d.Value.ToString());
            }
            reader.Close();
            return resourceEntries;
        }
        private void DisplayFullResult(Hashtable resourceEntries)
        {
            foreach (DictionaryEntry entry in resourceEntries)
            {
                    Console.WriteLine("{0}, {1}", entry.Key, entry.Value);
            
            }

        }
        public string ReturnDictionaryTranslation(Hashtable dictionaryData, string index)
        {
            if (dictionaryData.ContainsKey(index))
            {
                return dictionaryData.Values.ToString();
            }
            else return "";
        }
        public string ReturnResourcePath(string code)
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                string resourceName = @".\" + code + @".resx";
                return resourceName;
            }
            else
            {
                Console.WriteLine("Null is not accepted."); ;
            }
            return "";
        }
    }
}
