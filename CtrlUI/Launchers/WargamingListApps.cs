using Microsoft.Win32;
using System;
using System.Diagnostics;
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
        async Task WargamingScanAddLibrary()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey regKeyUninstall = registryKeyCurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
                    {
                        if (regKeyUninstall != null)
                        {
                            //Filter Wargaming applications
                            foreach (string appId in regKeyUninstall.GetSubKeyNames())
                            {
                                try
                                {
                                    using (RegistryKey installDetails = regKeyUninstall.OpenSubKey(appId))
                                    {
                                        //Check if the application is Wargaming
                                        string uninstallString = installDetails.GetValue("UninstallString").ToString();
                                        if (!uninstallString.Contains("wgc_api.exe"))
                                        {
                                            continue;
                                        }

                                        string displayIcon = installDetails.GetValue("DisplayIcon").ToString().Split(',').FirstOrDefault();
                                        string displayName = installDetails.GetValue("DisplayName").ToString().Replace("_", " ");
                                        string executablePath = uninstallString.Replace("\"", string.Empty).Replace("--uninstall", string.Empty);
                                        string executeArguments = "--open";
                                        await WargamingAddApplication(displayName, displayIcon, executablePath, executeArguments);
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
                Debug.WriteLine("Failed adding Wargaming library: " + ex.Message);
            }
        }

        async Task WargamingAddApplication(string displayName, string displayIcon, string executablePath, string executableArguments)
        {
            try
            {
                //Add application to check list
                vLauncherAppAvailableCheck.Add(executablePath);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == executablePath.ToLower());
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
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { displayName, displayIcon, "Wargaming" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Wargaming,
                    Name = displayName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = executablePath,
                    Argument = executableArguments,
                    StatusLauncherImage = vImagePreloadWargaming
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Wargaming app: " + displayName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Wargaming app: " + displayName);
            }
        }
    }
}