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
        async Task AmazonScanAddLibrary()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey registryKeyUninstall = registryKeyCurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
                    {
                        if (registryKeyUninstall != null)
                        {
                            foreach (string uninstallApp in registryKeyUninstall.GetSubKeyNames())
                            {
                                try
                                {
                                    using (RegistryKey installDetails = registryKeyUninstall.OpenSubKey(uninstallApp))
                                    {
                                        string uninstallString = installDetails.GetValue("UninstallString")?.ToString();
                                        if (uninstallString.Contains("Amazon Game"))
                                        {
                                            string appId = uninstallString.Split(new string[] { " -p " }, StringSplitOptions.None)[1];
                                            string appName = installDetails.GetValue("DisplayName")?.ToString();
                                            string appIcon = installDetails.GetValue("DisplayIcon")?.ToString().Replace("\"", string.Empty);
                                            string installDir = installDetails.GetValue("InstallLocation")?.ToString().Replace("\"", string.Empty);
                                            await AmazonAddApplication(appId, appName, appIcon, installDir);
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
                Debug.WriteLine("Failed adding Amazon library: " + ex.Message);
            }
        }

        async Task AmazonAddApplication(string appId, string appName, string appIcon, string installDir)
        {
            try
            {
                //Check if application name is ignored
                string appNameLower = appName.ToLower();
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher is on the blacklist skipping: " + appName);
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, x => x.Name.ToLower() == appNameLower);
                    return;
                }

                //Check if application is installed
                if (!Directory.Exists(installDir))
                {
                    Debug.WriteLine("Amazon game is not installed: " + appId);
                    return;
                }

                //Get launch argument
                string runCommand = "amazon-games://play/" + appId;
                vLauncherAppAvailableCheck.Add(runCommand);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.Where(x => x.PathExe.ToLower() == runCommand.ToLower()).FirstOrDefault();
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Amazon app already in list: " + appId);
                    return;
                }

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, appIcon, "Amazon" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 90, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Amazon,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    PathLaunch = installDir,
                    StatusLauncherImage = vImagePreloadAmazon
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Amazon app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Amazon app: " + appId);
            }
        }
    }
}