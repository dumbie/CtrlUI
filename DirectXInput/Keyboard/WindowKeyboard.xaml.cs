using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVWindowFunctions;
using static DirectXInput.AppVariables;
using static DirectXInput.WindowMain;
using static LibraryShared.FocusFunctions;
using static LibraryShared.Settings;
using static LibraryShared.SoundPlayer;
using static LibraryUsb.FakerInputDevice;

namespace DirectXInput.KeyboardCode
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

                //Set render mode to software
                HwndSource hwndSource = HwndSource.FromHwnd(vInteropWindowHandle);
                HwndTarget hwndTarget = hwndSource.CompositionTarget;
                hwndTarget.RenderMode = RenderMode.SoftwareOnly;

                //Update the window style
                WindowUpdateStyleVisible(vInteropWindowHandle, true, true, false);

                //Update the user interface clock style
                UpdateClockStyle();

                //Update keyboard keys
                UpdateKeyboardKeys();

                //Update the keyboard layout
                UpdateKeyboardLayout();

                //Set current keyboard mode
                await SetKeyboardMode();

                //Update the window position
                UpdateWindowPosition();

                //Update the listbox sources
                UpdateListBoxSources();

                //Register Hotkeys and Filtermessage
                ComponentDispatcher.ThreadFilterMessage += ReceivedFilterMessage;

                //Make window able to drag from border
                this.MouseDown += Window_MouseDown;

                //Check if resolution has changed
                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            }
            catch { }
        }

        //Hide the window
        public new async Task Hide()
        {
            try
            {
                if (vWindowVisible)
                {
                    //Play window close sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false, false);

                    //Stop the update tasks
                    await TasksBackgroundStop();

                    //Update the window visibility
                    UpdateWindowVisibility(false);

                    //Update last active status
                    vKeyboardKeypadLastActive = "Keyboard";

                    //Release keyboard and mouse
                    vFakerInputDevice.KeyboardReset();
                    vFakerInputDevice.MouseResetAbsolute();
                    vFakerInputDevice.MouseResetRelative();

                    //Delay CtrlUI output
                    vController0.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayTicks250;
                    vController1.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayTicks250;
                    vController2.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayTicks250;
                    vController3.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayTicks250;
                }
            }
            catch { }
        }

        //Show the window
        public async Task Show(bool forceKeyboardMode, bool resetPosition)
        {
            try
            {
                //Close other popups
                await App.vWindowKeypad.Hide();

                //Delay keyboard input
                vControllerDelay_Keyboard = GetSystemTicksMs() + vControllerDelayTicks250;

                //Play window open sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false, false);

                //Start the update tasks
                TasksBackgroundStart();

                //Disable hardware capslock
                DisableHardwareCapsLock();

                //Enable hardware numlock
                EnableHardwareNumLock();

                //Focus on keyboard button
                await FocusPopupButton(false, key_h);

                //Update the window visibility
                UpdateWindowVisibility(true);

                //Force keyboard mode
                if (forceKeyboardMode)
                {
                    await SetModeKeyboard();
                }

                //Update the window position
                if (resetPosition || Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "KeyboardResetPosition")))
                {
                    UpdateWindowPosition();
                }

                //Move mouse cursor to target
                MoveMousePosition();
            }
            catch { }
        }

        //Drag the window around
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
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
        void UpdateWindowVisibility(bool visible)
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
                        WindowUpdateStyleVisible(vInteropWindowHandle, true, true, false);

                        this.Title = "DirectXInput Keyboard (Visible)";
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

                        this.Title = "DirectXInput Keyboard (Hidden)";
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
                int monitorNumber = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "DisplayMonitor"));

                //Move the window position
                WindowUpdatePosition(monitorNumber, vInteropWindowHandle, AVWindowPosition.BottomCenter);
            }
            catch { }
        }

        //Focus on popup button
        async Task FocusPopupButton(bool forceFocus, Button targetButton)
        {
            try
            {
                if (forceFocus || Keyboard.FocusedElement == null || Keyboard.FocusedElement == this)
                {
                    await FrameworkElementFocus(targetButton, false, vInteropWindowHandle);
                }
            }
            catch { }
        }

        //Update the listbox sources
        void UpdateListBoxSources()
        {
            try
            {
                listbox_TextList.ItemsSource = vDirectKeyboardTextList;
                listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListSmiley;

                //Check if texts are set
                if (!vDirectKeyboardTextList.Any())
                {
                    textblock_TextListNoTextSet.Visibility = Visibility.Visible;
                }

                //Background brush variable
                SolidColorBrush selectedBrush = (SolidColorBrush)Application.Current.Resources["ApplicationAccentDarkBrush"];

                //Set background brush
                key_EmojiSmiley.Background = selectedBrush;
                key_EmojiSmiley.BorderBrush = selectedBrush;
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
                key_F1.Tag = KeyboardKeys.F1;
                key_F2.Content = "F2";
                key_F2.Tag = KeyboardKeys.F2;
                key_F3.Content = "F3";
                key_F3.Tag = KeyboardKeys.F3;
                key_F4.Content = "F4";
                key_F4.Tag = KeyboardKeys.F4;
                key_F5.Content = "F5";
                key_F5.Tag = KeyboardKeys.F5;
                key_F6.Content = "F6";
                key_F6.Tag = KeyboardKeys.F6;
                key_F7.Content = "F7";
                key_F7.Tag = KeyboardKeys.F7;
                key_F8.Content = "F8";
                key_F8.Tag = KeyboardKeys.F8;
                key_F9.Content = "F9";
                key_F9.Tag = KeyboardKeys.F9;
                key_F10.Content = "F10";
                key_F10.Tag = KeyboardKeys.F10;
                key_F11.Content = "F11";
                key_F11.Tag = KeyboardKeys.F11;
                key_F12.Content = "F12";
                key_F12.Tag = KeyboardKeys.F12;
                key_Delete.Content = "Delete";
                key_Delete.Tag = KeyboardKeys.Delete;
                key_End.Content = "End";
                key_End.Tag = KeyboardKeys.End;

                //Key Row 1
                key_Tilde_Normal.Text = "`";
                key_Tilde_Caps.Text = "~";
                key_Tilde.Tag = KeyboardKeys.Backtick;
                key_1_Normal.Text = "1";
                key_1_Caps.Text = "!";
                key_1.Tag = KeyboardKeys.Digit1;
                key_2_Normal.Text = "2";
                key_2_Caps.Text = "@";
                key_2.Tag = KeyboardKeys.Digit2;
                key_3_Normal.Text = "3";
                key_3_Caps.Text = "#";
                key_3.Tag = KeyboardKeys.Digit3;
                key_4_Normal.Text = "4";
                key_4_Caps.Text = "$";
                key_4.Tag = KeyboardKeys.Digit4;
                key_5_Normal.Text = "5";
                key_5_Caps.Text = "%";
                key_5.Tag = KeyboardKeys.Digit5;
                key_6_Normal.Text = "6";
                key_6_Caps.Text = "^";
                key_6.Tag = KeyboardKeys.Digit6;
                key_7_Normal.Text = "7";
                key_7_Caps.Text = "&";
                key_7.Tag = KeyboardKeys.Digit7;
                key_8_Normal.Text = "8";
                key_8_Caps.Text = "*";
                key_8.Tag = KeyboardKeys.Digit8;
                key_9_Normal.Text = "9";
                key_9_Caps.Text = "(";
                key_9.Tag = KeyboardKeys.Digit9;
                key_0_Normal.Text = "0";
                key_0_Caps.Text = ")";
                key_0.Tag = KeyboardKeys.Digit0;
                key_Minus_Normal.Text = "-";
                key_Minus_Caps.Text = "_";
                key_Minus.Tag = KeyboardKeys.Minus;
                key_Plus_Normal.Text = "=";
                key_Plus_Caps.Text = "+";
                key_Plus.Tag = KeyboardKeys.Equal;
                key_BackSpace.Content = "Backspace";
                key_BackSpace.Tag = KeyboardKeys.Backspace;

                //Key Row 2
                key_Tab.Content = "Tab>";
                key_Tab.Tag = KeyboardKeys.Tab;
                key_q.Content = "q";
                key_q.Tag = KeyboardKeys.Q;
                key_w.Content = "w";
                key_w.Tag = KeyboardKeys.W;
                key_e.Content = "e";
                key_e.Tag = KeyboardKeys.E;
                key_r.Content = "r";
                key_r.Tag = KeyboardKeys.R;
                key_t.Content = "t";
                key_t.Tag = KeyboardKeys.T;
                key_y.Content = "y";
                key_y.Tag = KeyboardKeys.Y;
                key_u.Content = "u";
                key_u.Tag = KeyboardKeys.U;
                key_i.Content = "i";
                key_i.Tag = KeyboardKeys.I;
                key_o.Content = "o";
                key_o.Tag = KeyboardKeys.O;
                key_p.Content = "p";
                key_p.Tag = KeyboardKeys.P;
                key_OpenBracket_Normal.Text = "[";
                key_OpenBracket_Caps.Text = "{";
                key_OpenBracket.Tag = KeyboardKeys.LeftBracket;
                key_CloseBracket_Normal.Text = "]";
                key_CloseBracket_Caps.Text = "}";
                key_CloseBracket.Tag = KeyboardKeys.RightBracket;

                //Key Row 3
                key_CapsLock.Content = "Caps";
                key_CapsLock.Tag = KeyboardKeys.CapsLock;
                key_a.Content = "a";
                key_a.Tag = KeyboardKeys.A;
                key_s.Content = "s";
                key_s.Tag = KeyboardKeys.S;
                key_d.Content = "d";
                key_d.Tag = KeyboardKeys.D;
                key_f.Content = "f";
                key_f.Tag = KeyboardKeys.F;
                key_g.Content = "g";
                key_g.Tag = KeyboardKeys.G;
                key_h.Content = "h";
                key_h.Tag = KeyboardKeys.H;
                key_j.Content = "j";
                key_j.Tag = KeyboardKeys.J;
                key_k.Content = "k";
                key_k.Tag = KeyboardKeys.K;
                key_l.Content = "l";
                key_l.Tag = KeyboardKeys.L;
                key_Semicolon_Normal.Text = ";";
                key_Semicolon_Caps.Text = ":";
                key_Semicolon.Tag = KeyboardKeys.Semicolon;
                key_Quote_Normal.Text = "'";
                key_Quote_Caps.Text = "\"";
                key_Quote.Tag = KeyboardKeys.Quote;
                key_Up.Content = "⯅";
                key_Up.Tag = KeyboardKeys.ArrowUp;
                key_Pipe_Normal.Text = "\\";
                key_Pipe_Caps.Text = "|";
                key_Pipe.Tag = KeyboardKeys.Backslash;

                //Key Row 4
                key_Shift.Content = "Shift";
                key_Shift.Tag = KeyboardModifiers.ShiftLeft;
                key_z.Content = "z";
                key_z.Tag = KeyboardKeys.Z;
                key_x.Content = "x";
                key_x.Tag = KeyboardKeys.X;
                key_c.Content = "c";
                key_c.Tag = KeyboardKeys.C;
                key_v.Content = "v";
                key_v.Tag = KeyboardKeys.V;
                key_b.Content = "b";
                key_b.Tag = KeyboardKeys.B;
                key_n.Content = "n";
                key_n.Tag = KeyboardKeys.N;
                key_m.Content = "m";
                key_m.Tag = KeyboardKeys.M;
                key_Comma_Normal.Text = ",";
                key_Comma_Caps.Text = "<";
                key_Comma.Tag = KeyboardKeys.Comma;
                key_Period_Normal.Text = ".";
                key_Period_Caps.Text = ">";
                key_Period.Tag = KeyboardKeys.Period;
                key_Question_Normal.Text = "/";
                key_Question_Caps.Text = "?";
                key_Question.Tag = KeyboardKeys.Slash;
                key_Left.Content = "⯇";
                key_Left.Tag = KeyboardKeys.ArrowLeft;
                key_Down.Content = "⯆";
                key_Down.Tag = KeyboardKeys.ArrowDown;
                key_Right.Content = "⯈";
                key_Right.Tag = KeyboardKeys.ArrowRight;

                //Key Row 5
                key_Control.Content = "Ctrl";
                key_Control.Tag = KeyboardModifiers.ControlLeft;
                key_Alt.Content = "Alt";
                key_Alt.Tag = KeyboardModifiers.AltLeft;
                key_LeftWindows.Content = "Windows";
                key_LeftWindows.Tag = KeyboardModifiers.WindowsLeft;
                key_Space.Content = "Space";
                key_Space.Tag = KeyboardKeys.Space;
                key_Enter.Content = "Enter";
                key_Enter.Tag = KeyboardKeys.Enter;
                key_Escape.Content = "Escape";
                key_Escape.Tag = KeyboardKeys.Escape;
                key_EmojiList.Tag = "EmojiPopup";
                key_TextList.Tag = "TextListPopup";
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
                if (Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "KeyboardLayout")) == 0)
                {
                    Debug.WriteLine("Switching keyboard layout: QWERTY");
                    key_a.Content = "a";
                    key_a.Tag = KeyboardKeys.A;
                    key_q.Content = "q";
                    key_q.Tag = KeyboardKeys.Q;
                    key_w.Content = "w";
                    key_w.Tag = KeyboardKeys.W;
                    key_y.Content = "y";
                    key_y.Tag = KeyboardKeys.Y;
                    key_z.Content = "z";
                    key_z.Tag = KeyboardKeys.Z;
                }
                else if (Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "KeyboardLayout")) == 1) //QWERTZ
                {
                    Debug.WriteLine("Switching keyboard layout: QWERTZ");
                    key_a.Content = "a";
                    key_a.Tag = KeyboardKeys.A;
                    key_q.Content = "q";
                    key_q.Tag = KeyboardKeys.Q;
                    key_w.Content = "w";
                    key_w.Tag = KeyboardKeys.W;
                    key_y.Content = "z";
                    key_y.Tag = KeyboardKeys.Z;
                    key_z.Content = "y";
                    key_z.Tag = KeyboardKeys.Y;
                }
                else if (Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "KeyboardLayout")) == 2) //AZERTY
                {
                    Debug.WriteLine("Switching keyboard layout: AZERTY");
                    key_a.Content = "q";
                    key_a.Tag = KeyboardKeys.Q;
                    key_q.Content = "a";
                    key_q.Tag = KeyboardKeys.A;
                    key_w.Content = "z";
                    key_w.Tag = KeyboardKeys.Z;
                    key_y.Content = "y";
                    key_y.Tag = KeyboardKeys.Y;
                    key_z.Content = "w";
                    key_z.Tag = KeyboardKeys.W;
                }
            }
            catch { }
        }

        //Switch capslock on and off
        public void SwitchCapsLock()
        {
            try
            {
                PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                Debug.WriteLine("Switching caps lock.");

                //Disable hardware capslock
                DisableHardwareCapsLock();

                //Enable hardware numlock
                EnableHardwareNumLock();

                //Enable or disable software capslock
                if (vCapsEnabled)
                {
                    vCapsEnabled = false;
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        textblock_ButtonLeft.Text = "Backspace";

                        key_Delete.Content = "Delete";
                        key_Delete.Tag = KeyboardKeys.Delete;

                        key_End.Content = "End";
                        key_End.Tag = KeyboardKeys.End;

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
                        key_Enter.Content = "Enter";

                        key_Escape.Content = "Escape";
                        key_Escape.Tag = KeyboardKeys.Escape;
                    });
                }
                else
                {
                    vCapsEnabled = true;
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        textblock_ButtonLeft.Text = "Delete";

                        key_Delete.Content = "Insert";
                        key_Delete.Tag = KeyboardKeys.Insert;

                        key_End.Content = "Home";
                        key_End.Tag = KeyboardKeys.Home;

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
                        key_Enter.Content = "Undo";

                        key_Escape.Content = "Menu";
                        key_Escape.Tag = KeyboardKeys.Menu;
                    });
                }
            }
            catch { }
        }
    }
}