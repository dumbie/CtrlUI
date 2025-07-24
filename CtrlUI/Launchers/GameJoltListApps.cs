using Newtonsoft.Json;
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
        async Task GameJoltScanAddLibrary()
        {
            try
            {
                //Get app json path
                string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string jsonPath = Path.Combine(roamingPath, "game-jolt-client\\User Data\\Default\\packages.wttf");

                //Load applications from json
                string launcherInstalledJson = File.ReadAllText(jsonPath);
                GameJoltApps installedDeserial = JsonConvert.DeserializeObject<GameJoltApps>(launcherInstalledJson);

                //Add applications from json
                foreach (var appInstalled in installedDeserial.objects)
                {
                    try
                    {
                        string appName = appInstalled.Value.title;
                        string executablePath = appInstalled.Value.launch_options.FirstOrDefault().executable_path;
                        if (string.IsNullOrWhiteSpace(appName))
                        {
                            appName = Path.GetFileNameWithoutExtension(executablePath);
                        }
                        string installPath = appInstalled.Value.install_dir + "\\data";
                        string launchPath = Path.Combine(installPath, executablePath);
                        await GameJoltAddApplication(appName, launchPath);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding GameJolt library: " + ex.Message);
            }
        }

        async Task GameJoltAddApplication(string appName, string launchPath)
        {
            try
            {
                //Add application to check list
                vLauncherAppAvailableCheck.Add(launchPath);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == launchPath.ToLower());
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
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, launchPath, "Game Jolt" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.GameJolt,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = launchPath,
                    StatusLauncherImage = vImagePreloadGameJolt
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added GameJolt app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding GameJolt app: " + appName);
            }
        }
    }
}