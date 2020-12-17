using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVImage;
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
    }
}