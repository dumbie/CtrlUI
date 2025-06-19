using System.Diagnostics;
using System.Linq;
using System.Windows;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        void FilePicker_FileCut_Clipboard(DataBindFile dataBindFile)
        {
            try
            {
                //Check the file or folder
                if (dataBindFile.FileType == FileType.FolderPre || dataBindFile.FileType == FileType.FilePre || dataBindFile.FileType == FileType.GoUpPre)
                {
                    Notification_Show_Status("Close", "Invalid file or folder");
                    Debug.WriteLine("Invalid file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                    return;
                }

                //Set the clipboard variables
                dataBindFile.ClipboardType = ClipboardType.Cut;
                vClipboardFiles.Add(dataBindFile);
            }
            catch { }
        }

        void FilePicker_FileCut_Single(DataBindFile dataBindFile)
        {
            try
            {
                Notification_Show_Status("Cut", "Cutting file or folder");
                Debug.WriteLine("Clipboard cut file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);

                //Reset and clear the clipboard
                Clipboard_ResetClear();

                //Add file to the clipboard
                FilePicker_FileCut_Clipboard(dataBindFile);

                //Update the clipboard status text
                Clipboard_UpdateStatusText();
            }
            catch { }
        }

        void FilePicker_FileCut_Checked()
        {
            try
            {
                Notification_Show_Status("Cut", "Cutting files and folders");
                Debug.WriteLine("Clipboard cut checked files and folders.");

                //Reset and clear the clipboard
                Clipboard_ResetClear();

                //Add file to the clipboard
                foreach (DataBindFile dataBindFile in List_FilePicker.Where(x => x.Checked == Visibility.Visible))
                {
                    FilePicker_FileCut_Clipboard(dataBindFile);
                }

                //Update the clipboard status text
                Clipboard_UpdateStatusText();
            }
            catch { }
        }
    }
}