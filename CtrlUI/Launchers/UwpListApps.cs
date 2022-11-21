using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.ApplicationModel;
using Windows.Gaming.Preview.GamesEnumeration;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
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

                        //Get uwp application package
                        Package appPackage = UwpGetAppPackageByFamilyName(appFamilyName);

                        //Get detailed application information
                        AppxDetails appxDetails = UwpGetAppxDetailsFromAppPackage(appPackage);

                        //Check if executable name is valid
                        if (string.IsNullOrWhiteSpace(appxDetails.ExecutableName))
                        {
                            continue;
                        }

                        //Check if application name is valid
                        if (string.IsNullOrWhiteSpace(appxDetails.DisplayName) || appxDetails.DisplayName.StartsWith("ms-resource"))
                        {
                            continue;
                        }

                        await UwpAddApplication(appPackage, appxDetails);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding uwp games library: " + ex.Message);
            }
        }

        async Task UwpAddApplication(Package appPackage, AppxDetails appxDetails)
        {
            try
            {
                //Get application name
                string appName = appxDetails.DisplayName;

                //Check if application name is ignored
                string appNameLower = appName.ToLower();
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher is on the blacklist skipping: " + appName);
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, x => x.Name.ToLower() == appNameLower);
                    return;
                }

                //Get basic application information
                string runCommand = appPackage.Id.FamilyName;
                vLauncherAppAvailableCheck.Add(runCommand);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.Where(x => x.PathExe.ToLower() == runCommand.ToLower()).FirstOrDefault();
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("UWP app already in list: " + appIds);
                    return;
                }

                //Load the application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, appxDetails.SquareLargestLogoPath, appxDetails.WideLargestLogoPath, "Microsoft" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vListBoxImageSize, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Type = ProcessType.UWP,
                    Launcher = AppLauncher.UWP,
                    Category = AppCategory.Launcher,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncherImage = vImagePreloadMicrosoft
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added UWP app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding UWP app: " + appxDetails.DisplayName);
            }
        }
    }
}