using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static CtrlUI.AppVariables;
using static LibraryShared.AppStartupCheck;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Add application to the specific list
        async Task AddAppToList(DataBindApp dataBindApp, bool generateAppNumber, bool loadAppImage)
        {
            try
            {
                //Generate application number
                if (generateAppNumber)
                {
                    dataBindApp.Number = GetHighestAppNumber();
                }

                //Check if application is a Win32 app
                if (dataBindApp.Type == ProcessType.Win32)
                {
                    //Check if application still exists
                    if (!File.Exists(dataBindApp.PathExe))
                    {
                        dataBindApp.StatusAvailable = Visibility.Visible;
                    }
                    //Check if the rom folder is available
                    else if (dataBindApp.Category == AppCategory.Emulator && !Directory.Exists(dataBindApp.PathRoms))
                    {
                        dataBindApp.StatusAvailable = Visibility.Visible;
                    }
                }

                //Check if application is an UWP or Win32Store app
                if (dataBindApp.Type == ProcessType.UWP || dataBindApp.Type == ProcessType.Win32Store)
                {
                    dataBindApp.StatusStore = Visibility.Visible;

                    //Check if application still exists
                    if (UwpGetAppPackageByAppUserModelId(dataBindApp.PathExe) == null)
                    {
                        dataBindApp.StatusAvailable = Visibility.Visible;
                    }
                }

                //Load and set application image
                if (loadAppImage)
                {
                    dataBindApp.ImageBitmap = FileToBitmapImage(new string[] { dataBindApp.Name, dataBindApp.PathExe, dataBindApp.PathImage }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 90, 0);
                }

                //Add application to the list
                if (dataBindApp.Category == AppCategory.Game)
                {
                    await ListBoxAddItem(lb_Games, List_Games, dataBindApp, false, false);
                }
                else if (dataBindApp.Category == AppCategory.App)
                {
                    await ListBoxAddItem(lb_Apps, List_Apps, dataBindApp, false, false);
                }
                else if (dataBindApp.Category == AppCategory.Emulator)
                {
                    await ListBoxAddItem(lb_Emulators, List_Emulators, dataBindApp, false, false);
                }

                //Save changes to Json file
                if (generateAppNumber)
                {
                    JsonSaveApplications();
                }
            }
            catch { }
        }

        //Remove application from the list
        async Task RemoveAppFromList(DataBindApp dataBindApp, bool saveJson, bool removeImageFile, bool silent)
        {
            try
            {
                //Remove application from the listboxes
                if (dataBindApp.Category == AppCategory.Game)
                {
                    await ListBoxRemoveItem(lb_Games, List_Games, dataBindApp, true);
                }
                else if (dataBindApp.Category == AppCategory.App)
                {
                    await ListBoxRemoveItem(lb_Apps, List_Apps, dataBindApp, true);
                }
                else if (dataBindApp.Category == AppCategory.Emulator)
                {
                    await ListBoxRemoveItem(lb_Emulators, List_Emulators, dataBindApp, true);
                }
                else if (dataBindApp.Category == AppCategory.Process)
                {
                    await ListBoxRemoveItem(lb_Processes, List_Processes, dataBindApp, true);
                }
                else if (dataBindApp.Category == AppCategory.Shortcut)
                {
                    await ListBoxRemoveItem(lb_Shortcuts, List_Shortcuts, dataBindApp, true);
                }

                //Remove application from search listbox
                if (vSearchOpen)
                {
                    await ListBoxRemoveItem(lb_Search, List_Search, dataBindApp, true);
                }

                //Save changes to Json file
                if (saveJson)
                {
                    JsonSaveApplications();
                }

                //Remove application image files
                if (removeImageFile)
                {
                    string imageFileTitle = "Assets/Apps/" + dataBindApp.Name + ".png";
                    string imageFileExe = "Assets/Apps/" + Path.GetFileNameWithoutExtension(dataBindApp.PathExe) + ".png";

                    //Check the application image
                    bool defaultImage = AssetsAppsFiles.Contains(imageFileTitle) || AssetsAppsFiles.Contains(imageFileExe);
                    if (defaultImage)
                    {
                        Debug.WriteLine("Default application images cannot be removed.");
                    }
                    else
                    {
                        File_Delete(imageFileTitle);
                        File_Delete(imageFileExe);
                    }
                }

                //Show removed notification
                if (!silent)
                {
                    Popup_Show_Status("Minus", "Removed " + dataBindApp.Name);
                    Debug.WriteLine("Removed application: " + dataBindApp.Name);
                }
            }
            catch { }
        }

        //Show the application edit popup
        async Task Popup_Show_AppEdit(ListBox editListBox)
        {
            try
            {
                int listboxSelectedIndex = editListBox.SelectedIndex;
                if (editListBox.SelectedItems.Count > 0 && listboxSelectedIndex != -1)
                {
                    grid_Popup_Manage_txt_Title.Text = "Edit application";
                    btn_AddAppLogo.IsEnabled = true;
                    btn_Manage_ResetAppLogo.IsEnabled = true;
                    tb_AddAppName.IsEnabled = true;
                    tb_AddAppExePath.IsEnabled = true;
                    tb_AddAppPathLaunch.IsEnabled = true;
                    btn_AddAppPathLaunch.IsEnabled = true;
                    tb_AddAppPathRoms.IsEnabled = true;
                    btn_Manage_SaveEditApp.Content = "Edit the application as filled in above";
                    grid_EditMoveApp.Visibility = Visibility.Visible;

                    //Set the variables as current edit app
                    vEditAppListBox = editListBox;
                    vEditAppDataBind = (DataBindApp)editListBox.SelectedItem;
                    vEditAppCategoryPrevious = vEditAppDataBind.Category;

                    //Load the application categories
                    List<DataBindString> listAppCategories = new List<DataBindString>();

                    DataBindString categoryApp = new DataBindString();
                    categoryApp.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/App.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    categoryApp.Name = "App & Media";
                    listAppCategories.Add(categoryApp);

                    DataBindString categoryGame = new DataBindString();
                    categoryGame.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Game.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    categoryGame.Name = "Game";
                    listAppCategories.Add(categoryGame);

                    DataBindString categoryEmulator = new DataBindString();
                    categoryEmulator.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Emulator.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    categoryEmulator.Name = "Emulator";
                    listAppCategories.Add(categoryEmulator);

                    lb_Manage_AddAppCategory.ItemsSource = listAppCategories;

                    //Select current application category
                    lb_Manage_AddAppCategory.SelectedIndex = (int)vEditAppDataBind.Category;

                    //Load application image
                    img_AddAppLogo.Source = FileToBitmapImage(new string[] { vEditAppDataBind.Name, vEditAppDataBind.PathExe, vEditAppDataBind.PathImage }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 120, 0);

                    //Fill the text boxes with application details
                    tb_AddAppName.Text = vEditAppDataBind.Name;
                    tb_AddAppExePath.Text = vEditAppDataBind.PathExe;
                    tb_AddAppPathLaunch.Text = vEditAppDataBind.PathLaunch;
                    tb_AddAppPathRoms.Text = vEditAppDataBind.PathRoms;
                    tb_AddAppArgument.Text = vEditAppDataBind.Argument;
                    checkbox_AddLaunchFilePicker.IsChecked = vEditAppDataBind.LaunchFilePicker;
                    checkbox_AddLaunchKeyboard.IsChecked = vEditAppDataBind.LaunchKeyboard;

                    //Hide and show situation based settings
                    if (vEditAppDataBind.Type == ProcessType.UWP)
                    {
                        sp_AddAppExePath.Visibility = Visibility.Collapsed;
                        sp_AddAppPathLaunch.Visibility = Visibility.Collapsed;
                        sp_AddAppPathRoms.Visibility = Visibility.Collapsed;
                        checkbox_AddLaunchFilePicker.Visibility = Visibility.Collapsed;
                        sp_AddAppArgument.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        sp_AddAppExePath.Visibility = Visibility.Visible;
                        sp_AddAppPathLaunch.Visibility = Visibility.Visible;
                        if (vEditAppDataBind.Category == AppCategory.Emulator) { sp_AddAppPathRoms.Visibility = Visibility.Visible; } else { sp_AddAppPathRoms.Visibility = Visibility.Collapsed; }
                        if (vEditAppDataBind.Category == AppCategory.App) { checkbox_AddLaunchFilePicker.Visibility = Visibility.Visible; } else { checkbox_AddLaunchFilePicker.Visibility = Visibility.Collapsed; }
                        sp_AddAppArgument.Visibility = Visibility.Visible;
                    }

                    //Show the manage popup
                    await Popup_Show(grid_Popup_Manage, btn_Manage_SaveEditApp);
                }
            }
            catch { }
        }

        //Show the exe application add popup
        async Task Popup_Show_AddExe()
        {
            try
            {
                //Reset the current edit application
                vEditAppDataBind = null;

                grid_Popup_Manage_txt_Title.Text = "Add application";
                btn_AddAppLogo.IsEnabled = false;
                btn_Manage_ResetAppLogo.IsEnabled = false;
                tb_AddAppName.IsEnabled = false;
                tb_AddAppExePath.IsEnabled = false;
                tb_AddAppPathLaunch.IsEnabled = false;
                btn_AddAppPathLaunch.IsEnabled = false;
                tb_AddAppPathRoms.IsEnabled = false;
                btn_Manage_SaveEditApp.Content = "Add the application as filled in above";
                grid_EditMoveApp.Visibility = Visibility.Collapsed;

                //Load the application categories
                List<DataBindString> listAppCategories = new List<DataBindString>();

                DataBindString categoryApp = new DataBindString();
                categoryApp.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/App.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                categoryApp.Name = "App & Media";
                listAppCategories.Add(categoryApp);

                DataBindString categoryGame = new DataBindString();
                categoryGame.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Game.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                categoryGame.Name = "Game";
                listAppCategories.Add(categoryGame);

                DataBindString categoryEmulator = new DataBindString();
                categoryEmulator.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Emulator.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                categoryEmulator.Name = "Emulator";
                listAppCategories.Add(categoryEmulator);

                lb_Manage_AddAppCategory.ItemsSource = listAppCategories;

                //Select current application category
                lb_Manage_AddAppCategory.SelectedIndex = 1;

                //Load application image
                img_AddAppLogo.Source = FileToBitmapImage(new string[] { "Assets/Apps/Unknown.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 120, 0);

                //Fill the text boxes with application details
                tb_AddAppName.Text = "Select application executable file first";
                tb_AddAppExePath.Text = string.Empty;
                tb_AddAppPathLaunch.Text = string.Empty;
                tb_AddAppPathRoms.Text = string.Empty;
                tb_AddAppArgument.Text = string.Empty;
                checkbox_AddLaunchFilePicker.IsChecked = false;
                checkbox_AddLaunchKeyboard.IsChecked = false;

                //Hide and show situation based settings
                sp_AddAppExePath.Visibility = Visibility.Visible;
                sp_AddAppPathLaunch.Visibility = Visibility.Visible;
                sp_AddAppPathRoms.Visibility = Visibility.Collapsed;
                checkbox_AddLaunchFilePicker.Visibility = Visibility.Collapsed;
                sp_AddAppArgument.Visibility = Visibility.Visible;

                //Show the manage popup
                await Popup_Show(grid_Popup_Manage, btn_AddAppExePath);
            }
            catch { }
        }

        //Add Windows store application
        async Task Popup_Show_AddStoreApp()
        {
            try
            {
                //Add application type categories
                List<DataBindString> answersCategory = new List<DataBindString>();

                BitmapImage imageGame = FileToBitmapImage(new string[] { "Assets/Icons/Game.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                DataBindString stringGame = new DataBindString() { Name = "Game", Data1 = "Game", ImageBitmap = imageGame };
                answersCategory.Add(stringGame);

                BitmapImage imageApp = FileToBitmapImage(new string[] { "Assets/Icons/App.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                DataBindString stringApp = new DataBindString() { Name = "App & Media", Data1 = "App", ImageBitmap = imageApp };
                answersCategory.Add(stringApp);

                BitmapImage imageEmulator = FileToBitmapImage(new string[] { "Assets/Icons/Emulator.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                DataBindString stringEmulator = new DataBindString() { Name = "Emulator", Data1 = "Emulator", ImageBitmap = imageEmulator };
                answersCategory.Add(stringEmulator);

                //Show the messagebox
                DataBindString messageResult = await Popup_Show_MessageBox("Application Category", "", "Please select a new application category:", answersCategory);
                if (messageResult != null)
                {
                    //Check the selected application category
                    Enum.TryParse(messageResult.Data1.ToString(), out AppCategory selectedAddCategory);

                    //Select Window Store application
                    vFilePickerFilterIn = new List<string>();
                    vFilePickerFilterOut = new List<string>();
                    vFilePickerTitle = "Window Store Applications";
                    vFilePickerDescription = "Please select a Windows store application to add as " + messageResult.Name + ":";
                    vFilePickerShowNoFile = false;
                    vFilePickerShowRoms = false;
                    vFilePickerShowFiles = false;
                    vFilePickerShowDirectories = false;
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    await Popup_Show_FilePicker("UWP", 0, false, null);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Check if new application already exists
                    if (CombineAppLists(false, false).Any(x => x.Name.ToLower() == vFilePickerResult.Name.ToLower() || x.PathExe.ToLower() == vFilePickerResult.PathFile.ToLower()))
                    {
                        List<DataBindString> answersConfirm = new List<DataBindString>();
                        DataBindString answerAlright = new DataBindString();
                        answerAlright.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Check.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                        answerAlright.Name = "Alright";
                        answersConfirm.Add(answerAlright);

                        await Popup_Show_MessageBox("This application already exists", "", "", answersConfirm);
                        return;
                    }

                    PlayInterfaceSound("Confirm", false);

                    Popup_Show_Status("Plus", "Added " + vFilePickerResult.Name);
                    Debug.WriteLine("Adding UWP app: " + tb_AddAppName.Text + " to the list.");
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.UWP, Category = selectedAddCategory, Name = vFilePickerResult.Name, NameExe = vFilePickerResult.NameExe, PathExe = vFilePickerResult.PathFile, PathImage = vFilePickerResult.PathImage, LaunchKeyboard = (bool)checkbox_AddLaunchKeyboard.IsChecked };
                    await AddAppToList(dataBindApp, true, true);
                }
            }
            catch { }
        }
    }
}