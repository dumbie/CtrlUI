using ArnoldVinkCode;
using Microsoft.Win32;
using System;
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
        //Get and list all the disk drives
        async Task FilePicker_LoadPC()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Enable or disable selection button in the list
                    grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Collapsed;

                    //Enable or disable file and folder availability
                    grid_Popup_FilePicker_textblock_NoFilesAvailable.Visibility = Visibility.Collapsed;

                    //Enable or disable the side navigate buttons
                    grid_Popup_FilePicker_button_ControllerLeft.Visibility = Visibility.Visible;
                    grid_Popup_FilePicker_button_ControllerUp.Visibility = Visibility.Collapsed;
                    grid_Popup_FilePicker_button_ControllerBack.Visibility = Visibility.Collapsed;
                    grid_Popup_FilePicker_button_ControllerStart.Visibility = Visibility.Collapsed;

                    //Enable or disable the copy paste status
                    if (vClipboardFiles.Any())
                    {
                        grid_Popup_FilePicker_textblock_ClipboardStatus.Visibility = Visibility.Visible;
                    }

                    //Enable or disable the current path
                    grid_Popup_FilePicker_textblock_CurrentPath.Visibility = Visibility.Collapsed;
                });

                //Load folder images
                BitmapImage imageFolder = FileToBitmapImage(new string[] { "Assets/Default/Icons/Folder.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                BitmapImage imageFolderDisc = FileToBitmapImage(new string[] { "Assets/Default/Icons/FolderDisc.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                BitmapImage imageFolderNetwork = FileToBitmapImage(new string[] { "Assets/Default/Icons/FolderNetwork.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                BitmapImage imageFolderPrevious = FileToBitmapImage(new string[] { "Assets/Default/Icons/Restart.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                BitmapImage imageFolderDocuments = FileToBitmapImage(new string[] { "Assets/Default/Icons/Copy.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                BitmapImage imageFolderDesktop = FileToBitmapImage(new string[] { "Assets/Default/Icons/Desktop.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                BitmapImage imageFolderDownload = FileToBitmapImage(new string[] { "Assets/Default/Icons/Download.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                BitmapImage imageFolderPictures = FileToBitmapImage(new string[] { "Assets/Default/Icons/Background.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                BitmapImage imageFolderVideos = FileToBitmapImage(new string[] { "Assets/Default/Icons/BackgroundVideo.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                BitmapImage imageFolderMusic = FileToBitmapImage(new string[] { "Assets/Default/Icons/Music.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);

                //Add the previous used path
                if (!string.IsNullOrWhiteSpace(vFilePickerPreviousPath))
                {
                    DataBindFile dataBindFilePreviousPath = new DataBindFile() { FileType = FileType.FolderPre, Name = "Previous", NameSub = "(" + vFilePickerPreviousPath + ")", ImageBitmap = imageFolderPrevious, PathFile = vFilePickerPreviousPath };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFilePreviousPath, false, false);
                }

                //Add launch without a file option
                if (vFilePickerSettings.ShowLaunchWithoutFile)
                {
                    string fileDescription = "Launch application without a file";
                    BitmapImage fileImage = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppLaunch.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    DataBindFile dataBindFileWithoutFile = new DataBindFile() { FileType = FileType.FilePre, Name = fileDescription, Description = fileDescription + ".", ImageBitmap = fileImage, PathFile = string.Empty };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileWithoutFile, false, false);
                }

                //Add emulator images folder
                if (vFilePickerSettings.ShowEmulatorImages)
                {
                    DataBindFile dataBindEmuImages = new DataBindFile() { FileType = FileType.FolderPre, Name = "Emulator images", ImageBitmap = vImagePreloadEmulator, PathFile = "Assets\\Default\\Emulators" };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindEmuImages, false, false);
                }

                //Add special folders
                DataBindFile dataBindFileDesktop = new DataBindFile() { FileType = FileType.FolderPre, Name = "My Desktop", ImageBitmap = imageFolderDesktop, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) };
                await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileDesktop, false, false);
                DataBindFile dataBindFileDocuments = new DataBindFile() { FileType = FileType.FolderPre, Name = "My Documents", ImageBitmap = imageFolderDocuments, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) };
                await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileDocuments, false, false);

                try
                {
                    string downloadsPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", string.Empty).ToString();
                    if (!string.IsNullOrWhiteSpace(downloadsPath) && Directory.Exists(downloadsPath))
                    {
                        DataBindFile dataBindFileDownloads = new DataBindFile() { FileType = FileType.FolderPre, Name = "My Downloads", ImageBitmap = imageFolderDownload, PathFile = downloadsPath };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileDownloads, false, false);
                    }
                }
                catch { }

                DataBindFile dataBindFileMusic = new DataBindFile() { FileType = FileType.FolderPre, Name = "My Music", ImageBitmap = imageFolderMusic, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) };
                await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileMusic, false, false);
                DataBindFile dataBindFilePictures = new DataBindFile() { FileType = FileType.FolderPre, Name = "My Pictures", ImageBitmap = imageFolderPictures, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) };
                await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFilePictures, false, false);
                DataBindFile dataBindFileVideos = new DataBindFile() { FileType = FileType.FolderPre, Name = "My Videos", ImageBitmap = imageFolderVideos, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) };
                await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileVideos, false, false);

                //Load file browser settings
                bool hideNetworkDrives = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "HideNetworkDrives"));
                bool notReadyNetworkDrives = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "NotReadyNetworkDrives"));

                //Add all disk drives that are connected
                DriveInfo[] diskDrives = DriveInfo.GetDrives();
                foreach (DriveInfo disk in diskDrives)
                {
                    try
                    {
                        //Skip network drive depending on the setting
                        if (disk.DriveType == DriveType.Network && hideNetworkDrives)
                        {
                            continue;
                        }

                        //Check if the disk is currently connected
                        if (disk.IsReady || (disk.DriveType == DriveType.Network && notReadyNetworkDrives))
                        {
                            //Get the current disk size
                            string freeSpace = AVFunctions.ConvertBytesSizeToString(disk.TotalFreeSpace);
                            string usedSpace = AVFunctions.ConvertBytesSizeToString(disk.TotalSize);
                            string diskSpace = freeSpace + "/" + usedSpace;

                            DataBindFile dataBindFileDisk = new DataBindFile() { FileType = FileType.Folder, Name = disk.Name, NameSub = disk.VolumeLabel, NameDetail = diskSpace, PathFile = disk.Name };
                            if (disk.DriveType == DriveType.CDRom)
                            {
                                dataBindFileDisk.FileType = FileType.FolderDisc;
                                dataBindFileDisk.ImageBitmap = imageFolderDisc;
                            }
                            else if (disk.DriveType == DriveType.Network)
                            {
                                dataBindFileDisk.ImageBitmap = imageFolderNetwork;
                            }
                            else
                            {
                                dataBindFileDisk.ImageBitmap = imageFolder;
                            }
                            await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileDisk, false, false);
                        }
                    }
                    catch { }
                }

                //Add Json file locations
                foreach (ProfileShared Locations in vCtrlLocationsFile)
                {
                    try
                    {
                        if (Directory.Exists(Locations.String2))
                        {
                            //Check if the location is a root folder
                            FileType locationType = FileType.FolderPre;
                            DirectoryInfo locationInfo = new DirectoryInfo(Locations.String2);
                            if (locationInfo.Parent == null)
                            {
                                locationType = FileType.Folder;
                            }

                            //Get the current disk size
                            string diskSpace = string.Empty;
                            DriveType disktype = DriveType.Unknown;
                            try
                            {
                                DriveInfo driveInfo = new DriveInfo(Locations.String2);
                                disktype = driveInfo.DriveType;
                                string freeSpace = AVFunctions.ConvertBytesSizeToString(driveInfo.TotalFreeSpace);
                                string usedSpace = AVFunctions.ConvertBytesSizeToString(driveInfo.TotalSize);
                                diskSpace = freeSpace + "/" + usedSpace;
                            }
                            catch { }

                            DataBindFile dataBindFileLocation = new DataBindFile() { FileType = locationType, Name = Locations.String2, NameSub = Locations.String1, NameDetail = diskSpace, PathFile = Locations.String2 };
                            if (disktype == DriveType.CDRom)
                            {
                                dataBindFileLocation.FileType = FileType.FolderDisc;
                                dataBindFileLocation.ImageBitmap = imageFolderDisc;
                            }
                            else if (disktype == DriveType.Network)
                            {
                                dataBindFileLocation.ImageBitmap = imageFolderNetwork;
                            }
                            else
                            {
                                dataBindFileLocation.ImageBitmap = imageFolder;
                            }
                            await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileLocation, false, false);
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}