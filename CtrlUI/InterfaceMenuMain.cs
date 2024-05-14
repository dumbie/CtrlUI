using ArnoldVinkCode;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle main menu keyboard/controller tapped
        async void ListBox_Menu_KeyPressUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space) { await Listbox_Menu_SingleTap(); }
            }
            catch { }
        }

        //Handle main menu mouse/touch tapped
        async void ListBox_Menu_MousePressUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Check if an actual ListBoxItem is clicked
                if (!AVFunctions.ListBoxItemClickCheck((DependencyObject)e.OriginalSource)) { return; }

                //Check which mouse button is pressed
                if (e.ClickCount == 1)
                {
                    vSingleTappedEvent = true;
                    await Task.Delay(500);
                    if (vSingleTappedEvent) { await Listbox_Menu_SingleTap(); }
                }
            }
            catch { }
        }

        //Handle main menu single tap
        async Task Listbox_Menu_SingleTap()
        {
            try
            {
                if (listbox_MainMenu.SelectedIndex >= 0)
                {
                    DataBindString selectedItem = (DataBindString)listbox_MainMenu.SelectedItem;
                    string selectedItemString = selectedItem.Data1.ToString();
                    if (selectedItemString == "menuButtonUpdateRestart") { await AppUpdateRestart(); }
                    else if (selectedItemString == "menuButtonMonitor") { await Popup_Show(grid_Popup_Monitor, btn_Monitor_Switch_Primary); }
                    else if (selectedItemString == "menuButtonAudioDevice") { await SwitchAudioDevice(); }
                    else if (selectedItemString == "menuButtonRunExe") { await LaunchExecutableFile(); }
                    else if (selectedItemString == "menuButtonRunStore") { await LaunchStoreApplication(); }
                    else if (selectedItemString == "menuButtonAddExe") { await Popup_Show_AddExe(); }
                    else if (selectedItemString == "menuButtonAddStore") { await Popup_Show_AddStore(); }
                    else if (selectedItemString == "menuButtonSettings") { await ShowLoadSettingsPopup(); }
                    else if (selectedItemString == "menuButtonHelp") { await Popup_Show(grid_Popup_Help, btn_Help_Focus); }
                    else if (selectedItemString == "menuButtonCloseLaunchers") { await CloseLaunchers(); }
                    else if (selectedItemString == "menuButtonDisconnect") { await CloseStreamers(); }
                    else if (selectedItemString == "menuButtonShutdown") { await AppExit.Exit_Prompt(); }
                    else if (selectedItemString == "menuButtonShowFileManager") { await ShowFileManager(); }
                    else if (selectedItemString == "menuButtonProfileManager") { await Popup_Show_ProfileManager(); }
                    else if (selectedItemString == "menuButtonRecycleBin") { await ShowRecycleBinManager(); }
                }
            }
            catch { }
        }

        //Add main menu items
        void MainMenuAddItems()
        {
            try
            {
                DataBindString menuButtonMonitor = new DataBindString
                {
                    ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Monitor.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0),
                    Name = "Change display monitor settings",
                    Data1 = "menuButtonMonitor"
                };
                List_MainMenu.Add(menuButtonMonitor);

                DataBindString menuButtonAudioDevice = new DataBindString
                {
                    ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/VolumeUp.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0),
                    Name = "Switch audio playback device",
                    Data1 = "menuButtonAudioDevice"
                };
                List_MainMenu.Add(menuButtonAudioDevice);

                DataBindString menuButtonRunExe = new DataBindString
                {
                    ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRunExe.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0),
                    Name = "Launch an executable file from disk",
                    Data1 = "menuButtonRunExe"
                };
                List_MainMenu.Add(menuButtonRunExe);

                DataBindString menuButtonRunStore = new DataBindString
                {
                    ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRunStore.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0),
                    Name = "Launch Windows store application",
                    Data1 = "menuButtonRunStore"
                };
                List_MainMenu.Add(menuButtonRunStore);

                DataBindString menuButtonAddExe = new DataBindString
                {
                    ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppAddExe.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0),
                    Name = "Add new executable application to the list",
                    Data1 = "menuButtonAddExe"
                };
                List_MainMenu.Add(menuButtonAddExe);

                DataBindString menuButtonAddStore = new DataBindString
                {
                    ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppAddStore.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0),
                    Name = "Add Windows store application to the list",
                    Data1 = "menuButtonAddStore"
                };
                List_MainMenu.Add(menuButtonAddStore);

                DataBindString menuButtonDisconnect = new DataBindString
                {
                    ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Stream.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0),
                    Name = "Disconnect active remote streams",
                    Data1 = "menuButtonDisconnect"
                };
                List_MainMenu.Add(menuButtonDisconnect);

                DataBindString menuButtonCloseLaunchers = new DataBindString
                {
                    ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppClose.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0),
                    Name = "Close other running app launchers",
                    Data1 = "menuButtonCloseLaunchers"
                };
                List_MainMenu.Add(menuButtonCloseLaunchers);

                DataBindString menuButtonShutdown = new DataBindString
                {
                    ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Shutdown.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0),
                    Name = "Close CtrlUI or shutdown the PC",
                    Data1 = "menuButtonShutdown"
                };
                List_MainMenu.Add(menuButtonShutdown);

                DataBindString menuButtonShowFileManager = new DataBindString
                {
                    ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Folder.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0),
                    Name = "Show file browser and manager",
                    Data1 = "menuButtonShowFileManager"
                };
                List_MainMenu.Add(menuButtonShowFileManager);

                DataBindString menuButtonRecycleBin = new DataBindString
                {
                    ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Remove.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0),
                    Name = "Manage Windows recycle bin",
                    Data1 = "menuButtonRecycleBin"
                };
                List_MainMenu.Add(menuButtonRecycleBin);

                DataBindString menuButtonProfileManager = new DataBindString
                {
                    ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Profile.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0),
                    Name = "Open the profile manager",
                    Data1 = "menuButtonProfileManager"
                };
                List_MainMenu.Add(menuButtonProfileManager);

                DataBindString menuButtonSettings = new DataBindString
                {
                    ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Settings.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0),
                    Name = "Open application settings",
                    Data1 = "menuButtonSettings"
                };
                List_MainMenu.Add(menuButtonSettings);

                DataBindString menuButtonHelp = new DataBindString
                {
                    ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Help.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0),
                    Name = "Show application help",
                    Data1 = "menuButtonHelp"
                };
                List_MainMenu.Add(menuButtonHelp);

                //Bind the list to main menu
                listbox_MainMenu.ItemsSource = List_MainMenu;
            }
            catch { }
        }

        //Insert update to main menu
        void MainMenuInsertUpdate()
        {
            try
            {
                if (!List_MainMenu.Any(x => x.Data1.ToString() == "menuButtonUpdateRestart"))
                {
                    DataBindString menuButtonUpdateRestart = new DataBindString
                    {
                        ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Refresh.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0),
                        Name = "Update and restart CtrlUI",
                        Data1 = "menuButtonUpdateRestart"
                    };
                    List_MainMenu.Insert(0, menuButtonUpdateRestart);
                }
            }
            catch { }
        }

        //Disable main menu buttons
        void MainMenuButtonsDisable()
        {
            try
            {
                button_MenuHamburger.IsEnabled = false;
                button_MenuClose.IsEnabled = false;
                button_MenuSorting.IsEnabled = false;
            }
            catch { }
        }

        //Enable main menu buttons
        void MainMenuButtonsEnable(bool forceEnable)
        {
            try
            {
                if (forceEnable || grid_Popup_Welcome.Visibility != Visibility.Visible)
                {
                    button_MenuHamburger.IsEnabled = true;
                    button_MenuClose.IsEnabled = true;
                    button_MenuSorting.IsEnabled = true;
                }
            }
            catch { }
        }
    }
}