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
        async Task StoveScanAddLibrary()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey regKeyStoveApps = registryKeyCurrentUser.OpenSubKey("Software\\SGUP\\apps"))
                    {
                        if (regKeyStoveApps != null)
                        {
                            foreach (string appId in regKeyStoveApps.GetSubKeyNames())
                            {
                                try
                                {
                                    using (RegistryKey installDetails = regKeyStoveApps.OpenSubKey(appId))
                                    {
                                        string exeName = installDetails.GetValue("ExeName").ToString();
                                        string gamePath = installDetails.GetValue("GamePath").ToString();
                                        string gameTitle = installDetails.GetValue("GameTitle").ToString();
                                        string executablePath = Path.Combine(gamePath, exeName);
                                        string runCommand = "sgup://run/" + appId;
                                        await StoveAddApplication(gameTitle, executablePath, runCommand);
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
                Debug.WriteLine("Failed adding Stove library: " + ex.Message);
            }
        }

        async Task StoveAddApplication(string appName, string appIcon, string runCommand)
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
                string appNameLower = appName.ToLower();
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher app is on the blacklist: " + appName);
                    return;
                }

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, appIcon, "Stove" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Stove,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncherImage = vImagePreloadStove
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Stove app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Stove app: " + appName);
            }
        }
    }
}