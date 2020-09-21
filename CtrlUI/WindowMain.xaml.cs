using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessWin32Functions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Settings;

namespace CtrlUI
{
    public partial class WindowMain : Window
    {
        //Window Initialize
        public WindowMain() { InitializeComponent(); }

        //Window Variables
        public static IntPtr vInteropWindowHandle = IntPtr.Zero;

        //Window Initialized
        protected override async void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Get interop window handle
                vInteropWindowHandle = new WindowInteropHelper(this).EnsureHandle();

                //Register Hotkeys and Filtermessage
                ComponentDispatcher.ThreadFilterMessage += ReceivedFilterMessage;
                RegisterHotKey(vInteropWindowHandle, HotKeyRegisterId, (byte)KeysModifier.Win, KeysVirtual.CapsLock);

                //Check application settings
                Settings_Check();
                Settings_Load();
                Settings_Save();

                //Update the window position
                await UpdateWindowPosition(false, true);

                //Change application accent color
                Settings_Load_AccentColor(vConfigurationCtrlUI);

                //Set the application background media
                UpdateBackgroundMedia();

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

                //Check settings if need to start in fullscreen of minimized
                if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "LaunchFullscreen")))
                {
                    await AppSwitchScreenMode(true, false);
                }
                else if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "LaunchMinimized")))
                {
                    await AppMinimize(false);
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

                //Register Interface Handlers
                RegisterInterfaceHandlers();

                //Register application events
                RegisterApplicationEvents();

                //Bind all the lists to ListBox
                ListBoxBindLists();

                //Select the first ListBox item
                ListBoxResetIndexes();

                //Load Json stored apps
                await JsonLoadList_Applications();

                //Load Json profiles
                JsonLoadProfile(ref vCtrlCloseLaunchers, "CtrlCloseLaunchers");
                JsonLoadProfile(ref vCtrlLocationsFile, "CtrlLocationsFile");
                JsonLoadProfile(ref vCtrlLocationsShortcut, "CtrlLocationsShortcut");
                JsonLoadProfile(ref vCtrlIgnoreLauncherName, "CtrlIgnoreLauncherName");
                JsonLoadProfile(ref vCtrlIgnoreShortcutName, "CtrlIgnoreShortcutName");
                JsonLoadProfile(ref vCtrlIgnoreShortcutUri, "CtrlIgnoreShortcutUri");
                JsonLoadProfile(ref vCtrlIgnoreProcessName, "CtrlIgnoreProcessName");
                JsonLoadProfile(ref vCtrlKeyboardExtensionName, "CtrlKeyboardExtensionName");
                JsonLoadProfile(ref vCtrlKeyboardProcessName, "CtrlKeyboardProcessName");

                //Load Json lists
                JsonLoadEmbedded(ref vApiIGDBGenres, "CtrlUI.Resources.IGDB.Genres.json");
                JsonLoadEmbedded(ref vApiIGDBPlatforms, "CtrlUI.Resources.IGDB.Platforms.json");

                //Update uwp application images
                UpdateUwpApplicationImages();

                //Start the background tasks
                TasksBackgroundStart();

                //Enable the socket server
                EnableSocketServer();

                //Launch DirectXInput application
                await LaunchDirectXInput(true);

                //Check settings if Fps Overlayer launches on start
                if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "LaunchFpsOverlayer")))
                {
                    await LaunchFpsOverlayer(false);
                }

                //Force window focus on CtrlUI
                if (!Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "LaunchMinimized")))
                {
                    await PrepareFocusProcessWindow("CtrlUI", vProcessCurrent.Id, vProcessCurrent.MainWindowHandle, 0, false, true, true, false);
                }

                //Check settings if this is the first application launch
                if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "AppFirstLaunch")))
                {
                    await AddFirstLaunchApps();
                }

                //Focus on the first available listbox
                if (vMainMenuElementFocus.FocusElement == null || Keyboard.FocusedElement == null)
                {
                    Debug.WriteLine("Focusing on the first available listbox.");
                    await ListboxFocusIndex(TopVisibleListBoxWithItems(), true, false, -1);
                }

                //Update the controller help
                UpdateControllerHelp();

                //Update the controller connection status
                await UpdateControllerConnected();

                //Check for available application update
                if (await CheckForAppUpdate(true))
                {
                    MainMenuInsertUpdate();
                }
            }
            catch { }
        }

        //Enable the socket server
        private void EnableSocketServer()
        {
            try
            {
                int SocketServerPort = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "ServerPort"));

                vArnoldVinkSockets = new ArnoldVinkSockets("127.0.0.1", SocketServerPort);
                vArnoldVinkSockets.vTcpClientTimeout = 250;
                vArnoldVinkSockets.EventBytesReceived += ReceivedSocketHandler;
            }
            catch { }
        }

        //Show the Windows start menu
        async Task ShowWindowStartMenu()
        {
            try
            {
                await Notification_Send_Status("Windows", "Showing start menu");
                await KeyPressSingleAuto(KeysVirtual.LeftWindows);

                //Launch the keyboard controller
                if (vAppActivated && vControllerAnyConnected())
                {
                    await KeyboardControllerHideShow(true);
                }
            }
            catch { }
        }

        //Move application to the next monitor
        async Task AppMoveMonitor()
        {
            try
            {
                //Check if there are multiple monitors
                int totalScreenCount = Screen.AllScreens.Count();
                if (totalScreenCount == 1)
                {
                    Debug.WriteLine("Only one monitor");
                    await Notification_Send_Status("MonitorNext", "Only one monitor");

                    //Save the new monitor number
                    Setting_Save(vConfigurationCtrlUI, "DisplayMonitor", "1");

                    //Update the window position
                    await UpdateWindowPosition(true, true);
                    return;
                }

                //Check the next target monitor
                int monitorNumber = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "DisplayMonitor"));
                if (monitorNumber >= totalScreenCount)
                {
                    monitorNumber = 1;
                }
                else
                {
                    monitorNumber++;
                }

                //Save the new monitor number
                Setting_Save(vConfigurationCtrlUI, "DisplayMonitor", monitorNumber.ToString());

                //Update the window position
                await UpdateWindowPosition(true, false);
            }
            catch { }
        }

        //Update the window position
        async Task UpdateWindowPosition(bool notifyApps, bool silent)
        {
            try
            {
                //Check if the application is maximized
                bool isMaximized = vAppMaximized;
                if (isMaximized)
                {
                    await AppSwitchScreenMode(false, true);
                }

                //Get the current active screen
                int monitorNumber = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "DisplayMonitor"));
                DisplayMonitorSettings displayMonitorSettings = GetScreenSettings(monitorNumber);

                //Get the current window size
                int windowWidth = (int)(this.ActualWidth * displayMonitorSettings.DpiScaleHorizontal);
                int windowHeight = (int)(this.ActualHeight * displayMonitorSettings.DpiScaleVertical);

                //Check and resize the target window size
                if (windowWidth > displayMonitorSettings.WidthNative || windowHeight > displayMonitorSettings.HeightNative)
                {
                    windowWidth = displayMonitorSettings.WidthNative;
                    windowHeight = displayMonitorSettings.HeightNative;
                    WindowResize(vInteropWindowHandle, windowWidth, windowHeight);
                }

                //Center the window on target screen
                int horizontalLeft = (int)(displayMonitorSettings.BoundsLeft + (displayMonitorSettings.WidthNative - windowWidth) / 2);
                int verticalTop = (int)(displayMonitorSettings.BoundsTop + (displayMonitorSettings.HeightNative - windowHeight) / 2);
                WindowMove(vInteropWindowHandle, horizontalLeft, verticalTop);

                //Restore the previous screen mode
                if (isMaximized)
                {
                    await AppSwitchScreenMode(true, false);
                }

                //Notify apps the monitor changed
                if (notifyApps)
                {
                    await NotifyDirectXInputSettingChanged("DisplayMonitor");
                    await NotifyFpsOverlayerSettingChanged("DisplayMonitor");
                }

                //Show monitor change notification
                if (!silent)
                {
                    await Notification_Send_Status("MonitorNext", "Moved to monitor " + monitorNumber);
                }

                Debug.WriteLine("Moved the application to monitor: " + monitorNumber);
            }
            catch { }
        }

        //Make sure the correct window style is set
        void CheckWindowStateAndStyle(object sender, EventArgs e)
        {
            try
            {
                if (WindowState == WindowState.Maximized && WindowStyle != WindowStyle.None) { WindowStyle = WindowStyle.None; }
                else if (WindowState == WindowState.Normal && WindowStyle != WindowStyle.SingleBorderWindow) { WindowStyle = WindowStyle.SingleBorderWindow; }
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
                await ProcessLauncherWin32Async("CtrlUI.exe", "", "-restart", false, false);
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
                    await PrepareFocusProcessWindow("CtrlUI", vProcessCurrent.Id, vProcessCurrent.MainWindowHandle, 0, false, true, true, false);
                }

                //Show the closing messagebox
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppClose.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                Answer1.Name = "Close CtrlUI";
                Answers.Add(Answer1);

                DataBindString Answer4 = new DataBindString();
                Answer4.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                Answer4.Name = "Restart CtrlUI";
                Answers.Add(Answer4);

                DataBindString Answer3 = new DataBindString();
                Answer3.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Shutdown.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                Answer3.Name = "Shutdown my PC";
                Answers.Add(Answer3);

                DataBindString Answer2 = new DataBindString();
                Answer2.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Restart.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
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
                        await ProcessLauncherWin32Async(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\shutdown.exe", "", "/r /t 0", false, true);

                        //Close CtrlUI
                        await Application_Exit();
                    }
                    else if (messageResult == Answer3)
                    {
                        await Notification_Send_Status("Shutdown", "Shutting down your PC");

                        //Close all other launchers
                        await CloseLaunchers(true);

                        //Shutdown the PC
                        await ProcessLauncherWin32Async(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\shutdown.exe", "", "/s /t 0", false, true);

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