using Shell32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.ApplicationModel;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.Settings;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Strip the shortcut file name
        string StripShortcutFilename(string shortcutFilename)
        {
            try
            {
                return shortcutFilename.Replace(".lnk", string.Empty).Replace(".url", string.Empty).Replace(".exe - Shortcut", string.Empty).Replace(" - Shortcut", string.Empty);
            }
            catch { }
            return shortcutFilename;
        }

        //Remove shortcut file prompt
        async Task RemoveShortcutFilePrompt(DataBindApp dataBindApp)
        {
            try
            {
                List<DataBindString> Answers = new List<DataBindString>();

                DataBindString AnswerRemove = new DataBindString();
                AnswerRemove.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Remove.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerRemove.Name = "Move shortcut file to recycle bin";
                Answers.Add(AnswerRemove);

                DataBindString messageResult = await Popup_Show_MessageBox("Shortcut target path no longer exists", string.Empty, "Would you like to remove the shortcut file?", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerRemove)
                    {
                        await RemoveShortcutFile(lb_Shortcuts, lb_Shortcuts.SelectedIndex, dataBindApp);
                    }
                }
            }
            catch { }
        }

        //Get details from a shortcut file
        ShortcutDetails ReadShortcutFile(string shortcutPath)
        {
            ShortcutDetails shortcutDetails = new ShortcutDetails();
            try
            {
                Thread thread = new Thread(delegate ()
                {
                    try
                    {
                        string folderString = Path.GetDirectoryName(shortcutPath);
                        string filenameString = Path.GetFileName(shortcutPath);
                        //Debug.WriteLine("Reading shortcut: " + shortcutPath);

                        Shell shell = new Shell();
                        Folder folder = shell.NameSpace(folderString);
                        FolderItem folderItem = folder.ParseName(filenameString);
                        ShellLinkObject shellLinkObject = folderItem.GetLink;

                        int iconIndex = 0;
                        string iconPath = string.Empty;
                        try
                        {
                            iconIndex = shellLinkObject.GetIconLocation(out iconPath);
                            iconPath = iconPath.Replace("file:///", string.Empty);
                            iconPath = WebUtility.UrlDecode(iconPath);
                        }
                        catch { }

                        string argumentString = string.Empty;
                        try
                        {
                            argumentString = shellLinkObject.Arguments;
                        }
                        catch { }

                        //Expand environment variables
                        string targetPath = Environment.ExpandEnvironmentVariables(shellLinkObject.Target.Path);
                        string workingPath = Environment.ExpandEnvironmentVariables(shellLinkObject.WorkingDirectory);
                        iconPath = Environment.ExpandEnvironmentVariables(iconPath);
                        shortcutPath = Environment.ExpandEnvironmentVariables(shortcutPath);

                        //Set shortcut details
                        shortcutDetails.Title = StripShortcutFilename(Path.GetFileNameWithoutExtension(shortcutPath));
                        shortcutDetails.TargetPath = targetPath;
                        shortcutDetails.WorkingPath = workingPath;
                        shortcutDetails.NameExe = Path.GetFileName(shortcutDetails.TargetPath);
                        shortcutDetails.IconIndex = iconIndex;
                        shortcutDetails.IconPath = iconPath;
                        shortcutDetails.ShortcutPath = shortcutPath;
                        shortcutDetails.Type = folderItem.Type;
                        shortcutDetails.Argument = argumentString;
                        shortcutDetails.Comment = shellLinkObject.Description;
                        shortcutDetails.TimeModify = folderItem.ModifyDate;
                    }
                    catch { }
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            }
            catch { }
            return shortcutDetails;
        }

        //Get all the shortcuts and update the list
        async Task RefreshListShortcuts(bool showStatus)
        {
            try
            {
                //Check if shortcuts need to be updated
                if (!Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowOtherShortcuts")))
                {
                    //Debug.WriteLine("Shortcuts don't need to be updated, cancelling.");
                    return;
                }

                //Check if already refreshing
                if (vBusyRefreshingShortcuts)
                {
                    Debug.WriteLine("Shortcuts are already refreshing, cancelling.");
                    return;
                }

                //Update the refreshing status
                vBusyRefreshingShortcuts = true;

                //Show refresh status message
                if (showStatus)
                {
                    await Notification_Send_Status("Refresh", "Refreshing shortcuts");
                }

                //Get all files from the shortcut directories
                IEnumerable<FileInfo> directoryShortcuts = Enumerable.Empty<FileInfo>();
                foreach (ProfileShared shortcutFolder in vCtrlLocationsShortcut)
                {
                    try
                    {
                        string editedShortcutFolder = shortcutFolder.String1;
                        editedShortcutFolder = editedShortcutFolder.Replace("%DESKTOPUSER%", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                        editedShortcutFolder = editedShortcutFolder.Replace("%DESKTOPPUBLIC%", Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
                        if (Directory.Exists(editedShortcutFolder))
                        {
                            DirectoryInfo directoryInfo = new DirectoryInfo(editedShortcutFolder);
                            IEnumerable<FileInfo> filterShortcuts = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly).Where(x => x.Name.ToLower().EndsWith(".url") || x.Name.ToLower().EndsWith(".lnk"));
                            directoryShortcuts = directoryShortcuts.Concat(filterShortcuts);
                        }
                    }
                    catch { }
                }

                //Sort and filter the list by shortcut name
                directoryShortcuts = directoryShortcuts.OrderBy(x => x.Name);

                //Remove shortcuts that are no longer available from the list
                Func<DataBindApp, bool> filterShortcutApp = x => x.Category == AppCategory.Shortcut && !directoryShortcuts.Any(y => StripShortcutFilename(y.Name) == x.Name);
                await ListBoxRemoveAll(lb_Shortcuts, List_Shortcuts, filterShortcutApp);
                await ListBoxRemoveAll(lb_Search, List_Search, filterShortcutApp);

                //Get shortcut information and add it to the list
                foreach (FileInfo file in directoryShortcuts)
                {
                    try
                    {
                        //Read shortcut file information
                        ShortcutDetails shortcutDetails = ReadShortcutFile(file.FullName);
                        string targetPathLower = shortcutDetails.TargetPath.ToLower();
                        string targetArgumentLower = shortcutDetails.Argument.ToLower();

                        //Check if already in combined list and remove it
                        if (CombineAppLists(false, false, true).Any(x => x.PathExe.ToLower() == targetPathLower))
                        {
                            //Debug.WriteLine("Shortcut is in the combined list skipping: " + fileNameStripped.ToLower());
                            await ListBoxRemoveAll(lb_Shortcuts, List_Shortcuts, x => x.PathExe.ToLower() == targetPathLower);
                            continue;
                        }

                        //Check if shortcut name is in shortcut blacklist
                        if (vCtrlIgnoreShortcutName.Any(x => x.String1.ToLower() == shortcutDetails.Title.ToLower()))
                        {
                            //Debug.WriteLine("Shortcut is on the blacklist skipping: " + fileNameStripped.ToLower());
                            await ListBoxRemoveAll(lb_Shortcuts, List_Shortcuts, x => x.PathExe.ToLower() == targetPathLower);
                            continue;
                        }

                        //Check if shortcut uri is in shortcut uri blacklist
                        if (vCtrlIgnoreShortcutUri.Any(x => targetPathLower.Contains(x.String1.ToLower())))
                        {
                            //Debug.WriteLine("Shortcut uri is on the uri blacklist skipping: " + targetPathLower);
                            await ListBoxRemoveAll(lb_Shortcuts, List_Shortcuts, x => x.PathExe.ToLower() == targetPathLower);
                            continue;
                        }

                        //Check if shortcut is already in the shortcut list
                        DataBindApp shortcutExistCheck = List_Shortcuts.Where(x => x.PathExe.ToLower() == targetPathLower && x.Argument.ToLower() == targetArgumentLower).FirstOrDefault();
                        if (shortcutExistCheck != null)
                        {
                            //Debug.WriteLine("Shortcut is already in list, updating: " + targetPathLower);
                            shortcutExistCheck.Name = shortcutDetails.Title;
                            shortcutExistCheck.ShortcutPath = shortcutDetails.ShortcutPath;
                            continue;
                        }

                        //Add shortcut to the shortcuts list
                        await ShortcutAddToListWithDetails(shortcutDetails);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed loading shortcuts: " + ex.Message);
            }
            //Update the refreshing status
            vBusyRefreshingShortcuts = false;
        }

        //Add shortcut to the shortcuts list
        async Task ShortcutAddToListWithDetails(ShortcutDetails shortcutDetails)
        {
            try
            {
                //Check if the shortcut name is set
                if (string.IsNullOrWhiteSpace(shortcutDetails.Title))
                {
                    Debug.WriteLine("Shortcut name is not set, skipping shortcut.");
                    return;
                }

                //Get the shortcut target file name
                string targetPathLower = shortcutDetails.TargetPath.ToLower();
                string targetExtensionLower = Path.GetExtension(targetPathLower).Replace(".", string.Empty);
                Visibility shortcutUrlProtocol = Visibility.Collapsed;
                Visibility shortcutWindowStore = Visibility.Collapsed;
                Visibility shortcutAvailable = Visibility.Collapsed;
                ProcessType shortcutProcessType = ProcessType.Win32;
                BitmapImage launcherImage = null;

                //Check if executable or UrlProtocol app shortcut
                if (targetPathLower.EndsWith(".exe"))
                {
                    //Check if the executable still exists
                    if (!File.Exists(targetPathLower))
                    {
                        shortcutAvailable = Visibility.Visible;
                    }
                }
                else if (targetPathLower.EndsWith(".bat"))
                {
                    //Check if the bat file still exists
                    if (!File.Exists(targetPathLower))
                    {
                        shortcutAvailable = Visibility.Visible;
                    }
                    else
                    {
                        shortcutUrlProtocol = Visibility.Visible;
                    }
                }
                else if (targetPathLower.Contains("://"))
                {
                    //Check if shortcut is url protocol
                    shortcutUrlProtocol = Visibility.Visible;

                    //Check if url protocol is a launcher and set icon
                    if (targetPathLower.Contains("steam://"))
                    {
                        launcherImage = vImagePreloadSteam;
                    }
                    else if (targetPathLower.Contains("com.epicgames.launcher://"))
                    {
                        launcherImage = vImagePreloadEpic;
                    }
                    else if (targetPathLower.Contains("uplay://"))
                    {
                        launcherImage = vImagePreloadUplay;
                    }
                    else if (targetPathLower.Contains("battlenet://"))
                    {
                        launcherImage = vImagePreloadBattleNet;
                    }
                    else if (targetPathLower.Contains("bethesdanet://"))
                    {
                        launcherImage = vImagePreloadBethesda;
                    }
                    else if (targetPathLower.Contains("origin://"))
                    {
                        launcherImage = vImagePreloadOrigin;
                    }
                }
                else if (!targetPathLower.Contains("/") && targetPathLower.Contains("!") && targetPathLower.Contains("_"))
                {
                    //Check if shortcut is windows store app
                    shortcutProcessType = ProcessType.UWP;
                    shortcutWindowStore = Visibility.Visible;

                    //Get basic application information
                    Package appPackage = UwpGetAppPackageByAppUserModelId(shortcutDetails.TargetPath);

                    //Check if the app still exists
                    if (appPackage == null)
                    {
                        shortcutAvailable = Visibility.Visible;
                    }
                    else
                    {
                        //Get detailed application information
                        AppxDetails appxDetails = UwpGetAppxDetailsFromAppPackage(appPackage);

                        //Set the application icon path
                        shortcutDetails.IconPath = appxDetails.SquareLargestLogoPath;

                        //Set the applicaton exe name
                        shortcutDetails.NameExe = appxDetails.ExecutableName;
                    }
                }
                else
                {
                    //Debug.WriteLine("Unknown shortcut: " + TargetPathLower);
                    return;
                }

                //Get icon image from the path
                BitmapImage iconBitmapImage = null;
                if (shortcutAvailable == Visibility.Visible)
                {
                    iconBitmapImage = FileToBitmapImage(new string[] { "Unknown" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 90, 0);
                }
                else
                {
                    iconBitmapImage = FileToBitmapImage(new string[] { shortcutDetails.Title, shortcutDetails.IconPath, targetPathLower, "Assets/Default/Extensions/" + targetExtensionLower + ".png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 90, shortcutDetails.IconIndex);
                }

                //Add the shortcut to the list
                DataBindApp dataBindApp = new DataBindApp() { Type = shortcutProcessType, Category = AppCategory.Shortcut, Name = shortcutDetails.Title, NameExe = shortcutDetails.NameExe, ImageBitmap = iconBitmapImage, PathExe = shortcutDetails.TargetPath, PathLaunch = shortcutDetails.WorkingPath, ShortcutPath = shortcutDetails.ShortcutPath, Argument = shortcutDetails.Argument, StatusStore = shortcutWindowStore, StatusUrlProtocol = shortcutUrlProtocol, StatusLauncher = launcherImage, TimeCreation = shortcutDetails.TimeModify, StatusAvailable = shortcutAvailable };
                await ListBoxAddItem(lb_Shortcuts, List_Shortcuts, dataBindApp, false, false);
            }
            catch
            {
                //Debug.WriteLine("Failed to add shortcut to list: " + shortcutDetails.ShortcutPath);
            }
        }
    }
}