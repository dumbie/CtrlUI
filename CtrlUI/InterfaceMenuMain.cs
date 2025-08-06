using ArnoldVinkStyles;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle main menu keyboard/controller tapped
        async void ListView_MainMenu_KeyPressUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                //Check which key is pressed
                if (e.Key == VirtualKey.Space)
                {
                    //Get clicked item
                    DataBindString clickedObject = AVListView.GetRoutedListViewItemObject<DataBindString>(e);

                    await Listbox_MainMenu_Click(clickedObject);
                }
            }
            catch { }
        }

        //Handle main menu mouse/touch tapped
        async void ListView_MainMenu_MousePressUp(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                //Check which mouse button is pressed
                if (vMousePressDownLeft)
                {
                    //Get clicked item
                    DataBindString clickedObject = AVListView.GetRoutedListViewItemObject<DataBindString>(e);

                    await Listbox_MainMenu_Click(clickedObject);
                }
            }
            catch { }
        }

        //Handle main menu click
        async Task Listbox_MainMenu_Click(DataBindString dataBindString)
        {
            try
            {
                //Check clicked object
                if (dataBindString == null)
                {
                    Debug.WriteLine("Clicked ListView object is null.");
                    return;
                }

                //Check clicked button
                string selectedItemString = dataBindString.Data1.ToString();
                if (selectedItemString == "menuButtonUpdateRestart") { UpdateRestart(); }
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
                else if (selectedItemString == "menuButtonShutdown") { await Exit_Prompt(); }
                else if (selectedItemString == "menuButtonShowFileManager") { await ShowFileManager(); }
                else if (selectedItemString == "menuButtonProfileManager") { await Popup_Show_ProfileManager(); }
                else if (selectedItemString == "menuButtonRecycleBin") { await ShowRecycleBinManager(); }
            }
            catch { }
        }

        //Add main menu items
        async Task MainMenuAddItems()
        {
            try
            {
                DataBindString menuButtonMonitor = new DataBindString
                {
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/Monitor.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    }),
                    Name = "Change display monitor settings",
                    Data1 = "menuButtonMonitor"
                };
                List_MainMenu.Add(menuButtonMonitor);

                DataBindString menuButtonAudioDevice = new DataBindString
                {
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/VolumeUp.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    }),
                    Name = "Switch audio playback device",
                    Data1 = "menuButtonAudioDevice"
                };
                List_MainMenu.Add(menuButtonAudioDevice);

                DataBindString menuButtonRunExe = new DataBindString
                {
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/AppRunExe.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    }),
                    Name = "Launch an executable file from disk",
                    Data1 = "menuButtonRunExe"
                };
                List_MainMenu.Add(menuButtonRunExe);

                DataBindString menuButtonRunStore = new DataBindString
                {
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/AppRunStore.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    }),
                    Name = "Launch Windows store application",
                    Data1 = "menuButtonRunStore"
                };
                List_MainMenu.Add(menuButtonRunStore);

                DataBindString menuButtonAddExe = new DataBindString
                {
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/AppAddExe.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    }),
                    Name = "Add new executable application to the list",
                    Data1 = "menuButtonAddExe"
                };
                List_MainMenu.Add(menuButtonAddExe);

                DataBindString menuButtonAddStore = new DataBindString
                {
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/AppAddStore.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    }),
                    Name = "Add Windows store application to the list",
                    Data1 = "menuButtonAddStore"
                };
                List_MainMenu.Add(menuButtonAddStore);

                DataBindString menuButtonDisconnect = new DataBindString
                {
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/Stream.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    }),
                    Name = "Disconnect active remote streams",
                    Data1 = "menuButtonDisconnect"
                };
                List_MainMenu.Add(menuButtonDisconnect);

                DataBindString menuButtonCloseLaunchers = new DataBindString
                {
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/AppClose.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    }),
                    Name = "Close other running app launchers",
                    Data1 = "menuButtonCloseLaunchers"
                };
                List_MainMenu.Add(menuButtonCloseLaunchers);

                DataBindString menuButtonShutdown = new DataBindString
                {
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/Shutdown.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    }),
                    Name = "Close CtrlUI or shutdown the PC",
                    Data1 = "menuButtonShutdown"
                };
                List_MainMenu.Add(menuButtonShutdown);

                DataBindString menuButtonShowFileManager = new DataBindString
                {
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/Folder.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    }),
                    Name = "Show file browser and manager",
                    Data1 = "menuButtonShowFileManager"
                };
                List_MainMenu.Add(menuButtonShowFileManager);

                DataBindString menuButtonRecycleBin = new DataBindString
                {
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/Remove.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    }),
                    Name = "Manage Windows recycle bin",
                    Data1 = "menuButtonRecycleBin"
                };
                List_MainMenu.Add(menuButtonRecycleBin);

                DataBindString menuButtonProfileManager = new DataBindString
                {
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/Profile.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    }),
                    Name = "Open the profile manager",
                    Data1 = "menuButtonProfileManager"
                };
                List_MainMenu.Add(menuButtonProfileManager);

                DataBindString menuButtonSettings = new DataBindString
                {
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/Settings.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    }),
                    Name = "Open application settings",
                    Data1 = "menuButtonSettings"
                };
                List_MainMenu.Add(menuButtonSettings);

                DataBindString menuButtonHelp = new DataBindString
                {
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/Help.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    }),
                    Name = "Show application help",
                    Data1 = "menuButtonHelp"
                };
                List_MainMenu.Add(menuButtonHelp);

                //Bind the list to main menu
                listView_MainMenu.ItemsSource = List_MainMenu;
            }
            catch { }
        }

        //Insert update to main menu
        async Task MainMenuInsertUpdate()
        {
            try
            {
                if (!List_MainMenu.Any(x => x.Data1.ToString() == "menuButtonUpdateRestart"))
                {
                    DataBindString menuButtonUpdateRestart = new DataBindString
                    {
                        ImageBitmap = await FileToBitmapImage(new AVImageFile()
                        {
                            FilePaths = ["Assets/Default/Icons/Refresh.png"],
                            BackupPath = vImageBackupSource,
                            Dispatcher = this.Dispatcher
                        }),
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