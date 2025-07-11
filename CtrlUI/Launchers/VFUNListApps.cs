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
        async Task VFUNScanAddLibrary()
        {
            try
            {
                //Fix find way to run games through launcher with SSO VALOFE.VFUN.Launcher://
                //Fix get application names from uninstall registry
                //HKLM\Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Atlantica
                //Fix get application icon from *\VLauncher\Image\Icon

                //Open the Windows registry
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey regKeyGames = registryKeyCurrentUser.OpenSubKey("Software\\Valofe"))
                    {
                        if (regKeyGames != null)
                        {
                            foreach (string appId in regKeyGames.GetSubKeyNames())
                            {
                                try
                                {
                                    using (RegistryKey installDetails = regKeyGames.OpenSubKey(appId))
                                    {
                                        string fileName = installDetails.GetValue("FILENAME").ToString();
                                        if (!fileName.Contains("VFUNLauncher"))
                                        {
                                            string filePath = installDetails.GetValue("PATH").ToString();
                                            string runCommand = Path.Combine(filePath, fileName);
                                            await VFUNAddApplication(fileName, runCommand);
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
                Debug.WriteLine("Failed adding VFUN library: " + ex.Message);
            }
        }

        async Task VFUNAddApplication(string displayName, string runCommand)
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
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { displayName, "VFUN" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Epic,
                    Name = displayName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncherImage = vImagePreloadEpic
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added VFUN app: " + displayName);
            }
            catch
            {
                Debug.WriteLine("Failed adding VFUN app: " + displayName);
            }
        }
    }
}