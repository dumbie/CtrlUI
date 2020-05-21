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
                int usedVirtualKey = windowMessage.wParam.ToInt32();
                bool pressedShiftKey = vKeyboardPreviousVirtualKey == (int)KeysVirtual.Shift;

                if (usedVirtualKey == (int)KeysVirtual.Tab && pressedShiftKey)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Move", false);
                }
                else if (usedVirtualKey == (int)KeysVirtual.Tab)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Move", false);
                }
                else if (usedVirtualKey == (int)KeysVirtual.F13)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Click", false);
                }
                else if (usedVirtualKey == (int)KeysVirtual.Home)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Click", false);
                }
                else if (usedVirtualKey == (int)KeysVirtual.Prior)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Click", false);
                }
                else if (usedVirtualKey == (int)KeysVirtual.End)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Click", false);
                }
                else if (usedVirtualKey == (int)KeysVirtual.Next)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Click", false);
                }
                else if (usedVirtualKey == (int)KeysVirtual.Left)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Move", false);
                }
                else if (usedVirtualKey == (int)KeysVirtual.Up)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Move", false);
                    NavigateUp(ref messageHandled);
                }
                else if (usedVirtualKey == (int)KeysVirtual.Right)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Move", false);
                }
                else if (usedVirtualKey == (int)KeysVirtual.Down)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Move", false);
                    NavigateDown(ref messageHandled);
                }
                else if (usedVirtualKey == (int)KeysVirtual.Space)
                {
                    PlayInterfaceSound(vConfigurationApplication, "Confirm", false);
                }

                vKeyboardPreviousVirtualKey = usedVirtualKey;
            }
            catch { }
        }

        //Handle keyboard up
        void HandleKeyboardUp(MSG WindowMessage, ref bool Handled)
        {
            try
            {
                //Check the pressed keys
                int UsedVirtualKey = WindowMessage.wParam.ToInt32();

                if (UsedVirtualKey == (int)KeysVirtual.Up) { Handled = true; }
                else if (UsedVirtualKey == (int)KeysVirtual.Down) { Handled = true; }
            }
            catch { }
        }

        //Navigate arrow down
        void NavigateDown(ref bool Handled)
        {
            try
            {
                FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;
                if (frameworkElement.GetType() == typeof(ListBoxItem))
                {
                    ListBox parentListbox = AVFunctions.FindVisualParent<ListBox>(frameworkElement);
                    if (vTabTargetLists.Any(x => x == parentListbox.Name))
                    {
                        EventKeyboardPressSingle((byte)KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                        Handled = true;
                        return;
                    }
                }
                else if (frameworkElement.GetType() == typeof(Button))
                {
                    if (vTabTargetButtons.Any(x => x == frameworkElement.Name))
                    {
                        EventKeyboardPressSingle((byte)KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                        Handled = true;
                        return;
                    }
                }
                else if (frameworkElement.GetType() == typeof(TextBox) || frameworkElement.GetType() == typeof(Slider))
                {
                    EventKeyboardPressSingle((byte)KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
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
                if (frameworkElement.GetType() == typeof(ListBoxItem))
                {
                    ListBox parentListbox = AVFunctions.FindVisualParent<ListBox>(frameworkElement);
                    if (vTabTargetLists.Any(x => x == parentListbox.Name))
                    {
                        EventKeyboardPressCombo((byte)KeysVirtual.Shift, (byte)KeysVirtual.Tab, false);
                        Handled = true;
                        return;
                    }
                }
                else if (frameworkElement.GetType() == typeof(Button))
                {
                    if (vTabTargetButtons.Any(x => x == frameworkElement.Name))
                    {
                        EventKeyboardPressCombo((byte)KeysVirtual.Shift, (byte)KeysVirtual.Tab, false);
                        Handled = true;
                        return;
                    }
                }
                else if (frameworkElement.GetType() == typeof(TextBox) || frameworkElement.GetType() == typeof(Slider))
                {
                    EventKeyboardPressCombo((byte)KeysVirtual.Shift, (byte)KeysVirtual.Tab, false);
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