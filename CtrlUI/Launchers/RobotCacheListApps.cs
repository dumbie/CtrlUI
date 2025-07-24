using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static CtrlUI.Classes;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task RobotCacheScanAddLibrary()
        {
            try
            {
                //Get app json path
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string jsonPath = Path.Combine(appDataPath, "RobotCache\\RobotCacheClient\\config\\appConfig.json");

                //Load config from json
                string configJson = File.ReadAllText(jsonPath);
                RobotCacheConfig configDeserial = JsonConvert.DeserializeObject<RobotCacheConfig>(configJson);

                //Search game json and icon files
                List<string> iconFiles = new List<string>();
                List<string> jsonFiles = new List<string>();
                foreach (string libraryPath in configDeserial.libraries)
                {
                    try
                    {
                        //Search game json
                        string rcDataPathGames = libraryPath + "\\rcdata\\games";
                        string[] foundJson = Directory.GetFiles(rcDataPathGames, "installScript.json", SearchOption.AllDirectories);
                        jsonFiles.AddRange(foundJson);

                        //Search game icon
                        string rcDataPathIcons = libraryPath + "\\rcdata\\icons";
                        string[] foundIcon = Directory.GetFiles(rcDataPathIcons, "*.ico", SearchOption.AllDirectories);
                        iconFiles.AddRange(foundIcon);
                    }
                    catch { }
                }

                //Add applications
                foreach (string jsonFile in jsonFiles)
                {
                    try
                    {
                        string appJson = File.ReadAllText(jsonFile);
                        RobotCacheApp appDeserial = JsonConvert.DeserializeObject<RobotCacheApp>(appJson);
                        RobotCacheExeInfo exeInfo = appDeserial.exeInfos.FirstOrDefault();

                        string appName = exeInfo.title;
                        string appId = exeInfo.depotInfo.GameId.ToString();

                        string appIcon = iconFiles.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).StartsWith(appId + "-"));
                        //Fix icon is not available until you manually create a shortcut

                        string runCommand = "robotcache://rungameid/" + appId;
                        await RobotCacheAddApplication(appName, appIcon, runCommand);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding RobotCache library: " + ex.Message);
            }
        }

        async Task RobotCacheAddApplication(string appName, string appIcon, string runCommand)
        {
            try
            {
                //Add application to check list
                vLauncherAppAvailableCheck.Add(runCommand);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == runCommand.ToLower());
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

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, appIcon, "RobotCache" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.RobotCache,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncherImage = vImagePreloadRobotCache
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added RobotCache app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding RobotCache app: " + appName);
            }
        }
    }
}