using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Update all uwp application images
        void UpdateUwpApplicationImages()
        {
            try
            {
                Debug.WriteLine("Checking all uwp application images for changes.");
                bool UpdatedImages = false;

                //Update all the uwp apps image paths
                foreach (DataBindApp dataBindApp in CombineAppLists(false, false).Where(x => x.Type == ProcessType.UWP || x.Type == ProcessType.Win32Store))
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(dataBindApp.PathImage) && !File.Exists(dataBindApp.PathImage))
                        {
                            Debug.WriteLine("Uwp application image not found: " + dataBindApp.PathImage);

                            //Get detailed application information
                            Package appPackage = UwpGetAppPackageFromAppUserModelId(dataBindApp.PathExe);
                            AppxDetails appxDetails = UwpGetAppxDetailsFromAppPackage(appPackage);

                            //Update the application icons
                            dataBindApp.PathImage = appxDetails.SquareLargestLogoPath;
                            dataBindApp.ImageBitmap = FileToBitmapImage(new string[] { dataBindApp.Name, appxDetails.SquareLargestLogoPath, appxDetails.WideLargestLogoPath }, IntPtr.Zero, 90);
                            UpdatedImages = true;
                        }
                    }
                    catch { }
                }

                //Save the updated uwp application images
                if (UpdatedImages) { JsonSaveApps(); }
            }
            catch { }
        }
    }
}