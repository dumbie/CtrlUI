using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVDiskInfo;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVSettings;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Get and list all the disk drives
        async Task FilePicker_LoadPC()
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    //File Picker change select mode
                    FilePicker_ChangeSelectMode(false);

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
                bool hideNetworkDrives = SettingLoad(vConfigurationCtrlUI, "HideNetworkDrives", typeof(bool));

                //Add all disk drives
                foreach (string diskDrive in Directory.GetLogicalDrives())
                {
                    try
                    {
                        //Get disk information
                        DiskInfo diskInfo = await AVDiskInfo.GetDiskInfo(diskDrive);

                        //Check network drive setting
                        if (diskInfo.Type == DriveTypes.Network && hideNetworkDrives)
                        {
                            continue;
                        }

                        //Create databind file
                        DataBindFile dataBindFile = new DataBindFile();

                        //Check and set file type and image
                        dataBindFile.FileType = FileType.FolderPre;
                        if (diskInfo.Type == DriveTypes.CDRom)
                        {
                            dataBindFile.FileType = FileType.FolderDisc;
                            dataBindFile.ImageBitmap = imageFolderDisc;
                        }
                        else if (diskInfo.Type == DriveTypes.Network)
                        {
                            dataBindFile.ImageBitmap = imageFolderNetwork;
                        }
                        else
                        {
                            dataBindFile.ImageBitmap = imageFolder;
                        }

                        //Set file information
                        dataBindFile.Name = diskInfo.Path;
                        dataBindFile.NameSub = diskInfo.Label;
                        dataBindFile.NameDetail = diskInfo.SizeString;
                        dataBindFile.PathFile = diskInfo.Path;

                        //Add databindfile to the list
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFile, false, false);
                    }
                    catch { }
                }

                //Add user file locations
                foreach (ProfileShared fileLocation in vCtrlLocationsFile)
                {
                    try
                    {
                        //Get disk information
                        DiskInfo diskInfo = await AVDiskInfo.GetDiskInfo(fileLocation.String2);

                        //Create databind file
                        DataBindFile dataBindFile = new DataBindFile();

                        //Check and set file type and image
                        dataBindFile.FileType = FileType.FolderPre;
                        if (diskInfo.Type == DriveTypes.CDRom)
                        {
                            dataBindFile.FileType = FileType.FolderDisc;
                            dataBindFile.ImageBitmap = imageFolderDisc;
                        }
                        else if (diskInfo.Type == DriveTypes.Network)
                        {
                            dataBindFile.ImageBitmap = imageFolderNetwork;
                        }
                        else
                        {
                            dataBindFile.ImageBitmap = imageFolder;
                        }

                        //Set file information
                        dataBindFile.Name = diskInfo.Path;
                        dataBindFile.NameSub = diskInfo.Label + " (" + fileLocation.String1 + ")";
                        dataBindFile.NameDetail = diskInfo.SizeString;
                        dataBindFile.PathFile = diskInfo.Path;

                        //Add databindfile to the list
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFile, false, false);
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}