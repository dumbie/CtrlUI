using Microsoft.Win32;
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
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.AppStartupCheck;
using static LibraryShared.Classes;
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
                    dataBindApp.ImageBitmap = FileToBitmapImage(new string[] { dataBindApp.Name, dataBindApp.PathExe, dataBindApp.PathImage }, IntPtr.Zero, 90, 0);
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
                    string imageFileTitle = "Assets\\Apps\\" + dataBindApp.Name + ".png";
                    string imageFileExe = "Assets\\Apps\\" + Path.GetFileNameWithoutExtension(dataBindApp.PathExe) + ".png";

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
                    categoryApp.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1, 0);
                    categoryApp.Name = "App & Media";
                    listAppCategories.Add(categoryApp);

                    DataBindString categoryGame = new DataBindString();
                    categoryGame.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Game.png" }, IntPtr.Zero, -1, 0);
                    categoryGame.Name = "Game";
                    listAppCategories.Add(categoryGame);

                    DataBindString categoryEmulator = new DataBindString();
                    categoryEmulator.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Emulator.png" }, IntPtr.Zero, -1, 0);
                    categoryEmulator.Name = "Emulator";
                    listAppCategories.Add(categoryEmulator);

                    lb_Manage_AddAppCategory.ItemsSource = listAppCategories;

                    //Select current application category
                    lb_Manage_AddAppCategory.SelectedIndex = (int)vEditAppDataBind.Category;

                    //Load application image
                    img_AddAppLogo.Source = FileToBitmapImage(new string[] { vEditAppDataBind.Name, vEditAppDataBind.PathExe, vEditAppDataBind.PathImage }, IntPtr.Zero, 120, 0);

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
                    await Popup_Show(grid_Popup_Manage, btn_Manage_SaveEditApp, true);
                }
            }
            catch { }
        }

        //Update manage interface according to category
        void Lb_Manage_AddAppCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //Get the currently selected category
                AppCategory selectedAddCategory = (AppCategory)lb_Manage_AddAppCategory.SelectedIndex;

                //Check if the application is UWP
                bool UwpApplication = sp_AddAppExePath.Visibility == Visibility.Collapsed;

                if (UwpApplication || selectedAddCategory != AppCategory.Emulator)
                {
                    sp_AddAppPathRoms.Visibility = Visibility.Collapsed;
                }
                else
                {
                    sp_AddAppPathRoms.Visibility = Visibility.Visible;
                }

                if (UwpApplication || selectedAddCategory != AppCategory.App)
                {
                    checkbox_AddLaunchFilePicker.Visibility = Visibility.Collapsed;
                }
                else
                {
                    checkbox_AddLaunchFilePicker.Visibility = Visibility.Visible;
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
                categoryApp.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1, 0);
                categoryApp.Name = "App & Media";
                listAppCategories.Add(categoryApp);

                DataBindString categoryGame = new DataBindString();
                categoryGame.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Game.png" }, IntPtr.Zero, -1, 0);
                categoryGame.Name = "Game";
                listAppCategories.Add(categoryGame);

                DataBindString categoryEmulator = new DataBindString();
                categoryEmulator.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Emulator.png" }, IntPtr.Zero, -1, 0);
                categoryEmulator.Name = "Emulator";
                listAppCategories.Add(categoryEmulator);

                lb_Manage_AddAppCategory.ItemsSource = listAppCategories;

                //Select current application category
                lb_Manage_AddAppCategory.SelectedIndex = 1;

                //Load application image
                img_AddAppLogo.Source = FileToBitmapImage(new string[] { "Assets\\Apps\\Unknown.png" }, IntPtr.Zero, 120, 0);

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
                await Popup_Show(grid_Popup_Manage, btn_Manage_SaveEditApp, true);
            }
            catch { }
        }

        //Reset the application image
        void Button_Manage_ResetAppLogo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (vEditAppDataBind != null)
                {
                    string imageFileTitle = "Assets\\Apps\\" + vEditAppDataBind.Name + ".png";
                    string imageFileExe = "Assets\\Apps\\" + Path.GetFileNameWithoutExtension(vEditAppDataBind.PathExe) + ".png";

                    //Check the application image
                    bool defaultImage = AssetsAppsFiles.Contains(imageFileTitle) || AssetsAppsFiles.Contains(imageFileExe);
                    if (defaultImage)
                    {
                        Popup_Show_Status("Close", "Cannot reset image");
                        Debug.WriteLine("Default application images cannot be reset.");
                        return;
                    }

                    //Remove application image files
                    File_Delete(imageFileTitle);
                    File_Delete(imageFileExe);

                    //Reload the application image
                    BitmapImage applicationImage = FileToBitmapImage(new string[] { vEditAppDataBind.Name, vEditAppDataBind.PathExe, vEditAppDataBind.PathImage }, IntPtr.Zero, 120, 0);
                    img_AddAppLogo.Source = applicationImage;
                    vEditAppDataBind.ImageBitmap = applicationImage;

                    Popup_Show_Status("Restart", "App image reset");
                    Debug.WriteLine("App image reset: " + vEditAppDataBind.Name);
                }
                else
                {
                    string imageFileTitle = "Assets\\Apps\\" + tb_AddAppName.Text + ".png";
                    string imageFileExe = "Assets\\Apps\\" + Path.GetFileNameWithoutExtension(tb_AddAppExePath.Text) + ".png";

                    //Check the application image
                    bool defaultImage = AssetsAppsFiles.Contains(imageFileTitle) || AssetsAppsFiles.Contains(imageFileExe);
                    if (defaultImage)
                    {
                        Popup_Show_Status("Close", "Cannot reset image");
                        Debug.WriteLine("Default application images cannot be reset.");
                        return;
                    }

                    //Remove application image files
                    File_Delete(imageFileTitle);
                    File_Delete(imageFileExe);

                    //Reload the application image
                    BitmapImage applicationImage = FileToBitmapImage(new string[] { tb_AddAppName.Text, tb_AddAppExePath.Text }, IntPtr.Zero, 120, 0);
                    img_AddAppLogo.Source = applicationImage;
                }
            }
            catch { }
        }

        //Add or edit application to list apps and Json file
        async void Button_Manage_SaveEditApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Check the selected application category
                AppCategory selectedAddCategory = (AppCategory)lb_Manage_AddAppCategory.SelectedIndex;

                //Check if there is an application name set
                if (string.IsNullOrWhiteSpace(tb_AddAppName.Text) || tb_AddAppName.Text == "Select application executable file first")
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Please enter an application name first", "", "", Answers);
                    return;
                }

                //Check if there is an application exe set
                if (string.IsNullOrWhiteSpace(tb_AddAppExePath.Text))
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Please select an application executable file first", "", "", Answers);
                    return;
                }

                //Prevent CtrlUI from been added to the list
                if (tb_AddAppExePath.Text.Contains("CtrlUI.exe") || tb_AddAppExePath.Text.Contains("CtrlUI-Admin.exe"))
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Sorry, you can't add CtrlUI as an application", "", "", Answers);
                    return;
                }

                //Check if the application paths exist for Win32 apps
                if (vEditAppDataBind != null && vEditAppDataBind.Type == ProcessType.Win32)
                {
                    //Validate the launch target
                    if (!File.Exists(tb_AddAppExePath.Text))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                        Answer1.Name = "Alright";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("Application exe not found, please select another one", "", "", Answers);
                        return;
                    }

                    //Validate the launch path
                    if (!string.IsNullOrWhiteSpace(tb_AddAppPathLaunch.Text) && !Directory.Exists(tb_AddAppPathLaunch.Text))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                        Answer1.Name = "Alright";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("Launch folder not found, please select another one", "", "", Answers);
                        return;
                    }

                    //Check if application is emulator and validate the rom path
                    if (selectedAddCategory == AppCategory.Emulator)
                    {
                        if (string.IsNullOrWhiteSpace(tb_AddAppPathRoms.Text))
                        {
                            List<DataBindString> Answers = new List<DataBindString>();
                            DataBindString Answer1 = new DataBindString();
                            Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                            Answer1.Name = "Alright";
                            Answers.Add(Answer1);

                            await Popup_Show_MessageBox("Please select an emulator rom folder first", "", "", Answers);
                            return;
                        }
                        if (!Directory.Exists(tb_AddAppPathRoms.Text))
                        {
                            List<DataBindString> Answers = new List<DataBindString>();
                            DataBindString Answer1 = new DataBindString();
                            Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                            Answer1.Name = "Alright";
                            Answers.Add(Answer1);

                            await Popup_Show_MessageBox("Rom folder not found, please select another one", "", "", Answers);
                            return;
                        }
                    }
                }

                //Check if application needs to be edited or added
                string BtnTextContent = (sender as Button).Content.ToString();
                if (BtnTextContent == "Add the application as filled in above")
                {
                    //Check if new application already exists
                    if (CombineAppLists(false, false).Any(x => x.Name.ToLower() == tb_AddAppName.Text.ToLower() || x.PathExe.ToLower() == tb_AddAppExePath.Text.ToLower()))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                        Answer1.Name = "Alright";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("This application already exists", "", "", Answers);
                        return;
                    }

                    PlayInterfaceSound("Confirm", false);

                    Popup_Show_Status("Plus", "Added " + tb_AddAppName.Text);
                    Debug.WriteLine("Adding Win32 app: " + tb_AddAppName.Text + " to the list.");
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = selectedAddCategory, Name = tb_AddAppName.Text, PathExe = tb_AddAppExePath.Text, PathLaunch = tb_AddAppPathLaunch.Text, PathRoms = tb_AddAppPathRoms.Text, Argument = tb_AddAppArgument.Text, LaunchFilePicker = (bool)checkbox_AddLaunchFilePicker.IsChecked, LaunchKeyboard = (bool)checkbox_AddLaunchKeyboard.IsChecked };
                    await AddAppToList(dataBindApp, true, true);

                    //Close the open popup
                    await Popup_Close_Top();

                    //Focus on the application list
                    if (selectedAddCategory == AppCategory.Game)
                    {
                        await ListboxFocusIndex(lb_Games, false, true, -1);
                    }
                    else if (selectedAddCategory == AppCategory.App)
                    {
                        await ListboxFocusIndex(lb_Apps, false, true, -1);
                    }
                    else if (selectedAddCategory == AppCategory.Emulator)
                    {
                        await ListboxFocusIndex(lb_Emulators, false, true, -1);
                    }
                }
                else
                {
                    //Check if application name already exists
                    if (vEditAppDataBind.Name.ToLower() == tb_AddAppName.Text.ToLower())
                    {
                        Debug.WriteLine("Application name has not changed or just caps.");
                    }
                    else if (CombineAppLists(false, false).Any(x => x.Name.ToLower() == tb_AddAppName.Text.ToLower()))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                        Answer1.Name = "Alright";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("This application name already exists, please use another one", "", "", Answers);
                        return;
                    }

                    //Check if application executable already exists
                    if (vEditAppDataBind.PathExe == tb_AddAppExePath.Text)
                    {
                        Debug.WriteLine("Application executable has not changed.");
                    }
                    else if (CombineAppLists(false, false).Any(x => x.PathExe.ToLower() == tb_AddAppExePath.Text.ToLower()))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                        Answer1.Name = "Alright";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("This application executable already exists, please select another one", "", "", Answers);
                        return;
                    }

                    //Rename application logo based on name and reload it
                    string imageFileNameOld = "Assets\\Apps\\" + vEditAppDataBind.Name + ".png";
                    if (vEditAppDataBind.Name != tb_AddAppName.Text && File.Exists(imageFileNameOld))
                    {
                        Debug.WriteLine("App name changed and application logo file exists.");
                        string imageFileNameNew = "Assets\\Apps\\" + tb_AddAppName.Text + ".png";

                        //Check the application image
                        bool defaultImage = AssetsAppsFiles.Contains(imageFileNameOld);
                        if (defaultImage)
                        {
                            Debug.WriteLine("Default application images cannot be renamed.");
                            File_Copy(imageFileNameOld, imageFileNameNew, true);
                        }
                        else
                        {
                            File_Move(imageFileNameOld, imageFileNameNew, true);
                        }
                    }

                    //Edit application in the list
                    vEditAppDataBind.Category = selectedAddCategory;
                    vEditAppDataBind.Name = tb_AddAppName.Text;
                    vEditAppDataBind.PathExe = tb_AddAppExePath.Text;
                    vEditAppDataBind.PathLaunch = tb_AddAppPathLaunch.Text;
                    vEditAppDataBind.PathRoms = tb_AddAppPathRoms.Text;
                    vEditAppDataBind.Argument = tb_AddAppArgument.Text;
                    vEditAppDataBind.LaunchFilePicker = (bool)checkbox_AddLaunchFilePicker.IsChecked;
                    vEditAppDataBind.LaunchKeyboard = (bool)checkbox_AddLaunchKeyboard.IsChecked;
                    vEditAppDataBind.StatusAvailable = Visibility.Collapsed;
                    vEditAppDataBind.ImageBitmap = FileToBitmapImage(new string[] { vEditAppDataBind.Name, vEditAppDataBind.PathExe, vEditAppDataBind.PathImage }, IntPtr.Zero, 90, 0);

                    Popup_Show_Status("Edit", "Edited " + vEditAppDataBind.Name);
                    Debug.WriteLine("Editing application: " + vEditAppDataBind.Name + " in the list.");

                    //Save changes to Json file
                    JsonSaveApplications();

                    //Close the open popup
                    await Popup_Close_Top();
                    await Task.Delay(500);

                    //Focus on the application list
                    if (vEditAppCategoryPrevious != vEditAppDataBind.Category)
                    {
                        Debug.WriteLine("App category changed to: " + vEditAppDataBind.Category);

                        //Remove app from previous category
                        if (vEditAppCategoryPrevious == AppCategory.Game)
                        {
                            await ListBoxRemoveItem(lb_Games, List_Games, vEditAppDataBind, false);
                        }
                        else if (vEditAppCategoryPrevious == AppCategory.App)
                        {
                            await ListBoxRemoveItem(lb_Apps, List_Apps, vEditAppDataBind, false);
                        }
                        else if (vEditAppCategoryPrevious == AppCategory.Emulator)
                        {
                            await ListBoxRemoveItem(lb_Emulators, List_Emulators, vEditAppDataBind, false);
                        }

                        //Add application to new category
                        if (vEditAppDataBind.Category == AppCategory.Game)
                        {
                            await ListBoxAddItem(lb_Games, List_Games, vEditAppDataBind, false, false);
                        }
                        else if (vEditAppDataBind.Category == AppCategory.App)
                        {
                            await ListBoxAddItem(lb_Apps, List_Apps, vEditAppDataBind, false, false);
                        }
                        else if (vEditAppDataBind.Category == AppCategory.Emulator)
                        {
                            await ListBoxAddItem(lb_Emulators, List_Emulators, vEditAppDataBind, false, false);
                        }

                        //Focus on the edited item listbox
                        if (vSearchOpen)
                        {
                            await ListboxFocusIndex(lb_Search, false, false, -1);
                        }
                        else
                        {
                            if (vEditAppDataBind.Category == AppCategory.Game) { await ListboxFocusIndex(lb_Games, false, true, -1); }
                            else if (vEditAppDataBind.Category == AppCategory.App) { await ListboxFocusIndex(lb_Apps, false, true, -1); }
                            else if (vEditAppDataBind.Category == AppCategory.Emulator) { await ListboxFocusIndex(lb_Emulators, false, true, -1); }
                        }
                    }
                    else
                    {
                        //Focus on the item listbox
                        if (vSearchOpen)
                        {
                            await ListboxFocusIndex(lb_Search, false, false, -1);
                        }
                        else
                        {
                            if (vEditAppDataBind.Category == AppCategory.Game) { await ListboxFocusIndex(lb_Games, false, false, -1); }
                            else if (vEditAppDataBind.Category == AppCategory.App) { await ListboxFocusIndex(lb_Apps, false, false, -1); }
                            else if (vEditAppDataBind.Category == AppCategory.Emulator) { await ListboxFocusIndex(lb_Emulators, false, false, -1); }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("App edit failed: " + ex.Message);
            }
        }

        void Btn_Manage_MoveAppRight_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MoveApplicationList_Right();
            }
            catch { }
        }

        void Btn_Manage_MoveAppLeft_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MoveApplicationList_Left();
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

                BitmapImage imageGame = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Game.png" }, IntPtr.Zero, -1, 0);
                DataBindString stringGame = new DataBindString() { Name = "Game", Data1 = "Game", ImageBitmap = imageGame };
                answersCategory.Add(stringGame);

                BitmapImage imageApp = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1, 0);
                DataBindString stringApp = new DataBindString() { Name = "App & Media", Data1 = "App", ImageBitmap = imageApp };
                answersCategory.Add(stringApp);

                BitmapImage imageEmulator = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Emulator.png" }, IntPtr.Zero, -1, 0);
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
                        answerAlright.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
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

        //Add first launch applications automatically
        async Task AddFirstLaunchApps()
        {
            try
            {
                //Set application first launch to false
                SettingSave("AppFirstLaunch", "False");

                //Open the Windows registry
                RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);

                //Search for Microsoft Edge install and add to the list
                //DataBindApp dataBindAppEdge = new DataBindApp() { Type = ProcessType.UWP, Category = AppCategory.App, Name = "Microsoft Edge", NameExe = "MicrosoftEdge.exe", PathExe = "Microsoft.MicrosoftEdge_8wekyb3d8bbwe!MicrosoftEdge", LaunchKeyboard = true };
                //await AddAppToList(dataBindAppEdge, true, true);
                using (RegistryKey RegKeyEdge = registryKeyLocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\App Paths\\msedge.exe"))
                {
                    if (RegKeyEdge != null)
                    {
                        string RegKeyExePath = RegKeyEdge.GetValue(null).ToString();
                        if (File.Exists(RegKeyExePath))
                        {
                            //Add application to the list
                            DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Microsoft Edge", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath), LaunchKeyboard = true };
                            await AddAppToList(dataBindApp, true, true);

                            //Disable the icon after selection
                            grid_Popup_Welcome_button_Edge.IsEnabled = false;
                            grid_Popup_Welcome_button_Edge.Opacity = 0.40;
                        }
                    }
                }

                //Search for Spotify install and add to the list
                string SpotifyExePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Spotify\\Spotify.exe";
                if (File.Exists(SpotifyExePath))
                {
                    //Add application to the list
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Spotify", PathExe = SpotifyExePath, PathLaunch = Path.GetDirectoryName(SpotifyExePath) };
                    await AddAppToList(dataBindApp, true, true);

                    //Disable the icon after selection
                    grid_Popup_Welcome_button_Spotify.IsEnabled = false;
                    grid_Popup_Welcome_button_Spotify.Opacity = 0.40;
                }

                //Search for Kodi install and add to the list
                using (RegistryKey RegKeyKodi = registryKeyCurrentUser.OpenSubKey("Software\\Kodi"))
                {
                    if (RegKeyKodi != null)
                    {
                        string RegKeyExePath = RegKeyKodi.GetValue(null).ToString() + "\\Kodi.exe";
                        if (File.Exists(RegKeyExePath))
                        {
                            //Add application to the list
                            DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Kodi", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath) };
                            await AddAppToList(dataBindApp, true, true);

                            //Disable the icon after selection
                            grid_Popup_Welcome_button_Kodi.IsEnabled = false;
                            grid_Popup_Welcome_button_Kodi.Opacity = 0.40;
                        }
                    }
                }

                //Add Xbox uwp application to the list
                DataBindApp dataBindAppXbox = new DataBindApp() { Type = ProcessType.UWP, Category = AppCategory.App, Name = "Xbox", NameExe = "XboxApp.exe", PathExe = "Microsoft.XboxApp_8wekyb3d8bbwe!Microsoft.XboxApp" };
                await AddAppToList(dataBindAppXbox, true, true);

                //Search for PS4 Remote Play install and add to the list
                using (RegistryKey RegKeyPS4Remote = registryKeyLocalMachine.OpenSubKey("SOFTWARE\\Sony Corporation\\PS4 Remote Play"))
                {
                    if (RegKeyPS4Remote != null)
                    {
                        string RegKeyExePath = RegKeyPS4Remote.GetValue("Path").ToString() + "\\RemotePlay.exe";
                        if (File.Exists(RegKeyExePath))
                        {
                            //Add application to the list
                            DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Remote Play", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath) };
                            await AddAppToList(dataBindApp, true, true);

                            //Disable the icon after selection
                            grid_Popup_Welcome_button_PS4Remote.IsEnabled = false;
                            grid_Popup_Welcome_button_PS4Remote.Opacity = 0.40;
                        }
                    }
                }

                //Search for Steam install and add to the list
                using (RegistryKey RegKeySteam = registryKeyCurrentUser.OpenSubKey("Software\\Valve\\Steam"))
                {
                    if (RegKeySteam != null)
                    {
                        string RegKeyExePath = RegKeySteam.GetValue("SteamExe").ToString();
                        if (File.Exists(RegKeyExePath))
                        {
                            //Add application to the list
                            DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Steam", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath), Argument = "-bigpicture", QuickLaunch = true };
                            await AddAppToList(dataBindApp, true, true);

                            //Disable the icon after selection
                            grid_Popup_Welcome_button_Steam.IsEnabled = false;
                            grid_Popup_Welcome_button_Steam.Opacity = 0.40;
                        }
                    }
                }

                //Search for GoG install and add to the list
                using (RegistryKey RegKeyGoG = registryKeyLocalMachine.OpenSubKey("Software\\GOG.com\\GalaxyClient\\paths"))
                {
                    if (RegKeyGoG != null)
                    {
                        string RegKeyExePath = RegKeyGoG.GetValue("client").ToString() + "\\GalaxyClient.exe";
                        if (File.Exists(RegKeyExePath))
                        {
                            //Add application to the list
                            DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "GoG", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath) };
                            await AddAppToList(dataBindApp, true, true);

                            //Disable the icon after selection
                            grid_Popup_Welcome_button_GoG.IsEnabled = false;
                            grid_Popup_Welcome_button_GoG.Opacity = 0.40;
                        }
                    }
                }

                //Search for Origin install and add to the list
                using (RegistryKey RegKeyOrigin = registryKeyLocalMachine.OpenSubKey("Software\\Origin"))
                {
                    if (RegKeyOrigin != null)
                    {
                        string RegKeyExePath = RegKeyOrigin.GetValue("ClientPath").ToString();
                        if (File.Exists(RegKeyExePath))
                        {
                            //Add application to the list
                            DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Origin", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath) };
                            await AddAppToList(dataBindApp, true, true);

                            //Disable the icon after selection
                            grid_Popup_Welcome_button_Origin.IsEnabled = false;
                            grid_Popup_Welcome_button_Origin.Opacity = 0.40;
                        }
                    }
                }

                //Search for Uplay install and add to the list
                using (RegistryKey RegKeyUplay = registryKeyLocalMachine.OpenSubKey("Software\\Ubisoft\\Launcher"))
                {
                    if (RegKeyUplay != null)
                    {
                        string RegKeyExePath = RegKeyUplay.GetValue("InstallDir").ToString() + "upc.exe";
                        if (File.Exists(RegKeyExePath))
                        {
                            //Add application to the list
                            DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Uplay", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath) };
                            await AddAppToList(dataBindApp, true, true);

                            //Disable the icon after selection
                            grid_Popup_Welcome_button_Uplay.IsEnabled = false;
                            grid_Popup_Welcome_button_Uplay.Opacity = 0.40;
                        }
                    }
                }

                //Search for Battle.net install and add to the list
                using (RegistryKey RegKeyBattle = registryKeyLocalMachine.OpenSubKey("Software\\Blizzard Entertainment\\Battle.net\\Capabilities"))
                {
                    if (RegKeyBattle != null)
                    {
                        string RegKeyExePath = RegKeyBattle.GetValue("ApplicationIcon").ToString().Replace("\"", "").Replace(",0", "");
                        if (File.Exists(RegKeyExePath))
                        {
                            //Add application to the list
                            DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Battle.net", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath) };
                            await AddAppToList(dataBindApp, true, true);

                            //Disable the icon after selection
                            grid_Popup_Welcome_button_Battle.IsEnabled = false;
                            grid_Popup_Welcome_button_Battle.Opacity = 0.40;
                        }
                    }
                }

                //Close and dispose the registry
                registryKeyLocalMachine.Dispose();
                registryKeyCurrentUser.Dispose();

                //Show the welcome screen popup
                await Popup_Show(grid_Popup_Welcome, grid_Popup_Welcome_button_LaunchDirectXInput, false);
            }
            catch { }
        }
    }
}