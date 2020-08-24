using ArnoldVinkCode;
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
        string UplayInstallPath()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    //Search for Uplay install directory
                    using (RegistryKey RegKeyUplay = registryKeyLocalMachine.OpenSubKey("Software\\Ubisoft\\Launcher"))
                    {
                        if (RegKeyUplay != null)
                        {
                            string RegKeyExePath = RegKeyUplay.GetValue("InstallDir").ToString() + "upc.exe";
                            if (File.Exists(RegKeyExePath))
                            {
                                return Path.GetDirectoryName(RegKeyExePath);
                            }
                        }
                    }
                }
            }
            catch { }
            return string.Empty;
        }

        async Task UplayScanAddLibrary()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey RegKeyUplay = registryKeyLocalMachine.OpenSubKey("Software\\Ubisoft\\Launcher\\Installs"))
                    {
                        if (RegKeyUplay != null)
                        {
                            foreach (string appId in RegKeyUplay.GetSubKeyNames())
                            {
                                try
                                {
                                    using (RegistryKey installDetails = RegKeyUplay.OpenSubKey(appId))
                                    {
                                        string installDir = installDetails.GetValue("InstallDir").ToString();
                                        installDir = AVFunctions.StringRemoveEnd(installDir, "/");
                                        await UplayAddApplication(appId, installDir);
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
                Debug.WriteLine("Failed adding uplay library: " + ex.Message);
            }
        }

        async Task UplayAddApplication(string appId, string installDir)
        {
            try
            {
                //Check if application is installed
                if (!Directory.Exists(installDir))
                {
                    Debug.WriteLine("Uplay game is not installed: " + appId);
                    return;
                }

                //Get launch argument
                string runCommand = "uplay://launch/" + appId + "/0";
                vLauncherAppAvailableCheck.Add(runCommand);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.Where(x => x.PathExe.ToLower() == runCommand.ToLower()).FirstOrDefault();
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Uplay app already in list: " + appIds);
                    return;
                }

                //Get application name
                string appName = Path.GetFileName(installDir);

                //Get application image
                //Fix open yaml configurations and look for image (Uplay Launcher\cache\assets)
                //string configurationsPath = Path.Combine(UplayInstallPath(), "cache\\configuration\\configurations"); > thumbimage
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, "Uplay" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 90, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Uplay,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncher = vImagePreloadUplay
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added uplay app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding uplay app: " + appId);
            }
        }
    }
}