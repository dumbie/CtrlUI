using System;
using System.Collections.Generic;
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
        async Task GooglePlayScanAddLibrary()
        {
            try
            {
                //Get app shortcuts path
                string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string shortcutPath = Path.Combine(roamingPath, "Microsoft\\Windows\\Start Menu\\Programs\\Google Play Games");

                //Get all shortcut files
                DirectoryInfo directoryInfo = new DirectoryInfo(shortcutPath);
                IEnumerable<FileInfo> fileInfo = directoryInfo.GetFiles();

                //Get details from shortcut files
                foreach (FileInfo shortcutFile in fileInfo)
                {
                    try
                    {
                        ShortcutDetails shortcutDetails = ReadShortcutFile(shortcutFile.FullName);
                        await GooglePlayAddApplication(shortcutDetails);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding google play library: " + ex.Message);
            }
        }

        async Task GooglePlayAddApplication(ShortcutDetails shortcutDetails)
        {
            try
            {
                //Add application to check list
                vLauncherAppAvailableCheck.Add(shortcutDetails.TargetPath);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == shortcutDetails.TargetPath.ToLower());
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Launcher app already in list: " + shortcutDetails.TargetPath);
                    return;
                }

                //Check if application name is ignored
                string appNameLower = shortcutDetails.Title.ToLower();
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher app is on the blacklist: " + appName);
                    return;
                }

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { shortcutDetails.Title, shortcutDetails.IconPath, "Google Play" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.GooglePlay,
                    Name = shortcutDetails.Title,
                    ImageBitmap = iconBitmapImage,
                    PathExe = shortcutDetails.TargetPath,
                    StatusLauncherImage = vImagePreloadGooglePlay
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added google play app: " + shortcutDetails.Title);
            }
            catch
            {
                Debug.WriteLine("Failed adding google play app: " + shortcutDetails.Title);
            }
        }
    }
}