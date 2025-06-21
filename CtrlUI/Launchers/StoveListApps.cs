using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        public class StoveLauncher
        {
            public string defaultPath { get; set; }
            public List<string> defaultPathList { get; set; }
        }

        public partial class StoveApp
        {
            public string game_id { get; set; }
            public string install_path { get; set; }
            public string game_title { get; set; }
            public string shortcut_link_url { get; set; }
        }

        async Task StoveScanAddLibrary()
        {
            try
            {
                //Get app json path
                string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string jsonPath = Path.Combine(roamingPath, "STOVE\\Config\\ClientConfig.json");

                //Load applications from json
                string jsonString = File.ReadAllText(jsonPath);
                StoveLauncher installDeserial = JsonConvert.DeserializeObject<StoveLauncher>(jsonString);

                //Add applications from json path
                foreach (string installPath in installDeserial.defaultPathList)
                {
                    try
                    {
                        string[] infoFiles = Directory.GetFiles(installPath, "GameManifest*.upf", SearchOption.AllDirectories);
                        foreach (string infoFile in infoFiles)
                        {
                            try
                            {
                                string infoFileString = File.ReadAllText(infoFile);
                                StoveApp infoApplication = JsonConvert.DeserializeObject<StoveApp>(infoFileString);
                                string appName = infoApplication.game_title;
                                string runCommand = "sgup://run/" + infoApplication.game_id;
                                await StoveAddApplication(appName, runCommand);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Failed to deserialize Stove game: " + infoFile + " / " + ex.Message);
                            }
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding Stove library: " + ex.Message);
            }
        }

        async Task StoveAddApplication(string appName, string runCommand)
        {
            try
            {
                //Get launch argument
                vLauncherAppAvailableCheck.Add(runCommand);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == runCommand.ToLower());
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Stove app already in list: " + appName);
                    return;
                }

                //Check if application name is ignored
                string appNameLower = appName.ToLower();
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher is on the blacklist skipping: " + appName);
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, x => x.Name.ToLower() == appNameLower);
                    return;
                }

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, "Stove" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Epic,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncherImage = vImagePreloadEpic
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Stove app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Stove app: " + appName);
            }
        }
    }
}