using ArnoldVinkCode;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVFocus;
using static ArnoldVinkCode.AVInputOutputClass;
using static DirectXInput.AppVariables;
using static DirectXInput.XboxGameDVR;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

namespace DirectXInput.KeyboardCode
{
    partial class WindowKeyboard
    {
        //Show or hide the ShortcutList menu
        async Task ShowHideShortcutListPopup()
        {
            try
            {
                if (!CheckTextPopupsOpen())
                {
                    await ShowShortcutPopup();
                }
                else
                {
                    await HideTextPopups();
                }
            }
            catch { }
        }

        //Show the shortcut popup
        async Task ShowShortcutPopup()
        {
            try
            {
                //Play window open sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false, false);

                //Store keyboard focus button
                AVFocusDetailsSave(vFocusedButtonKeyboard, null);

                //Show the ShortcutList menu
                border_ShortcutListPopup.Visibility = Visibility.Visible;
                grid_Keyboard_Keys.IsEnabled = false;

                //Focus on popup button
                if (vFocusedButtonShortcut.FocusListBox == null)
                {
                    AVFocusDetails focusListbox = new AVFocusDetails();
                    focusListbox.FocusListBox = listbox_ShortcutList;
                    focusListbox.FocusIndex = vLastPopupListShortcutIndex;
                    await AVFocusDetailsFocus(focusListbox, vInteropWindowHandle);
                }
                else
                {
                    await AVFocusDetailsFocus(vFocusedButtonShortcut, vInteropWindowHandle);
                }
            }
            catch { }
        }

        //Hide the shortcut popup
        async Task HideShortcutPopup()
        {
            try
            {
                //Play window close sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false, false);

                //Store open focus button
                AVFocusDetailsSave(vFocusedButtonShortcut, null);

                //Hide the ShortcutList menu
                border_ShortcutListPopup.Visibility = Visibility.Collapsed;
                grid_Keyboard_Keys.IsEnabled = true;
                vLastPopupListType = "Shortcut";
                vLastPopupListShortcutIndex = listbox_ShortcutList.SelectedIndex;

                //Focus on keyboard button
                if (vFocusedButtonKeyboard.FocusElement == null)
                {
                    await FocusElement(key_ShortcutList, vInteropWindowHandle);
                }
                else
                {
                    await AVFocusDetailsFocus(vFocusedButtonKeyboard, vInteropWindowHandle);
                }
            }
            catch { }
        }

        //Handle ShortcutList close
        async void ButtonCloseShortcutList_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    await ShowHideShortcutListPopup();
                }
            }
            catch { }
        }
        async void ButtonCloseShortcutList_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                await ShowHideShortcutListPopup();
            }
            catch { }
        }

        //Handle ShortcutList click
        void Listbox_ShortcutList_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Check if an actual ListBoxItem is clicked
                if (!AVFunctions.ListBoxItemClickCheck((DependencyObject)e.OriginalSource)) { return; }

                ShortcutPressKeyboard(sender);
            }
            catch { }
        }

        void Listbox_ShortcutList_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    ShortcutPressKeyboard(sender);
                }
            }
            catch { }
        }

        //Press shortcut on keyboard
        void ShortcutPressKeyboard(object sender)
        {
            try
            {
                ListBox ListboxSender = (ListBox)sender;
                if (ListboxSender.SelectedItems.Count > 0 && ListboxSender.SelectedIndex != -1)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                    ProfileShared selectedItem = (ProfileShared)ListboxSender.SelectedItem;

                    //Show Xbox Game Bar
                    if (selectedItem.Object1.ToString() == "ShowXboxGameBar")
                    {
                        ShowXboxGameBar();
                    }
                    else
                    {
                        vFakerInputDevice.KeyboardPressRelease((KeysHidAction)selectedItem.Object1);
                    }
                }
            }
            catch { }
        }

        //Add shortcuts to the list
        void ShortcutList_AddItems()
        {
            try
            {
                if (!vDirectKeyboardShortcutList.Any())
                {
                    ProfileShared shortcutDesktop = new ProfileShared();
                    shortcutDesktop.String1 = "Toggle Desktop (Windows+D)";
                    shortcutDesktop.Object1 = new KeysHidAction { Modifiers = KeysModifierHid.WindowsLeft, Key0 = KeysHid.D };
                    vDirectKeyboardShortcutList.Add(shortcutDesktop);

                    ProfileShared shortcutMaximize = new ProfileShared();
                    shortcutMaximize.String1 = "Maximize Window (Alt+Enter)";
                    shortcutMaximize.Object1 = new KeysHidAction { Modifiers = KeysModifierHid.AltLeft, Key0 = KeysHid.Enter };
                    vDirectKeyboardShortcutList.Add(shortcutMaximize);

                    ProfileShared shortcutSwitch = new ProfileShared();
                    shortcutSwitch.String1 = "Switch Application (Alt+Tab)";
                    shortcutSwitch.Object1 = new KeysHidAction { Modifiers = KeysModifierHid.AltLeft, Key0 = KeysHid.Tab };
                    vDirectKeyboardShortcutList.Add(shortcutSwitch);

                    ProfileShared shortcutClose = new ProfileShared();
                    shortcutClose.String1 = "Close Application (Alt+F4)";
                    shortcutClose.Object1 = new KeysHidAction { Modifiers = KeysModifierHid.AltLeft, Key0 = KeysHid.F4 };
                    vDirectKeyboardShortcutList.Add(shortcutClose);

                    ProfileShared shortcutTaskMan = new ProfileShared();
                    shortcutTaskMan.String1 = "Task Manager (Ctrl+Shift+Esc)";
                    shortcutTaskMan.Object1 = new KeysHidAction { Modifiers = KeysModifierHid.CtrlLeft | KeysModifierHid.ShiftLeft, Key0 = KeysHid.Escape };
                    vDirectKeyboardShortcutList.Add(shortcutTaskMan);

                    ProfileShared shortcutXboxGameBar = new ProfileShared();
                    shortcutXboxGameBar.String1 = "Show Xbox Game Bar";
                    shortcutXboxGameBar.Object1 = "ShowXboxGameBar";
                    vDirectKeyboardShortcutList.Add(shortcutXboxGameBar);

                    ProfileShared shortcutPrintScreen = new ProfileShared();
                    shortcutPrintScreen.String1 = "Print Screen";
                    shortcutPrintScreen.Object1 = new KeysHidAction { Key0 = KeysHid.PrintScreen };
                    vDirectKeyboardShortcutList.Add(shortcutPrintScreen);

                    ProfileShared shortcutNumericLock = new ProfileShared();
                    shortcutNumericLock.String1 = "Numeric Lock";
                    shortcutNumericLock.Object1 = new KeysHidAction { Key0 = KeysHid.NumpadLock };
                    vDirectKeyboardShortcutList.Add(shortcutNumericLock);

                    ProfileShared shortcutScrollLock = new ProfileShared();
                    shortcutScrollLock.String1 = "Scroll Lock";
                    shortcutScrollLock.Object1 = new KeysHidAction { Key0 = KeysHid.ScrollLock };
                    vDirectKeyboardShortcutList.Add(shortcutScrollLock);

                    ProfileShared shortcutCapsLock = new ProfileShared();
                    shortcutCapsLock.String1 = "Caps Lock";
                    shortcutCapsLock.Object1 = new KeysHidAction { Key0 = KeysHid.CapsLock };
                    vDirectKeyboardShortcutList.Add(shortcutCapsLock);

                    ProfileShared shortcutPauseBreak = new ProfileShared();
                    shortcutPauseBreak.String1 = "Pause Break";
                    shortcutPauseBreak.Object1 = new KeysHidAction { Key0 = KeysHid.Pause };
                    vDirectKeyboardShortcutList.Add(shortcutPauseBreak);
                }
            }
            catch { }
        }
    }
}