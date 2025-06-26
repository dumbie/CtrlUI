using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static CtrlUI.Classes;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task NexonScanAddLibrary()
        {
            try
            {
                //Fix find way to load application image

                //Get app json path
                string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string jsonPath = Path.Combine(roamingPath, "NexonLauncher\\installed-apps.db");

                //Load applications from json
                string launcherInstalledJson = File.ReadAllText(jsonPath);
                NexonApps installedDeserial = JsonConvert.DeserializeObject<NexonApps>(launcherInstalledJson);

                //Add applications from json
                foreach (var appInstalled in installedDeserial.installedApps)
                {
                    try
                    {
                        string appName = StringToTitleCase(appInstalled.Value.name);
                        string runcommand = "nxl://launch/" + appInstalled.Key;
                        await NexonAddApplication(appName, runcommand);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding Nexon library: " + ex.Message);
            }
        }

        async Task NexonAddApplication(string appName, string runCommand)
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
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, "Nexon" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Nexon,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncherImage = vImagePreloadNexon
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Nexon app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Nexon app: " + appName);
            }
        }
    }
}