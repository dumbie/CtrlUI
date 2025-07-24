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
        async Task HumbleScanAddLibrary()
        {
            try
            {
                //Get installed json path
                string commonApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string installedJsonPath = Path.Combine(commonApplicationDataPath, "Humble App\\config.json");

                //Convert json
                string fileText = File.ReadAllText(installedJsonPath);
                HumbleAppJson fileJson = JsonConvert.DeserializeObject<HumbleAppJson>(fileText);

                //Add apps to launcher list
                foreach (GameCollection4 installedApp in fileJson.gamecollection4)
                {
                    try
                    {
                        await HumbleAddApplication(installedApp);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding Humble library: " + ex.Message);
            }
        }

        async Task HumbleAddApplication(GameCollection4 installedApp)
        {
            try
            {
                //Get application name
                string appName = installedApp.gameName;
                string appNameLower = appName.ToLower();

                //Check application status
                if (installedApp.status != "downloaded" && installedApp.status != "installed")
                {
                    //Debug.WriteLine("Humble app is not installed: " + appName);
                    return;
                }

                //Get run command
                string runCommand = "humble://launch/" + installedApp.downloadMachineName;

                //Combine directories
                string executablePath = Path.Combine(installedApp.filePath, installedApp.executablePath);

                ////Check if application is installed
                //if (!File.Exists(executablePath))
                //{
                //    Debug.WriteLine("Humble app is not installed: " + appName);
                //    return;
                //}

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
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher app is on the blacklist: " + appName);
                    return;
                }

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, executablePath, "Humble" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Humble,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncherImage = vImagePreloadHumble
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Humble app: " + appName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding Humble app: " + ex.Message);
            }
        }
    }
}