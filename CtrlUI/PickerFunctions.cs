using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show the File Picker Popup Task
        async Task Popup_Show_FilePicker(string targetPath, int targetIndex, bool storeIndex, FrameworkElement previousFocus)
        {
            try
            {
                async Task TaskAction()
                {
                    try
                    {
                        await Popup_Show_FilePicker_Task(targetPath, targetIndex, storeIndex, previousFocus);
                    }
                    catch { }
                }
                await AVActions.TaskStartAsync(TaskAction, null);
            }
            catch { }
        }

        //Show the File Picker Popup
        async Task Popup_Show_FilePicker_Task(string targetPath, int targetIndex, bool storeIndex, FrameworkElement previousFocus)
        {
            try
            {
                //Check if the popup is already open
                if (!vFilePickerOpen)
                {
                    //Play the popup opening sound
                    PlayInterfaceSound("PopupOpen", false);

                    //Save the previous focus element
                    Popup_PreviousElementFocus_Save(vFilePickerElementFocus, previousFocus);
                }

                //Reset file picker variables
                vFilePickerCompleted = false;
                vFilePickerCancelled = false;
                vFilePickerResult = null;
                vFilePickerOpen = true;

                //Disable the file picker list
                AVActions.ElementSetValue(lb_FilePicker, IsEnabledProperty, false);

                //Set file picker header texts
                AVActions.ElementSetValue(grid_Popup_FilePicker_txt_Title, TextBlock.TextProperty, vFilePickerTitle);
                AVActions.ElementSetValue(grid_Popup_FilePicker_txt_Description, TextBlock.TextProperty, vFilePickerDescription);

                //Change the list picker item style
                if (vFilePickerShowRoms)
                {
                    AVActions.ElementSetValue(lb_FilePicker, StyleProperty, Application.Current.Resources["ListBoxWrapPanel"] as Style);
                    AVActions.ElementSetValue(lb_FilePicker, ListBox.ItemTemplateProperty, Application.Current.Resources["ListBoxItemRom"] as DataTemplate);
                    AVActions.ElementSetValue(grid_Popup_Filepicker_Row1, HorizontalAlignmentProperty, HorizontalAlignment.Center);
                }
                else
                {
                    AVActions.ElementSetValue(lb_FilePicker, StyleProperty, Application.Current.Resources["ListBoxVertical"] as Style);
                    AVActions.ElementSetValue(lb_FilePicker, ListBox.ItemTemplateProperty, Application.Current.Resources["ListBoxItemString"] as DataTemplate);
                    AVActions.ElementSetValue(grid_Popup_Filepicker_Row1, HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
                }

                //Show the popup
                Popup_Show_Element(grid_Popup_FilePicker);

                //Get and update the current index and path
                vFilePickerCurrentPath = targetPath;
                if (storeIndex)
                {
                    int selectedIndex = (int)AVActions.ElementGetValue(lb_FilePicker, ListBox.SelectedIndexProperty);
                    Debug.WriteLine("Adding navigation history index: " + selectedIndex);
                    vFilePickerNavigateIndexes.Add(selectedIndex);
                }

                //Clear the current file picker list
                AVActions.ActionDispatcherInvoke(delegate
                {
                    List_FilePicker.Clear();
                });

                //Get and list all the disk drives
                if (targetPath == "PC")
                {
                    //Enable or disable selection button in the list
                    AVActions.ElementSetValue(grid_Popup_FilePicker_button_SelectFolder, VisibilityProperty, Visibility.Collapsed);

                    //Enable or disable file and folder availability
                    AVActions.ElementSetValue(grid_Popup_FilePicker_textblock_NoFilesAvailable, VisibilityProperty, Visibility.Collapsed);

                    //Enable or disable the side navigate buttons
                    AVActions.ElementSetValue(grid_Popup_FilePicker_button_ControllerLeft, VisibilityProperty, Visibility.Collapsed);
                    AVActions.ElementSetValue(grid_Popup_FilePicker_button_ControllerUp, VisibilityProperty, Visibility.Collapsed);

                    BitmapImage imageFolder = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Folder.png" }, IntPtr.Zero, -1, 0);

                    //Add my documents and pictures folder
                    DataBindFile dataBindFilePictures = new DataBindFile() { Type = "PreDirectory", Name = "My Pictures", ImageBitmap = imageFolder, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFilePictures, false, false);
                    DataBindFile dataBindFileDocuments = new DataBindFile() { Type = "PreDirectory", Name = "My Documents", ImageBitmap = imageFolder, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileDocuments, false, false);

                    //Check and add the previous path
                    if (!string.IsNullOrWhiteSpace(vFilePickerPreviousPath))
                    {
                        DataBindFile dataBindFilePreviousPath = new DataBindFile() { Type = "PreDirectory", Name = "Previous Path", NameSub = "(" + vFilePickerPreviousPath + ")", ImageBitmap = imageFolder, PathFile = vFilePickerPreviousPath };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFilePreviousPath, false, false);
                    }

                    //Add launch without a file option
                    if (vFilePickerShowNoFile)
                    {
                        string fileDescription = "Launch application without a file";
                        BitmapImage fileImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1, 0);
                        DataBindFile dataBindFileWithoutFile = new DataBindFile() { Type = "File", Name = fileDescription, Description = fileDescription + ".", ImageBitmap = fileImage, PathFile = string.Empty };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileWithoutFile, false, false);
                    }

                    //Add all disk drives that are connected
                    DriveInfo[] diskDrives = DriveInfo.GetDrives();
                    foreach (DriveInfo disk in diskDrives)
                    {
                        try
                        {
                            //Skip network drive depending on the setting
                            if (disk.DriveType == DriveType.Network && Convert.ToBoolean(ConfigurationManager.AppSettings["HideNetworkDrives"]))
                            {
                                continue;
                            }

                            //Check if the disk is currently connected
                            if (disk.IsReady)
                            {
                                //Get the current disk size
                                string freeSpace = AVFunctions.ConvertBytesSizeToString(disk.TotalFreeSpace);
                                string usedSpace = AVFunctions.ConvertBytesSizeToString(disk.TotalSize);
                                string diskSpace = freeSpace + "/" + usedSpace;

                                DataBindFile dataBindFileDisk = new DataBindFile() { Type = "Directory", Name = disk.Name, NameSub = disk.VolumeLabel, NameDetail = diskSpace, ImageBitmap = imageFolder, PathFile = disk.Name };
                                await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileDisk, false, false);
                            }
                        }
                        catch { }
                    }

                    //Add Json file locations
                    foreach (ProfileShared Locations in vCtrlLocationsFile)
                    {
                        try
                        {
                            if (Directory.Exists(Locations.String2))
                            {
                                DataBindFile dataBindFileLocation = new DataBindFile() { Type = "Directory", Name = Locations.String2, NameSub = Locations.String1, ImageBitmap = imageFolder, PathFile = Locations.String2 };
                                await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileLocation, false, false);
                            }
                        }
                        catch { }
                    }
                }
                //Get and list all the UWP applications
                else if (targetPath == "UWP")
                {
                    //Enable or disable selection button in the list
                    AVActions.ElementSetValue(grid_Popup_FilePicker_button_SelectFolder, VisibilityProperty, Visibility.Collapsed);

                    //Enable or disable file and folder availability
                    AVActions.ElementSetValue(grid_Popup_FilePicker_textblock_NoFilesAvailable, VisibilityProperty, Visibility.Collapsed);

                    //Enable or disable the side navigate buttons
                    AVActions.ElementSetValue(grid_Popup_FilePicker_button_ControllerLeft, VisibilityProperty, Visibility.Visible);
                    AVActions.ElementSetValue(grid_Popup_FilePicker_button_ControllerUp, VisibilityProperty, Visibility.Collapsed);

                    //Add uwp applications to the filepicker list
                    await ListLoadAllUwpApplications(lb_FilePicker, List_FilePicker);
                }
                else
                {
                    //Clean the target path string
                    targetPath = Path.GetFullPath(targetPath);

                    //Add the Go up directory to the list
                    if (Path.GetPathRoot(targetPath) != targetPath)
                    {
                        BitmapImage imageBack = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Up.png" }, IntPtr.Zero, -1, 0);
                        DataBindFile dataBindFileGoUp = new DataBindFile() { Type = "GoUp", Name = "Go up", NameSub = "(" + targetPath + ")", Description = "Go up to the previous folder.", ImageBitmap = imageBack, PathFile = Path.GetDirectoryName(targetPath) };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileGoUp, false, false);
                    }
                    else
                    {
                        BitmapImage imageBack = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Up.png" }, IntPtr.Zero, -1, 0);
                        DataBindFile dataBindFileGoUp = new DataBindFile() { Type = "GoUp", Name = "Go up", NameSub = "(" + targetPath + ")", Description = "Go up to the previous folder.", ImageBitmap = imageBack, PathFile = "PC" };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileGoUp, false, false);
                    }

                    //Add launch emulator options
                    if (vFilePickerShowRoms)
                    {
                        string fileDescription = "Launch the emulator without a rom loaded";
                        BitmapImage fileImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Emulator.png" }, IntPtr.Zero, -1, 0);
                        DataBindFile dataBindFileWithoutRom = new DataBindFile() { Type = "File", Name = fileDescription, Description = fileDescription + ".", ImageBitmap = fileImage, PathFile = string.Empty };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileWithoutRom, false, false);

                        string romDescription = "Launch the emulator with this folder as rom";
                        BitmapImage romImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Emulator.png" }, IntPtr.Zero, -1, 0);
                        DataBindFile dataBindFileFolderRom = new DataBindFile() { Type = "File", Name = romDescription, Description = romDescription + ".", ImageBitmap = romImage, PathFile = targetPath };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFolderRom, false, false);
                    }

                    //Enable or disable the side navigate buttons
                    AVActions.ElementSetValue(grid_Popup_FilePicker_button_ControllerLeft, VisibilityProperty, Visibility.Visible);
                    AVActions.ElementSetValue(grid_Popup_FilePicker_button_ControllerUp, VisibilityProperty, Visibility.Visible);

                    //Get all the top files and folders
                    DirectoryInfo directoryInfo = new DirectoryInfo(targetPath);
                    DirectoryInfo[] directoryFolders = null;
                    FileInfo[] directoryFiles = null;
                    if (vFilePickerSortByName)
                    {
                        directoryFolders = directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly).OrderBy(x => x.Name).ToArray();
                        directoryFiles = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly).OrderBy(x => x.Name).ToArray();
                    }
                    else
                    {
                        directoryFolders = directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly).OrderByDescending(x => x.LastWriteTime).ToArray();
                        directoryFiles = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly).OrderByDescending(x => x.LastWriteTime).ToArray();
                    }

                    //Get all rom images and descriptions
                    FileInfo[] directoryRomImages = new FileInfo[] { };
                    FileInfo[] directoryRomDescriptions = new FileInfo[] { };
                    if (vFilePickerShowRoms)
                    {
                        string[] imageFilter = new string[] { "jpg", "png" };
                        string[] descriptionFilter = new string[] { "txt" };

                        DirectoryInfo directoryInfoRoms = new DirectoryInfo("Assets\\Roms");
                        FileInfo[] directoryPathsRoms = directoryInfoRoms.GetFiles("*", SearchOption.AllDirectories);

                        FileInfo[] romsImages = directoryPathsRoms.Where(file => imageFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                        FileInfo[] filesImages = directoryFiles.Where(file => imageFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                        directoryRomImages = filesImages.Concat(romsImages).OrderByDescending(x => x.Name.Length).ToArray();

                        FileInfo[] romsDescriptions = directoryPathsRoms.Where(file => descriptionFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                        FileInfo[] filesDescriptions = directoryFiles.Where(file => descriptionFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                        directoryRomDescriptions = filesDescriptions.Concat(romsDescriptions).OrderByDescending(x => x.Name.Length).ToArray();
                    }

                    //Get all the directories from target directory
                    if (vFilePickerShowDirectories)
                    {
                        try
                        {
                            //Fill the file picker listbox with folders
                            foreach (DirectoryInfo listFolder in directoryFolders)
                            {
                                try
                                {
                                    BitmapImage listImage = null;
                                    string listDescription = string.Empty;

                                    //Load image files for the list
                                    if (vFilePickerShowRoms)
                                    {
                                        GetRomDetails(listFolder.Name, listFolder.FullName, directoryRomImages, directoryRomDescriptions, ref listImage, ref listDescription);
                                    }
                                    else
                                    {
                                        listImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Folder.png" }, IntPtr.Zero, -1, 0);
                                    }

                                    //Get the folder size
                                    //string folderSize = AVFunctions.ConvertBytesSizeToString(GetDirectorySize(listDirectory));

                                    //Get the folder date
                                    string folderDate = listFolder.LastWriteTime.ToShortDateString().Replace("-", "/");

                                    //Set the detailed text
                                    string folderDetailed = folderDate;

                                    //Add folder to the list
                                    bool systemFileFolder = listFolder.Attributes.HasFlag(FileAttributes.System);
                                    bool hiddenFileFolder = listFolder.Attributes.HasFlag(FileAttributes.Hidden);
                                    if (!systemFileFolder && (!hiddenFileFolder || Convert.ToBoolean(ConfigurationManager.AppSettings["ShowHiddenFilesFolders"])))
                                    {
                                        DataBindFile dataBindFileFolder = new DataBindFile() { Type = "Directory", Name = listFolder.Name, NameDetail = folderDetailed, Description = listDescription, DateModified = listFolder.LastWriteTime, ImageBitmap = listImage, PathFile = listFolder.FullName };
                                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFolder, false, false);
                                    }
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }

                    //Get all the files from target directory
                    if (vFilePickerShowFiles)
                    {
                        try
                        {
                            //Enable or disable selection button in the list
                            AVActions.ElementSetValue(grid_Popup_FilePicker_button_SelectFolder, VisibilityProperty, Visibility.Collapsed);

                            //Filter files in and out
                            if (vFilePickerFilterIn.Any())
                            {
                                directoryFiles = directoryFiles.Where(file => vFilePickerFilterIn.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                            }
                            if (vFilePickerFilterOut.Any())
                            {
                                directoryFiles = directoryFiles.Where(file => !vFilePickerFilterOut.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                            }

                            //Fill the file picker listbox with files
                            foreach (FileInfo listFile in directoryFiles)
                            {
                                try
                                {
                                    BitmapImage listImage = null;
                                    string listDescription = string.Empty;

                                    //Load image files for the list
                                    if (vFilePickerShowRoms)
                                    {
                                        GetRomDetails(listFile.Name, string.Empty, directoryRomImages, directoryRomDescriptions, ref listImage, ref listDescription);
                                    }
                                    else
                                    {
                                        string listFileFullNameLower = listFile.FullName.ToLower();
                                        if (listFileFullNameLower.EndsWith(".jpg") || listFileFullNameLower.EndsWith(".png") || listFileFullNameLower.EndsWith(".gif"))
                                        {
                                            listImage = FileToBitmapImage(new string[] { listFile.FullName }, IntPtr.Zero, 50, 0);
                                        }
                                        else if (listFileFullNameLower.EndsWith(".exe"))
                                        {
                                            listImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1, 0);
                                        }
                                        else if (listFileFullNameLower.EndsWith(".bat"))
                                        {
                                            listImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/FileBat.png" }, IntPtr.Zero, -1, 0);
                                        }
                                        else
                                        {
                                            listImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/File.png" }, IntPtr.Zero, -1, 0);
                                        }
                                    }

                                    //Get the file size
                                    string fileSize = AVFunctions.ConvertBytesSizeToString(listFile.Length);

                                    //Get the file date
                                    string fileDate = listFile.LastWriteTime.ToShortDateString().Replace("-", "/");

                                    //Set the detailed text
                                    string fileDetailed = fileSize + " (" + fileDate + ")";

                                    //Add file to the list
                                    bool systemFileFolder = listFile.Attributes.HasFlag(FileAttributes.System);
                                    bool hiddenFileFolder = listFile.Attributes.HasFlag(FileAttributes.Hidden);
                                    if (!systemFileFolder && (!hiddenFileFolder || Convert.ToBoolean(ConfigurationManager.AppSettings["ShowHiddenFilesFolders"])))
                                    {
                                        DataBindFile dataBindFileFile = new DataBindFile() { Type = "File", Name = listFile.Name, NameDetail = fileDetailed, Description = listDescription, DateModified = listFile.LastWriteTime, ImageBitmap = listImage, PathFile = listFile.FullName };
                                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFile, false, false);
                                    }
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        //Enable or disable selection button in the list
                        AVActions.ElementSetValue(grid_Popup_FilePicker_button_SelectFolder, VisibilityProperty, Visibility.Visible);
                    }

                    //Check if there are files or folders
                    FilePicker_CheckFilesAndFoldersCount();
                }

                //Enable the file picker list
                AVActions.ElementSetValue(lb_FilePicker, IsEnabledProperty, true);

                //Focus on the file picker listbox
                await ListboxFocusIndex(lb_FilePicker, false, false, targetIndex);
            }
            catch (Exception ex)
            {
                Popup_Show_Status("Close", "Picker failed");
                Debug.WriteLine("Failed loading filepicker: " + ex.Message);
            }
        }

        //Get rom details image and description
        void GetRomDetails(string listName, string listPath, FileInfo[] directoryRomImages, FileInfo[] directoryRomDescription, ref BitmapImage listImage, ref string listDescription)
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
                    romNameFiltered = listName.Replace("'", string.Empty).Replace(".", string.Empty).Replace("-", string.Empty).Replace("_", string.Empty).Replace(" ", string.Empty).ToLower();
                    subPathImagePng = Path.Combine(listPath, listName + ".png");
                    subPathImageJpg = Path.Combine(listPath, listName + ".jpg");
                    subPathDescription = Path.Combine(listPath, listName + ".txt");
                }
                else
                {
                    romNameFiltered = Path.GetFileNameWithoutExtension(listName).Replace("'", string.Empty).Replace(".", string.Empty).Replace("-", string.Empty).Replace("_", string.Empty).Replace(" ", string.Empty).ToLower();
                }

                //Check if rom directory has image
                foreach (FileInfo foundImage in directoryRomImages)
                {
                    try
                    {
                        string imageNameFiltered = Path.GetFileNameWithoutExtension(foundImage.Name).Replace("'", string.Empty).Replace(".", string.Empty).Replace("-", string.Empty).Replace("_", string.Empty).Replace(" ", string.Empty).ToLower();
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
                        string descNameFiltered = Path.GetFileNameWithoutExtension(foundDesc.Name).Replace("'", string.Empty).Replace(".", string.Empty).Replace("-", string.Empty).Replace("_", string.Empty).Replace(" ", string.Empty).ToLower();
                        if (romNameFiltered.Contains(descNameFiltered))
                        {
                            romPathDescription = foundDesc.FullName;
                            break;
                        }
                    }
                    catch { }
                }

                //Update description and image
                listDescription = FileToString(new string[] { romPathDescription, subPathDescription });
                if (string.IsNullOrWhiteSpace(listDescription)) { listDescription = "There is no description available."; }
                listImage = FileToBitmapImage(new string[] { romPathImage, subPathImagePng, subPathImageJpg, "Rom" }, IntPtr.Zero, 210, 0);
            }
            catch { }
        }

        //Check if there are files or folders
        void FilePicker_CheckFilesAndFoldersCount()
        {
            try
            {
                int totalFileCount = List_FilePicker.Count - 1; //Filter out GoUp
                if (totalFileCount > 0)
                {
                    //Enable or disable file and folder availability
                    AVActions.ElementSetValue(grid_Popup_FilePicker_textblock_NoFilesAvailable, VisibilityProperty, Visibility.Collapsed);
                    Debug.WriteLine("There are files and folders in the list.");
                }
                else
                {
                    //Enable or disable file and folder availability
                    AVActions.ElementSetValue(grid_Popup_FilePicker_textblock_NoFilesAvailable, VisibilityProperty, Visibility.Visible);
                    Debug.WriteLine("None files and folders in the list.");
                }
            }
            catch { }
        }

        //Close file picker popup
        async Task Popup_Close_FilePicker(bool IsCompleted, bool CurrentFolder)
        {
            try
            {
                PlayInterfaceSound("PopupClose", false);

                //Reset and update popup variables
                vFilePickerOpen = false;
                if (IsCompleted)
                {
                    vFilePickerCompleted = true;
                    if (CurrentFolder)
                    {
                        DataBindFile targetPath = new DataBindFile();
                        targetPath.PathFile = vFilePickerCurrentPath;
                        vFilePickerResult = targetPath;
                    }
                    else
                    {
                        vFilePickerResult = (DataBindFile)lb_FilePicker.SelectedItem;
                    }
                }
                else
                {
                    vFilePickerCancelled = true;
                }

                //Store the current picker path
                if (vFilePickerCurrentPath.Contains(":"))
                {
                    Debug.WriteLine("Closed the picker on: " + vFilePickerCurrentPath);
                    vFilePickerPreviousPath = vFilePickerCurrentPath;
                }

                //Clear the current file picker list
                vFilePickerNavigateIndexes.Clear();
                List_FilePicker.Clear();

                //Hide the popup
                Popup_Hide_Element(grid_Popup_FilePicker);

                //Focus on the previous focus element
                await Popup_PreviousElementFocus_Focus(vFilePickerElementFocus);
            }
            catch { }
        }
    }
}