//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Security.Principal;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Media.Imaging;
//using Windows.ApplicationModel;
//using Windows.Management.Deployment;
//using static ArnoldVinkCode.AVImage;
//using static ArnoldVinkCode.ProcessClasses;
//using static ArnoldVinkCode.ProcessUwpFunctions;
//using static CtrlUI.AppVariables;

//namespace CtrlUI
//{
//    partial class WindowMain
//    {
//        async Task UwpScanAddLibrary()
//        {
//            try
//            {
//                //Get launcher icon image
//                BitmapImage launcherImage = FileToBitmapImage(new string[] { "Xbox" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 10, 0);

//                //Load uwp games executables
//                string commonApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
//                string knownGameListPath = Path.Combine(commonApplicationDataPath, "Microsoft\\GameDVR\\KnownGameList.bin");
//                string knownGameListString = File.ReadAllText(knownGameListPath, Encoding.Unicode);
//                //KnownGameList unfortunately does not contain uwp games

//                //Get all the installed uwp apps
//                PackageManager deployPackageManager = new PackageManager();
//                string currentUserIdentity = WindowsIdentity.GetCurrent().User.Value;
//                IEnumerable<Package> appPackages = deployPackageManager.FindPackagesForUser(currentUserIdentity);
//                foreach (Package appPackage in appPackages)
//                {
//                    try
//                    {
//                        //Get detailed application information
//                        AppxDetails appxDetails = UwpGetAppxDetailsFromAppPackage(appPackage);

//                        //Check if executable name is valid
//                        if (string.IsNullOrWhiteSpace(appxDetails.ExecutableName))
//                        {
//                            continue;
//                        }

//                        //Check if application name is valid
//                        if (string.IsNullOrWhiteSpace(appxDetails.DisplayName) || appxDetails.DisplayName.StartsWith("ms-resource"))
//                        {
//                            continue;
//                        }

//                        //Check if the application is a game
//                        if (!knownGameListString.Contains(appxDetails.ExecutableName))
//                        {
//                            Debug.WriteLine(appxDetails.ExecutableName + " is not a uwp game.");
//                            continue;
//                        }

//                        //Get basic application information
//                        string appFamilyName = appPackage.Id.FamilyName;

//                        //Load the application image
//                        BitmapImage uwpListImage = FileToBitmapImage(new string[] { appxDetails.SquareLargestLogoPath, appxDetails.WideLargestLogoPath }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 50, 0);
//                    }
//                    catch { }
//                }
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine("Failed adding uwp games library: " + ex.Message);
//            }
//        }
//    }
//}