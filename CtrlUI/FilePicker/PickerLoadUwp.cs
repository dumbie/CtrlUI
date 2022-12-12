using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.ApplicationModel;
using Windows.Management.Deployment;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVUwpAppx;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Get and list all uwp applications
        async Task FilePicker_LoadUwpApps()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Enable or disable selection button in the list
                    grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Collapsed;

                    //Enable or disable file and folder availability
                    grid_Popup_FilePicker_textblock_NoFilesAvailable.Visibility = Visibility.Collapsed;

                    //Enable or disable the side navigate buttons
                    grid_Popup_FilePicker_button_ControllerLeft.Visibility = Visibility.Visible;
                    grid_Popup_FilePicker_button_ControllerUp.Visibility = Visibility.Collapsed;
                    grid_Popup_FilePicker_button_ControllerBack.Visibility = Visibility.Collapsed;
                    grid_Popup_FilePicker_button_ControllerStart.Visibility = Visibility.Collapsed;

                    //Enable or disable the copy paste status
                    grid_Popup_FilePicker_textblock_ClipboardStatus.Visibility = Visibility.Collapsed;

                    //Enable or disable the current path
                    grid_Popup_FilePicker_textblock_CurrentPath.Visibility = Visibility.Collapsed;
                });

                //Set uwp application filters
                string[] whiteListFamilyName = { "Microsoft.MicrosoftEdge_8wekyb3d8bbwe" };
                string[] blackListAppUserModelId = { "Microsoft.MicrosoftEdge_8wekyb3d8bbwe!PdfReader" };

                //Get all the installed uwp apps
                PackageManager deployPackageManager = new PackageManager();
                string currentUserIdentity = WindowsIdentity.GetCurrent().User.Value;
                IEnumerable<Package> appPackages = deployPackageManager.FindPackagesForUser(currentUserIdentity);
                foreach (Package appPackage in appPackages)
                {
                    try
                    {
                        //Cancel loading
                        if (vFilePickerLoadCancel)
                        {
                            Debug.WriteLine("File picker uwp apps load cancelled.");
                            return;
                        }

                        //Get basic application information
                        string appFamilyName = appPackage.Id.FamilyName;

                        //Check if the application is in whitelist
                        if (!whiteListFamilyName.Contains(appFamilyName))
                        {
                            //Filter out system apps and others
                            if (appPackage.IsBundle) { continue; }
                            if (appPackage.IsOptional) { continue; }
                            if (appPackage.IsFramework) { continue; }
                            if (appPackage.IsResourcePackage) { continue; }
                            if (appPackage.SignatureKind != PackageSignatureKind.Store) { continue; }
                        }

                        //Get detailed application information
                        AppxDetails appxDetails = GetUwpAppxDetailsFromAppPackage(appPackage);

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

                        //Check if the application is in blacklist
                        if (blackListAppUserModelId.Contains(appxDetails.AppUserModelId))
                        {
                            continue;
                        }

                        //Load the application image
                        BitmapImage uwpListImage = FileToBitmapImage(new string[] { appxDetails.SquareLargestLogoPath, appxDetails.WideLargestLogoPath }, null, vImageBackupSource, IntPtr.Zero, 50, 0);

                        //Add the application to the list
                        DataBindFile dataBindFile = new DataBindFile() { FileType = FileType.UwpApp, Name = appxDetails.DisplayName, NameExe = appxDetails.ExecutableName, PathFile = appxDetails.AppUserModelId, PathFull = appxDetails.FullPackageName, PathImage = appxDetails.SquareLargestLogoPath, ImageBitmap = uwpListImage };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFile, false, false);
                    }
                    catch { }
                }

                //Sort the application list by name
                SortFunction<DataBindFile> sortFuncName = new SortFunction<DataBindFile>();
                sortFuncName.function = x => x.Name;

                List<SortFunction<DataBindFile>> orderListPicker = new List<SortFunction<DataBindFile>>();
                orderListPicker.Add(sortFuncName);

                SortObservableCollection(lb_FilePicker, List_FilePicker, orderListPicker, null);
            }
            catch { }
        }
    }
}