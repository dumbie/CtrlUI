using ArnoldVinkCode;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkCode.AVDiskInfo;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVShellInfo;
using static ArnoldVinkStyles.AVDispatcherInvoke;
using static ArnoldVinkStyles.AVImage;
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
                DispatcherInvoke(delegate
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

                //Add the previous used path
                if (!string.IsNullOrWhiteSpace(vFilePickerPreviousPath))
                {
                    BitmapImage imageFolderPrevious = FileToBitmapImage(new string[] { "Assets/Default/Icons/Restart.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    DataBindFile dataBindFilePreviousPath = new DataBindFile() { FileType = FileType.FolderPre, Name = "Previous", NameSub = "(" + vFilePickerPreviousPath + ")", ImageBitmap = imageFolderPrevious, PathFile = vFilePickerPreviousPath };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFilePreviousPath, false, false);
                }

                //Add launch without a file option
                if (vFilePickerSettings.ShowLaunchWithoutFile)
                {
                    string fileDescription = "Launch application without a file";
                    BitmapImage fileImage = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppLaunch.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    DataBindFile dataBindFileWithoutFile = new DataBindFile() { FileType = FileType.FilePre, Name = fileDescription, Description = fileDescription + ".", ImageBitmap = fileImage, PathFile = string.Empty };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileWithoutFile, false, false);
                }

                //Add emulator images folder
                if (vFilePickerSettings.ShowEmulatorImages)
                {
                    DataBindFile dataBindEmuImages = new DataBindFile() { FileType = FileType.FolderPre, Name = "Emulator images", ImageBitmap = vImagePreloadEmulator, PathFile = "Assets\\Default\\Emulators" };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindEmuImages, false, false);
                }

                //Add desktop folder
                BitmapImage imageFolderDesktop = FileCacheToBitmapImage(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), vImageBackupSource, 50, 0, true);
                DataBindFile dataBindFileDesktop = new DataBindFile() { FileType = FileType.FolderPre, Name = "My Desktop", ImageBitmap = imageFolderDesktop, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) };
                await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileDesktop, false, false);

                //Add documents folder
                BitmapImage imageFolderDocuments = FileCacheToBitmapImage(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), vImageBackupSource, 50, 0, true);
                DataBindFile dataBindFileDocuments = new DataBindFile() { FileType = FileType.FolderPre, Name = "My Documents", ImageBitmap = imageFolderDocuments, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) };
                await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileDocuments, false, false);

                //Add downloads folder
                string downloadsPath = AVShellInfo.ShellPath_KnownFolder(KnownFolder.Downloads);
                if (!string.IsNullOrWhiteSpace(downloadsPath) && Directory.Exists(downloadsPath))
                {
                    BitmapImage imageFolderDownload = FileCacheToBitmapImage(downloadsPath, vImageBackupSource, 50, 0, true);
                    DataBindFile dataBindFileDownloads = new DataBindFile() { FileType = FileType.FolderPre, Name = "My Downloads", ImageBitmap = imageFolderDownload, PathFile = downloadsPath };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileDownloads, false, false);
                }

                //Add music folder
                BitmapImage imageFolderMusic = FileCacheToBitmapImage(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), vImageBackupSource, 50, 0, true);
                DataBindFile dataBindFileMusic = new DataBindFile() { FileType = FileType.FolderPre, Name = "My Music", ImageBitmap = imageFolderMusic, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) };
                await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileMusic, false, false);

                //Add pictures folder
                BitmapImage imageFolderPictures = FileCacheToBitmapImage(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), vImageBackupSource, 50, 0, true);
                DataBindFile dataBindFilePictures = new DataBindFile() { FileType = FileType.FolderPre, Name = "My Pictures", ImageBitmap = imageFolderPictures, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) };
                await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFilePictures, false, false);

                //Add videos folder
                BitmapImage imageFolderVideos = FileCacheToBitmapImage(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), vImageBackupSource, 50, 0, true);
                DataBindFile dataBindFileVideos = new DataBindFile() { FileType = FileType.FolderPre, Name = "My Videos", ImageBitmap = imageFolderVideos, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) };
                await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileVideos, false, false);

                //Add onedrive folder
                string onedrivePath = AVShellInfo.ShellPath_KnownFolder(KnownFolder.OneDrive);
                if (!string.IsNullOrWhiteSpace(onedrivePath) && Directory.Exists(onedrivePath))
                {
                    BitmapImage imageFolderOnedrive = FileCacheToBitmapImage(onedrivePath, vImageBackupSource, 50, 0, true);
                    DataBindFile dataBindFileOnedrive = new DataBindFile() { FileType = FileType.FolderPre, Name = "OneDrive", ImageBitmap = imageFolderOnedrive, PathFile = onedrivePath };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileOnedrive, false, false);
                }

                //Load file browser settings
                bool hideNetworkDrives = SettingLoad(vConfigurationCtrlUI, "HideNetworkDrives", typeof(bool));

                //Add disk drives
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
                        }

                        //Set file information
                        dataBindFile.Name = diskInfo.Path;
                        dataBindFile.NameSub = diskInfo.Label;
                        dataBindFile.NameDetail = diskInfo.SizeString;
                        dataBindFile.PathFile = diskInfo.Path;
                        dataBindFile.ImageBitmap = FileCacheToBitmapImage(dataBindFile.PathFile, vImageBackupSource, 50, 0, true);

                        //Add databindfile to the list
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFile, false, false);
                    }
                    catch { }
                }

                //Add user locations
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
                        }

                        //Set file information
                        dataBindFile.Name = diskInfo.Path;
                        dataBindFile.NameSub = diskInfo.Label + " (" + fileLocation.String1 + ")";
                        dataBindFile.NameDetail = diskInfo.SizeString;
                        dataBindFile.PathFile = diskInfo.Path;
                        dataBindFile.ImageBitmap = FileCacheToBitmapImage(dataBindFile.PathFile, vImageBackupSource, 50, 0, true);

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