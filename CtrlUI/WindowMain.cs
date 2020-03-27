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
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessWin32Functions;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    public partial class WindowMain : Window
    {
        //Window Initialize
        public WindowMain() { InitializeComponent(); }

        //Window Initialized
        protected override void OnSourceInitialized(EventArgs e)
        {
            try
            {
                IntPtr InteropHandle = new WindowInteropHelper(this).EnsureHandle();

                ComponentDispatcher.ThreadFilterMessage += ReceivedFilterMessage;

                RegisterHotKey(InteropHandle, HotKeyRegisterId, (byte)KeysModifiers.Win, (byte)KeysVirtual.CapsLock);

                Debug.WriteLine("Application source initialized.");
            }
            catch { }
        }

        //Window Startup
        public async Task Startup()
        {
            try
            {
                //Initialize Settings
                Settings_Check();
                await Settings_Load();
                Settings_Save();

                //Set the application background media
                UpdateBackgroundMedia();

                //Set content and resource images with Cache OnLoad
                SetContentResourceXamlImages();

                //Adjust the application font size
                AdjustApplicationFontSize();

                //Check if application has launched as admin
                if (vAdministratorPermission)
                {
                    this.Title += " (Admin)";
                }

                //Check settings if need to start in fullscreen of minimized
                if (ConfigurationManager.AppSettings["LaunchFullscreen"] == "True")
                {
                    await AppSwitchScreenMode(true, false);
                }
                else if (ConfigurationManager.AppSettings["LaunchMinimized"] == "True")
                {
                    await AppMinimize(false);
                }

                //Restore the last known window size and center the application
                if (ConfigurationManager.AppSettings["LaunchFullscreen"] == "False" && ConfigurationManager.AppSettings["LaunchMinimized"] == "False")
                {
                    AdjustScreenSizeMonitor(Convert.ToInt32(ConfigurationManager.AppSettings["DisplayMonitor"]), true, true, true, true);
                }

                //Workaround for 64bit Windows problems with System32
                Wow64DisableWow64FsRedirection(IntPtr.Zero);

                //Registry enable linked connections
                RegistryEnableLinkedConnections();

                //Change application accent color
                ChangeApplicationAccentColor();

                //Update the clock
                UpdateClock();

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
                JsonLoadProfile(ref vCtrlKeyboardProcessName, "CtrlKeyboardProcessName");

                //Update uwp application images
                UpdateUwpApplicationImages();

                //Start application tasks
                TasksBackgroundStart();

                //Check settings if DirectXInput launches on start
                if (ConfigurationManager.AppSettings["LaunchDirectXInput"] == "True")
                {
                    LaunchDirectXInput();
                }

                //Check settings if Fps Overlayer launches on start
                if (ConfigurationManager.AppSettings["LaunchFpsOverlayer"] == "True")
                {
                    LaunchFpsOverlayer();
                }

                //Force window focus on CtrlUI
                if (ConfigurationManager.AppSettings["LaunchMinimized"] == "False")
                {
                    FocusProcessWindowPrepare("CtrlUI", vProcessCurrent.Id, vProcessCurrent.MainWindowHandle, 0, false, true, true, false);
                }

                //Check settings if this is the first application launch
                if (ConfigurationManager.AppSettings["AppFirstLaunch"] == "True")
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

                Debug.WriteLine("Application has launched.");
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
                int ScreenCount = Screen.AllScreens.Count();
                if (ScreenCount == 1)
                {
                    Debug.WriteLine("Only one monitor");
                    Popup_Show_Status("MonitorNext", "Only one monitor");
                    SettingSave("DisplayMonitor", "0");
                    return;
                }

                if (vAppCurrentMonitor + 1 >= ScreenCount)
                {
                    vAppCurrentMonitor = 0;
                }
                else
                {
                    vAppCurrentMonitor++;
                }

                bool IsMaximized = vAppMaximized;
                if (IsMaximized)
                {
                    await AppSwitchScreenMode(false, true);
                }

                AdjustScreenSizeMonitor(vAppCurrentMonitor, false, true, true, false);
                SettingSave("DisplayMonitor", vAppCurrentMonitor.ToString());

                if (IsMaximized)
                {
                    await AppSwitchScreenMode(true, false);
                }
            }
            catch { }
        }

        void AdjustScreenSizeMonitor(int MonitorNumber, bool SettingSize, bool ResizeWindow, bool CenterScreen, bool Silent)
        {
            try
            {
                //Get the current active screen
                Screen targetScreen = GetActiveScreen(MonitorNumber);

                int ScreenWidth = targetScreen.WorkingArea.Width;
                int ScreenHeight = targetScreen.WorkingArea.Height;

                int WindowWidth = 0;
                int WindowHeight = 0;
                if (SettingSize)
                {
                    WindowWidth = Convert.ToInt32(ConfigurationManager.AppSettings["WindowSizeWidth"]);
                    WindowHeight = Convert.ToInt32(ConfigurationManager.AppSettings["WindowSizeHeight"]);
                }
                else
                {
                    WindowWidth = Convert.ToInt32(this.ActualWidth);
                    WindowHeight = Convert.ToInt32(this.ActualHeight);
                }

                if (WindowWidth > ScreenWidth) { WindowWidth = ScreenWidth; }
                if (WindowHeight > ScreenHeight) { WindowHeight = ScreenHeight; }

                //Resize the application window
                if (ResizeWindow)
                {
                    this.Width = WindowWidth;
                    this.Height = WindowHeight;
                }

                //Center the window on screen
                if (CenterScreen)
                {
                    int HorizontalCenter = Convert.ToInt32((ScreenWidth - WindowWidth) / 2);
                    int VerticalCenter = Convert.ToInt32((ScreenHeight - WindowHeight) / 2);
                    this.Top = targetScreen.WorkingArea.Top + VerticalCenter;
                    this.Left = targetScreen.WorkingArea.Left + HorizontalCenter;
                }
                else
                {
                    this.Top = targetScreen.WorkingArea.Top;
                    this.Left = targetScreen.WorkingArea.Left;
                }

                Debug.WriteLine("Moved the application to monitor: " + vAppCurrentMonitor);
                if (!Silent)
                {
                    Popup_Show_Status("MonitorNext", "Moved to monitor " + vAppCurrentMonitor);
                }
            }
            catch { }
        }

        //Get the current active screen
        public Screen GetActiveScreen(int MonitorNumber)
        {
            Screen targetScreen = null;
            try
            {
                if (MonitorNumber > 0)
                {
                    try
                    {
                        targetScreen = Screen.AllScreens[MonitorNumber];
                        vAppCurrentMonitor = MonitorNumber;
                    }
                    catch
                    {
                        targetScreen = Screen.PrimaryScreen;
                        vAppCurrentMonitor = 0;
                    }
                }
                else
                {
                    targetScreen = Screen.PrimaryScreen;
                    vAppCurrentMonitor = 0;
                }
            }
            catch { }
            return targetScreen;
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
                await ProcessLauncherWin32Async("CtrlUI.exe", "", "", false, false);
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
                FocusProcessWindowPrepare("CtrlUI", vProcessCurrent.Id, vProcessCurrent.MainWindowHandle, 0, false, true, true, false);

                //Show the closing messagebox
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Closing.png" }, IntPtr.Zero, -1, 0);
                Answer1.Name = "Close CtrlUI";
                Answers.Add(Answer1);

                DataBindString Answer4 = new DataBindString();
                Answer4.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Restart.png" }, IntPtr.Zero, -1, 0);
                Answer4.Name = "Restart CtrlUI";
                Answers.Add(Answer4);

                DataBindString Answer3 = new DataBindString();
                Answer3.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Shutdown.png" }, IntPtr.Zero, -1, 0);
                Answer3.Name = "Shutdown my PC";
                Answers.Add(Answer3);

                DataBindString Answer2 = new DataBindString();
                Answer2.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Restart.png" }, IntPtr.Zero, -1, 0);
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

                //Stop the background tasks
                TasksBackgroundStop();

                //Disable the socket server
                await vArnoldVinkSockets.SocketServerDisable();

                //Close the application
                Environment.Exit(0);
            }
            catch { }
        }
    }
}