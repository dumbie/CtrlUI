using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task HoYoPlayScanAddLibrary()
        {
            try
            {
                //Json source: AppData\Roaming\Cognosphere\HYP\1_0\data\gamedata.dat
                //Icon source: %APPDATA%\Cognosphere\HYP\1_0\ico\*id*.ico
                //Shortcuts: Start Menu\Programs\HoYoPlay\

                //Open the Windows registry
                using (RegistryKey registryKeyLocal = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    using (RegistryKey regKeyUninstall = registryKeyLocal.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
                    {
                        if (regKeyUninstall != null)
                        {
                            //Filter HoYoPlay applications
                            var regKeyGames = regKeyUninstall.GetSubKeyNames().Where(x => x.EndsWith("_production")).ToList();
                            foreach (string appId in regKeyGames)
                            {
                                try
                                {
                                    using (RegistryKey installDetails = regKeyUninstall.OpenSubKey(appId))
                                    {
                                        string uninstallString = installDetails.GetValue("UninstallString").ToString();
                                        if (uninstallString.Contains("HoYoPlay"))
                                        {
                                            string displayName = installDetails.GetValue("DisplayName").ToString();
                                            string displayIcon = installDetails.GetValue("DisplayIcon").ToString();
                                            string[] uninstallSplit = uninstallString.Split("--");
                                            string executablePath = uninstallSplit.FirstOrDefault();
                                            string executeArguments = string.Empty;
                                            foreach (string splitString in uninstallSplit.Skip(1))
                                            {
                                                executeArguments += "--" + splitString;
                                            }
                                            executeArguments = executeArguments.Replace("--uninstall_game", "--game");
                                            await HoYoPlayAddApplication(displayName, displayIcon, executablePath, executeArguments);
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
                Debug.WriteLine("Failed adding HoYoPlay library: " + ex.Message);
            }
        }

        async Task HoYoPlayAddApplication(string displayName, string displayIcon, string executablePath, string executeArguments)
        {
            try
            {
                //Add application to check list
                vLauncherAppAvailableCheck.Add(executablePath);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == executablePath.ToLower() && x.Argument.ToLower() == executeArguments.ToLower());
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
                BitmapImage bitmapImageApplication = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = [displayName, displayIcon, "HoYoPlay"],
                    SearchPaths = vImageSourceFoldersAppsCombined,
                    BackupPath = vImageBackupSource,
                    Width = vImageLoadSizeApplication,
                    Dispatcher = this.Dispatcher
                });

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.HoYoPlay,
                    Name = displayName,
                    ImageBitmap = bitmapImageApplication,
                    PathExe = executablePath,
                    Argument = executeArguments,
                    StatusLauncherImage = await LoadLauncherImage(AppLauncher.HoYoPlay, vImageLoadSizeApplication, 0)
                };

                await ListViewAddItem(listView_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added HoYoPlay app: " + displayName);
            }
            catch
            {
                Debug.WriteLine("Failed adding HoYoPlay app: " + displayName);
            }
        }
    }
}