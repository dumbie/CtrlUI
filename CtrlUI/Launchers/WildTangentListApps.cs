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
        async Task WildTangentScanAddLibrary()
        {
            try
            {
                //Get program files path
                string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

                //Open the Windows registry
                using (RegistryKey registryKeyLocal = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey regKeyGames = registryKeyLocal.OpenSubKey("Software\\WOW6432Node\\WildTangent\\InstalledSKUs"))
                    {
                        if (regKeyGames != null)
                        {
                            foreach (string appId in regKeyGames.GetSubKeyNames())
                            {
                                try
                                {
                                    using (RegistryKey installDetails = regKeyGames.OpenSubKey(appId))
                                    {
                                        string productCodeName = installDetails.GetValue("ProductCodeName").ToString();
                                        string appName = installDetails.GetValue("ProductDisplayName").ToString();
                                        string installDirectory = installDetails.GetValue("InstallDirectory").ToString();
                                        string launchExeName = installDetails.GetValue("launchExeName").ToString();
                                        string appImage = Path.Combine(installDirectory, launchExeName);
                                        string executablePath = Path.Combine(programFilesPath, "WildTangent Games\\App\\GameConsole-wt.exe");
                                        string executableArgument = "/action play " + productCodeName;
                                        await WildTangentAddApplication(appName, appImage, executablePath, executableArgument);
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
                Debug.WriteLine("Failed adding WildTangent library: " + ex.Message);
            }
        }

        async Task WildTangentAddApplication(string appName, string appImage, string executablePath, string executableArgument)
        {
            try
            {
                //Add application to check list
                vLauncherAppAvailableCheck.Add(executablePath);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == executablePath.ToLower() && x.Argument.ToLower() == executableArgument.ToLower());
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Launcher app already in list: " + appName);
                    return;
                }

                //Check if application name is ignored
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appName.ToLower()))
                {
                    //Debug.WriteLine("Launcher app is on the blacklist: " + appName);
                    return;
                }

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, appImage, "WildTangent" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.WildTangent,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = executablePath,
                    Argument = executableArgument,
                    StatusLauncherImage = vImagePreloadWildTangent
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added WildTangent app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding WildTangent app: " + appName);
            }
        }
    }
}