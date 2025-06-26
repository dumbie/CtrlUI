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
        async Task ArcScanAddLibrary()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey regKeyPerfect = registryKeyCurrentUser.OpenSubKey("Software\\WOW6432Node\\Perfect World Entertainment"))
                    {
                        if (regKeyPerfect != null)
                        {
                            //Get launcher path
                            string executablePath = string.Empty;
                            using (RegistryKey launcherDetails = regKeyPerfect.OpenSubKey("Arc"))
                            {
                                executablePath = launcherDetails.GetValue("launcher").ToString();
                            }

                            //Get Arc applications
                            using (RegistryKey launcherApps = regKeyPerfect.OpenSubKey("Core"))
                            {
                                var regKeyGames = launcherApps.GetSubKeyNames().Where(x => x.EndsWith("en")).ToList();
                                foreach (string coreId in regKeyGames)
                                {
                                    try
                                    {
                                        using (RegistryKey installDetails = launcherApps.OpenSubKey(coreId))
                                        {
                                            string appId = installDetails.GetValue("APP_ABBR").ToString();
                                            string displayIcon = installDetails.GetValue("CLIENT_PATH").ToString();
                                            string displayName = Path.GetFileNameWithoutExtension(displayIcon).Replace("_en", string.Empty);
                                            string executeArguments = "gamecustom " + appId;
                                            await ArcAddApplication(displayName, displayIcon, executablePath, executeArguments);
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding Arc library: " + ex.Message);
            }
        }

        async Task ArcAddApplication(string displayName, string displayIcon, string executablePath, string executeArguments)
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
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { displayName, displayIcon, "Arc" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Arc,
                    Name = displayName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = executablePath,
                    Argument = executeArguments,
                    StatusLauncherImage = vImagePreloadArc
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Arc app: " + displayName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Arc app: " + displayName);
            }
        }
    }
}