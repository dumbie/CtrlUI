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
        async Task EpicScanAddLibrary()
        {
            try
            {
                //Get launcher icon image
                BitmapImage launcherImage = FileToBitmapImage(new string[] { "Epic Games" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 10, 0);

                //Get launcher paths
                string commonApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string launcherInstalledPath = Path.Combine(commonApplicationDataPath, "Epic\\UnrealEngineLauncher\\LauncherInstalled.dat");
                string manifestsPath = Path.Combine(commonApplicationDataPath, "Epic\\EpicGamesLauncher\\Data\\Manifests");

                //Load applications from json
                string launcherInstalledJson = File.ReadAllText(launcherInstalledPath);
                EpicLauncherInstalled launcherInstalledDeserial = JsonConvert.DeserializeObject<EpicLauncherInstalled>(launcherInstalledJson);

                //Load manifests from json
                List<EpicInstalledManifest> installedManifests = new List<EpicInstalledManifest>();
                foreach (string manifestFile in Directory.GetFiles(manifestsPath, "*.item"))
                {
                    try
                    {
                        string manifestFileJson = File.ReadAllText(manifestFile);
                        installedManifests.Add(JsonConvert.DeserializeObject<EpicInstalledManifest>(manifestFileJson));
                    }
                    catch { }
                }

                //Add applications from json
                foreach (var appInstalled in launcherInstalledDeserial.InstallationList)
                {
                    try
                    {
                        await EpicAddApplication(appInstalled.AppName, appInstalled.InstallLocation, installedManifests, launcherImage);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding epic library: " + ex.Message);
            }
        }

        async Task EpicAddApplication(string appNameId, string installLocation, List<EpicInstalledManifest> installedManifests, BitmapImage launcherImage)
        {
            try
            {
                //Check if application is installed
                if (!Directory.Exists(installLocation))
                {
                    Debug.WriteLine("Epic game is not installed: " + appNameId);
                    return;
                }

                //Get application manifest
                EpicInstalledManifest appManifest = installedManifests.Where(x => x.AppName.ToLower() == appNameId.ToLower()).FirstOrDefault();

                //Get launch argument
                string runCommand = "com.epicgames.launcher://apps/" + appNameId + "?action=launch&silent=true";
                vLauncherAppAvailableCheck.Add(runCommand);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.Where(x => x.PathExe.ToLower() == runCommand.ToLower()).FirstOrDefault();
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Epic app already in list: " + appIds);
                    return;
                }

                //Get application name
                string appName = appManifest.DisplayName;

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, "Epic Games" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 90, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncher = launcherImage
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added epic app: " + appNameId);
            }
            catch
            {
                Debug.WriteLine("Failed adding epic app: " + appNameId);
            }
        }
    }
}