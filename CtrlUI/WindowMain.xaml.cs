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

                //Check settings if need to minimize or focus window
                if (SettingLoad(vConfigurationCtrlUI, "LaunchMinimized", typeof(bool)))
                {
                    //Minimize CtrlUI window
                    await AppWindowMinimize(false, true);
                }
                else
                {
                    //Prevent or allow monitor sleep
                    UpdateMonitorSleepAuto();

                    //Focus on CtrlUI window
                    await AppWindowShow(true, true);
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
                AVProcess.Launch_ShellExecute("CtrlUI.exe", "", "-restart", true);
                await Application_Exit();
            }
            catch { }
        }

        //Application close prompt
        async Task Application_Exit_Prompt()
        {
            try
            {
                //Show the closing messagebox
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString AnswerCloseCtrlUI = new DataBindString();
                AnswerCloseCtrlUI.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppClose.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerCloseCtrlUI.Name = "Close CtrlUI";
                Answers.Add(AnswerCloseCtrlUI);

                DataBindString AnswerRestartCtrlUI = new DataBindString();
                AnswerRestartCtrlUI.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerRestartCtrlUI.Name = "Restart CtrlUI";
                Answers.Add(AnswerRestartCtrlUI);

                DataBindString AnswerShutdownPC = new DataBindString();
                AnswerShutdownPC.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Shutdown.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerShutdownPC.Name = "Shutdown my PC";
                Answers.Add(AnswerShutdownPC);

                DataBindString AnswerRestartPC = new DataBindString();
                AnswerRestartPC.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Restart.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerRestartPC.Name = "Restart my PC";
                Answers.Add(AnswerRestartPC);

                DataBindString AnswerLockPC = new DataBindString();
                AnswerLockPC.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Lock.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerLockPC.Name = "Lock my PC";
                Answers.Add(AnswerLockPC);

                DataBindString messageResult = await Popup_Show_MessageBox("Would you like to close CtrlUI or shutdown your PC?", "If you have DirectXInput running and a controller connected you can launch CtrlUI by pressing on the 'Guide' button.", "", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerCloseCtrlUI)
                    {
                        await Notification_Send_Status("AppClose", "Closing CtrlUI");
                        await Application_Exit();
                    }
                    else if (messageResult == AnswerRestartCtrlUI)
                    {
                        await Notification_Send_Status("AppRestart", "Restarting CtrlUI");
                        await Application_Restart();
                    }
                    else if (messageResult == AnswerRestartPC)
                    {
                        await Notification_Send_Status("Restart", "Restarting your PC");

                        //Restart the PC
                        AVProcess.Launch_ShellExecute(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\shutdown.exe", "", "/r /f /t 0", true);

                        //Close CtrlUI
                        await Application_Exit();
                    }
                    else if (messageResult == AnswerShutdownPC)
                    {
                        await Notification_Send_Status("Shutdown", "Shutting down your PC");

                        //Shutdown the PC
                        AVProcess.Launch_ShellExecute(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\shutdown.exe", "", "/s /f /t 0", true);

                        //Close CtrlUI
                        await Application_Exit();
                    }
                    else if (messageResult == AnswerLockPC)
                    {
                        await Notification_Send_Status("Lock", "Locking your PC");

                        //Lock the PC
                        LockWorkStation();
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