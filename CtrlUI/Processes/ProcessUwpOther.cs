using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkCode.AVUwpAppx;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Update uwp application
        async Task UwpListUpdateApplication(DataBindFile selectedItem)
        {
            try
            {
                await Notification_Send_Status("Refresh", "Updating " + selectedItem.Name);

                //Update application from list
                UwpUpdateApplicationByAppUserModelId(selectedItem.PathFile);
            }
            catch { }
        }

        //Remove uwp application
        async Task UwpListRemoveApplication(DataBindFile selectedItem)
        {
            try
            {
                await Notification_Send_Status("RemoveCross", "Removing " + selectedItem.Name);

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

        //Update all uwp application images
        void UpdateUwpApplicationImages()
        {
            try
            {
                Debug.WriteLine("Checking all uwp application images for changes.");
                bool UpdatedImages = false;

                //Update all the uwp apps image paths
                foreach (DataBindApp dataBindApp in CombineAppLists(false, false, false).Where(x => x.Type == ProcessType.UWP || x.Type == ProcessType.Win32Store))
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(dataBindApp.PathImage) && !File.Exists(dataBindApp.PathImage))
                        {
                            Debug.WriteLine("Uwp application image not found: " + dataBindApp.PathImage);

                            //Get detailed application information
                            Package appPackage = GetUwpAppPackageByAppUserModelId(dataBindApp.PathExe);
                            AppxDetails appxDetails = GetUwpAppxDetailsByAppPackage(appPackage);

                            //Update the application icons
                            dataBindApp.PathImage = appxDetails.SquareLargestLogoPath;
                            dataBindApp.ImageBitmap = FileToBitmapImage(new string[] { dataBindApp.Name, appxDetails.SquareLargestLogoPath, appxDetails.WideLargestLogoPath }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
                            UpdatedImages = true;
                        }
                    }
                    catch { }
                }

                //Save the updated uwp application images
                if (UpdatedImages) { JsonSaveApplications(); }
            }
            catch { }
        }
    }
}