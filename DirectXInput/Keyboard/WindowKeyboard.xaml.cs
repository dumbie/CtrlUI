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
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVWindowFunctions;
using static DirectXInput.AppVariables;
using static DirectXInput.WindowMain;
using static LibraryShared.ControllerTimings;
using static LibraryShared.Enums;
using static LibraryShared.SoundPlayer;

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
                ShortcutList_AddItems();
                UpdateListBoxSources();

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

                //Focus on keyboard button
                if (vKeyboardCurrentMode == KeyboardMode.Keyboard)
                {
                    await FocusPopupButton(false, key_h);
                }
                else
                {
                    await FocusPopupButton(true, key_Tool_SwitchMode);
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
                    await FocusElement(targetButton, vInteropWindowHandle);
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
                key_F1.Tag = new KeysHidAction() { Key0 = KeysHid.F1 };
                key_F2_Normal.Text = "F2";
                key_F2_Caps.Text = "£";
                key_F2.Tag = new KeysHidAction() { Key0 = KeysHid.F2 };
                key_F3_Normal.Text = "F3";
                key_F3_Caps.Text = "¥";
                key_F3.Tag = new KeysHidAction() { Key0 = KeysHid.F3 };
                key_F4_Normal.Text = "F4";
                key_F4_Caps.Text = "¢";
                key_F4.Tag = new KeysHidAction() { Key0 = KeysHid.F4 };
                key_F5_Normal.Text = "F5";
                key_F5_Caps.Text = "°";
                key_F5.Tag = new KeysHidAction() { Key0 = KeysHid.F5 };
                key_F6_Normal.Text = "F6";
                key_F6_Caps.Text = "÷";
                key_F6.Tag = new KeysHidAction() { Key0 = KeysHid.F6 };
                key_F7_Normal.Text = "F7";
                key_F7_Caps.Text = "±";
                key_F7.Tag = new KeysHidAction() { Key0 = KeysHid.F7 };
                key_F8_Normal.Text = "F8";
                key_F8_Caps.Text = "½";
                key_F8.Tag = new KeysHidAction() { Key0 = KeysHid.F8 };
                key_F9_Normal.Text = "F9";
                key_F9_Caps.Text = "¾";
                key_F9.Tag = new KeysHidAction() { Key0 = KeysHid.F9 };
                key_F10_Normal.Text = "F10";
                key_F10_Caps.Text = "©";
                key_F10.Tag = new KeysHidAction() { Key0 = KeysHid.F10 };
                key_F11_Normal.Text = "F11";
                key_F11_Caps.Text = "™";
                key_F11.Tag = new KeysHidAction() { Key0 = KeysHid.F11 };
                key_F12_Normal.Text = "F12";
                key_F12_Caps.Text = "¿";
                key_F12.Tag = new KeysHidAction() { Key0 = KeysHid.F12 };
                key_Insert.Content = "Insert";
                key_Insert.Tag = new KeysHidAction() { Key0 = KeysHid.Insert };
                key_End.Content = "End";
                key_End.Tag = new KeysHidAction() { Key0 = KeysHid.End };
                key_Save.Content = "Save";
                key_Save.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.CtrlLeft, Key0 = KeysHid.S };
                key_Page.Content = "PgDn";
                key_Page.Tag = new KeysHidAction() { Key0 = KeysHid.PageDown };

                //Key Row 1
                key_Tilde_Normal.Text = "`";
                key_Tilde_Caps.Text = "~";
                key_Tilde.Tag = new KeysHidAction() { Key0 = KeysHid.Tilde };
                key_1_Normal.Text = "1";
                key_1_Caps.Text = "!";
                key_1.Tag = new KeysHidAction() { Key0 = KeysHid.Digit1 };
                key_2_Normal.Text = "2";
                key_2_Caps.Text = "@";
                key_2.Tag = new KeysHidAction() { Key0 = KeysHid.Digit2 };
                key_3_Normal.Text = "3";
                key_3_Caps.Text = "#";
                key_3.Tag = new KeysHidAction() { Key0 = KeysHid.Digit3 };
                key_4_Normal.Text = "4";
                key_4_Caps.Text = "$";
                key_4.Tag = new KeysHidAction() { Key0 = KeysHid.Digit4 };
                key_5_Normal.Text = "5";
                key_5_Caps.Text = "%";
                key_5.Tag = new KeysHidAction() { Key0 = KeysHid.Digit5 };
                key_6_Normal.Text = "6";
                key_6_Caps.Text = "^";
                key_6.Tag = new KeysHidAction() { Key0 = KeysHid.Digit6 };
                key_7_Normal.Text = "7";
                key_7_Caps.Text = "&";
                key_7.Tag = new KeysHidAction() { Key0 = KeysHid.Digit7 };
                key_8_Normal.Text = "8";
                key_8_Caps.Text = "*";
                key_8.Tag = new KeysHidAction() { Key0 = KeysHid.Digit8 };
                key_9_Normal.Text = "9";
                key_9_Caps.Text = "(";
                key_9.Tag = new KeysHidAction() { Key0 = KeysHid.Digit9 };
                key_0_Normal.Text = "0";
                key_0_Caps.Text = ")";
                key_0.Tag = new KeysHidAction() { Key0 = KeysHid.Digit0 };
                key_Minus_Normal.Text = "-";
                key_Minus_Caps.Text = "_";
                key_Minus.Tag = new KeysHidAction() { Key0 = KeysHid.Minus };
                key_Plus_Normal.Text = "=";
                key_Plus_Caps.Text = "+";
                key_Plus.Tag = new KeysHidAction() { Key0 = KeysHid.Plus };
                key_BackSpace.Content = "Backspace";
                key_BackSpace.Tag = new KeysHidAction() { Key0 = KeysHid.BackSpace };

                //Key Row 2
                key_Tab.Content = "Tab>";
                key_Tab.Tag = new KeysHidAction() { Key0 = KeysHid.Tab };
                key_q.Content = "q";
                key_q.Tag = new KeysHidAction() { Key0 = KeysHid.Q };
                key_w.Content = "w";
                key_w.Tag = new KeysHidAction() { Key0 = KeysHid.W };
                key_e.Content = "e";
                key_e.Tag = new KeysHidAction() { Key0 = KeysHid.E };
                key_r.Content = "r";
                key_r.Tag = new KeysHidAction() { Key0 = KeysHid.R };
                key_t.Content = "t";
                key_t.Tag = new KeysHidAction() { Key0 = KeysHid.T };
                key_y.Content = "y";
                key_y.Tag = new KeysHidAction() { Key0 = KeysHid.Y };
                key_u.Content = "u";
                key_u.Tag = new KeysHidAction() { Key0 = KeysHid.U };
                key_i.Content = "i";
                key_i.Tag = new KeysHidAction() { Key0 = KeysHid.I };
                key_o.Content = "o";
                key_o.Tag = new KeysHidAction() { Key0 = KeysHid.O };
                key_p.Content = "p";
                key_p.Tag = new KeysHidAction() { Key0 = KeysHid.P };
                key_OpenBracket_Normal.Text = "[";
                key_OpenBracket_Caps.Text = "{";
                key_OpenBracket.Tag = new KeysHidAction() { Key0 = KeysHid.CloseBracket };
                key_CloseBracket_Normal.Text = "]";
                key_CloseBracket_Caps.Text = "}";
                key_CloseBracket.Tag = new KeysHidAction() { Key0 = KeysHid.OpenBracket };

                //Key Row 3
                key_CapsLock.Content = "Caps";
                key_CapsLock.Tag = new KeysHidAction() { Key0 = KeysHid.CapsLock };
                key_a.Content = "a";
                key_a.Tag = new KeysHidAction() { Key0 = KeysHid.A };
                key_s.Content = "s";
                key_s.Tag = new KeysHidAction() { Key0 = KeysHid.S };
                key_d.Content = "d";
                key_d.Tag = new KeysHidAction() { Key0 = KeysHid.D };
                key_f.Content = "f";
                key_f.Tag = new KeysHidAction() { Key0 = KeysHid.F };
                key_g.Content = "g";
                key_g.Tag = new KeysHidAction() { Key0 = KeysHid.G };
                key_h.Content = "h";
                key_h.Tag = new KeysHidAction() { Key0 = KeysHid.H };
                key_j.Content = "j";
                key_j.Tag = new KeysHidAction() { Key0 = KeysHid.J };
                key_k.Content = "k";
                key_k.Tag = new KeysHidAction() { Key0 = KeysHid.K };
                key_l.Content = "l";
                key_l.Tag = new KeysHidAction() { Key0 = KeysHid.L };
                key_Semicolon_Normal.Text = ";";
                key_Semicolon_Caps.Text = ":";
                key_Semicolon.Tag = new KeysHidAction() { Key0 = KeysHid.Semicolon };
                key_Quote_Normal.Text = "'";
                key_Quote_Caps.Text = "\"";
                key_Quote.Tag = new KeysHidAction() { Key0 = KeysHid.Quote };
                key_Up.Content = "⯅";
                key_Up.Tag = new KeysHidAction() { Key0 = KeysHid.ArrowUp };
                key_Pipe_Normal.Text = "\\";
                key_Pipe_Caps.Text = "|";
                key_Pipe.Tag = new KeysHidAction() { Key0 = KeysHid.BackSlash };

                //Key Row 4
                key_Shift.Content = "Shift";
                key_Shift.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft };
                key_z.Content = "z";
                key_z.Tag = new KeysHidAction() { Key0 = KeysHid.Z };
                key_x.Content = "x";
                key_x.Tag = new KeysHidAction() { Key0 = KeysHid.X };
                key_c.Content = "c";
                key_c.Tag = new KeysHidAction() { Key0 = KeysHid.C };
                key_v.Content = "v";
                key_v.Tag = new KeysHidAction() { Key0 = KeysHid.V };
                key_b.Content = "b";
                key_b.Tag = new KeysHidAction() { Key0 = KeysHid.B };
                key_n.Content = "n";
                key_n.Tag = new KeysHidAction() { Key0 = KeysHid.N };
                key_m.Content = "m";
                key_m.Tag = new KeysHidAction() { Key0 = KeysHid.M };
                key_Comma_Normal.Text = ",";
                key_Comma_Caps.Text = "<";
                key_Comma.Tag = new KeysHidAction() { Key0 = KeysHid.Comma };
                key_Period_Normal.Text = ".";
                key_Period_Caps.Text = ">";
                key_Period.Tag = new KeysHidAction() { Key0 = KeysHid.Period };
                key_Question_Normal.Text = "/";
                key_Question_Caps.Text = "?";
                key_Question.Tag = new KeysHidAction() { Key0 = KeysHid.Question };
                key_Left.Content = "⯇";
                key_Left.Tag = new KeysHidAction() { Key0 = KeysHid.ArrowLeft };
                key_Down.Content = "⯆";
                key_Down.Tag = new KeysHidAction() { Key0 = KeysHid.ArrowDown };
                key_Right.Content = "⯈";
                key_Right.Tag = new KeysHidAction() { Key0 = KeysHid.ArrowRight };

                //Key Row 5
                key_Control.Content = "Ctrl";
                key_Control.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.CtrlLeft };
                key_Alt.Content = "Alt";
                key_Alt.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.AltLeft };
                key_LeftWindows.Content = "Windows";
                key_LeftWindows.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.WindowsLeft };
                key_Space.Content = "Space";
                key_Space.Tag = new KeysHidAction() { Key0 = KeysHid.Space };
                key_Enter.Content = "Enter";
                key_Enter.Tag = new KeysHidAction() { Key0 = KeysHid.Enter };
                key_Escape.Content = "Escape";
                key_Escape.Tag = new KeysHidAction() { Key0 = KeysHid.Escape };
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
                key_F1.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.AltLeft, Key0 = KeysHid.Numpad0, Key1 = KeysHid.Numpad1, Key2 = KeysHid.Numpad2, Key3 = KeysHid.Numpad8 };
                key_F2_Normal.Text = "£";
                key_F2_Caps.Text = "F2";
                key_F2.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.AltLeft, Key0 = KeysHid.Numpad0, Key1 = KeysHid.Numpad1, Key2 = KeysHid.Numpad6, Key3 = KeysHid.Numpad3 };
                key_F3_Normal.Text = "¥";
                key_F3_Caps.Text = "F3";
                key_F3.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.AltLeft, Key0 = KeysHid.Numpad0, Key1 = KeysHid.Numpad1, Key2 = KeysHid.Numpad6, Key3 = KeysHid.Numpad5 };
                key_F4_Normal.Text = "¢";
                key_F4_Caps.Text = "F4";
                key_F4.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.AltLeft, Key0 = KeysHid.Numpad0, Key1 = KeysHid.Numpad1, Key2 = KeysHid.Numpad6, Key3 = KeysHid.Numpad2 };
                key_F5_Normal.Text = "°";
                key_F5_Caps.Text = "F5";
                key_F5.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.AltLeft, Key0 = KeysHid.Numpad0, Key1 = KeysHid.Numpad1, Key2 = KeysHid.Numpad7, Key3 = KeysHid.Numpad6 };
                key_F6_Normal.Text = "÷";
                key_F6_Caps.Text = "F6";
                key_F6.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.AltLeft, Key0 = KeysHid.Numpad0, Key1 = KeysHid.Numpad2, Key2 = KeysHid.Numpad4, Key3 = KeysHid.Numpad7 };
                key_F7_Normal.Text = "±";
                key_F7_Caps.Text = "F7";
                key_F7.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.AltLeft, Key1 = KeysHid.Numpad2, Key2 = KeysHid.Numpad4, Key3 = KeysHid.Numpad1 };
                key_F8_Normal.Text = "½";
                key_F8_Caps.Text = "F8";
                key_F8.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.AltLeft, Key0 = KeysHid.Numpad0, Key1 = KeysHid.Numpad1, Key2 = KeysHid.Numpad8, Key3 = KeysHid.Numpad9 };
                key_F9_Normal.Text = "¾";
                key_F9_Caps.Text = "F9";
                key_F9.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.AltLeft, Key0 = KeysHid.Numpad0, Key1 = KeysHid.Numpad1, Key2 = KeysHid.Numpad9, Key3 = KeysHid.Numpad0 };
                key_F10_Normal.Text = "©";
                key_F10_Caps.Text = "F10";
                key_F10.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.AltLeft, Key0 = KeysHid.Numpad0, Key1 = KeysHid.Numpad1, Key2 = KeysHid.Numpad6, Key3 = KeysHid.Numpad9 };
                key_F11_Normal.Text = "™";
                key_F11_Caps.Text = "F11";
                key_F11.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.AltLeft, Key0 = KeysHid.Numpad0, Key1 = KeysHid.Numpad1, Key2 = KeysHid.Numpad5, Key3 = KeysHid.Numpad3 };
                key_F12_Normal.Text = "¿";
                key_F12_Caps.Text = "F12";
                key_F12.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.AltLeft, Key0 = KeysHid.Numpad0, Key1 = KeysHid.Numpad1, Key2 = KeysHid.Numpad9, Key3 = KeysHid.Numpad1 };
                key_Insert.Content = "Redo";
                key_Insert.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.CtrlLeft, Key0 = KeysHid.Y };
                key_End.Content = "Home";
                key_End.Tag = new KeysHidAction() { Key0 = KeysHid.Home };
                key_Save.Content = "Search";
                key_Save.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.CtrlLeft, Key0 = KeysHid.F };
                key_Page.Content = "PgUp";
                key_Page.Tag = new KeysHidAction() { Key0 = KeysHid.PageUp };

                //Key Row 1
                key_Tilde_Normal.Text = "~";
                key_Tilde_Caps.Text = "`";
                key_Tilde.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Tilde };
                key_1_Normal.Text = "!";
                key_1_Caps.Text = "1";
                key_1.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Digit1 };
                key_2_Normal.Text = "@";
                key_2_Caps.Text = "2";
                key_2.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Digit2 };
                key_3_Normal.Text = "#";
                key_3_Caps.Text = "3";
                key_3.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Digit3 };
                key_4_Normal.Text = "$";
                key_4_Caps.Text = "4";
                key_4.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Digit4 };
                key_5_Normal.Text = "%";
                key_5_Caps.Text = "5";
                key_5.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Digit5 };
                key_6_Normal.Text = "^";
                key_6_Caps.Text = "6";
                key_6.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Digit6 };
                key_7_Normal.Text = "&";
                key_7_Caps.Text = "7";
                key_7.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Digit7 };
                key_8_Normal.Text = "*";
                key_8_Caps.Text = "8";
                key_8.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Digit8 };
                key_9_Normal.Text = "(";
                key_9_Caps.Text = "9";
                key_9.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Digit9 };
                key_0_Normal.Text = ")";
                key_0_Caps.Text = "0";
                key_0.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Digit0 };
                key_Minus_Normal.Text = "_";
                key_Minus_Caps.Text = "-";
                key_Minus.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Minus };
                key_Plus_Normal.Text = "+";
                key_Plus_Caps.Text = "=";
                key_Plus.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Plus };
                key_BackSpace.Content = "Delete";
                key_BackSpace.Tag = new KeysHidAction() { Key0 = KeysHid.Delete };

                //Key Row 2
                key_Tab.Content = "<Tab";
                key_Tab.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Tab };
                key_q.Content = "Q";
                key_q.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Q };
                key_w.Content = "W";
                key_w.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.W };
                key_e.Content = "E";
                key_e.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.E };
                key_r.Content = "R";
                key_r.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.R };
                key_t.Content = "T";
                key_t.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.T };
                key_y.Content = "Y";
                key_y.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Y };
                key_u.Content = "U";
                key_u.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.U };
                key_i.Content = "I";
                key_i.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.I };
                key_o.Content = "O";
                key_o.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.O };
                key_p.Content = "P";
                key_p.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.P };
                key_OpenBracket_Normal.Text = "{";
                key_OpenBracket_Caps.Text = "[";
                key_OpenBracket.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.CloseBracket };
                key_CloseBracket_Normal.Text = "}";
                key_CloseBracket_Caps.Text = "]";
                key_CloseBracket.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.OpenBracket };

                //Key Row 3
                key_CapsLock.Content = "CAPS";
                key_CapsLock.Tag = new KeysHidAction() { Key0 = KeysHid.CapsLock };
                key_a.Content = "A";
                key_a.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.A };
                key_s.Content = "S";
                key_s.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.S };
                key_d.Content = "D";
                key_d.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.D };
                key_f.Content = "F";
                key_f.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.F };
                key_g.Content = "G";
                key_g.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.G };
                key_h.Content = "H";
                key_h.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.H };
                key_j.Content = "J";
                key_j.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.J };
                key_k.Content = "K";
                key_k.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.K };
                key_l.Content = "L";
                key_l.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.L };
                key_Semicolon_Normal.Text = ":";
                key_Semicolon_Caps.Text = ";";
                key_Semicolon.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Semicolon };
                key_Quote_Normal.Text = "\"";
                key_Quote_Caps.Text = "'";
                key_Quote.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Quote };
                key_Up.Content = "⯅";
                key_Up.Tag = new KeysHidAction() { Key0 = KeysHid.ArrowUp };
                key_Pipe_Normal.Text = "|";
                key_Pipe_Caps.Text = "\\";
                key_Pipe.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.BackSlash };

                //Key Row 4
                key_Shift.Content = "Cut";
                key_Shift.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.CtrlLeft, Key0 = KeysHid.X };
                key_z.Content = "Z";
                key_z.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Z };
                key_x.Content = "X";
                key_x.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.X };
                key_c.Content = "C";
                key_c.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.C };
                key_v.Content = "V";
                key_v.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.V };
                key_b.Content = "B";
                key_b.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.B };
                key_n.Content = "N";
                key_n.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.N };
                key_m.Content = "M";
                key_m.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.M };
                key_Comma_Normal.Text = "<";
                key_Comma_Caps.Text = ",";
                key_Comma.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Comma };
                key_Period_Normal.Text = ">";
                key_Period_Caps.Text = ".";
                key_Period.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Period };
                key_Question_Normal.Text = "?";
                key_Question_Caps.Text = "/";
                key_Question.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.ShiftLeft, Key0 = KeysHid.Question };
                key_Left.Content = "⯇";
                key_Left.Tag = new KeysHidAction() { Key0 = KeysHid.ArrowLeft };
                key_Down.Content = "⯆";
                key_Down.Tag = new KeysHidAction() { Key0 = KeysHid.ArrowDown };
                key_Right.Content = "⯈";
                key_Right.Tag = new KeysHidAction() { Key0 = KeysHid.ArrowRight };

                //Key Row 5
                key_Control.Content = "Copy";
                key_Control.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.CtrlLeft, Key0 = KeysHid.C };
                key_Alt.Content = "Paste";
                key_Alt.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.CtrlLeft, Key0 = KeysHid.V };
                key_LeftWindows.Content = "Select all";
                key_LeftWindows.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.CtrlLeft, Key0 = KeysHid.A };
                key_Space.Content = "Space";
                key_Space.Tag = new KeysHidAction() { Key0 = KeysHid.Space };
                key_Enter.Content = "Undo";
                key_Enter.Tag = new KeysHidAction() { Modifiers = KeysModifierHid.CtrlLeft, Key0 = KeysHid.Z };
                key_Escape.Content = "Menu";
                key_Escape.Tag = new KeysHidAction() { Key0 = KeysHid.ContextMenu };
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
                    key_a.Tag = new KeysHidAction() { Key0 = KeysHid.A };
                    key_q.Content = "q";
                    key_q.Tag = new KeysHidAction() { Key0 = KeysHid.Q };
                    key_w.Content = "w";
                    key_w.Tag = new KeysHidAction() { Key0 = KeysHid.W };
                    key_y.Content = "y";
                    key_y.Tag = new KeysHidAction() { Key0 = KeysHid.Y };
                    key_z.Content = "z";
                    key_z.Tag = new KeysHidAction() { Key0 = KeysHid.Z };
                }
                else if (SettingLoad(vConfigurationDirectXInput, "KeyboardLayout", typeof(int)) == 1) //QWERTZ
                {
                    Debug.WriteLine("Switching keyboard layout: QWERTZ");
                    key_a.Content = "a";
                    key_a.Tag = new KeysHidAction() { Key0 = KeysHid.A };
                    key_q.Content = "q";
                    key_q.Tag = new KeysHidAction() { Key0 = KeysHid.Q };
                    key_w.Content = "w";
                    key_w.Tag = new KeysHidAction() { Key0 = KeysHid.W };
                    key_y.Content = "z";
                    key_y.Tag = new KeysHidAction() { Key0 = KeysHid.Z };
                    key_z.Content = "y";
                    key_z.Tag = new KeysHidAction() { Key0 = KeysHid.Y };
                }
                else if (SettingLoad(vConfigurationDirectXInput, "KeyboardLayout", typeof(int)) == 2) //AZERTY
                {
                    Debug.WriteLine("Switching keyboard layout: AZERTY");
                    key_a.Content = "q";
                    key_a.Tag = new KeysHidAction() { Key0 = KeysHid.Q };
                    key_q.Content = "a";
                    key_q.Tag = new KeysHidAction() { Key0 = KeysHid.A };
                    key_w.Content = "z";
                    key_w.Tag = new KeysHidAction() { Key0 = KeysHid.Z };
                    key_y.Content = "y";
                    key_y.Tag = new KeysHidAction() { Key0 = KeysHid.Y };
                    key_z.Content = "w";
                    key_z.Tag = new KeysHidAction() { Key0 = KeysHid.W };
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