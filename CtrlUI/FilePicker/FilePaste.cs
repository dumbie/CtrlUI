using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.AVFocus;
using static ArnoldVinkCode.AVInteropDll;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
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
                        Notification_Show_Status("Cut", "Moving file or folder");
                        Debug.WriteLine("Moving file or folder: " + oldFilePath + " to " + newFilePath);

                        //Check if moving to same directory
                        if (oldFilePath == newFilePath)
                        {
                            Notification_Show_Status("Cut", "Invalid move folder");
                            Debug.WriteLine("Moving file or folder to the same directory.");
                            return;
                        }

                        //Check if moving in the directory
                        if (newFilePath.Contains(oldFilePath))
                        {
                            Notification_Show_Status("Cut", "Invalid move folder");
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
                        if (!CloneObjectShallow(clipboardFile, out DataBindFile updatedClipboard)) { return; }
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
                            Notification_Show_Status("Cut", "File or folder moved");
                            Debug.WriteLine("File or folder moved: " + oldFilePath + " to " + newFilePath);
                        }
                        else if (shFileOpstruct.fAnyOperationsAborted)
                        {
                            Notification_Show_Status("Cut", "File or folder move aborted");
                            Debug.WriteLine("File or folder move aborted: " + oldFilePath + " to " + newFilePath);
                        }
                        else
                        {
                            Notification_Show_Status("Cut", "File or folder move failed");
                            Debug.WriteLine("File or folder move failed: " + oldFilePath + " to " + newFilePath);
                        }
                    }
                    else
                    {
                        Notification_Show_Status("Copy", "Copying file or folder");
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
                        if (!CloneObjectShallow(clipboardFile, out DataBindFile updatedClipboard)) { return; }
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
                            Notification_Show_Status("Copy", "File or folder copied");
                            Debug.WriteLine("File or folder copied: " + oldFilePath + " to " + newFilePath);
                        }
                        else if (shFileOpstruct.fAnyOperationsAborted)
                        {
                            Notification_Show_Status("Copy", "File or folder copy aborted");
                            Debug.WriteLine("File or folder copy aborted: " + oldFilePath + " to " + newFilePath);
                        }
                        else
                        {
                            Notification_Show_Status("Copy", "File or folder copy failed");
                            Debug.WriteLine("File or folder copy failed: " + oldFilePath + " to " + newFilePath);
                        }
                    }
                }

                //Focus on the listbox item
                await ListBoxFocusIndex(lb_FilePicker, true, 0, vProcessCurrent.WindowHandleMain);

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
                Notification_Show_Status("Paste", "Failed pasting");
                Debug.WriteLine("Failed pasting file or folder: " + ex.Message);
            }
        }
    }
}