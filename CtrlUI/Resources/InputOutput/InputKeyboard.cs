using ArnoldVinkCode;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.OutputKeyboard;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle keyboard down
        void HandleKeyboardDown(ref MSG WindowMessage, ref bool Handled)
        {
            try
            {
                //Check the pressed keys
                int UsedVirtualKey = WindowMessage.wParam.ToInt32();

                if (UsedVirtualKey == (int)KeysVirtual.Left)
                {
                    PlayInterfaceSound("Click", false);
                }
                else if (UsedVirtualKey == (int)KeysVirtual.Up)
                {
                    PlayInterfaceSound("Click", false);
                    NavigateUp(ref Handled);
                }
                else if (UsedVirtualKey == (int)KeysVirtual.Right)
                {
                    PlayInterfaceSound("Click", false);
                }
                else if (UsedVirtualKey == (int)KeysVirtual.Down)
                {
                    PlayInterfaceSound("Click", false);
                    NavigateDown(ref Handled);
                }
                else if (UsedVirtualKey == (int)KeysVirtual.Space)
                {
                    PlayInterfaceSound("Confirm", false);
                }
            }
            catch { }
        }

        //Handle keyboard up
        void HandleKeyboardUp(ref MSG WindowMessage, ref bool Handled)
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

        //Navigate list down
        void NavigateDown(ref bool Handled)
        {
            try
            {
                FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;
                if (frameworkElement.GetType() == typeof(ListBoxItem))
                {
                    ListBox parentListbox = AVFunctions.FindVisualParent<ListBox>(frameworkElement);
                    if (vListTabTarget.Any(x => x == parentListbox.Name))
                    {
                        KeySendSingle((byte)KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                        Handled = true;
                        return;
                    }
                }
                else if (frameworkElement.GetType() == typeof(TextBox) || frameworkElement.GetType() == typeof(Slider))
                {
                    KeySendSingle((byte)KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                    Handled = true;
                    return;
                }
            }
            catch { }
        }

        //Navigate list down
        void NavigateUp(ref bool Handled)
        {
            try
            {
                FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;
                if (frameworkElement.GetType() == typeof(ListBoxItem))
                {
                    ListBox parentListbox = AVFunctions.FindVisualParent<ListBox>(frameworkElement);
                    if (vListTabTarget.Any(x => x == parentListbox.Name))
                    {
                        //Improve: KeySendCombo((byte)KeysVirtual.Shift, (byte)KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                        KeyPressCombo((byte)KeysVirtual.Shift, (byte)KeysVirtual.Tab, false);
                        Handled = true;
                        return;
                    }
                }
                else if (frameworkElement.GetType() == typeof(TextBox) || frameworkElement.GetType() == typeof(Slider))
                {
                    //Improve: KeySendCombo((byte)KeysVirtual.Shift, (byte)KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                    KeyPressCombo((byte)KeysVirtual.Shift, (byte)KeysVirtual.Tab, false);
                    Handled = true;
                    return;
                }
            }
            catch { }
        }

        //Handle main menu keyboard/controller tapped
        async void ListBox_Menu_KeyPressUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space) { await listbox_Menu_SingleTap(); }
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
                else if (e.Key == Key.Insert) { await Popup_Show_AppAdd(); }
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
                    if (e.SystemKey == Key.Enter) { AppSwitchScreenMode(false, false); }
                }
                //Handle Ctrl + Key press
                else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                {
                }
                else
                {
                    //Debug.WriteLine("Key pressed: " + e.Key);
                    if (e.Key == Key.Escape) { await Popup_Close_Top(); }
                    else if (e.Key == Key.F1) { await Popup_Show(grid_Popup_Help, grid_Popup_Help_button_Close, true); }
                    else if (e.Key == Key.F2) { await QuickActionPrompt(); }
                    else if (e.Key == Key.F3) { await Popup_ShowHide_Search(false); }
                    else if (e.Key == Key.F4) { SortAppLists(false, false); }
                    else if (e.Key == Key.F5) { await RefreshApplicationLists(false, false, true, true); }
                    else if (e.Key == Key.F6) { await Popup_ShowHide_MainMenu(false); }
                }
            }
            catch { }
        }
    }
}