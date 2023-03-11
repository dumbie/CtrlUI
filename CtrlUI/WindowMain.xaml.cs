using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVJsonFunctions;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.Styles.MainColors;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    public partial class WindowMain : Window
    {
        //Window Initialize
        public WindowMain() { InitializeComponent(); }

        //Window Variables
        private IntPtr vInteropWindowHandle = IntPtr.Zero;

        //Window Initialized
        protected override async void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Get interop window handle
                vInteropWindowHandle = new WindowInteropHelper(this).EnsureHandle();

                //Register filter message
                ComponentDispatcher.ThreadFilterMessage += ReceivedFilterMessage;

                //Check application settings
                Settings_Check();
                await Settings_Load();
                Settings_Save();

                //Update the window position
                await UpdateWindowPosition(false, true);

                //Check if resolution has changed
                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

                //Change application accent color
                string colorLightHex = SettingLoad(vConfigurationCtrlUI, "ColorAccentLight", typeof(string));
                ChangeApplicationAccentColor(colorLightHex);

                //Set the application clock style
                UpdateClockStyle();

                //Set content and resource images with Cache OnLoad
                SetContentResourceXamlImages();

                //Adjust the application font family
                UpdateAppFontStyle();

                //Adjust the application font size
                AdjustApplicationFontSize();

                //Check if application has launched as admin
                if (vAdministratorPermission)
                {
                    this.Title += " (Admin)";
                }

                //Check settings if need to start minimized
                if (SettingLoad(vConfigurationCtrlUI, "LaunchMinimized", typeof(bool)))
                {
                    await AppWindowMinimize(false, true);
                }

                //Workaround for 64bit Windows problems with System32
                Wow64DisableWow64FsRedirection(IntPtr.Zero);

                //Registry enable linked connections
                RegistryEnableLinkedConnections();

                //Update the clock time
                UpdateClockTime();

                //Load the help text
                LoadHelp();

                //Add main menu items
                MainMenuAddItems();

                //Add categories to edit interface
                ManageInterface_AddCategories();

                //Register Interface Handlers
                RegisterInterfaceHandlers();

                //Bind all the lists to ListBox
                ListBoxBindLists();

                //Select the first ListBox item
                ListBoxResetIndexes();

                //Update IGDB api files
                await ApiIGDB_UpdateFiles();

                //Backup Json profiles
                ProfileMakeBackup();

                //Load Json stored apps
                await JsonLoadList_Applications();

                //Load Json default profiles
                JsonLoadFile(ref vCtrlChromiumBrowsers, @"Profiles\Default\CtrlChromiumBrowsers.json");
                JsonLoadFile(ref vCtrlCloseLaunchers, @"Profiles\Default\CtrlCloseLaunchers.json");
                JsonLoadFile(ref vCtrlIgnoreProcessName, @"Profiles\Default\CtrlIgnoreProcessName.json");

                //Load Json user profiles
                JsonLoadFile(ref vCtrlIgnoreLauncherName, @"Profiles\User\CtrlIgnoreLauncherName.json");
                JsonLoadFile(ref vCtrlIgnoreShortcutName, @"Profiles\User\CtrlIgnoreShortcutName.json");
                JsonLoadFile(ref vCtrlKeyboardExtensionName, @"Profiles\User\CtrlKeyboardExtensionName.json");
                JsonLoadFile(ref vCtrlKeyboardProcessName, @"Profiles\User\CtrlKeyboardProcessName.json");
                JsonLoadFile(ref vCtrlLocationsFile, @"Profiles\User\CtrlLocationsFile.json");
                JsonLoadFile(ref vCtrlLocationsShortcut, @"Profiles\User\CtrlLocationsShortcut.json");

                //Load Json lists
                JsonLoadFile(ref vApiIGDBGenres, @"Resources\ApiIGDB\Genres.json");
                JsonLoadFile(ref vApiIGDBPlatforms, @"Resources\ApiIGDB\Platforms.json");

                //Start the background tasks
                TasksBackgroundStart();

                //Check settings if DirectXInput launches on start
                if (SettingLoad(vConfigurationCtrlUI, "LaunchDirectXInput", typeof(bool)))
                {
                    await LaunchDirectXInput(true);
                }

                //Check settings if Fps Overlayer launches on start
                if (SettingLoad(vConfigurationCtrlUI, "LaunchFpsOverlayer", typeof(bool)))
                {
                    await LaunchFpsOverlayer(false);
                }

                //Force window focus on CtrlUI
                if (!SettingLoad(vConfigurationCtrlUI, "LaunchMinimized", typeof(bool)))
                {
                    //Show the CtrlUI window
                    await AppWindowShow(true);

                    //Prevent or allow monitor sleep
                    UpdateMonitorSleepAuto();
                }

                //Check settings if this is the first application launch
                if (SettingLoad(vConfigurationCtrlUI, "AppFirstLaunch", typeof(bool)))
                {
                    await AddFirstLaunchApps();
                }

                //Update the controller help
                UpdateControllerHelp();

                //Update the controller connection status
                await UpdateControllerConnected();

                //Update the controller color
                UpdateControllerColor();

                //Enable the socket server
                await EnableSocketServer();

                //Switch to last used list category
                await CategoryListSwitchToSetting();

                //Check for available application update
                await CheckForAppUpdate(true);
            }
            catch { }
        }

        //Enable the socket server
        private async Task EnableSocketServer()
        {
            try
            {
                int SocketServerPort = SettingLoad(vConfigurationCtrlUI, "ServerPort", typeof(int));

                vArnoldVinkSockets = new ArnoldVinkSockets("127.0.0.1", SocketServerPort, false, true);
                vArnoldVinkSockets.vSocketTimeout = 250;
                vArnoldVinkSockets.EventBytesReceived += ReceivedSocketHandler;
                await vArnoldVinkSockets.SocketServerEnable();
            }
            catch { }
        }

        //Application Close Handler
        protected async override void OnClosing(CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                await Application_Exit_Prompt();
            }
            catch { }
        }

        //Restart the application
        public async Task Application_Restart()
        {
            try
            {
                AVProcess.Launch_ExecuteInherit("CtrlUI.exe", "", "-restart", true);
                await Application_Exit();
            }
            catch { }
        }

        //Application close prompt
        async Task Application_Exit_Prompt()
        {
            try
            {
                //Force focus on CtrlUI
                if (!vAppActivated)
                {
                    await PrepareShowProcessWindow("CtrlUI", vProcessCurrent.Identifier, vProcessCurrent.WindowHandleMain, false, true, false);
                }

                //Show the closing messagebox
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppClose.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                Answer1.Name = "Close CtrlUI";
                Answers.Add(Answer1);

                DataBindString Answer4 = new DataBindString();
                Answer4.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                Answer4.Name = "Restart CtrlUI";
                Answers.Add(Answer4);

                DataBindString Answer3 = new DataBindString();
                Answer3.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Shutdown.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                Answer3.Name = "Shutdown my PC";
                Answers.Add(Answer3);

                DataBindString Answer2 = new DataBindString();
                Answer2.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Restart.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                Answer2.Name = "Restart my PC";
                Answers.Add(Answer2);

                DataBindString messageResult = await Popup_Show_MessageBox("Would you like to close CtrlUI or shutdown your PC?", "If you have DirectXInput running and a controller connected you can launch CtrlUI by pressing on the 'Guide' button.", "", Answers);
                if (messageResult != null)
                {
                    if (messageResult == Answer1)
                    {
                        await Notification_Send_Status("AppClose", "Closing CtrlUI");
                        await Application_Exit();
                    }
                    else if (messageResult == Answer4)
                    {
                        await Notification_Send_Status("AppRestart", "Restarting CtrlUI");
                        await Application_Restart();
                    }
                    else if (messageResult == Answer2)
                    {
                        await Notification_Send_Status("Restart", "Restarting your PC");

                        //Close all other launchers
                        await CloseLaunchers(true);

                        //Restart the PC
                        AVProcess.Launch_ExecuteInherit(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\shutdown.exe", "", "/r /t 0", false);

                        //Close CtrlUI
                        await Application_Exit();
                    }
                    else if (messageResult == Answer3)
                    {
                        await Notification_Send_Status("Shutdown", "Shutting down your PC");

                        //Close all other launchers
                        await CloseLaunchers(true);

                        //Shutdown the PC
                        AVProcess.Launch_ExecuteInherit(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\shutdown.exe", "", "/s /t 0", false);

                        //Close CtrlUI
                        await Application_Exit();
                    }
                }
            }
            catch { }
        }

        //Close the application
        public async Task Application_Exit()
        {
            try
            {
                Debug.WriteLine("Exiting application.");

                //Disable application window
                AppWindowDisable("Closing CtrlUI, please wait.");

                //Stop the background tasks
                await TasksBackgroundStop();

                //Disable the socket server
                if (vArnoldVinkSockets != null)
                {
                    await vArnoldVinkSockets.SocketServerDisable();
                }

                //Close the application
                Environment.Exit(0);
            }
            catch { }
        }
    }
}