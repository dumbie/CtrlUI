using ArnoldVinkCode;
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
using static ArnoldVinkCode.AVActions;
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
                AnswerRemove.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Remove.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerRemove.Name = "Move shortcut file to recycle bin";
                Answers.Add(AnswerRemove);

                DataBindString messageResult = await Popup_Show_MessageBox("Shortcut target path no longer exists", string.Empty, "Would you like to remove the shortcut file?", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerRemove)
                    {
                        await RemoveShortcutFile(lb_Shortcuts, lb_Shortcuts.SelectedIndex, dataBindApp, true);
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

                        //Check shortcut type
                        if (Check_PathUrlProtocol(targetPath))
                        {
                            shortcutDetails.Type = ShortcutType.UrlProtocol;
                        }
                        else if (Check_PathUwpApplication(targetPath))
                        {
                            shortcutDetails.Type = ShortcutType.UWP;
                        }
                        else
                        {
                            shortcutDetails.NameExe = Path.GetFileName(targetPath);
                        }

                        //Set shortcut details
                        shortcutDetails.Title = StripShortcutFilename(Path.GetFileNameWithoutExtension(shortcutPath));
                        shortcutDetails.TargetPath = targetPath;
                        shortcutDetails.WorkingPath = workingPath;
                        shortcutDetails.IconIndex = iconIndex;
                        shortcutDetails.IconPath = iconPath;
                        shortcutDetails.ShortcutPath = shortcutPath;
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
                //Check if application is activated
                if (!vAppActivated)
                {
                    return;
                }

                //Check if already refreshing
                if (vBusyRefreshingShortcuts)
                {
                    Debug.WriteLine("Shortcuts are already refreshing, cancelling.");
                    return;
                }

                //Check last update time
                long updateTime = GetSystemTicksMs();
                long updateOffset = updateTime - vLastUpdateShortcuts;
                if (updateOffset < 30000)
                {
                    //Debug.WriteLine("Shortcuts recently refreshed, cancelling.");
                    return;
                }

                //Update refreshing status
                vLastUpdateShortcuts = updateTime;
                vBusyRefreshingShortcuts = true;

                //Show the loading gif
                AVActions.DispatcherInvoke(delegate
                {
                    gif_List_Loading.Show();
                });

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
                        string targetTitleLower = shortcutDetails.Title.ToLower();
                        string targetPathLower = shortcutDetails.TargetPath.ToLower();
                        string targetArgumentLower = shortcutDetails.Argument.ToLower();

                        //Set shortcut filters
                        Func<DataBindApp, bool> combineCheckShortcut = x => x.Name.ToLower() == targetTitleLower || x.PathExe.ToLower() == targetPathLower || x.AppUserModelId.ToLower() == targetPathLower;
                        Func<ProfileShared, bool> blacklistCheckShortcut = x => x.String1.ToLower() == targetTitleLower;
                        Func<DataBindApp, bool> duplicateCheckShortcut = x => (x.PathExe.ToLower() == targetPathLower && x.Argument.ToLower() == targetArgumentLower) || x.AppUserModelId.ToLower() == targetPathLower;

                        //Check if already in combined list and remove it
                        if (CombineAppLists(true, true, true, true, false, false).Any(combineCheckShortcut))
                        {
                            //Debug.WriteLine("Shortcut is in the combined list skipping: " + targetPathLower);
                            await ListBoxRemoveAll(lb_Shortcuts, List_Shortcuts, x => x.PathExe.ToLower() == targetPathLower);
                            continue;
                        }

                        //Check if shortcut name is in shortcut blacklist
                        if (vCtrlIgnoreShortcutName.Any(blacklistCheckShortcut))
                        {
                            //Debug.WriteLine("Shortcut is on the blacklist skipping: " + targetTitleLower);
                            await ListBoxRemoveAll(lb_Shortcuts, List_Shortcuts, x => x.PathExe.ToLower() == targetPathLower);
                            continue;
                        }

                        //Check if shortcut is already in the shortcut list
                        DataBindApp shortcutExistCheck = List_Shortcuts.FirstOrDefault(duplicateCheckShortcut);
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

                //Hide the loading gif
                if (vBusyRefreshingCount() == 1)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        gif_List_Loading.Hide();
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed loading shortcuts: " + ex.Message);
            }
            finally
            {
                //Update list load status
                vListLoadedShortcuts = true;

                //Update the refreshing status
                vBusyRefreshingShortcuts = false;
            }
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
                Visibility shortcutUrlBrowser = Visibility.Collapsed;
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
                else if (shortcutDetails.Type == ShortcutType.UrlProtocol)
                {
                    //Check if shortcut is url protocol
                    shortcutUrlProtocol = Visibility.Visible;

                    //Check if url protocol is browser and set icon
                    if (targetPathLower.StartsWith("http") || targetPathLower.StartsWith("ftp"))
                    {
                        shortcutUrlProtocol = Visibility.Collapsed;
                        shortcutUrlBrowser = Visibility.Visible;
                    }

                    //Check if url protocol is launcher and set icon
                    if (targetPathLower.Contains("steam:"))
                    {
                        launcherImage = vImagePreloadSteam;
                    }
                    else if (targetPathLower.Contains("com.epicgames.launcher:"))
                    {
                        launcherImage = vImagePreloadEpic;
                    }
                    else if (targetPathLower.Contains("uplay:"))
                    {
                        launcherImage = vImagePreloadUbisoft;
                    }
                    else if (targetPathLower.Contains("battlenet:"))
                    {
                        launcherImage = vImagePreloadBattleNet;
                    }
                    else if (targetPathLower.Contains("origin:"))
                    {
                        launcherImage = vImagePreloadEADesktop;
                    }
                    else if (targetPathLower.Contains("link2ea:"))
                    {
                        launcherImage = vImagePreloadEADesktop;
                    }
                    else if (targetPathLower.Contains("amazon-games:"))
                    {
                        launcherImage = vImagePreloadAmazon;
                    }
                }
                else if (shortcutDetails.Type == ShortcutType.UWP)
                {
                    //Check if shortcut is windows store app
                    shortcutProcessType = ProcessType.UWP;
                    shortcutWindowStore = Visibility.Visible;

                    //Get basic application information
                    Package appPackage = GetUwpAppPackageByAppUserModelId(shortcutDetails.TargetPath);

                    //Check if the app still exists
                    if (appPackage == null)
                    {
                        shortcutAvailable = Visibility.Visible;
                    }
                    else
                    {
                        //Get detailed application information
                        AppxDetails appxDetails = GetUwpAppxDetailsByUwpAppPackage(appPackage);

                        //Set the application icon path
                        shortcutDetails.IconPath = appxDetails.SquareLargestLogoPath;

                        //Set the applicaton exe name
                        shortcutDetails.NameExe = appxDetails.ExecutableAliasName;
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
                    iconBitmapImage = vImagePreloadUnknownApp;
                }
                else
                {
                    iconBitmapImage = FileToBitmapImage(new string[] { shortcutDetails.Title, shortcutDetails.IconPath, targetPathLower, "Assets/Default/Extensions/" + targetExtensionLower + ".png" }, vImageSourceFoldersAppsCombined, vImageBackupSource, IntPtr.Zero, vImageLoadSize, shortcutDetails.IconIndex);
                }

                //Add the shortcut to the list
                DataBindApp dataBindApp = new DataBindApp() { Type = shortcutProcessType, Category = AppCategory.Shortcut, Name = shortcutDetails.Title, NameExe = shortcutDetails.NameExe, ImageBitmap = iconBitmapImage, PathLaunch = shortcutDetails.WorkingPath, ShortcutPath = shortcutDetails.ShortcutPath, Argument = shortcutDetails.Argument, StatusStore = shortcutWindowStore, StatusUrlProtocol = shortcutUrlProtocol, StatusUrlBrowser = shortcutUrlBrowser, StatusLauncherImage = launcherImage, TimeCreation = shortcutDetails.TimeModify, StatusAvailable = shortcutAvailable };
                if (shortcutDetails.Type == ShortcutType.UWP)
                {
                    dataBindApp.AppUserModelId = shortcutDetails.TargetPath;
                }
                else
                {
                    dataBindApp.PathExe = shortcutDetails.TargetPath;
                }

                await ListBoxAddItem(lb_Shortcuts, List_Shortcuts, dataBindApp, false, false);
            }
            catch
            {
                //Debug.WriteLine("Failed to add shortcut to list: " + shortcutDetails.ShortcutPath);
            }
        }
    }
}