using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVFocus;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task FilePicker_CreateFolder()
        {
            try
            {
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
                    BitmapImage folderImage = FileToBitmapImage(new string[] { "Assets/Default/Icons/Folder.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    DataBindFile dataBindFileFolder = new DataBindFile() { FileType = FileType.Folder, Name = listDirectory.Name, DateModified = listDirectory.LastWriteTime, ImageBitmap = folderImage, PathFile = listDirectory.FullName };

                    //Add the new listbox item
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFolder, false, false);

                    //Focus on the listbox item
                    await ListBoxFocusIndex(lb_FilePicker, true, 0, vProcessCurrent.WindowHandleMain);

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
    }
}