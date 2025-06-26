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
        async Task BigFishScanAddLibrary()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey regKeyGameDB = registryKeyCurrentUser.OpenSubKey("Software\\WOW6432Node\\Big Fish Games\\Persistence\\GameDB"))
                    {
                        if (regKeyGameDB != null)
                        {
                            foreach (string appId in regKeyGameDB.GetSubKeyNames())
                            {
                                try
                                {
                                    using (RegistryKey installDetails = regKeyGameDB.OpenSubKey(appId))
                                    {
                                        string displayIcon = installDetails.GetValue("feature").ToString();
                                        string displayName = installDetails.GetValue("Name").ToString();
                                        string executablePath = installDetails.GetValue("ExecutablePath").ToString();
                                        if (!executablePath.Contains("CasinoActivator"))
                                        {
                                            string executableRoot = Path.GetDirectoryName(executablePath);
                                            string runCommand = Path.Combine(executableRoot, "LaunchGame.bfg");
                                            await BigFishAddApplication(displayName, displayIcon, runCommand);
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
                Debug.WriteLine("Failed adding BigFish library: " + ex.Message);
            }
        }

        async Task BigFishAddApplication(string displayName, string displayIcon, string runCommand)
        {
            try
            {
                //Add application to check list
                vLauncherAppAvailableCheck.Add(runCommand);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == runCommand.ToLower());
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Launcher app already in list: " + displayName);
                    return;
                }

                //Check if application name is ignored
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == displayName.ToLower()))
                {
                    //Debug.WriteLine("Launcher app is on the blacklist: " + displayName);
                    return;
                }

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { displayName, displayIcon, "Big Fish" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.BigFish,
                    Name = displayName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncherImage = vImagePreloadBigFish
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added BigFish app: " + displayName);
            }
            catch
            {
                Debug.WriteLine("Failed adding BigFish app: " + displayName);
            }
        }
    }
}