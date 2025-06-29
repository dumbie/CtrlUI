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
        async Task LoadingBayScanAddLibrary()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey regKeyGames = registryKeyCurrentUser.OpenSubKey("Software\\LoadingBay\\LoadingBayInstaller\\game"))
                    {
                        if (regKeyGames != null)
                        {
                            foreach (string appId in regKeyGames.GetSubKeyNames())
                            {
                                try
                                {
                                    using (RegistryKey installDetails = regKeyGames.OpenSubKey(appId))
                                    {
                                        string displayName = Path.GetFileNameWithoutExtension(installDetails.GetValue("startMenuPath").ToString());
                                        string installPath = installDetails.GetValue("InstallPath").ToString();
                                        string startupPath = installDetails.GetValue("StartupPath").ToString();
                                        string displayIcon = Path.Combine(installPath, startupPath);
                                        string runCommand = "loadingbay://mygame/?gameId=" + appId;
                                        await LoadingBayAddApplication(displayName, displayIcon, runCommand);
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
                Debug.WriteLine("Failed adding LoadingBay library: " + ex.Message);
            }
        }

        async Task LoadingBayAddApplication(string displayName, string displayIcon, string runCommand)
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
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { displayName, displayIcon, "Loading Bay" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.LoadingBay,
                    Name = displayName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncherImage = vImagePreloadLoadingBay
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added LoadingBay app: " + displayName);
            }
            catch
            {
                Debug.WriteLine("Failed adding LoadingBay app: " + displayName);
            }
        }
    }
}