using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using static CtrlUI.AppVariables;
using static LibraryShared.AppImport;
using static LibraryShared.Classes;
using static LibraryShared.OutputKeyboard;
using static LibraryShared.Processes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Window Initialize
        public WindowMain() { InitializeComponent(); }

        //Window Initialized
        protected override void OnSourceInitialized(EventArgs e)
        {
            try
            {
                IntPtr InteropHandle = new WindowInteropHelper(this).Handle;

                ComponentDispatcher.ThreadFilterMessage += ReceiveInput;

                RegisterHotKey(InteropHandle, HotKeyRegisterId, (byte)KeysModifiers.Win, (byte)KeysVirtual.CapsLock);

                Debug.WriteLine("Application source initialized.");
            }
            catch { }
        }

        public static byte[] GetStringToBytes(string value)
        {
            SoapHexBinary shb = SoapHexBinary.Parse(value);
            return shb.Value;
        }

        public static string GetBytesToString(byte[] value)
        {
            SoapHexBinary shb = new SoapHexBinary(value);
            return shb.ToString();
        }

        //Window Startup
        public async Task Startup()
        {
            try
            {
                //Initialize Settings
                Settings_Check();
                Settings_LoadSocket();
                await Settings_Load();
                Settings_Save();

                //Set the application background image
                UpdateBackgroundImage();

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
                if (ConfigurationManager.AppSettings["LaunchFullscreen"] == "True") { AppSwitchScreenMode(true, false); }
                else if (ConfigurationManager.AppSettings["LaunchMinimized"] == "True") { await AppMinimize(false); }

                //Restore the last known window size and center the application
                if (ConfigurationManager.AppSettings["LaunchFullscreen"] == "False" && ConfigurationManager.AppSettings["LaunchMinimized"] == "False")
                {
                    AdjustScreenSizeMonitor(Convert.ToInt32(ConfigurationManager.AppSettings["DisplayMonitor"]), true, true, true, true);
                }

                //Workaround for 64bit Windows problems with System32
                Wow64DisableWow64FsRedirection(IntPtr.Zero);

                //Registry enable linked connections
                RegistryEnableLinkedConnections();

                //Update the clock
                UpdateClock();

                //Load the help text
                LoadHelp();

                //Register Interface Handlers
                RegisterInterfaceHandlers();

                //Bind all the lists to ListBox
                ListBoxBindLists();

                //Select the first ListBox item
                ListBoxSelectIndexes();

                //Load Json stored apps
                JsonLoadApps();

                //Load Json apps other launchers
                JsonLoadAppsOtherLaunchers();

                //Load Json file locations
                JsonLoadFileLocations();

                //Load Json apps blacklist shortcuts
                JsonLoadAppsBlacklistShortcut();

                //Load Json apps blacklist shortcuts uri
                JsonLoadAppsBlacklistShortcutUri();

                //Load Json apps blacklist process
                JsonLoadAppsBlacklistProcess();

                //Refresh the application lists stats
                ShowHideEmptyList(false, false);
                ListsUpdateCount();

                //Validate application numbers
                ValidateAppNumbers();

                //Update uwp application images
                UpdateUwpAppImages();

                //Start application tasks
                TasksBackgroundStart();

                //Focus on the first available listbox
                if (vMainMenuPreviousFocus == null)
                {
                    await FocusOnListbox(TopVisibleListBox(), true, false, -1, true);
                }

                //Check settings if DirectXInput launches on start
                if (ConfigurationManager.AppSettings["LaunchDirectXInput"] == "True")
                {
                    if (!CheckRunningProcessByName("DirectXInput", false))
                    {
                        await ProcessLauncherWin32Prepare("DirectXInput-Admin.exe", "", "", true, true, false);
                    }
                }

                //Check settings if Fps Overlayer launches on start
                if (ConfigurationManager.AppSettings["LaunchFpsOverlayer"] == "True")
                {
                    if (!CheckRunningProcessByName("FpsOverlayer", false))
                    {
                        await ProcessLauncherWin32Prepare("FpsOverlayer-Admin.exe", "", "", true, true, false);
                        await Task.Delay(1000);
                    }
                }

                //Force focus on CtrlUI
                if (ConfigurationManager.AppSettings["LaunchMinimized"] == "False")
                {
                    FocusWindowHandlePrepare("CtrlUI", Process.GetCurrentProcess().MainWindowHandle, 0, false, true, true, true, true, true);
                }

                //Check settings if this is the first application launch
                if (ConfigurationManager.AppSettings["AppFirstLaunch"] == "True") { await AddFirstLaunchApps(); }

                //Check for available application update
                if (DateTime.Now.Subtract(DateTime.Parse(ConfigurationManager.AppSettings["AppUpdateCheck"], vAppCultureInfo)).Days >= 5)
                {
                    await CheckForAppUpdate(true);
                }

                //Update the controller help
                UpdateControllerHelp();

                //Update the controller connection status
                UpdateControllerConnected();

                //Enable the socket server
                await vSocketServer.SocketServerSwitch(false, false);
                vSocketServer.EventBytesReceived += ReceivedSocketHandler;

                Debug.WriteLine("Application has launched.");
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

        //Switch the display monitor
        void SwitchDisplayMonitor()
        {
            try
            {
                Popup_Show_Status("MonitorSwitch", "Switching display monitor");
                KeyPressCombo((byte)KeysVirtual.LeftWindows, (byte)KeysVirtual.P, false);
            }
            catch { }
        }

        //Move application to the next monitor
        void AppMoveMonitor()
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
                if (IsMaximized) { AppSwitchScreenMode(false, true); }
                AdjustScreenSizeMonitor(vAppCurrentMonitor, false, true, true, false);
                SettingSave("DisplayMonitor", vAppCurrentMonitor.ToString());
                if (IsMaximized) { AppSwitchScreenMode(true, false); }
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
                await Application_Exit(false);
            }
            catch { }
        }

        //Close the application
        async Task Application_Exit(bool SilentClose)
        {
            try
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Closing.png" }, IntPtr.Zero, -1);
                Answer1.Name = "Close CtrlUI";
                Answers.Add(Answer1);

                DataBindString cancelString = new DataBindString();
                cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                cancelString.Name = "Cancel";
                Answers.Add(cancelString);

                DataBindString Result = null;

                if (!SilentClose)
                {
                    Result = await Popup_Show_MessageBox("Do you really want to close CtrlUI?", "If you have DirectXInput running and a controller connected you can launch CtrlUI by pressing on the 'Guide' button.", "", Answers);
                }

                if (SilentClose || (Result != null && Result == Answer1))
                {
                    CloseProcessesByName("KeyboardController", false);
                    TasksBackgroundStop();

                    vSocketClient.SocketClientDisconnectAll();
                    await vSocketServer.SocketServerDisable();

                    Environment.Exit(0);
                }
            }
            catch { }
        }
    }
}