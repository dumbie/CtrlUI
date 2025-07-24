using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static CtrlUI.Classes;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task PlariumScanAddLibrary()
        {
            try
            {
                //Get app json path
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string jsonPath = Path.Combine(appDataPath, "PlariumPlay\\gamestorage.gsfn"); //Fix does not contain Nords and Sparta
                string executablePath = Path.Combine(appDataPath, "PlariumPlay\\PlariumPlay.exe");
                string iconsPath = Path.Combine(appDataPath, "PlariumPlay\\Icons");

                //Load applications from json
                string launcherInstalledJson = File.ReadAllText(jsonPath);
                PlariumApps installedDeserial = JsonConvert.DeserializeObject<PlariumApps>(launcherInstalledJson);

                //List all available icons
                string[] iconFiles = Directory.GetFiles(iconsPath);

                //Add applications from json
                foreach (var appInstalled in installedDeserial.InstalledGames)
                {
                    try
                    {
                        string appIdentifier = appInstalled.Value.Id.ToString();
                        string appIcon = iconFiles.Where(x => x.Contains(appIdentifier + "_")).FirstOrDefault();
                        string appName = StringToTitleCase(appInstalled.Value.InsalledGames.Keys.FirstOrDefault().Replace("-", " "));
                        string executableArguments = "-gameid=" + appIdentifier + " -tray-start";
                        await PlariumAddApplication(appName, appIcon, executablePath, executableArguments);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding Plarium library: " + ex.Message);
            }
        }

        async Task PlariumAddApplication(string appName, string appIcon, string executablePath, string executableArgument)
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
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, appIcon, "Plarium" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Plarium,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = executablePath,
                    Argument = executableArgument,
                    StatusLauncherImage = vImagePreloadPlarium
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Plarium app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Plarium app: " + appName);
            }
        }
    }
}