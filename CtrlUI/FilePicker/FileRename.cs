using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVFiles;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task FilePicker_FileRename(DataBindFile dataBindFile)
        {
            try
            {
                //Check the file or folder
                if (dataBindFile.FileType == FileType.FolderPre || dataBindFile.FileType == FileType.FilePre || dataBindFile.FileType == FileType.GoUpPre)
                {
                    await Notification_Send_Status("Close", "Invalid rename file or folder");
                    Debug.WriteLine("Invalid file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                    return;
                }

                Debug.WriteLine("Renaming file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);

                //Show the text input popup
                string textInputString = await Popup_ShowHide_TextInput("Rename file or folder", dataBindFile.Name, "Rename the file or folder", false);

                //Check if file name changed
                if (textInputString == dataBindFile.Name)
                {
                    await Notification_Send_Status("Rename", "File name not changed");
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

                    //Update image and description
                    await FilePicker_LoadDetails(dataBindFile);

                    //Update clipboard status text
                    Clipboard_UpdateStatusText();

                    await Notification_Send_Status("Rename", "Renamed file or folder");
                    Debug.WriteLine("Renamed file or folder to: " + newFileName + newFileExtension);
                }
            }
            catch (Exception ex)
            {
                await Notification_Send_Status("Rename", "Failed renaming");
                Debug.WriteLine("Failed renaming file or folder: " + ex.Message);
            }
        }
    }
}