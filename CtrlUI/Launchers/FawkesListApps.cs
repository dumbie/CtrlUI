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
        async Task FawkesScanAddLibrary()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey regKeyGames = registryKeyCurrentUser.OpenSubKey("Software\\XSOLLA\\1383"))
                    {
                        if (regKeyGames != null)
                        {
                            foreach (string appId in regKeyGames.GetSubKeyNames())
                            {
                                try
                                {
                                    using (RegistryKey installDetails = regKeyGames.OpenSubKey(appId).OpenSubKey("default"))
                                    {
                                        string prefix = installDetails.GetValue("prefix").ToString();
                                        string isInstalled = installDetails.GetValue("isInstalled").ToString();
                                        if (isInstalled == "true")
                                        {
                                            string displayName = Path.GetFileNameWithoutExtension(prefix);
                                            string runCommand = "xl-1383://game/" + appId;
                                            await FawkesAddApplication(displayName, runCommand);
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
                Debug.WriteLine("Failed adding Fawkes library: " + ex.Message);
            }
        }

        async Task FawkesAddApplication(string displayName, string runCommand)
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
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == displayName.ToLower()))
                {
                    //Debug.WriteLine("Launcher app is on the blacklist: " + appName);
                    return;
                }

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { displayName, "Fawkes" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Fawkes,
                    Name = displayName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncherImage = vImagePreloadFawkes
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Fawkes app: " + displayName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Fawkes app: " + displayName);
            }
        }
    }
}