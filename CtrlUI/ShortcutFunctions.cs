using ArnoldVinkCode;
using Shell32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.ApplicationModel;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
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

                        string iconPath = string.Empty;
                        try
                        {
                            shellLinkObject.GetIconLocation(out iconPath);
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

                        shortcutDetails.Name = Path.GetFileNameWithoutExtension(shortcutPath).Replace(".lnk", string.Empty).Replace(".url", string.Empty).Replace(".exe - Shortcut", string.Empty).Replace(" - Shortcut", string.Empty);
                        shortcutDetails.TargetPath = shellLinkObject.Target.Path;
                        shortcutDetails.WorkingPath = shellLinkObject.WorkingDirectory;
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
        async Task ListLoadShortcuts(bool showStatus)
        {
            try
            {
                if (ConfigurationManager.AppSettings["ShowOtherShortcuts"] == "False")
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        sp_Shortcuts.Visibility = Visibility.Collapsed;
                    });
                    List_Shortcuts.Clear();
                    GC.Collect();
                    return;
                }

                //Show refresh status message
                if (showStatus)
                {
                    Popup_Show_Status("Refresh", "Refreshing shortcuts");
                }

                //Get all files from the shortcut directories
                IEnumerable<FileInfo> directoryShortcuts = Enumerable.Empty<FileInfo>();
                foreach (string shortcutFolder in vShortcutLocations)
                {
                    try
                    {
                        string editedShortcutFolder = shortcutFolder;
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
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    await ListBoxRemoveAll(lb_Shortcuts, List_Shortcuts, x => !directoryShortcuts.Any(y => y.Name.Replace(".lnk", string.Empty).Replace(".url", string.Empty).Replace(".exe - Shortcut", string.Empty).Replace(" - Shortcut", string.Empty) == x.Name));
                });

                //Get shortcut information and add it to the list
                foreach (FileInfo file in directoryShortcuts)
                {
                    try
                    {
                        //Read shortcut file information
                        ShortcutDetails shortcutDetails = ReadShortcutFile(file.FullName);
                        string targetPathLower = shortcutDetails.TargetPath.ToLower();
                        string fileNameStripped = file.Name.Replace(".lnk", string.Empty).Replace(".url", string.Empty).Replace(".exe - Shortcut", string.Empty).Replace(" - Shortcut", string.Empty);

                        //Check if already in combined list and remove it
                        if (CombineAppLists(false, false).Any(x => x.PathExe.ToLower() == targetPathLower))
                        {
                            //Debug.WriteLine("Shortcut is already in other list: " + targetPathLower);
                            await AVActions.ActionDispatcherInvokeAsync(async delegate
                            {
                                await ListBoxRemoveAll(lb_Shortcuts, List_Shortcuts, x => x.PathExe.ToLower() == targetPathLower);
                            });
                            continue;
                        }

                        //Check if shortcut is already in the shortcut list
                        if (List_Shortcuts.Any(x => x.PathExe.ToLower() == targetPathLower))
                        {
                            //Debug.WriteLine("Shortcut is already in list skipping: " + targetPathLower);
                            continue;
                        }

                        //Check if shortcut is in shortcut blacklist
                        if (vAppsBlacklistShortcut.Any(x => x.ToLower() == fileNameStripped.ToLower()))
                        {
                            //Debug.WriteLine("Shortcut is on the blacklist skipping: " + fileNameStripped.ToLower());
                            continue;
                        }

                        //Check if shortcut uri is in shortcut uri blacklist
                        if (vAppsBlacklistShortcutUri.Any(x => targetPathLower.Contains(x.ToLower())))
                        {
                            //Debug.WriteLine("Shortcut uri is on the uri blacklist skipping: " + targetPathLower);
                            continue;
                        }

                        //Add shortcut to the shortcuts list
                        ShortcutAddToListWithDetails(shortcutDetails);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed loading shortcuts: " + ex.Message);
            }
        }

        //Add shortcut to the shortcuts list
        void ShortcutAddToListWithDetails(ShortcutDetails shortcutDetails)
        {
            try
            {
                //Check if the shortcut name is set
                if (string.IsNullOrWhiteSpace(shortcutDetails.Name))
                {
                    Debug.WriteLine("Shortcut name is not set, skipping shortcut.");
                    return;
                }

                //Get the shortcut target file name
                string targetPathLower = shortcutDetails.TargetPath.ToLower();
                Visibility shortcutLauncher = Visibility.Collapsed;
                Visibility shortcutWindowStore = Visibility.Collapsed;
                ProcessType shortcutProcessType = ProcessType.Win32;

                //Check if executable or launcher app shortcut
                if (targetPathLower.EndsWith(".exe"))
                {
                    //Check if the executable still exists
                    if (!File.Exists(targetPathLower)) { return; }
                }
                else if (targetPathLower.EndsWith(".bat"))
                {
                    //Check if the bat file still exists
                    if (!File.Exists(targetPathLower)) { return; }
                }
                else if (targetPathLower.Contains("://"))
                {
                    //Check if shortcut is url protocol
                    shortcutLauncher = Visibility.Visible;
                }
                else if (!targetPathLower.Contains("/") && targetPathLower.Contains("!") && targetPathLower.Contains("_"))
                {
                    //Check if shortcut is windows store app
                    shortcutProcessType = ProcessType.UWP;
                    shortcutWindowStore = Visibility.Visible;

                    //Get detailed application information
                    Package appPackage = UwpGetAppPackageFromAppUserModelId(shortcutDetails.TargetPath);
                    AppxDetails appxDetails = UwpGetAppxDetailsFromAppPackage(appPackage);

                    //Set the application icon path
                    shortcutDetails.IconPath = appxDetails.SquareLargestLogoPath;

                    //Set the applicaton exe name
                    shortcutDetails.NameExe = appxDetails.ExecutableName;
                }
                else
                {
                    //Debug.WriteLine("Unknown shortcut: " + TargetPathLower);
                    return;
                }

                //Get icon image from the path
                BitmapImage IconBitmapImage = null;
                if (targetPathLower.EndsWith(".bat"))
                {
                    IconBitmapImage = FileToBitmapImage(new string[] { shortcutDetails.IconPath, "pack://application:,,,/Assets/Icons/FileBat.png" }, IntPtr.Zero, 90);
                }
                else
                {
                    IconBitmapImage = FileToBitmapImage(new string[] { targetPathLower, shortcutDetails.IconPath }, IntPtr.Zero, 90);
                }

                //Add the shortcut to the list
                AVActions.ActionDispatcherInvoke(delegate
                {
                    List_Shortcuts.Add(new DataBindApp() { Type = shortcutProcessType, Category = AppCategory.Shortcut, Name = shortcutDetails.Name, NameExe = shortcutDetails.NameExe, ImageBitmap = IconBitmapImage, ImagePath = shortcutDetails.IconPath, PathExe = shortcutDetails.TargetPath, PathLaunch = shortcutDetails.WorkingPath, ShortcutPath = shortcutDetails.ShortcutPath, Argument = shortcutDetails.Argument, StatusStore = shortcutWindowStore, StatusLauncher = shortcutLauncher, TimeCreation = shortcutDetails.TimeModify });
                });
            }
            catch
            {
                Debug.WriteLine("Failed to add shortcut to list: " + shortcutDetails.ShortcutPath);
            }
        }
    }
}