using ArnoldVinkStyles;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Load file details
        async Task FilePicker_LoadDetails()
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
                        await FilePicker_LoadDetails(dataBindFile);
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
        async Task FilePicker_LoadDetails(DataBindFile dataBindFile)
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
                        listImageBitmap = await FileToBitmapImage(new AVImageFile()
                        {
                            FilePaths = [imageSearchPng, imageSearchJpg, fileNameFull, fileNameNoExt, "_Rom"],
                            SearchPaths = vImageSourceFoldersEmulatorsCombined,
                            BackupPath = vImageBackupSource,
                            Width = vImageLoadSizeFilePicker,
                            UseThumbnail = true,
                            Dispatcher = this.Dispatcher
                        });
                    }
                    else
                    {
                        listImageBitmap = await FileToBitmapImage(new AVImageFile()
                        {
                            FilePaths = [dataBindFile.PathFile],
                            BackupPath = vImageBackupSource,
                            Width = vImageLoadSizeFilePicker,
                            UseThumbnail = true,
                            Dispatcher = this.Dispatcher
                        });
                    }

                    //Update databind file
                    if (listImageBitmap != null)
                    {
                        AVDispatcherInvoke.DispatcherInvoke(this.Dispatcher, delegate
                        {
                            dataBindFile.ImageBitmap = listImageBitmap;
                        });
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