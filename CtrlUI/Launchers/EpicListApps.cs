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
using static CtrlUI.Classes;
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
                        await EpicAddApplication(appInstalled.AppName, appInstalled.InstallLocation, installedManifests);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding epic library: " + ex.Message);
            }
        }

        async Task EpicAddApplication(string appNameId, string installLocation, List<EpicInstalledManifest> installedManifests)
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
                EpicInstalledManifest appManifest = installedManifests.FirstOrDefault(x => x.AppName.ToLower() == appNameId.ToLower());

                //Get launch argument
                string runCommand = "com.epicgames.launcher://apps/" + appNameId + "?action=launch&silent=true";
                vLauncherAppAvailableCheck.Add(runCommand);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == runCommand.ToLower());
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Epic app already in list: " + appIds);
                    return;
                }

                //Get application name
                string appName = appManifest.DisplayName;

                //Check if application name is ignored
                string appNameLower = appName.ToLower();
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher is on the blacklist skipping: " + appName);
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, x => x.Name.ToLower() == appNameLower);
                    return;
                }

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, "Epic" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

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
                //Debug.WriteLine("Added epic app: " + appNameId);
            }
            catch
            {
                Debug.WriteLine("Failed adding epic app: " + appNameId);
            }
        }
    }
}