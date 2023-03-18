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
using static ArnoldVinkCode.AVFocus;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVWindowFunctions;
using static DirectXInput.AppVariables;
using static DirectXInput.WindowMain;
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

                //Update window style
                WindowUpdateStyle(vInteropWindowHandle, true, true, false);

                //Update the user interface clock style
                UpdateClockStyle();

                //Update keyboard keys to normal
                UpdateKeyboardKeysNormal();

                //Update the keyboard layout
                UpdateKeyboardLayout();

                //Set current keyboard mode
                await SetKeyboardMode();

                //Update window position
                UpdateWindowPosition();

                //Update the listbox sources
                ToolList_AddItems();
                ShortcutList_AddItems();
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

                //Update the window visibility
                UpdateWindowVisibility(true);

                //Focus on keyboard button
                await FocusPopupButton(false, key_h);

                //Force keyboard mode
                if (forceKeyboardMode)
                {
                    await SetModeKeyboard();
                }

                //Update window position
                if (resetPosition || SettingLoad(vConfigurationDirectXInput, "KeyboardResetPosition", typeof(bool)))
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

        //Update window position on resolution change
        public async void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                //Wait for change to complete
                await Task.Delay(2000);

                //Update window style
                WindowUpdateStyle(vInteropWindowHandle, true, true, false);

                //Update window position
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

                        //Update window visibility
                        WindowUpdateVisibility(vInteropWindowHandle, true);

                        //Update window style
                        WindowUpdateStyle(vInteropWindowHandle, true, true, false);

                        this.Title = "DirectXInput Keyboard (Visible)";
                        vWindowVisible = true;
                        Debug.WriteLine("Showing the window.");
                    }
                }
                else
                {
                    if (vWindowVisible)
                    {
                        //Update window visibility
                        WindowUpdateVisibility(vInteropWindowHandle, false);

                        this.Title = "DirectXInput Keyboard (Hidden)";
                        vWindowVisible = false;
                        Debug.WriteLine("Hiding the window.");
                    }
                }
            }
            catch { }
        }

        //Update window position
        public void UpdateWindowPosition()
        {
            try
            {
                //Get the current active screen
                int monitorNumber = SettingLoad(vConfigurationCtrlUI, "DisplayMonitor", typeof(int));

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
                    await FocusElement(targetButton, this, vInteropWindowHandle);
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
                listbox_ShortcutList.ItemsSource = vDirectKeyboardShortcutList;
                listbox_ToolList.ItemsSource = vDirectKeyboardToolList;

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

        //Update keyboard keys to normal
        public void UpdateKeyboardKeysNormal()
        {
            try
            {
                //Help text
                textblock_ButtonLeft.Text = "Backspace";

                //Key Row 0
                key_F1_Normal.Text = "F1";
                key_F1_Caps.Text = "€";
                key_F1.Tag = new KeyboardAction() { Key0 = KeyboardKeys.F1 };
                key_F2_Normal.Text = "F2";
                key_F2_Caps.Text = "£";
                key_F2.Tag = new KeyboardAction() { Key0 = KeyboardKeys.F2 };
                key_F3_Normal.Text = "F3";
                key_F3_Caps.Text = "¥";
                key_F3.Tag = new KeyboardAction() { Key0 = KeyboardKeys.F3 };
                key_F4_Normal.Text = "F4";
                key_F4_Caps.Text = "¢";
                key_F4.Tag = new KeyboardAction() { Key0 = KeyboardKeys.F4 };
                key_F5_Normal.Text = "F5";
                key_F5_Caps.Text = "°";
                key_F5.Tag = new KeyboardAction() { Key0 = KeyboardKeys.F5 };
                key_F6_Normal.Text = "F6";
                key_F6_Caps.Text = "÷";
                key_F6.Tag = new KeyboardAction() { Key0 = KeyboardKeys.F6 };
                key_F7_Normal.Text = "F7";
                key_F7_Caps.Text = "½";
                key_F7.Tag = new KeyboardAction() { Key0 = KeyboardKeys.F7 };
                key_F8_Normal.Text = "F8";
                key_F8_Caps.Text = "©";
                key_F8.Tag = new KeyboardAction() { Key0 = KeyboardKeys.F8 };
                key_F9_Normal.Text = "F9";
                key_F9_Caps.Text = "™";
                key_F9.Tag = new KeyboardAction() { Key0 = KeyboardKeys.F9 };
                key_F10_Normal.Text = "F10";
                key_F10_Caps.Text = "¿";
                key_F10.Tag = new KeyboardAction() { Key0 = KeyboardKeys.F10 };
                key_F11_Normal.Text = "F11";
                key_F11_Caps.Text = "PgD";
                key_F11.Tag = new KeyboardAction() { Key0 = KeyboardKeys.F11 };
                key_F12_Normal.Text = "F12";
                key_F12_Caps.Text = "PgU";
                key_F12.Tag = new KeyboardAction() { Key0 = KeyboardKeys.F12 };
                key_Insert.Content = "Insert";
                key_Insert.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Insert };
                key_End.Content = "End";
                key_End.Tag = new KeyboardAction() { Key0 = KeyboardKeys.End };

                //Key Row 1
                key_Tilde_Normal.Text = "`";
                key_Tilde_Caps.Text = "~";
                key_Tilde.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Backtick };
                key_1_Normal.Text = "1";
                key_1_Caps.Text = "!";
                key_1.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Digit1 };
                key_2_Normal.Text = "2";
                key_2_Caps.Text = "@";
                key_2.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Digit2 };
                key_3_Normal.Text = "3";
                key_3_Caps.Text = "#";
                key_3.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Digit3 };
                key_4_Normal.Text = "4";
                key_4_Caps.Text = "$";
                key_4.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Digit4 };
                key_5_Normal.Text = "5";
                key_5_Caps.Text = "%";
                key_5.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Digit5 };
                key_6_Normal.Text = "6";
                key_6_Caps.Text = "^";
                key_6.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Digit6 };
                key_7_Normal.Text = "7";
                key_7_Caps.Text = "&";
                key_7.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Digit7 };
                key_8_Normal.Text = "8";
                key_8_Caps.Text = "*";
                key_8.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Digit8 };
                key_9_Normal.Text = "9";
                key_9_Caps.Text = "(";
                key_9.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Digit9 };
                key_0_Normal.Text = "0";
                key_0_Caps.Text = ")";
                key_0.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Digit0 };
                key_Minus_Normal.Text = "-";
                key_Minus_Caps.Text = "_";
                key_Minus.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Minus };
                key_Plus_Normal.Text = "=";
                key_Plus_Caps.Text = "+";
                key_Plus.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Equal };
                key_BackSpace.Content = "Backspace";
                key_BackSpace.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Backspace };

                //Key Row 2
                key_Tab.Content = "Tab>";
                key_Tab.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Tab };
                key_q.Content = "q";
                key_q.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Q };
                key_w.Content = "w";
                key_w.Tag = new KeyboardAction() { Key0 = KeyboardKeys.W };
                key_e.Content = "e";
                key_e.Tag = new KeyboardAction() { Key0 = KeyboardKeys.E };
                key_r.Content = "r";
                key_r.Tag = new KeyboardAction() { Key0 = KeyboardKeys.R };
                key_t.Content = "t";
                key_t.Tag = new KeyboardAction() { Key0 = KeyboardKeys.T };
                key_y.Content = "y";
                key_y.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Y };
                key_u.Content = "u";
                key_u.Tag = new KeyboardAction() { Key0 = KeyboardKeys.U };
                key_i.Content = "i";
                key_i.Tag = new KeyboardAction() { Key0 = KeyboardKeys.I };
                key_o.Content = "o";
                key_o.Tag = new KeyboardAction() { Key0 = KeyboardKeys.O };
                key_p.Content = "p";
                key_p.Tag = new KeyboardAction() { Key0 = KeyboardKeys.P };
                key_OpenBracket_Normal.Text = "[";
                key_OpenBracket_Caps.Text = "{";
                key_OpenBracket.Tag = new KeyboardAction() { Key0 = KeyboardKeys.LeftBracket };
                key_CloseBracket_Normal.Text = "]";
                key_CloseBracket_Caps.Text = "}";
                key_CloseBracket.Tag = new KeyboardAction() { Key0 = KeyboardKeys.RightBracket };

                //Key Row 3
                key_CapsLock.Content = "Caps";
                key_CapsLock.Tag = new KeyboardAction() { Key0 = KeyboardKeys.CapsLock };
                key_a.Content = "a";
                key_a.Tag = new KeyboardAction() { Key0 = KeyboardKeys.A };
                key_s.Content = "s";
                key_s.Tag = new KeyboardAction() { Key0 = KeyboardKeys.S };
                key_d.Content = "d";
                key_d.Tag = new KeyboardAction() { Key0 = KeyboardKeys.D };
                key_f.Content = "f";
                key_f.Tag = new KeyboardAction() { Key0 = KeyboardKeys.F };
                key_g.Content = "g";
                key_g.Tag = new KeyboardAction() { Key0 = KeyboardKeys.G };
                key_h.Content = "h";
                key_h.Tag = new KeyboardAction() { Key0 = KeyboardKeys.H };
                key_j.Content = "j";
                key_j.Tag = new KeyboardAction() { Key0 = KeyboardKeys.J };
                key_k.Content = "k";
                key_k.Tag = new KeyboardAction() { Key0 = KeyboardKeys.K };
                key_l.Content = "l";
                key_l.Tag = new KeyboardAction() { Key0 = KeyboardKeys.L };
                key_Semicolon_Normal.Text = ";";
                key_Semicolon_Caps.Text = ":";
                key_Semicolon.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Semicolon };
                key_Quote_Normal.Text = "'";
                key_Quote_Caps.Text = "\"";
                key_Quote.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Quote };
                key_Up.Content = "⯅";
                key_Up.Tag = new KeyboardAction() { Key0 = KeyboardKeys.ArrowUp };
                key_Pipe_Normal.Text = "\\";
                key_Pipe_Caps.Text = "|";
                key_Pipe.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Backslash };

                //Key Row 4
                key_Shift.Content = "Shift";
                key_Shift.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft };
                key_z.Content = "z";
                key_z.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Z };
                key_x.Content = "x";
                key_x.Tag = new KeyboardAction() { Key0 = KeyboardKeys.X };
                key_c.Content = "c";
                key_c.Tag = new KeyboardAction() { Key0 = KeyboardKeys.C };
                key_v.Content = "v";
                key_v.Tag = new KeyboardAction() { Key0 = KeyboardKeys.V };
                key_b.Content = "b";
                key_b.Tag = new KeyboardAction() { Key0 = KeyboardKeys.B };
                key_n.Content = "n";
                key_n.Tag = new KeyboardAction() { Key0 = KeyboardKeys.N };
                key_m.Content = "m";
                key_m.Tag = new KeyboardAction() { Key0 = KeyboardKeys.M };
                key_Comma_Normal.Text = ",";
                key_Comma_Caps.Text = "<";
                key_Comma.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Comma };
                key_Period_Normal.Text = ".";
                key_Period_Caps.Text = ">";
                key_Period.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Period };
                key_Question_Normal.Text = "/";
                key_Question_Caps.Text = "?";
                key_Question.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Slash };
                key_Left.Content = "⯇";
                key_Left.Tag = new KeyboardAction() { Key0 = KeyboardKeys.ArrowLeft };
                key_Down.Content = "⯆";
                key_Down.Tag = new KeyboardAction() { Key0 = KeyboardKeys.ArrowDown };
                key_Right.Content = "⯈";
                key_Right.Tag = new KeyboardAction() { Key0 = KeyboardKeys.ArrowRight };

                //Key Row 5
                key_Control.Content = "Ctrl";
                key_Control.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ControlLeft };
                key_Alt.Content = "Alt";
                key_Alt.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.AltLeft };
                key_LeftWindows.Content = "Windows";
                key_LeftWindows.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.WindowsLeft };
                key_Space.Content = "Space";
                key_Space.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Space };
                key_Enter.Content = "Enter";
                key_Enter.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Enter };
                key_Escape.Content = "Escape";
                key_Escape.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Escape };
                key_EmojiList.Tag = "EmojiPopup";
                key_TextList.Tag = "TextListPopup";
                key_ShortcutList.Tag = "ShortcutListPopup";
            }
            catch { }
        }

        //Update keyboard keys to caps
        public void UpdateKeyboardKeysCaps()
        {
            try
            {
                //Help text
                textblock_ButtonLeft.Text = "Delete";

                //Key Row 0
                key_F1_Normal.Text = "€";
                key_F1_Caps.Text = "F1";
                key_F1.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.AltLeft, Key0 = KeyboardKeys.Numpad0, Key1 = KeyboardKeys.Numpad1, Key2 = KeyboardKeys.Numpad2, Key3 = KeyboardKeys.Numpad8 };
                key_F2_Normal.Text = "£";
                key_F2_Caps.Text = "F2";
                key_F2.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.AltLeft, Key0 = KeyboardKeys.Numpad0, Key1 = KeyboardKeys.Numpad1, Key2 = KeyboardKeys.Numpad6, Key3 = KeyboardKeys.Numpad3 };
                key_F3_Normal.Text = "¥";
                key_F3_Caps.Text = "F3";
                key_F3.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.AltLeft, Key0 = KeyboardKeys.Numpad0, Key1 = KeyboardKeys.Numpad1, Key2 = KeyboardKeys.Numpad6, Key3 = KeyboardKeys.Numpad5 };
                key_F4_Normal.Text = "¢";
                key_F4_Caps.Text = "F4";
                key_F4.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.AltLeft, Key0 = KeyboardKeys.Numpad0, Key1 = KeyboardKeys.Numpad1, Key2 = KeyboardKeys.Numpad6, Key3 = KeyboardKeys.Numpad2 };
                key_F5_Normal.Text = "°";
                key_F5_Caps.Text = "F5";
                key_F5.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.AltLeft, Key0 = KeyboardKeys.Numpad0, Key1 = KeyboardKeys.Numpad1, Key2 = KeyboardKeys.Numpad7, Key3 = KeyboardKeys.Numpad6 };
                key_F6_Normal.Text = "÷";
                key_F6_Caps.Text = "F6";
                key_F6.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.AltLeft, Key0 = KeyboardKeys.Numpad0, Key1 = KeyboardKeys.Numpad2, Key2 = KeyboardKeys.Numpad4, Key3 = KeyboardKeys.Numpad7 };
                key_F7_Normal.Text = "½";
                key_F7_Caps.Text = "F7";
                key_F7.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.AltLeft, Key0 = KeyboardKeys.Numpad0, Key1 = KeyboardKeys.Numpad1, Key2 = KeyboardKeys.Numpad8, Key3 = KeyboardKeys.Numpad9 };
                key_F8_Normal.Text = "©";
                key_F8_Caps.Text = "F8";
                key_F8.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.AltLeft, Key0 = KeyboardKeys.Numpad0, Key1 = KeyboardKeys.Numpad1, Key2 = KeyboardKeys.Numpad6, Key3 = KeyboardKeys.Numpad9 };
                key_F9_Normal.Text = "™";
                key_F9_Caps.Text = "F9";
                key_F9.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.AltLeft, Key0 = KeyboardKeys.Numpad0, Key1 = KeyboardKeys.Numpad1, Key2 = KeyboardKeys.Numpad5, Key3 = KeyboardKeys.Numpad3 };
                key_F10_Normal.Text = "¿";
                key_F10_Caps.Text = "F10";
                key_F10.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.AltLeft, Key0 = KeyboardKeys.Numpad0, Key1 = KeyboardKeys.Numpad1, Key2 = KeyboardKeys.Numpad9, Key3 = KeyboardKeys.Numpad1 };
                key_F11_Normal.Text = "PgD";
                key_F11_Caps.Text = "F11";
                key_F11.Tag = new KeyboardAction() { Key0 = KeyboardKeys.PageDown };
                key_F12_Normal.Text = "PgU";
                key_F12_Caps.Text = "F12";
                key_F12.Tag = new KeyboardAction() { Key0 = KeyboardKeys.PageUp };
                key_Insert.Content = "Redo";
                key_Insert.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ControlLeft, Key0 = KeyboardKeys.Y };
                key_End.Content = "Home";
                key_End.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Home };

                //Key Row 1
                key_Tilde_Normal.Text = "~";
                key_Tilde_Caps.Text = "`";
                key_Tilde.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Backtick };
                key_1_Normal.Text = "!";
                key_1_Caps.Text = "1";
                key_1.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Digit1 };
                key_2_Normal.Text = "@";
                key_2_Caps.Text = "2";
                key_2.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Digit2 };
                key_3_Normal.Text = "#";
                key_3_Caps.Text = "3";
                key_3.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Digit3 };
                key_4_Normal.Text = "$";
                key_4_Caps.Text = "4";
                key_4.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Digit4 };
                key_5_Normal.Text = "%";
                key_5_Caps.Text = "5";
                key_5.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Digit5 };
                key_6_Normal.Text = "^";
                key_6_Caps.Text = "6";
                key_6.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Digit6 };
                key_7_Normal.Text = "&";
                key_7_Caps.Text = "7";
                key_7.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Digit7 };
                key_8_Normal.Text = "*";
                key_8_Caps.Text = "8";
                key_8.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Digit8 };
                key_9_Normal.Text = "(";
                key_9_Caps.Text = "9";
                key_9.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Digit9 };
                key_0_Normal.Text = ")";
                key_0_Caps.Text = "0";
                key_0.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Digit0 };
                key_Minus_Normal.Text = "_";
                key_Minus_Caps.Text = "-";
                key_Minus.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Minus };
                key_Plus_Normal.Text = "+";
                key_Plus_Caps.Text = "=";
                key_Plus.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Equal };
                key_BackSpace.Content = "Delete";
                key_BackSpace.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Delete };

                //Key Row 2
                key_Tab.Content = "<Tab";
                key_Tab.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Tab };
                key_q.Content = "Q";
                key_q.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Q };
                key_w.Content = "W";
                key_w.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.W };
                key_e.Content = "E";
                key_e.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.E };
                key_r.Content = "R";
                key_r.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.R };
                key_t.Content = "T";
                key_t.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.T };
                key_y.Content = "Y";
                key_y.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Y };
                key_u.Content = "U";
                key_u.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.U };
                key_i.Content = "I";
                key_i.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.I };
                key_o.Content = "O";
                key_o.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.O };
                key_p.Content = "P";
                key_p.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.P };
                key_OpenBracket_Normal.Text = "{";
                key_OpenBracket_Caps.Text = "[";
                key_OpenBracket.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.LeftBracket };
                key_CloseBracket_Normal.Text = "}";
                key_CloseBracket_Caps.Text = "]";
                key_CloseBracket.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.RightBracket };

                //Key Row 3
                key_CapsLock.Content = "CAPS";
                key_CapsLock.Tag = new KeyboardAction() { Key0 = KeyboardKeys.CapsLock };
                key_a.Content = "A";
                key_a.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.A };
                key_s.Content = "S";
                key_s.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.S };
                key_d.Content = "D";
                key_d.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.D };
                key_f.Content = "F";
                key_f.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.F };
                key_g.Content = "G";
                key_g.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.G };
                key_h.Content = "H";
                key_h.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.H };
                key_j.Content = "J";
                key_j.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.J };
                key_k.Content = "K";
                key_k.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.K };
                key_l.Content = "L";
                key_l.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.L };
                key_Semicolon_Normal.Text = ":";
                key_Semicolon_Caps.Text = ";";
                key_Semicolon.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Semicolon };
                key_Quote_Normal.Text = "\"";
                key_Quote_Caps.Text = "'";
                key_Quote.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Quote };
                key_Up.Content = "⯅";
                key_Up.Tag = new KeyboardAction() { Key0 = KeyboardKeys.ArrowUp };
                key_Pipe_Normal.Text = "|";
                key_Pipe_Caps.Text = "\\";
                key_Pipe.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Backslash };

                //Key Row 4
                key_Shift.Content = "Cut";
                key_Shift.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ControlLeft, Key0 = KeyboardKeys.X };
                key_z.Content = "Z";
                key_z.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Z };
                key_x.Content = "X";
                key_x.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.X };
                key_c.Content = "C";
                key_c.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.C };
                key_v.Content = "V";
                key_v.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.V };
                key_b.Content = "B";
                key_b.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.B };
                key_n.Content = "N";
                key_n.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.N };
                key_m.Content = "M";
                key_m.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.M };
                key_Comma_Normal.Text = "<";
                key_Comma_Caps.Text = ",";
                key_Comma.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Comma };
                key_Period_Normal.Text = ">";
                key_Period_Caps.Text = ".";
                key_Period.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Period };
                key_Question_Normal.Text = "?";
                key_Question_Caps.Text = "/";
                key_Question.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ShiftLeft, Key0 = KeyboardKeys.Slash };
                key_Left.Content = "⯇";
                key_Left.Tag = new KeyboardAction() { Key0 = KeyboardKeys.ArrowLeft };
                key_Down.Content = "⯆";
                key_Down.Tag = new KeyboardAction() { Key0 = KeyboardKeys.ArrowDown };
                key_Right.Content = "⯈";
                key_Right.Tag = new KeyboardAction() { Key0 = KeyboardKeys.ArrowRight };

                //Key Row 5
                key_Control.Content = "Copy";
                key_Control.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ControlLeft, Key0 = KeyboardKeys.C };
                key_Alt.Content = "Paste";
                key_Alt.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ControlLeft, Key0 = KeyboardKeys.V };
                key_LeftWindows.Content = "Select all";
                key_LeftWindows.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ControlLeft, Key0 = KeyboardKeys.A };
                key_Space.Content = "Space";
                key_Space.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Space };
                key_Enter.Content = "Undo";
                key_Enter.Tag = new KeyboardAction() { Modifiers = KeyboardModifiers.ControlLeft, Key0 = KeyboardKeys.Z };
                key_Escape.Content = "Menu";
                key_Escape.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Menu };
                key_EmojiList.Tag = "EmojiPopup";
                key_TextList.Tag = "TextListPopup";
                key_ShortcutList.Tag = "ShortcutListPopup";
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
                if (SettingLoad(vConfigurationDirectXInput, "KeyboardLayout", typeof(int)) == 0) //QWERTY
                {
                    Debug.WriteLine("Switching keyboard layout: QWERTY");
                    key_a.Content = "a";
                    key_a.Tag = new KeyboardAction() { Key0 = KeyboardKeys.A };
                    key_q.Content = "q";
                    key_q.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Q };
                    key_w.Content = "w";
                    key_w.Tag = new KeyboardAction() { Key0 = KeyboardKeys.W };
                    key_y.Content = "y";
                    key_y.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Y };
                    key_z.Content = "z";
                    key_z.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Z };
                }
                else if (SettingLoad(vConfigurationDirectXInput, "KeyboardLayout", typeof(int)) == 1) //QWERTZ
                {
                    Debug.WriteLine("Switching keyboard layout: QWERTZ");
                    key_a.Content = "a";
                    key_a.Tag = new KeyboardAction() { Key0 = KeyboardKeys.A };
                    key_q.Content = "q";
                    key_q.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Q };
                    key_w.Content = "w";
                    key_w.Tag = new KeyboardAction() { Key0 = KeyboardKeys.W };
                    key_y.Content = "z";
                    key_y.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Z };
                    key_z.Content = "y";
                    key_z.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Y };
                }
                else if (SettingLoad(vConfigurationDirectXInput, "KeyboardLayout", typeof(int)) == 2) //AZERTY
                {
                    Debug.WriteLine("Switching keyboard layout: AZERTY");
                    key_a.Content = "q";
                    key_a.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Q };
                    key_q.Content = "a";
                    key_q.Tag = new KeyboardAction() { Key0 = KeyboardKeys.A };
                    key_w.Content = "z";
                    key_w.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Z };
                    key_y.Content = "y";
                    key_y.Tag = new KeyboardAction() { Key0 = KeyboardKeys.Y };
                    key_z.Content = "w";
                    key_z.Tag = new KeyboardAction() { Key0 = KeyboardKeys.W };
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
                    AVActions.DispatcherInvoke(delegate
                    {
                        UpdateKeyboardKeysNormal();
                    });
                }
                else
                {
                    vCapsEnabled = true;
                    AVActions.DispatcherInvoke(delegate
                    {
                        UpdateKeyboardKeysCaps();
                    });
                }
            }
            catch { }
        }

        //Check if text popups are open
        bool CheckTextPopupsOpen()
        {
            bool textPopupsOpen = false;
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    textPopupsOpen = border_EmojiListPopup.Visibility == Visibility.Visible || border_TextListPopup.Visibility == Visibility.Visible || border_ShortcutListPopup.Visibility == Visibility.Visible;
                });
            }
            catch { }
            return textPopupsOpen;
        }

        //Hide open texts popup
        async Task HideTextPopups()
        {
            try
            {
                await AVActions.DispatcherInvoke(async delegate
                {
                    if (border_EmojiListPopup.Visibility == Visibility.Visible)
                    {
                        await HideEmojiPopup();
                    }
                    if (border_TextListPopup.Visibility == Visibility.Visible)
                    {
                        await HideTextPopup();
                    }
                    if (border_ShortcutListPopup.Visibility == Visibility.Visible)
                    {
                        await HideShortcutPopup();
                    }
                });
            }
            catch { }
        }
    }
}