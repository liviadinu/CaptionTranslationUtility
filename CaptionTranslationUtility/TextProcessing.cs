using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CaptionTranslationUtility
{
    class TextProcessing
    {
        public string GetCaptionKey(string textline)
        {
            string pattern = @"(\w\d)*.{ 1,}-(\w\d)*.(-L999)\:";
            if (System.Text.RegularExpressions.Regex.IsMatch(textline, pattern))
            {
                string newtextline = System.Text.RegularExpressions.Regex.Match(textline, pattern).Value;
                return newtextline;
            }
            else return "";
        }

        public string ReturnTextLineWithoutKey(string line)
        {
            string pattern = @"(\w\d)*.{ 1,}-(\w\d)*.(-L999)\:";
            if (System.Text.RegularExpressions.Regex.IsMatch(line, pattern))
            {
            string pattern2 = System.Text.RegularExpressions.Regex.Match(line, pattern).Value;
            string newline = line.Replace(pattern2 , "");
                newline = newline.Replace("&","").Replace("\"","");
                return newline;
            }
            else                        
            return line;
        }
        public string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                string newpth = strIn.Replace("\"", "");
                return @newpth ;
               // return @Regex.Replace(strIn, '"', "",
                //                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters, 
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }
        public string ConvertText(string langcode)
        {
            try
            {
                if (!string.IsNullOrEmpty(langcode))
                {
                    if (langcode == "en")
                        langcode = langcode + "-" + "us";
                    else langcode = langcode + "-" + langcode;

                    return langcode;
                }
                else
                {
                    throw new Exception("Null strings are not accepted!");
                }
            }
            catch (Exception e)
            {
                throw new Exception("{0}", e.InnerException);
            }
        }
    }
}
