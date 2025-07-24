using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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
        async Task ParadoxScanAddLibrary()
        {
            try
            {
                //Get launcher paths
                string localDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string roamingDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string gameMetadataPath = Path.Combine(roamingDataPath, "Paradox Interactive\\launcher-v2\\game-metadata\\game-metadata");
                string userSettingsPath = Path.Combine(roamingDataPath, "Paradox Interactive\\launcher-v2\\userSettings.json");

                //Read user settings json file
                string userSettingsString = File.ReadAllText(userSettingsPath);
                ParadoxUserSettings userSettingsJson = JsonConvert.DeserializeObject<ParadoxUserSettings>(userSettingsString);

                //Read game metadata json file
                string gameMetadataString = File.ReadAllText(gameMetadataPath);
                ParadoxGameMetadata gameMetadataJson = JsonConvert.DeserializeObject<ParadoxGameMetadata>(gameMetadataString);

                //Add applications from library paths
                foreach (JToken jtoken in userSettingsJson.gameLibraryPaths)
                {
                    try
                    {
                        if (jtoken.GetType() == typeof(JObject))
                        {
                            //Convert jtoken to object
                            ParadoxGameLibraryPath gameLibrary = jtoken.ToObject<ParadoxGameLibraryPath>();

                            //Get matching meta data
                            ParadoxGameMetadataGame gameMeta = gameMetadataJson.data.games.FirstOrDefault(x => x.id == gameLibrary.gameId);

                            //Read launcher settings json file
                            string launcherSettingsPath = Path.Combine(gameLibrary.launcherSettingsDirPath, "launcher-settings.json");
                            string launcherSettingsString = File.ReadAllText(launcherSettingsPath);
                            ParadoxLauncherSettings launcherSettingsJson = JsonConvert.DeserializeObject<ParadoxLauncherSettings>(launcherSettingsString);

                            //Direct arguments
                            string appName = gameMeta.name;
                            string executableName = launcherSettingsJson.exePath;
                            string executablePath = Path.Combine(gameLibrary.launcherSettingsDirPath, executableName);
                            string executableArgument = string.Empty;
                            if (launcherSettingsJson.exeArgs != null)
                            {
                                executableArgument = string.Join(" ", launcherSettingsJson.exeArgs);
                            }

                            //Launcher arguments
                            //string executablePath = Path.Combine(localDataPath, "Programs\\Paradox Interactive\\launcher\\bootstrapper-v2.exe");
                            //string executableArgument = "--pdxlGameDir \"" + gameLibrary.launcherSettingsDirPath+ "\" --gameDir \"" + gameLibrary.launcherSettingsDirPath + "\"";

                            //Note: some games need authentication so they currently don't run even when using the launcher method.

                            //Add application to the list
                            await ParadoxAddApplication(appName, executablePath, executablePath, executableArgument);
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding Paradox library: " + ex.Message);
            }
        }

        async Task ParadoxAddApplication(string appName, string appImage, string executablePath, string executableArgument)
        {
            try
            {
                //Add application to check list
                vLauncherAppAvailableCheck.Add(executablePath);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == executablePath.ToLower() && x.Argument == executableArgument.ToLower());
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
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, appImage, "Paradox" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Paradox,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = executablePath,
                    Argument = executableArgument,
                    StatusLauncherImage = vImagePreloadParadox
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Paradox game: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Paradox game: " + appName);
            }
        }
    }
}