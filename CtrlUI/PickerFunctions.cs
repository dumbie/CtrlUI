using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessClasses;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show the File Picker Popup
        async Task Popup_Show_FilePicker(string targetPath, int targetIndex, bool storeIndex, FrameworkElement previousFocus)
        {
            try
            {
                //Play the popup opening sound
                if (!vFilePickerOpen)
                {
                    PlayInterfaceSound(vInterfaceSoundVolume, "PopupOpen", false);
                }

                //Save previous focus element
                Popup_PreviousFocusSave(vFilePickerElementFocus, previousFocus);

                //Reset file picker variables
                vFilePickerCompleted = false;
                vFilePickerCancelled = false;
                vFilePickerResult = null;
                vFilePickerOpen = true;

                //Set file picker header texts
                grid_Popup_FilePicker_txt_Title.Text = vFilePickerTitle;
                grid_Popup_FilePicker_txt_Description.Text = vFilePickerDescription;

                //Change the list picker item style
                if (vFilePickerShowRoms)
                {
                    lb_FilePicker.Style = Application.Current.Resources["ListBoxWrapPanel"] as Style;
                    lb_FilePicker.ItemTemplate = Application.Current.Resources["ListBoxItemRom"] as DataTemplate;
                    grid_Popup_Filepicker_Row1.HorizontalAlignment = HorizontalAlignment.Center;
                }
                else
                {
                    lb_FilePicker.Style = Application.Current.Resources["ListBoxVertical"] as Style;
                    lb_FilePicker.ItemTemplate = Application.Current.Resources["ListBoxItemString"] as DataTemplate;
                    grid_Popup_Filepicker_Row1.HorizontalAlignment = HorizontalAlignment.Stretch;
                }

                //Show the popup with animation
                AVAnimations.Ani_Visibility(grid_Popup_FilePicker, true, true, 0.10);
                AVAnimations.Ani_Opacity(grid_Main, 0.08, true, false, 0.10);

                if (vTextInputOpen) { AVAnimations.Ani_Opacity(grid_Popup_TextInput, 0.02, true, false, 0.10); }
                if (vMessageBoxOpen) { AVAnimations.Ani_Opacity(grid_Popup_MessageBox, 0.02, true, false, 0.10); }
                //if (vFilePickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_FilePicker, 0.02, true, false, 0.10); }
                if (vPopupOpen) { AVAnimations.Ani_Opacity(vPopupElementTarget, 0.02, true, false, 0.10); }
                if (vColorPickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_ColorPicker, 0.02, true, false, 0.10); }
                if (vSearchOpen) { AVAnimations.Ani_Opacity(grid_Popup_Search, 0.02, true, false, 0.10); }
                if (vMainMenuOpen) { AVAnimations.Ani_Opacity(grid_Popup_MainMenu, 0.02, true, false, 0.10); }

                //Get and update the current index and path
                vFilePickerCurrentPath = targetPath;
                if (storeIndex)
                {
                    File_Picker_AddIndexHistory(lb_FilePicker.SelectedIndex);
                }

                //Clear the current file picker list
                List_FilePicker.Clear();
                GC.Collect();

                //Get and set all strings to list
                if (targetPath == "String")
                {
                    //Disable selection button in the list
                    grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Collapsed;

                    //Disable the side navigate buttons
                    grid_Popup_FilePicker_button_ControllerLeft.Visibility = Visibility.Collapsed;
                    grid_Popup_FilePicker_button_ControllerUp.Visibility = Visibility.Collapsed;

                    //Add all the strings from array
                    foreach (string[] stringPicker in vFilePickerStrings)
                    {
                        try
                        {
                            BitmapImage imageFile = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/" + stringPicker[1] + ".png" }, IntPtr.Zero, -1);
                            DataBindFile dataBindFile = new DataBindFile() { Type = "File", Name = stringPicker[0], PathFile = stringPicker[1], ImageBitmap = imageFile };
                            await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFile, false, false);
                        }
                        catch { }
                    }
                }
                //Get and list all the disk drives
                else if (targetPath == "PC")
                {
                    //Disable selection button in the list
                    grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Collapsed;

                    //Disable the side navigate buttons
                    grid_Popup_FilePicker_button_ControllerLeft.Visibility = Visibility.Collapsed;
                    grid_Popup_FilePicker_button_ControllerUp.Visibility = Visibility.Collapsed;

                    BitmapImage imageFolder = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Folder.png" }, IntPtr.Zero, -1);

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
                        BitmapImage fileImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1);
                        DataBindFile dataBindFileWithoutFile = new DataBindFile() { Type = "File", Name = fileDescription, Description = fileDescription + ".", ImageBitmap = fileImage, PathFile = string.Empty };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileWithoutFile, false, false);
                    }

                    //Add all disk drives that are connected
                    DriveInfo[] DiskDrives = DriveInfo.GetDrives();
                    foreach (DriveInfo Disk in DiskDrives)
                    {
                        try
                        {
                            //Skip network drive depending on the setting
                            if (Disk.DriveType == DriveType.Network && Convert.ToBoolean(ConfigurationManager.AppSettings["HideNetworkDrives"]))
                            {
                                continue;
                            }

                            //Check if the disk is currently connected
                            if (Disk.IsReady)
                            {
                                DataBindFile dataBindFileDisk = new DataBindFile() { Type = "Directory", Name = Disk.Name, NameSub = Disk.VolumeLabel, ImageBitmap = imageFolder, PathFile = Disk.Name };
                                await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileDisk, false, false);
                            }
                        }
                        catch { }
                    }

                    //Add Json file locations
                    foreach (FileLocation Locations in vFileLocations)
                    {
                        try
                        {
                            if (Directory.Exists(Locations.Path))
                            {
                                DataBindFile dataBindFileLocation = new DataBindFile() { Type = "Directory", Name = Locations.Path, NameSub = Locations.Name, ImageBitmap = imageFolder, PathFile = Locations.Path };
                                await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileLocation, false, false);
                            }
                        }
                        catch { }
                    }
                }
                //Get and list all the UWP applications
                else if (targetPath == "UWP")
                {
                    //Disable selection button in the list
                    grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Collapsed;

                    //Disable the side navigate buttons
                    grid_Popup_FilePicker_button_ControllerLeft.Visibility = Visibility.Collapsed;
                    grid_Popup_FilePicker_button_ControllerUp.Visibility = Visibility.Collapsed;

                    //Add uwp applications to the filepicker list
                    await ListLoadAllUwpApplications(List_FilePicker);

                    //Sort the uwp application list by name
                    SortObservableCollection(List_FilePicker, x => x.Name, null, true);
                }
                else
                {
                    //Clean the target path string
                    targetPath = Path.GetFullPath(targetPath);

                    //Add the Go up directory to the list
                    if (Path.GetPathRoot(targetPath) != targetPath)
                    {
                        BitmapImage imageBack = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Up.png" }, IntPtr.Zero, -1);
                        DataBindFile dataBindFileGoUp = new DataBindFile() { Type = "GoUp", Name = "Go up", NameSub = "(" + targetPath + ")", Description = "Go up to the previous folder.", ImageBitmap = imageBack, PathFile = Path.GetDirectoryName(targetPath) };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileGoUp, false, false);
                    }
                    else
                    {
                        BitmapImage imageBack = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Up.png" }, IntPtr.Zero, -1);
                        DataBindFile dataBindFileGoUp = new DataBindFile() { Type = "GoUp", Name = "Go up", NameSub = "(" + targetPath + ")", Description = "Go up to the previous folder.", ImageBitmap = imageBack, PathFile = "PC" };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileGoUp, false, false);
                    }

                    //Add launch emulator options
                    if (vFilePickerShowRoms)
                    {
                        string fileDescription = "Launch the emulator without a rom loaded";
                        BitmapImage fileImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Emulator.png" }, IntPtr.Zero, -1);
                        DataBindFile dataBindFileWithoutRom = new DataBindFile() { Type = "File", Name = fileDescription, Description = fileDescription + ".", ImageBitmap = fileImage, PathFile = string.Empty };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileWithoutRom, false, false);

                        string romDescription = "Launch the emulator with this folder as rom";
                        BitmapImage romImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Emulator.png" }, IntPtr.Zero, -1);
                        DataBindFile dataBindFileFolderRom = new DataBindFile() { Type = "File", Name = romDescription, Description = romDescription + ".", ImageBitmap = romImage, PathFile = targetPath };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFolderRom, false, false);
                    }

                    //Enable the side navigate buttons
                    grid_Popup_FilePicker_button_ControllerLeft.Visibility = Visibility.Visible;
                    grid_Popup_FilePicker_button_ControllerUp.Visibility = Visibility.Visible;

                    //Get all the directories from target directory
                    if (vFilePickerShowDirectories)
                    {
                        try
                        {
                            //Get all the files or folders
                            DirectoryInfo directoryInfo = new DirectoryInfo(targetPath);
                            DirectoryInfo[] directoryPaths = null;
                            if (vFilePickerSortByName)
                            {
                                directoryPaths = directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly).OrderBy(x => x.Name).ToArray();
                            }
                            else
                            {
                                directoryPaths = directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly).OrderByDescending(x => x.LastWriteTime).ToArray();
                            }

                            //Fill the file picker listbox with directories
                            BitmapImage folderImage = null;
                            string folderDescription = string.Empty;
                            foreach (DirectoryInfo listDirectory in directoryPaths)
                            {
                                try
                                {
                                    //Load image files for the list
                                    if (vFilePickerShowRoms)
                                    {
                                        //Get folder name
                                        string folderName = listDirectory.Name.ToLower();

                                        //Get description names
                                        string folderRomsTxt = "Assets\\Roms\\" + folderName + ".txt";
                                        string folderImageTxt = Path.Combine(listDirectory.FullName, folderName + ".txt");
                                        folderDescription = FileToString(new string[] { folderRomsTxt, folderImageTxt });

                                        //Get image names
                                        string folderRomsJpg = "Assets\\Roms\\" + folderName + ".jpg";
                                        string folderRomsPng = "Assets\\Roms\\" + folderName + ".png";
                                        string folderImageJpg = Path.Combine(listDirectory.FullName, folderName + ".jpg");
                                        string folderImagePng = Path.Combine(listDirectory.FullName, folderName + ".png");

                                        folderImage = FileToBitmapImage(new string[] { folderRomsPng, folderImagePng, folderRomsJpg, folderImageJpg, "pack://application:,,,/Assets/Icons/Folder.png" }, IntPtr.Zero, 180);
                                    }
                                    else
                                    {
                                        folderImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Folder.png" }, IntPtr.Zero, 50);
                                    }

                                    //Add folder to the list
                                    bool systemFileFolder = listDirectory.Attributes.HasFlag(FileAttributes.System);
                                    bool hiddenFileFolder = listDirectory.Attributes.HasFlag(FileAttributes.Hidden);
                                    if (!systemFileFolder && (!systemFileFolder || Convert.ToBoolean(ConfigurationManager.AppSettings["ShowHiddenFilesFolders"])))
                                    {
                                        DataBindFile dataBindFileFolder = new DataBindFile() { Type = "Directory", Name = listDirectory.Name, Description = folderDescription, DateModified = listDirectory.LastWriteTime, ImageBitmap = folderImage, PathFile = listDirectory.FullName };
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
                            //Disable selection button in the list
                            grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Collapsed;

                            //Get all the files or folders
                            DirectoryInfo directoryInfo = new DirectoryInfo(targetPath);
                            FileInfo[] directoryPathsFiles = null;
                            if (vFilePickerSortByName)
                            {
                                directoryPathsFiles = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly).OrderBy(x => x.Name).ToArray();
                            }
                            else
                            {
                                directoryPathsFiles = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly).OrderByDescending(x => x.LastWriteTime).ToArray();
                            }

                            string[] imageFilter = new string[] { "jpg", "png" };
                            string[] descriptionFilter = new string[] { "txt" };
                            FileInfo[] romImagesDirectory = new FileInfo[] { };
                            FileInfo[] romDescriptionsDirectory = new FileInfo[] { };

                            //Filter rom images and descriptions
                            if (vFilePickerShowRoms)
                            {
                                DirectoryInfo directoryInfoRoms = new DirectoryInfo("Assets\\Roms");
                                FileInfo[] directoryPathsRoms = directoryInfoRoms.GetFiles("*", SearchOption.TopDirectoryOnly).OrderBy(z => z.Name).ToArray();

                                FileInfo[] RomsImages = directoryPathsRoms.Where(file => imageFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                                FileInfo[] FilesImages = directoryPathsFiles.Where(file => imageFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                                romImagesDirectory = FilesImages.Concat(RomsImages).OrderByDescending(s => s.Name.Length).ToArray();

                                FileInfo[] RomsDescriptions = directoryPathsRoms.Where(file => descriptionFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                                FileInfo[] FilesDescriptions = directoryPathsFiles.Where(file => descriptionFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                                romDescriptionsDirectory = FilesDescriptions.Concat(RomsDescriptions).OrderByDescending(s => s.Name.Length).ToArray();
                            }

                            //Filter files in and out
                            if (vFilePickerFilterIn.Any())
                            {
                                directoryPathsFiles = directoryPathsFiles.Where(file => vFilePickerFilterIn.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                            }
                            if (vFilePickerFilterOut.Any())
                            {
                                directoryPathsFiles = directoryPathsFiles.Where(file => !vFilePickerFilterOut.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                            }

                            //Fill the file picker listbox with files based on filter
                            BitmapImage fileImage = null;
                            string fileDescription = string.Empty;
                            foreach (FileInfo listFile in directoryPathsFiles)
                            {
                                try
                                {
                                    //Load image files for the list
                                    if (vFilePickerShowRoms)
                                    {
                                        //Get rom file names
                                        string romFoundPathImage = string.Empty;
                                        string romFoundPathDescription = string.Empty;
                                        string romNameWithoutExtension = Path.GetFileNameWithoutExtension(listFile.Name).Replace(" ", string.Empty).ToLower();

                                        //Check if rom directory has image
                                        foreach (FileInfo FoundRom in romImagesDirectory)
                                        {
                                            try
                                            {
                                                if (romNameWithoutExtension.Contains(Path.GetFileNameWithoutExtension(FoundRom.Name.Replace(" ", string.Empty).ToLower())))
                                                {
                                                    romFoundPathImage = FoundRom.FullName;
                                                    break;
                                                }
                                            }
                                            catch { }
                                        }

                                        //Check if rom directory has description
                                        foreach (FileInfo FoundRom in romDescriptionsDirectory)
                                        {
                                            try
                                            {
                                                if (romNameWithoutExtension.Contains(Path.GetFileNameWithoutExtension(FoundRom.Name.Replace(" ", string.Empty).ToLower())))
                                                {
                                                    romFoundPathDescription = FoundRom.FullName;
                                                    break;
                                                }
                                            }
                                            catch { }
                                        }

                                        fileDescription = FileToString(new string[] { romFoundPathDescription });
                                        fileImage = FileToBitmapImage(new string[] { romFoundPathImage, "Rom" }, IntPtr.Zero, 180);
                                    }
                                    else
                                    {
                                        fileDescription = string.Empty;
                                        if (listFile.FullName.ToLower().EndsWith(".jpg") || listFile.FullName.ToLower().EndsWith(".png"))
                                        {
                                            fileImage = FileToBitmapImage(new string[] { listFile.FullName }, IntPtr.Zero, 50);
                                        }
                                        else if (listFile.FullName.ToLower().EndsWith(".exe"))
                                        {
                                            fileImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1);
                                        }
                                        else if (listFile.FullName.ToLower().EndsWith(".bat"))
                                        {
                                            fileImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/FileBat.png" }, IntPtr.Zero, -1);
                                        }
                                        else
                                        {
                                            fileImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/File.png" }, IntPtr.Zero, -1);
                                        }
                                    }

                                    //Add file to the list
                                    bool systemFileFolder = listFile.Attributes.HasFlag(FileAttributes.System);
                                    bool hiddenFileFolder = listFile.Attributes.HasFlag(FileAttributes.Hidden);
                                    if (!systemFileFolder && (!systemFileFolder || Convert.ToBoolean(ConfigurationManager.AppSettings["ShowHiddenFilesFolders"])))
                                    {
                                        DataBindFile dataBindFileFile = new DataBindFile() { Type = "File", Name = listFile.Name, Description = fileDescription, DateModified = listFile.LastWriteTime, ImageBitmap = fileImage, PathFile = listFile.FullName };
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
                        //Enable selection button in the list
                        grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Visible;
                    }
                }

                //Focus on the file picker listbox
                await ListboxFocus(lb_FilePicker, false, false, targetIndex);
            }
            catch { }
        }

        //Store the picker navigation index
        void File_Picker_AddIndexHistory(int PreviousIndex)
        {
            try
            {
                Debug.WriteLine("Adding navigation history: " + PreviousIndex);
                vFilePickerNavigateIndexes.Add(PreviousIndex);
            }
            catch { }
        }

        //Close file picker popup
        async Task Popup_Close_FilePicker(bool IsCompleted, bool CurrentFolder)
        {
            try
            {
                PlayInterfaceSound(vInterfaceSoundVolume, "PopupClose", false);

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
                GC.Collect();

                //Hide the popup with animation
                AVAnimations.Ani_Visibility(grid_Popup_FilePicker, false, false, 0.10);

                if (!Popup_Any_Open()) { AVAnimations.Ani_Opacity(grid_Main, 1, true, true, 0.10); }
                else if (vTextInputOpen) { AVAnimations.Ani_Opacity(grid_Popup_TextInput, 1, true, true, 0.10); }
                else if (vMessageBoxOpen) { AVAnimations.Ani_Opacity(grid_Popup_MessageBox, 1, true, true, 0.10); }
                else if (vFilePickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_FilePicker, 1, true, true, 0.10); }
                else if (vPopupOpen) { AVAnimations.Ani_Opacity(vPopupElementTarget, 1, true, true, 0.10); }
                else if (vColorPickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_ColorPicker, 1, true, true, 0.10); }
                else if (vSearchOpen) { AVAnimations.Ani_Opacity(grid_Popup_Search, 1, true, true, 0.10); }
                else if (vMainMenuOpen) { AVAnimations.Ani_Opacity(grid_Popup_MainMenu, 1, true, true, 0.10); }

                while (grid_Popup_FilePicker.Visibility == Visibility.Visible) { await Task.Delay(10); }

                //Focus on the previous focus element
                await Popup_PreviousFocusForce(vFilePickerElementFocus);
            }
            catch { }
        }

        //Show string based picker
        async void Button_ShowStringPicker(object sender, RoutedEventArgs e)
        {
            try
            {
                Button ButtonSender = (sender as Button);
                string ButtonName = ButtonSender.Name;

                //Change the manage application category
                if (ButtonName == "btn_Manage_AddAppCategory")
                {
                    //Check if the application is UWP
                    bool UwpApplication = sp_AddAppExePath.Visibility == Visibility.Collapsed;

                    //Set the application type categories
                    if (UwpApplication)
                    {
                        vFilePickerStrings = new string[][] { new[] { "Game", "Game" }, new[] { "App & Media", "App" } };
                    }
                    else
                    {
                        vFilePickerStrings = new string[][] { new[] { "Game", "Game" }, new[] { "App & Media", "App" }, new[] { "Emulator", "Emulator" } };
                    }

                    vFilePickerFilterIn = new string[] { };
                    vFilePickerFilterOut = new string[] { };
                    vFilePickerTitle = "Application Category";
                    vFilePickerDescription = "Please select a new application category:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = false;
                    vFilePickerShowDirectories = false;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("String", -1, false, null);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    image_Manage_AddAppCategory.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/" + vFilePickerResult.PathFile + ".png" }, IntPtr.Zero, -1);
                    textblock_Manage_AddAppCategory.Text = vFilePickerResult.Name;
                    btn_Manage_AddAppCategory.Tag = vFilePickerResult.PathFile;

                    if (UwpApplication || vFilePickerResult.PathFile != "Emulator")
                    {
                        sp_AddAppPathRoms.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        sp_AddAppPathRoms.Visibility = Visibility.Visible;
                    }

                    if (UwpApplication || vFilePickerResult.PathFile != "App")
                    {
                        checkbox_AddLaunchFilePicker.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        checkbox_AddLaunchFilePicker.Visibility = Visibility.Visible;
                    }
                }
                //Change the quick launch app
                else if (ButtonName == "btn_Settings_AppQuickLaunch")
                {
                    //Add all apps to the string list
                    vFilePickerStrings = CombineAppLists(false, false).Select(x => new[] { x.Name, x.Category.ToString() }).ToArray();

                    vFilePickerFilterIn = new string[] { };
                    vFilePickerFilterOut = new string[] { };
                    vFilePickerTitle = "Quick Launch Application";
                    vFilePickerDescription = "Please select a new quick launch application:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = false;
                    vFilePickerShowDirectories = false;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("String", -1, false, null);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    btn_Settings_AppQuickLaunch.Content = "Change quick launch app: " + vFilePickerResult.Name;

                    //Set previous quick launch application to false
                    foreach (DataBindApp dataBindApp in CombineAppLists(false, false).Where(x => x.QuickLaunch))
                    {
                        dataBindApp.QuickLaunch = false;
                    }

                    //Set new quick launch application to true
                    foreach (DataBindApp dataBindApp in CombineAppLists(false, false).Where(x => x.Name.ToLower() == vFilePickerResult.Name.ToLower()))
                    {
                        dataBindApp.QuickLaunch = true;
                    }

                    //Save changes to Json file
                    JsonSaveApps();
                }
            }
            catch { }
        }

        //Show file based picker
        async void Button_ShowFilePicker(object sender, RoutedEventArgs e)
        {
            try
            {
                Button ButtonSender = (sender as Button);
                string ButtonName = ButtonSender.Name;

                //Add and Edit application page
                if (ButtonName == "btn_AddAppLogo")
                {
                    //Check if there is an application name set
                    if (string.IsNullOrWhiteSpace(tb_AddAppName.Text) || tb_AddAppName.Text == "Select application executable file first")
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                        Answer1.Name = "Alright";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("Please enter an application name first", "", "", Answers);
                        return;
                    }

                    vFilePickerFilterIn = new string[] { "jpg", "png" };
                    vFilePickerFilterOut = new string[] { };
                    vFilePickerTitle = "Application Image";
                    vFilePickerDescription = "Please select a new application image:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, null);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Load and copy the new application image
                    BitmapImage applicationImage = FileToBitmapImage(new string[] { vFilePickerResult.PathFile }, IntPtr.Zero, 120);
                    img_AddAppLogo.Source = applicationImage;
                    if (vEditAppDataBind != null) { vEditAppDataBind.ImageBitmap = applicationImage; }
                    File.Copy(vFilePickerResult.PathFile, "Assets\\Apps\\" + tb_AddAppName.Text + ".png", true);
                }
                else if (ButtonName == "btn_AddAppExePath")
                {
                    vFilePickerFilterIn = new string[] { "exe" };
                    vFilePickerFilterOut = new string[] { };
                    vFilePickerTitle = "Application Executable";
                    vFilePickerDescription = "Please select an application executable:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, null);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Set fullpath to exe path textbox
                    tb_AddAppExePath.Text = vFilePickerResult.PathFile;
                    tb_AddAppExePath.IsEnabled = true;

                    //Set application launch path to textbox
                    tb_AddAppPathLaunch.Text = Path.GetDirectoryName(vFilePickerResult.PathFile);
                    tb_AddAppPathLaunch.IsEnabled = true;
                    btn_AddAppPathLaunch.IsEnabled = true;

                    //Set application name to textbox
                    tb_AddAppName.Text = vFilePickerResult.Name.Replace(".exe", "");
                    tb_AddAppName.IsEnabled = true;

                    //Set application image to image preview
                    img_AddAppLogo.Source = FileToBitmapImage(new string[] { tb_AddAppName.Text, vFilePickerResult.PathFile }, IntPtr.Zero, 120);
                    btn_AddAppLogo.IsEnabled = true;
                    btn_Manage_ResetAppLogo.IsEnabled = true;
                }
                else if (ButtonName == "btn_AddAppPathLaunch")
                {
                    vFilePickerFilterIn = new string[] { };
                    vFilePickerFilterOut = new string[] { };
                    vFilePickerTitle = "Launch Folder";
                    vFilePickerDescription = "Please select the launch folder:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = false;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, null);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    tb_AddAppPathLaunch.Text = vFilePickerResult.PathFile;
                    tb_AddAppPathLaunch.IsEnabled = true;
                    btn_AddAppPathLaunch.IsEnabled = true;
                }
                else if (ButtonName == "btn_AddAppPathRoms")
                {
                    vFilePickerFilterIn = new string[] { };
                    vFilePickerFilterOut = new string[] { };
                    vFilePickerTitle = "Rom Folder";
                    vFilePickerDescription = "Please select the rom folder:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = false;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, null);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    tb_AddAppPathRoms.Text = vFilePickerResult.PathFile;
                    tb_AddAppPathRoms.IsEnabled = true;
                }

                //Settings Background Changer
                else if (ButtonName == "btn_Settings_ChangeBackground")
                {
                    vFilePickerFilterIn = new string[] { "jpg", "png" };
                    vFilePickerFilterOut = new string[] { };
                    vFilePickerTitle = "Background Image";
                    vFilePickerDescription = "Please select a new background image:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, null);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    File.Copy(vFilePickerResult.PathFile, "Assets\\Background.png", true);
                    cb_SettingsDesktopBackground.IsChecked = false;
                    SettingSave("DesktopBackground", "False");
                    UpdateBackgroundImage();
                }

                //First launch quick setup
                else if (ButtonName == "grid_Popup_Welcome_button_Steam")
                {
                    vFilePickerFilterIn = new string[] { "steam.exe" };
                    vFilePickerFilterOut = new string[] { };
                    vFilePickerTitle = "Steam";
                    vFilePickerDescription = "Please select the Steam executable:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Add application to the list
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Steam", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile), Argument = "-bigpicture", QuickLaunch = true };
                    await AddAppToList(dataBindApp, true, true);

                    //Disable the icon after selection
                    ButtonSender.IsEnabled = false;
                    ButtonSender.Opacity = 0.30;
                }
                else if (ButtonName == "grid_Popup_Welcome_button_Origin")
                {
                    vFilePickerFilterIn = new string[] { "origin.exe" };
                    vFilePickerFilterOut = new string[] { };
                    vFilePickerTitle = "Origin";
                    vFilePickerDescription = "Please select the Origin executable:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Add application to the list
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Origin", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                    await AddAppToList(dataBindApp, true, true);

                    //Disable the icon after selection
                    ButtonSender.IsEnabled = false;
                    ButtonSender.Opacity = 0.30;
                }
                else if (ButtonName == "grid_Popup_Welcome_button_Uplay")
                {
                    vFilePickerFilterIn = new string[] { "upc.exe" };
                    vFilePickerFilterOut = new string[] { };
                    vFilePickerTitle = "Uplay";
                    vFilePickerDescription = "Please select the Uplay executable:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Add application to the list
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Uplay", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                    await AddAppToList(dataBindApp, true, true);

                    //Disable the icon after selection
                    ButtonSender.IsEnabled = false;
                    ButtonSender.Opacity = 0.30;
                }
                else if (ButtonName == "grid_Popup_Welcome_button_GoG")
                {
                    vFilePickerFilterIn = new string[] { "galaxyclient.exe" };
                    vFilePickerFilterOut = new string[] { };
                    vFilePickerTitle = "GoG";
                    vFilePickerDescription = "Please select the GoG executable:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Add application to the list
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "GoG", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                    await AddAppToList(dataBindApp, true, true);

                    //Disable the icon after selection
                    ButtonSender.IsEnabled = false;
                    ButtonSender.Opacity = 0.30;
                }
                else if (ButtonName == "grid_Popup_Welcome_button_Battle")
                {
                    vFilePickerFilterIn = new string[] { "battle.net.exe" };
                    vFilePickerFilterOut = new string[] { };
                    vFilePickerTitle = "Battle.net";
                    vFilePickerDescription = "Please select the Battle.net executable:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Add application to the list
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Battle.net", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                    await AddAppToList(dataBindApp, true, true);

                    //Disable the icon after selection
                    ButtonSender.IsEnabled = false;
                    ButtonSender.Opacity = 0.30;
                }
                else if (ButtonName == "grid_Popup_Welcome_button_PS4Remote")
                {
                    vFilePickerFilterIn = new string[] { "RemotePlay.exe" };
                    vFilePickerFilterOut = new string[] { };
                    vFilePickerTitle = "PS4 Remote Play";
                    vFilePickerDescription = "Please select the PS4 Remote Play executable:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Add application to the list
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Remote Play", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                    await AddAppToList(dataBindApp, true, true);

                    //Disable the icon after selection
                    ButtonSender.IsEnabled = false;
                    ButtonSender.Opacity = 0.30;
                }
                else if (ButtonName == "grid_Popup_Welcome_button_Kodi")
                {
                    vFilePickerFilterIn = new string[] { "kodi.exe" };
                    vFilePickerFilterOut = new string[] { };
                    vFilePickerTitle = "Kodi";
                    vFilePickerDescription = "Please select the Kodi executable:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Add application to the list
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Kodi", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                    await AddAppToList(dataBindApp, true, true);

                    //Disable the icon after selection
                    ButtonSender.IsEnabled = false;
                    ButtonSender.Opacity = 0.30;
                }
                else if (ButtonName == "grid_Popup_Welcome_button_Spotify")
                {
                    vFilePickerFilterIn = new string[] { "spotify.exe" };
                    vFilePickerFilterOut = new string[] { };
                    vFilePickerTitle = "Spotify";
                    vFilePickerDescription = "Please select the Spotify executable:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = true;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, grid_Popup_Welcome_button_Start);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Add application to the list
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Spotify", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) };
                    await AddAppToList(dataBindApp, true, true);

                    //Disable the icon after selection
                    ButtonSender.IsEnabled = false;
                    ButtonSender.Opacity = 0.30;
                }
            }
            catch { }
        }

        //Set copy file from the file picker
        void FilePicker_FileCopy(DataBindFile dataBindFile)
        {
            try
            {
                //Check the file or folder
                if (dataBindFile.Type == "PreDirectory" || dataBindFile.Type == "GoUp")
                {
                    Popup_Show_Status("Close", "Invalid file or folder");
                    Debug.WriteLine("Invalid file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                    return;
                }

                Popup_Show_Status("Copy", "Copying file or folder");
                Debug.WriteLine("Copying file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);

                //Set the clipboard variables
                vClipboardFile = dataBindFile;
                vClipboardType = "Copy";

                //Update the interface text
                grid_Popup_FilePicker_textblock_ClipboardStatus.Text = "Clipboard (" + vClipboardType + ") " + vClipboardFile.PathFile;
            }
            catch { }
        }

        //Set cut file from the file picker
        void FilePicker_FileCut(DataBindFile dataBindFile)
        {
            try
            {
                //Check the file or folder
                if (dataBindFile.Type == "PreDirectory" || dataBindFile.Type == "GoUp")
                {
                    Popup_Show_Status("Close", "Invalid file or folder");
                    Debug.WriteLine("Invalid file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                    return;
                }

                Popup_Show_Status("Cut", "Cutting file or folder");
                Debug.WriteLine("Cutting file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);

                //Set the clipboard variables
                vClipboardFile = dataBindFile;
                vClipboardType = "Cut";

                //Update the interface text
                grid_Popup_FilePicker_textblock_ClipboardStatus.Text = "Clipboard (" + vClipboardType + ") " + vClipboardFile.PathFile;
            }
            catch { }
        }

        //Paste file from the file picker
        async Task FilePicker_FilePaste()
        {
            try
            {
                //Get the current file picker path
                string oldFilePath = Path.GetFullPath(vClipboardFile.PathFile);
                string newFileName = Path.GetFileNameWithoutExtension(oldFilePath);
                string newFileExtension = Path.GetExtension(oldFilePath);
                string newFileDirectory = Path.GetFullPath(vFilePickerCurrentPath);
                string newFilePath = Path.Combine(newFileDirectory, newFileName + newFileExtension);

                //Move or copy the file or folder
                if (vClipboardType == "Cut")
                {
                    Popup_Show_Status("Cut", "Moving file or folder");
                    Debug.WriteLine("Moving file or folder: " + oldFilePath + " to " + newFilePath);

                    //Check if moving to same directory
                    if (oldFilePath == newFilePath)
                    {
                        Popup_Show_Status("Cut", "Invalid move folder");
                        Debug.WriteLine("Moving file or folder to the same directory.");
                        return;
                    }

                    //Check file or folder
                    FileAttributes fileAttribute = File.GetAttributes(oldFilePath);
                    if (fileAttribute.HasFlag(FileAttributes.Directory))
                    {
                        //Check if the directory exists
                        if (Directory.Exists(newFilePath))
                        {
                            //Count existing file names
                            int fileCount = Directory.GetDirectories(newFileDirectory, "*" + newFileName + "*").Count();

                            //Update the file name
                            newFileName += " - Cut (" + fileCount + ")";
                            newFilePath = Path.Combine(newFileDirectory, newFileName + newFileExtension);
                        }
                    }
                    else
                    {
                        //Check if the file exists
                        if (File.Exists(newFilePath))
                        {
                            //Count existing file names
                            int fileCount = Directory.GetFiles(newFileDirectory, "*" + newFileName + "*").Count();

                            //Update the file name
                            newFileName += " - Cut (" + fileCount + ")";
                            newFilePath = Path.Combine(newFileDirectory, newFileName + newFileExtension);
                        }
                    }

                    //Update file name in new clipboard
                    DataBindFile updatedClipboard = CloneClassObject(vClipboardFile);
                    updatedClipboard.Name = newFileName + newFileExtension;
                    updatedClipboard.PathFile = newFilePath;

                    //Update the file or folder in the list
                    await AVActions.ActionDispatcherInvokeAsync(async delegate
                    {
                        //Remove the moved listbox item
                        await ListBoxRemoveItem(lb_FilePicker, List_FilePicker, vClipboardFile);

                        //Add and select the listbox item
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, updatedClipboard, false, true);
                    });

                    //Reset the current clipboard
                    vClipboardFile = null;
                    vClipboardType = string.Empty;
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        grid_Popup_FilePicker_textblock_ClipboardStatus.Text = string.Empty;
                    });

                    //Move file or folder
                    SHFILEOPSTRUCT shFileOpstruct = new SHFILEOPSTRUCT();
                    shFileOpstruct.wFunc = FILEOP_FUNC.FO_MOVE;
                    shFileOpstruct.pFrom = oldFilePath + "\0\0";
                    shFileOpstruct.pTo = newFilePath + "\0\0";
                    SHFileOperation(ref shFileOpstruct);
                }
                else
                {
                    Popup_Show_Status("Copy", "Copying file or folder");
                    Debug.WriteLine("Copying file or folder: " + oldFilePath + " to " + newFilePath);

                    //Check file or folder
                    FileAttributes fileAttribute = File.GetAttributes(oldFilePath);
                    if (fileAttribute.HasFlag(FileAttributes.Directory))
                    {
                        //Check if the directory exists
                        if (Directory.Exists(newFilePath))
                        {
                            //Count existing file names
                            int fileCount = Directory.GetDirectories(newFileDirectory, "*" + newFileName + "*").Count();

                            //Update the file name
                            newFileName += " - Copy (" + fileCount + ")";
                            newFilePath = Path.Combine(newFileDirectory, newFileName + newFileExtension);
                        }
                    }
                    else
                    {
                        //Check if the file exists
                        if (File.Exists(newFilePath))
                        {
                            //Count existing file names
                            int fileCount = Directory.GetFiles(newFileDirectory, "*" + newFileName + "*").Count();

                            //Update the file name
                            newFileName += " - Copy (" + fileCount + ")";
                            newFilePath = Path.Combine(newFileDirectory, newFileName + newFileExtension);
                        }
                    }

                    //Update file name in new clipboard
                    DataBindFile updatedClipboard = CloneClassObject(vClipboardFile);
                    updatedClipboard.Name = newFileName + newFileExtension;
                    updatedClipboard.PathFile = newFilePath;

                    //Update the file or folder in the list
                    await AVActions.ActionDispatcherInvokeAsync(async delegate
                    {
                        //Add and select the listbox item
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, updatedClipboard, false, true);
                    });

                    //Copy file or folder
                    SHFILEOPSTRUCT shFileOpstruct = new SHFILEOPSTRUCT();
                    shFileOpstruct.wFunc = FILEOP_FUNC.FO_COPY;
                    shFileOpstruct.pFrom = oldFilePath + "\0\0";
                    shFileOpstruct.pTo = newFilePath + "\0\0";
                    SHFileOperation(ref shFileOpstruct);
                }
            }
            catch (Exception ex)
            {
                Popup_Show_Status("Paste", "Failed pasting");
                Debug.WriteLine("Failed pasting file or folder: " + ex.Message);
            }
        }

        //Rename file from the file picker
        async Task FilePicker_FileRename(DataBindFile dataBindFile)
        {
            try
            {
                //Check the file or folder
                if (dataBindFile.Type == "PreDirectory" || dataBindFile.Type == "GoUp")
                {
                    Popup_Show_Status("Close", "Invalid file or folder");
                    Debug.WriteLine("Invalid file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                    return;
                }

                Popup_Show_Status("Rename", "Renaming file or folder");
                Debug.WriteLine("Renaming file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);

                //Show the text input popup
                string textInputString = await Popup_ShowHide_TextInput("Rename file or folder", dataBindFile.Name, "Rename the file or folder");

                //Check if file name changed
                if (textInputString == dataBindFile.Name)
                {
                    Popup_Show_Status("Rename", "File name not changed");
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
                        Directory.Move(oldFilePath, newFilePath);
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
                        File.Move(oldFilePath, newFilePath);
                    }

                    //Update file name in listbox
                    dataBindFile.Name = newFileName + newFileExtension;
                    dataBindFile.PathFile = newFilePath;

                    //Check if the renamed item is clipboard and update it
                    if (vClipboardFile != null && vClipboardFile.PathFile == dataBindFile.PathFile)
                    {
                        grid_Popup_FilePicker_textblock_ClipboardStatus.Text = "Clipboard (" + vClipboardType + ") " + vClipboardFile.PathFile;
                    }

                    Popup_Show_Status("Rename", "Renamed file or folder");
                    Debug.WriteLine("Renamed file or folder to: " + newFileName + newFileExtension);
                }
            }
            catch (Exception ex)
            {
                Popup_Show_Status("Rename", "Failed renaming");
                Debug.WriteLine("Failed renaming file or folder: " + ex.Message);
            }
        }

        //Remove file from the file picker
        async Task FilePicker_FileRemove(DataBindFile dataBindFile, bool useRecycleBin)
        {
            try
            {
                //Check the file or folder
                if (dataBindFile.Type == "PreDirectory" || dataBindFile.Type == "GoUp")
                {
                    Popup_Show_Status("Close", "Invalid file or folder");
                    Debug.WriteLine("Invalid file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
                    return;
                }

                Popup_Show_Status("Remove", "Remove file or folder");
                Debug.WriteLine("Removing file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);

                //Remove file or folder
                SHFILEOPSTRUCT shFileOpstruct = new SHFILEOPSTRUCT();
                shFileOpstruct.wFunc = FILEOP_FUNC.FO_DELETE;
                shFileOpstruct.pFrom = dataBindFile.PathFile + "\0\0";
                if (useRecycleBin)
                {
                    shFileOpstruct.fFlags = FILEOP_FLAGS.FOF_NOCONFIRMATION | FILEOP_FLAGS.FOF_ALLOWUNDO;
                }
                else
                {
                    shFileOpstruct.fFlags = FILEOP_FLAGS.FOF_NOCONFIRMATION;
                }
                SHFileOperation(ref shFileOpstruct);

                //Check if the removed item is clipboard and reset it
                if (vClipboardFile != null && vClipboardFile.PathFile == dataBindFile.PathFile)
                {
                    vClipboardFile = null;
                    vClipboardType = string.Empty;
                    grid_Popup_FilePicker_textblock_ClipboardStatus.Text = string.Empty;
                }

                //Remove file from the listbox
                await ListBoxRemoveItem(lb_FilePicker, List_FilePicker, dataBindFile);

                Popup_Show_Status("Remove", "Removed file or folder");
                Debug.WriteLine("Removed file or folder: " + dataBindFile.Name + " path: " + dataBindFile.PathFile);
            }
            catch (Exception ex)
            {
                Popup_Show_Status("Remove", "Failed removing");
                Debug.WriteLine("Failed removing file or folder: " + ex.Message);
            }
        }

        //Empty the Windows Recycle Bin
        async Task RecycleBin_Empty()
        {
            try
            {
                List<DataBindString> messageAnswers = new List<DataBindString>();
                DataBindString answerEmpty = new DataBindString();
                answerEmpty.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Remove.png" }, IntPtr.Zero, -1);
                answerEmpty.Name = "Empty recycle bin";
                messageAnswers.Add(answerEmpty);

                DataBindString answerCancel = new DataBindString();
                answerCancel.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                answerCancel.Name = "Cancel";
                messageAnswers.Add(answerCancel);

                DataBindString messageResult = await Popup_Show_MessageBox("Empty the recycle bin?", "", "This will permanently remove all the files from your recycle bin.", messageAnswers);
                if (messageResult == answerEmpty)
                {
                    Popup_Show_Status("Remove", "Emptying recycle bin");
                    Debug.WriteLine("Emptying the Windows recycle bin.");

                    //Play recycle bin empty sound
                    PlayInterfaceSound(vInterfaceSoundVolume, "RecycleBinEmpty", false);

                    //Empty the windows recycle bin
                    SHEmptyRecycleBin(IntPtr.Zero, null, RecycleBin_FLAGS.SHRB_NOCONFIRMATION | RecycleBin_FLAGS.SHRB_NOSOUND);
                }
            }
            catch { }
        }
    }
}