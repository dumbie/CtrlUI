using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static ArnoldVinkCode.AVFunctions;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Filter file name
        public string FilterNameFile(string nameFile)
        {
            try
            {
                //Remove invalid characters
                nameFile = string.Join(string.Empty, nameFile.Split(Path.GetInvalidFileNameChars()));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed filtering file name: " + ex.Message);
            }
            return nameFile;
        }

        //Filter rom name
        public string FilterNameRom(string nameFile, bool removeExtension, bool removeSpaces, bool removeConsole, int takeWords)
        {
            try
            {
                //Remove invalid characters
                nameFile = FilterNameFile(nameFile);

                //Remove unicode characters
                nameFile = StringRemoveUnicode(nameFile);

                //Remove file extension
                if (removeExtension)
                {
                    nameFile = Path.GetFileNameWithoutExtension(nameFile);
                }

                //Lowercase the rom name
                nameFile = nameFile.ToLower();

                //Remove symbols with text
                nameFile = Regex.Replace(nameFile, @"\((.*?)\)+", string.Empty);
                nameFile = Regex.Replace(nameFile, @"\{(.*?)\}+", string.Empty);
                nameFile = Regex.Replace(nameFile, @"\[(.*?)\]+", string.Empty);

                //Replace characters
                nameFile = nameFile.Replace("'", " ").Replace(".", " ").Replace(",", " ").Replace("-", " ").Replace("_", " ");

                //Remove disc and number
                nameFile = Regex.Replace(nameFile, @"disc\s?\d+", string.Empty);

                //Remove multi and number
                nameFile = Regex.Replace(nameFile, @"multi\s?\d+", string.Empty);

                //Remove whole words
                string[] nameFilterRemove = { "usa", "eur", "pal", "ntsc", "repack", "proper", "ps2dvd", "psn" };
                foreach (string replaceString in nameFilterRemove)
                {
                    nameFile = Regex.Replace(nameFile, @"\b" + replaceString + @"\b", string.Empty);
                }

                if (removeConsole)
                {
                    IEnumerable<string> consoleSlugNames = vApiIGDBPlatforms.Select(x => x.slug).Where(x => !string.IsNullOrWhiteSpace(x));
                    foreach (string replaceString in consoleSlugNames)
                    {
                        nameFile = Regex.Replace(nameFile, @"\b" + replaceString + @"\b", string.Empty);
                    }
                }

                //Replace double spaces
                nameFile = Regex.Replace(nameFile, @"\s+", " ");

                //Split and take words 
                string[] nameRomSplit = nameFile.Split(' ');
                if (takeWords <= 0)
                {
                    nameFile = string.Join(" ", nameRomSplit);
                }
                else
                {
                    nameFile = string.Join(" ", nameRomSplit.Take(takeWords));
                }

                //Remove spaces
                if (removeSpaces)
                {
                    nameFile = nameFile.Replace(" ", string.Empty);
                }
                else
                {
                    nameFile = AVFunctions.StringRemoveStart(nameFile, " ");
                    nameFile = AVFunctions.StringRemoveEnd(nameFile, " ");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed filtering rom name: " + ex.Message);
            }
            return nameFile;
        }
    }
}