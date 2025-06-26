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
        async Task ElixirScanAddLibrary()
        {
            try
            {
                //Fix wait for shortcut launch support
                //Fix find way to load application image
                //Fix find way to load proper application name

                //Get app data paths
                string localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                //Load applications from json
                string jsonPath = Path.Combine(roamingPath, "Elixir Gaming\\config.json");
                string launcherInstalledJson = File.ReadAllText(jsonPath);
                ElixirApps installedDeserial = JsonConvert.DeserializeObject<ElixirApps>(launcherInstalledJson);

                //Get launcher path
                string executablePath = Path.Combine(localPath, "Programs\\Elixir Gaming\\Elixir Gaming.exe");

                //Add applications from json
                foreach (var appInstalled in installedDeserial.GameRegistry)
                {
                    try
                    {
                        string appId = "--launchElixir=" + appInstalled.Key;
                        string installPath = new DirectoryInfo(appInstalled.Value.MASTER.installPath).Name;
                        string appName = StringToTitleCase(installPath.Replace("-", " ")).Trim();
                        await ElixirAddApplication(appName, executablePath, appId);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding Elixir library: " + ex.Message);
            }
        }

        async Task ElixirAddApplication(string appName, string executablePath, string executableArgument)
        {
            try
            {
                //Add application to check list
                vLauncherAppAvailableCheck.Add(executablePath);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == executablePath.ToLower() && x.Argument.ToLower() == executableArgument.ToLower());
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
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, "Elixir" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Elixir,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = executablePath,
                    Argument = executableArgument,
                    StatusLauncherImage = vImagePreloadElixir
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Elixir app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Elixir app: " + appName);
            }
        }
    }
}