using ArnoldVinkCode;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static CtrlUI.Classes;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task AsobimoScanAddLibrary()
        {
            try
            {
                //Fix get proper application names
                //https://asobimo-launcher.com/home
                //https://asobimo-game-launcher.akamaized.net/asobimo_games_launcher_pc/Release_Abcw0C/LauncherConfigData.json 

                //Get launcher path
                string launcherPath = string.Empty;
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey regKeyUninstall = registryKeyCurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{E631D5D7-36CA-40EB-8577-ED49F33B94B7}_is1"))
                    {
                        if (regKeyUninstall != null)
                        {
                            launcherPath = regKeyUninstall.GetValue("InstallLocation").ToString();
                        }
                    }
                }

                //Get app json path
                string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData\\LocalLow");
                string playerLogPathOriginal = Path.Combine(appDataPath, "Asobimo,Inc\\ASOBIMOLauncher\\Player.log");

                //Make copy of player log to avoid read issues
                string playerLogPathCopy = Path.Combine(appDataPath, "Asobimo,Inc\\ASOBIMOLauncher\\Player-copy.log");
                AVFiles.File_Copy(playerLogPathOriginal, playerLogPathCopy, true);

                //Open the player log file
                string playerLogString = File.ReadAllText(playerLogPathCopy);

                //Extract json from player log
                Match regex = Regex.Matches(playerLogString, @"(json:)(.*?)(\(Filename:)", RegexOptions.Singleline).FirstOrDefault();
                string asobimoJson = regex.Groups[2].ToString();
                AsobimoApps asobimoDeserial = JsonConvert.DeserializeObject<AsobimoApps>(asobimoJson);

                //Add applications from json
                foreach (AsobimoTitleData title_data in asobimoDeserial.title_data)
                {
                    try
                    {
                        string appName = title_data.internalgamefoldername;
                        string executablePath = Path.Combine(launcherPath, title_data.internalgamefoldername, title_data.fullexefilename);
                        string executableArguments = "-launcherPassword hM8wDGiX";
                        if (File.Exists(executablePath))
                        {
                            await AsobimoAddApplication(appName, executablePath, executableArguments);
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding Asobimo library: " + ex.Message);
            }
        }

        async Task AsobimoAddApplication(string appName, string executablePath, string executableArguments)
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

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, executablePath, "Asobimo" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Asobimo,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = executablePath,
                    Argument = executableArguments,
                    StatusLauncherImage = vImagePreloadAsobimo
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Asobimo app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Asobimo app: " + appName);
            }
        }
    }
}