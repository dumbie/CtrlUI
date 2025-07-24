using Microsoft.Win32;
using System;
using System.Diagnostics;
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
        async Task NCSoftScanAddLibrary()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey regKeyUninstall = registryKeyCurrentUser.OpenSubKey("Software\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
                    {
                        if (regKeyUninstall != null)
                        {
                            //Filter NCSoft applications
                            var regKeyGames = regKeyUninstall.GetSubKeyNames().Where(x => x.StartsWith("NCSOFT ")).ToList();
                            foreach (string appId in regKeyGames)
                            {
                                try
                                {
                                    using (RegistryKey installDetails = regKeyUninstall.OpenSubKey(appId))
                                    {
                                        string applicationId = appId.Replace("NCSOFT ", string.Empty);
                                        string displayIcon = installDetails.GetValue("DisplayIcon").ToString();
                                        string displayName = installDetails.GetValue("DisplayName").ToString();
                                        string executablePath = installDetails.GetValue("UninstallString").ToString().Replace("\"", string.Empty).Split('-').FirstOrDefault();
                                        string executeArguments = "--game-id " + applicationId;
                                        await NCSoftAddApplication(displayName, displayIcon, executablePath, executeArguments);
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
                Debug.WriteLine("Failed adding NCSoft library: " + ex.Message);
            }
        }

        async Task NCSoftAddApplication(string displayName, string displayIcon, string executablePath, string executeArguments)
        {
            try
            {
                //Add application to check list
                vLauncherAppAvailableCheck.Add(executablePath);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == executablePath.ToLower() && x.Argument.ToLower() == executeArguments.ToLower());
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Launcher app already in list: " + displayName);
                    return;
                }

                //Check if application name is ignored
                string appNameLower = displayName.ToLower();
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher app is on the blacklist: " + displayName);
                    return;
                }

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { displayName, displayIcon, "NCSoft" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.NCSoft,
                    Name = displayName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = executablePath,
                    Argument = executeArguments,
                    StatusLauncherImage = vImagePreloadNCSoft
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added NCSoft app: " + displayName);
            }
            catch
            {
                Debug.WriteLine("Failed adding NCSoft app: " + displayName);
            }
        }
    }
}