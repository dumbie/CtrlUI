using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
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
        async Task JagexScanAddLibrary()
        {
            try
            {
                //Fix find way to detect Jagex games without hardcoded checks
                //It is required to kill the Jagex launcher before launching games
                //Options: runescape rs_nxt osrs osrs_ehc osrs_runelite dragonwilds

                //Open Local Machine Windows registry
                string launcherPath = string.Empty;
                using (RegistryKey registryKeyLocal = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    //Get Jagex launcher path
                    using (RegistryKey registryKey = registryKeyLocal.OpenSubKey("Software\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Jagex Launcher"))
                    {
                        if (registryKey != null)
                        {
                            launcherPath = registryKey.GetValue("DisplayIcon").ToString().Replace("\"", string.Empty);
                        }
                    }

                    //Check Next Runescape
                    using (RegistryKey registryKey = registryKeyLocal.OpenSubKey("Software\\Jagex\\JagexLauncher\\RuneScape"))
                    {
                        if (registryKey != null)
                        {
                            string appName = "RuneScape";
                            string installLocation = registryKey.GetValue("InstallLocation").ToString();
                            string appIcon = Path.Combine(installLocation, "RuneScape.exe");
                            string executeArgument = "--launch=rs_nxt";
                            await JagexAddApplication(appName, appIcon, launcherPath, executeArgument);
                        }
                    }

                    //Check Old School Runescape Official
                    using (RegistryKey registryKey = registryKeyLocal.OpenSubKey("Software\\WOW6432Node\\Jagex\\JagexLauncher\\osclient"))
                    {
                        if (registryKey != null)
                        {
                            string appName = "Old School RuneScape";
                            string installLocation = registryKey.GetValue("InstallLocation").ToString();
                            string appIcon = Path.Combine(installLocation, "osclient.exe");
                            string executeArgument = "--launch=osrs_ehc";
                            await JagexAddApplication(appName, appIcon, launcherPath, executeArgument);

                        }
                    }
                }

                //Open Current User Windows registry
                using (RegistryKey registryKeyUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                {
                    //Check Old School RuneScape Lite
                    using (RegistryKey registryKey = registryKeyUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\RuneLite Launcher_is1"))
                    {
                        if (registryKey != null)
                        {
                            string appName = "RuneLite";
                            string appIcon = registryKey.GetValue("DisplayIcon").ToString();
                            string executeArgument = "--launch=osrs_runelite";
                            await JagexAddApplication(appName, appIcon, launcherPath, executeArgument);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding Jagex library: " + ex.Message);
            }
        }

        async Task JagexAddApplication(string appName, string appIcon, string executePath, string executeArgument)
        {
            try
            {
                //Check if application name is ignored
                string appNameLower = appName.ToLower();
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher app is on the blacklist: " + appName);
                    return;
                }

                //Add application to check list
                vLauncherAppAvailableCheck.Add(executePath);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == executePath.ToLower() && x.Argument == executeArgument.ToLower());
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Launcher app already in list: " + appId);
                    return;
                }

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, appIcon, "Jagex" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Jagex,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = executePath,
                    Argument = executeArgument,
                    StatusLauncherImage = vImagePreloadJagex
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Jagex app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Jagex app: " + appName);
            }
        }
    }
}