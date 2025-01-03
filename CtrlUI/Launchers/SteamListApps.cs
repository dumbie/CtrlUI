using Microsoft.Win32;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVSearch;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        public static string[] vSteamIdBlacklist = { "218", "228980" };

        string SteamInstallPath()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    //Search for Steam install directory
                    using (RegistryKey RegKeySteam = registryKeyCurrentUser.OpenSubKey("Software\\Valve\\Steam"))
                    {
                        if (RegKeySteam != null)
                        {
                            string RegKeyExePath = RegKeySteam.GetValue("SteamExe").ToString();
                            if (File.Exists(RegKeyExePath))
                            {
                                return Path.GetDirectoryName(RegKeyExePath);
                            }
                        }
                    }
                }
            }
            catch { }
            return string.Empty;
        }

        List<string> SteamLibraryPaths()
        {
            List<string> libraryPaths = new List<string>();
            try
            {
                //Get steam main installation path
                string steamInstallPath = SteamInstallPath();
                if (string.IsNullOrWhiteSpace(steamInstallPath))
                {
                    Debug.Write("Steam installation not found.");
                    return libraryPaths;
                }

                //Set steam library folders path
                string steamAppsPath = Path.Combine(steamInstallPath, "steamapps\\libraryfolders.vdf");

                //Parse steam library folder file
                KeyValue keyValue = new KeyValue();
                keyValue.ReadFileAsText(steamAppsPath);

                //Add steam installation paths
                libraryPaths.Add(steamInstallPath);
                foreach (KeyValue child in keyValue.Children)
                {
                    try
                    {
                        foreach (KeyValue subChild in child.Children)
                        {
                            try
                            {
                                if (subChild.Name == "path" && Directory.Exists(subChild.Value))
                                {
                                    libraryPaths.Add(subChild.Value);
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return libraryPaths;
        }

        async Task SteamScanAddLibrary()
        {
            try
            {
                //Get steam library paths
                List<string> libraryPaths = SteamLibraryPaths();

                //Get steam main path
                string steamMainPath = libraryPaths.FirstOrDefault();

                foreach (string path in libraryPaths)
                {
                    try
                    {
                        string steamAppsPath = Path.Combine(path, "steamapps");
                        //Debug.WriteLine("Scanning steam library: " + steamAppsPath);
                        foreach (string manifestPath in Directory.GetFiles(steamAppsPath, "appmanifest*.acf"))
                        {
                            await SteamAddApplication(manifestPath, steamMainPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed scanning steam library: " + ex.Message);
                    }
                }
            }
            catch { }
        }

        async Task SteamAddApplication(string appmanifestPath, string steamMainPath)
        {
            try
            {
                KeyValue keyValue = new KeyValue();
                keyValue.ReadFileAsText(appmanifestPath);

                //Get application id
                string appId = keyValue["appID"].Value;

                //Get launch argument
                string runCommand = "steam://rungameid/" + appId;
                vLauncherAppAvailableCheck.Add(runCommand);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == runCommand.ToLower());
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Steam app already in list: " + appId);
                    return;
                }

                //Check if application id is in blacklist
                if (vSteamIdBlacklist.Contains(appId))
                {
                    Debug.WriteLine("Steam id is blacklisted: " + appId);
                    return;
                }

                //Get application name
                string appName = keyValue["name"].Value;
                if (string.IsNullOrWhiteSpace(appName) || appName.Contains("appid"))
                {
                    appName = keyValue["installDir"].Value;
                }

                //Check if application name is ignored
                string appNameLower = appName.ToLower();
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    Debug.WriteLine("Launcher is on the blacklist skipping: " + appName);
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, x => x.Name.ToLower() == appNameLower);
                    return;
                }

                //Search application image
                SearchSource[] searchSources =
                {
                    new SearchSource() { SearchPath = steamMainPath + "\\appcache\\librarycache\\" + appId, SearchPatterns = ["*.png", "*.jpg"], SearchOption = SearchOption.AllDirectories }
                };
                string searchImage = Search_Files(["library_600x900", "logo"], searchSources, false).FirstOrDefault();

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage([appName, searchImage, "Steam"], vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Steam,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncherImage = vImagePreloadSteam
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added steam app: " + appId + "/" + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding steam app: " + appmanifestPath);
            }
        }
    }
}