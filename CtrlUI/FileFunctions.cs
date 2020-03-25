using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.AVInteropDll;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

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

                //Launch the Win32 application
                await LaunchProcessManuallyWin32(vFilePickerResult.PathFile, "", "", false, true, false, false);
            }
            catch { }
        }


        //Set copy file from the file picker
        void FilePicker_FileCopy(DataBindFile dataBindFile)
        {
            try
            {
                //Check the file or folder
                if (dataBindFile.Type == "PreDirectory" || dataBindFile.Type == "GoUp")
                {
                    Popup_Show_Status("Close", "Invalid file or folder");
                    Debug.WriteLine("Invalid file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                    return;
                }

                Popup_Show_Status("Copy", "Copying file or folder");
                Debug.WriteLine("Clipboard copy file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);

                //Set the clipboard variables
                vClipboardFile = dataBindFile;
                vClipboardType = "Copy";

                //Update the interface text
                grid_Popup_FilePicker_textblock_ClipboardStatus.Text = "Clipboard (" + vClipboardType + ") " + vClipboardFile.PathFile;
            }
            catch { }
        }

        //Set cut file from the file picker
        void FilePicker_FileCut(DataBindFile dataBindFile)
        {
            try
            {
                //Check the file or folder
                if (dataBindFile.Type == "PreDirectory" || dataBindFile.Type == "GoUp")
                {
                    Popup_Show_Status("Close", "Invalid file or folder");
                    Debug.WriteLine("Invalid file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                    return;
                }

                Popup_Show_Status("Cut", "Cutting file or folder");
                Debug.WriteLine("Clipboard cut file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);

                //Set the clipboard variables
                vClipboardFile = dataBindFile;
                vClipboardType = "Cut";

                //Update the interface text
                grid_Popup_FilePicker_textblock_ClipboardStatus.Text = "Clipboard (" + vClipboardType + ") " + vClipboardFile.PathFile;
            }
            catch { }
        }

        //Paste file from the file picker
        async Task FilePicker_FilePaste()
        {
            try
            {
                //Get the current file picker path
                string oldFilePath = Path.GetFullPath(vClipboardFile.PathFile);
                string newFileName = Path.GetFileNameWithoutExtension(oldFilePath);
                string newFileExtension = Path.GetExtension(oldFilePath);
                string newFileDirectory = Path.GetFullPath(vFilePickerCurrentPath);
                string newFilePath = Path.Combine(newFileDirectory, newFileName + newFileExtension);

                //Move or copy the file or folder
                if (vClipboardType == "Cut")
                {
                    Popup_Show_Status("Cut", "Moving file or folder");
                    Debug.WriteLine("Moving file or folder: " + oldFilePath + " to " + newFilePath);

                    //Check if moving to same directory
                    if (oldFilePath == newFilePath)
                    {
                        Popup_Show_Status("Cut", "Invalid move folder");
                        Debug.WriteLine("Moving file or folder to the same directory.");
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
                    DataBindFile updatedClipboard = CloneClassObject(vClipboardFile);
                    updatedClipboard.Name = newFileName + newFileExtension;
                    updatedClipboard.PathFile = newFilePath;

                    //Remove the moved listbox item
                    await ListBoxRemoveItem(lb_FilePicker, List_FilePicker, vClipboardFile, false);

                    //Add the new listbox item
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, updatedClipboard, false, false);

                    //Focus on the listbox item
                    await ListboxFocus(lb_FilePicker, false, true, -1);

                    //Reset the current clipboard
                    vClipboardFile = null;
                    vClipboardType = string.Empty;
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        grid_Popup_FilePicker_textblock_ClipboardStatus.Text = string.Empty;
                    });

                    //Move file or folder
                    SHFILEOPSTRUCT shFileOpstruct = new SHFILEOPSTRUCT();
                    shFileOpstruct.wFunc = FILEOP_FUNC.FO_MOVE;
                    shFileOpstruct.pFrom = oldFilePath + "\0\0";
                    shFileOpstruct.pTo = newFilePath + "\0\0";
                    int shFileResult = SHFileOperation(ref shFileOpstruct);

                    //Check file operation status
                    if (shFileResult == 0 && !shFileOpstruct.fAnyOperationsAborted)
                    {
                        Popup_Show_Status("Cut", "File or folder moved");
                        Debug.WriteLine("File or folder moved: " + oldFilePath + " to " + newFilePath);
                    }
                    else if (shFileOpstruct.fAnyOperationsAborted)
                    {
                        Popup_Show_Status("Cut", "File or folder move aborted");
                        Debug.WriteLine("File or folder move aborted: " + oldFilePath + " to " + newFilePath);
                    }
                    else
                    {
                        Popup_Show_Status("Cut", "File or folder move failed");
                        Debug.WriteLine("File or folder move failed: " + oldFilePath + " to " + newFilePath);
                    }
                }
                else
                {
                    Popup_Show_Status("Copy", "Copying file or folder");
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
                    DataBindFile updatedClipboard = CloneClassObject(vClipboardFile);
                    updatedClipboard.Name = newFileName + newFileExtension;
                    updatedClipboard.PathFile = newFilePath;

                    //Add the new listbox item
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, updatedClipboard, false, false);

                    //Focus on the listbox item
                    await ListboxFocus(lb_FilePicker, false, true, -1);

                    //Copy file or folder
                    SHFILEOPSTRUCT shFileOpstruct = new SHFILEOPSTRUCT();
                    shFileOpstruct.wFunc = FILEOP_FUNC.FO_COPY;
                    shFileOpstruct.pFrom = oldFilePath + "\0\0";
                    shFileOpstruct.pTo = newFilePath + "\0\0";
                    int shFileResult = SHFileOperation(ref shFileOpstruct);

                    //Check file operation status
                    if (shFileResult == 0 && !shFileOpstruct.fAnyOperationsAborted)
                    {
                        Popup_Show_Status("Copy", "File or folder copied");
                        Debug.WriteLine("File or folder copied: " + oldFilePath + " to " + newFilePath);
                    }
                    else if (shFileOpstruct.fAnyOperationsAborted)
                    {
                        Popup_Show_Status("Copy", "File or folder copy aborted");
                        Debug.WriteLine("File or folder copy aborted: " + oldFilePath + " to " + newFilePath);
                    }
                    else
                    {
                        Popup_Show_Status("Copy", "File or folder copy failed");
                        Debug.WriteLine("File or folder copy failed: " + oldFilePath + " to " + newFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Popup_Show_Status("Paste", "Failed pasting");
                Debug.WriteLine("Failed pasting file or folder: " + ex.Message);
            }
        }

        //Rename file from the file picker
        async Task FilePicker_FileRename(DataBindFile dataBindFile)
        {
            try
            {
                //Check the file or folder
                if (dataBindFile.Type == "PreDirectory" || dataBindFile.Type == "GoUp")
                {
                    Popup_Show_Status("Close", "Invalid file or folder");
                    Debug.WriteLine("Invalid file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                    return;
                }

                Popup_Show_Status("Rename", "Renaming file or folder");
                Debug.WriteLine("Renaming file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);

                //Show the text input popup
                string textInputString = await Popup_ShowHide_TextInput("Rename file or folder", dataBindFile.Name, "Rename the file or folder");

                //Check if file name changed
                if (textInputString == dataBindFile.Name)
                {
                    Popup_Show_Status("Rename", "File name not changed");
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

                    //Check if the renamed item is clipboard and update it
                    if (vClipboardFile != null && vClipboardFile.PathFile == dataBindFile.PathFile)
                    {
                        grid_Popup_FilePicker_textblock_ClipboardStatus.Text = "Clipboard (" + vClipboardType + ") " + vClipboardFile.PathFile;
                    }

                    Popup_Show_Status("Rename", "Renamed file or folder");
                    Debug.WriteLine("Renamed file or folder to: " + newFileName + newFileExtension);
                }
            }
            catch (Exception ex)
            {
                Popup_Show_Status("Rename", "Failed renaming");
                Debug.WriteLine("Failed renaming file or folder: " + ex.Message);
            }
        }

        //Create a new folder
        async Task FilePicker_FolderCreate()
        {
            try
            {
                Popup_Show_Status("FolderAdd", "Creating new folder");
                Debug.WriteLine("Creating new folder in: " + vFilePickerCurrentPath);

                //Show the text input popup
                string textInputString = await Popup_ShowHide_TextInput("Create folder", string.Empty, "Create new folder");

                //Check the folder create name
                if (!string.IsNullOrWhiteSpace(textInputString))
                {
                    string newFolderPath = Path.Combine(vFilePickerCurrentPath, textInputString);

                    //Check if the folder exists
                    if (Directory.Exists(newFolderPath))
                    {
                        Popup_Show_Status("FolderAdd", "Folder already exists");
                        Debug.WriteLine("Create folder already exists.");
                        return;
                    }

                    //Create the new folder
                    DirectoryInfo listDirectory = Directory.CreateDirectory(newFolderPath);

                    //Create new folder databindfile
                    BitmapImage folderImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Folder.png" }, IntPtr.Zero, -1, 0);
                    DataBindFile dataBindFileFolder = new DataBindFile() { Type = "Directory", Name = listDirectory.Name, DateModified = listDirectory.LastWriteTime, ImageBitmap = folderImage, PathFile = listDirectory.FullName };

                    //Add the new listbox item
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFolder, false, false);

                    //Focus on the listbox item
                    await ListboxFocus(lb_FilePicker, false, true, -1);

                    //Check if there are files or folders
                    FilePicker_CheckFilesAndFoldersCount();

                    Popup_Show_Status("FolderAdd", "Created new folder");
                    Debug.WriteLine("Created new folder in: " + newFolderPath);
                }
            }
            catch (Exception ex)
            {
                Popup_Show_Status("FolderAdd", "Failed creating");
                Debug.WriteLine("Failed creating new folder: " + ex.Message);
            }
        }

        //Remove file from the file picker
        async Task FilePicker_FileRemove(DataBindFile dataBindFile, bool useRecycleBin)
        {
            try
            {
                //Check the file or folder
                if (dataBindFile.Type == "PreDirectory" || dataBindFile.Type == "GoUp")
                {
                    Popup_Show_Status("Close", "Invalid file or folder");
                    Debug.WriteLine("Invalid file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                    return;
                }

                Popup_Show_Status("Remove", "Remove file or folder");
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
                if (vClipboardFile != null && vClipboardFile.PathFile == dataBindFile.PathFile)
                {
                    vClipboardFile = null;
                    vClipboardType = string.Empty;
                    grid_Popup_FilePicker_textblock_ClipboardStatus.Text = string.Empty;
                }

                //Remove file from the listbox
                await ListBoxRemoveItem(lb_FilePicker, List_FilePicker, dataBindFile, true);

                //Check if there are files or folders
                FilePicker_CheckFilesAndFoldersCount();

                //Check file operation status
                if (shFileResult == 0 && !shFileOpstruct.fAnyOperationsAborted)
                {
                    Popup_Show_Status("Remove", "Re/moved file or folder");
                    Debug.WriteLine("Removed file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                }
                else if (shFileOpstruct.fAnyOperationsAborted)
                {
                    Popup_Show_Status("Remove", "File or folder removal aborted");
                    Debug.WriteLine("File or folder removal aborted: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                }
                else
                {
                    Popup_Show_Status("Remove", "File or folder removal failed");
                    Debug.WriteLine("File or folder removal failed: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                }
            }
            catch (Exception ex)
            {
                Popup_Show_Status("Remove", "Failed removing");
                Debug.WriteLine("Failed removing file or folder: " + ex.Message);
            }
        }

        //Empty the Windows Recycle Bin
        async Task RecycleBin_Empty()
        {
            try
            {
                List<DataBindString> messageAnswers = new List<DataBindString>();
                DataBindString answerEmpty = new DataBindString();
                answerEmpty.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Remove.png" }, IntPtr.Zero, -1, 0);
                answerEmpty.Name = "Empty recycle bin";
                messageAnswers.Add(answerEmpty);

                DataBindString messageResult = await Popup_Show_MessageBox("Empty the recycle bin?", "", "This will permanently remove all the files from your recycle bin.", messageAnswers);
                if (messageResult != null && messageResult == answerEmpty)
                {
                    Popup_Show_Status("Remove", "Emptying recycle bin");
                    Debug.WriteLine("Emptying the Windows recycle bin.");

                    //Play recycle bin empty sound
                    PlayInterfaceSound(vInterfaceSoundVolume, "RecycleBinEmpty", false);

                    //Prepare the recycle bin task
                    void TaskAction()
                    {
                        try
                        {
                            SHEmptyRecycleBin(IntPtr.Zero, null, RecycleBin_FLAGS.SHRB_NOCONFIRMATION | RecycleBin_FLAGS.SHRB_NOSOUND);
                        }
                        catch { }
                    }

                    //Empty the windows recycle bin
                    await AVActions.TaskStart(TaskAction, null);
                }
            }
            catch { }
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