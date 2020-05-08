using ArnoldVinkCode;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVInterface;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessFunctions;
using static KeyboardController.AppVariables;
using static LibraryShared.Settings;
using static LibraryShared.SoundPlayer;

namespace KeyboardController
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
                //Get application interop window handle
                vInteropWindowHandle = new WindowInteropHelper(this).EnsureHandle();

                //Check application settings
                WindowSettings.Settings_Check();
                Settings_Load_CtrlUI(ref vConfigurationCtrlUI);
                Settings_Load_AccentColor(vConfigurationCtrlUI);

                //Create tray icon
                Application_CreateTrayMenu();

                //Disable hardware capslock
                DisableHardwareCapsLock();

                //Update the keyboard layout
                UpdateKeyboardLayout();

                //Update the keyboard mode
                UpdateKeyboardMode();

                //Play window open sound
                PlayInterfaceSound("PopupOpen", false);

                //Update the window position
                UpdateWindowPosition();

                //Activate window and focus on key
                await KeyboardWindowActivate(key_g);

                //Make window able to drag from border
                this.MouseDown += WindowMain_MouseDown;

                //Start application tasks
                TasksBackgroundStart();

                //Launch DirectXInput application
                await LaunchDirectXInput();

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
                int SocketServerPort = Convert.ToInt32(vConfigurationCtrlUI.AppSettings.Settings["ServerPort"].Value) + 2;

                vArnoldVinkSockets = new ArnoldVinkSockets("127.0.0.1", SocketServerPort);
                vArnoldVinkSockets.vTcpClientTimeout = 250;
                vArnoldVinkSockets.EventBytesReceived += ReceivedSocketHandler;
            }
            catch { }
        }

        //Drag the window around
        private void WindowMain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            }
            catch { }
        }

        //Update the window style
        void UpdateWindowStyle()
        {
            try
            {
                //Set the window style
                IntPtr UpdatedStyle = new IntPtr((uint)WindowStyles.WS_VISIBLE);
                SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_STYLE, UpdatedStyle);

                //Set the window style ex
                IntPtr UpdatedExStyle = new IntPtr((uint)(WindowStylesEx.WS_EX_TOPMOST | WindowStylesEx.WS_EX_NOACTIVATE));
                SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_EXSTYLE, UpdatedExStyle);

                //Set the window as top most
                SetWindowPos(vInteropWindowHandle, (IntPtr)WindowPosition.TopMost, 0, 0, 0, 0, (int)(WindowSWP.NOMOVE | WindowSWP.NOSIZE));

                Debug.WriteLine("The window style has been updated.");
            }
            catch { }
        }

        //Update the window position
        void UpdateWindowPosition()
        {
            try
            {
                //Get the current active screen
                int monitorNumber = Convert.ToInt32(vConfigurationCtrlUI.AppSettings.Settings["DisplayMonitor"].Value);
                DisplayMonitorSettings displayMonitorSettings = GetScreenSettings(monitorNumber);

                //Move the window to bottom center
                AVActions.ActionDispatcherInvoke(delegate
                {
                    this.Left = displayMonitorSettings.BoundsLeft + (displayMonitorSettings.WidthDpi - this.ActualWidth) / 2;
                    this.Top = displayMonitorSettings.BoundsTop + displayMonitorSettings.HeightDpi - this.ActualHeight;
                });
            }
            catch { }
        }

        //Activate keyboard window
        async Task KeyboardWindowActivate(FrameworkElement focusKey)
        {
            try
            {
                //Update the window style
                UpdateWindowStyle();

                //Get the current active screen
                int monitorNumber = Convert.ToInt32(vConfigurationCtrlUI.AppSettings.Settings["DisplayMonitor"].Value);
                DisplayMonitorSettings displayMonitorSettings = GetScreenSettings(monitorNumber);

                //Get the current mouse position
                GetCursorPos(out PointWin previousCursorPosition);

                //Check if mouse cursor is in keyboard
                if ((displayMonitorSettings.HeightDpi - previousCursorPosition.Y) <= this.Height)
                {
                    previousCursorPosition.Y = Convert.ToInt32(displayMonitorSettings.HeightDpi - this.Height - 20);
                    SetCursorPos(previousCursorPosition.X, previousCursorPosition.Y);
                    await Task.Delay(10);
                }

                //Focus on keyboard button
                if (focusKey != null)
                {
                    await FocusOnElement(focusKey, false, vProcessCurrent.MainWindowHandle);
                    await Task.Delay(10);
                }

                //Update the keyboard opacity
                UpdateKeyboardOpacity();
                await Task.Delay(10);
            }
            catch { }
        }

        //Update keyboard opacity
        public void UpdateKeyboardOpacity()
        {
            try
            {
                this.Opacity = Convert.ToDouble(ConfigurationManager.AppSettings["KeyboardOpacity"]);
            }
            catch { }
        }

        //Switch keyboard layout
        public void UpdateKeyboardLayout()
        {
            try
            {
                //Disable caps lock
                if (vCapsEnabled)
                {
                    SwitchCapsLock();
                }

                //Change the keyboard layout
                if (Convert.ToInt32(ConfigurationManager.AppSettings["KeyboardLayout"]) == 0)
                {
                    Debug.WriteLine("Switching keyboard layout: QWERTY");
                    key_a.Content = "a";
                    key_a.Tag = "a";
                    key_q.Content = "q";
                    key_q.Tag = "q";
                    key_w.Content = "w";
                    key_w.Tag = "w";
                    key_y.Content = "y";
                    key_y.Tag = "y";
                    key_z.Content = "z";
                    key_z.Tag = "z";
                }
                else if (Convert.ToInt32(ConfigurationManager.AppSettings["KeyboardLayout"]) == 1) //QWERTZ
                {
                    Debug.WriteLine("Switching keyboard layout: QWERTZ");
                    key_a.Content = "a";
                    key_a.Tag = "a";
                    key_q.Content = "q";
                    key_q.Tag = "q";
                    key_w.Content = "w";
                    key_w.Tag = "w";
                    key_y.Content = "z";
                    key_y.Tag = "z";
                    key_z.Content = "y";
                    key_z.Tag = "y";
                }
                else if (Convert.ToInt32(ConfigurationManager.AppSettings["KeyboardLayout"]) == 2) //AZERTY
                {
                    Debug.WriteLine("Switching keyboard layout: AZERTY");
                    key_a.Content = "q";
                    key_a.Tag = "q";
                    key_q.Content = "a";
                    key_q.Tag = "a";
                    key_w.Content = "z";
                    key_w.Tag = "z";
                    key_y.Content = "y";
                    key_y.Tag = "y";
                    key_z.Content = "w";
                    key_z.Tag = "w";
                }
            }
            catch { }
        }

        //Update the domain extension
        public void UpdateDomainExtension()
        {
            try
            {
                if (vCapsEnabled)
                {
                    key_DotCom.Content = ConfigurationManager.AppSettings["DomainExtension"].ToString();
                }
                else
                {
                    key_DotCom.Content = ".com";
                }
            }
            catch { }
        }

        //Switch capslock on and off
        public void SwitchCapsLock()
        {
            try
            {
                PlayInterfaceSound("KeyboardPress", false);
                Debug.WriteLine("Switching caps lock.");

                //Disable hardware capslock
                DisableHardwareCapsLock();

                //Enable or disable software capslock
                if (vCapsEnabled)
                {
                    vCapsEnabled = false;
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        key_Del.Content = "Delete";
                        key_Del.Tag = "Delete";

                        key_End.Content = "End";
                        key_End.Tag = "End";

                        key_OEM3_Normal.Text = "`";
                        key_OEM3_Caps.Text = "~";

                        key_1_Normal.Text = "1";
                        key_1_Caps.Text = "!";

                        key_2_Normal.Text = "2";
                        key_2_Caps.Text = "@";

                        key_3_Normal.Text = "3";
                        key_3_Caps.Text = "#";

                        key_4_Normal.Text = "4";
                        key_4_Caps.Text = "$";

                        key_5_Normal.Text = "5";
                        key_5_Caps.Text = "%";

                        key_6_Normal.Text = "6";
                        key_6_Caps.Text = "^";

                        key_7_Normal.Text = "7";
                        key_7_Caps.Text = "&";

                        key_8_Normal.Text = "8";
                        key_8_Caps.Text = "*";

                        key_9_Normal.Text = "9";
                        key_9_Caps.Text = "(";

                        key_0_Normal.Text = "0";
                        key_0_Caps.Text = ")";

                        key_Subtract_Normal.Text = "-";
                        key_Subtract_Caps.Text = "_";

                        key_Add_Normal.Text = "=";
                        key_Add_Caps.Text = "+";

                        key_Tab.Content = "Tab>";
                        key_q.Content = key_q.Content.ToString().ToLower();
                        key_w.Content = key_w.Content.ToString().ToLower();
                        key_e.Content = key_e.Content.ToString().ToLower();
                        key_r.Content = key_r.Content.ToString().ToLower();
                        key_t.Content = key_t.Content.ToString().ToLower();
                        key_y.Content = key_y.Content.ToString().ToLower();
                        key_u.Content = key_u.Content.ToString().ToLower();
                        key_i.Content = key_i.Content.ToString().ToLower();
                        key_o.Content = key_o.Content.ToString().ToLower();
                        key_p.Content = key_p.Content.ToString().ToLower();

                        key_OEM4_Normal.Text = "[";
                        key_OEM4_Caps.Text = "{";

                        key_OEM6_Normal.Text = "]";
                        key_OEM6_Caps.Text = "}";

                        key_Caps.Content = "Caps";
                        key_a.Content = key_a.Content.ToString().ToLower();
                        key_s.Content = key_s.Content.ToString().ToLower();
                        key_d.Content = key_d.Content.ToString().ToLower();
                        key_f.Content = key_f.Content.ToString().ToLower();
                        key_g.Content = key_g.Content.ToString().ToLower();
                        key_h.Content = key_h.Content.ToString().ToLower();
                        key_j.Content = key_j.Content.ToString().ToLower();
                        key_k.Content = key_k.Content.ToString().ToLower();
                        key_l.Content = key_l.Content.ToString().ToLower();

                        key_OEM1_Normal.Text = ";";
                        key_OEM1_Caps.Text = ":";

                        key_OEM7_Normal.Text = "'";
                        key_OEM7_Caps.Text = "\"";

                        key_OEM5_Normal.Text = "\\";
                        key_OEM5_Caps.Text = "|";

                        key_z.Content = key_z.Content.ToString().ToLower();
                        key_x.Content = key_x.Content.ToString().ToLower();
                        key_c.Content = key_c.Content.ToString().ToLower();
                        key_v.Content = key_v.Content.ToString().ToLower();
                        key_b.Content = key_b.Content.ToString().ToLower();
                        key_n.Content = key_n.Content.ToString().ToLower();
                        key_m.Content = key_m.Content.ToString().ToLower();

                        key_OEMComma_Normal.Text = ",";
                        key_OEMComma_Caps.Text = "<";

                        key_OEMPeriod_Normal.Text = ".";
                        key_OEMPeriod_Caps.Text = ">";

                        key_OEM2_Normal.Text = "/";
                        key_OEM2_Caps.Text = "?";

                        key_Shift.Content = "Shift";
                        key_Control.Content = "Ctrl";
                        key_Menu.Content = "Alt";
                        key_LeftWindows.Content = "Windows";
                        key_Space.Content = "Space";
                        key_Return.Content = "Enter";

                        key_Escape.Content = "Escape";
                        key_Escape.Tag = "Escape";

                        //Update the domain extension
                        UpdateDomainExtension();

                        key_VolumeDown.Tag = "VolumeDown";
                        image_VolumeDown.Source = FileToBitmapImage(new string[] { "Assets/Icons/VolumeDown.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    });
                }
                else
                {
                    vCapsEnabled = true;
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        key_Del.Content = "Insert";
                        key_Del.Tag = "Insert";

                        key_End.Content = "Hme";
                        key_End.Tag = "Home";

                        key_OEM3_Normal.Text = "~";
                        key_OEM3_Caps.Text = "`";

                        key_1_Normal.Text = "!";
                        key_1_Caps.Text = "1";

                        key_2_Normal.Text = "@";
                        key_2_Caps.Text = "2";

                        key_3_Normal.Text = "#";
                        key_3_Caps.Text = "3";

                        key_4_Normal.Text = "$";
                        key_4_Caps.Text = "4";

                        key_5_Normal.Text = "%";
                        key_5_Caps.Text = "5";

                        key_6_Normal.Text = "^";
                        key_6_Caps.Text = "6";

                        key_7_Normal.Text = "&";
                        key_7_Caps.Text = "7";

                        key_8_Normal.Text = "*";
                        key_8_Caps.Text = "8";

                        key_9_Normal.Text = "(";
                        key_9_Caps.Text = "9";

                        key_0_Normal.Text = ")";
                        key_0_Caps.Text = "0";

                        key_Subtract_Normal.Text = "_";
                        key_Subtract_Caps.Text = "-";

                        key_Add_Normal.Text = "+";
                        key_Add_Caps.Text = "=";

                        key_Tab.Content = "<Tab";
                        key_q.Content = key_q.Content.ToString().ToUpper();
                        key_w.Content = key_w.Content.ToString().ToUpper();
                        key_e.Content = key_e.Content.ToString().ToUpper();
                        key_r.Content = key_r.Content.ToString().ToUpper();
                        key_t.Content = key_t.Content.ToString().ToUpper();
                        key_y.Content = key_y.Content.ToString().ToUpper();
                        key_u.Content = key_u.Content.ToString().ToUpper();
                        key_i.Content = key_i.Content.ToString().ToUpper();
                        key_o.Content = key_o.Content.ToString().ToUpper();
                        key_p.Content = key_p.Content.ToString().ToUpper();

                        key_OEM4_Normal.Text = "{";
                        key_OEM4_Caps.Text = "[";

                        key_OEM6_Normal.Text = "}";
                        key_OEM6_Caps.Text = "]";

                        key_Caps.Content = "CAPS";
                        key_a.Content = key_a.Content.ToString().ToUpper();
                        key_s.Content = key_s.Content.ToString().ToUpper();
                        key_d.Content = key_d.Content.ToString().ToUpper();
                        key_f.Content = key_f.Content.ToString().ToUpper();
                        key_g.Content = key_g.Content.ToString().ToUpper();
                        key_h.Content = key_h.Content.ToString().ToUpper();
                        key_j.Content = key_j.Content.ToString().ToUpper();
                        key_k.Content = key_k.Content.ToString().ToUpper();
                        key_l.Content = key_l.Content.ToString().ToUpper();

                        key_OEM1_Normal.Text = ":";
                        key_OEM1_Caps.Text = ";";

                        key_OEM7_Normal.Text = "\"";
                        key_OEM7_Caps.Text = "'";

                        key_OEM5_Normal.Text = "|";
                        key_OEM5_Caps.Text = "\\";

                        key_z.Content = key_z.Content.ToString().ToUpper();
                        key_x.Content = key_x.Content.ToString().ToUpper();
                        key_c.Content = key_c.Content.ToString().ToUpper();
                        key_v.Content = key_v.Content.ToString().ToUpper();
                        key_b.Content = key_b.Content.ToString().ToUpper();
                        key_n.Content = key_n.Content.ToString().ToUpper();
                        key_m.Content = key_m.Content.ToString().ToUpper();

                        key_OEMComma_Normal.Text = "<";
                        key_OEMComma_Caps.Text = ",";

                        key_OEMPeriod_Normal.Text = ">";
                        key_OEMPeriod_Caps.Text = ".";

                        key_OEM2_Normal.Text = "?";
                        key_OEM2_Caps.Text = "/";

                        key_Shift.Content = "Cut";
                        key_Control.Content = "Copy";
                        key_Menu.Content = "Paste";
                        key_LeftWindows.Content = "Select all";
                        key_Space.Content = "Task manager";
                        key_Return.Content = "Undo";

                        key_Escape.Content = "PrtSc";
                        key_Escape.Tag = "Snapshot";

                        //Update the domain extension
                        UpdateDomainExtension();

                        key_VolumeDown.Tag = "VolumeMute";
                        image_VolumeDown.Source = FileToBitmapImage(new string[] { "Assets/Icons/VolumeMute.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    });
                }
            }
            catch { }
        }

        //Disable hardware capslock
        public void DisableHardwareCapsLock()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    if (Keyboard.GetKeyStates(Key.CapsLock) == KeyStates.Toggled)
                    {
                        KeyPressSingle((byte)KeysVirtual.CapsLock, false);
                    }
                });
            }
            catch { }
        }

        //Check if keyboard opener is the foreground window
        async Task UpdateWindowStatus()
        {
            try
            {
                int FocusedAppId = GetFocusedProcess().Identifier;

                if (vProcessCurrent.Id == FocusedAppId)
                {
                    await AppWindowActivated();
                }
                else
                {
                    await AppWindowDeactivated();
                }
            }
            catch { }
        }

        //Application window activated event
        async Task AppWindowActivated()
        {
            try
            {
                if (vKeysEnabled)
                {
                    vKeysEnabled = false;
                    await Notification_Send_Status("Keyboard", "Keyboard blocked");

                    await AVActions.ActionDispatcherInvokeAsync(async delegate
                    {
                        //Disable all the key rows
                        foreach (FrameworkElement child in sp_Row0.Children) { child.IsEnabled = false; }
                        foreach (FrameworkElement child in sp_Row1.Children) { child.IsEnabled = false; }
                        foreach (FrameworkElement child in sp_Row2.Children) { child.IsEnabled = false; }
                        foreach (FrameworkElement child in sp_Row3.Children) { child.IsEnabled = false; }
                        foreach (FrameworkElement child in sp_Row4.Children) { child.IsEnabled = false; }
                        foreach (FrameworkElement child in sp_Row5.Children) { child.IsEnabled = false; }
                        await Task.Delay(10);

                        //Enable the close button
                        key_Close.IsEnabled = true;
                        await Task.Delay(10);

                        //Focus on close button
                        await FocusOnElement(key_Close, false, vProcessCurrent.MainWindowHandle);
                        await Task.Delay(10);
                    });
                }
            }
            catch { }
        }

        //Application window deactivated event
        async Task AppWindowDeactivated()
        {
            try
            {
                if (!vKeysEnabled)
                {
                    vKeysEnabled = true;
                    await Notification_Send_Status("Keyboard", "Keyboard enabled");

                    await AVActions.ActionDispatcherInvokeAsync(async delegate
                    {
                        //Enable all the key rows
                        foreach (FrameworkElement child in sp_Row0.Children) { child.IsEnabled = true; }
                        foreach (FrameworkElement child in sp_Row1.Children) { child.IsEnabled = true; }
                        foreach (FrameworkElement child in sp_Row2.Children) { child.IsEnabled = true; }
                        foreach (FrameworkElement child in sp_Row3.Children) { child.IsEnabled = true; }
                        foreach (FrameworkElement child in sp_Row4.Children) { child.IsEnabled = true; }
                        foreach (FrameworkElement child in sp_Row5.Children) { child.IsEnabled = true; }
                        await Task.Delay(10);

                        //Activate keyboard window and focus on key
                        await KeyboardWindowActivate(key_g);
                    });
                }
            }
            catch { }
        }

        //Application Close Handler
        protected async override void OnClosing(CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                await Application_Exit();
            }
            catch { }
        }

        //Close the application
        async Task Application_Exit()
        {
            try
            {
                Debug.WriteLine("Exiting application.");
                AVActions.ActionDispatcherInvoke(delegate
                {
                    this.Opacity = 0.80;
                    this.IsEnabled = false;
                });

                //Play window close sound
                PlayInterfaceSound("PopupClose", false);

                //Stop the background tasks
                TasksBackgroundStop();

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