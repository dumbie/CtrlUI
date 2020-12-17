using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task FilePicker_FileCut_Clipboard(DataBindFile dataBindFile)
        {
            try
            {
                //Check the file or folder
                if (dataBindFile.FileType == FileType.FolderPre || dataBindFile.FileType == FileType.FilePre || dataBindFile.FileType == FileType.GoUp)
                {
                    await Notification_Send_Status("Close", "Invalid file or folder");
                    Debug.WriteLine("Invalid file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                    return;
                }

                //Set the clipboard variables
                dataBindFile.ClipboardType = ClipboardType.Cut;
                vClipboardFiles.Add(dataBindFile);
            }
            catch { }
        }

        async Task FilePicker_FileCut_Single(DataBindFile dataBindFile)
        {
            try
            {
                await Notification_Send_Status("Cut", "Cutting file or folder");
                Debug.WriteLine("Clipboard cut file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);

                //Reset and clear the clipboard
                Clipboard_ResetClear();

                //Add file to the clipboard
                await FilePicker_FileCut_Clipboard(dataBindFile);

                //Update the clipboard status text
                Clipboard_UpdateStatusText();
            }
            catch { }
        }

        async Task FilePicker_FileCut_Checked()
        {
            try
            {
                await Notification_Send_Status("Cut", "Cutting files and folders");
                Debug.WriteLine("Clipboard cut checked files and folders.");

                //Reset and clear the clipboard
                Clipboard_ResetClear();

                //Add file to the clipboard
                foreach (DataBindFile dataBindFile in List_FilePicker.Where(x => x.Checked == Visibility.Visible))
                {
                    await FilePicker_FileCut_Clipboard(dataBindFile);
                }

                //Update the clipboard status text
                Clipboard_UpdateStatusText();
            }
            catch { }
        }
    }
}