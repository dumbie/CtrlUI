using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInteropDll;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task FilePicker_FileRemove_Remove(DataBindFile dataBindFile, bool useRecycleBin)
        {
            try
            {
                //Check the file or folder
                if (dataBindFile.FileType == FileType.FolderPre || dataBindFile.FileType == FileType.FilePre || dataBindFile.FileType == FileType.GoUpPre)
                {
                    await Notification_Send_Status("Close", "Invalid file or folder");
                    Debug.WriteLine("Invalid file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                    return;
                }

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
                }

                //Remove file from the listbox
                await ListBoxRemoveItem(lb_FilePicker, List_FilePicker, dataBindFile, true);

                //Check if there are files or folders
                FilePicker_CheckFilesAndFoldersCount();

                //Check file operation status
                if (shFileResult == 0 && !shFileOpstruct.fAnyOperationsAborted)
                {
                    if (useRecycleBin)
                    {
                        await Notification_Send_Status("Remove", "Recycled file or folder");
                    }
                    else
                    {
                        await Notification_Send_Status("Remove", "Removed file or folder");
                    }
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
            catch { }
        }

        async Task FilePicker_FileRemove_Single(DataBindFile dataBindFile)
        {
            try
            {
                //Confirm file remove prompt
                List<DataBindString> messageAnswers = new List<DataBindString>();
                DataBindString answerRecycle = new DataBindString();
                answerRecycle.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Remove.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                answerRecycle.Name = "Move file or folder to recycle bin*";
                messageAnswers.Add(answerRecycle);

                DataBindString answerPerma = new DataBindString();
                answerPerma.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/RemoveCross.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                answerPerma.Name = "Remove file or folder permanently";
                messageAnswers.Add(answerPerma);

                bool useRecycleBin = true;
                string deleteString = "Do you want to remove: " + dataBindFile.Name + "?";
                DataBindString messageResult = await Popup_Show_MessageBox("Remove file or folder", "* Files and folders on a network drive get permanently deleted.", deleteString, messageAnswers);
                if (messageResult != null)
                {
                    if (messageResult == answerPerma)
                    {
                        useRecycleBin = false;
                    }
                }
                else
                {
                    Debug.WriteLine("Cancelled file or folder removal.");
                    return;
                }

                await Notification_Send_Status("Remove", "Removing file or folder");
                Debug.WriteLine("Removing file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);

                //Remove the file or folder
                await FilePicker_FileRemove_Remove(dataBindFile, useRecycleBin);

                //Update the clipboard status text
                Clipboard_UpdateStatusText();
            }
            catch { }
        }

        async Task FilePicker_FileRemove_Checked()
        {
            try
            {
                //Confirm file remove prompt
                List<DataBindString> messageAnswers = new List<DataBindString>();
                DataBindString answerRecycle = new DataBindString();
                answerRecycle.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Remove.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                answerRecycle.Name = "Move files or folders to recycle bin*";
                messageAnswers.Add(answerRecycle);

                DataBindString answerPerma = new DataBindString();
                answerPerma.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/RemoveCross.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                answerPerma.Name = "Remove files or folders permanently";
                messageAnswers.Add(answerPerma);

                bool useRecycleBin = true;
                string deleteString = "Do you want to remove the selected files or folders?";
                DataBindString messageResult = await Popup_Show_MessageBox("Remove files or folders", "* Files and folders on a network drive get permanently deleted.", deleteString, messageAnswers);
                if (messageResult != null)
                {
                    if (messageResult == answerPerma)
                    {
                        useRecycleBin = false;
                    }
                }
                else
                {
                    Debug.WriteLine("Cancelled files or folders removal.");
                    return;
                }

                await Notification_Send_Status("Remove", "Removing files or folders");

                //Remove the files or folders
                foreach (DataBindFile dataBindFile in List_FilePicker.Where(x => x.Checked == Visibility.Visible).ToList())
                {
                    Debug.WriteLine("Removing files or folders: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                    await FilePicker_FileRemove_Remove(dataBindFile, useRecycleBin);
                }

                //Update the clipboard status text
                Clipboard_UpdateStatusText();
            }
            catch { }
        }
    }
}