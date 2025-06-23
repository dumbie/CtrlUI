using Newtonsoft.Json;
using System;
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
        async Task NetmarbleScanAddLibrary()
        {
            try
            {
                //Get app json path
                string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string jsonPath = Path.Combine(appdataPath, "Netmarble Launcher\\config.json");

                //Load applications from json
                string launcherInstalledJson = File.ReadAllText(jsonPath);
                NetmarbleApps installedDeserial = JsonConvert.DeserializeObject<NetmarbleApps>(launcherInstalledJson);

                //Get launcher path
                string executablePath = Path.Combine(installedDeserial.AppDrive, "Netmarble Launcher.exe");

                //Add applications from json
                foreach (var appInstalled in installedDeserial.game)
                {
                    try
                    {
                        string appName = appInstalled.Value.build.name;
                        string installPath = appInstalled.Value.build.installPath;
                        string executableName = appInstalled.Value.build.download.executeFileName;
                        string appIcon = Path.Combine(installPath, executableName);
                        string executableArguments = "--productcode=/Game/" + appInstalled.Value.build.id + " --buildcode=" + appInstalled.Value.build.download.buildCode;
                        await NetmarbleAddApplication(appName, appIcon, executablePath, executableArguments);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding Netmarble library: " + ex.Message);
            }
        }

        async Task NetmarbleAddApplication(string appName, string appIcon, string executablePath, string executableArguments)
        {
            try
            {
                //Get launch argument
                vLauncherAppAvailableCheck.Add(executablePath);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == executablePath.ToLower() && x.Argument.ToLower() == executableArguments.ToLower());
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Launcher app already in list: " + appName);
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
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, appIcon, "Netmarble" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Netmarble,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = executablePath,
                    Argument = executableArguments,
                    StatusLauncherImage = vImagePreloadNetmarble
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Netmarble app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Netmarble app: " + appName);
            }
        }
    }
}