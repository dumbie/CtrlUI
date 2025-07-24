using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Load file details
        void FilePicker_LoadDetails()
        {
            try
            {
                foreach (DataBindFile dataBindFile in List_FilePicker)
                {
                    try
                    {
                        //Cancel loading
                        if (vFilePickerLoadCancel)
                        {
                            Debug.WriteLine("File picker details load cancelled.");
                            return;
                        }

                        //Update image and description
                        FilePicker_LoadDetails(dataBindFile);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to update file list details: " + ex.Message);
            }
        }

        //Load file details
        void FilePicker_LoadDetails(DataBindFile dataBindFile)
        {
            try
            {
                //Check file type
                BitmapImage listImageBitmap = null;
                if (dataBindFile.FileType == FileType.File || dataBindFile.FileType == FileType.Folder)
                {
                    //Get image file
                    if (vFilePickerSettings.ShowEmulatorInterface)
                    {
                        string fileNameFull = dataBindFile.Name;
                        string fileNameNoExt = Path.GetFileNameWithoutExtension(dataBindFile.Name);
                        string imageSearchPng = GetAssetsImageFilePath(dataBindFile, ".png", false);
                        string imageSearchJpg = GetAssetsImageFilePath(dataBindFile, ".jpg", false);
                        listImageBitmap = FileToBitmapImage([imageSearchPng, imageSearchJpg, fileNameFull, fileNameNoExt, "_Rom"], vImageSourceFoldersEmulatorsCombined, vImageBackupSource, 210, 0, IntPtr.Zero, 0);
                    }
                    else
                    {
                        listImageBitmap = FileCacheToBitmapImage(dataBindFile.PathFile, vImageBackupSource, 50, 0, false);
                    }

                    //Update databind file
                    if (listImageBitmap != null)
                    {
                        dataBindFile.ImageBitmap = listImageBitmap;
                    }
                }

                //Debug.WriteLine("Updated file databind details: " + dataBindFile.Name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to update file databind details: " + ex.Message);
            }
        }
    }
}