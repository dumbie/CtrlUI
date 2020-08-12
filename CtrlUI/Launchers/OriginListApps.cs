using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task OriginScanAddLibrary()
        {
            try
            {
                //Get origin icon image
                BitmapImage originBitmapImage = FileToBitmapImage(new string[] { "Origin" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 10, 0);

                //Get local content path
                string programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string localContentPath = Path.Combine(programDataPath, "Origin\\LocalContent");

                //Check local content paths
                string[] localContentDirectories = Directory.GetDirectories(localContentPath, "*");
                foreach (string localContentAppPath in localContentDirectories)
                {
                    try
                    {
                        await OriginAddApplication(localContentAppPath, originBitmapImage);
                    }
                    catch { }
                }
            }
            catch
            {
                Debug.WriteLine("Failed adding origin library.");
            }
        }

        async Task OriginAddApplication(string localContentAppPath, BitmapImage originBitmapImage)
        {
            try
            {
                //Get application name
                string appName = Path.GetFileName(localContentAppPath);

                //Get application mfst files
                string appIds = string.Empty;
                string[] localContentFiles = Directory.GetFiles(localContentAppPath, "*.mfst");

                //Check if application is installed
                if (!localContentFiles.Any())
                {
                    Debug.WriteLine("Origin game is not installed: " + appName);
                    return;
                }

                //Get application ids
                foreach (string localFile in localContentFiles)
                {
                    try
                    {
                        //Fix Open mfst > get dipinstallpath > get exe path from reg > image from exe
                        string appId = Path.GetFileNameWithoutExtension(localFile);
                        if (appId.StartsWith("OFB-EAST"))
                        {
                            appId = appId.Replace("OFB-EAST", "OFB-EAST:");
                        }
                        appIds += appId + ",";
                    }
                    catch { }
                }
                appIds = AVFunctions.StringRemoveEnd(appIds, ",");

                //Set run command
                string runCommand = "origin://LaunchGame/" + appIds;
                vLauncherAppAvailableCheck.Add(runCommand);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.Where(x => x.PathExe.ToLower() == runCommand.ToLower()).FirstOrDefault();
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Origin id already in list: " + appIds);
                    return;
                }

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, "Origin" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 90, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncher = originBitmapImage
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added origin app: " + appIds + "/" + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding origin app: " + localContentAppPath);
            }
        }
    }
}