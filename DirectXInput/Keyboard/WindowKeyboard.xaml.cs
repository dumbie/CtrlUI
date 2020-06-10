using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVInterface;
using static ArnoldVinkCode.AVInteropDll;
using static DirectXInput.AppVariables;
using static LibraryShared.SoundPlayer;

namespace DirectXInput.Keyboard
{
    public partial class WindowKeyboard : Window
    {
        //Window Initialize
        public WindowKeyboard() { InitializeComponent(); }

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

                //Update the window style
                UpdateWindowStyle();

                //Update keyboard keys
                UpdateKeyboardKeys();

                //Update the keyboard layout
                await UpdateKeyboardLayout();

                //Update the keyboard mode
                UpdateKeyboardMode();

                //Update the window position
                UpdateWindowPosition();

                //Check mouse cursor position
                CheckMousePosition();

                //Focus on keyboard button
                await FocusKeyboardButton(true);

                //Make window able to drag from border
                this.MouseDown += WindowKeyboard_MouseDown;

                //Check if resolution has changed
                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            }
            catch { }
        }

        //Hide the keyboard window
        public new void Hide()
        {
            try
            {
                if (vWindowVisible)
                {
                    //Delay CtrlUI output
                    vController0.Delay_CtrlUIOutput = Environment.TickCount + vControllerDelayMediumTicks;
                    vController1.Delay_CtrlUIOutput = Environment.TickCount + vControllerDelayMediumTicks;
                    vController2.Delay_CtrlUIOutput = Environment.TickCount + vControllerDelayMediumTicks;
                    vController3.Delay_CtrlUIOutput = Environment.TickCount + vControllerDelayMediumTicks;

                    //Play window close sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false);

                    //Update the keyboard opacity
                    this.Opacity = 0;

                    //Update the keyboard visibility
                    this.Title = "DirectXInput Keyboard (Hidden)";
                    vWindowVisible = false;
                    Debug.WriteLine("Hiding the keyboard window.");
                }
            }
            catch { }
        }

        //Show the keyboard window
        public new async Task Show()
        {
            try
            {
                //Delay keyboard input
                vControllerDelay_Keyboard = Environment.TickCount + vControllerDelayMediumTicks;

                //Play window open sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false);

                //Disable hardware capslock
                await DisableHardwareCapsLock();

                //Check mouse cursor position
                CheckMousePosition();

                //Focus on keyboard button
                await FocusKeyboardButton(false);

                //Update the window style (focus workaround)
                UpdateWindowStyle();

                //Update the keyboard opacity
                UpdateKeyboardOpacity(true);

                //Update the keyboard visibility
                this.Title = "DirectXInput Keyboard (Visible)";
                this.Visibility = Visibility.Visible;
                vWindowVisible = true;
                Debug.WriteLine("Showing the keyboard window.");
            }
            catch { }
        }

        //Update keyboard opacity
        public void UpdateKeyboardOpacity(bool forceUpdate)
        {
            try
            {
                if (forceUpdate || this.Opacity != 0)
                {
                    this.Opacity = Convert.ToDouble(ConfigurationManager.AppSettings["KeyboardOpacity"]);
                }
            }
            catch { }
        }

        //Drag the window around
        private void WindowKeyboard_MouseDown(object sender, MouseButtonEventArgs e)
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

        //Update the window position on resolution change
        public void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateWindowPosition();
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
        public void UpdateWindowPosition()
        {
            try
            {
                //Get the current active screen
                int monitorNumber = Convert.ToInt32(vConfigurationCtrlUI.AppSettings.Settings["DisplayMonitor"].Value);
                DisplayMonitorSettings displayMonitorSettings = GetScreenSettings(monitorNumber);

                //Get the current window size
                int windowWidth = (int)(this.ActualWidth * displayMonitorSettings.DpiScaleHorizontal);
                int windowHeight = (int)(this.ActualHeight * displayMonitorSettings.DpiScaleVertical);

                //Move the window to bottom center
                int horizontalLeft = (int)(displayMonitorSettings.BoundsLeft + (displayMonitorSettings.WidthNative - windowWidth) / 2);
                int verticalTop = (int)(displayMonitorSettings.BoundsTop + displayMonitorSettings.HeightNative - windowHeight);
                WindowMove(vInteropWindowHandle, horizontalLeft, verticalTop);
            }
            catch { }
        }

        //Activate keyboard window
        void CheckMousePosition()
        {
            try
            {
                //Get the current active screen
                int monitorNumber = Convert.ToInt32(vConfigurationCtrlUI.AppSettings.Settings["DisplayMonitor"].Value);
                DisplayMonitorSettings displayMonitorSettings = GetScreenSettings(monitorNumber);

                //Get the current mouse position
                GetCursorPos(out PointWin previousCursorPosition);

                //Check if mouse cursor is in keyboard
                if ((displayMonitorSettings.HeightNative - previousCursorPosition.Y) <= this.Height)
                {
                    previousCursorPosition.Y = Convert.ToInt32(displayMonitorSettings.HeightNative - this.Height - 20);
                    SetCursorPos(previousCursorPosition.X, previousCursorPosition.Y);
                }
            }
            catch { }
        }

        //Focus on keyboard button
        async Task FocusKeyboardButton(bool forceFocus)
        {
            try
            {
                if (forceFocus || System.Windows.Input.Keyboard.FocusedElement == null)
                {
                    await FocusOnElement(key_h, false, vInteropWindowHandle);
                }
            }
            catch { }
        }

        //Update keyboard keys
        public void UpdateKeyboardKeys()
        {
            try
            {
                //Key Row 0
                key_F1.Content = "F1";
                key_F1.Tag = KeysVirtual.F1;
                key_F2.Content = "F2";
                key_F2.Tag = KeysVirtual.F2;
                key_F3.Content = "F3";
                key_F3.Tag = KeysVirtual.F3;
                key_F4.Content = "F4";
                key_F4.Tag = KeysVirtual.F4;
                key_F5.Content = "F5";
                key_F5.Tag = KeysVirtual.F5;
                key_F6.Content = "F6";
                key_F6.Tag = KeysVirtual.F6;
                key_F7.Content = "F7";
                key_F7.Tag = KeysVirtual.F7;
                key_F8.Content = "F8";
                key_F8.Tag = KeysVirtual.F8;
                key_F9.Content = "F9";
                key_F9.Tag = KeysVirtual.F9;
                key_F10.Content = "F10";
                key_F10.Tag = KeysVirtual.F10;
                key_F11.Content = "F11";
                key_F11.Tag = KeysVirtual.F11;
                key_F12.Content = "F12";
                key_F12.Tag = KeysVirtual.F12;
                key_Delete.Content = "Delete";
                key_Delete.Tag = KeysVirtual.Delete;
                key_End.Content = "End";
                key_End.Tag = KeysVirtual.End;

                //Key Row 1
                key_Tilde_Normal.Text = "`";
                key_Tilde_Caps.Text = "~";
                key_Tilde.Tag = KeysVirtual.OEMTilde;
                key_1_Normal.Text = "1";
                key_1_Caps.Text = "!";
                key_1.Tag = KeysVirtual.Digit1;
                key_2_Normal.Text = "2";
                key_2_Caps.Text = "@";
                key_2.Tag = KeysVirtual.Digit2;
                key_3_Normal.Text = "3";
                key_3_Caps.Text = "#";
                key_3.Tag = KeysVirtual.Digit3;
                key_4_Normal.Text = "4";
                key_4_Caps.Text = "$";
                key_4.Tag = KeysVirtual.Digit4;
                key_5_Normal.Text = "5";
                key_5_Caps.Text = "%";
                key_5.Tag = KeysVirtual.Digit5;
                key_6_Normal.Text = "6";
                key_6_Caps.Text = "^";
                key_6.Tag = KeysVirtual.Digit6;
                key_7_Normal.Text = "7";
                key_7_Caps.Text = "&";
                key_7.Tag = KeysVirtual.Digit7;
                key_8_Normal.Text = "8";
                key_8_Caps.Text = "*";
                key_8.Tag = KeysVirtual.Digit8;
                key_9_Normal.Text = "9";
                key_9_Caps.Text = "(";
                key_9.Tag = KeysVirtual.Digit9;
                key_0_Normal.Text = "0";
                key_0_Caps.Text = ")";
                key_0.Tag = KeysVirtual.Digit0;
                key_Minus_Normal.Text = "-";
                key_Minus_Caps.Text = "_";
                key_Minus.Tag = KeysVirtual.OEMMinus;
                key_Plus_Normal.Text = "=";
                key_Plus_Caps.Text = "+";
                key_Plus.Tag = KeysVirtual.OEMPlus;
                key_BackSpace.Content = "Backspc";
                key_BackSpace.Tag = KeysVirtual.BackSpace;

                //Key Row 2
                key_Tab.Content = "Tab>";
                key_Tab.Tag = KeysVirtual.Tab;
                key_q.Content = "q";
                key_q.Tag = KeysVirtual.Q;
                key_w.Content = "w";
                key_w.Tag = KeysVirtual.W;
                key_e.Content = "e";
                key_e.Tag = KeysVirtual.E;
                key_r.Content = "r";
                key_r.Tag = KeysVirtual.R;
                key_t.Content = "t";
                key_t.Tag = KeysVirtual.T;
                key_y.Content = "y";
                key_y.Tag = KeysVirtual.Y;
                key_u.Content = "u";
                key_u.Tag = KeysVirtual.U;
                key_i.Content = "i";
                key_i.Tag = KeysVirtual.I;
                key_o.Content = "o";
                key_o.Tag = KeysVirtual.O;
                key_p.Content = "p";
                key_p.Tag = KeysVirtual.P;
                key_OpenBracket_Normal.Text = "[";
                key_OpenBracket_Caps.Text = "{";
                key_OpenBracket.Tag = KeysVirtual.OEMOpenBracket;
                key_CloseBracket_Normal.Text = "]";
                key_CloseBracket_Caps.Text = "}";
                key_CloseBracket.Tag = KeysVirtual.OEMCloseBracket;
                key_DotCom.Content = ".com";
                key_DotCom.Tag = "DotCom";

                //Key Row 3
                key_CapsLock.Content = "Caps";
                key_CapsLock.Tag = KeysVirtual.CapsLock;
                key_a.Content = "a";
                key_a.Tag = KeysVirtual.A;
                key_s.Content = "s";
                key_s.Tag = KeysVirtual.S;
                key_d.Content = "d";
                key_d.Tag = KeysVirtual.D;
                key_f.Content = "f";
                key_f.Tag = KeysVirtual.F;
                key_g.Content = "g";
                key_g.Tag = KeysVirtual.G;
                key_h.Content = "h";
                key_h.Tag = KeysVirtual.H;
                key_j.Content = "j";
                key_j.Tag = KeysVirtual.J;
                key_k.Content = "k";
                key_k.Tag = KeysVirtual.K;
                key_l.Content = "l";
                key_l.Tag = KeysVirtual.L;
                key_Semicolon_Normal.Text = ";";
                key_Semicolon_Caps.Text = ":";
                key_Semicolon.Tag = KeysVirtual.OEMSemicolon;
                key_Quote_Normal.Text = "'";
                key_Quote_Caps.Text = "\"";
                key_Quote.Tag = KeysVirtual.OEMQuote;
                key_Up.Content = "⯅";
                key_Up.Tag = KeysVirtual.Up;
                key_Pipe_Normal.Text = "\\";
                key_Pipe_Caps.Text = "|";
                key_Pipe.Tag = KeysVirtual.OEMPipe;

                //Key Row 4
                key_Shift.Content = "Shift";
                key_Shift.Tag = KeysVirtual.Shift;
                key_z.Content = "z";
                key_z.Tag = KeysVirtual.Z;
                key_x.Content = "x";
                key_x.Tag = KeysVirtual.X;
                key_c.Content = "c";
                key_c.Tag = KeysVirtual.C;
                key_v.Content = "v";
                key_v.Tag = KeysVirtual.V;
                key_b.Content = "b";
                key_b.Tag = KeysVirtual.B;
                key_n.Content = "n";
                key_n.Tag = KeysVirtual.N;
                key_m.Content = "m";
                key_m.Tag = KeysVirtual.M;
                key_Comma_Normal.Text = ",";
                key_Comma_Caps.Text = "<";
                key_Comma.Tag = KeysVirtual.OEMComma;
                key_Period_Normal.Text = ".";
                key_Period_Caps.Text = ">";
                key_Period.Tag = KeysVirtual.OEMPeriod;
                key_Question_Normal.Text = "/";
                key_Question_Caps.Text = "?";
                key_Question.Tag = KeysVirtual.OEMQuestion;
                key_Left.Content = "⯇";
                key_Left.Tag = KeysVirtual.Left;
                key_Down.Content = "⯆";
                key_Down.Tag = KeysVirtual.Down;
                key_Right.Content = "⯈";
                key_Right.Tag = KeysVirtual.Right;

                //Key Row 5
                key_Control.Content = "Ctrl";
                key_Control.Tag = KeysVirtual.Control;
                key_Alt.Content = "Alt";
                key_Alt.Tag = KeysVirtual.Alt;
                key_LeftWindows.Content = "Windows";
                key_LeftWindows.Tag = KeysVirtual.LeftWindows;
                key_Space.Content = "Space";
                key_Space.Tag = KeysVirtual.Space;
                key_Enter.Content = "Enter";
                key_Enter.Tag = KeysVirtual.Enter;
                key_Escape.Content = "Escape";
                key_Escape.Tag = KeysVirtual.Escape;
                key_VolumeDown.Tag = KeysVirtual.VolumeDown;
                key_VolumeUp.Tag = KeysVirtual.VolumeUp;
            }
            catch { }
        }

        //Switch keyboard layout
        public async Task UpdateKeyboardLayout()
        {
            try
            {
                //Disable caps lock
                if (vCapsEnabled)
                {
                    await SwitchCapsLock();
                }

                //Change the keyboard layout
                if (Convert.ToInt32(ConfigurationManager.AppSettings["KeyboardLayout"]) == 0)
                {
                    Debug.WriteLine("Switching keyboard layout: QWERTY");
                    key_a.Content = "a";
                    key_a.Tag = KeysVirtual.A;
                    key_q.Content = "q";
                    key_q.Tag = KeysVirtual.Q;
                    key_w.Content = "w";
                    key_w.Tag = KeysVirtual.W;
                    key_y.Content = "y";
                    key_y.Tag = KeysVirtual.Y;
                    key_z.Content = "z";
                    key_z.Tag = KeysVirtual.Z;
                }
                else if (Convert.ToInt32(ConfigurationManager.AppSettings["KeyboardLayout"]) == 1) //QWERTZ
                {
                    Debug.WriteLine("Switching keyboard layout: QWERTZ");
                    key_a.Content = "a";
                    key_a.Tag = KeysVirtual.A;
                    key_q.Content = "q";
                    key_q.Tag = KeysVirtual.Q;
                    key_w.Content = "w";
                    key_w.Tag = KeysVirtual.W;
                    key_y.Content = "z";
                    key_y.Tag = KeysVirtual.Z;
                    key_z.Content = "y";
                    key_z.Tag = KeysVirtual.Y;
                }
                else if (Convert.ToInt32(ConfigurationManager.AppSettings["KeyboardLayout"]) == 2) //AZERTY
                {
                    Debug.WriteLine("Switching keyboard layout: AZERTY");
                    key_a.Content = "q";
                    key_a.Tag = KeysVirtual.Q;
                    key_q.Content = "a";
                    key_q.Tag = KeysVirtual.A;
                    key_w.Content = "z";
                    key_w.Tag = KeysVirtual.Z;
                    key_y.Content = "y";
                    key_y.Tag = KeysVirtual.Y;
                    key_z.Content = "w";
                    key_z.Tag = KeysVirtual.W;
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
                    string extensionString = ConfigurationManager.AppSettings["KeyboardDomainExtension"].ToString();
                    key_DotCom.Content = AVFunctions.StringCut(extensionString, 4, string.Empty);
                }
                else
                {
                    key_DotCom.Content = ".com";
                }
            }
            catch { }
        }

        //Switch capslock on and off
        public async Task SwitchCapsLock()
        {
            try
            {
                PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                Debug.WriteLine("Switching caps lock.");

                //Disable hardware capslock
                await DisableHardwareCapsLock();

                //Enable or disable software capslock
                if (vCapsEnabled)
                {
                    vCapsEnabled = false;
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        key_Delete.Content = "Delete";
                        key_Delete.Tag = KeysVirtual.Delete;

                        key_End.Content = "End";
                        key_End.Tag = KeysVirtual.End;

                        key_Tilde_Normal.Text = "`";
                        key_Tilde_Caps.Text = "~";

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

                        key_Minus_Normal.Text = "-";
                        key_Minus_Caps.Text = "_";

                        key_Plus_Normal.Text = "=";
                        key_Plus_Caps.Text = "+";

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

                        key_OpenBracket_Normal.Text = "[";
                        key_OpenBracket_Caps.Text = "{";

                        key_CloseBracket_Normal.Text = "]";
                        key_CloseBracket_Caps.Text = "}";

                        key_CapsLock.Content = "Caps";
                        key_a.Content = key_a.Content.ToString().ToLower();
                        key_s.Content = key_s.Content.ToString().ToLower();
                        key_d.Content = key_d.Content.ToString().ToLower();
                        key_f.Content = key_f.Content.ToString().ToLower();
                        key_g.Content = key_g.Content.ToString().ToLower();
                        key_h.Content = key_h.Content.ToString().ToLower();
                        key_j.Content = key_j.Content.ToString().ToLower();
                        key_k.Content = key_k.Content.ToString().ToLower();
                        key_l.Content = key_l.Content.ToString().ToLower();

                        key_Semicolon_Normal.Text = ";";
                        key_Semicolon_Caps.Text = ":";

                        key_Quote_Normal.Text = "'";
                        key_Quote_Caps.Text = "\"";

                        key_Pipe_Normal.Text = "\\";
                        key_Pipe_Caps.Text = "|";

                        key_z.Content = key_z.Content.ToString().ToLower();
                        key_x.Content = key_x.Content.ToString().ToLower();
                        key_c.Content = key_c.Content.ToString().ToLower();
                        key_v.Content = key_v.Content.ToString().ToLower();
                        key_b.Content = key_b.Content.ToString().ToLower();
                        key_n.Content = key_n.Content.ToString().ToLower();
                        key_m.Content = key_m.Content.ToString().ToLower();

                        key_Comma_Normal.Text = ",";
                        key_Comma_Caps.Text = "<";

                        key_Period_Normal.Text = ".";
                        key_Period_Caps.Text = ">";

                        key_Question_Normal.Text = "/";
                        key_Question_Caps.Text = "?";

                        key_Shift.Content = "Shift";
                        key_Control.Content = "Ctrl";
                        key_Alt.Content = "Alt";
                        key_LeftWindows.Content = "Windows";
                        key_Space.Content = "Space";
                        key_Enter.Content = "Enter";

                        key_Escape.Content = "Escape";
                        key_Escape.Tag = KeysVirtual.Escape;

                        //Update the domain extension
                        UpdateDomainExtension();

                        //Update the volume down button
                        key_VolumeDown.Tag = KeysVirtual.VolumeDown;
                        image_VolumeDown.Source = FileToBitmapImage(new string[] { "Assets/Icons/VolumeDown.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    });
                }
                else
                {
                    vCapsEnabled = true;
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        key_Delete.Content = "Insert";
                        key_Delete.Tag = KeysVirtual.Insert;

                        key_End.Content = "Hme";
                        key_End.Tag = KeysVirtual.Home;

                        key_Tilde_Normal.Text = "~";
                        key_Tilde_Caps.Text = "`";

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

                        key_Minus_Normal.Text = "_";
                        key_Minus_Caps.Text = "-";

                        key_Plus_Normal.Text = "+";
                        key_Plus_Caps.Text = "=";

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

                        key_OpenBracket_Normal.Text = "{";
                        key_OpenBracket_Caps.Text = "[";

                        key_CloseBracket_Normal.Text = "}";
                        key_CloseBracket_Caps.Text = "]";

                        key_CapsLock.Content = "CAPS";
                        key_a.Content = key_a.Content.ToString().ToUpper();
                        key_s.Content = key_s.Content.ToString().ToUpper();
                        key_d.Content = key_d.Content.ToString().ToUpper();
                        key_f.Content = key_f.Content.ToString().ToUpper();
                        key_g.Content = key_g.Content.ToString().ToUpper();
                        key_h.Content = key_h.Content.ToString().ToUpper();
                        key_j.Content = key_j.Content.ToString().ToUpper();
                        key_k.Content = key_k.Content.ToString().ToUpper();
                        key_l.Content = key_l.Content.ToString().ToUpper();

                        key_Semicolon_Normal.Text = ":";
                        key_Semicolon_Caps.Text = ";";

                        key_Quote_Normal.Text = "\"";
                        key_Quote_Caps.Text = "'";

                        key_Pipe_Normal.Text = "|";
                        key_Pipe_Caps.Text = "\\";

                        key_z.Content = key_z.Content.ToString().ToUpper();
                        key_x.Content = key_x.Content.ToString().ToUpper();
                        key_c.Content = key_c.Content.ToString().ToUpper();
                        key_v.Content = key_v.Content.ToString().ToUpper();
                        key_b.Content = key_b.Content.ToString().ToUpper();
                        key_n.Content = key_n.Content.ToString().ToUpper();
                        key_m.Content = key_m.Content.ToString().ToUpper();

                        key_Comma_Normal.Text = "<";
                        key_Comma_Caps.Text = ",";

                        key_Period_Normal.Text = ">";
                        key_Period_Caps.Text = ".";

                        key_Question_Normal.Text = "?";
                        key_Question_Caps.Text = "/";

                        key_Shift.Content = "Cut";
                        key_Control.Content = "Copy";
                        key_Alt.Content = "Paste";
                        key_LeftWindows.Content = "Select all";
                        key_Space.Content = "Task manager";
                        key_Enter.Content = "Undo";

                        key_Escape.Content = "Menu";
                        key_Escape.Tag = KeysVirtual.ContextMenu;

                        //Update the domain extension
                        UpdateDomainExtension();

                        //Update the volume mute button
                        key_VolumeDown.Tag = KeysVirtual.VolumeMute;
                        image_VolumeDown.Source = FileToBitmapImage(new string[] { "Assets/Icons/VolumeMute.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    });
                }
            }
            catch { }
        }

        //Disable hardware capslock
        public async Task DisableHardwareCapsLock()
        {
            try
            {
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    if (System.Windows.Input.Keyboard.GetKeyStates(Key.CapsLock) == KeyStates.Toggled)
                    {
                        await KeyPressSingle((byte)KeysVirtual.CapsLock, false);
                    }
                });
            }
            catch { }
        }
    }
}