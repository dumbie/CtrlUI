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
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.FocusFunctions;
using static LibraryShared.JsonFunctions;
using static LibraryShared.Settings;

namespace CtrlUI
{
    partial class WindowMain
    {
        async void Button_AddAppLogo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Check if there is an application name set
                if (string.IsNullOrWhiteSpace(tb_AddAppName.Text))
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Ok";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Please enter an application name first", "", "", Answers);
                    return;
                }

                //Check if there is an application name set
                if (tb_AddAppName.Text == "Select application executable file first" || tb_AddAppEmulatorName.Text == "Select application executable file first")
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Ok";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Please select an application executable file first", "", "", Answers);
                    return;
                }

                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.FilterIn = new List<string> { "jpg", "png" };
                vFilePickerSettings.Title = "Application Image";
                vFilePickerSettings.Description = "Please select a new application image:";
                vFilePickerSettings.ShowEmulatorImages = true;
                await Popup_Show_FilePicker("PC", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Load the new application image
                BitmapImage applicationImage = FileToBitmapImage(new string[] { vFilePickerResult.PathFile }, null, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);

                //Update the new application image
                img_AddAppLogo.Source = applicationImage;
                if (vEditAppDataBind != null)
                {
                    vEditAppDataBind.ImageBitmap = applicationImage;
                }

                //Copy the new application image
                File_Copy(vFilePickerResult.PathFile, "Assets/User/Apps/" + tb_AddAppName.Text + ".png", true);
            }
            catch { }
        }

        async void Button_AddAppExePath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.FilterIn = new List<string> { "exe" };
                vFilePickerSettings.Title = "Application Executable";
                vFilePickerSettings.Description = "Please select an application executable:";
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
                if ((AppCategory)lb_Manage_AddAppCategory.SelectedIndex == AppCategory.Emulator)
                {
                    tb_AddAppName.Text = string.Empty;
                    tb_AddAppEmulatorName.Text = vFilePickerResult.Name.Replace(".exe", "");
                }
                else
                {
                    tb_AddAppName.Text = vFilePickerResult.Name.Replace(".exe", "");
                    tb_AddAppEmulatorName.Text = string.Empty;
                }

                tb_AddAppName.IsEnabled = true;
                tb_AddAppEmulatorName.IsEnabled = true;

                //Set application image to image preview
                img_AddAppLogo.Source = FileToBitmapImage(new string[] { tb_AddAppName.Text, vFilePickerResult.PathFile }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
                btn_AddAppLogo.IsEnabled = true;
                btn_Manage_ResetAppLogo.IsEnabled = true;
            }
            catch { }
        }

        async void Button_AddAppPathLaunch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.Title = "Launch Folder";
                vFilePickerSettings.Description = "Please select the launch folder:";
                vFilePickerSettings.ShowFiles = false;
                await Popup_Show_FilePicker("PC", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                tb_AddAppPathLaunch.Text = vFilePickerResult.PathFile;
                tb_AddAppPathLaunch.IsEnabled = true;
                btn_AddAppPathLaunch.IsEnabled = true;
            }
            catch { }
        }

        async void Button_AddAppPathRoms_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.Title = "Rom Folder";
                vFilePickerSettings.Description = "Please select the rom folder:";
                vFilePickerSettings.ShowFiles = false;
                await Popup_Show_FilePicker("PC", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                tb_AddAppPathRoms.Text = vFilePickerResult.PathFile;
                tb_AddAppPathRoms.IsEnabled = true;
            }
            catch { }
        }

        //Update manage interface according to category
        void Lb_Manage_AddAppCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ManageInterface_UpdateCategory((AppCategory)lb_Manage_AddAppCategory.SelectedIndex, false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to switch app category: " + ex.Message);
            }
        }

        //Reset the application image
        async void Button_Manage_ResetAppLogo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (vEditAppDataBind != null)
                {
                    //Remove application image files
                    string imageFileTitle = "Assets/User/Apps/" + vEditAppDataBind.Name + ".png";
                    string imageFileExe = "Assets/User/Apps/" + Path.GetFileNameWithoutExtension(vEditAppDataBind.PathExe) + ".png";
                    File_Delete(imageFileTitle);
                    File_Delete(imageFileExe);

                    //Reload the application image
                    BitmapImage applicationImage = FileToBitmapImage(new string[] { vEditAppDataBind.Name, vEditAppDataBind.PathExe, vEditAppDataBind.PathImage }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
                    img_AddAppLogo.Source = applicationImage;
                    vEditAppDataBind.ImageBitmap = applicationImage;

                    await Notification_Send_Status("Restart", "App image reset");
                    Debug.WriteLine("App image reset: " + vEditAppDataBind.Name);
                }
                else
                {
                    //Remove application image files
                    string imageFileTitle = "Assets/User/Apps/" + tb_AddAppName.Text + ".png";
                    string imageFileExe = "Assets/User/Apps/" + Path.GetFileNameWithoutExtension(tb_AddAppExePath.Text) + ".png";
                    File_Delete(imageFileTitle);
                    File_Delete(imageFileExe);

                    //Reload the application image
                    BitmapImage applicationImage = FileToBitmapImage(new string[] { tb_AddAppName.Text, tb_AddAppExePath.Text }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
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
                //Check the selected categories
                AppCategory selectedAppCategory = (AppCategory)lb_Manage_AddAppCategory.SelectedIndex;
                EmulatorCategory selectedEmulatorCategory = (EmulatorCategory)lb_Manage_AddEmulatorCategory.SelectedIndex;

                //Check if there is an application name set
                if (string.IsNullOrWhiteSpace(tb_AddAppName.Text))
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Ok";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Please enter an application name first", "", "", Answers);
                    return;
                }

                //Check if there is an application name set
                if (tb_AddAppName.Text == "Select application executable file first" || tb_AddAppEmulatorName.Text == "Select application executable file first")
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Ok";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Please select an application executable file first", "", "", Answers);
                    return;
                }

                //Check if there is an application exe set
                if (string.IsNullOrWhiteSpace(tb_AddAppExePath.Text))
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Ok";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Please select an application executable file first", "", "", Answers);
                    return;
                }

                //Prevent CtrlUI from been added to the list
                if (tb_AddAppExePath.Text.Contains("CtrlUI.exe") || tb_AddAppExePath.Text.Contains("CtrlUI-Launcher.exe"))
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Ok";
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
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        Answer1.Name = "Ok";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("Application executable not found, please select another one", "", "", Answers);
                        return;
                    }

                    //Validate the launch path
                    if (!string.IsNullOrWhiteSpace(tb_AddAppPathLaunch.Text) && !Directory.Exists(tb_AddAppPathLaunch.Text))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        Answer1.Name = "Ok";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("Launch folder not found, please select another one", "", "", Answers);
                        return;
                    }

                    //Check if application is emulator and validate the rom path
                    if (selectedAppCategory == AppCategory.Emulator && !(bool)checkbox_AddLaunchSkipRom.IsChecked)
                    {
                        if (string.IsNullOrWhiteSpace(tb_AddAppPathRoms.Text))
                        {
                            List<DataBindString> Answers = new List<DataBindString>();
                            DataBindString Answer1 = new DataBindString();
                            Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            Answer1.Name = "Ok";
                            Answers.Add(Answer1);

                            await Popup_Show_MessageBox("Please select an emulator rom folder first", "", "", Answers);
                            return;
                        }
                        if (!Directory.Exists(tb_AddAppPathRoms.Text))
                        {
                            List<DataBindString> Answers = new List<DataBindString>();
                            DataBindString Answer1 = new DataBindString();
                            Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            Answer1.Name = "Ok";
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
                    if (CombineAppLists(false, false, false).Any(x => x.Name.ToLower() == tb_AddAppName.Text.ToLower() || x.PathExe.ToLower() == tb_AddAppExePath.Text.ToLower()))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        Answer1.Name = "Ok";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("This application already exists", "", "", Answers);
                        return;
                    }

                    await Notification_Send_Status("Plus", "Added " + tb_AddAppName.Text);
                    Debug.WriteLine("Adding Win32 app: " + tb_AddAppName.Text + " to the list.");
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = selectedAppCategory, EmulatorCategory = selectedEmulatorCategory, Name = tb_AddAppName.Text, EmulatorName = tb_AddAppEmulatorName.Text, PathExe = tb_AddAppExePath.Text, PathLaunch = tb_AddAppPathLaunch.Text, PathRoms = tb_AddAppPathRoms.Text, Argument = tb_AddAppArgument.Text, NameExe = tb_AddAppNameExe.Text, LaunchFilePicker = (bool)checkbox_AddLaunchFilePicker.IsChecked, LaunchSkipRom = (bool)checkbox_AddLaunchSkipRom.IsChecked, LaunchKeyboard = (bool)checkbox_AddLaunchKeyboard.IsChecked };
                    await AddAppToList(dataBindApp, true, true);

                    //Close the open popup
                    await Popup_Close_Top();

                    //Focus on the application list
                    if (selectedAppCategory == AppCategory.Game)
                    {
                        await ListboxFocusIndex(lb_Games, false, true, -1, vProcessCurrent.MainWindowHandle);
                    }
                    else if (selectedAppCategory == AppCategory.App)
                    {
                        await ListboxFocusIndex(lb_Apps, false, true, -1, vProcessCurrent.MainWindowHandle);
                    }
                    else if (selectedAppCategory == AppCategory.Emulator)
                    {
                        await ListboxFocusIndex(lb_Emulators, false, true, -1, vProcessCurrent.MainWindowHandle);
                    }
                }
                else
                {
                    //Check if application name already exists
                    if (vEditAppDataBind.Name.ToLower() == tb_AddAppName.Text.ToLower())
                    {
                        Debug.WriteLine("Application name has not changed or just caps.");
                    }
                    else if (CombineAppLists(false, false, false).Any(x => x.Name.ToLower() == tb_AddAppName.Text.ToLower()))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        Answer1.Name = "Ok";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("This application name already exists, please use another one", "", "", Answers);
                        return;
                    }

                    //Check if application executable already exists
                    if (vEditAppDataBind.PathExe == tb_AddAppExePath.Text)
                    {
                        Debug.WriteLine("Application executable has not changed.");
                    }
                    else if (CombineAppLists(false, false, false).Any(x => x.PathExe.ToLower() == tb_AddAppExePath.Text.ToLower()))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        Answer1.Name = "Ok";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("This application executable already exists, please select another one", "", "", Answers);
                        return;
                    }

                    //Rename application logo based on name and reload it
                    string imageFileNameOld = "Assets/User/Apps/" + vEditAppDataBind.Name + ".png";
                    if (vEditAppDataBind.Name != tb_AddAppName.Text && File.Exists(imageFileNameOld))
                    {
                        Debug.WriteLine("App name changed and application logo file exists.");
                        string imageFileNameNew = "Assets/User/Apps/" + tb_AddAppName.Text + ".png";
                        File_Move(imageFileNameOld, imageFileNameNew, true);
                    }

                    //Edit application in the list
                    vEditAppDataBind.Category = selectedAppCategory;
                    vEditAppDataBind.EmulatorCategory = selectedEmulatorCategory;
                    vEditAppDataBind.Name = tb_AddAppName.Text;
                    vEditAppDataBind.EmulatorName = tb_AddAppEmulatorName.Text;
                    vEditAppDataBind.PathExe = tb_AddAppExePath.Text;
                    vEditAppDataBind.PathLaunch = tb_AddAppPathLaunch.Text;
                    vEditAppDataBind.PathRoms = tb_AddAppPathRoms.Text;
                    vEditAppDataBind.Argument = tb_AddAppArgument.Text;
                    vEditAppDataBind.NameExe = tb_AddAppNameExe.Text;
                    vEditAppDataBind.LaunchFilePicker = (bool)checkbox_AddLaunchFilePicker.IsChecked;
                    vEditAppDataBind.LaunchSkipRom = (bool)checkbox_AddLaunchSkipRom.IsChecked;
                    vEditAppDataBind.LaunchKeyboard = (bool)checkbox_AddLaunchKeyboard.IsChecked;

                    //Edit images in the list
                    vEditAppDataBind.ImageBitmap = FileToBitmapImage(new string[] { vEditAppDataBind.Name, vEditAppDataBind.PathExe, vEditAppDataBind.PathImage }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
                    if (vEditAppDataBind.EmulatorCategory == EmulatorCategory.Console) { vEditAppDataBind.StatusCategoryImage = vImagePreloadConsole; }
                    else if (vEditAppDataBind.EmulatorCategory == EmulatorCategory.Handheld) { vEditAppDataBind.StatusCategoryImage = vImagePreloadHandheld; }
                    else if (vEditAppDataBind.EmulatorCategory == EmulatorCategory.Computer) { vEditAppDataBind.StatusCategoryImage = vImagePreloadComputer; }
                    else if (vEditAppDataBind.EmulatorCategory == EmulatorCategory.Arcade) { vEditAppDataBind.StatusCategoryImage = vImagePreloadArcade; }
                    else if (vEditAppDataBind.EmulatorCategory == EmulatorCategory.Pinball) { vEditAppDataBind.StatusCategoryImage = vImagePreloadPinball; }
                    else if (vEditAppDataBind.EmulatorCategory == EmulatorCategory.Pong) { vEditAppDataBind.StatusCategoryImage = vImagePreloadPong; }
                    else if (vEditAppDataBind.EmulatorCategory == EmulatorCategory.Chess) { vEditAppDataBind.StatusCategoryImage = vImagePreloadChess; }
                    else if (vEditAppDataBind.EmulatorCategory == EmulatorCategory.VirtualReality) { vEditAppDataBind.StatusCategoryImage = vImagePreloadVirtualReality; }
                    else if (vEditAppDataBind.EmulatorCategory == EmulatorCategory.OperatingSystem) { vEditAppDataBind.StatusCategoryImage = vImagePreloadOperatingSystem; }
                    else if (vEditAppDataBind.EmulatorCategory == EmulatorCategory.Other) { vEditAppDataBind.StatusCategoryImage = null; }

                    //Reset application status
                    vEditAppDataBind.ResetStatus();

                    await Notification_Send_Status("Edit", "Edited " + vEditAppDataBind.Name);
                    Debug.WriteLine("Editing application: " + vEditAppDataBind.Name + " in the list.");

                    //Save changes to Json file
                    JsonSaveApplications();

                    //Close the open popup
                    await Popup_Close_Top();
                    await Task.Delay(500);

                    //Focus on the application list
                    if (vEditAppDataBindCategory != vEditAppDataBind.Category)
                    {
                        Debug.WriteLine("App category changed to: " + vEditAppDataBind.Category);

                        //Remove app from previous category
                        if (vEditAppDataBindCategory == AppCategory.Game)
                        {
                            await ListBoxRemoveItem(lb_Games, List_Games, vEditAppDataBind, false);
                        }
                        else if (vEditAppDataBindCategory == AppCategory.App)
                        {
                            await ListBoxRemoveItem(lb_Apps, List_Apps, vEditAppDataBind, false);
                        }
                        else if (vEditAppDataBindCategory == AppCategory.Emulator)
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
                        ListCategory listAppCategory = (ListCategory)Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "ListAppCategory"));
                        if (listAppCategory == ListCategory.Search)
                        {
                            await ListboxFocusIndex(lb_Search, false, false, -1, vProcessCurrent.MainWindowHandle);
                        }
                        else
                        {
                            if (vEditAppDataBind.Category == AppCategory.Game)
                            {
                                await ListboxFocusIndex(lb_Games, false, true, -1, vProcessCurrent.MainWindowHandle);
                            }
                            else if (vEditAppDataBind.Category == AppCategory.App)
                            {
                                await ListboxFocusIndex(lb_Apps, false, true, -1, vProcessCurrent.MainWindowHandle);
                            }
                            else if (vEditAppDataBind.Category == AppCategory.Emulator)
                            {
                                await ListboxFocusIndex(lb_Emulators, false, true, -1, vProcessCurrent.MainWindowHandle);
                            }
                        }
                    }
                    else
                    {
                        //Focus on the item listbox
                        ListCategory listAppCategory = (ListCategory)Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "ListAppCategory"));
                        if (listAppCategory == ListCategory.Search)
                        {
                            await ListboxFocusIndex(lb_Search, false, false, -1, vProcessCurrent.MainWindowHandle);
                        }
                        else
                        {
                            if (vEditAppDataBind.Category == AppCategory.Game)
                            {
                                await ListboxFocusIndex(lb_Games, false, false, -1, vProcessCurrent.MainWindowHandle);
                            }
                            else if (vEditAppDataBind.Category == AppCategory.App)
                            {
                                await ListboxFocusIndex(lb_Apps, false, false, -1, vProcessCurrent.MainWindowHandle);
                            }
                            else if (vEditAppDataBind.Category == AppCategory.Emulator)
                            {
                                await ListboxFocusIndex(lb_Emulators, false, false, -1, vProcessCurrent.MainWindowHandle);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Application edit or add failed: " + ex.Message);
            }
        }

        private void Checkbox_AddLaunchSkipRom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox senderSource = (CheckBox)sender;
                if ((bool)senderSource.IsChecked)
                {
                    tb_AddAppPathRoms.IsEnabled = false;
                    btn_AddAppPathRoms.IsEnabled = false;
                }
                else
                {
                    tb_AddAppPathRoms.IsEnabled = true;
                    btn_AddAppPathRoms.IsEnabled = true;
                }
            }
            catch { }
        }

        private void Checkbox_AddLaunchEnableHDR_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Enable monitor HDR
                string executableName = string.Empty;
                string executableNameRaw = string.Empty;
                if (string.IsNullOrWhiteSpace(tb_AddAppNameExe.Text))
                {
                    executableName = Path.GetFileNameWithoutExtension(tb_AddAppExePath.Text).ToLower();
                    executableNameRaw = tb_AddAppExePath.Text.ToLower();
                }
                else
                {
                    executableName = Path.GetFileNameWithoutExtension(tb_AddAppNameExe.Text).ToLower();
                    executableNameRaw = tb_AddAppNameExe.Text.ToLower();
                }
                List<ProfileShared> enabledHDR = vCtrlHDRProcessName.Where(x => x.String1.ToLower() == executableName || x.String1.ToLower() == executableNameRaw).ToList();

                if ((bool)checkbox_AddLaunchEnableHDR.IsChecked)
                {
                    if (!enabledHDR.Any())
                    {
                        ProfileShared newProfile = new ProfileShared();
                        newProfile.String1 = executableNameRaw;
                        vCtrlHDRProcessName.Add(newProfile);
                        JsonSaveObject(vCtrlHDRProcessName, @"User\CtrlHDRProcessName");
                    }
                    Debug.WriteLine("Enabled HDR profile for: " + executableNameRaw);
                }
                else
                {
                    if (enabledHDR.Any())
                    {
                        foreach (ProfileShared removeProfile in enabledHDR)
                        {
                            vCtrlHDRProcessName.Remove(removeProfile);
                        }
                        JsonSaveObject(vCtrlHDRProcessName, @"User\CtrlHDRProcessName");
                    }
                    Debug.WriteLine("Disabled HDR profile for: " + executableNameRaw);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to change enable HDR profile: " + ex.Message);
            }
        }
    }
}