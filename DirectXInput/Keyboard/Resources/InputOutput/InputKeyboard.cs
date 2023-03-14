using ArnoldVinkCode;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using static ArnoldVinkCode.AVInputOutputClass;
using static DirectXInput.AppVariables;
using static LibraryShared.FocusFunctions;

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
                if (usedVirtualKey == KeysVirtual.Up)
                {
                    NavigateArrowUp(ref messageHandled);
                }
                else if (usedVirtualKey == KeysVirtual.Down)
                {
                    NavigateArrowDown(ref messageHandled);
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
        void NavigateArrowDown(ref bool Handled)
        {
            try
            {
                FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;
                if (frameworkElement != null && frameworkElement.GetType() == typeof(ListBoxItem))
                {
                    ListBox parentListbox = AVFunctions.FindVisualParent<ListBox>(frameworkElement);
                    if (vVerticalLoopTargetLists.Contains(parentListbox.Name))
                    {
                        int itemsCount = parentListbox.Items.Count;
                        if ((parentListbox.SelectedIndex + 1) == itemsCount)
                        {
                            ListboxFocusIndex(parentListbox, false, false, 0, vInteropWindowHandle);
                            Handled = true;
                            return;
                        }
                    }
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
                    if (vVerticalLoopTargetLists.Contains(parentListbox.Name))
                    {
                        if (parentListbox.SelectedIndex == 0)
                        {
                            int itemsCount = parentListbox.Items.Count;
                            ListboxFocusIndex(parentListbox, false, false, itemsCount - 1, vInteropWindowHandle);
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