using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task EADesktopScanAddLibrary()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey registryKeyUninstall = registryKeyLocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
                    {
                        if (registryKeyUninstall != null)
                        {
                            foreach (string uninstallApp in registryKeyUninstall.GetSubKeyNames())
                            {
                                try
                                {
                                    using (RegistryKey installDetails = registryKeyUninstall.OpenSubKey(uninstallApp))
                                    {
                                        string uninstallString = installDetails.GetValue("UninstallString")?.ToString();
                                        if (uninstallString.Contains("EAInstaller"))
                                        {
                                            string appName = installDetails.GetValue("DisplayName")?.ToString();
                                            string appIcon = installDetails.GetValue("DisplayIcon")?.ToString().Replace("\"", string.Empty);
                                            string installDir = installDetails.GetValue("InstallLocation")?.ToString().Replace("\"", string.Empty);
                                            await EADesktopAddApplication(appName, appIcon, installDir);
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Failed adding EA Desktop library.");
            }

            string EADesktopGetContentID(string installDir)
            {
                try
                {
                    //Open installer data xml file
                    string xmlDataPath = Path.Combine(installDir, @"__Installer\InstallerData.xml");
                    //Debug.WriteLine("EA install xml path: " + xmlDataPath);

                    //Get content ids from data
                    string contentIds = string.Empty;
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(xmlDataPath);

                    XmlNodeList nodeIdsList = xmlDocument.GetElementsByTagName("contentIDs");
                    foreach (XmlNode nodeIds in nodeIdsList)
                    {
                        XmlNodeList nodeIdList = nodeIds.SelectNodes("contentID");
                        foreach (XmlNode nodeId in nodeIdList)
                        {
                            contentIds += nodeId.InnerText + ",";
                            contentIds += nodeId.InnerText + "_pc,";
                        }
                    }

                    return AVFunctions.StringRemoveEnd(contentIds, ",");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to get EA contentID: " + ex.Message);
                    return string.Empty;
                }
            }

            async Task EADesktopAddApplication(string appName, string appIcon, string installDir)
            {
                try
                {
                    //Get contentIds
                    string contentIds = EADesktopGetContentID(installDir);
                    if (string.IsNullOrWhiteSpace(contentIds))
                    {
                        Debug.WriteLine("No EA contentId found for: " + appName);
                        return;
                    }

                    //Set run command
                    string runCommand = "origin://LaunchGame/" + contentIds;
                    vLauncherAppAvailableCheck.Add(runCommand);

                    //Check if application is already added
                    DataBindApp launcherExistCheck = List_Launchers.Where(x => x.PathExe.ToLower() == runCommand.ToLower()).FirstOrDefault();
                    if (launcherExistCheck != null)
                    {
                        //Debug.WriteLine("EA Desktop app already in list: " + appIds);
                        return;
                    }

                    //Get application name
                    string appNameLower = appName.ToLower();

                    //Check if application name is ignored
                    if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                    {
                        //Debug.WriteLine("Launcher is on the blacklist skipping: " + appName);
                        await ListBoxRemoveAll(lb_Launchers, List_Launchers, x => x.Name.ToLower() == appNameLower);
                        return;
                    }

                    //Get application image
                    BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, appIcon, "EA Desktop" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 90, 0);

                    //Add the application to the list
                    DataBindApp dataBindApp = new DataBindApp()
                    {
                        Category = AppCategory.Launcher,
                        Launcher = AppLauncher.EADesktop,
                        Name = appName,
                        ImageBitmap = iconBitmapImage,
                        PathExe = runCommand,
                        StatusLauncherImage = vImagePreloadEADesktop
                    };

                    await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                    //Debug.WriteLine("Added EA Desktop app: " + appIds + "/" + appName);
                }
                catch
                {
                    Debug.WriteLine("Failed adding EA Desktop app: " + appName);
                }
            }
        }
    }
}