using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Add application to the specific list
        void AddAppToList(DataBindApp AddApp, bool GenerateAppNumber, bool LoadImage)
        {
            try
            {
                //Generate application number
                if (GenerateAppNumber)
                {
                    AddApp.Number = GetHighestAppNumber();
                }

                //Check if the exe file is available
                if (AddApp.Type != "UWP" && !File.Exists(AddApp.PathExe))
                {
                    AddApp.StatusAvailable = Visibility.Visible;
                }

                //Check if the rom folder is available
                if (AddApp.Category == "Emulator" && !Directory.Exists(AddApp.PathRoms))
                {
                    AddApp.StatusAvailable = Visibility.Visible;
                }

                //Check if application is an uwp app
                if (AddApp.Type == "UWP")
                {
                    AddApp.StatusStore = Visibility.Visible;
                }

                //Load and set application image
                if (LoadImage) { AddApp.ImageBitmap = FileToBitmapImage(new string[] { AddApp.Name, AddApp.PathExe, AddApp.PathImage }, IntPtr.Zero, 90); }

                //Add application to the list
                if (AddApp.Category == "Game") { List_Games.Add(AddApp); }
                else if (AddApp.Category == "App") { List_Apps.Add(AddApp); }
                else if (AddApp.Category == "Emulator") { List_Emulators.Add(AddApp); }

                //Save changes to Json file
                if (GenerateAppNumber) { JsonSaveApps(); }
            }
            catch { }
        }

        //Remove application from the list
        async Task RemoveAppFromList(DataBindApp RemoveApp, bool SaveJson, bool RemoveImageFile, bool Silent)
        {
            try
            {
                int ListboxSelectedIndex = -1;
                ListBox ListBoxSender = null;

                //Delete application from the lists
                if (RemoveApp.Category == "Game")
                {
                    ListBoxSender = lb_Games;
                    ListboxSelectedIndex = ListBoxSender.SelectedIndex;
                    List_Games.Remove(RemoveApp);
                }
                else if (RemoveApp.Category == "App")
                {
                    ListBoxSender = lb_Apps;
                    ListboxSelectedIndex = ListBoxSender.SelectedIndex;
                    List_Apps.Remove(RemoveApp);
                }
                else if (RemoveApp.Category == "Emulator")
                {
                    ListBoxSender = lb_Emulators;
                    ListboxSelectedIndex = ListBoxSender.SelectedIndex;
                    List_Emulators.Remove(RemoveApp);
                }
                else if (RemoveApp.Category == "Process")
                {
                    ListBoxSender = lb_Processes;
                    ListboxSelectedIndex = ListBoxSender.SelectedIndex;
                    List_Processes.Remove(RemoveApp);
                }
                else if (RemoveApp.Category == "Shortcut")
                {
                    ListBoxSender = lb_Shortcuts;
                    ListboxSelectedIndex = ListBoxSender.SelectedIndex;
                    List_Shortcuts.Remove(RemoveApp);
                }

                //Delete application from search results
                if (vSearchOpen)
                {
                    ListBoxSender = lb_Search;
                    ListboxSelectedIndex = ListBoxSender.SelectedIndex;
                    List_Search.Remove(RemoveApp);
                }

                //Refresh the application lists
                await RefreshApplicationLists(true, true, false, false);

                //Select the previous index
                await FocusOnListbox(ListBoxSender, false, false, ListboxSelectedIndex);

                //Save changes to Json file
                if (SaveJson)
                {
                    JsonSaveApps();
                }

                //Remove application image files
                if (RemoveImageFile)
                {
                    if (File.Exists("Assets\\Apps\\" + RemoveApp.Name + ".png")) { File.Delete("Assets\\Apps\\" + RemoveApp.Name + ".png"); }
                    if (File.Exists("Assets\\Apps\\" + Path.GetFileNameWithoutExtension(RemoveApp.PathExe) + ".png")) { File.Delete("Assets\\Apps\\" + Path.GetFileNameWithoutExtension(RemoveApp.PathExe) + ".png"); }
                }

                //Show removed notification
                if (!Silent)
                {
                    Popup_Show_Status("Minus", "Removed " + RemoveApp.Name);
                    Debug.WriteLine("Removed application: " + RemoveApp.Name);
                }
            }
            catch { }
        }

        //Show the application edit popup
        async Task Popup_Show_AppEdit(ListBox ListBox)
        {
            try
            {
                int ListboxSelectedIndex = ListBox.SelectedIndex;
                if (ListBox.SelectedItems.Count > 0 && ListboxSelectedIndex != -1)
                {
                    DataBindApp EditApp = (DataBindApp)ListBox.SelectedItem;

                    grid_Popup_Manage_txt_Title.Text = "Edit application";
                    btn_AddAppLogo.IsEnabled = true;
                    tb_AddAppName.IsEnabled = true;
                    tb_AddAppExePath.IsEnabled = true;
                    tb_AddAppPathLaunch.IsEnabled = true;
                    btn_AddAppPathLaunch.IsEnabled = true;
                    tb_AddAppPathRoms.IsEnabled = true;
                    btn_Manage_SaveEditApp.Content = "Edit the application as filled in above";
                    btn_Manage_AddUwpAdd.Visibility = Visibility.Collapsed;
                    grid_EditMoveApp.Visibility = Visibility.Visible;

                    //Set the variables as current edit app
                    vEditAppListBox = ListBox;
                    vEditAppDataBind = EditApp;
                    vEditAppCategoryPrevious = vEditAppDataBind.Category;

                    //Load the application category
                    if (vEditAppDataBind.Category == "App")
                    {
                        textblock_Manage_AddAppCategory.Text = "App & Media";
                    }
                    else
                    {
                        textblock_Manage_AddAppCategory.Text = vEditAppDataBind.Category;
                    }

                    image_Manage_AddAppCategory.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/" + vEditAppDataBind.Category + ".png" }, IntPtr.Zero, -1);
                    btn_Manage_AddAppCategory.Tag = vEditAppDataBind.Category;

                    //Load application image
                    img_AddAppLogo.Source = FileToBitmapImage(new string[] { vEditAppDataBind.Name, vEditAppDataBind.PathExe, vEditAppDataBind.PathImage }, IntPtr.Zero, 120);

                    //Fill the text boxes with application details
                    tb_AddAppName.Text = vEditAppDataBind.Name;
                    tb_AddAppExePath.Text = vEditAppDataBind.PathExe;
                    tb_AddAppPathLaunch.Text = vEditAppDataBind.PathLaunch;
                    tb_AddAppPathRoms.Text = vEditAppDataBind.PathRoms;
                    tb_AddAppArgument.Text = vEditAppDataBind.Argument;
                    checkbox_AddFilePickerLaunch.IsChecked = vEditAppDataBind.FilePickerLaunch;

                    //Hide and show situation based settings
                    if (vEditAppDataBind.Type == "UWP")
                    {
                        sp_AddAppName.Visibility = Visibility.Collapsed;
                        sp_AddAppExePath.Visibility = Visibility.Collapsed;
                        sp_AddAppPathLaunch.Visibility = Visibility.Collapsed;
                        sp_AddAppPathRoms.Visibility = Visibility.Collapsed;
                        checkbox_AddFilePickerLaunch.Visibility = Visibility.Collapsed;
                        sp_AddAppArgument.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        sp_AddAppName.Visibility = Visibility.Visible;
                        sp_AddAppExePath.Visibility = Visibility.Visible;
                        sp_AddAppPathLaunch.Visibility = Visibility.Visible;
                        if (vEditAppDataBind.Category == "Emulator") { sp_AddAppPathRoms.Visibility = Visibility.Visible; } else { sp_AddAppPathRoms.Visibility = Visibility.Collapsed; }
                        if (vEditAppDataBind.Category == "App") { checkbox_AddFilePickerLaunch.Visibility = Visibility.Visible; } else { checkbox_AddFilePickerLaunch.Visibility = Visibility.Collapsed; }
                        sp_AddAppArgument.Visibility = Visibility.Visible;
                    }

                    //Show the manage popup
                    await Popup_Show(grid_Popup_Manage, btn_Manage_SaveEditApp, true);
                }
            }
            catch { }
        }

        //Show the application drop popup
        async Task Popup_Show_AppDrop(DataBindApp DropApp)
        {
            try
            {
                grid_Popup_Manage_txt_Title.Text = "Dropped application";
                btn_AddAppLogo.IsEnabled = true;
                tb_AddAppName.IsEnabled = true;
                tb_AddAppExePath.IsEnabled = true;
                tb_AddAppPathLaunch.IsEnabled = true;
                btn_AddAppPathLaunch.IsEnabled = true;
                tb_AddAppPathRoms.IsEnabled = false;
                btn_Manage_SaveEditApp.Content = "Add the application as filled in above";
                btn_Manage_AddUwpAdd.Visibility = Visibility.Collapsed;
                grid_EditMoveApp.Visibility = Visibility.Collapsed;

                //Load the application category
                image_Manage_AddAppCategory.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Game.png" }, IntPtr.Zero, -1);
                textblock_Manage_AddAppCategory.Text = "Game";
                btn_Manage_AddAppCategory.Tag = "Game";

                //Load application image
                img_AddAppLogo.Source = FileToBitmapImage(new string[] { DropApp.PathExe }, IntPtr.Zero, 120);

                //Fill the text boxes with application details
                tb_AddAppName.Text = Path.GetFileNameWithoutExtension(DropApp.PathExe);
                tb_AddAppExePath.Text = DropApp.PathExe;
                tb_AddAppPathLaunch.Text = DropApp.PathLaunch;
                tb_AddAppPathRoms.Text = string.Empty;
                tb_AddAppArgument.Text = string.Empty;
                checkbox_AddFilePickerLaunch.IsChecked = false;

                //Hide and show situation based settings
                sp_AddAppName.Visibility = Visibility.Visible;
                sp_AddAppExePath.Visibility = Visibility.Visible;
                sp_AddAppPathLaunch.Visibility = Visibility.Visible;
                sp_AddAppPathRoms.Visibility = Visibility.Collapsed;
                checkbox_AddFilePickerLaunch.Visibility = Visibility.Collapsed;
                sp_AddAppArgument.Visibility = Visibility.Visible;

                //Show the manage popup
                await Popup_Show(grid_Popup_Manage, btn_Manage_SaveEditApp, true);
            }
            catch { }
        }

        //Show the application add popup
        async Task Popup_Show_AppAdd()
        {
            try
            {
                grid_Popup_Manage_txt_Title.Text = "Add application";
                btn_AddAppLogo.IsEnabled = false;
                tb_AddAppName.IsEnabled = false;
                tb_AddAppExePath.IsEnabled = false;
                tb_AddAppPathLaunch.IsEnabled = false;
                btn_AddAppPathLaunch.IsEnabled = false;
                tb_AddAppPathRoms.IsEnabled = false;
                btn_Manage_SaveEditApp.Content = "Add the application as filled in above";
                btn_Manage_AddUwpAdd.Visibility = Visibility.Visible;
                grid_EditMoveApp.Visibility = Visibility.Collapsed;

                //Load the application category
                image_Manage_AddAppCategory.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Game.png" }, IntPtr.Zero, -1);
                textblock_Manage_AddAppCategory.Text = "Game";
                btn_Manage_AddAppCategory.Tag = "Game";

                //Load application image
                img_AddAppLogo.Source = FileToBitmapImage(new string[] { "Assets\\Apps\\Unknown.png" }, IntPtr.Zero, 120);

                //Fill the text boxes with application details
                tb_AddAppName.Text = "Select application executable file first";
                tb_AddAppExePath.Text = string.Empty;
                tb_AddAppPathLaunch.Text = string.Empty;
                tb_AddAppPathRoms.Text = string.Empty;
                tb_AddAppArgument.Text = string.Empty;
                checkbox_AddFilePickerLaunch.IsChecked = false;

                //Hide and show situation based settings
                sp_AddAppName.Visibility = Visibility.Visible;
                sp_AddAppExePath.Visibility = Visibility.Visible;
                sp_AddAppPathLaunch.Visibility = Visibility.Visible;
                sp_AddAppPathRoms.Visibility = Visibility.Collapsed;
                checkbox_AddFilePickerLaunch.Visibility = Visibility.Collapsed;
                sp_AddAppArgument.Visibility = Visibility.Visible;

                //Show the manage popup
                await Popup_Show(grid_Popup_Manage, btn_Manage_SaveEditApp, true);
            }
            catch { }
        }

        //Cancel application editing and close popup
        async void Btn_Manage_Cancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Close the open popup
                await Popup_Close_Top();
            }
            catch { }
        }

        //Add or edit application to list apps and Json file
        async void Button_Manage_SaveEditApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Check the selected application category
                string SelectedAddCategory = btn_Manage_AddAppCategory.Tag.ToString();

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

                //Check if there is an application exe set
                if (string.IsNullOrWhiteSpace(tb_AddAppExePath.Text))
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
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
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Sorry, you can't add CtrlUI as an application", "", "", Answers);
                    return;
                }

                //Check if application is emulator and validate the rom path
                if (SelectedAddCategory == "Emulator")
                {
                    if (string.IsNullOrWhiteSpace(tb_AddAppPathRoms.Text))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                        Answer1.Name = "Alright";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("Please select an emulator rom folder first", "", "", Answers);
                        return;
                    }
                    if (!Directory.Exists(tb_AddAppPathRoms.Text))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                        Answer1.Name = "Alright";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("Rom folder not found, please select another one", "", "", Answers);
                        return;
                    }
                }

                //Check if the application paths exist for non uwp apps
                if (vEditAppDataBind != null && vEditAppDataBind.Type != "UWP")
                {
                    if (!File.Exists(tb_AddAppExePath.Text))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                        Answer1.Name = "Alright";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("Application exe not found, please select another one", "", "", Answers);
                        return;
                    }

                    if (!string.IsNullOrWhiteSpace(tb_AddAppPathLaunch.Text) && !Directory.Exists(tb_AddAppPathLaunch.Text))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                        Answer1.Name = "Alright";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("Launch folder not found, please select another one", "", "", Answers);
                        return;
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
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                        Answer1.Name = "Alright";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("This application already exists, please select another one", "", "", Answers);
                        return;
                    }

                    PlayInterfaceSound("Confirm", false);

                    Popup_Show_Status("Plus", "Added " + tb_AddAppName.Text);
                    Debug.WriteLine("Adding application: " + tb_AddAppName.Text + " to the list.");

                    AddAppToList(new DataBindApp() { Category = SelectedAddCategory, Name = tb_AddAppName.Text, PathExe = tb_AddAppExePath.Text, PathLaunch = tb_AddAppPathLaunch.Text, PathRoms = tb_AddAppPathRoms.Text, Argument = tb_AddAppArgument.Text, FilePickerLaunch = (bool)checkbox_AddFilePickerLaunch.IsChecked }, true, true);

                    //Close the open popup
                    await Popup_Close_Top();

                    //Refresh the application lists
                    await RefreshApplicationLists(true, true, false, false);

                    //Focus on the application list
                    if (SelectedAddCategory == "Game") { await FocusOnListbox(lb_Games, false, true, -1); }
                    else if (SelectedAddCategory == "App") { await FocusOnListbox(lb_Apps, false, true, -1); }
                    else if (SelectedAddCategory == "Emulator") { await FocusOnListbox(lb_Emulators, false, true, -1); }
                }
                else
                {
                    //Check if application name already exists
                    if (vEditAppDataBind.Name == tb_AddAppName.Text)
                    {
                        Debug.WriteLine("Application name has not changed.");
                    }
                    else if (CombineAppLists(false, false).Any(x => x.Name.ToLower() == tb_AddAppName.Text.ToLower()))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
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
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                        Answer1.Name = "Alright";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("This application executable already exists, please select another one", "", "", Answers);
                        return;
                    }

                    //Rename application logo based on name and reload it
                    if (File.Exists("Assets\\Apps\\" + vEditAppDataBind.Name + ".png") && vEditAppDataBind.Name != tb_AddAppName.Text)
                    {
                        Debug.WriteLine("App name changed and application logo file exists so renaming it.");
                        File.Delete("Assets\\Apps\\" + tb_AddAppName.Text + ".png");
                        File.Move("Assets\\Apps\\" + vEditAppDataBind.Name + ".png", "Assets\\Apps\\" + tb_AddAppName.Text + ".png");
                    }

                    //Edit application in the list
                    vEditAppDataBind.Category = SelectedAddCategory;
                    vEditAppDataBind.Name = tb_AddAppName.Text;
                    vEditAppDataBind.PathExe = tb_AddAppExePath.Text;
                    vEditAppDataBind.PathLaunch = tb_AddAppPathLaunch.Text;
                    vEditAppDataBind.PathRoms = tb_AddAppPathRoms.Text;
                    vEditAppDataBind.Argument = tb_AddAppArgument.Text;
                    vEditAppDataBind.FilePickerLaunch = (bool)checkbox_AddFilePickerLaunch.IsChecked;
                    vEditAppDataBind.StatusAvailable = Visibility.Collapsed;
                    vEditAppDataBind.ImageBitmap = FileToBitmapImage(new string[] { vEditAppDataBind.Name, vEditAppDataBind.PathExe, vEditAppDataBind.PathImage }, IntPtr.Zero, 90);

                    Popup_Show_Status("Edit", "Edited " + vEditAppDataBind.Name);
                    Debug.WriteLine("Editing application: " + vEditAppDataBind.Name + " in the list.");

                    //Save changes to Json file
                    JsonSaveApps();

                    //Close the open popup
                    await Popup_Close_Top();
                    await Task.Delay(500);

                    //Focus on the application list
                    if (vEditAppCategoryPrevious != vEditAppDataBind.Category)
                    {
                        //Add application to new category
                        if (vEditAppDataBind.Category == "Game") { List_Games.Add(vEditAppDataBind); }
                        else if (vEditAppDataBind.Category == "App") { List_Apps.Add(vEditAppDataBind); }
                        else if (vEditAppDataBind.Category == "Emulator") { List_Emulators.Add(vEditAppDataBind); }

                        //Remove the edited application
                        if (vEditAppCategoryPrevious == "Game") { List_Games.Remove(vEditAppDataBind); }
                        else if (vEditAppCategoryPrevious == "App") { List_Apps.Remove(vEditAppDataBind); }
                        else if (vEditAppCategoryPrevious == "Emulator") { List_Emulators.Remove(vEditAppDataBind); }

                        //Focus on the edited item listbox
                        if (vSearchOpen)
                        {
                            await FocusOnListbox(lb_Search, false, false, -1);
                        }
                        else
                        {
                            if (vEditAppDataBind.Category == "Game") { await FocusOnListbox(lb_Games, false, true, -1); }
                            else if (vEditAppDataBind.Category == "App") { await FocusOnListbox(lb_Apps, false, true, -1); }
                            else if (vEditAppDataBind.Category == "Emulator") { await FocusOnListbox(lb_Emulators, false, true, -1); }
                        }

                        ////Sort the lists by number
                        //if (vSortType == "Number")
                        //{
                        //    SortAppLists(true, true);
                        //}
                    }
                    else
                    {
                        //Focus on the item listbox
                        if (vSearchOpen)
                        {
                            await FocusOnListbox(lb_Search, false, false, -1);
                        }
                        else
                        {
                            if (vEditAppDataBind.Category == "Game") { await FocusOnListbox(lb_Games, false, false, -1); }
                            else if (vEditAppDataBind.Category == "App") { await FocusOnListbox(lb_Apps, false, false, -1); }
                            else if (vEditAppDataBind.Category == "Emulator") { await FocusOnListbox(lb_Emulators, false, false, -1); }
                        }

                        ////Sort the lists by number
                        //if (vSortType == "Number")
                        //{
                        //    SortAppLists(true, true);
                        //}
                    }

                    //Refresh the application lists
                    await RefreshApplicationLists(true, true, false, false);
                }
            }
            catch { }
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

        //Add UWP application
        async void Button_Manage_AddUwpAdd_Click(object sender, EventArgs args)
        {
            try
            {
                //Check the selected application category
                string SelectedAddCategoryTag = btn_Manage_AddAppCategory.Tag.ToString();
                string SelectedAddCategoryContent = textblock_Manage_AddAppCategory.Text;
                if (SelectedAddCategoryTag == "Emulator")
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Invalid category for a Windows store app", "", "", Answers);
                    return;
                }

                vFilePickerFilterIn = new string[] { };
                vFilePickerFilterOut = new string[] { };
                vFilePickerTitle = "Window Store Applications";
                vFilePickerDescription = "Please select a Windows store application to add as " + SelectedAddCategoryContent + ":";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = false;
                vFilePickerShowDirectories = false;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("UWP", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Check if uwp application already exists
                if (CombineAppLists(false, false).Any(x => x.Name.ToLower() == vFilePickerResult.Name.ToLower() || x.PathExe.ToLower() == vFilePickerResult.PathFile.ToLower()))
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("This application already exists, please select another one", "", "", Answers);
                    Button_Manage_AddUwpAdd_Click(null, null);
                    return;
                }

                PlayInterfaceSound("Confirm", false);

                Popup_Show_Status("Plus", "Added " + vFilePickerResult.Name);
                Debug.WriteLine("Adding uwp app: " + vFilePickerResult.Name + " Path " + vFilePickerResult.PathFile + " Image " + vFilePickerResult.PathImage);
                AddAppToList(new DataBindApp() { Category = SelectedAddCategoryTag, Name = vFilePickerResult.Name, PathExe = vFilePickerResult.PathFile, PathImage = vFilePickerResult.PathImage, Type = "UWP" }, true, true);
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

                //Add default uwp applications to the list
                AddAppToList(new DataBindApp() { Category = "App", Name = "Xbox", PathExe = "Microsoft.XboxApp_8wekyb3d8bbwe!Microsoft.XboxApp", Type = "UWP" }, true, true);

                //Check for applications in current user registry
                using (RegistryKey RegisteryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    //Search for Kodi install and add to the list
                    using (RegistryKey RegKeyKodi = RegisteryKeyCurrentUser.OpenSubKey("Software\\Kodi"))
                    {
                        if (RegKeyKodi != null)
                        {
                            string RegKeyExePath = RegKeyKodi.GetValue(null).ToString() + "\\Kodi.exe";
                            if (File.Exists(RegKeyExePath))
                            {
                                //Add application to the list
                                AddAppToList(new DataBindApp() { Category = "App", Name = "Kodi", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath) }, true, true);

                                //Disable the icon after selection
                                grid_Popup_Welcome_button_Kodi.IsEnabled = false;
                                grid_Popup_Welcome_button_Kodi.Opacity = 0.30;
                            }
                        }
                    }

                    //Search for Steam install and add to the list
                    using (RegistryKey RegKeySteam = RegisteryKeyCurrentUser.OpenSubKey("Software\\Valve\\Steam"))
                    {
                        if (RegKeySteam != null)
                        {
                            string RegKeyExePath = RegKeySteam.GetValue("SteamExe").ToString();
                            if (File.Exists(RegKeyExePath))
                            {
                                //Add application to the list
                                AddAppToList(new DataBindApp() { Category = "Game", Name = "Steam", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath), Argument = "-bigpicture", QuickLaunch = true }, true, true);

                                //Disable the icon after selection
                                grid_Popup_Welcome_button_Steam.IsEnabled = false;
                                grid_Popup_Welcome_button_Steam.Opacity = 0.30;
                            }
                        }
                    }
                }

                //Check for applications in local machine registry
                using (RegistryKey RegisteryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    //Search for Origin install and add to the list
                    using (RegistryKey RegKeyOrigin = RegisteryKeyLocalMachine.OpenSubKey("Software\\Origin"))
                    {
                        if (RegKeyOrigin != null)
                        {
                            string RegKeyExePath = RegKeyOrigin.GetValue("ClientPath").ToString();
                            if (File.Exists(RegKeyExePath))
                            {
                                //Add application to the list
                                AddAppToList(new DataBindApp() { Category = "Game", Name = "Origin", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath) }, true, true);

                                //Disable the icon after selection
                                grid_Popup_Welcome_button_Origin.IsEnabled = false;
                                grid_Popup_Welcome_button_Origin.Opacity = 0.30;
                            }
                        }
                    }

                    //Search for GoG install and add to the list
                    using (RegistryKey RegKeyGoG = RegisteryKeyLocalMachine.OpenSubKey("Software\\GOG.com\\GalaxyClient\\paths"))
                    {
                        if (RegKeyGoG != null)
                        {
                            string RegKeyExePath = RegKeyGoG.GetValue("client").ToString() + "\\GalaxyClient.exe";
                            if (File.Exists(RegKeyExePath))
                            {
                                //Add application to the list
                                AddAppToList(new DataBindApp() { Category = "Game", Name = "GoG", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath) }, true, true);

                                //Disable the icon after selection
                                grid_Popup_Welcome_button_GoG.IsEnabled = false;
                                grid_Popup_Welcome_button_GoG.Opacity = 0.30;
                            }
                        }
                    }

                    //Search for Uplay install and add to the list
                    using (RegistryKey RegKeyUplay = RegisteryKeyLocalMachine.OpenSubKey("Software\\Ubisoft\\Launcher"))
                    {
                        if (RegKeyUplay != null)
                        {
                            string RegKeyExePath = RegKeyUplay.GetValue("InstallDir").ToString() + "upc.exe";
                            if (File.Exists(RegKeyExePath))
                            {
                                //Add application to the list
                                AddAppToList(new DataBindApp() { Category = "Game", Name = "Uplay", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath) }, true, true);

                                //Disable the icon after selection
                                grid_Popup_Welcome_button_Uplay.IsEnabled = false;
                                grid_Popup_Welcome_button_Uplay.Opacity = 0.30;
                            }
                        }
                    }

                    //Search for Battle.net install and add to the list
                    using (RegistryKey RegKeyBattle = RegisteryKeyLocalMachine.OpenSubKey("Software\\Blizzard Entertainment\\Battle.net\\Capabilities"))
                    {
                        if (RegKeyBattle != null)
                        {
                            string RegKeyExePath = RegKeyBattle.GetValue("ApplicationIcon").ToString().Replace("\"", "").Replace(",0", "");
                            if (File.Exists(RegKeyExePath))
                            {
                                //Add application to the list
                                AddAppToList(new DataBindApp() { Category = "Game", Name = "Battle.net", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath) }, true, true);

                                //Disable the icon after selection
                                grid_Popup_Welcome_button_Battle.IsEnabled = false;
                                grid_Popup_Welcome_button_Battle.Opacity = 0.30;
                            }
                        }
                    }

                    //Search for PS4 Remote Play install and add to the list
                    using (RegistryKey RegKeyPS4Remote = RegisteryKeyLocalMachine.OpenSubKey("SOFTWARE\\Sony Corporation\\PS4 Remote Play"))
                    {
                        if (RegKeyPS4Remote != null)
                        {
                            string RegKeyExePath = RegKeyPS4Remote.GetValue("Path").ToString() + "\\RemotePlay.exe";
                            if (File.Exists(RegKeyExePath))
                            {
                                //Add application to the list
                                AddAppToList(new DataBindApp() { Category = "App", Name = "Remote Play", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath) }, true, true);

                                //Disable the icon after selection
                                grid_Popup_Welcome_button_PS4Remote.IsEnabled = false;
                                grid_Popup_Welcome_button_PS4Remote.Opacity = 0.30;
                            }
                        }
                    }
                }

                //Search for Spotify install and add to the list
                string SpotifyExePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Spotify\\Spotify.exe";
                if (File.Exists(SpotifyExePath))
                {
                    //Add application to the list
                    AddAppToList(new DataBindApp() { Category = "App", Name = "Spotify", PathExe = SpotifyExePath, PathLaunch = Path.GetDirectoryName(SpotifyExePath) }, true, true);

                    //Disable the icon after selection
                    grid_Popup_Welcome_button_Spotify.IsEnabled = false;
                    grid_Popup_Welcome_button_Spotify.Opacity = 0.30;
                }

                //Show the welcome screen popup
                await Popup_Show(grid_Popup_Welcome, grid_Popup_Welcome_button_Kodi, false);
            }
            catch { }
        }
    }
}