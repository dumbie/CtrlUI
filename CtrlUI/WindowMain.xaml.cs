using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
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
                //Register Hotkeys and Filtermessage
                vInteropWindowHandle = new WindowInteropHelper(this).EnsureHandle();
                ComponentDispatcher.ThreadFilterMessage += ReceivedFilterMessage;
                RegisterHotKey(vInteropWindowHandle, HotKeyRegisterId, (byte)KeysModifiers.Win, (byte)KeysVirtual.CapsLock);

                //Check application settings
                Settings_Check();
                Settings_Load_DirectXInput(ref vConfigurationDirectXInput);
                Settings_Load();
                Settings_Save();

                //Change application accent color
                Settings_Load_AccentColor(null);

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
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["LaunchFullscreen"]))
                {
                    await AppSwitchScreenMode(true, false);
                }
                else if (Convert.ToBoolean(ConfigurationManager.AppSettings["LaunchMinimized"]))
                {
                    await AppMinimize(false);
                }

                //Restore the last known window size and center the application
                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["LaunchMinimized"]))
                {
                    int monitorNumber = Convert.ToInt32(ConfigurationManager.AppSettings["DisplayMonitor"]);
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["LaunchFullscreen"]))
                    {
                        await UpdateWindowPosition(monitorNumber, false, false, true);
                    }
                    else
                    {
                        await UpdateWindowPosition(monitorNumber, true, true, true);
                    }
                }

                //Workaround for 64bit Windows problems with System32
                Wow64DisableWow64FsRedirection(IntPtr.Zero);

                //Registry enable linked connections
                RegistryEnableLinkedConnections();

                //Update the clock time
                UpdateClockTime();

                //Load the help text
                LoadHelp();

                //Register Interface Handlers
                RegisterInterfaceHandlers();

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

                //Start application tasks
                TasksBackgroundStart();

                //Launch DirectXInput application
                await LaunchDirectXInput();

                //Check settings if Fps Overlayer launches on start
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["LaunchFpsOverlayer"]))
                {
                    await LaunchFpsOverlayer();
                }

                //Force window focus on CtrlUI
                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["LaunchMinimized"]))
                {
                    await PrepareFocusProcessWindow("CtrlUI", vProcessCurrent.Id, vProcessCurrent.MainWindowHandle, 0, false, true, true, false);
                }

                //Check settings if this is the first application launch
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["AppFirstLaunch"]))
                {
                    await AddFirstLaunchApps();
                }

                //Focus on the first available listbox
                if (vMainMenuElementFocus.FocusElement == null || Keyboard.FocusedElement == null)
                {
                    Debug.WriteLine("Focusing on the first available listbox.");
                    await ListboxFocusIndex(TopVisibleListBoxWithItems(), true, false, -1);
                }

                //Check for available application update
                if (DateTime.Now.Subtract(DateTime.Parse(ConfigurationManager.AppSettings["AppUpdateCheck"], vAppCultureInfo)).Days >= 5)
                {
                    await CheckForAppUpdate(true);
                }

                //Update the controller help
                UpdateControllerHelp();

                //Update the controller connection status
                await UpdateControllerConnected();

                //Enable the socket server
                EnableSocketServer();
            }
            catch { }
        }

        //Enable the socket server
        private void EnableSocketServer()
        {
            try
            {
                int SocketServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["ServerPort"]);

                vArnoldVinkSockets = new ArnoldVinkSockets("127.0.0.1", SocketServerPort);
                vArnoldVinkSockets.vTcpClientTimeout = 250;
                vArnoldVinkSockets.EventBytesReceived += ReceivedSocketHandler;
            }
            catch { }
        }

        //Show the Windows start menu
        void ShowWindowStartMenu()
        {
            try
            {
                Popup_Show_Status("Windows", "Showing the start menu");
                KeyPressSingle((byte)KeysVirtual.LeftWindows, false);
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
                    Popup_Show_Status("MonitorNext", "Only one monitor");
                    SettingSave("DisplayMonitor", "0");
                    return;
                }

                //Check the next target monitor
                int monitorNumber = Convert.ToInt32(ConfigurationManager.AppSettings["DisplayMonitor"]);
                if (monitorNumber + 1 >= totalScreenCount)
                {
                    monitorNumber = 0;
                }
                else
                {
                    monitorNumber++;
                }

                //Move to the next monitor
                await UpdateWindowPosition(monitorNumber, false, true, false);

                //Save the new monitor number
                SettingSave("DisplayMonitor", monitorNumber.ToString());
            }
            catch { }
        }

        //Update the window position
        async Task UpdateWindowPosition(int monitorNumber, bool settingSize, bool resizeWindow, bool silent)
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
                Screen targetScreen = GetScreenByNumber(monitorNumber, out bool monitorSuccess);
                int screenWidth = targetScreen.WorkingArea.Width;
                int screenHeight = targetScreen.WorkingArea.Height;

                int windowWidth = 0;
                int windowHeight = 0;
                if (settingSize)
                {
                    windowWidth = Convert.ToInt32(ConfigurationManager.AppSettings["WindowSizeWidth"]);
                    windowHeight = Convert.ToInt32(ConfigurationManager.AppSettings["WindowSizeHeight"]);
                }
                else
                {
                    windowWidth = Convert.ToInt32(this.ActualWidth);
                    windowHeight = Convert.ToInt32(this.ActualHeight);
                }

                if (windowWidth > screenWidth) { windowWidth = screenWidth; }
                if (windowHeight > screenHeight) { windowHeight = screenHeight; }

                //Resize the application window
                if (resizeWindow)
                {
                    this.Width = windowWidth;
                    this.Height = windowHeight;
                }

                //Center the window on target screen
                int horizontalCenter = Convert.ToInt32((screenWidth - windowWidth) / 2);
                int verticalCenter = Convert.ToInt32((screenHeight - windowHeight) / 2);
                this.Top = targetScreen.WorkingArea.Top + verticalCenter;
                this.Left = targetScreen.WorkingArea.Left + horizontalCenter;

                //Restore the previous screen mode
                if (isMaximized)
                {
                    await AppSwitchScreenMode(true, false);
                }

                Debug.WriteLine("Moved the application to monitor: " + monitorNumber);
                if (!silent)
                {
                    Popup_Show_Status("MonitorNext", "Moved to monitor " + monitorNumber);
                }
            }
            catch { }
        }

        //Monitor window state changes
        void CheckWindowStateAndSize(object sender, EventArgs e)
        {
            try
            {
                //Save the current window size on change
                if (WindowState == WindowState.Normal)
                {
                    SettingSave("WindowSizeWidth", this.ActualWidth.ToString());
                    SettingSave("WindowSizeHeight", this.ActualHeight.ToString());
                }

                //Make sure the correct title style is set
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
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Closing.png" }, IntPtr.Zero, -1, 0);
                Answer1.Name = "Close CtrlUI";
                Answers.Add(Answer1);

                DataBindString Answer4 = new DataBindString();
                Answer4.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Restart.png" }, IntPtr.Zero, -1, 0);
                Answer4.Name = "Restart CtrlUI";
                Answers.Add(Answer4);

                DataBindString Answer3 = new DataBindString();
                Answer3.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Shutdown.png" }, IntPtr.Zero, -1, 0);
                Answer3.Name = "Shutdown my PC";
                Answers.Add(Answer3);

                DataBindString Answer2 = new DataBindString();
                Answer2.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Restart.png" }, IntPtr.Zero, -1, 0);
                Answer2.Name = "Restart my PC";
                Answers.Add(Answer2);

                DataBindString messageResult = await Popup_Show_MessageBox("Would you like to close CtrlUI or shutdown your PC?", "If you have DirectXInput running and a controller connected you can launch CtrlUI by pressing on the 'Guide' button.", "", Answers);
                if (messageResult != null)
                {
                    if (messageResult == Answer1)
                    {
                        Popup_Show_Status("Closing", "Closing CtrlUI");
                        await Application_Exit();
                    }
                    else if (messageResult == Answer4)
                    {
                        Popup_Show_Status("Closing", "Restarting CtrlUI");
                        await Application_Restart();
                    }
                    else if (messageResult == Answer2)
                    {
                        Popup_Show_Status("Shutdown", "Restarting your PC");

                        //Close all other launchers
                        await CloseLaunchers(true);

                        //Restart the PC
                        await ProcessLauncherWin32Async(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\shutdown.exe", "", "/r /t 0", false, true);

                        //Close CtrlUI
                        await Application_Exit();
                    }
                    else if (messageResult == Answer3)
                    {
                        Popup_Show_Status("Shutdown", "Shutting down your PC");

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
                AVActions.ActionDispatcherInvoke(delegate
                {
                    this.IsEnabled = false;
                });

                //Stop the background tasks
                TasksBackgroundStop();

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