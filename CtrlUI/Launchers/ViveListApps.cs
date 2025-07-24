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
        async Task ViveScanAddLibrary()
        {
            try
            {
                //Get app json path
                string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string jsonPath = Path.Combine(appdataPath, "HTC\\Viveport\\installed_apps.json");

                //Load applications from json
                string launcherInstalledJson = File.ReadAllText(jsonPath);
                List<ViveApps> installedDeserial = JsonConvert.DeserializeObject<List<ViveApps>>(launcherInstalledJson);

                //Add applications from json
                foreach (ViveApps appInstalled in installedDeserial)
                {
                    try
                    {
                        //Check if application id is in blacklist
                        if (vViveAppIdBlacklist.Contains(appInstalled.appId))
                        {
                            Debug.WriteLine("Vive id is blacklisted: " + appInstalled.appId);
                            continue;
                        }

                        string appName = appInstalled.title;
                        string appImage = Path.Combine(appInstalled.path, "Thumbnail-square.jpg");
                        string runcommand = appInstalled.uri;
                        await ViveAddApplication(appName, appImage, runcommand);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding Vive library: " + ex.Message);
            }
        }

        async Task ViveAddApplication(string appName, string appImage, string runCommand)
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
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, appImage, "Vive" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Vive,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncherImage = vImagePreloadVive
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Vive app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Vive app: " + appName);
            }
        }
    }
}