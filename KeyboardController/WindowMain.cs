using ArnoldVinkCode;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using static KeyboardController.AppVariables;
using static LibraryShared.AppImport;
using static LibraryShared.Classes;
using static LibraryShared.OutputKeyboard;
using static ArnoldVinkCode.ArnoldVinkProcesses;

namespace KeyboardController
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
                //Get application interop window handle
                IntPtr InteropHandle = new WindowInteropHelper(this).Handle;

                //Set the window style
                IntPtr UpdatedStyle = new IntPtr((uint)WindowStyles.WS_VISIBLE);
                SetWindowLongAuto(InteropHandle, (int)WindowLongFlags.GWL_STYLE, UpdatedStyle);

                //Set the window style ex
                IntPtr UpdatedExStyle = new IntPtr((uint)(WindowStylesEx.WS_EX_TOPMOST | WindowStylesEx.WS_EX_NOACTIVATE));
                SetWindowLongAuto(InteropHandle, (int)WindowLongFlags.GWL_EXSTYLE, UpdatedExStyle);

                //Set the window as top most
                SetWindowPos(InteropHandle, (IntPtr)WindowPosition.TopMost, 0, 0, 0, 0, (int)(WindowSWP.NOMOVE | WindowSWP.NOSIZE));

                Debug.WriteLine("The window is now set as top most.");
            }
            catch { }
        }

        //Window Startup
        public async Task Startup()
        {
            try
            {
                //Check application settings
                WindowSettings.Settings_Check();
                WindowSettings.Settings_LoadSocket();

                //Create tray icon
                Application_CreateTrayMenu();

                //Update the keyboard layout
                UpdateKeyboardLayout();

                //Update the keyboard mode
                UpdateKeyboardMode();

                //Play window open sound
                PlayInterfaceSound("PopupOpen", false);

                //Focus default key on the keyboard
                await FocusOnKeyboard();

                //Make window able to drag from border
                this.MouseDown += WindowMain_MouseDown;

                //Start application tasks
                TasksBackgroundStart();

                //Enable the socket server
                await vSocketServer.SocketServerSwitch(false, false);
                vSocketServer.EventBytesReceived += ReceivedSocketHandler;

                Debug.WriteLine("Application has launched.");
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

        //Focus a key on the keyboard
        async Task FocusOnKeyboard()
        {
            try
            {
                //Get the current active screen
                Screen targetScreen = GetActiveScreen();

                //Move the window to top left
                this.Top = targetScreen.WorkingArea.Top;
                this.Left = targetScreen.WorkingArea.Left;
                await Task.Delay(10);

                //Fullscreen the application
                grid_Application.Height = targetScreen.WorkingArea.Height;
                grid_Application.Width = targetScreen.WorkingArea.Width;
                await Task.Delay(10);

                //Focus on the beginning key
                Mouse.Capture(key_g);
                await Task.Delay(10);

                //Store the previous cursor position
                GetCursorPos(out PointWin PreviousCursorPosition);
                await Task.Delay(10);

                //Move cursor to the click position
                int TargetX = Convert.ToInt32(targetScreen.Bounds.Location.X + (targetScreen.WorkingArea.Width / 2));
                int TargetY = Convert.ToInt32(targetScreen.Bounds.Location.Y + (targetScreen.WorkingArea.Height / 2));
                SetCursorPos(TargetX, TargetY);
                await Task.Delay(10);

                //Click on the application window
                uint mouseFlags = (uint)(MouseEvents.MOUSEEVENTF_LEFTDOWN | MouseEvents.MOUSEEVENTF_LEFTUP);
                mouse_event(mouseFlags, 0, 0, 0, IntPtr.Zero);
                await Task.Delay(10);

                //Resize the application
                grid_Application.Height = double.NaN;
                grid_Application.Width = double.NaN;
                await Task.Delay(10);

                //Check if mouse cursor is in keyboard
                if ((targetScreen.WorkingArea.Height - PreviousCursorPosition.Y) <= this.Height) { PreviousCursorPosition.Y = Convert.ToInt32(targetScreen.WorkingArea.Height - this.Height - 20); }
                SetCursorPos(PreviousCursorPosition.X, PreviousCursorPosition.Y);
                await Task.Delay(10);

                //Move window to bottom center
                this.Left = targetScreen.Bounds.Location.X + (targetScreen.WorkingArea.Width - this.ActualWidth) / 2;
                this.Top = targetScreen.Bounds.Location.Y + targetScreen.WorkingArea.Height - this.ActualHeight;
                await Task.Delay(10);

                //Focus on the beginning key
                Mouse.Capture(key_g);
                await Task.Delay(10);

                //Show the keyboard
                UpdateKeyboardOpacity();
                await Task.Delay(10);
            }
            catch { }
        }

        //Get the current active screen
        public Screen GetActiveScreen()
        {
            try
            {
                //Get default monitor
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = "CtrlUI.exe.Config";

                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                int MonitorNumber = Convert.ToInt32(config.AppSettings.Settings["DisplayMonitor"].Value);

                //Get the target screen
                if (MonitorNumber > 0)
                {
                    try
                    {
                        return Screen.AllScreens[MonitorNumber];
                    }
                    catch
                    {
                        return Screen.PrimaryScreen;
                    }
                }
                else
                {
                    return Screen.PrimaryScreen;
                }
            }
            catch { return Screen.PrimaryScreen; }
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

        //Switch capslock on and off
        void SwitchCapsLock()
        {
            try
            {
                PlayInterfaceSound("KeyboardPress", false);
                Debug.WriteLine("Switching caps lock.");

                //Disable hardware capslock
                AVActions.ActionDispatcherInvoke(delegate
                {
                    if (Keyboard.GetKeyStates(Key.CapsLock) == KeyStates.Toggled)
                    {
                        KeyPressSingle((byte)KeysVirtual.CapsLock, false);
                    }
                });

                //Enable or disable software capslock
                if (vCapsEnabled)
                {
                    vCapsEnabled = false;
                    AVActions.ActionDispatcherInvoke(delegate
                    {
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

                        key_Control.Content = "Ctrl";
                        key_Menu.Content = "Alt";
                    });
                }
                else
                {
                    vCapsEnabled = true;
                    AVActions.ActionDispatcherInvoke(delegate
                    {
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

                        key_Control.Content = "Copy";
                        key_Menu.Content = "Paste";
                    });
                }
            }
            catch { }
        }

        //Check if keyboard opener is the foreground window
        async Task UpdateWindowStatus()
        {
            try
            {
                int FocusedAppId = GetFocusedProcess().Process.Id;
                if (vCurrentProcessId == FocusedAppId)
                {
                    if (vKeysEnabled)
                    {
                        vKeysEnabled = false;
                        Popup_Show_Status("Keyboard blocked from usage.");

                        //Disable all the key rows
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            foreach (FrameworkElement child in sp_Row0.Children) { child.IsEnabled = false; }
                            foreach (FrameworkElement child in sp_Row1.Children) { child.IsEnabled = false; }
                            foreach (FrameworkElement child in sp_Row2.Children) { child.IsEnabled = false; }
                            foreach (FrameworkElement child in sp_Row3.Children) { child.IsEnabled = false; }
                            foreach (FrameworkElement child in sp_Row4.Children) { child.IsEnabled = false; }
                            foreach (FrameworkElement child in sp_Row5.Children) { child.IsEnabled = false; }
                        });

                        //Focus on the close button
                        await AVActions.ActionDispatcherInvokeAsync(async delegate
                        {
                            await Task.Delay(10);
                            key_Close.IsEnabled = true;
                            await Task.Delay(10);
                            key_Close.Focus();
                            Keyboard.Focus(key_Close);
                            await Task.Delay(10);
                            KeyPressSingle((byte)KeysVirtual.Tab, false);
                        });
                    }
                }
                else
                {
                    if (!vKeysEnabled)
                    {
                        vKeysEnabled = true;
                        Popup_Show_Status("Keyboard enabled for usage.");

                        //Enable all the key rows
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            foreach (FrameworkElement child in sp_Row0.Children) { child.IsEnabled = true; }
                            foreach (FrameworkElement child in sp_Row1.Children) { child.IsEnabled = true; }
                            foreach (FrameworkElement child in sp_Row2.Children) { child.IsEnabled = true; }
                            foreach (FrameworkElement child in sp_Row3.Children) { child.IsEnabled = true; }
                            foreach (FrameworkElement child in sp_Row4.Children) { child.IsEnabled = true; }
                            foreach (FrameworkElement child in sp_Row5.Children) { child.IsEnabled = true; }
                        });
                    }
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
                Debug.WriteLine("Exiting Keyboard Controller.");

                //Stop the background tasks
                TasksBackgroundStop();

                await vSocketServer.SocketServerDisable();

                //Play window close sound and animation
                AVAnimations.Ani_Visibility(this, false, false, 0.10);
                PlayInterfaceSound("PopupClose", false);
                await Task.Delay(1000);

                //Close the application
                TrayNotifyIcon.Visible = false;
                Environment.Exit(0);
            }
            catch { }
        }
    }
}