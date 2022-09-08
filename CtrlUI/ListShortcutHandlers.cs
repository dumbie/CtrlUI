using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInteropDll;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.FocusFunctions;
using static LibraryShared.JsonFunctions;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task RightClickShortcut(ListBox listboxSender, int listboxSelectedIndex, DataBindApp dataBindApp)
        {
            try
            {
                //Get process details
                string processDetails = dataBindApp.PathExe;
                if (!string.IsNullOrWhiteSpace(dataBindApp.NameExe))
                {
                    processDetails += " (" + dataBindApp.NameExe + ")";
                }

                //Get process running time
                string processRunningTimeString = ApplicationRunningTimeString(dataBindApp.RunningTime, "shortcut process");
                if (string.IsNullOrWhiteSpace(processRunningTimeString))
                {
                    processRunningTimeString = processDetails;
                }
                else
                {
                    processRunningTimeString += "\n" + processDetails;
                }

                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString AnswerDownload = new DataBindString();
                AnswerDownload.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Download.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerDownload.Name = "Download game information";
                Answers.Add(AnswerDownload);

                DataBindString AnswerRemove = new DataBindString();
                AnswerRemove.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Remove.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerRemove.Name = "Move shortcut file to recycle bin";
                Answers.Add(AnswerRemove);

                DataBindString AnswerRename = new DataBindString();
                AnswerRename.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Rename.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerRename.Name = "Rename the shortcut file";
                Answers.Add(AnswerRename);

                DataBindString AnswerHide = new DataBindString();
                AnswerHide.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Hide.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerHide.Name = "Hide this shortcut";
                Answers.Add(AnswerHide);

                DataBindString messageResult = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", processRunningTimeString, "", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerDownload)
                    {
                        DownloadInfoGame informationDownloaded = await DownloadInfoGame(dataBindApp.Name, 100, true);
                        if (informationDownloaded != null)
                        {
                            if (informationDownloaded.ImageBitmap != null)
                            {
                                dataBindApp.ImageBitmap = informationDownloaded.ImageBitmap;
                            }
                        }
                    }
                    else if (messageResult == AnswerRemove)
                    {
                        await RemoveShortcutFile(listboxSender, listboxSelectedIndex, dataBindApp, false);
                    }
                    else if (messageResult == AnswerRename)
                    {
                        await RenameShortcutFile(dataBindApp);
                    }
                    else if (messageResult == AnswerHide)
                    {
                        await HideShortcutFile(listboxSender, listboxSelectedIndex, dataBindApp);
                    }
                }
            }
            catch { }
        }

        //Hide the shortcut file
        async Task HideShortcutFile(ListBox listboxSender, int listboxSelectedIndex, DataBindApp dataBindApp)
        {
            try
            {
                await Notification_Send_Status("Hide", "Hiding shortcut " + dataBindApp.Name);
                Debug.WriteLine("Hiding shortcut by name: " + dataBindApp.Name + " path: " + dataBindApp.ShortcutPath);

                //Create new profile shared
                ProfileShared profileShared = new ProfileShared();
                profileShared.String1 = dataBindApp.Name;

                //Add shortcut file to the ignore list
                vCtrlIgnoreShortcutName.Add(profileShared);
                JsonSaveObject(vCtrlIgnoreShortcutName, @"User\CtrlIgnoreShortcutName");

                //Remove application from the list
                await RemoveAppFromList(dataBindApp, false, false, true);

                //Select the previous index
                await ListboxFocusIndex(listboxSender, false, false, listboxSelectedIndex, vProcessCurrent.MainWindowHandle);
            }
            catch (Exception ex)
            {
                await Notification_Send_Status("Hide", "Failed hiding");
                Debug.WriteLine("Failed hiding shortcut: " + ex.Message);
            }
        }

        //Remove the shortcut file
        async Task RemoveShortcutFile(ListBox listboxSender, int listboxSelectedIndex, DataBindApp dataBindApp, bool silent)
        {
            try
            {
                //Confirm shortcut remove prompt
                if (!silent)
                {
                    List<DataBindString> messageAnswers = new List<DataBindString>();
                    DataBindString answerYes = new DataBindString();
                    answerYes.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Remove.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    answerYes.Name = "Move shortcut file to recycle bin";
                    messageAnswers.Add(answerYes);

                    string deleteString = "Are you sure you want to remove: " + dataBindApp.Name + "?";
                    DataBindString messageResult = await Popup_Show_MessageBox("Remove shortcut", "", deleteString, messageAnswers);
                    if (messageResult == null)
                    {
                        Debug.WriteLine("Cancelled shortcut removal.");
                        return;
                    }
                }

                await Notification_Send_Status("Minus", "Removing shortcut " + dataBindApp.Name);
                Debug.WriteLine("Removing shortcut: " + dataBindApp.Name + " path: " + dataBindApp.ShortcutPath);

                //Move the shortcut file to recycle bin
                SHFILEOPSTRUCT shFileOpstruct = new SHFILEOPSTRUCT();
                shFileOpstruct.wFunc = FILEOP_FUNC.FO_DELETE;
                shFileOpstruct.pFrom = dataBindApp.ShortcutPath + "\0\0";
                shFileOpstruct.fFlags = FILEOP_FLAGS.FOF_NOCONFIRMATION | FILEOP_FLAGS.FOF_ALLOWUNDO;
                int shFileResult = SHFileOperation(ref shFileOpstruct);

                //Check file operation status
                if (shFileResult == 0 && !shFileOpstruct.fAnyOperationsAborted)
                {
                    //Show the removal status notification
                    await Notification_Send_Status("Minus", "Re/moved shortcut " + dataBindApp.Name);
                    Debug.WriteLine("Removed shortcut: " + dataBindApp.Name + " path: " + dataBindApp.ShortcutPath);

                    //Remove application from the list
                    await RemoveAppFromList(dataBindApp, false, false, true);
                }
                else if (shFileOpstruct.fAnyOperationsAborted)
                {
                    await Notification_Send_Status("Minus", "Remove shortcut aborted");
                    Debug.WriteLine("Remove shortcut aborted: " + dataBindApp.Name + " path: " + dataBindApp.ShortcutPath);
                }
                else
                {
                    await Notification_Send_Status("Minus", "Remove shortcut failed");
                    Debug.WriteLine("Remove shortcut failed: " + dataBindApp.Name + " path: " + dataBindApp.ShortcutPath);
                }

                //Select the previous index
                await ListboxFocusIndex(listboxSender, false, false, listboxSelectedIndex, vProcessCurrent.MainWindowHandle);
            }
            catch (Exception ex)
            {
                await Notification_Send_Status("Minus", "Failed removing");
                Debug.WriteLine("Failed removing shortcut: " + ex.Message);
            }
        }

        //Rename the shortcut file
        async Task RenameShortcutFile(DataBindApp dataBindApp)
        {
            try
            {
                await Notification_Send_Status("Rename", "Renaming shortcut");
                Debug.WriteLine("Renaming shortcut: " + dataBindApp.Name + " path: " + dataBindApp.ShortcutPath);

                //Show the text input popup
                string textInputString = await Popup_ShowHide_TextInput("Rename shortcut", dataBindApp.Name, "Rename the shortcut file", false);

                //Check if file name changed
                if (textInputString == dataBindApp.Name)
                {
                    await Notification_Send_Status("Rename", "File name not changed");
                    Debug.WriteLine("The file name did not change.");
                    return;
                }

                //Check the changed file name
                if (!string.IsNullOrWhiteSpace(textInputString))
                {
                    string shortcutDirectory = Path.GetDirectoryName(dataBindApp.ShortcutPath);
                    string fileExtension = Path.GetExtension(dataBindApp.ShortcutPath);
                    string newFilePath = Path.Combine(shortcutDirectory, textInputString + fileExtension);

                    bool fileRenamed = File_Move(dataBindApp.ShortcutPath, newFilePath, true);
                    if (fileRenamed)
                    {
                        dataBindApp.Name = textInputString;
                        dataBindApp.ShortcutPath = newFilePath;

                        await Notification_Send_Status("Rename", "Renamed shortcut");
                        Debug.WriteLine("Renamed shortcut file to: " + textInputString);
                    }
                    else
                    {
                        await Notification_Send_Status("Rename", "Failed renaming");
                        Debug.WriteLine("Failed renaming shortcut.");
                    }
                }
            }
            catch (Exception ex)
            {
                await Notification_Send_Status("Rename", "Failed renaming");
                Debug.WriteLine("Failed renaming shortcut: " + ex.Message);
            }
        }
    }
}