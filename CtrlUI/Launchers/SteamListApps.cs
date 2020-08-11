using Microsoft.Win32;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVImage;
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
                RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);

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

                //Close and dispose the registry
                registryKeyCurrentUser.Dispose();
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
                using (FileStream fileStream = new FileStream(steamAppsPath, FileMode.Open, FileAccess.Read))
                {
                    keyValue.ReadAsText(fileStream);
                }

                //Add steam installation paths
                libraryPaths.Add(steamInstallPath);
                foreach (KeyValue child in keyValue.Children)
                {
                    try
                    {
                        if (child.Value.Contains("\\") && Directory.Exists(child.Value))
                        {
                            libraryPaths.Add(child.Value);
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return libraryPaths;
        }

        async Task SteamScanAddLibrary(List<string> libraryPaths)
        {
            try
            {
                string steamMainPath = libraryPaths.FirstOrDefault();
                List<string> steamApplicationAvailable = new List<string>();
                foreach (string path in libraryPaths)
                {
                    try
                    {
                        string steamAppsPath = Path.Combine(path, "steamapps");
                        Debug.WriteLine("Scanning steam library: " + steamAppsPath);
                        string[] manifestFiles = Directory.GetFiles(steamAppsPath, "appmanifest*");
                        foreach (string manifestPath in manifestFiles)
                        {
                            string runCommand = await SteamAddApplication(manifestPath, steamMainPath);
                            steamApplicationAvailable.Add(runCommand);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed scanning steam library: " + ex.Message);
                    }
                }

                //Remove deleted steam applications
                await ListBoxRemoveAll(lb_Launchers, List_Launchers, x => !steamApplicationAvailable.Any(y => y == x.PathExe));
            }
            catch { }
        }

        async Task<string> SteamAddApplication(string appmanifestPath, string steamMainPath)
        {
            try
            {
                KeyValue keyValue = new KeyValue();
                keyValue.ReadFileAsText(appmanifestPath);

                //Get application id
                string appId = keyValue["appID"].Value;

                //Get launch argument
                string runCommand = "steam://rungameid/" + appId;

                //Check if steam application is already added
                DataBindApp shortcutExistCheck = List_Launchers.Where(x => x.PathExe.ToLower() == runCommand.ToLower()).FirstOrDefault();
                if (shortcutExistCheck != null)
                {
                    Debug.WriteLine("Steam id already in list: " + appId);
                    return runCommand;
                }

                //Check if application id is in blacklist
                if (vSteamIdBlacklist.Contains(appId))
                {
                    Debug.WriteLine("Steam id is blacklisted: " + appId);
                    return runCommand;
                }

                //Get application name
                string appName = keyValue["name"].Value;
                if (string.IsNullOrWhiteSpace(appName) || appName.Contains("appid"))
                {
                    appName = keyValue["installDir"].Value;
                }

                //Get application image
                string libraryImageName = steamMainPath + "\\appcache\\librarycache\\" + appId + "_library_600x900.jpg";
                string logoImageName = steamMainPath + "\\appcache\\librarycache\\" + appId + "_logo.png";
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { libraryImageName, logoImageName }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 90, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncher = Visibility.Visible
                };
                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                Debug.WriteLine("Added steam app: " + appId + "/" + appName);
                return runCommand;
            }
            catch
            {
                Debug.WriteLine("Failed adding steam app: " + appmanifestPath);
                return string.Empty;
            }
        }
    }
}