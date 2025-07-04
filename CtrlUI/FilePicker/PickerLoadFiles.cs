using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVSettings;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Get and list all files and folders
        async Task FilePicker_LoadFilesFolders(string targetPath)
        {
            try
            {
                //Clean the target path string
                targetPath = Path.GetFullPath(targetPath);

                //Add the Go up directory to the list
                if (vFilePickerSettings.RootPath != targetPath)
                {
                    if (Path.GetPathRoot(targetPath) != targetPath)
                    {
                        BitmapImage imageBack = FileToBitmapImage(new string[] { "Assets/Default/Icons/Up.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        DataBindFile dataBindFileGoUp = new DataBindFile() { FileType = FileType.GoUpPre, Name = "Go up", Description = "Go up to the previous folder.", ImageBitmap = imageBack, PathFile = Path.GetDirectoryName(targetPath) };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileGoUp, false, false);
                    }
                    else
                    {
                        BitmapImage imageBack = FileToBitmapImage(new string[] { "Assets/Default/Icons/Up.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        DataBindFile dataBindFileGoUp = new DataBindFile() { FileType = FileType.GoUpPre, Name = "Go up", Description = "Go up to the previous folder.", ImageBitmap = imageBack, PathFile = "PC" };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileGoUp, false, false);
                    }
                }

                AVActions.DispatcherInvoke(delegate
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
                if (vFilePickerSettings.ShowEmulatorInterface)
                {
                    string fileDescription = "Launch without a rom loaded";
                    DataBindFile dataBindFileWithoutRom = new DataBindFile() { FileType = FileType.FilePre, Name = fileDescription, Description = fileDescription + ".", ImageBitmap = vImagePreloadEmulator, PathFile = string.Empty };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileWithoutRom, false, false);

                    string romDescription = "Launch with this folder as rom";
                    DataBindFile dataBindFileFolderRom = new DataBindFile() { FileType = FileType.FilePre, Name = romDescription, Description = romDescription + ".", ImageBitmap = vImagePreloadEmulator, PathFile = targetPath };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFolderRom, false, false);
                }

                //Enable or disable the side navigate buttons
                AVActions.DispatcherInvoke(delegate
                {
                    grid_Popup_FilePicker_button_ControllerLeft.Visibility = Visibility.Visible;
                    grid_Popup_FilePicker_button_ControllerUp.Visibility = Visibility.Visible;
                    grid_Popup_FilePicker_button_ControllerBack.Visibility = Visibility.Visible;
                    grid_Popup_FilePicker_button_ControllerStart.Visibility = Visibility.Visible;
                });

                //Get all the top files and folders
                DirectoryInfo directoryInfo = new DirectoryInfo(targetPath);
                DirectoryInfo[] directoryFolders = directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly).OrderBy(x => x.Name).ToArray();
                FileInfo[] directoryFiles = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly).OrderBy(x => x.Name).ToArray();

                //Get all the directories from target directory
                if (vFilePickerSettings.ShowDirectories)
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
                                    Debug.WriteLine("File picker folders load cancelled.");
                                    return;
                                }

                                //Get the folder size
                                //string folderSize = AVFunctions.ConvertBytesSizeToString(GetDirectorySize(listDirectory));

                                //Get the folder date
                                string folderDate = listFolder.LastWriteTime.ToShortDateString().Replace("-", "/");

                                //Set the detailed text
                                string folderDetailed = string.Empty;
                                if (vFilePickerSettings.ShowEmulatorInterface)
                                {
                                    folderDetailed = vFilePickerSettings.SourceDataBindApp.Name;
                                }
                                else
                                {
                                    folderDetailed = folderDate;
                                }

                                //Check the copy cut type
                                ClipboardType clipboardType = ClipboardType.None;
                                DataBindFile clipboardFile = vClipboardFiles.FirstOrDefault(x => x.PathFile == listFolder.FullName);
                                if (clipboardFile != null)
                                {
                                    clipboardType = clipboardFile.ClipboardType;
                                }

                                //Add folder to the list
                                bool systemFileFolder = listFolder.Attributes.HasFlag(FileAttributes.System);
                                bool hiddenFileFolder = listFolder.Attributes.HasFlag(FileAttributes.Hidden);
                                if (!systemFileFolder && (!hiddenFileFolder || SettingLoad(vConfigurationCtrlUI, "ShowHiddenFilesFolders", typeof(bool))))
                                {
                                    DataBindFile dataBindFileFolder = new DataBindFile() { FileType = FileType.Folder, ClipboardType = clipboardType, Name = listFolder.Name, NameDetail = folderDetailed, DateModified = listFolder.LastWriteTime, PathFile = listFolder.FullName, PathRoot = targetPath };
                                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFolder, false, false);
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                }

                //Get all the files from target directory
                if (vFilePickerSettings.ShowFiles)
                {
                    try
                    {
                        //File Picker change select mode
                        FilePicker_ChangeSelectMode(false);

                        //Filter files in and out
                        if (vFilePickerSettings.FilterIn.Any())
                        {
                            directoryFiles = directoryFiles.Where(file => vFilePickerSettings.FilterIn.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                        }
                        if (vFilePickerSettings.FilterOut.Any())
                        {
                            directoryFiles = directoryFiles.Where(file => !vFilePickerSettings.FilterOut.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                        }

                        //Fill the file picker listbox with files
                        foreach (FileInfo listFile in directoryFiles)
                        {
                            try
                            {
                                //Cancel loading
                                if (vFilePickerLoadCancel)
                                {
                                    Debug.WriteLine("File picker files load cancelled.");
                                    return;
                                }

                                //Get the file size
                                string fileSize = AVFunctions.ConvertBytesSizeToString(listFile.Length);

                                //Get the file date
                                string fileDate = listFile.LastWriteTime.ToShortDateString().Replace("-", "/");

                                //Set the detailed text
                                string fileDetailed = string.Empty;
                                if (vFilePickerSettings.ShowEmulatorInterface)
                                {
                                    fileDetailed = vFilePickerSettings.SourceDataBindApp.Name;
                                }
                                else
                                {
                                    fileDetailed = fileSize + " (" + fileDate + ")";
                                }

                                //Check if file is a shortcut
                                bool fileIsShortcut = false;
                                if (listFile.Extension == ".url" || listFile.Extension == ".lnk" || listFile.Extension == ".pif")
                                {
                                    fileIsShortcut = true;
                                }

                                //Check the copy cut type
                                ClipboardType clipboardType = ClipboardType.None;
                                DataBindFile clipboardFile = vClipboardFiles.FirstOrDefault(x => x.PathFile == listFile.FullName);
                                if (clipboardFile != null)
                                {
                                    clipboardType = clipboardFile.ClipboardType;
                                }

                                //Add file to the list
                                bool systemFileFolder = listFile.Attributes.HasFlag(FileAttributes.System);
                                bool hiddenFileFolder = listFile.Attributes.HasFlag(FileAttributes.Hidden);
                                if (!systemFileFolder && (!hiddenFileFolder || SettingLoad(vConfigurationCtrlUI, "ShowHiddenFilesFolders", typeof(bool))))
                                {
                                    DataBindFile dataBindFileFile = new DataBindFile() { FileType = FileType.File, ClipboardType = clipboardType, IsShortcut = fileIsShortcut, Name = listFile.Name, NameDetail = fileDetailed, DateModified = listFile.LastWriteTime, PathFile = listFile.FullName, PathRoot = targetPath };
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
                    //File Picker change select mode
                    FilePicker_ChangeSelectMode(true);
                }

                //Check if there are files or folders
                FilePicker_CheckFilesAndFoldersCount();
            }
            catch { }
        }
    }
}