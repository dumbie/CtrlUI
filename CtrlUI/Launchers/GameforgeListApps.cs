using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task GameforgeScanAddLibrary()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey regKeyUninstall = registryKeyCurrentUser.OpenSubKey("Software\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
                    {
                        if (regKeyUninstall != null)
                        {
                            //Filter Gameforge applications
                            var regKeyGames = regKeyUninstall.GetSubKeyNames().Where(x => x.StartsWith("{") && x.EndsWith("}")).ToList();
                            foreach (string appId in regKeyGames)
                            {
                                try
                                {
                                    using (RegistryKey installDetails = regKeyUninstall.OpenSubKey(appId))
                                    {
                                        string uninstallString = installDetails.GetValue("UninstallString").ToString();
                                        if (uninstallString.Contains("gfclient"))
                                        {
                                            string applicationId = appId.Replace("{", string.Empty).Replace("}", string.Empty);
                                            string[] splitRegionTag = applicationId.Split('.');
                                            string runCommand = "gfclient://view?game=" + splitRegionTag[0]; //start,play,game-login,hidden-start

                                            string regionTag = string.Empty;
                                            if (splitRegionTag.Count() > 1)
                                            {
                                                regionTag = splitRegionTag[1];
                                                runCommand += "&region=" + regionTag;
                                            }

                                            string displayIcon = installDetails.GetValue("DisplayIcon").ToString();
                                            string displayName = installDetails.GetValue("DisplayName").ToString();
                                            if (!string.IsNullOrWhiteSpace(regionTag))
                                            {
                                                displayName = displayName.Replace(regionTag, string.Empty).Trim();
                                            }

                                            await GameforgeAddApplication(displayName, displayIcon, runCommand);
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding Gameforge library: " + ex.Message);
            }
        }

        async Task GameforgeAddApplication(string displayName, string displayIcon, string runCommand)
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
                string appNameLower = displayName.ToLower();
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher app is on the blacklist: " + appName);
                    return;
                }

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { displayName, displayIcon, "Gameforge" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Gameforge,
                    Name = displayName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncherImage = vImagePreloadGameforge
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Gameforge app: " + displayName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Gameforge app: " + displayName);
            }
        }
    }
}