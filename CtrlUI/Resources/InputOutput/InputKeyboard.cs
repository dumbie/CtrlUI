using ArnoldVinkCode;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using static ArnoldVinkCode.AVInputOutputClass;
using static CtrlUI.AppVariables;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle keyboard down
        void HandleKeyboardDown(MSG windowMessage, ref bool messageHandled)
        {
            try
            {
                //Check the pressed keys
                KeysVirtual usedVirtualKey = (KeysVirtual)windowMessage.wParam;

                //Check pressed key modifier
                KeysVirtual? usedModifierKey = null;
                System.Windows.Forms.Keys keysData = (System.Windows.Forms.Keys)(int)usedVirtualKey | System.Windows.Forms.Control.ModifierKeys;
                if (keysData.HasFlag(System.Windows.Forms.Keys.Control)) { usedModifierKey = KeysVirtual.Control; }
                else if (keysData.HasFlag(System.Windows.Forms.Keys.Alt)) { usedModifierKey = KeysVirtual.Alt; }
                else if (keysData.HasFlag(System.Windows.Forms.Keys.Shift)) { usedModifierKey = KeysVirtual.Shift; }

                //Check if a textbox is focused
                bool focusedTextBox = false;
                FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;
                if (frameworkElement != null && frameworkElement.GetType() == typeof(TextBox))
                {
                    focusedTextBox = true;
                }

                if (usedVirtualKey == KeysVirtual.Tab && usedModifierKey == KeysVirtual.Shift)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Move", false);
                }
                else if (usedVirtualKey == KeysVirtual.Tab)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Move", false);
                }
                else if (usedVirtualKey == KeysVirtual.F13)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Click", false);
                }
                else if (usedVirtualKey == KeysVirtual.Home)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Click", false);
                }
                else if (usedVirtualKey == KeysVirtual.Prior)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Click", false);
                }
                else if (usedVirtualKey == KeysVirtual.End)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Click", false);
                }
                else if (usedVirtualKey == KeysVirtual.Next)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Click", false);
                }
                else if (usedVirtualKey == KeysVirtual.Left)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Move", false);
                }
                else if (usedVirtualKey == KeysVirtual.Up)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Move", false);
                    NavigateUp(ref messageHandled);
                }
                else if (usedVirtualKey == KeysVirtual.Right)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Move", false);
                }
                else if (usedVirtualKey == KeysVirtual.Down)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Move", false);
                    NavigateDown(ref messageHandled);
                }
                else if (usedVirtualKey == KeysVirtual.Space)
                {
                    if (!focusedTextBox)
                    {
                        PlayInterfaceSound(vConfigurationApplication, "Confirm", false);
                    }
                }
                else if (usedVirtualKey == KeysVirtual.BackSpace)
                {
                    if (vFilePickerOpen && !focusedTextBox)
                    {
                        PlayInterfaceSound(vConfigurationApplication, "Confirm", false);
                    }
                }
            }
            catch { }
        }

        //Handle keyboard up
        void HandleKeyboardUp(MSG windowMessage, ref bool messageHandled)
        {
            try
            {
                //Check the pressed keys
                KeysVirtual usedVirtualKey = (KeysVirtual)windowMessage.wParam;

                if (usedVirtualKey == KeysVirtual.Up) { messageHandled = true; }
                else if (usedVirtualKey == KeysVirtual.Down) { messageHandled = true; }
            }
            catch { }
        }

        //Navigate arrow down
        void NavigateDown(ref bool Handled)
        {
            try
            {
                FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;
                if (frameworkElement != null && frameworkElement.GetType() == typeof(ListBoxItem))
                {
                    ListBox parentListbox = AVFunctions.FindVisualParent<ListBox>(frameworkElement);
                    if (vTabTargetLists.Contains(parentListbox.Name))
                    {
                        EventKeyboardPressSingle(KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                        Handled = true;
                        return;
                    }
                }
                else if (frameworkElement != null && frameworkElement.GetType() == typeof(Button))
                {
                    if (vTabTargetButtons.Any(x => x == frameworkElement.Name))
                    {
                        EventKeyboardPressSingle(KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                        Handled = true;
                        return;
                    }
                }
                else if (frameworkElement != null && (frameworkElement.GetType() == typeof(TextBox) || frameworkElement.GetType() == typeof(Slider)))
                {
                    EventKeyboardPressSingle(KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                    Handled = true;
                    return;
                }
            }
            catch { }
        }

        //Navigate arrow up
        void NavigateUp(ref bool Handled)
        {
            try
            {
                FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;
                if (frameworkElement != null && frameworkElement.GetType() == typeof(ListBoxItem))
                {
                    ListBox parentListbox = AVFunctions.FindVisualParent<ListBox>(frameworkElement);
                    if (vTabTargetLists.Contains(parentListbox.Name))
                    {
                        EventKeyboardPressCombo(KeysVirtual.Shift, KeysVirtual.Tab);
                        Handled = true;
                        return;
                    }
                }
                else if (frameworkElement != null && frameworkElement.GetType() == typeof(Button))
                {
                    if (vTabTargetButtons.Any(x => x == frameworkElement.Name))
                    {
                        EventKeyboardPressCombo(KeysVirtual.Shift, KeysVirtual.Tab);
                        Handled = true;
                        return;
                    }
                }
                else if (frameworkElement != null && (frameworkElement.GetType() == typeof(TextBox) || frameworkElement.GetType() == typeof(Slider)))
                {
                    EventKeyboardPressCombo(KeysVirtual.Shift, KeysVirtual.Tab);
                    Handled = true;
                    return;
                }
            }
            catch { }
        }

        //Handle app list keyboard/controller tapped
        async void ListBox_Apps_KeyPressUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space) { await lb_AppList_LeftClick(sender); }
                else if (e.Key == Key.Delete || e.Key == Key.Back) { await lb_AppList_RightClick(sender); }
                else if (e.Key == Key.Insert) { await Popup_Show_AddExe(); }
            }
            catch { }
        }

        //Handle application keyboard presses
        async void WindowMain_KeyPressUp(object sender, KeyEventArgs e)
        {
            try
            {
                //Handle Alt + Key press
                if (e.KeyboardDevice.Modifiers == ModifierKeys.Alt)
                {
                    if (e.SystemKey == Key.Enter)
                    {
                        await AppSwitchScreenMode(false, false);
                    }
                }
                //Handle Ctrl + Key press
                else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                {
                }
                else
                {
                    //Debug.WriteLine("Key pressed: " + e.Key);
                    if (e.Key == Key.Escape) { await Popup_Close_Top(); }
                    else if (e.Key == Key.F1) { await Popup_Show(grid_Popup_Help, grid_Popup_Help_button_Close); }
                    else if (e.Key == Key.F2) { await QuickActionPrompt(); }
                    else if (e.Key == Key.F3) { await Popup_ShowHide_Search(false); }
                    else if (e.Key == Key.F4) { await SortAppLists(false, false); }
                    else if (e.Key == Key.F6) { await Popup_ShowHide_MainMenu(false); }
                }
            }
            catch { }
        }
    }
}