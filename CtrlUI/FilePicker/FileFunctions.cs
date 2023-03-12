using ArnoldVinkCode;
using System.Diagnostics;
using System.IO;
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
        //Show the file manager popup
        async Task ShowFileManager()
        {
            try
            {
                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.Title = "File Manager";
                vFilePickerSettings.Description = "Please select a file to run or interact with:";
                Popup_Show_FilePicker("PC", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Check keyboard controller launch
                string fileExtension = Path.GetExtension(vFilePickerResult.PathFile).Replace(".", string.Empty);
                string fileNameNoExtension = Path.GetFileNameWithoutExtension(vFilePickerResult.PathFile);
                bool keyboardProcess = vCtrlKeyboardProcessName.Any(x => x.String1.ToLower() == fileNameNoExtension.ToLower());
                bool keyboardExtension = vCtrlKeyboardExtensionName.Any(x => x.String1.ToLower() == fileExtension.ToLower());
                bool keyboardLaunch = (keyboardExtension || keyboardProcess) && vControllerAnyConnected();

                //Launch the Win32 application
                await PrepareProcessLauncherWin32Async(fileNameNoExtension, vFilePickerResult.PathFile, "", "", false, false, keyboardLaunch);
            }
            catch { }
        }

        //Reset and clear the clipboard
        void Clipboard_ResetClear()
        {
            try
            {
                foreach (DataBindFile dataBindFile in vClipboardFiles)
                {
                    dataBindFile.ClipboardType = ClipboardType.None;
                }
                vClipboardFiles.Clear();
            }
            catch
            {
                Debug.WriteLine("Failed to reset and clear clipboard.");
            }
        }

        //Update the clipboard status text
        void Clipboard_UpdateStatusText()
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    if (vClipboardFiles.Count == 1)
                    {
                        DataBindFile clipboardFile = vClipboardFiles.FirstOrDefault();
                        grid_Popup_FilePicker_textblock_ClipboardStatus.Text = "Clipboard (" + clipboardFile.FileType.ToString() + " " + clipboardFile.ClipboardType.ToString() + ") " + clipboardFile.PathFile;
                        grid_Popup_FilePicker_textblock_ClipboardStatus.Visibility = Visibility.Visible;
                    }
                    else if (vClipboardFiles.Count > 1)
                    {
                        int copyCount = vClipboardFiles.Count(x => x.ClipboardType == ClipboardType.Copy);
                        int cutCount = vClipboardFiles.Count(x => x.ClipboardType == ClipboardType.Cut);
                        string statusCount = string.Empty;
                        if (copyCount > cutCount)
                        {
                            statusCount = "(" + copyCount + "x copy)";
                        }
                        else
                        {
                            statusCount = "(" + cutCount + "x cut)";
                        }

                        grid_Popup_FilePicker_textblock_ClipboardStatus.Text = "Clipboard " + statusCount + " files or folders.";
                        grid_Popup_FilePicker_textblock_ClipboardStatus.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        grid_Popup_FilePicker_textblock_ClipboardStatus.Text = string.Empty;
                        grid_Popup_FilePicker_textblock_ClipboardStatus.Visibility = Visibility.Collapsed;
                    }
                });
            }
            catch { }
        }

        //Get the total directory size
        public static long GetDirectorySize(DirectoryInfo directoryInfo)
        {
            long directorySize = 0;
            try
            {
                FileInfo[] fileList = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
                foreach (FileInfo file in fileList)
                {
                    directorySize += file.Length;
                }
            }
            catch { }
            return directorySize;
        }
    }
}