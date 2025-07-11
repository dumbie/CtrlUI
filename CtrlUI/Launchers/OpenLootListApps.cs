using ArnoldVinkCode;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVJsonFunctions;
using static CtrlUI.AppVariables;
using static CtrlUI.Classes;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task OpenLootScanAddLibrary()
        {
            try
            {
                //Fix find way to get proper application names

                //Get launcher paths
                string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                string settingsPath = Path.Combine(programFilesPath, "Open Loot\\Open Loot Launcher\\settings");

                //Read settings json file
                string settingsString = File.ReadAllText(settingsPath);
                OpenLootSettings settingsJson = JsonConvert.DeserializeObject<OpenLootSettings>(settingsString);

                //Get all game install paths
                List<JToken> jsonTokens = new List<JToken>();
                JsonFindTokens("dirPath", settingsJson.apps, ref jsonTokens);
                foreach (JToken jsonToken in jsonTokens)
                {
                    try
                    {
                        //Get app data path
                        string appRootPath = jsonToken.ToString();
                        string appDataPath = Path.Combine(appRootPath, "app_data.json");

                        //Read game metadata json file
                        string appDataString = File.ReadAllText(appDataPath);
                        OpenLootAppData appDataJson = JsonConvert.DeserializeObject<OpenLootAppData>(appDataString);

                        //Check if game is installed
                        if (appDataJson.is_installed)
                        {
                            //Get game details
                            string appName = Path.GetFileNameWithoutExtension(appDataJson.start_exe_path);
                            appName = AVFunctions.StringRemoveStart(appName, "EAC");
                            string executablePath = Path.Combine(appRootPath, appDataJson.start_exe_path);
                            string executableArgument = appDataJson.start_exe_args;

                            //Add application to the list
                            await OpenLootAddApplication(appName, executablePath, executablePath, executableArgument);
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding Open Loot library: " + ex.Message);
            }
        }

        async Task OpenLootAddApplication(string appName, string appImage, string executablePath, string executableArgument)
        {
            try
            {
                //Add application to check list
                vLauncherAppAvailableCheck.Add(executablePath);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == executablePath.ToLower());
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Launcher app already in list: " + appName);
                    return;
                }

                //Check if application name is ignored
                string appNameLower = appName.ToLower();
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher app is on the blacklist: " + appName);
                    return;
                }

                //Load application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, appImage, "Open Loot" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.OpenLoot,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = executablePath,
                    Argument = executableArgument,
                    StatusLauncherImage = vImagePreloadOpenLoot
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added OpenLoot game: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Open Loot game: " + appName);
            }
        }
    }
}