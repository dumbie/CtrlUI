﻿using Microsoft.Win32;
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
        async Task RiotScanAddLibrary()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey regKeyUninstall = registryKeyCurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
                    {
                        if (regKeyUninstall != null)
                        {
                            //Filter Riot applications
                            var regKeyGames = regKeyUninstall.GetSubKeyNames().Where(x => x.StartsWith("Riot Game ")).ToList();
                            foreach (string appId in regKeyGames)
                            {
                                try
                                {
                                    if (!appId.Contains("Riot_Client"))
                                    {
                                        using (RegistryKey installDetails = regKeyUninstall.OpenSubKey(appId))
                                        {
                                            string displayName = installDetails.GetValue("DisplayName").ToString();
                                            string displayIcon = installDetails.GetValue("DisplayIcon").ToString();
                                            string uninstallString = installDetails.GetValue("UninstallString").ToString().Replace("\"", string.Empty);
                                            string[] uninstallSplit = uninstallString.Split("--");
                                            string executablePath = uninstallSplit.FirstOrDefault();
                                            string executeArguments = string.Empty;
                                            foreach (string splitString in uninstallSplit.Skip(1))
                                            {
                                                executeArguments += "--" + splitString;
                                            }
                                            executeArguments = executeArguments.Replace("-uninstall", "-launch");
                                            await RiotAddApplication(displayName, displayIcon, executablePath, executeArguments);
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
                Debug.WriteLine("Failed adding Riot library: " + ex.Message);
            }
        }

        async Task RiotAddApplication(string displayName, string displayIcon, string executablePath, string executeArguments)
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
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { displayName, displayIcon, "Riot" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Riot,
                    Name = displayName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = executablePath,
                    Argument = executeArguments,
                    StatusLauncherImage = vImagePreloadRiot
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Riot app: " + displayName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Riot app: " + displayName);
            }
        }
    }
}