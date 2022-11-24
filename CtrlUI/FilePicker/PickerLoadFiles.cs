using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.Settings;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Get and list all files and folders
        async Task PickerLoadFilesFolders(string targetPath)
        {
            try
            {
                //Clean the target path string
                targetPath = Path.GetFullPath(targetPath);

                //Add the Go up directory to the list
                if (vFilePickerBlockGoUpPath != targetPath)
                {
                    if (Path.GetPathRoot(targetPath) != targetPath)
                    {
                        BitmapImage imageBack = FileToBitmapImage(new string[] { "Assets/Default/Icons/Up.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        DataBindFile dataBindFileGoUp = new DataBindFile() { FileType = FileType.GoUpPre, Name = "Go up", Description = "Go up to the previous folder.", ImageBitmap = imageBack, PathFile = Path.GetDirectoryName(targetPath) };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileGoUp, false, false);
                    }
                    else
                    {
                        BitmapImage imageBack = FileToBitmapImage(new string[] { "Assets/Default/Icons/Up.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        DataBindFile dataBindFileGoUp = new DataBindFile() { FileType = FileType.GoUpPre, Name = "Go up", Description = "Go up to the previous folder.", ImageBitmap = imageBack, PathFile = "PC" };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileGoUp, false, false);
                    }
                }

                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Enable or disable the copy paste status
                    if (vClipboardFiles.Any())
                    {
                        grid_Popup_FilePicker_textblock_ClipboardStatus.Visibility = Visibility.Visible;
                    }

                    //Enable or disable the current path
                    grid_Popup_FilePicker_textblock_CurrentPath.Text = "Current path: " + targetPath;
                    grid_Popup_FilePicker_textblock_CurrentPath.Visibility = Visibility.Visible;
                });

                //Add launch emulator options
                if (vFilePickerShowRoms)
                {
                    //Fix Load platform description
                    BitmapImage platformImage = vImagePreloadHelp;
                    string platformDescription = "Download " + vFilePickerSourceDataBindApp.Name + " platform information.";
                    DownloadInfoPlatform informationDownloaded = await DownloadInfoPlatform(vFilePickerSourceDataBindApp.Name, 0, false, true);
                    if (informationDownloaded != null)
                    {
                        platformDescription = informationDownloaded.Summary;
                        if (informationDownloaded.ImageBitmap != null)
                        {
                            platformImage = informationDownloaded.ImageBitmap;
                        }
                    }

                    string platformName = "Emulator platform information";
                    DataBindFile dataBindFileplatform = new DataBindFile() { FileType = FileType.PlatformDesc, Name = platformName, Description = platformDescription, ImageBitmap = platformImage, PathFile = vFilePickerSourceDataBindApp.Name };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileplatform, false, false);

                    string fileDescription = "Launch the emulator without a rom loaded";
                    DataBindFile dataBindFileWithoutRom = new DataBindFile() { FileType = FileType.FilePre, Name = fileDescription, Description = fileDescription + ".", ImageBitmap = vImagePreloadEmulator, PathFile = string.Empty };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileWithoutRom, false, false);

                    string romDescription = "Launch the emulator with this folder as rom";
                    DataBindFile dataBindFileFolderRom = new DataBindFile() { FileType = FileType.FilePre, Name = romDescription, Description = romDescription + ".", ImageBitmap = vImagePreloadEmulator, PathFile = targetPath };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFolderRom, false, false);
                }

                //Enable or disable the side navigate buttons
                AVActions.ActionDispatcherInvoke(delegate
                {
                    grid_Popup_FilePicker_button_ControllerLeft.Visibility = Visibility.Visible;
                    grid_Popup_FilePicker_button_ControllerUp.Visibility = Visibility.Visible;
                    grid_Popup_FilePicker_button_ControllerBack.Visibility = Visibility.Visible;
                    grid_Popup_FilePicker_button_ControllerStart.Visibility = Visibility.Visible;
                });

                //Get all the top files and folders
                DirectoryInfo directoryInfo = new DirectoryInfo(targetPath);
                DirectoryInfo[] directoryFolders = null;
                FileInfo[] directoryFiles = null;
                if (vFilePickerSortType == SortingType.Name)
                {
                    directoryFolders = directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly).OrderBy(x => x.Name).ToArray();
                    directoryFiles = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly).OrderBy(x => x.Name).ToArray();
                }
                else
                {
                    directoryFolders = directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly).OrderByDescending(x => x.LastWriteTime).ToArray();
                    directoryFiles = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly).OrderByDescending(x => x.LastWriteTime).ToArray();
                }

                //Get all rom images and descriptions
                FileInfo[] directoryRomImages = new FileInfo[] { };
                FileInfo[] directoryRomDescriptions = new FileInfo[] { };
                if (vFilePickerShowRoms)
                {
                    string[] imageFilter = { "jpg", "png" };
                    string[] descriptionFilter = { "json" };

                    DirectoryInfo directoryInfoRomsUser = new DirectoryInfo("Assets/User/Emulators");
                    FileInfo[] directoryPathsRomsUser = directoryInfoRomsUser.GetFiles("*", SearchOption.AllDirectories);
                    DirectoryInfo directoryInfoRomsDefault = new DirectoryInfo("Assets/Default/Emulators");
                    FileInfo[] directoryPathsRomsDefault = directoryInfoRomsDefault.GetFiles("*", SearchOption.AllDirectories);
                    IEnumerable<FileInfo> directoryPathsRoms = directoryPathsRomsUser.Concat(directoryPathsRomsDefault);

                    FileInfo[] romsImages = directoryPathsRoms.Where(file => imageFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                    FileInfo[] filesImages = directoryFiles.Where(file => imageFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                    directoryRomImages = filesImages.Concat(romsImages).OrderByDescending(x => x.Name.Length).ToArray();

                    FileInfo[] romsDescriptions = directoryPathsRoms.Where(file => descriptionFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                    FileInfo[] filesDescriptions = directoryFiles.Where(file => descriptionFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                    directoryRomDescriptions = filesDescriptions.Concat(romsDescriptions).OrderByDescending(x => x.Name.Length).ToArray();
                }

                //Get all the directories from target directory
                if (vFilePickerShowDirectories)
                {
                    try
                    {
                        //Fill the file picker listbox with folders
                        foreach (DirectoryInfo listFolder in directoryFolders)
                        {
                            try
                            {
                                //Cancel loading
                                if (vFilePickerLoadCancel)
                                {
                                    Debug.WriteLine("File picker folder load cancelled.");
                                    vFilePickerLoadCancel = false;
                                    vFilePickerLoadBusy = false;
                                    return;
                                }

                                BitmapImage listImage = null;
                                string listDescription = string.Empty;

                                //Load image files for the list
                                if (vFilePickerShowRoms)
                                {
                                    GetRomDetails(listFolder.Name, listFolder.FullName, directoryRomImages, directoryRomDescriptions, ref listImage, ref listDescription);
                                }
                                else
                                {
                                    listImage = FileToBitmapImage(new string[] { "Assets/Default/Icons/Folder.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                                }

                                //Get the folder size
                                //string folderSize = AVFunctions.ConvertBytesSizeToString(GetDirectorySize(listDirectory));

                                //Get the folder date
                                string folderDate = listFolder.LastWriteTime.ToShortDateString().Replace("-", "/");

                                //Set the detailed text
                                string folderDetailed = folderDate;

                                //Check the copy cut type
                                ClipboardType clipboardType = ClipboardType.None;
                                DataBindFile clipboardFile = vClipboardFiles.Where(x => x.PathFile == listFolder.FullName).FirstOrDefault();
                                if (clipboardFile != null)
                                {
                                    clipboardType = clipboardFile.ClipboardType;
                                }

                                //Add folder to the list
                                bool systemFileFolder = listFolder.Attributes.HasFlag(FileAttributes.System);
                                bool hiddenFileFolder = listFolder.Attributes.HasFlag(FileAttributes.Hidden);
                                if (!systemFileFolder && (!hiddenFileFolder || Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowHiddenFilesFolders"))))
                                {
                                    DataBindFile dataBindFileFolder = new DataBindFile() { FileType = FileType.Folder, ClipboardType = clipboardType, Name = listFolder.Name, NameDetail = folderDetailed, Description = listDescription, DateModified = listFolder.LastWriteTime, ImageBitmap = listImage, PathFile = listFolder.FullName };
                                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFolder, false, false);
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                }

                //Get all the files from target directory
                if (vFilePickerShowFiles)
                {
                    try
                    {
                        //Enable or disable selection button in the list
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Collapsed;
                        });

                        //Filter files in and out
                        if (vFilePickerFilterIn.Any())
                        {
                            directoryFiles = directoryFiles.Where(file => vFilePickerFilterIn.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                        }
                        if (vFilePickerFilterOut.Any())
                        {
                            directoryFiles = directoryFiles.Where(file => !vFilePickerFilterOut.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                        }

                        //Fill the file picker listbox with files
                        foreach (FileInfo listFile in directoryFiles)
                        {
                            try
                            {
                                //Cancel loading
                                if (vFilePickerLoadCancel)
                                {
                                    Debug.WriteLine("File picker file load cancelled.");
                                    vFilePickerLoadCancel = false;
                                    vFilePickerLoadBusy = false;
                                    return;
                                }

                                BitmapImage listImage = null;
                                string listDescription = string.Empty;

                                //Load image files for the list
                                if (vFilePickerShowRoms)
                                {
                                    GetRomDetails(listFile.Name, string.Empty, directoryRomImages, directoryRomDescriptions, ref listImage, ref listDescription);
                                }
                                else
                                {
                                    string listFileFullNameLower = listFile.FullName.ToLower();
                                    string listFileExtensionLower = listFile.Extension.ToLower().Replace(".", string.Empty);
                                    if (listFileFullNameLower.EndsWith(".jpg") || listFileFullNameLower.EndsWith(".png") || listFileFullNameLower.EndsWith(".gif"))
                                    {
                                        listImage = FileToBitmapImage(new string[] { listFile.FullName }, null, vImageBackupSource, IntPtr.Zero, 50, 0);
                                    }
                                    else
                                    {
                                        listImage = FileToBitmapImage(new string[] { "Assets/Default/Extensions/" + listFileExtensionLower + ".png", "Assets/Default/Icons/File.png" }, null, vImageBackupSource, IntPtr.Zero, 50, 0);
                                    }
                                }

                                //Get the file size
                                string fileSize = AVFunctions.ConvertBytesSizeToString(listFile.Length);

                                //Get the file date
                                string fileDate = listFile.LastWriteTime.ToShortDateString().Replace("-", "/");

                                //Set the detailed text
                                string fileDetailed = fileSize + " (" + fileDate + ")";

                                //Check the copy cut type
                                ClipboardType clipboardType = ClipboardType.None;
                                DataBindFile clipboardFile = vClipboardFiles.Where(x => x.PathFile == listFile.FullName).FirstOrDefault();
                                if (clipboardFile != null)
                                {
                                    clipboardType = clipboardFile.ClipboardType;
                                }

                                //Add file to the list
                                bool systemFileFolder = listFile.Attributes.HasFlag(FileAttributes.System);
                                bool hiddenFileFolder = listFile.Attributes.HasFlag(FileAttributes.Hidden);
                                if (!systemFileFolder && (!hiddenFileFolder || Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowHiddenFilesFolders"))))
                                {
                                    FileType fileType = FileType.File;
                                    string fileExtension = Path.GetExtension(listFile.Name);
                                    if (fileExtension == ".url" || fileExtension == ".lnk")
                                    {
                                        fileType = FileType.Link;
                                    }
                                    DataBindFile dataBindFileFile = new DataBindFile() { FileType = fileType, ClipboardType = clipboardType, Name = listFile.Name, NameDetail = fileDetailed, Description = listDescription, DateModified = listFile.LastWriteTime, ImageBitmap = listImage, PathFile = listFile.FullName };
                                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFile, false, false);
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
                else
                {
                    //Enable or disable selection button in the list
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Visible;
                    });
                }

                //Check if there are files or folders
                FilePicker_CheckFilesAndFoldersCount();
            }
            catch { }
        }
    }
}