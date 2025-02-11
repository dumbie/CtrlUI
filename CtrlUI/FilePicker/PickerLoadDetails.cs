using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Load image and descriptions
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

        //Load image and description
        void FilePicker_LoadDetails(DataBindFile dataBindFile)
        {
            try
            {
                //Get all rom images
                FileInfo[] directoryRomImages = new FileInfo[] { };
                if (vFilePickerSettings.ShowEmulatorInterface && (dataBindFile.FileType == FileType.File || dataBindFile.FileType == FileType.Link || dataBindFile.FileType == FileType.Folder))
                {
                    //Get all root top files
                    DirectoryInfo directoryInfo = new DirectoryInfo(dataBindFile.PathRoot);
                    FileInfo[] directoryFiles = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly);

                    //Get all emulator files
                    DirectoryInfo directoryInfoRomsUser = new DirectoryInfo("Assets/User/Emulators");
                    FileInfo[] directoryPathsRomsUser = directoryInfoRomsUser.GetFiles("*", SearchOption.AllDirectories);
                    DirectoryInfo directoryInfoRomsDefault = new DirectoryInfo("Assets/Default/Emulators");
                    FileInfo[] directoryPathsRomsDefault = directoryInfoRomsDefault.GetFiles("*", SearchOption.AllDirectories);
                    IEnumerable<FileInfo> directoryPathsRoms = directoryPathsRomsUser.Concat(directoryPathsRomsDefault);

                    //Set image filter
                    string[] imageFilter = { "jpg", "png" };
                    FileInfo[] romsImages = directoryPathsRoms.Where(file => imageFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                    FileInfo[] filesImages = directoryFiles.Where(file => imageFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                    directoryRomImages = filesImages.Concat(romsImages).OrderByDescending(x => x.Name.Length).ToArray();
                }

                //Check file type
                BitmapImage listImageBitmap = null;
                if (dataBindFile.FileType == FileType.File || dataBindFile.FileType == FileType.Link)
                {
                    if (vFilePickerSettings.ShowEmulatorInterface)
                    {
                        listImageBitmap = FilePicker_GetRomBitmapImage(dataBindFile.Name, dataBindFile.PathRoot, directoryRomImages);
                    }
                    else
                    {
                        listImageBitmap = FileCacheToBitmapImage(dataBindFile.PathFile, vImageBackupSource, 50, 0, false);
                    }
                }
                else if (dataBindFile.FileType == FileType.Folder)
                {
                    if (vFilePickerSettings.ShowEmulatorInterface)
                    {
                        listImageBitmap = FilePicker_GetRomBitmapImage(dataBindFile.Name, dataBindFile.PathFile, directoryRomImages);
                    }
                    else
                    {
                        listImageBitmap = FileCacheToBitmapImage(dataBindFile.PathFile, vImageBackupSource, 50, 0, false);
                    }
                }

                //Update databind file
                if (listImageBitmap != null)
                {
                    dataBindFile.ImageBitmap = listImageBitmap;
                }

                //Debug.WriteLine("Updated file databind details: " + dataBindFile.Name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to update file databind details: " + ex.Message);
            }
        }

        //Get rom bitmap image
        BitmapImage FilePicker_GetRomBitmapImage(string listName, string listPath, FileInfo[] directoryRomImages)
        {
            try
            {
                //Set rom file names
                string romPathImage = string.Empty;
                string romPathDescription = string.Empty;
                string romNameFiltered = string.Empty;

                //Set sub directory image paths
                string subPathImagePng = string.Empty;
                string subPathImageJpg = string.Empty;
                if (!string.IsNullOrWhiteSpace(listPath))
                {
                    romNameFiltered = FilterNameGame(listName, false, true, 0);
                    subPathImagePng = Path.Combine(listPath, listName + ".png");
                    subPathImageJpg = Path.Combine(listPath, listName + ".jpg");
                }
                else
                {
                    romNameFiltered = FilterNameGame(listName, true, true, 0);
                }

                //Check if rom directory has image
                foreach (FileInfo foundImage in directoryRomImages)
                {
                    try
                    {
                        string imageNameFiltered = FilterNameGame(foundImage.Name, true, true, 0);
                        //Debug.WriteLine(imageNameFiltered + " / " + romNameFiltered);
                        if (romNameFiltered.Contains(imageNameFiltered))
                        {
                            romPathImage = foundImage.FullName;
                            break;
                        }
                    }
                    catch { }
                }

                //Return image
                return FileToBitmapImage(new string[] { romPathImage, subPathImagePng, subPathImageJpg, "_Rom" }, vImageSourceFoldersEmulatorsCombined, vImageBackupSource, 210, 0, IntPtr.Zero, 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load rom bitmap image: " + ex.Message);
                return null;
            }
        }
    }
}