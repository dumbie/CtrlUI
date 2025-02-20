using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
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
        //Classes
        public class IndieGalaInstalledApp
        {
            public class GameData
            {
                public string description_short { get; set; }
                public string description_long { get; set; }
                public string system_requirements { get; set; }
                public List<string> categories { get; set; }
                public string downloadable_win { get; set; }
                public string downloadable_mac { get; set; }
                public string downloadable_lin { get; set; }
                public List<string> os { get; set; }
                public int views { get; set; }
                public Rating rating { get; set; }
                public string stars { get; set; }
                public List<string> specs { get; set; }
                public List<string> tags { get; set; }
                public bool in_collection { get; set; }
                public string youtube_best_video { get; set; }
                public string exe_path { get; set; }
                public string cwd { get; set; }
                public string args { get; set; }
            }

            public class ItemData
            {
                public string name { get; set; }
                public string slugged_name { get; set; }
                public string id_key_name { get; set; }
                public string dev_id { get; set; }
                public string dev_image { get; set; }
                public string dev_cover { get; set; }
                public DateTime date { get; set; }
                public bool in_collection { get; set; }
                public string build_version { get; set; }
                public List<string> tags { get; set; }
                public List<string> build_download_path { get; set; }
            }

            public class Rating
            {
                public int count { get; set; }
                public double avg_rating { get; set; }
                public bool voted { get; set; }
            }

            public class Target
            {
                public ItemData item_data { get; set; }
                public GameData game_data { get; set; }
            }

            public Target target { get; set; }
            public List<string> path { get; set; }
            public double playtime { get; set; }
            public bool needsUpdate { get; set; }
        }

        async Task IndieGalaScanAddLibrary()
        {
            try
            {
                //Get installed json path
                string commonApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string installedJsonPath = Path.Combine(commonApplicationDataPath, "IGClient\\storage\\installed.json");

                //Check if json exists
                if (!File.Exists(installedJsonPath)) { return; }

                //Convert json
                string jsonFileText = File.ReadAllText(installedJsonPath);
                List<IndieGalaInstalledApp> igInstalledApps = JsonConvert.DeserializeObject<List<IndieGalaInstalledApp>>(jsonFileText);

                //Add apps to launcher list
                foreach (IndieGalaInstalledApp igInstalledApp in igInstalledApps)
                {
                    try
                    {
                        await IndieGalaAddApplication(igInstalledApp);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding IndieGala library: " + ex.Message);
            }
        }

        async Task IndieGalaAddApplication(IndieGalaInstalledApp igInstalledApp)
        {
            try
            {
                //Get application name
                string appName = igInstalledApp.target.item_data.name;
                string appNameLower = appName.ToLower();

                //Combine directories
                string executableName = igInstalledApp.target.game_data.exe_path;
                string executablePath = Path.Combine(igInstalledApp.path.FirstOrDefault(), igInstalledApp.target.item_data.slugged_name);

                //Check executable name
                if (string.IsNullOrWhiteSpace(executableName))
                {
                    //Debug.WriteLine("IndieGala game executable not set: " + appName + "/" + executablePath);
                    string[] searchIgnore = { "python", "zsync", "crashhandler", "config", "setting", "setup", "unins", "install" };
                    string[] searchExecutables = Directory.GetFiles(executablePath, "*.exe", SearchOption.AllDirectories);
                    if (searchExecutables.Any())
                    {
                        executableName = searchExecutables.Where(x => !searchIgnore.Any(z => x.ToLower().Contains(z))).OrderBy(x => x.Length).FirstOrDefault();
                        //Debug.WriteLine("IndieGala game executable found: " + appName + "/" + executableName);
                    }
                    else
                    {
                        //Debug.WriteLine("IndieGala game executable not found: " + appName + "/" + executablePath);
                        return;
                    }
                }

                //Combine executable path
                executablePath = Path.Combine(executablePath, executableName);

                //Check if application is installed
                if (!File.Exists(executablePath))
                {
                    Debug.WriteLine("IndieGala game is not installed: " + appName);
                    return;
                }

                //Add application to available list
                vLauncherAppAvailableCheck.Add(executablePath);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == executablePath.ToLower());
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("IndieGala app already in list: " + appName);
                    return;
                }

                //Check if application name is ignored
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher is on the blacklist skipping: " + appName);
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, x => x.Name.ToLower() == appNameLower);
                    return;
                }

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, executablePath, "IndieGala" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.IndieGala,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = executablePath,
                    StatusLauncherImage = vImagePreloadIndieGala
                };

                //Check launch arguments
                if (string.IsNullOrWhiteSpace(igInstalledApp.target.game_data.args))
                {
                    dataBindApp.Argument = igInstalledApp.target.game_data.args;
                }

                ////Check content work directory
                //if (string.IsNullOrWhiteSpace(igInstalledApp.target.game_data.cwd))
                //{
                //    dataBindApp.PathLaunch = igInstalledApp.target.game_data.cwd;
                //}

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added IndieGala app: " + appName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding IndieGala app: " + ex.Message);
            }
        }
    }
}