using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using static ArnoldVinkStyles.AVFocus;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task List_FileRemove_Prompt(ListView listboxSender, int listboxSelectedIndex, DataBindApp dataBindApp)
        {
            try
            {
                //Read databind app information
                string fileName = dataBindApp.Name;
                string filePath = string.Empty;
                string fileCategory = string.Empty;
                if (dataBindApp.Category == AppCategory.Shortcut)
                {
                    fileCategory = "shortcut file";
                    filePath = dataBindApp.PathShortcut;
                }
                else if (dataBindApp.Category == AppCategory.Gallery)
                {
                    fileCategory = "media file";
                    filePath = dataBindApp.PathGallery;
                }

                //Confirm file remove prompt
                List<DataBindString> messageAnswers = new List<DataBindString>();
                DataBindString answerRecycle = new DataBindString();
                answerRecycle.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Assets/Default/Icons/Remove.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                answerRecycle.Name = "Move " + fileCategory + " to recycle bin*";
                messageAnswers.Add(answerRecycle);

                DataBindString answerPerma = new DataBindString();
                answerPerma.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Assets/Default/Icons/RemoveCross.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                answerPerma.Name = "Remove " + fileCategory + " permanently";
                messageAnswers.Add(answerPerma);

                bool useRecycleBin = true;
                string deleteString = "Do you want to remove: " + fileName + "?";
                DataBindString messageResult = await Popup_Show_MessageBox("Remove " + fileCategory, "* Files and folders on a network drive get permanently deleted.", deleteString, messageAnswers);
                if (messageResult != null)
                {
                    if (messageResult == answerPerma)
                    {
                        useRecycleBin = false;
                    }
                }
                else
                {
                    Debug.WriteLine("Cancelled " + fileCategory + " removal.");
                    return;
                }

                await Notification_Show_Status("Remove", "Removing " + fileCategory);
                Debug.WriteLine("Removing file or folder: " + fileName + " path: " + filePath);

                //Remove file or folder
                if (await FileRemove(fileName, filePath, fileCategory, useRecycleBin))
                {
                    //Remove item from list
                    await RemoveAppFromList(dataBindApp, false, false, true);

                    //Select previous index
                    await ListViewFocusIndex(listboxSender, false, listboxSelectedIndex);
                }
            }
            catch { }
        }
    }
}