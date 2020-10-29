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
        string UbisoftInstallPath()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    //Search for Ubisoft install directory
                    using (RegistryKey RegKeyUbisoft = registryKeyLocalMachine.OpenSubKey("Software\\Ubisoft\\Launcher"))
                    {
                        if (RegKeyUbisoft != null)
                        {
                            string RegKeyExePath = RegKeyUbisoft.GetValue("InstallDir").ToString() + "upc.exe";
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

        async Task UbisoftScanAddLibrary()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey RegKeyUbisoft = registryKeyLocalMachine.OpenSubKey("Software\\Ubisoft\\Launcher\\Installs"))
                    {
                        if (RegKeyUbisoft != null)
                        {
                            foreach (string appId in RegKeyUbisoft.GetSubKeyNames())
                            {
                                try
                                {
                                    using (RegistryKey installDetails = RegKeyUbisoft.OpenSubKey(appId))
                                    {
                                        string installDir = installDetails.GetValue("InstallDir").ToString();
                                        installDir = AVFunctions.StringRemoveEnd(installDir, "/");
                                        await UbisoftAddApplication(appId, installDir);
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
                Debug.WriteLine("Failed adding Ubisoft library: " + ex.Message);
            }
        }

        async Task UbisoftAddApplication(string appId, string installDir)
        {
            try
            {
                //Check if application is installed
                if (!Directory.Exists(installDir))
                {
                    Debug.WriteLine("Ubisoft game is not installed: " + appId);
                    return;
                }

                //Get launch argument
                string runCommand = "uplay://launch/" + appId + "/0";
                vLauncherAppAvailableCheck.Add(runCommand);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.Where(x => x.PathExe.ToLower() == runCommand.ToLower()).FirstOrDefault();
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Ubisoft app already in list: " + appIds);
                    return;
                }

                //Get application name
                string appName = Path.GetFileName(installDir);

                //Check if application name is ignored
                string appNameLower = appName.ToLower();
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher is on the blacklist skipping: " + appName);
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, x => x.Name.ToLower() == appNameLower);
                    return;
                }

                //Get application image
                //Fix open yaml configurations and look for image (Ubisoft Launcher\cache\assets)
                //string configurationsPath = Path.Combine(UbisoftInstallPath(), "cache\\configuration\\configurations"); > thumbimage
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, "Ubisoft" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 90, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Ubisoft,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncher = vImagePreloadUbisoft
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Ubisoft app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Ubisoft app: " + appId);
            }
        }
    }
}