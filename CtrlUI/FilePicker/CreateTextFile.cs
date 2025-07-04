using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVFocus;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task FilePicker_CreateTextFile()
        {
            try
            {
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
                        Notification_Show_Status("Font", "Text file already exists");
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
                    DataBindFile dataBindFileFile = new DataBindFile() { FileType = FileType.File, Extension = ".txt", Name = fileName, NameDetail = fileDetailed, DateCreated = dateCreated, DateModified = dateCreated, PathFile = newFilePath };

                    //Update file details in databindfile
                    FilePicker_LoadDetails(dataBindFileFile);

                    //Add the new listbox item
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFile, false, false);

                    //Focus on the listbox item
                    await ListBoxFocusIndex(lb_FilePicker, true, 0, vProcessCurrent.WindowHandleMain);

                    //Check if there are files or folders
                    FilePicker_CheckFilesAndFoldersCount();

                    Notification_Show_Status("Font", "Created new text file");
                    Debug.WriteLine("Created new text file in: " + newFilePath);
                }
            }
            catch (Exception ex)
            {
                Notification_Show_Status("Font", "Failed creating file");
                Debug.WriteLine("Failed creating new text file: " + ex.Message);
            }
        }
    }
}