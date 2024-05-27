using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.ApplicationModel;
using Windows.Gaming.Preview.GamesEnumeration;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkCode.AVUwpAppx;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Arrays
        private static string[] vUwpAppBlacklist = { "microsoft.windowsnotepad_8wekyb3d8bbwe", "windows.immersivecontrolpanel_cw5n1h2txyewy" };

        async Task UwpScanAddLibrary()
        {
            try
            {
                //Get all the installed uwp games
                IEnumerable<GameListEntry> uwpGameList = (await GameList.FindAllAsync()).Where(x => x.Category == GameListCategory.ConfirmedBySystem);
                if (!uwpGameList.Any())
                {
                    Debug.WriteLine("No installed uwp games found to list.");
                    return;
                }

                //Add all the installed uwp games
                foreach (GameListEntry uwpGame in uwpGameList)
                {
                    try
                    {
                        //Get and check uwp application FamilyName
                        string appFamilyName = uwpGame.Properties.FirstOrDefault(x => x.Key == "PackageFamilyName").Value.ToString();
                        if (string.IsNullOrWhiteSpace(appFamilyName))
                        {
                            continue;
                        }

                        //Check if application is in blacklist
                        if (vUwpAppBlacklist.Contains(appFamilyName.ToLower()))
                        {
                            Debug.WriteLine("UWP app is blacklisted: " + appFamilyName);
                            continue;
                        }

                        //Get uwp application package
                        Package appPackage = GetUwpAppPackageByFamilyName(appFamilyName);

                        //Get detailed application information
                        AppxDetails appxDetails = GetUwpAppxDetailsByUwpAppPackage(appPackage);

                        //Check if executable name is valid
                        if (string.IsNullOrWhiteSpace(appxDetails.ExecutableAliasName))
                        {
                            continue;
                        }

                        //Check if application name is valid
                        if (string.IsNullOrWhiteSpace(appxDetails.DisplayName) || appxDetails.DisplayName.StartsWith("ms-resource"))
                        {
                            continue;
                        }

                        //Add application to list
                        await UwpAddApplication(appxDetails);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding uwp games library: " + ex.Message);
            }
        }

        async Task UwpAddApplication(AppxDetails appxDetails)
        {
            try
            {
                //Get application name
                string appName = appxDetails.DisplayName;
                string appNameLower = appName.ToLower();

                //Check if application name is ignored
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher is on the blacklist skipping: " + appName);
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, x => x.Name.ToLower() == appNameLower);
                    return;
                }

                //Get basic application information
                string appUserModelId = appxDetails.AppUserModelId;
                vLauncherAppAvailableCheck.Add(appUserModelId);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.AppUserModelId.ToLower() == appUserModelId.ToLower());
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("UWP app already in list: " + appIds);
                    return;
                }

                //Load the application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, appxDetails.SquareLargestLogoPath, appxDetails.WideLargestLogoPath, "Microsoft" }, vImageSourceFoldersAppsCombined, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Type = ProcessType.UWP,
                    Launcher = AppLauncher.UWP,
                    Category = AppCategory.Launcher,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    NameExe = appxDetails.ExecutableAliasName,
                    AppUserModelId = appUserModelId,
                    StatusLauncherImage = vImagePreloadMicrosoft
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                Debug.WriteLine("Added UWP app: " + appName + "/" + appUserModelId);
            }
            catch
            {
                Debug.WriteLine("Failed adding UWP app: " + appxDetails.DisplayName);
            }
        }

        //Uninstall uwp application
        async Task UwpListUninstallApplication(DataBindFile selectedItem)
        {
            try
            {
                await Notification_Send_Status("RemoveCross", "Uninstalling " + selectedItem.Name);

                //Remove application from pc
                bool uwpRemoved = UwpRemoveApplicationByPackageFullName(selectedItem.PathFull);

                //Remove application from list
                if (uwpRemoved)
                {
                    await ListBoxRemoveItem(lb_FilePicker, List_FilePicker, selectedItem, true);
                }
            }
            catch { }
        }
    }
}