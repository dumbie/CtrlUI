using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static ArnoldVinkCode.AVFunctions;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Get assets image file path
        public string GetAssetsImageFilePath(AppCategory appCategory, string appName, string fileExtension, bool defaultPath)
        {
            try
            {
                //Set root path
                string rootPath = defaultPath ? "Default" : "User";

                //Get file path
                string saveFilePath = string.Empty;
                if (appCategory == AppCategory.Emulator)
                {
                    saveFilePath = "Assets/" + rootPath + "/Emulators/";
                }
                else
                {
                    saveFilePath = "Assets/" + rootPath + "/Apps/";
                }

                //Get file name
                string saveFileName = AVFiles.FileNameReplaceInvalidChars(appName, string.Empty) + fileExtension;

                //Combine path and filename
                return Path.Combine(saveFilePath, saveFileName);
            }
            catch { }
            return string.Empty;
        }

        //Get assets image file path
        public string GetAssetsImageFilePath(DataBindApp dataBindApp, string fileExtension, bool defaultPath)
        {
            try
            {
                //Set root path
                string rootPath = defaultPath ? "Default" : "User";

                //Get file path
                string saveFilePath = string.Empty;
                if (dataBindApp.Category == AppCategory.Emulator)
                {
                    saveFilePath = "Assets/" + rootPath + "/Emulators/";
                }
                else
                {
                    saveFilePath = "Assets/" + rootPath + "/Apps/";
                }

                //Get file name
                string saveFileName = AVFiles.FileNameReplaceInvalidChars(dataBindApp.Name, string.Empty) + fileExtension;

                //Combine path and filename
                return Path.Combine(saveFilePath, saveFileName);
            }
            catch { }
            return string.Empty;
        }

        //Get assets image file path
        public string GetAssetsImageFilePath(DataBindFile dataBindFile, string fileExtension, bool defaultPath)
        {
            try
            {
                //Set root path
                string rootPath = defaultPath ? "Default" : "User";

                //Get file path
                string saveFilePath = "Assets/" + rootPath + "/Emulators/" + dataBindFile.NameDetail;

                //Get file name
                string saveFileName = FilterNameGame(dataBindFile.Name, true, false, 0) + fileExtension;

                //Combine path and filename
                return Path.Combine(saveFilePath, saveFileName);
            }
            catch { }
            return string.Empty;
        }

        //Filter game name
        public string FilterNameGame(string nameFile, bool removeExtension, bool removeSpaces, int takeWords)
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

                //Remove all dots
                nameFile = nameFile.Replace(".", string.Empty);

                //Replace all characters
                nameFile = Regex.Replace(nameFile, @"[^a-zA-Z0-9]", " ");

                //Remove disc and number
                nameFile = Regex.Replace(nameFile, @"disc\s?\d+", string.Empty);

                //Remove multi and number
                nameFile = Regex.Replace(nameFile, @"multi\s?\d+", string.Empty);

                //Remove whole words
                string[] nameFilterRemove = { "usa", "eur", "jpn", "pal", "ntsc", "repack", "proper", "ps2dvd", "psn", "nsw", "decrypted", "readnfo" };
                foreach (string replaceString in nameFilterRemove)
                {
                    nameFile = StringReplaceWholeWord(nameFile, replaceString, string.Empty);
                }

                //Replace double spaces
                nameFile = Regex.Replace(nameFile, @"\s+", " ");

                //Split and take words 
                if (takeWords > 0)
                {
                    string[] nameRomSplit = nameFile.Split(' ');
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