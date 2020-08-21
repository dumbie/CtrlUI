using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInteropDll;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryUsb.NativeMethods_DeviceManager;
using static LibraryUsb.NativeMethods_Hid;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show the file manager popup
        async Task ShowFileManager()
        {
            try
            {
                vFilePickerFilterIn = new List<string>();
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "File manager";
                vFilePickerDescription = "Please select a file to run or interact with:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("PC", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Check keyboard controller launch
                string fileExtension = Path.GetExtension(vFilePickerResult.PathFile).Replace(".", string.Empty);
                string fileNameNoExtension = Path.GetFileNameWithoutExtension(vFilePickerResult.PathFile);
                bool keyboardProcess = vCtrlKeyboardProcessName.Any(x => x.String1.ToLower() == fileNameNoExtension.ToLower());
                bool keyboardExtension = vCtrlKeyboardExtensionName.Any(x => x.String1.ToLower() == fileExtension.ToLower());
                bool keyboardLaunch = (keyboardExtension || keyboardProcess) && vControllerAnyConnected();

                //Launch the Win32 application
                await PrepareProcessLauncherWin32Async(fileNameNoExtension, vFilePickerResult.PathFile, "", "", false, true, false, false, keyboardLaunch, true);
            }
            catch { }
        }

        //Reset and clear the clipboard
        void Clipboard_ResetClear()
        {
            try
            {
                foreach (DataBindFile dataBindFile in vClipboardFiles)
                {
                    dataBindFile.ClipboardType = ClipboardType.None;
                }
                vClipboardFiles.Clear();
            }
            catch
            {
                Debug.WriteLine("Failed to reset and clear clipboard.");
            }
        }

        //Update the clipboard status text
        void Clipboard_UpdateStatusText()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    if (vClipboardFiles.Count == 1)
                    {
                        DataBindFile clipboardFile = vClipboardFiles.FirstOrDefault();
                        grid_Popup_FilePicker_textblock_ClipboardStatus.Text = "Clipboard (" + clipboardFile.FileType.ToString() + " " + clipboardFile.ClipboardType.ToString() + ") " + clipboardFile.PathFile;
                        grid_Popup_FilePicker_textblock_ClipboardStatus.Visibility = Visibility.Visible;
                    }
                    else if (vClipboardFiles.Count > 1)
                    {
                        int copyCount = vClipboardFiles.Count(x => x.ClipboardType == ClipboardType.Copy);
                        int cutCount = vClipboardFiles.Count(x => x.ClipboardType == ClipboardType.Cut);
                        string statusCount = string.Empty;
                        if (copyCount > cutCount)
                        {
                            statusCount = "(" + copyCount + "x copy)";
                        }
                        else
                        {
                            statusCount = "(" + cutCount + "x cut)";
                        }

                        grid_Popup_FilePicker_textblock_ClipboardStatus.Text = "Clipboard " + statusCount + " files or folders.";
                        grid_Popup_FilePicker_textblock_ClipboardStatus.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        grid_Popup_FilePicker_textblock_ClipboardStatus.Text = string.Empty;
                        grid_Popup_FilePicker_textblock_ClipboardStatus.Visibility = Visibility.Collapsed;
                    }
                });
            }
            catch { }
        }

        //Set copy file from the file picker
        async Task FilePicker_FileCopy_Single(DataBindFile dataBindFile)
        {
            try
            {
                //Check the file or folder
                if (dataBindFile.FileType == FileType.FolderPre || dataBindFile.FileType == FileType.FilePre || dataBindFile.FileType == FileType.GoUp)
                {
                    await Notification_Send_Status("Close", "Invalid file or folder");
                    Debug.WriteLine("Invalid file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                    return;
                }

                await Notification_Send_Status("Copy", "Copying file or folder");
                Debug.WriteLine("Clipboard copy file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);

                //Reset and clear the clipboard
                Clipboard_ResetClear();

                //Set the clipboard variables
                dataBindFile.ClipboardType = ClipboardType.Copy;
                vClipboardFiles.Add(dataBindFile);

                //Update the clipboard status text
                Clipboard_UpdateStatusText();
            }
            catch { }
        }

        //Set copy file from the file picker
        async Task FilePicker_FileCopy_Checked()
        {
            try
            {
                await Notification_Send_Status("Copy", "Copying files and folders");
                Debug.WriteLine("Clipboard copy checked files and folders.");

                //Reset and clear the clipboard
                Clipboard_ResetClear();

                foreach (DataBindFile dataBindFile in List_FilePicker.Where(x => x.Checked == Visibility.Visible))
                {
                    //Check the file or folder
                    if (dataBindFile.FileType == FileType.FolderPre || dataBindFile.FileType == FileType.FilePre || dataBindFile.FileType == FileType.GoUp)
                    {
                        await Notification_Send_Status("Close", "Invalid file or folder");
                        Debug.WriteLine("Invalid file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                        return;
                    }

                    //Set the clipboard variables
                    dataBindFile.ClipboardType = ClipboardType.Copy;
                    vClipboardFiles.Add(dataBindFile);
                }

                //Update the clipboard status text
                Clipboard_UpdateStatusText();
            }
            catch { }
        }

        //Set cut file from the file picker
        async Task FilePicker_FileCut_Single(DataBindFile dataBindFile)
        {
            try
            {
                //Check the file or folder
                if (dataBindFile.FileType == FileType.FolderPre || dataBindFile.FileType == FileType.FilePre || dataBindFile.FileType == FileType.GoUp)
                {
                    await Notification_Send_Status("Close", "Invalid file or folder");
                    Debug.WriteLine("Invalid file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                    return;
                }

                await Notification_Send_Status("Cut", "Cutting file or folder");
                Debug.WriteLine("Clipboard cut file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);

                //Reset and clear the clipboard
                Clipboard_ResetClear();

                //Set the clipboard variables
                dataBindFile.ClipboardType = ClipboardType.Cut;
                vClipboardFiles.Add(dataBindFile);

                //Update the clipboard status text
                Clipboard_UpdateStatusText();
            }
            catch { }
        }

        //Set cut file from the file picker
        async Task FilePicker_FileCut_Checked()
        {
            try
            {
                await Notification_Send_Status("Cut", "Cutting files and folders");
                Debug.WriteLine("Clipboard cut checked files and folders.");

                //Reset and clear the clipboard
                Clipboard_ResetClear();

                foreach (DataBindFile dataBindFile in List_FilePicker.Where(x => x.Checked == Visibility.Visible))
                {
                    //Check the file or folder
                    if (dataBindFile.FileType == FileType.FolderPre || dataBindFile.FileType == FileType.FilePre || dataBindFile.FileType == FileType.GoUp)
                    {
                        await Notification_Send_Status("Close", "Invalid file or folder");
                        Debug.WriteLine("Invalid file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                        return;
                    }

                    //Set the clipboard variables
                    dataBindFile.ClipboardType = ClipboardType.Cut;
                    vClipboardFiles.Add(dataBindFile);
                }

                //Update the clipboard status text
                Clipboard_UpdateStatusText();
            }
            catch { }
        }

        //Paste file from the file picker
        async Task FilePicker_FilePaste()
        {
            try
            {
                bool resetClipboard = false;
                foreach (DataBindFile clipboardFile in vClipboardFiles)
                {
                    //Get the current file picker path
                    string oldFilePath = Path.GetFullPath(clipboardFile.PathFile);
                    string newFileName = Path.GetFileNameWithoutExtension(oldFilePath);
                    string newFileExtension = Path.GetExtension(oldFilePath);
                    string newFileDirectory = Path.GetFullPath(vFilePickerCurrentPath);
                    string newFilePath = Path.Combine(newFileDirectory, newFileName + newFileExtension);

                    //Move or copy the file or folder
                    if (clipboardFile.ClipboardType == ClipboardType.Cut)
                    {
                        await Notification_Send_Status("Cut", "Moving file or folder");
                        Debug.WriteLine("Moving file or folder: " + oldFilePath + " to " + newFilePath);

                        //Check if moving to same directory
                        if (oldFilePath == newFilePath)
                        {
                            await Notification_Send_Status("Cut", "Invalid move folder");
                            Debug.WriteLine("Moving file or folder to the same directory.");
                            return;
                        }

                        //Check if moving in the directory
                        if (newFilePath.Contains(oldFilePath))
                        {
                            await Notification_Send_Status("Cut", "Invalid move folder");
                            Debug.WriteLine("Moving file or folder to the sub directory.");
                            return;
                        }

                        //Check file or folder
                        FileAttributes fileAttribute = File.GetAttributes(oldFilePath);
                        if (fileAttribute.HasFlag(FileAttributes.Directory))
                        {
                            //Check if the directory exists
                            if (Directory.Exists(newFilePath))
                            {
                                //Count existing file names
                                int fileCount = Directory.GetDirectories(newFileDirectory, "*" + newFileName + "*").Count();

                                //Update the file name
                                newFileName += " - Cut (" + fileCount + ")";
                                newFilePath = Path.Combine(newFileDirectory, newFileName + newFileExtension);
                            }
                        }
                        else
                        {
                            //Check if the file exists
                            if (File.Exists(newFilePath))
                            {
                                //Count existing file names
                                int fileCount = Directory.GetFiles(newFileDirectory, "*" + newFileName + "*").Count();

                                //Update the file name
                                newFileName += " - Cut (" + fileCount + ")";
                                newFilePath = Path.Combine(newFileDirectory, newFileName + newFileExtension);
                            }
                        }

                        //Update file name in new clipboard
                        DataBindFile updatedClipboard = CloneClassObject(clipboardFile);
                        updatedClipboard.Name = newFileName + newFileExtension;
                        updatedClipboard.PathFile = newFilePath;
                        updatedClipboard.ClipboardType = ClipboardType.None;
                        updatedClipboard.Checked = Visibility.Collapsed;

                        //Remove the moved listbox item
                        await ListBoxRemoveItem(lb_FilePicker, List_FilePicker, clipboardFile, false);

                        //Add the new listbox item
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, updatedClipboard, false, false);

                        resetClipboard = true;

                        //Move file or folder
                        SHFILEOPSTRUCT shFileOpstruct = new SHFILEOPSTRUCT();
                        shFileOpstruct.wFunc = FILEOP_FUNC.FO_MOVE;
                        shFileOpstruct.pFrom = oldFilePath + "\0\0";
                        shFileOpstruct.pTo = newFilePath + "\0\0";
                        int shFileResult = SHFileOperation(ref shFileOpstruct);

                        //Check file operation status
                        if (shFileResult == 0 && !shFileOpstruct.fAnyOperationsAborted)
                        {
                            await Notification_Send_Status("Cut", "File or folder moved");
                            Debug.WriteLine("File or folder moved: " + oldFilePath + " to " + newFilePath);
                        }
                        else if (shFileOpstruct.fAnyOperationsAborted)
                        {
                            await Notification_Send_Status("Cut", "File or folder move aborted");
                            Debug.WriteLine("File or folder move aborted: " + oldFilePath + " to " + newFilePath);
                        }
                        else
                        {
                            await Notification_Send_Status("Cut", "File or folder move failed");
                            Debug.WriteLine("File or folder move failed: " + oldFilePath + " to " + newFilePath);
                        }
                    }
                    else
                    {
                        await Notification_Send_Status("Copy", "Copying file or folder");
                        Debug.WriteLine("Copying file or folder: " + oldFilePath + " to " + newFilePath);

                        //Check file or folder
                        FileAttributes fileAttribute = File.GetAttributes(oldFilePath);
                        if (fileAttribute.HasFlag(FileAttributes.Directory))
                        {
                            //Check if the directory exists
                            if (Directory.Exists(newFilePath))
                            {
                                //Count existing file names
                                int fileCount = Directory.GetDirectories(newFileDirectory, "*" + newFileName + "*").Count();

                                //Update the file name
                                newFileName += " - Copy (" + fileCount + ")";
                                newFilePath = Path.Combine(newFileDirectory, newFileName + newFileExtension);
                            }
                        }
                        else
                        {
                            //Check if the file exists
                            if (File.Exists(newFilePath))
                            {
                                //Count existing file names
                                int fileCount = Directory.GetFiles(newFileDirectory, "*" + newFileName + "*").Count();

                                //Update the file name
                                newFileName += " - Copy (" + fileCount + ")";
                                newFilePath = Path.Combine(newFileDirectory, newFileName + newFileExtension);
                            }
                        }

                        //Update file name in new clipboard
                        DataBindFile updatedClipboard = CloneClassObject(clipboardFile);
                        updatedClipboard.Name = newFileName + newFileExtension;
                        updatedClipboard.PathFile = newFilePath;
                        updatedClipboard.ClipboardType = ClipboardType.None;
                        updatedClipboard.Checked = Visibility.Collapsed;

                        //Add the new listbox item
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, updatedClipboard, false, false);

                        //Copy file or folder
                        SHFILEOPSTRUCT shFileOpstruct = new SHFILEOPSTRUCT();
                        shFileOpstruct.wFunc = FILEOP_FUNC.FO_COPY;
                        shFileOpstruct.pFrom = oldFilePath + "\0\0";
                        shFileOpstruct.pTo = newFilePath + "\0\0";
                        int shFileResult = SHFileOperation(ref shFileOpstruct);

                        //Check file operation status
                        if (shFileResult == 0 && !shFileOpstruct.fAnyOperationsAborted)
                        {
                            await Notification_Send_Status("Copy", "File or folder copied");
                            Debug.WriteLine("File or folder copied: " + oldFilePath + " to " + newFilePath);
                        }
                        else if (shFileOpstruct.fAnyOperationsAborted)
                        {
                            await Notification_Send_Status("Copy", "File or folder copy aborted");
                            Debug.WriteLine("File or folder copy aborted: " + oldFilePath + " to " + newFilePath);
                        }
                        else
                        {
                            await Notification_Send_Status("Copy", "File or folder copy failed");
                            Debug.WriteLine("File or folder copy failed: " + oldFilePath + " to " + newFilePath);
                        }
                    }
                }

                //Focus on the listbox item
                await ListboxFocusIndex(lb_FilePicker, false, true, -1);

                if (resetClipboard)
                {
                    //Reset and clear the clipboard
                    Clipboard_ResetClear();

                    //Update the clipboard status text
                    Clipboard_UpdateStatusText();
                }
            }
            catch (Exception ex)
            {
                await Notification_Send_Status("Paste", "Failed pasting");
                Debug.WriteLine("Failed pasting file or folder: " + ex.Message);
            }
        }

        //Rename file from the file picker
        async Task FilePicker_FileRename(DataBindFile dataBindFile)
        {
            try
            {
                //Check the file or folder
                if (dataBindFile.FileType == FileType.FolderPre || dataBindFile.FileType == FileType.FilePre || dataBindFile.FileType == FileType.GoUp)
                {
                    await Notification_Send_Status("Close", "Invalid file or folder");
                    Debug.WriteLine("Invalid file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                    return;
                }

                Debug.WriteLine("Renaming file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);

                //Show the text input popup
                string textInputString = await Popup_ShowHide_TextInput("Rename file or folder", dataBindFile.Name, "Rename the file or folder", false);

                //Check if file name changed
                if (textInputString == dataBindFile.Name)
                {
                    await Notification_Send_Status("Rename", "File name not changed");
                    Debug.WriteLine("The file name did not change.");
                    return;
                }

                //Check the changed file name
                if (!string.IsNullOrWhiteSpace(textInputString))
                {
                    string oldFilePath = Path.GetFullPath(dataBindFile.PathFile);
                    string newFileName = Path.GetFileNameWithoutExtension(textInputString);
                    string newFileExtension = Path.GetExtension(textInputString);
                    string newFileDirectory = Path.GetDirectoryName(oldFilePath);
                    string newFilePath = Path.Combine(newFileDirectory, newFileName + newFileExtension);

                    //Move file or folder
                    FileAttributes fileAttribute = File.GetAttributes(oldFilePath);
                    if (fileAttribute.HasFlag(FileAttributes.Directory))
                    {
                        //Check if the folder exists
                        if (Directory.Exists(newFilePath))
                        {
                            //Count existing file names
                            int fileCount = Directory.GetDirectories(newFileDirectory, "*" + newFileName + "*").Count();

                            //Update the file name
                            newFileName += " - Rename (" + fileCount + ")";
                            newFilePath = Path.Combine(newFileDirectory, newFileName + newFileExtension);
                        }
                        Directory_Move(oldFilePath, newFilePath, true);
                    }
                    else
                    {
                        //Check if the file exists
                        if (File.Exists(newFilePath))
                        {
                            //Count existing file names
                            int fileCount = Directory.GetFiles(newFileDirectory, "*" + newFileName + "*").Count();

                            //Update the file name
                            newFileName += " - Rename (" + fileCount + ")";
                            newFilePath = Path.Combine(newFileDirectory, newFileName + newFileExtension);
                        }
                        File_Move(oldFilePath, newFilePath, true);
                    }

                    //Update file name in listbox
                    dataBindFile.Name = newFileName + newFileExtension;
                    dataBindFile.PathFile = newFilePath;

                    //Update the clipboard status text
                    Clipboard_UpdateStatusText();

                    await Notification_Send_Status("Rename", "Renamed file or folder");
                    Debug.WriteLine("Renamed file or folder to: " + newFileName + newFileExtension);
                }
            }
            catch (Exception ex)
            {
                await Notification_Send_Status("Rename", "Failed renaming");
                Debug.WriteLine("Failed renaming file or folder: " + ex.Message);
            }
        }

        //Create a new folder
        async Task FilePicker_CreateFolder()
        {
            try
            {
                await Notification_Send_Status("FolderAdd", "Creating new folder");
                Debug.WriteLine("Creating new folder in: " + vFilePickerCurrentPath);

                //Show the text input popup
                string textInputString = await Popup_ShowHide_TextInput("Create folder", string.Empty, "Create new folder", false);

                //Check the folder create name
                if (!string.IsNullOrWhiteSpace(textInputString))
                {
                    string newFolderPath = Path.Combine(vFilePickerCurrentPath, textInputString);

                    //Check if the folder exists
                    if (Directory.Exists(newFolderPath))
                    {
                        await Notification_Send_Status("FolderAdd", "Folder already exists");
                        Debug.WriteLine("Create folder already exists.");
                        return;
                    }

                    //Create the new folder
                    DirectoryInfo listDirectory = Directory.CreateDirectory(newFolderPath);

                    //Create new folder databindfile
                    BitmapImage folderImage = FileToBitmapImage(new string[] { "Assets/Default/Icons/Folder.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    DataBindFile dataBindFileFolder = new DataBindFile() { FileType = FileType.Folder, Name = listDirectory.Name, DateModified = listDirectory.LastWriteTime, ImageBitmap = folderImage, PathFile = listDirectory.FullName };

                    //Add the new listbox item
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFolder, false, false);

                    //Focus on the listbox item
                    await ListboxFocusIndex(lb_FilePicker, false, true, -1);

                    //Check if there are files or folders
                    FilePicker_CheckFilesAndFoldersCount();

                    await Notification_Send_Status("FolderAdd", "Created new folder");
                    Debug.WriteLine("Created new folder in: " + newFolderPath);
                }
            }
            catch (Exception ex)
            {
                await Notification_Send_Status("FolderAdd", "Failed creating folder");
                Debug.WriteLine("Failed creating new folder: " + ex.Message);
            }
        }

        //Create a new text file
        async Task FilePicker_CreateTextFile()
        {
            try
            {
                await Notification_Send_Status("Font", "Creating text file");
                Debug.WriteLine("Creating new text file in: " + vFilePickerCurrentPath);

                //Show the text input popup
                string textInputString = await Popup_ShowHide_TextInput("Create text file", string.Empty, "Create new text file", false);

                //Check the text file create name
                if (!string.IsNullOrWhiteSpace(textInputString))
                {
                    string fileName = textInputString + ".txt";
                    string newFilePath = Path.Combine(vFilePickerCurrentPath, fileName);

                    //Check if the text file exists
                    if (File.Exists(newFilePath))
                    {
                        await Notification_Send_Status("Font", "Text file already exists");
                        Debug.WriteLine("Create text file already exists.");
                        return;
                    }

                    //Create the new text file
                    File.Create(newFilePath).Dispose();
                    DateTime dateCreated = DateTime.Now;

                    //Get the file size
                    string fileSize = AVFunctions.ConvertBytesSizeToString(0);

                    //Get the file date
                    string fileDate = dateCreated.ToShortDateString().Replace("-", "/");

                    //Set the detailed text
                    string fileDetailed = fileSize + " (" + fileDate + ")";

                    //Create new file databindfile
                    BitmapImage fileImage = FileToBitmapImage(new string[] { "Assets/Default/Extensions/Txt.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    DataBindFile dataBindFileFile = new DataBindFile() { FileType = FileType.File, Name = fileName, NameDetail = fileDetailed, DateModified = dateCreated, ImageBitmap = fileImage, PathFile = newFilePath };

                    //Add the new listbox item
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFile, false, false);

                    //Focus on the listbox item
                    await ListboxFocusIndex(lb_FilePicker, false, true, -1);

                    //Check if there are files or folders
                    FilePicker_CheckFilesAndFoldersCount();

                    await Notification_Send_Status("Font", "Created new text file");
                    Debug.WriteLine("Created new text file in: " + newFilePath);
                }
            }
            catch (Exception ex)
            {
                await Notification_Send_Status("Font", "Failed creating file");
                Debug.WriteLine("Failed creating new text file: " + ex.Message);
            }
        }

        //Remove file from the file picker
        async Task FilePicker_FileRemove(DataBindFile dataBindFile)
        {
            try
            {
                //Check the file or folder
                if (dataBindFile.FileType == FileType.FolderPre || dataBindFile.FileType == FileType.FilePre || dataBindFile.FileType == FileType.GoUp)
                {
                    await Notification_Send_Status("Close", "Invalid file or folder");
                    Debug.WriteLine("Invalid file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                    return;
                }

                //Use recyclebin or remove permanently
                bool useRecycleBin = true;

                //Confirm file remove prompt
                List<DataBindString> messageAnswers = new List<DataBindString>();
                DataBindString answerRecycle = new DataBindString();
                answerRecycle.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Remove.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                answerRecycle.Name = "Move file or folder to recycle bin*";
                messageAnswers.Add(answerRecycle);

                DataBindString answerPerma = new DataBindString();
                answerPerma.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/RemoveCross.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                answerPerma.Name = "Remove file or folder permanently";
                messageAnswers.Add(answerPerma);

                string deleteString = "Do you want to remove: " + dataBindFile.Name + "?";
                DataBindString messageResult = await Popup_Show_MessageBox("Remove file or folder", "* Files and folders on a network drive get permanently deleted.", deleteString, messageAnswers);
                if (messageResult == null)
                {
                    Debug.WriteLine("Cancelled file or folder removal.");
                    return;
                }
                else if (messageResult == answerPerma)
                {
                    useRecycleBin = false;
                }

                await Notification_Send_Status("Remove", "Remove file or folder");
                Debug.WriteLine("Removing file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);

                //Remove file or folder
                SHFILEOPSTRUCT shFileOpstruct = new SHFILEOPSTRUCT();
                shFileOpstruct.wFunc = FILEOP_FUNC.FO_DELETE;
                shFileOpstruct.pFrom = dataBindFile.PathFile + "\0\0";
                if (useRecycleBin)
                {
                    shFileOpstruct.fFlags = FILEOP_FLAGS.FOF_NOCONFIRMATION | FILEOP_FLAGS.FOF_ALLOWUNDO;
                }
                else
                {
                    shFileOpstruct.fFlags = FILEOP_FLAGS.FOF_NOCONFIRMATION;
                }
                int shFileResult = SHFileOperation(ref shFileOpstruct);

                //Check if the removed item is clipboard and reset it
                DataBindFile clipboardFile = vClipboardFiles.Where(x => x.PathFile == dataBindFile.PathFile).FirstOrDefault();
                if (clipboardFile != null)
                {
                    //Remove the clipboard item
                    clipboardFile.ClipboardType = ClipboardType.None;
                    vClipboardFiles.Remove(clipboardFile);

                    //Update the clipboard status text
                    Clipboard_UpdateStatusText();
                }

                //Remove file from the listbox
                await ListBoxRemoveItem(lb_FilePicker, List_FilePicker, dataBindFile, true);

                //Check if there are files or folders
                FilePicker_CheckFilesAndFoldersCount();

                //Check file operation status
                if (shFileResult == 0 && !shFileOpstruct.fAnyOperationsAborted)
                {
                    await Notification_Send_Status("Remove", "Re/moved file or folder");
                    Debug.WriteLine("Removed file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                }
                else if (shFileOpstruct.fAnyOperationsAborted)
                {
                    await Notification_Send_Status("Remove", "File or folder removal aborted");
                    Debug.WriteLine("File or folder removal aborted: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                }
                else
                {
                    await Notification_Send_Status("Remove", "File or folder removal failed");
                    Debug.WriteLine("File or folder removal failed: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                }
            }
            catch (Exception ex)
            {
                await Notification_Send_Status("Remove", "Failed removing");
                Debug.WriteLine("Failed removing file or folder: " + ex.Message);
            }
        }

        //Eject or unmount disc
        async Task<bool> FilePicker_EjectDrive(DataBindFile dataBindFile, string driveLetter)
        {
            try
            {
                driveLetter = Path.GetPathRoot(driveLetter).Replace("\\", string.Empty);
                string devicePath = @"\\.\" + driveLetter;

                Debug.WriteLine("Ejecting the disc drive: " + devicePath);
                await Notification_Send_Status("FolderDisc", "Ejecting the disc");

                SECURITY_ATTRIBUTES security = new SECURITY_ATTRIBUTES();
                security.lpSecurityDescriptor = IntPtr.Zero;
                security.bInheritHandle = true;
                security.nLength = Marshal.SizeOf(security);

                uint fileAttributes = (uint)FILE_ATTRIBUTE.FILE_ATTRIBUTE_NORMAL | (uint)FILE_FLAG.FILE_FLAG_NORMAL;
                uint desiredAccess = (uint)GENERIC_MODE.GENERIC_WRITE | (uint)GENERIC_MODE.GENERIC_READ;
                uint shareMode = (uint)FILE_SHARE.FILE_SHARE_READ | (uint)FILE_SHARE.FILE_SHARE_WRITE;

                //Open the drive
                IntPtr fileHandle = CreateFile(devicePath, desiredAccess, shareMode, ref security, OPEN_EXISTING, fileAttributes, 0);
                if (fileHandle == IntPtr.Zero || fileHandle == (IntPtr)INVALID_HANDLE_VALUE)
                {
                    await Notification_Send_Status("Close", "Ejecting disc failed");
                    return false;
                }

                //Eject the media
                DeviceIoControl(fileHandle, IoControlCodes.IOCTL_STORAGE_EJECT_MEDIA, IntPtr.Zero, 0, IntPtr.Zero, 0, out uint TransferredEject, IntPtr.Zero);

                //Close the drive
                LibraryUsb.NativeMethods_Hid.CloseHandle(fileHandle);

                //Remove drive from the listbox
                await ListBoxRemoveItem(lb_FilePicker, List_FilePicker, dataBindFile, true);

                await Notification_Send_Status("FolderDisc", "Ejected the disc");
                return true;
            }
            catch { }
            return false;
        }

        //Get the total directory size
        public static long GetDirectorySize(DirectoryInfo directoryInfo)
        {
            long directorySize = 0;
            try
            {
                FileInfo[] fileList = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
                foreach (FileInfo file in fileList)
                {
                    directorySize += file.Length;
                }
            }
            catch { }
            return directorySize;
        }
    }
}