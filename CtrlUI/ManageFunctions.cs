using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkCode.AVUwpAppx;
using static ArnoldVinkStyles.AVFocus;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check and return the highest app number
        public int GetHighestAppNumber()
        {
            int newNumber = 0;
            try
            {
                IEnumerable<DataBindApp> CurrentApps = CombineAppLists(true, true, true, false, false, false, false);
                if (CurrentApps.Any())
                {
                    newNumber = CurrentApps.Select(x => x.Number).Max() + 1;
                }
                else
                {
                    newNumber = 1;
                }
            }
            catch { }
            return newNumber;
        }

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

                //Check if application executable exists
                if (dataBindApp.Type == ProcessType.Win32 || dataBindApp.Type == ProcessType.Unknown)
                {
                    if (!string.IsNullOrWhiteSpace(dataBindApp.PathExe) && !File.Exists(dataBindApp.PathExe))
                    {
                        dataBindApp.StatusAvailable = Visibility.Visible;
                    }
                }

                //Check if application is UWP or Win32Store app
                if (dataBindApp.Type == ProcessType.UWP || dataBindApp.Type == ProcessType.Win32Store)
                {
                    //Set store status to visible
                    dataBindApp.StatusStore = Visibility.Visible;

                    //Check if application is installed
                    if (!string.IsNullOrWhiteSpace(dataBindApp.AppUserModelId) && GetUwpAppPackageByAppUserModelId(dataBindApp.AppUserModelId) == null)
                    {
                        dataBindApp.StatusAvailable = Visibility.Visible;
                    }
                }

                //Check if application rom folder exists
                if (dataBindApp.Category == AppCategory.Emulator)
                {
                    if (!string.IsNullOrWhiteSpace(dataBindApp.PathRoms) && !Directory.Exists(dataBindApp.PathRoms))
                    {
                        dataBindApp.StatusAvailable = Visibility.Visible;
                    }
                }

                //Load and set application image
                if (loadAppImage)
                {
                    dataBindApp.ImageBitmap = Image_Application_Load(dataBindApp, vImageLoadSize, 0);
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
                    //Set emulator category image
                    if (dataBindApp.EmulatorCategory == EmulatorCategory.Console) { dataBindApp.StatusEmulatorCategoryImage = vImagePreloadConsole; }
                    else if (dataBindApp.EmulatorCategory == EmulatorCategory.Handheld) { dataBindApp.StatusEmulatorCategoryImage = vImagePreloadHandheld; }
                    else if (dataBindApp.EmulatorCategory == EmulatorCategory.Computer) { dataBindApp.StatusEmulatorCategoryImage = vImagePreloadComputer; }
                    else if (dataBindApp.EmulatorCategory == EmulatorCategory.Arcade) { dataBindApp.StatusEmulatorCategoryImage = vImagePreloadArcade; }
                    else if (dataBindApp.EmulatorCategory == EmulatorCategory.Pinball) { dataBindApp.StatusEmulatorCategoryImage = vImagePreloadPinball; }
                    else if (dataBindApp.EmulatorCategory == EmulatorCategory.Pong) { dataBindApp.StatusEmulatorCategoryImage = vImagePreloadPong; }
                    else if (dataBindApp.EmulatorCategory == EmulatorCategory.Chess) { dataBindApp.StatusEmulatorCategoryImage = vImagePreloadChess; }
                    else if (dataBindApp.EmulatorCategory == EmulatorCategory.VirtualReality) { dataBindApp.StatusEmulatorCategoryImage = vImagePreloadVirtualReality; }
                    else if (dataBindApp.EmulatorCategory == EmulatorCategory.OperatingSystem) { dataBindApp.StatusEmulatorCategoryImage = vImagePreloadOperatingSystem; }
                    else if (dataBindApp.EmulatorCategory == EmulatorCategory.Other) { dataBindApp.StatusEmulatorCategoryImage = null; }

                    await ListBoxAddItem(lb_Emulators, List_Emulators, dataBindApp, false, false);
                }

                //Save changes to Json file
                if (generateAppNumber)
                {
                    JsonSaveList_Applications();
                }
            }
            catch { }
        }

        //Remove application from the list
        async Task RemoveAppFromList(DataBindApp dataBindApp, bool saveJson, bool removeImageFile, bool silent)
        {
            try
            {
                //Confirm application remove prompt
                if (!silent)
                {
                    List<DataBindString> messageAnswers = new List<DataBindString>();
                    DataBindString answerYes = new DataBindString();
                    answerYes.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Remove.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    answerYes.Name = "Remove application from list";
                    messageAnswers.Add(answerYes);

                    string deleteString = "Are you sure you want to remove: " + dataBindApp.Name + "?";
                    DataBindString messageResult = await Popup_Show_MessageBox("Remove application", "", deleteString, messageAnswers);
                    if (messageResult == null)
                    {
                        Debug.WriteLine("Cancelled application removal.");
                        return;
                    }
                }

                //Remove application from listboxes
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
                else if (dataBindApp.Category == AppCategory.Launcher)
                {
                    await ListBoxRemoveItem(lb_Launchers, List_Launchers, dataBindApp, true);
                }
                else if (dataBindApp.Category == AppCategory.Gallery)
                {
                    await ListBoxRemoveItem(lb_Gallery, List_Gallery, dataBindApp, true);
                }

                //Remove application from search listbox
                await ListBoxRemoveItem(lb_Search, List_Search, dataBindApp, true);

                //Save changes to Json file
                if (saveJson)
                {
                    JsonSaveList_Applications();
                }

                //Remove application image files
                if (removeImageFile)
                {
                    Image_Application_Remove(dataBindApp);
                }

                //Show removed notification
                if (!silent)
                {
                    Notification_Show_Status("Minus", "Removed " + dataBindApp.Name);
                    Debug.WriteLine("Removed application: " + dataBindApp.Name);
                }
            }
            catch { }
        }

        //Add or edit application to list apps and Json file
        async Task SaveEditManageApplication()
        {
            try
            {
                //Check the selected categories
                AppCategory selectedAppCategory = (AppCategory)lb_Manage_AddAppCategory.SelectedIndex;
                EmulatorCategory selectedEmulatorCategory = (EmulatorCategory)lb_Manage_AddEmulatorCategory.SelectedIndex;

                //Check if adding or editing application
                bool addApplication = vEditAppDataBind == null;
                bool editApplication = vEditAppDataBind != null;

                //Check the editing application type
                bool win32Application = true;
                bool uwpApplication = false;
                if (editApplication)
                {
                    win32Application = vEditAppDataBind.Type == ProcessType.Win32;
                    uwpApplication = vEditAppDataBind.Type == ProcessType.UWP || vEditAppDataBind.Type == ProcessType.Win32Store;
                }

                //Check if there is an application name set
                if (string.IsNullOrWhiteSpace(tb_AddAppName.Text))
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    Answer1.Name = "Ok";
                    Answers.Add(Answer1);

                    if (selectedAppCategory == AppCategory.Emulator)
                    {
                        await Popup_Show_MessageBox("Please enter a platform name first", "", "", Answers);
                    }
                    else
                    {
                        await Popup_Show_MessageBox("Please enter an application name first", "", "", Answers);
                    }

                    return;
                }

                //Check if there is an application exe set
                if (win32Application && tb_AddAppName.Text == "Select application executable file first")
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    Answer1.Name = "Ok";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Please select an application executable file first", "", "", Answers);
                    return;
                }

                //Check if there is an application exe set
                if (win32Application && string.IsNullOrWhiteSpace(tb_AddAppPathExe.Text))
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    Answer1.Name = "Ok";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Please select an application executable file first", "", "", Answers);
                    return;
                }

                //Prevent CtrlUI from been added to the list
                if (win32Application && tb_AddAppPathExe.Text.ToLower().Contains("ctrlui"))
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    Answer1.Name = "Ok";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Sorry, you cannot add CtrlUI as an application", "", "", Answers);
                    return;
                }

                //Check if set application exe exists
                if (win32Application && !File.Exists(tb_AddAppPathExe.Text))
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    Answer1.Name = "Ok";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Application executable not found, please select another one", "", "", Answers);
                    return;
                }

                //Validate application launch path
                if (win32Application && !string.IsNullOrWhiteSpace(tb_AddAppPathLaunch.Text) && !Directory.Exists(tb_AddAppPathLaunch.Text))
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
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
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        Answer1.Name = "Ok";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("Please select an emulator rom folder first", "", "", Answers);
                        return;
                    }
                    if (!Directory.Exists(tb_AddAppPathRoms.Text))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        Answer1.Name = "Ok";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("Rom folder not found, please select another one", "", "", Answers);
                        return;
                    }
                }

                //Check if application needs to be edited or added
                if (addApplication)
                {
                    //Check if application executable already exists
                    if (CombineAppLists(true, true, true, false, false, false, false).Any(x => x.PathExe.ToLower() == tb_AddAppPathExe.Text.ToLower()))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        Answer1.Name = "Ok";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("Application executable already exists", "", "Please select another executable that is not yet added.", Answers);
                        return;
                    }

                    Notification_Show_Status("Plus", "Added " + tb_AddAppName.Text);
                    Debug.WriteLine("Adding Win32 app: " + tb_AddAppName.Text + " to the list.");
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = selectedAppCategory, EmulatorCategory = selectedEmulatorCategory, Name = tb_AddAppName.Text, EmulatorName = tb_AddAppEmulatorName.Text, PathExe = tb_AddAppPathExe.Text, PathLaunch = tb_AddAppPathLaunch.Text, PathRoms = tb_AddAppPathRoms.Text, Argument = tb_AddAppArgument.Text, NameExe = tb_AddAppNameExe.Text, LaunchFilePicker = (bool)checkbox_AddLaunchFilePicker.IsChecked, LaunchSkipRom = (bool)checkbox_AddLaunchSkipRom.IsChecked, LaunchKeyboard = (bool)checkbox_AddLaunchKeyboard.IsChecked, LaunchEnableDisplayHDR = (bool)checkbox_AddLaunchEnableDisplayHDR.IsChecked, LaunchEnableAutoHDR = (bool)checkbox_AddLaunchEnableAutoHDR.IsChecked, LaunchAsAdmin = (bool)checkbox_AddLaunchAsAdmin.IsChecked };
                    await AddAppToList(dataBindApp, true, true);

                    //Close the open popup
                    await Popup_Close_Top(true);

                    //Focus on the application list
                    if (selectedAppCategory == AppCategory.Game)
                    {
                        await ListBoxFocusIndex(lb_Games, true, 0, vProcessCurrent.WindowHandleMain);
                    }
                    else if (selectedAppCategory == AppCategory.App)
                    {
                        await ListBoxFocusIndex(lb_Apps, true, 0, vProcessCurrent.WindowHandleMain);
                    }
                    else if (selectedAppCategory == AppCategory.Emulator)
                    {
                        await ListBoxFocusIndex(lb_Emulators, true, 0, vProcessCurrent.WindowHandleMain);
                    }
                }
                else
                {
                    //Check if application executable already exists
                    if (vEditAppDataBind.PathExe == tb_AddAppPathExe.Text)
                    {
                        Debug.WriteLine("Application executable has not changed.");
                    }
                    else if (CombineAppLists(true, true, true, false, false, false, false).Any(x => x.PathExe.ToLower() == tb_AddAppPathExe.Text.ToLower()))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        Answer1.Name = "Ok";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("Application executable already exists", "", "Please select another executable that is not yet added.", Answers);
                        return;
                    }

                    //Rename application image and info files
                    Image_Application_Rename(vEditAppDataBind, tb_AddAppName.Text);

                    //Edit application in the list
                    vEditAppDataBind.Category = selectedAppCategory;
                    vEditAppDataBind.EmulatorCategory = selectedEmulatorCategory;
                    vEditAppDataBind.Name = tb_AddAppName.Text;
                    vEditAppDataBind.EmulatorName = tb_AddAppEmulatorName.Text;
                    vEditAppDataBind.PathExe = tb_AddAppPathExe.Text;
                    vEditAppDataBind.PathLaunch = tb_AddAppPathLaunch.Text;
                    vEditAppDataBind.PathRoms = tb_AddAppPathRoms.Text;
                    vEditAppDataBind.Argument = tb_AddAppArgument.Text;
                    vEditAppDataBind.NameExe = tb_AddAppNameExe.Text;
                    vEditAppDataBind.LaunchAsAdmin = (bool)checkbox_AddLaunchAsAdmin.IsChecked;
                    vEditAppDataBind.LaunchFilePicker = (bool)checkbox_AddLaunchFilePicker.IsChecked;
                    vEditAppDataBind.LaunchSkipRom = (bool)checkbox_AddLaunchSkipRom.IsChecked;
                    vEditAppDataBind.LaunchKeyboard = (bool)checkbox_AddLaunchKeyboard.IsChecked;
                    vEditAppDataBind.LaunchEnableDisplayHDR = (bool)checkbox_AddLaunchEnableDisplayHDR.IsChecked;
                    vEditAppDataBind.LaunchEnableAutoHDR = (bool)checkbox_AddLaunchEnableAutoHDR.IsChecked;
                    vEditAppDataBind.LightImageBackground = (bool)checkbox_LightImageBackground.IsChecked;

                    //Edit application image in the list
                    vEditAppDataBind.ImageBitmap = Image_Application_Load(vEditAppDataBind, vImageLoadSize, 0);

                    //Edit emulator image in the list
                    if (vEditAppDataBind.EmulatorCategory == EmulatorCategory.Console) { vEditAppDataBind.StatusEmulatorCategoryImage = vImagePreloadConsole; }
                    else if (vEditAppDataBind.EmulatorCategory == EmulatorCategory.Handheld) { vEditAppDataBind.StatusEmulatorCategoryImage = vImagePreloadHandheld; }
                    else if (vEditAppDataBind.EmulatorCategory == EmulatorCategory.Computer) { vEditAppDataBind.StatusEmulatorCategoryImage = vImagePreloadComputer; }
                    else if (vEditAppDataBind.EmulatorCategory == EmulatorCategory.Arcade) { vEditAppDataBind.StatusEmulatorCategoryImage = vImagePreloadArcade; }
                    else if (vEditAppDataBind.EmulatorCategory == EmulatorCategory.Pinball) { vEditAppDataBind.StatusEmulatorCategoryImage = vImagePreloadPinball; }
                    else if (vEditAppDataBind.EmulatorCategory == EmulatorCategory.Pong) { vEditAppDataBind.StatusEmulatorCategoryImage = vImagePreloadPong; }
                    else if (vEditAppDataBind.EmulatorCategory == EmulatorCategory.Chess) { vEditAppDataBind.StatusEmulatorCategoryImage = vImagePreloadChess; }
                    else if (vEditAppDataBind.EmulatorCategory == EmulatorCategory.VirtualReality) { vEditAppDataBind.StatusEmulatorCategoryImage = vImagePreloadVirtualReality; }
                    else if (vEditAppDataBind.EmulatorCategory == EmulatorCategory.OperatingSystem) { vEditAppDataBind.StatusEmulatorCategoryImage = vImagePreloadOperatingSystem; }
                    else if (vEditAppDataBind.EmulatorCategory == EmulatorCategory.Other) { vEditAppDataBind.StatusEmulatorCategoryImage = null; }

                    //Reset application status
                    vEditAppDataBind.ResetStatus(true);

                    Notification_Show_Status("Edit", "Edited " + vEditAppDataBind.Name);
                    Debug.WriteLine("Editing application: " + vEditAppDataBind.Name + " in the list.");

                    //Save changes to Json file
                    JsonSaveList_Applications();

                    //Close the open popup
                    await Popup_Close_Top(true);

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

                        //Edit search image in the list
                        if (vCurrentListCategory == ListCategory.Search)
                        {
                            SearchAppSetCategoryImage(vEditAppDataBind);
                        }
                    }

                    //Focus on the edit listbox item
                    await FocusEditListboxItem();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Application edit or add failed: " + ex.Message);
            }
        }

        //Focus on the edit listbox item
        async Task FocusEditListboxItem()
        {
            try
            {
                if (vCurrentListCategory == ListCategory.Search)
                {
                    await ListBoxFocusItem(lb_Search, vEditAppDataBind, vProcessCurrent.WindowHandleMain);
                }
                else if (vEditAppDataBind.Category == AppCategory.Game)
                {
                    await ListBoxFocusItem(lb_Games, vEditAppDataBind, vProcessCurrent.WindowHandleMain);
                }
                else if (vEditAppDataBind.Category == AppCategory.App)
                {
                    await ListBoxFocusItem(lb_Apps, vEditAppDataBind, vProcessCurrent.WindowHandleMain);
                }
                else if (vEditAppDataBind.Category == AppCategory.Emulator)
                {
                    await ListBoxFocusItem(lb_Emulators, vEditAppDataBind, vProcessCurrent.WindowHandleMain);
                }
            }
            catch { }
        }

        //Add categories to manage interface
        void ManageInterface_AddCategories()
        {
            try
            {
                //Load application categories
                List<DataBindString> listAppCategories = new List<DataBindString>();

                DataBindString categoryApp = new DataBindString();
                categoryApp.ImageBitmap = vImagePreloadApp;
                categoryApp.Name = "App";
                listAppCategories.Add(categoryApp);

                DataBindString categoryGame = new DataBindString();
                categoryGame.ImageBitmap = vImagePreloadGame;
                categoryGame.Name = "Game";
                listAppCategories.Add(categoryGame);

                DataBindString categoryEmulator = new DataBindString();
                categoryEmulator.ImageBitmap = vImagePreloadEmulator;
                categoryEmulator.Name = "Emulator";
                listAppCategories.Add(categoryEmulator);

                //Load emulator categories
                List<DataBindString> listEmulatorCategories = new List<DataBindString>();

                DataBindString categoryOther = new DataBindString();
                categoryOther.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Other.png" }, null, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);
                categoryOther.Name = "Other";
                listEmulatorCategories.Add(categoryOther);

                DataBindString categoryConsole = new DataBindString();
                categoryConsole.ImageBitmap = vImagePreloadConsole;
                categoryConsole.Name = "Console";
                listEmulatorCategories.Add(categoryConsole);

                DataBindString categoryHandheld = new DataBindString();
                categoryHandheld.ImageBitmap = vImagePreloadHandheld;
                categoryHandheld.Name = "Handheld";
                listEmulatorCategories.Add(categoryHandheld);

                DataBindString categoryComputer = new DataBindString();
                categoryComputer.ImageBitmap = vImagePreloadComputer;
                categoryComputer.Name = "Computer";
                listEmulatorCategories.Add(categoryComputer);

                DataBindString categoryArcade = new DataBindString();
                categoryArcade.ImageBitmap = vImagePreloadArcade;
                categoryArcade.Name = "Arcade";
                listEmulatorCategories.Add(categoryArcade);

                DataBindString categoryPinball = new DataBindString();
                categoryPinball.ImageBitmap = vImagePreloadPinball;
                categoryPinball.Name = "Pinball";
                listEmulatorCategories.Add(categoryPinball);

                DataBindString categoryPong = new DataBindString();
                categoryPong.ImageBitmap = vImagePreloadPong;
                categoryPong.Name = "Pong";
                listEmulatorCategories.Add(categoryPong);

                DataBindString categoryChess = new DataBindString();
                categoryChess.ImageBitmap = vImagePreloadChess;
                categoryChess.Name = "Chess";
                listEmulatorCategories.Add(categoryChess);

                DataBindString categoryVirtualReality = new DataBindString();
                categoryVirtualReality.ImageBitmap = vImagePreloadVirtualReality;
                categoryVirtualReality.Name = "VR";
                listEmulatorCategories.Add(categoryVirtualReality);

                DataBindString categoryOperatingSystem = new DataBindString();
                categoryOperatingSystem.ImageBitmap = vImagePreloadOperatingSystem;
                categoryOperatingSystem.Name = "OS";
                listEmulatorCategories.Add(categoryOperatingSystem);

                //Set lists itemsource
                lb_Manage_AddAppCategory.ItemsSource = listAppCategories;
                lb_Manage_AddEmulatorCategory.ItemsSource = listEmulatorCategories;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to add manage categories: " + ex.Message);
            }
        }

        //Update manage interface based on category
        void ManageInterface_UpdateCategory(AppCategory appCategory, bool disableInterface)
        {
            try
            {
                //Check if adding or editing application
                bool addApplication = vEditAppDataBind == null;
                bool editApplication = vEditAppDataBind != null;

                //Check the editing application type
                bool win32Application = true;
                bool uwpApplication = false;
                if (editApplication)
                {
                    win32Application = vEditAppDataBind.Type == ProcessType.Win32;
                    uwpApplication = vEditAppDataBind.Type == ProcessType.UWP || vEditAppDataBind.Type == ProcessType.Win32Store;
                }

                //Set default interface
                if (editApplication)
                {
                    grid_Popup_Manage_txt_Title.Text = "Edit application";
                    grid_Popup_Manage_button_Save.ToolTip = new ToolTip() { Content = "Edit the application" };
                    ManageInterface_Enable();
                }
                else
                {
                    grid_Popup_Manage_txt_Title.Text = "Add application";
                    grid_Popup_Manage_button_Save.ToolTip = new ToolTip() { Content = "Save the application" };
                    if (disableInterface)
                    {
                        ManageInterface_Disable();
                    }
                }

                //Hide and show default settings
                sp_AddAppExePath.Visibility = Visibility.Visible;
                sp_AddAppPathLaunch.Visibility = Visibility.Visible;
                sp_AddAppArgument.Visibility = Visibility.Visible;
                sp_AddAppNameExe.Visibility = Visibility.Visible;
                checkbox_AddLaunchAsAdmin.Visibility = Visibility.Visible;
                checkbox_AddLaunchEnableAutoHDR.Visibility = Visibility.Visible;

                //Hide and show category based settings
                if (appCategory == AppCategory.Emulator)
                {
                    textblock_AddAppName.Text = "Platform name:";
                    sp_AddEmulatorPathRoms.Visibility = Visibility.Visible;
                    sp_AddEmulatorName.Visibility = Visibility.Visible;
                    sp_AddEmulatorCategory.Visibility = Visibility.Visible;
                    checkbox_AddLaunchFilePicker.Visibility = Visibility.Collapsed;
                }
                else
                {
                    textblock_AddAppName.Text = "Application name:";
                    sp_AddEmulatorPathRoms.Visibility = Visibility.Collapsed;
                    sp_AddEmulatorName.Visibility = Visibility.Collapsed;
                    sp_AddEmulatorCategory.Visibility = Visibility.Collapsed;
                    checkbox_AddLaunchFilePicker.Visibility = Visibility.Visible;
                }

                //Hide and show uwp based settings
                if (uwpApplication)
                {
                    sp_AddAppExePath.Visibility = Visibility.Collapsed;
                    sp_AddAppPathLaunch.Visibility = Visibility.Collapsed;
                    sp_AddAppNameExe.Visibility = Visibility.Collapsed;
                    checkbox_AddLaunchAsAdmin.Visibility = Visibility.Collapsed;
                    checkbox_AddLaunchEnableAutoHDR.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to update edit interface: " + ex.Message);
            }
        }

        //Disable manage interface
        void ManageInterface_Disable()
        {
            try
            {
                btn_Manage_AddAppLogo.IsEnabled = false;
                btn_Manage_ResetAppLogo.IsEnabled = false;
                tb_AddAppName.IsEnabled = false;
                tb_AddAppEmulatorName.IsEnabled = false;
                tb_AddAppPathExe.IsEnabled = false;
                tb_AddAppPathLaunch.IsEnabled = false;
                btn_AddAppPathLaunch.IsEnabled = false;
                checkbox_AddLaunchSkipRom.IsEnabled = false;
                tb_AddAppPathRoms.IsEnabled = false;
                btn_AddAppPathRoms.IsEnabled = false;
                tb_AddAppArgument.IsEnabled = false;
                tb_AddAppNameExe.IsEnabled = false;
            }
            catch { }
        }

        //Enable manage interface
        void ManageInterface_Enable()
        {
            try
            {
                btn_Manage_AddAppLogo.IsEnabled = true;
                btn_Manage_ResetAppLogo.IsEnabled = true;
                tb_AddAppName.IsEnabled = true;
                tb_AddAppEmulatorName.IsEnabled = true;
                tb_AddAppPathExe.IsEnabled = true;
                tb_AddAppPathLaunch.IsEnabled = true;
                btn_AddAppPathLaunch.IsEnabled = true;
                checkbox_AddLaunchSkipRom.IsEnabled = true;
                if (!(bool)checkbox_AddLaunchSkipRom.IsChecked)
                {
                    tb_AddAppPathRoms.IsEnabled = true;
                    btn_AddAppPathRoms.IsEnabled = true;
                }
                tb_AddAppArgument.IsEnabled = true;
                tb_AddAppNameExe.IsEnabled = true;
            }
            catch { }
        }

        //Show the application edit popup
        async Task Popup_Show_AppEdit(DataBindApp dataBindApp)
        {
            try
            {
                //Set current edit app variable
                vEditAppDataBind = dataBindApp;
                vEditAppDataBindCategory = dataBindApp.Category;

                //Select current categories
                lb_Manage_AddAppCategory.SelectedIndex = (int)dataBindApp.Category;
                lb_Manage_AddEmulatorCategory.SelectedIndex = (int)dataBindApp.EmulatorCategory;

                //Update application manage interface
                ManageInterface_UpdateCategory(dataBindApp.Category, true);

                //Load application image
                img_AddAppLogo.Source = Image_Application_Load(dataBindApp, vImageLoadSize, 0);

                //Fill the text boxes with application details
                tb_AddAppName.Text = dataBindApp.Name;
                tb_AddAppEmulatorName.Text = dataBindApp.EmulatorName;
                tb_AddAppPathExe.Text = dataBindApp.PathExe;
                tb_AddAppPathLaunch.Text = dataBindApp.PathLaunch;
                tb_AddAppPathRoms.Text = dataBindApp.PathRoms;
                tb_AddAppArgument.Text = dataBindApp.Argument;
                tb_AddAppNameExe.Text = dataBindApp.NameExe;
                checkbox_AddLaunchAsAdmin.IsChecked = dataBindApp.LaunchAsAdmin;
                checkbox_AddLaunchFilePicker.IsChecked = dataBindApp.LaunchFilePicker;
                checkbox_AddLaunchKeyboard.IsChecked = dataBindApp.LaunchKeyboard;
                checkbox_AddLaunchSkipRom.IsChecked = dataBindApp.LaunchSkipRom;
                checkbox_AddLaunchEnableDisplayHDR.IsChecked = dataBindApp.LaunchEnableDisplayHDR;
                checkbox_AddLaunchEnableAutoHDR.IsChecked = dataBindApp.LaunchEnableAutoHDR;
                checkbox_LightImageBackground.IsChecked = dataBindApp.LightImageBackground;

                //Load skip rom
                tb_AddAppPathRoms.IsEnabled = !dataBindApp.LaunchSkipRom;
                btn_AddAppPathRoms.IsEnabled = !dataBindApp.LaunchSkipRom;

                //Show the manage popup
                await Popup_Show(grid_Popup_Manage, tb_AddAppName);
            }
            catch { }
        }

        //Show the exe application add popup
        async Task Popup_Show_AddExe()
        {
            try
            {
                //Reset current edit application
                vEditAppDataBind = null;

                //Reset current categories
                lb_Manage_AddAppCategory.SelectedIndex = (int)AppCategory.Game;
                lb_Manage_AddEmulatorCategory.SelectedIndex = (int)EmulatorCategory.Other;

                //Update application manage interface
                ManageInterface_UpdateCategory(AppCategory.Game, true);

                //Load application image
                img_AddAppLogo.Source = vImagePreloadUnknownApp;

                //Fill the text boxes with application details
                tb_AddAppName.Text = "Select application executable file first";
                tb_AddAppEmulatorName.Text = string.Empty;
                tb_AddAppPathExe.Text = string.Empty;
                tb_AddAppPathLaunch.Text = string.Empty;
                tb_AddAppPathRoms.Text = string.Empty;
                tb_AddAppArgument.Text = string.Empty;
                tb_AddAppNameExe.Text = string.Empty;
                checkbox_AddLaunchAsAdmin.IsChecked = true;
                checkbox_AddLaunchFilePicker.IsChecked = false;
                checkbox_AddLaunchKeyboard.IsChecked = false;
                checkbox_AddLaunchSkipRom.IsChecked = false;
                checkbox_AddLaunchEnableDisplayHDR.IsChecked = false;
                checkbox_AddLaunchEnableAutoHDR.IsChecked = false;
                checkbox_LightImageBackground.IsChecked = false;

                //Show the manage popup
                await Popup_Show(grid_Popup_Manage, btn_AddAppPathExe);
            }
            catch { }
        }

        //Add Windows store application
        async Task Popup_Show_AddStore()
        {
            try
            {
                //Add application type categories
                List<DataBindString> answersCategory = new List<DataBindString>();

                BitmapImage imageApp = vImagePreloadApp;
                DataBindString stringApp = new DataBindString() { Name = "App", Data1 = "App", ImageBitmap = imageApp };
                answersCategory.Add(stringApp);

                BitmapImage imageGame = vImagePreloadGame;
                DataBindString stringGame = new DataBindString() { Name = "Game", Data1 = "Game", ImageBitmap = imageGame };
                answersCategory.Add(stringGame);

                BitmapImage imageEmulator = vImagePreloadEmulator;
                DataBindString stringEmulator = new DataBindString() { Name = "Emulator", Data1 = "Emulator", ImageBitmap = imageEmulator };
                answersCategory.Add(stringEmulator);

                //Show the messagebox
                DataBindString messageResult = await Popup_Show_MessageBox("Application Category", "", "Please select a new application category:", answersCategory);
                if (messageResult != null)
                {
                    //Check the selected application category
                    Enum.TryParse(messageResult.Data1.ToString(), out AppCategory selectedAddCategory);

                    //Select Window Store application
                    vFilePickerSettings = new FilePickerSettings();
                    vFilePickerSettings.Title = "Window Store Applications";
                    vFilePickerSettings.Description = "Please select a Windows store application to add as " + messageResult.Name + ":";
                    await Popup_Show_FilePicker("UWP", 0, false, null);

                    while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                    if (vFilePickerCancelled) { return; }

                    //Check if new application already exists
                    if (CombineAppLists(true, true, true, false, false, false, false).Any(x => x.Name.ToLower() == vFilePickerResult.Name.ToLower() || x.AppUserModelId.ToLower() == vFilePickerResult.PathFile.ToLower()))
                    {
                        List<DataBindString> answersConfirm = new List<DataBindString>();
                        DataBindString answerAlright = new DataBindString();
                        answerAlright.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        answerAlright.Name = "Ok";
                        answersConfirm.Add(answerAlright);

                        await Popup_Show_MessageBox("This application already exists", "", "", answersConfirm);
                        return;
                    }

                    Notification_Show_Status("Plus", "Added " + vFilePickerResult.Name);
                    Debug.WriteLine("Adding UWP app: " + tb_AddAppName.Text + " to the list.");
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.UWP, Category = selectedAddCategory, Name = vFilePickerResult.Name, NameExe = vFilePickerResult.NameExe, AppUserModelId = vFilePickerResult.PathFile, LaunchKeyboard = (bool)checkbox_AddLaunchKeyboard.IsChecked, LaunchEnableDisplayHDR = (bool)checkbox_AddLaunchEnableDisplayHDR.IsChecked, LaunchEnableAutoHDR = (bool)checkbox_AddLaunchEnableAutoHDR.IsChecked, LaunchAsAdmin = (bool)checkbox_AddLaunchAsAdmin.IsChecked };
                    await AddAppToList(dataBindApp, true, true);
                }
            }
            catch { }
        }
    }
}