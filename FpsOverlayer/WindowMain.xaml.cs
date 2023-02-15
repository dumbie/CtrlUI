using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVJsonFunctions;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVWindowFunctions;
using static ArnoldVinkCode.Styles.MainColors;
using static FpsOverlayer.AppTasks;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowMain : Window
    {
        //Window Initialize
        public WindowMain() { InitializeComponent(); }

        //Window Variables
        private IntPtr vInteropWindowHandle = IntPtr.Zero;
        public bool vWindowVisible = false;

        //Window Initialized
        protected override async void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Get interop window handle
                vInteropWindowHandle = new WindowInteropHelper(this).EnsureHandle();

                //Set render mode to software
                HwndSource hwndSource = HwndSource.FromHwnd(vInteropWindowHandle);
                HwndTarget hwndTarget = hwndSource.CompositionTarget;
                hwndTarget.RenderMode = RenderMode.SoftwareOnly;

                //Update the window style
                WindowUpdateStyleVisible(vInteropWindowHandle, true, true, true);

                //Check application settings
                vWindowSettings.Settings_Check();

                //Change application accent color
                string colorLightHex = SettingLoad(vConfigurationCtrlUI, "ColorAccentLight", typeof(string));
                ChangeApplicationAccentColor(colorLightHex);

                //Update the window position
                UpdateWindowPosition();

                //Update the fps overlay style
                UpdateFpsOverlayStyle();

                //Update the crosshair overlay style
                UpdateCrosshairOverlayStyle();

                //Create tray icon
                Application_CreateTrayMenu();

                //Load Json profiles
                JsonLoadFile(ref vFpsPositionProcessName, @"Profiles\User\FpsPositionProcessName.json");
                JsonLoadFile(ref vFpsBrowserLinks, @"Profiles\User\FpsBrowserLinks.json");

                //Bind all the lists to ListBox
                ListBoxBindLists();

                //Start process monitoring
                StartMonitorProcess();

                //Start taskbar monitoring
                StartMonitorTaskbar();

                //Start fps monitoring
                StartMonitorFps();

                //Start hardware monitoring
                StartMonitorHardware();

                //Show crosshair when enabled
                if (SettingLoad(vConfigurationFpsOverlayer, "CrosshairLaunch", typeof(bool)))
                {
                    ShowCrosshairVisibility();
                }

                //Show browser when enabled
                if (SettingLoad(vConfigurationFpsOverlayer, "BrowserShowStartup", typeof(bool)))
                {
                    vWindowBrowser.Show();
                }

                //Register keyboard hotkeys
                vAVInputOutputHotKey.EventHotKeyPressed += EventHotKeyPressed;
                vAVInputOutputHotKey.RegisterHotKey(KeysModifier.Alt, KeysVirtual.F8);
                vAVInputOutputHotKey.RegisterHotKey(KeysModifier.Alt, KeysVirtual.F9);
                vAVInputOutputHotKey.RegisterHotKey(KeysModifier.Alt, KeysVirtual.F10);
                vAVInputOutputHotKey.RegisterHotKey(KeysModifier.Alt, KeysVirtual.F11);

                //Enable the socket server
                await EnableSocketServer();

                //Check if resolution has changed
                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            }
            catch { }
        }

        //Bind the lists to the listbox elements
        void ListBoxBindLists()
        {
            try
            {
                polyline_Chart.Points = vPointFrameTimes;
            }
            catch { }
        }

        //Hide the window
        public new void Hide()
        {
            try
            {
                //Update the window visibility
                UpdateWindowVisibility(false);
            }
            catch { }
        }

        //Show the window
        public new void Show()
        {
            try
            {
                //Update the window visibility
                UpdateWindowVisibility(true);
            }
            catch { }
        }

        //Enable the socket server
        private async Task EnableSocketServer()
        {
            try
            {
                int SocketServerPort = SettingLoad(vConfigurationCtrlUI, "ServerPort", typeof(int)) + 2;

                vArnoldVinkSockets = new ArnoldVinkSockets("127.0.0.1", SocketServerPort, false, true);
                vArnoldVinkSockets.vSocketTimeout = 250;
                vArnoldVinkSockets.EventBytesReceived += ReceivedSocketHandler;
                await vArnoldVinkSockets.SocketServerEnable();
            }
            catch { }
        }

        //Update the window position on resolution change
        public async void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                //Wait for change to complete
                await Task.Delay(1000);

                //Update the window position
                UpdateWindowPosition();
            }
            catch { }
        }

        //Update the window visibility
        public void UpdateWindowVisibility(bool visible)
        {
            try
            {
                if (visible)
                {
                    if (!vWindowVisible)
                    {
                        //Create and show the window
                        base.Show();

                        //Update the window style
                        WindowUpdateStyleVisible(vInteropWindowHandle, true, true, true);

                        this.Title = "Fps Overlayer (Visible)";
                        vWindowVisible = true;
                        Debug.WriteLine("Showing the window.");
                    }
                }
                else
                {
                    if (vWindowVisible)
                    {
                        //Update the window style
                        WindowUpdateStyleHidden(vInteropWindowHandle);

                        this.Title = "Fps Overlayer (Hidden)";
                        vWindowVisible = false;
                        Debug.WriteLine("Hiding the window.");
                    }
                }
            }
            catch { }
        }

        //Update the window position
        public void UpdateWindowPosition()
        {
            try
            {
                //Get the current active screen
                int monitorNumber = SettingLoad(vConfigurationCtrlUI, "DisplayMonitor", typeof(int));

                //Move the window position
                WindowUpdatePosition(monitorNumber, vInteropWindowHandle, AVWindowPosition.FullScreen);
            }
            catch { }
        }

        //Adjust the application font family
        void UpdateAppFontStyle()
        {
            try
            {
                string interfaceFontStyleName = SettingLoad(vConfigurationFpsOverlayer, "InterfaceFontStyleName", typeof(string));
                if (interfaceFontStyleName == "Segoe UI" || interfaceFontStyleName == "Verdana" || interfaceFontStyleName == "Consolas" || interfaceFontStyleName == "Arial")
                {
                    this.FontFamily = new FontFamily(interfaceFontStyleName);
                }
                else
                {
                    string fontPathUser = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Assets/User/Fonts/" + interfaceFontStyleName + ".ttf";
                    string fontPathDefault = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Assets/Default/Fonts/" + interfaceFontStyleName + ".ttf";
                    if (File.Exists(fontPathUser))
                    {
                        ICollection<FontFamily> fontFamilies = Fonts.GetFontFamilies(fontPathUser);
                        this.FontFamily = fontFamilies.FirstOrDefault();
                    }
                    else if (File.Exists(fontPathDefault))
                    {
                        ICollection<FontFamily> fontFamilies = Fonts.GetFontFamilies(fontPathDefault);
                        this.FontFamily = fontFamilies.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed setting application font: " + ex.Message);
            }
        }

        //Application Close Handler
        protected override async void OnClosing(CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                await Application_Exit();
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
                    this.Opacity = 0.80;
                    this.IsEnabled = false;
                });

                //Stop monitoring the hardware
                vHardwareComputer.Close();

                //Stop the background tasks
                await TasksBackgroundStop();

                //Disable the socket server
                if (vArnoldVinkSockets != null)
                {
                    await vArnoldVinkSockets.SocketServerDisable();
                }

                //Hide the visible tray icon
                TrayNotifyIcon.Visible = false;

                //Close the application
                Environment.Exit(0);
            }
            catch { }
        }
    }
}