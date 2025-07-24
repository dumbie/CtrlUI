using ArnoldVinkCode;
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
        async Task HikariFieldScanAddLibrary()
        {
            try
            {
                //Get launcher paths
                string roamingDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string installsPath = Path.Combine(roamingDataPath, "hikari-field-client\\installs.json");

                //Read installs json file
                string installsString = File.ReadAllText(installsPath);
                HikariFieldApps installsJson = JsonConvert.DeserializeObject<HikariFieldApps>(installsString);

                //Add applications to list
                foreach (var install in installsJson.installs)
                {
                    //Check if install is application
                    string executableFile = install.Value.exec_file;
                    if (executableFile.EndsWith(".exe"))
                    {
                        //Read and adjust name
                        string appName = install.Key;
                        appName = appName.Replace("_", " ");
                        appName = appName.Trim();
                        appName = AVFunctions.StringToTitleCase(appName);

                        string installPath = install.Value.installed_path;
                        string runCommand = Path.Combine(installPath, executableFile);
                        await HikariFieldAddApplication(appName, runCommand, runCommand);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding Hikari Field library: " + ex.Message);
            }
        }

        async Task HikariFieldAddApplication(string appName, string appImage, string runCommand)
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

                //Load application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, appImage, "Hikari Field" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.HikariField,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncherImage = vImagePreloadHikariField
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Hikari Field game: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Hikari Field game: " + appName);
            }
        }
    }
}