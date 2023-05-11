using ArnoldVinkCode;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using static ArnoldVinkCode.AVFocus;
using static ArnoldVinkCode.AVInputOutputClass;
using static DirectXInput.AppVariables;

namespace DirectXInput.KeyboardCode
{
    partial class WindowKeyboard
    {
        //Handle keyboard down
        void HandleKeyboardDown(MSG windowMessage, ref bool messageHandled)
        {
            try
            {
                //Get the pressed keys
                KeysVirtual usedVirtualKey = (KeysVirtual)windowMessage.wParam;

                //Check the pressed key
                if (usedVirtualKey == KeysVirtual.ArrowUp)
                {
                    NavigateArrowUpLeft(ref messageHandled);
                }
                else if (usedVirtualKey == KeysVirtual.ArrowDown)
                {
                    NavigateArrowDownRight(ref messageHandled);
                }
                else if (usedVirtualKey == KeysVirtual.ArrowLeft)
                {
                    NavigateArrowUpLeft(ref messageHandled);
                }
                else if (usedVirtualKey == KeysVirtual.ArrowRight)
                {
                    NavigateArrowDownRight(ref messageHandled);
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

        //Navigate arrow down and right
        void NavigateArrowDownRight(ref bool Handled)
        {
            try
            {
                FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;
                if (frameworkElement != null && frameworkElement.GetType() == typeof(ListBoxItem))
                {
                    ListBox parentListbox = AVFunctions.FindVisualParent<ListBox>(frameworkElement);
                    if (vLoopTargetListsFirstLastItem.Contains(parentListbox.Name))
                    {
                        int itemsCount = parentListbox.Items.Count;
                        if ((parentListbox.SelectedIndex + 1) == itemsCount)
                        {
                            ListBoxFocusIndex(parentListbox, false, 0, vInteropWindowHandle).Start();
                            Handled = true;
                            return;
                        }
                    }
                }
            }
            catch { }
        }

        //Navigate arrow up and left
        void NavigateArrowUpLeft(ref bool Handled)
        {
            try
            {
                FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;
                if (frameworkElement != null && frameworkElement.GetType() == typeof(ListBoxItem))
                {
                    ListBox parentListbox = AVFunctions.FindVisualParent<ListBox>(frameworkElement);
                    if (vLoopTargetListsFirstLastItem.Contains(parentListbox.Name))
                    {
                        if (parentListbox.SelectedIndex == 0)
                        {
                            ListBoxFocusIndex(parentListbox, true, 0, vInteropWindowHandle).Start();
                            Handled = true;
                            return;
                        }
                    }
                }
            }
            catch { }
        }
    }
}