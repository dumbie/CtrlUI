using ArnoldVinkCode;
using Shell32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInteropDll;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show the recyclebin manager popup
        async Task ShowRecycleBinManager()
        {
            try
            {
                List<DataBindString> Answers = new List<DataBindString>();

                //Add empty the recycle bin
                DataBindString answerEmpty = new DataBindString();
                answerEmpty.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Remove.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                answerEmpty.Name = "Empty the recycle bin";
                Answers.Add(answerEmpty);

                //Add files and folders from recycle bin
                ListLoadAllRecycleBinFiles(Answers);

                //Check if there are any items
                if (Answers.Count <= 1)
                {
                    await Notification_Send_Status("Remove", "No files in recycle bin");
                    Debug.WriteLine("Recycle bin does not have any files or folders.");
                    return;
                }

                //Show recycle bin prompt
                DataBindString messageResult = await Popup_Show_MessageBox("Recycle bin", "", "Please select a file or folder to interact with:", Answers);
                if (messageResult != null)
                {
                    if (messageResult == answerEmpty)
                    {
                        await RecycleBin_Empty();
                    }
                    else if (messageResult.Data1 != null)
                    {
                        await RecycleBin_RestoreDelete((FolderItem)messageResult.Data1, (Folder)messageResult.Data2);
                    }
                }
            }
            catch { }
        }

        //Add files and folders from recycle bin
        void ListLoadAllRecycleBinFiles(List<DataBindString> targetList)
        {
            try
            {
                Shell shell = new Shell();
                Folder folderShell = shell.NameSpace(10);

                //Load file and folder images
                BitmapImage listImageFolder = FileToBitmapImage(new string[] { "Assets/Icons/Folder.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                BitmapImage listImageFile = FileToBitmapImage(new string[] { "Assets/Icons/File.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);

                //Add recycle bin items to the list
                foreach (FolderItem folderItem in folderShell.Items())
                {
                    DataBindString answerRecycleItem = new DataBindString();
                    if (folderItem.IsFolder)
                    {
                        answerRecycleItem.ImageBitmap = listImageFolder;
                    }
                    else
                    {
                        answerRecycleItem.ImageBitmap = listImageFile;
                    }
                    answerRecycleItem.Name = folderItem.Name;
                    answerRecycleItem.Data1 = folderItem;
                    answerRecycleItem.Data2 = folderShell;
                    targetList.Add(answerRecycleItem);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Recyclebin list error: " + ex.Message);
            }
        }

        //Restore or delete item from reycle bin
        async Task RecycleBin_RestoreDelete(FolderItem folderItem, Folder folder)
        {
            try
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString answerRestore = new DataBindString();
                answerRestore.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Restart.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                if (folderItem.IsFolder)
                {
                    answerRestore.Name = "Restore folder to disk";
                }
                else
                {
                    answerRestore.Name = "Restore file to disk";
                }
                Answers.Add(answerRestore);

                DataBindString answerDelete = new DataBindString();
                answerDelete.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Remove.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                if (folderItem.IsFolder)
                {
                    answerDelete.Name = "Permanently delete folder";
                }
                else
                {
                    answerDelete.Name = "Permanently delete file";
                }
                Answers.Add(answerDelete);

                DataBindString messageResult = await Popup_Show_MessageBox("Restore or delete", "", "Do you want to restore or permanantly delete " + folderItem.Name + "?", Answers);
                if (messageResult != null)
                {
                    if (messageResult == answerRestore)
                    {
                        //Get the original file path
                        string originalFullPath = Path.Combine(folder.GetDetailsOf(folderItem, 1), folder.GetDetailsOf(folderItem, 0));

                        //Restore the selected item
                        SHFILEOPSTRUCT shFileOpstruct = new SHFILEOPSTRUCT();
                        shFileOpstruct.wFunc = FILEOP_FUNC.FO_MOVE;
                        shFileOpstruct.pFrom = folderItem.Path + "\0\0";
                        shFileOpstruct.pTo = originalFullPath + "\0\0";
                        int shFileResult = SHFileOperation(ref shFileOpstruct);

                        //Check file operation status
                        if (shFileResult == 0 && !shFileOpstruct.fAnyOperationsAborted)
                        {
                            await Notification_Send_Status("Restart", "File or folder restored");
                        }
                    }
                    else if (messageResult == answerDelete)
                    {
                        //Delete the selected item
                        SHFILEOPSTRUCT shFileOpstruct = new SHFILEOPSTRUCT();
                        shFileOpstruct.wFunc = FILEOP_FUNC.FO_DELETE;
                        shFileOpstruct.pFrom = folderItem.Path + "\0\0";
                        shFileOpstruct.fFlags = FILEOP_FLAGS.FOF_NOCONFIRMATION;
                        int shFileResult = SHFileOperation(ref shFileOpstruct);

                        //Check file operation status
                        if (shFileResult == 0 && !shFileOpstruct.fAnyOperationsAborted)
                        {
                            await Notification_Send_Status("Remove", "File or folder deleted");
                        }
                    }
                }
            }
            catch { }
        }

        //Empty the Windows Recycle Bin
        async Task RecycleBin_Empty()
        {
            try
            {
                List<DataBindString> messageAnswers = new List<DataBindString>();
                DataBindString answerEmpty = new DataBindString();
                answerEmpty.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Remove.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                answerEmpty.Name = "Empty recycle bin";
                messageAnswers.Add(answerEmpty);

                DataBindString messageResult = await Popup_Show_MessageBox("Empty the recycle bin?", "", "This will permanently remove all the files and folders from your recycle bin.", messageAnswers);
                if (messageResult != null && messageResult == answerEmpty)
                {
                    await Notification_Send_Status("Remove", "Emptying recycle bin");
                    Debug.WriteLine("Emptying the Windows recycle bin.");

                    //Play recycle bin empty sound
                    PlayInterfaceSound(vConfigurationApplication, "RecycleBinEmpty", false);

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
                    await AVActions.TaskStart(TaskAction);
                }
            }
            catch { }
        }
    }
}