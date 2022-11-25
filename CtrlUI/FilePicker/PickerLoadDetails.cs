using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Load image and descriptions
        async Task FilePicker_LoadDetails()
        {
            try
            {
                foreach (DataBindFile dataBindFile in List_FilePicker)
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to update file list details: " + ex.Message);
            }
        }

        //Load image and description
        async Task FilePicker_LoadDetails(DataBindFile dataBindFile)
        {
            try
            {
                //Fix preload before loop
                //Get all rom images and descriptions
                FileInfo[] directoryRomImages = new FileInfo[] { };
                FileInfo[] directoryRomDescriptions = new FileInfo[] { };
                if (vFilePickerSettings.ShowEmulatorInterface && (dataBindFile.FileType == FileType.File || dataBindFile.FileType == FileType.Link || dataBindFile.FileType == FileType.Folder))
                {
                    string[] imageFilter = { "jpg", "png" };
                    string[] descriptionFilter = { "json" };

                    //Get all root top files
                    DirectoryInfo directoryInfo = new DirectoryInfo(dataBindFile.PathRoot);
                    FileInfo[] directoryFiles = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly);

                    //Get all image and descriptions
                    DirectoryInfo directoryInfoRomsUser = new DirectoryInfo("Assets/User/Emulators");
                    FileInfo[] directoryPathsRomsUser = directoryInfoRomsUser.GetFiles("*", SearchOption.AllDirectories);
                    DirectoryInfo directoryInfoRomsDefault = new DirectoryInfo("Assets/Default/Emulators");
                    FileInfo[] directoryPathsRomsDefault = directoryInfoRomsDefault.GetFiles("*", SearchOption.AllDirectories);
                    IEnumerable<FileInfo> directoryPathsRoms = directoryPathsRomsUser.Concat(directoryPathsRomsDefault);

                    FileInfo[] romsImages = directoryPathsRoms.Where(file => imageFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                    FileInfo[] filesImages = directoryFiles.Where(file => imageFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                    directoryRomImages = filesImages.Concat(romsImages).OrderByDescending(x => x.Name.Length).ToArray();

                    FileInfo[] romsDescriptions = directoryPathsRoms.Where(file => descriptionFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                    FileInfo[] filesDescriptions = directoryFiles.Where(file => descriptionFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                    directoryRomDescriptions = filesDescriptions.Concat(romsDescriptions).OrderByDescending(x => x.Name.Length).ToArray();
                }

                BitmapImage listImageBitmap = null;
                string listDescription = string.Empty;

                //Check the file type
                if (dataBindFile.FileType == FileType.File || dataBindFile.FileType == FileType.Link)
                {
                    if (vFilePickerSettings.ShowEmulatorInterface)
                    {
                        FilePicker_GetRomDetails(dataBindFile.Name, dataBindFile.PathRoot, directoryRomImages, directoryRomDescriptions, ref listImageBitmap, ref listDescription);
                    }
                    else
                    {
                        string listFileFullNameLower = dataBindFile.PathFile.ToLower();
                        string listFileExtensionLower = Path.GetExtension(dataBindFile.PathFile).ToLower().Replace(".", string.Empty);
                        if (listFileFullNameLower.EndsWith(".jpg") || listFileFullNameLower.EndsWith(".png") || listFileFullNameLower.EndsWith(".gif"))
                        {
                            listImageBitmap = FileToBitmapImage(new string[] { dataBindFile.PathFile }, null, vImageBackupSource, IntPtr.Zero, 50, 0);
                        }
                        else
                        {
                            listImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Extensions/" + listFileExtensionLower + ".png", "Assets/Default/Icons/File.png" }, null, vImageBackupSource, IntPtr.Zero, 50, 0);
                        }
                    }
                }
                else if (dataBindFile.FileType == FileType.Folder)
                {
                    if (vFilePickerSettings.ShowEmulatorInterface)
                    {
                        FilePicker_GetRomDetails(dataBindFile.Name, dataBindFile.PathFile, directoryRomImages, directoryRomDescriptions, ref listImageBitmap, ref listDescription);
                    }
                    else
                    {
                        listImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Folder.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    }
                }
                else if (dataBindFile.FileType == FileType.PlatformDesc)
                {
                    DownloadInfoPlatform informationDownloaded = await DownloadInfoPlatform(vFilePickerSettings.SourceDataBindApp.Name, 0, false, true);
                    if (informationDownloaded != null)
                    {
                        listDescription = informationDownloaded.Summary;
                    }
                }

                //Update databind file
                if (listImageBitmap != null)
                {
                    dataBindFile.ImageBitmap = listImageBitmap;
                }
                if (!string.IsNullOrWhiteSpace(listDescription))
                {
                    dataBindFile.Description = listDescription;
                }

                //Debug.WriteLine("Updated file databind details: " + dataBindFile.Name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to update file databind details: " + ex.Message);
            }
        }

        //Get rom details image and description
        void FilePicker_GetRomDetails(string listName, string listPath, FileInfo[] directoryRomImages, FileInfo[] directoryRomDescription, ref BitmapImage listImage, ref string listDescription)
        {
            try
            {
                //Set rom file names
                string romPathImage = string.Empty;
                string romPathDescription = string.Empty;
                string romNameFiltered = string.Empty;

                //Create sub directory image paths
                string subPathImagePng = string.Empty;
                string subPathImageJpg = string.Empty;
                string subPathDescription = string.Empty;
                if (!string.IsNullOrWhiteSpace(listPath))
                {
                    romNameFiltered = FilterNameGame(listName, false, true, false, 0);
                    subPathImagePng = Path.Combine(listPath, listName + ".png");
                    subPathImageJpg = Path.Combine(listPath, listName + ".jpg");
                    subPathDescription = Path.Combine(listPath, listName + ".json");
                }
                else
                {
                    romNameFiltered = FilterNameGame(listName, true, true, false, 0);
                }

                //Check if rom directory has image
                foreach (FileInfo foundImage in directoryRomImages)
                {
                    try
                    {
                        string imageNameFiltered = FilterNameGame(foundImage.Name, true, true, false, 0);
                        //Debug.WriteLine(imageNameFiltered + " / " + romNameFiltered);
                        if (romNameFiltered.Contains(imageNameFiltered))
                        {
                            romPathImage = foundImage.FullName;
                            break;
                        }
                    }
                    catch { }
                }

                //Check if rom directory has description
                foreach (FileInfo foundDesc in directoryRomDescription)
                {
                    try
                    {
                        string descNameFiltered = FilterNameGame(foundDesc.Name, true, true, false, 0);
                        //Debug.WriteLine(descNameFiltered + " / " + romNameFiltered);
                        if (romNameFiltered.Contains(descNameFiltered))
                        {
                            romPathDescription = foundDesc.FullName;
                            break;
                        }
                    }
                    catch { }
                }

                //Update description and image
                listImage = FileToBitmapImage(new string[] { romPathImage, subPathImagePng, subPathImageJpg, "_Rom" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 210, 0);
                string jsonFile = FileToString(new string[] { romPathDescription, subPathDescription });
                if (jsonFile.Contains("platform_logo"))
                {
                    ApiIGDBPlatformVersions platformVersionsJson = JsonConvert.DeserializeObject<ApiIGDBPlatformVersions>(jsonFile);
                    listDescription = ApiIGDB_PlatformSummaryString(platformVersionsJson);
                }
                else
                {
                    ApiIGDBGames gamesJson = JsonConvert.DeserializeObject<ApiIGDBGames>(jsonFile);
                    listDescription = ApiIGDB_GameSummaryString(gamesJson);
                }
            }
            catch { }
        }
    }
}