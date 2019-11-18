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
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVInterface;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show the File Picker Popup
        async Task Popup_Show_FilePicker(string TargetPath, int TargetIndex, bool StoreIndex, FrameworkElement PreviousFocus)
        {
            try
            {
                //Play the popup opening sound
                if (!vFilePickerOpen)
                {
                    PlayInterfaceSound("PopupOpen", false);
                }

                //Save previous focus element
                if (PreviousFocus != null)
                {
                    vFilePickerElementFocus.FocusPrevious = PreviousFocus;
                    if (vFilePickerElementFocus.FocusPrevious.GetType() == typeof(ListBoxItem))
                    {
                        vFilePickerElementFocus.FocusListBox = AVFunctions.FindVisualParent<ListBox>(vFilePickerElementFocus.FocusPrevious);
                        vFilePickerElementFocus.FocusIndex = vFilePickerElementFocus.FocusListBox.SelectedIndex;
                    }
                }
                else if (!vFilePickerOpen && Keyboard.FocusedElement != null)
                {
                    vFilePickerElementFocus.FocusPrevious = (FrameworkElement)Keyboard.FocusedElement;
                    if (vFilePickerElementFocus.FocusPrevious.GetType() == typeof(ListBoxItem))
                    {
                        vFilePickerElementFocus.FocusListBox = AVFunctions.FindVisualParent<ListBox>(vFilePickerElementFocus.FocusPrevious);
                        vFilePickerElementFocus.FocusIndex = vFilePickerElementFocus.FocusListBox.SelectedIndex;
                    }
                }

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

                if (vMessageBoxOpen) { AVAnimations.Ani_Opacity(grid_Popup_MessageBox, 0.02, true, false, 0.10); }
                //if (vFilePickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_FilePicker, 0.02, true, false, 0.10); }
                if (vPopupOpen) { AVAnimations.Ani_Opacity(vPopupElementTarget, 0.02, true, false, 0.10); }
                if (vColorPickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_ColorPicker, 0.02, true, false, 0.10); }
                if (vSearchOpen) { AVAnimations.Ani_Opacity(grid_Popup_Search, 0.02, true, false, 0.10); }
                if (vMainMenuOpen) { AVAnimations.Ani_Opacity(grid_Popup_MainMenu, 0.02, true, false, 0.10); }

                //Get and update the current index and path
                vFilePickerCurrentPath = TargetPath;
                if (StoreIndex)
                {
                    File_Picker_AddIndexHistory(lb_FilePicker.SelectedIndex);
                }

                //Clear the current file picker list
                List_FilePicker.Clear();
                GC.Collect();

                //Get and set all strings to list
                if (TargetPath == "String")
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
                            BitmapImage ImageFile = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/" + stringPicker[1] + ".png" }, IntPtr.Zero, -1);
                            List_FilePicker.Add(new DataBindFile() { Type = "File", Name = stringPicker[0], PathFile = stringPicker[1], ImageBitmap = ImageFile });
                        }
                        catch { }
                    }
                }
                //Get and list all the disk drives
                else if (TargetPath == "PC")
                {
                    //Disable selection button in the list
                    grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Collapsed;

                    //Disable the side navigate buttons
                    grid_Popup_FilePicker_button_ControllerLeft.Visibility = Visibility.Collapsed;
                    grid_Popup_FilePicker_button_ControllerUp.Visibility = Visibility.Collapsed;

                    BitmapImage ImageFolder = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Folder.png" }, IntPtr.Zero, -1);

                    //Add my documents and pictures folder
                    List_FilePicker.Add(new DataBindFile() { Type = "PreDirectory", Name = "My Pictures", ImageBitmap = ImageFolder, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) });
                    List_FilePicker.Add(new DataBindFile() { Type = "PreDirectory", Name = "My Documents", ImageBitmap = ImageFolder, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) });

                    //Check and set the previous path
                    if (!string.IsNullOrWhiteSpace(vFilePickerPreviousPath))
                    {
                        List_FilePicker.Add(new DataBindFile() { Type = "PreDirectory", Name = "Previous Path", NameSub = "(" + vFilePickerPreviousPath + ")", ImageBitmap = ImageFolder, PathFile = vFilePickerPreviousPath });
                    }

                    //Add no file load options
                    if (vFilePickerShowNoFile)
                    {
                        string FileDescription = "Launch application without a file";
                        BitmapImage FileImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1);
                        List_FilePicker.Add(new DataBindFile() { Type = "File", Name = FileDescription, Description = FileDescription + ".", ImageBitmap = FileImage, PathFile = "" });
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
                                List_FilePicker.Add(new DataBindFile() { Type = "Directory", Name = Disk.Name, NameSub = Disk.VolumeLabel, ImageBitmap = ImageFolder, PathFile = Disk.Name });
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
                                List_FilePicker.Add(new DataBindFile() { Type = "Directory", Name = Locations.Path, NameSub = Locations.Name, ImageBitmap = ImageFolder, PathFile = Locations.Path });
                            }
                        }
                        catch { }
                    }
                }
                //Get and list all the UWP applications
                else if (TargetPath == "UWP")
                {
                    //Disable selection button in the list
                    grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Collapsed;

                    //Disable the side navigate buttons
                    grid_Popup_FilePicker_button_ControllerLeft.Visibility = Visibility.Collapsed;
                    grid_Popup_FilePicker_button_ControllerUp.Visibility = Visibility.Collapsed;

                    //Add uwp applications to the filepicker list
                    UwpAppListAll(List_FilePicker);

                    //Sort the uwp application list by name
                    SortObservableCollection(List_FilePicker, x => x.Name, null, true);
                }
                else
                {
                    //Add the Go up directory to the list
                    if (Path.GetPathRoot(TargetPath) != TargetPath)
                    {
                        BitmapImage ImageBack = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Up.png" }, IntPtr.Zero, -1);
                        List_FilePicker.Add(new DataBindFile() { Type = "GoUp", Name = "Go up", NameSub = "(" + TargetPath + ")", Description = "Go up to the previous folder.", ImageBitmap = ImageBack, PathFile = Path.GetDirectoryName(TargetPath) });
                    }
                    else
                    {
                        BitmapImage ImageBack = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Up.png" }, IntPtr.Zero, -1);
                        List_FilePicker.Add(new DataBindFile() { Type = "GoUp", Name = "Go up", NameSub = "(" + TargetPath + ")", Description = "Go up to the previous folder.", ImageBitmap = ImageBack, PathFile = "PC" });
                    }

                    //Add launch emulator options
                    if (vFilePickerShowRoms)
                    {
                        string FileDescription = "Launch the emulator without a rom loaded";
                        BitmapImage FileImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Emulator.png" }, IntPtr.Zero, -1);
                        List_FilePicker.Add(new DataBindFile() { Type = "File", Name = FileDescription, Description = FileDescription + ".", ImageBitmap = FileImage, PathFile = "" });

                        string RomDescription = "Launch the emulator with this folder as rom";
                        BitmapImage RomImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Emulator.png" }, IntPtr.Zero, -1);
                        List_FilePicker.Add(new DataBindFile() { Type = "File", Name = RomDescription, Description = RomDescription + ".", ImageBitmap = RomImage, PathFile = TargetPath });
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
                            DirectoryInfo directoryInfo = new DirectoryInfo(TargetPath);
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
                            BitmapImage FolderImage = null;
                            string FolderDescription = string.Empty;
                            foreach (DirectoryInfo ListDirectory in directoryPaths)
                            {
                                try
                                {
                                    //Load image files for the list
                                    if (vFilePickerShowRoms)
                                    {
                                        //Get folder name
                                        string FolderName = ListDirectory.Name.ToLower();

                                        //Get description names
                                        string FolderRomsTxt = "Assets\\Roms\\" + FolderName + ".txt";
                                        string FolderImageTxt = ListDirectory.FullName + "\\" + FolderName + ".txt";
                                        FolderDescription = FileToString(new string[] { FolderRomsTxt, FolderImageTxt });

                                        //Get image names
                                        string FolderRomsJpg = "Assets\\Roms\\" + FolderName + ".jpg";
                                        string FolderRomsPng = "Assets\\Roms\\" + FolderName + ".png";
                                        string FolderImageJpg = ListDirectory.FullName + "\\" + FolderName + ".jpg";
                                        string FolderImagePng = ListDirectory.FullName + "\\" + FolderName + ".png";

                                        FolderImage = FileToBitmapImage(new string[] { FolderRomsPng, FolderImagePng, FolderRomsJpg, FolderImageJpg, "pack://application:,,,/Assets/Icons/Folder.png" }, IntPtr.Zero, 180);
                                    }
                                    else
                                    {
                                        FolderImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Folder.png" }, IntPtr.Zero, 50);
                                    }

                                    //Add folder to the list
                                    bool HiddenFileFolder = (ListDirectory.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
                                    if (!HiddenFileFolder || Convert.ToBoolean(ConfigurationManager.AppSettings["ShowHiddenFilesFolders"]))
                                    {
                                        List_FilePicker.Add(new DataBindFile() { Type = "Directory", Name = ListDirectory.Name, Description = FolderDescription, DateModified = ListDirectory.LastWriteTime, ImageBitmap = FolderImage, PathFile = ListDirectory.FullName });
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
                            DirectoryInfo directoryInfo = new DirectoryInfo(TargetPath);
                            FileInfo[] directoryPathsFiles = null;
                            if (vFilePickerSortByName)
                            {
                                directoryPathsFiles = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly).OrderBy(x => x.Name).ToArray();
                            }
                            else
                            {
                                directoryPathsFiles = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly).OrderByDescending(x => x.LastWriteTime).ToArray();
                            }

                            string[] ImageFilter = new string[] { "jpg", "png" };
                            string[] DescriptionFilter = new string[] { "txt" };
                            FileInfo[] RomImagesDirectory = new FileInfo[] { };
                            FileInfo[] RomDescriptionsDirectory = new FileInfo[] { };

                            //Filter rom images and descriptions
                            if (vFilePickerShowRoms)
                            {
                                DirectoryInfo directoryInfoRoms = new DirectoryInfo("Assets\\Roms");
                                FileInfo[] directoryPathsRoms = directoryInfoRoms.GetFiles("*", SearchOption.TopDirectoryOnly).OrderBy(z => z.Name).ToArray();

                                FileInfo[] RomsImages = directoryPathsRoms.Where(file => ImageFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                                FileInfo[] FilesImages = directoryPathsFiles.Where(file => ImageFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                                RomImagesDirectory = FilesImages.Concat(RomsImages).OrderByDescending(s => s.Name.Length).ToArray();

                                FileInfo[] RomsDescriptions = directoryPathsRoms.Where(file => DescriptionFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                                FileInfo[] FilesDescriptions = directoryPathsFiles.Where(file => DescriptionFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                                RomDescriptionsDirectory = FilesDescriptions.Concat(RomsDescriptions).OrderByDescending(s => s.Name.Length).ToArray();
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
                            BitmapImage FileImage = null;
                            string FileDescription = string.Empty;
                            foreach (FileInfo ListFile in directoryPathsFiles)
                            {
                                try
                                {
                                    //Load image files for the list
                                    if (vFilePickerShowRoms)
                                    {
                                        //Get rom file names
                                        string RomFoundPathImage = string.Empty;
                                        string RomFoundPathDescription = string.Empty;
                                        string RomNameWithoutExtension = Path.GetFileNameWithoutExtension(ListFile.Name).Replace(" ", string.Empty).ToLower();

                                        //Check if rom directory has image
                                        foreach (FileInfo FoundRom in RomImagesDirectory)
                                        {
                                            try
                                            {
                                                if (RomNameWithoutExtension.Contains(Path.GetFileNameWithoutExtension(FoundRom.Name.Replace(" ", string.Empty).ToLower())))
                                                {
                                                    RomFoundPathImage = FoundRom.FullName;
                                                    break;
                                                }
                                            }
                                            catch { }
                                        }

                                        //Check if rom directory has description
                                        foreach (FileInfo FoundRom in RomDescriptionsDirectory)
                                        {
                                            try
                                            {
                                                if (RomNameWithoutExtension.Contains(Path.GetFileNameWithoutExtension(FoundRom.Name.Replace(" ", string.Empty).ToLower())))
                                                {
                                                    RomFoundPathDescription = FoundRom.FullName;
                                                    break;
                                                }
                                            }
                                            catch { }
                                        }

                                        FileDescription = FileToString(new string[] { RomFoundPathDescription });
                                        FileImage = FileToBitmapImage(new string[] { RomFoundPathImage, "Rom" }, IntPtr.Zero, 180);
                                    }
                                    else
                                    {
                                        FileDescription = string.Empty;
                                        if (ListFile.FullName.ToLower().EndsWith(".jpg") || ListFile.FullName.ToLower().EndsWith(".png"))
                                        {
                                            FileImage = FileToBitmapImage(new string[] { ListFile.FullName }, IntPtr.Zero, 50);
                                        }
                                        else if (ListFile.FullName.ToLower().EndsWith(".exe"))
                                        {
                                            FileImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1);
                                        }
                                        else if (ListFile.FullName.ToLower().EndsWith(".bat"))
                                        {
                                            FileImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/FileBat.png" }, IntPtr.Zero, -1);
                                        }
                                        else
                                        {
                                            FileImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/File.png" }, IntPtr.Zero, -1);
                                        }
                                    }

                                    //Add file to the list
                                    bool HiddenFileFolder = (ListFile.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
                                    if (!HiddenFileFolder || Convert.ToBoolean(ConfigurationManager.AppSettings["ShowHiddenFilesFolders"]))
                                    {
                                        List_FilePicker.Add(new DataBindFile() { Type = "File", Name = ListFile.Name, Description = FileDescription, DateModified = ListFile.LastWriteTime, ImageBitmap = FileImage, PathFile = ListFile.FullName });
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
                await FocusOnListbox(lb_FilePicker, false, false, TargetIndex);
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
                GC.Collect();

                //Hide the popup with animation
                AVAnimations.Ani_Visibility(grid_Popup_FilePicker, false, false, 0.10);

                if (!Popup_Any_Open()) { AVAnimations.Ani_Opacity(grid_Main, 1, true, true, 0.10); }
                else if (vMessageBoxOpen) { AVAnimations.Ani_Opacity(grid_Popup_MessageBox, 1, true, true, 0.10); }
                else if (vFilePickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_FilePicker, 1, true, true, 0.10); }
                else if (vPopupOpen) { AVAnimations.Ani_Opacity(vPopupElementTarget, 1, true, true, 0.10); }
                else if (vColorPickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_ColorPicker, 1, true, true, 0.10); }
                else if (vSearchOpen) { AVAnimations.Ani_Opacity(grid_Popup_Search, 1, true, true, 0.10); }
                else if (vMainMenuOpen) { AVAnimations.Ani_Opacity(grid_Popup_MainMenu, 1, true, true, 0.10); }

                while (grid_Popup_FilePicker.Visibility == Visibility.Visible) { await Task.Delay(10); }

                //Force focus on an element
                if (vFilePickerElementFocus.FocusTarget != null)
                {
                    await FocusOnElement(vFilePickerElementFocus.FocusTarget, false, vProcessCurrent.MainWindowHandle);
                }
                else if (vFilePickerElementFocus.FocusListBox != null)
                {
                    await FocusOnListbox(vFilePickerElementFocus.FocusListBox, false, false, vFilePickerElementFocus.FocusIndex);
                }
                else
                {
                    await FocusOnElement(vFilePickerElementFocus.FocusPrevious, false, vProcessCurrent.MainWindowHandle);
                }

                //Reset previous focus
                vFilePickerElementFocus.Reset();
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
                    vFilePickerStrings = CombineAppLists(false, false).Select(x => new[] { x.Name, x.Category }).ToArray();

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
                    foreach (DataBindApp QuickLaunchApps in CombineAppLists(false, false).Where(x => x.QuickLaunch)) { QuickLaunchApps.QuickLaunch = false; }

                    //Set new quick launch application to true
                    foreach (DataBindApp QuickLaunchApps in CombineAppLists(false, false).Where(x => x.Name.ToLower() == vFilePickerResult.Name.ToLower())) { QuickLaunchApps.QuickLaunch = true; }

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

                    img_AddAppLogo.Source = FileToBitmapImage(new string[] { vFilePickerResult.PathFile }, IntPtr.Zero, 120);
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

                //Settings change shortcuts directory
                else if (ButtonName == "btn_Settings_ChangeShortcutsDirectory")
                {
                    vFilePickerFilterIn = new string[] { };
                    vFilePickerFilterOut = new string[] { };
                    vFilePickerTitle = "Shortcut Folder";
                    vFilePickerDescription = "Please select a new shortcut folder:";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = false;
                    vFilePickerShowDirectories = true;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("PC", -1, false, null);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Save the new shortcuts directory path
                    SettingSave("DirectoryShortcuts", vFilePickerResult.PathFile);
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
                    AddAppToList(new DataBindApp() { Category = "Game", Name = "Steam", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile), Argument = "-bigpicture", QuickLaunch = true }, true, true);

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
                    AddAppToList(new DataBindApp() { Category = "Game", Name = "Origin", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) }, true, true);

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
                    AddAppToList(new DataBindApp() { Category = "Game", Name = "Uplay", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) }, true, true);

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
                    AddAppToList(new DataBindApp() { Category = "Game", Name = "GoG", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) }, true, true);

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
                    AddAppToList(new DataBindApp() { Category = "Game", Name = "Battle.net", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) }, true, true);

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
                    AddAppToList(new DataBindApp() { Category = "App", Name = "Remote Play", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) }, true, true);

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
                    AddAppToList(new DataBindApp() { Category = "App", Name = "Kodi", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) }, true, true);

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
                    AddAppToList(new DataBindApp() { Category = "App", Name = "Spotify", PathExe = vFilePickerResult.PathFile, PathLaunch = Path.GetDirectoryName(vFilePickerResult.PathFile) }, true, true);

                    //Disable the icon after selection
                    ButtonSender.IsEnabled = false;
                    ButtonSender.Opacity = 0.30;
                }
            }
            catch { }
        }
    }
}