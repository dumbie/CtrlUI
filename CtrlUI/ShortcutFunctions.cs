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
        async Task ListLoadShortcuts(bool ShowStatus)
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

                //Get all files from the public/user desktop and shortcut folder
                string ShortcutsDirectory = Convert.ToString(ConfigurationManager.AppSettings["DirectoryShortcuts"]);

                DirectoryInfo DirectoryInfoUser = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                IEnumerable<FileInfo> DirectoryFiles = DirectoryInfoUser.GetFiles();

                //Merge the list with public desktop shortcuts
                try
                {
                    DirectoryInfo DirectoryInfoPublic = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
                    DirectoryFiles = DirectoryFiles.Concat(DirectoryInfoPublic.GetFiles());
                }
                catch { }

                //Merge the list with custom folder shortcuts
                try
                {
                    DirectoryInfo DirectoryInfoCustom = new DirectoryInfo(ShortcutsDirectory);
                    DirectoryFiles = DirectoryFiles.Concat(DirectoryInfoCustom.GetFiles());
                }
                catch { }

                //Sort and filter the list by shortcut name
                DirectoryFiles = DirectoryFiles.Where(x => x.Name.ToLower().EndsWith(".url") || x.Name.ToLower().EndsWith(".lnk")).OrderBy(x => x.Name);

                //Show refresh status message
                if (ShowStatus)
                {
                    Popup_Show_Status("Refresh", "Refreshing shortcuts");
                }

                //Remove shortcuts that are no longer available from the list
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    await ListBoxRemoveAll(lb_Shortcuts, List_Shortcuts, x => !DirectoryFiles.Any(y => y.Name.Replace(".lnk", string.Empty).Replace(".url", string.Empty).Replace(".exe - Shortcut", string.Empty).Replace(" - Shortcut", string.Empty) == x.Name));
                });

                //Get shortcut information and add it to the list
                foreach (FileInfo file in DirectoryFiles)
                {
                    try
                    {
                        string fileNameStripped = file.Name.Replace(".lnk", string.Empty).Replace(".url", string.Empty).Replace(".exe - Shortcut", string.Empty).Replace(" - Shortcut", string.Empty);

                        //Check if shortcut is in shortcut blacklist
                        if (vAppsBlacklistShortcut.Any(x => x.ToLower() == fileNameStripped.ToLower()))
                        {
                            //Debug.WriteLine("Shortcut is on the blacklist skipping: " + fileNameStripped);
                            continue;
                        }

                        //Check if shortcut is already in the list
                        if (List_Shortcuts.Any(x => x.ShortcutPath.ToLower() == file.FullName.ToLower()))
                        {
                            //Debug.WriteLine("Shortcut is already in list skipping: " + fileNameStripped);
                            continue;
                        }

                        //Read shortcut file information
                        ShortcutDetails shortcutDetails = ReadShortcutFile(file.FullName);
                        string TargetPathLower = shortcutDetails.TargetPath.ToLower();

                        //Check if shortcut uri is in blacklist
                        if (vAppsBlacklistShortcutUri.Any(x => TargetPathLower.Contains(x.ToLower())))
                        {
                            //Debug.WriteLine("Shortcut is on the uri blacklist skipping: " + TargetPathLower);
                            continue;
                        }

                        //Check if shortcut is already in the list
                        if (List_Shortcuts.Any(x => x.PathExe.ToLower() == TargetPathLower))
                        {
                            //Debug.WriteLine("Shortcut is already in list skipping: " + TargetPathLower);
                            continue;
                        }

                        //Add the shortcut to list
                        await ShortcutAddToListWithDetails(shortcutDetails);
                    }
                    catch { }
                }
            }
            catch { }
        }

        async Task ShortcutAddToListWithDetails(ShortcutDetails shortcutDetails)
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
                string TargetPathLower = shortcutDetails.TargetPath.ToLower();
                Visibility ShortcutLauncher = Visibility.Collapsed;
                Visibility ShortcutWindowStore = Visibility.Collapsed;
                string ShortcutType = "Win32";

                //Check if already in combined list and remove it
                if (CombineAppLists(false, false).Any(x => x.PathExe.ToLower() == TargetPathLower))
                {
                    //Debug.WriteLine("Shortcut is already in other list: " + TargetPathLower);
                    await AVActions.ActionDispatcherInvokeAsync(async delegate
                    {
                        await ListBoxRemoveAll(lb_Shortcuts, List_Shortcuts, x => x.PathExe.ToLower() == TargetPathLower);
                    });
                    return;
                }

                //Check if executable or launcher app shortcut
                if (TargetPathLower.EndsWith(".exe"))
                {
                    //Check if the executable still exists
                    if (!File.Exists(TargetPathLower)) { return; }
                }
                else if (TargetPathLower.EndsWith(".bat"))
                {
                    //Check if the bat file still exists
                    if (!File.Exists(TargetPathLower)) { return; }
                }
                else if (TargetPathLower.Contains("://"))
                {
                    //Check if shortcut is url protocol
                    ShortcutLauncher = Visibility.Visible;
                }
                else if (!TargetPathLower.Contains("/") && TargetPathLower.Contains("!") && TargetPathLower.Contains("_"))
                {
                    //Check if shortcut is windows store app
                    ShortcutType = "UWP";
                    ShortcutWindowStore = Visibility.Visible;
                    shortcutDetails.IconPath = GetUwpAppImagePath(shortcutDetails.TargetPath);
                }
                else
                {
                    //Debug.WriteLine("Unknown shortcut: " + TargetPathLower);
                    return;
                }

                //Get icon image from the path
                BitmapImage IconBitmapImage = null;
                if (TargetPathLower.EndsWith(".bat"))
                {
                    IconBitmapImage = FileToBitmapImage(new string[] { shortcutDetails.IconPath, "pack://application:,,,/Assets/Icons/FileBat.png" }, IntPtr.Zero, 90);
                }
                else
                {
                    IconBitmapImage = FileToBitmapImage(new string[] { TargetPathLower, shortcutDetails.IconPath }, IntPtr.Zero, 90);
                }

                //Add the shortcut to the list
                AVActions.ActionDispatcherInvoke(delegate
                {
                    List_Shortcuts.Add(new DataBindApp() { Category = "Shortcut", Type = ShortcutType, Name = shortcutDetails.Name, ImageBitmap = IconBitmapImage, ImagePath = shortcutDetails.IconPath, PathExe = shortcutDetails.TargetPath, PathLaunch = shortcutDetails.WorkingPath, ShortcutPath = shortcutDetails.ShortcutPath, Argument = shortcutDetails.Argument, StatusStore = ShortcutWindowStore, StatusLauncher = ShortcutLauncher, TimeCreation = shortcutDetails.TimeModify });
                });
            }
            catch
            {
                Debug.WriteLine("Failed to add shortcut to list: " + shortcutDetails.ShortcutPath);
            }
        }
    }
}