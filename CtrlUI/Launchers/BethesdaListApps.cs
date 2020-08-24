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
        async Task BethesdaScanAddLibrary()
        {
            try
            {
                //Improve read installed applications from cdpprod.dch file

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
                                        string uninstallString = installDetails.GetValue("UninstallString")?.ToString();
                                        if (uninstallString.Contains("bethesdanetupdater"))
                                        {
                                            string appId = installDetails.GetValue("ProductID")?.ToString();
                                            string appName = installDetails.GetValue("DisplayName")?.ToString();
                                            string appIcon = installDetails.GetValue("DisplayIcon")?.ToString();
                                            string installDir = installDetails.GetValue("Path")?.ToString().Replace("\"", string.Empty);
                                            await BethesdaAddApplication(appId, appName, appIcon, installDir);
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
                Debug.WriteLine("Failed adding Bethesda library: " + ex.Message);
            }
        }

        async Task BethesdaAddApplication(string appId, string appName, string appIcon, string installDir)
        {
            try
            {
                //Check if application is installed
                if (!Directory.Exists(installDir))
                {
                    Debug.WriteLine("Bethesda game is not installed: " + appId);
                    return;
                }

                //Get launch argument
                string runCommand = "bethesdanet://run/" + appId;
                vLauncherAppAvailableCheck.Add(runCommand);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.Where(x => x.PathExe.ToLower() == runCommand.ToLower()).FirstOrDefault();
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Bethesda app already in list: " + appId);
                    return;
                }

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appIcon, appName, "Bethesda" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 90, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Bethesda,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    PathLaunch = installDir,
                    StatusLauncher = vImagePreloadBethesda
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Bethesda app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Bethesda app: " + appId);
            }
        }
    }
}