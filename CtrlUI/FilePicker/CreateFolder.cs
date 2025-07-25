﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using static ArnoldVinkStyles.AVFocus;
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
                        Notification_Show_Status("FolderAdd", "Folder already exists");
                        Debug.WriteLine("Create folder already exists.");
                        return;
                    }

                    //Create the new folder
                    DirectoryInfo listDirectory = Directory.CreateDirectory(newFolderPath);

                    //Create new folder databindfile
                    DataBindFile dataBindFileFolder = new DataBindFile() { FileType = FileType.Folder, Name = listDirectory.Name, DateCreated = listDirectory.CreationTime, DateModified = listDirectory.LastWriteTime, PathFile = listDirectory.FullName };

                    //Update folder details in databindfile
                    FilePicker_LoadDetails(dataBindFileFolder);

                    //Add the new listbox item
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFolder, false, false);

                    //Focus on the listbox item
                    await ListBoxFocusIndex(lb_FilePicker, true, 0, vProcessCurrent.WindowHandleMain);

                    //Check if there are files or folders
                    FilePicker_CheckFilesAndFoldersCount();

                    Notification_Show_Status("FolderAdd", "Created new folder");
                    Debug.WriteLine("Created new folder in: " + newFolderPath);
                }
            }
            catch (Exception ex)
            {
                Notification_Show_Status("FolderAdd", "Failed creating folder");
                Debug.WriteLine("Failed creating new folder: " + ex.Message);
            }
        }
    }
}