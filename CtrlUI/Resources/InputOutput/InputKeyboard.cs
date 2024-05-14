using ArnoldVinkCode;
using ArnoldVinkCode.Styles;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static CtrlUI.AppVariables;
using static LibraryShared.Enums;
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
                //Get the pressed keys
                KeysVirtual usedVirtualKey = (KeysVirtual)windowMessage.wParam;

                //Check pressed key modifier
                KeysVirtual? usedModifierKey = null;
                System.Windows.Forms.Keys keysData = (System.Windows.Forms.Keys)(int)usedVirtualKey | System.Windows.Forms.Control.ModifierKeys;
                if (keysData.HasFlag(System.Windows.Forms.Keys.Control)) { usedModifierKey = KeysVirtual.CtrlLeft; }
                else if (keysData.HasFlag(System.Windows.Forms.Keys.Alt)) { usedModifierKey = KeysVirtual.AltLeft; }
                else if (keysData.HasFlag(System.Windows.Forms.Keys.Shift)) { usedModifierKey = KeysVirtual.ShiftLeft; }

                //Check if a textbox is focused
                bool focusedTextBox = false;
                FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;
                if (frameworkElement != null && frameworkElement.GetType() == typeof(TextBox))
                {
                    focusedTextBox = true;
                }

                //Check the pressed key
                if (usedVirtualKey == KeysVirtual.Tab && usedModifierKey == KeysVirtual.ShiftLeft)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "Move", false, false);
                }
                else if (usedVirtualKey == KeysVirtual.Tab)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "Move", false, false);
                }
                else if (usedVirtualKey == KeysVirtual.F13)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                }
                else if (usedVirtualKey == KeysVirtual.Home)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                }
                else if (usedVirtualKey == KeysVirtual.PageUp)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                }
                else if (usedVirtualKey == KeysVirtual.End)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                }
                else if (usedVirtualKey == KeysVirtual.PageDown)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                }
                else if (usedVirtualKey == KeysVirtual.ArrowLeft)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "Move", false, false);
                }
                else if (usedVirtualKey == KeysVirtual.ArrowUp)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "Move", false, false);
                    NavigateArrowUp(ref messageHandled);
                }
                else if (usedVirtualKey == KeysVirtual.ArrowRight)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "Move", false, false);
                }
                else if (usedVirtualKey == KeysVirtual.ArrowDown)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "Move", false, false);
                    NavigateArrowDown(ref messageHandled);
                }
                else if (usedVirtualKey == KeysVirtual.Space)
                {
                    if (!focusedTextBox)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Confirm", false, false);
                    }
                }
                else if (usedVirtualKey == KeysVirtual.BackSpace)
                {
                    if (vFilePickerOpen && !focusedTextBox)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Confirm", false, false);
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

                if (usedVirtualKey == KeysVirtual.ArrowUp) { messageHandled = true; }
                else if (usedVirtualKey == KeysVirtual.ArrowDown) { messageHandled = true; }
            }
            catch { }
        }

        //Navigate arrow down
        void NavigateArrowDown(ref bool Handled)
        {
            try
            {
                FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;
                if (frameworkElement != null && frameworkElement.GetType() == typeof(ListBoxItem))
                {
                    ListBox parentListbox = AVFunctions.FindVisualParent<ListBox>(frameworkElement);
                    if (vTabTargetListsSingleColumn.Contains(parentListbox.Name))
                    {
                        KeySendSingle(KeysVirtual.Tab, vProcessCurrent.WindowHandleMain);
                        Handled = true;
                        return;
                    }
                    else if (vTabTargetListsFirstLastItem.Contains(parentListbox.Name))
                    {
                        if ((parentListbox.SelectedIndex + 1) == parentListbox.Items.Count)
                        {
                            KeySendSingle(KeysVirtual.Tab, vProcessCurrent.WindowHandleMain);
                            Handled = true;
                            return;
                        }
                    }
                    else if (vTabTargetListsFirstLastColumn.Contains(parentListbox.Name))
                    {
                        if (ListBoxItemColumnPosition(parentListbox, (ListBoxItem)frameworkElement, false))
                        {
                            KeySendSingle(KeysVirtual.Tab, vProcessCurrent.WindowHandleMain);
                            Handled = true;
                            return;
                        }
                    }
                }
                else if (frameworkElement != null && frameworkElement.GetType() == typeof(Button) || (frameworkElement.GetType() == typeof(TextBox) || frameworkElement.GetType() == typeof(Slider) || frameworkElement.GetType() == typeof(SliderDelay)))
                {
                    KeySendSingle(KeysVirtual.Tab, vProcessCurrent.WindowHandleMain);
                    Handled = true;
                    return;
                }
            }
            catch { }
        }

        //Navigate arrow up
        void NavigateArrowUp(ref bool Handled)
        {
            try
            {
                FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;
                if (frameworkElement != null && frameworkElement.GetType() == typeof(ListBoxItem))
                {
                    ListBox parentListbox = AVFunctions.FindVisualParent<ListBox>(frameworkElement);
                    if (vTabTargetListsSingleColumn.Contains(parentListbox.Name))
                    {
                        KeyPressReleaseCombo(KeysVirtual.ShiftLeft, KeysVirtual.Tab);
                        Handled = true;
                        return;
                    }
                    else if (vTabTargetListsFirstLastItem.Contains(parentListbox.Name))
                    {
                        if (parentListbox.SelectedIndex == 0)
                        {
                            KeyPressReleaseCombo(KeysVirtual.ShiftLeft, KeysVirtual.Tab);
                            Handled = true;
                            return;
                        }
                    }
                    else if (vTabTargetListsFirstLastColumn.Contains(parentListbox.Name))
                    {
                        if (ListBoxItemColumnPosition(parentListbox, (ListBoxItem)frameworkElement, true))
                        {
                            KeyPressReleaseCombo(KeysVirtual.ShiftLeft, KeysVirtual.Tab);
                            Handled = true;
                            return;
                        }
                    }
                }
                else if (frameworkElement != null && frameworkElement.GetType() == typeof(Button) || (frameworkElement.GetType() == typeof(TextBox) || frameworkElement.GetType() == typeof(Slider) || frameworkElement.GetType() == typeof(SliderDelay)))
                {
                    KeyPressReleaseCombo(KeysVirtual.ShiftLeft, KeysVirtual.Tab);
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
                if (e.Key == Key.Space) { await ListBox_Apps_LeftClick(sender); }
                else if (e.Key == Key.Delete || e.Key == Key.Back) { await ListBox_Apps_RightClick(sender); }
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
                }
                //Handle Ctrl + Key press
                else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                {
                }
                else
                {
                    //Debug.WriteLine("Key pressed: " + e.Key);
                    if (e.Key == Key.Escape)
                    {
                        if (Popup_Open_Check(grid_Popup_Manage))
                        {
                            await Popup_Close_Top(true, true);
                        }
                        else
                        {
                            await Popup_Close_Top(false);
                        }
                    }
                    else if (e.Key == Key.F1) { await Popup_Show(grid_Popup_Help, grid_Popup_Help_button_Close); }
                    else if (e.Key == Key.F2)
                    {
                        if (!vFilePickerOpen)
                        {
                            await QuickLaunchPrompt();
                        }
                    }
                    else if (e.Key == Key.F3) { await CategoryListChange(ListCategory.Search); }
                    else if (e.Key == Key.F4) { await SortListsAuto(); }
                    else if (e.Key == Key.F6) { await Popup_ShowHide_MainMenu(false); }
                    else if (e.Key == Key.F7) { await ShowFileManager(); }
                }
            }
            catch { }
        }
    }
}