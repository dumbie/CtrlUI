using Microsoft.Win32;
using System;
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
        async Task RockstarScanAddLibrary()
        {
            try
            {
                //Improve Launcher.exe with argument -launchTitleInFolder *gamepath*

                //Open the Windows registry
                using (RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey registryKeyUninstall = registryKeyLocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
                    {
                        if (registryKeyUninstall != null)
                        {
                            foreach (string uninstallApp in registryKeyUninstall.GetSubKeyNames())
                            {
                                try
                                {
                                    using (RegistryKey installDetails = registryKeyUninstall.OpenSubKey(uninstallApp))
                                    {
                                        string publisher = installDetails.GetValue("Publisher")?.ToString();
                                        string uninstallString = installDetails.GetValue("UninstallString")?.ToString();
                                        if (publisher.Contains("Rockstar") && uninstallString.Contains("Launcher.exe") && uninstallString.Contains("-uninstall"))
                                        {
                                            string appName = installDetails.GetValue("DisplayName")?.ToString();
                                            string appExe = installDetails.GetValue("DisplayIcon")?.ToString().Replace("\"", string.Empty);
                                            string installDir = installDetails.GetValue("InstallLocation")?.ToString().Replace("\"", string.Empty);
                                            await RockstarAddApplication(appExe, appName, installDir);
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
                Debug.WriteLine("Failed adding Rockstar library: " + ex.Message);
            }
        }

        async Task RockstarAddApplication(string appExe, string appName, string installDir)
        {
            try
            {
                //Check if application is installed
                if (!Directory.Exists(installDir))
                {
                    Debug.WriteLine("Rockstar game is not installed: " + appExe);
                    return;
                }

                //Check if application is already added
                vLauncherAppAvailableCheck.Add(appExe);
                DataBindApp launcherExistCheck = List_Launchers.Where(x => x.PathExe.ToLower() == appExe.ToLower()).FirstOrDefault();
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Rockstar app already in list: " + appId);
                    return;
                }

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appExe, appName, "Rockstar" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 90, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Rockstar,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = appExe,
                    PathLaunch = installDir,
                    StatusLauncher = vImagePreloadRockstar
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Rockstar app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Rockstar app: " + appExe);
            }
        }
    }
}