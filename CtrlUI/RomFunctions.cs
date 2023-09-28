﻿using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using static ArnoldVinkCode.AVFunctions;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Filter game name
        public string FilterNameGame(string nameFile, bool removeExtension, bool removeSpaces, bool removePlatform, int takeWords)
        {
            try
            {
                //Remove invalid characters
                nameFile = AVFiles.FileNameReplaceInvalidChars(nameFile, string.Empty);

                //Remove unicode characters
                nameFile = StringRemoveUnicode(nameFile);

                //Remove file extension
                if (removeExtension)
                {
                    nameFile = AVFunctions.GetFileNameNoExtension(nameFile);
                }

                //Lowercase the rom name
                nameFile = nameFile.ToLower();

                //Remove symbols with text
                nameFile = Regex.Replace(nameFile, @"\((.*?)\)+", string.Empty);
                nameFile = Regex.Replace(nameFile, @"\{(.*?)\}+", string.Empty);
                nameFile = Regex.Replace(nameFile, @"\[(.*?)\]+", string.Empty);

                //Remove version and number
                nameFile = Regex.Replace(nameFile, @"v(\d+\.\d+)\s?", string.Empty);
                nameFile = Regex.Replace(nameFile, @"ver(\d+\.\d+)\s?", string.Empty);
                nameFile = Regex.Replace(nameFile, @"version(\d+\.\d+)\s?", string.Empty);

                //Replace all characters
                nameFile = Regex.Replace(nameFile, @"[^a-zA-Z0-9]", " ");

                //Remove disc and number
                nameFile = Regex.Replace(nameFile, @"disc\s?\d+", string.Empty);

                //Remove multi and number
                nameFile = Regex.Replace(nameFile, @"multi\s?\d+", string.Empty);

                //Remove whole words
                string[] nameFilterRemove = { "usa", "eur", "pal", "ntsc", "repack", "proper", "ps2dvd", "psn", "nsw", "decrypted" };
                foreach (string replaceString in nameFilterRemove)
                {
                    nameFile = Regex.Replace(nameFile, @"\b" + replaceString + @"\b", string.Empty);
                }

                //Remove platform names
                if (removePlatform)
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
                    nameFile = nameFile.Trim();
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