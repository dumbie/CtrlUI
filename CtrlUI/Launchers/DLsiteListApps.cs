using LiteDB;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static CtrlUI.Classes;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task DLsiteScanAddLibrary()
        {
            try
            {
                //Fix run applications in Japanese language to fix start crash (LEProc)

                //Get launcher paths
                string roamingDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string databasePath = Path.Combine(roamingDataPath, "DLsiteNest\\dlsite.db");

                //Open database file
                ConnectionString liteConnection = new ConnectionString()
                {
                    Filename = databasePath,
                    ReadOnly = true
                };

                using (LiteDatabase liteDb = new LiteDatabase(liteConnection))
                {
                    //Merge uncommited log file
                    liteDb.Commit();

                    //Get all product information
                    var productInfoList = liteDb.GetCollection<DLsiteProductAdditionalInfo>("productAdditionalInfo");
                    foreach (DLsiteProductAdditionalInfo productInfo in productInfoList.FindAll())
                    {
                        try
                        {
                            //Set executable path
                            string executablePath = string.Empty;
                            if (!string.IsNullOrWhiteSpace(productInfo.ExeFilePath))
                            {
                                executablePath = productInfo.ExeFilePath;
                            }
                            else
                            {
                                //Scan DownloadPath for executable files and select
                                string[] exeBlacklist = { "setup", "config", "cfg", "notification_helper", "crashhandler" };
                                string[] exePaths = Directory.GetFiles(productInfo.DownloadPath, "*.exe", SearchOption.AllDirectories);
                                var exePathsFiltered = exePaths.Where(x => !exeBlacklist.Any(y => x.ToLower().Contains(y.ToLower())));
                                executablePath = exePathsFiltered.FirstOrDefault();
                            }

                            //Add application to list
                            if (!string.IsNullOrWhiteSpace(executablePath))
                            {
                                string appName = productInfo.WorkName;
                                await DLsiteAddApplication(appName, executablePath, executablePath);
                            }
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding DLsite library: " + ex.Message);
            }
        }

        async Task DLsiteAddApplication(string appName, string appImage, string executablePath)
        {
            try
            {
                //Add application to check list
                vLauncherAppAvailableCheck.Add(executablePath);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == executablePath.ToLower());
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Launcher app already in list: " + appName);
                    return;
                }

                //Check if application name is ignored
                string appNameLower = appName.ToLower();
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher app is on the blacklist: " + appName);
                    return;
                }

                //Load application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, appImage, "DLsite" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.DLsite,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = executablePath,
                    StatusLauncherImage = vImagePreloadDLsite
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added DLsite game: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding DLsite game: " + appName);
            }
        }
    }
}