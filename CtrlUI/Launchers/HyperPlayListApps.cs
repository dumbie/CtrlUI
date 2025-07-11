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
        async Task HyperPlayScanAddLibrary()
        {
            try
            {
                //Get app json path
                string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string jsonPath = Path.Combine(roamingPath, "hyperplay\\hp_store\\library.json");

                //Load applications from json
                string libraryJson = File.ReadAllText(jsonPath);
                HyperPlayLibrary libraryDeserial = JsonConvert.DeserializeObject<HyperPlayLibrary>(libraryJson);

                //Add applications from json
                foreach (HyperPlayGame appInstalled in libraryDeserial.games)
                {
                    try
                    {
                        //Check if application is installed
                        if (appInstalled.is_installed)
                        {
                            //if type != native
                            string appName = appInstalled.title;
                            string appImage = appInstalled.install.executable;
                            string runCommand = "hyperplay://launch/hyperplay/" + appInstalled.app_name;
                            await HyperPlayAddApplication(appName, appImage, runCommand);
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding HyperPlay library: " + ex.Message);
            }
        }

        async Task HyperPlayAddApplication(string appName, string appImage, string runCommand)
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
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, appImage, "HyperPlay" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.HyperPlay,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncherImage = vImagePreloadHyperPlay
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added HyperPlay app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding HyperPlay app: " + appName);
            }
        }
    }
}