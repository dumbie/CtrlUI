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
        async Task LegacyGamesScanAddLibrary()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey regKeyLegacyGames = registryKeyCurrentUser.OpenSubKey("Software\\Legacy Games"))
                    {
                        if (regKeyLegacyGames != null)
                        {
                            foreach (string appId in regKeyLegacyGames.GetSubKeyNames())
                            {
                                try
                                {
                                    using (RegistryKey installDetails = regKeyLegacyGames.OpenSubKey(appId))
                                    {
                                        string productName = installDetails.GetValue("ProductName").ToString();
                                        string gameExe = installDetails.GetValue("GameExe").ToString();
                                        string instDir = installDetails.GetValue("InstDir").ToString();
                                        string executablePath = Path.Combine(instDir, gameExe);
                                        await LegacyGamesAddApplication(productName, executablePath);
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
                Debug.WriteLine("Failed adding Legacy Games library: " + ex.Message);
            }
        }

        async Task LegacyGamesAddApplication(string productName, string executablePath)
        {
            try
            {
                //Check if application is installed
                if (!File.Exists(executablePath))
                {
                    Debug.WriteLine("Legacy Games app is not installed: " + productName);
                    return;
                }

                //Add application to check list
                vLauncherAppAvailableCheck.Add(executablePath);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == executablePath.ToLower());
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Launcher app already in list: " + appIds);
                    return;
                }

                //Check if application name is ignored
                string appNameLower = productName.ToLower();
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher app is on the blacklist: " + appName);
                    return;
                }

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { productName, executablePath, "Legacy Games" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.LegacyGames,
                    Name = productName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = executablePath,
                    StatusLauncherImage = vImagePreloadLegacyGames
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Legacy Games app: " + productName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Legacy Games app: " + productName);
            }
        }
    }
}