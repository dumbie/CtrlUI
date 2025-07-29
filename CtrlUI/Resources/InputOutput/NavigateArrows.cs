using ArnoldVinkStyles;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkStyles.AVInterface;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Navigate arrow left
        private void NavigateArrowLeft()
        {
            try
            {
                FocusManager.TryMoveFocus(FocusNavigationDirection.Left);
            }
            catch { }
        }

        //Navigate arrow right
        private void NavigateArrowRight()
        {
            try
            {
                FocusManager.TryMoveFocus(FocusNavigationDirection.Right);
            }
            catch { }
        }

        //Navigate arrow down
        private void NavigateArrowDown()
        {
            try
            {
                FrameworkElement frameworkElement = GetFocusedFrameworkElement();
                if (frameworkElement != null)
                {
                    if (frameworkElement.GetType() == typeof(ListViewItem))
                    {
                        ListView parentListbox = AVVisualTree.FindVisualParent<ListView>(frameworkElement);
                        if (vTabTargetListsSingleColumn.Contains(parentListbox.Name))
                        {
                            KeyPressReleaseSingle(KeysVirtual.Tab);
                            return;
                        }
                        else if (vTabTargetListsFirstLastItem.Contains(parentListbox.Name))
                        {
                            if ((parentListbox.SelectedIndex + 1) == parentListbox.Items.Count)
                            {
                                KeyPressReleaseSingle(KeysVirtual.Tab);
                                return;
                            }
                        }
                        else if (vTabTargetListsFirstLastColumn.Contains(parentListbox.Name))
                        {
                            if (ListViewItemColumnPosition(parentListbox, (ListViewItem)frameworkElement, false))
                            {
                                KeyPressReleaseSingle(KeysVirtual.Tab);
                                return;
                            }
                        }
                    }
                    else if (frameworkElement.GetType() == typeof(Button) || (frameworkElement.GetType() == typeof(TextBox) || frameworkElement.GetType() == typeof(Slider) || frameworkElement.GetType() == typeof(SliderDelay)))
                    {
                        KeyPressReleaseSingle(KeysVirtual.Tab);
                        return;
                    }
                }

                FocusManager.TryMoveFocus(FocusNavigationDirection.Down);
            }
            catch { }
        }

        //Navigate arrow up
        private void NavigateArrowUp()
        {
            try
            {
                FrameworkElement frameworkElement = GetFocusedFrameworkElement();
                if (frameworkElement != null)
                {
                    if (frameworkElement.GetType() == typeof(ListViewItem))
                    {
                        ListView parentListbox = AVVisualTree.FindVisualParent<ListView>(frameworkElement);
                        if (vTabTargetListsSingleColumn.Contains(parentListbox.Name))
                        {
                            KeyPressReleaseCombo(KeysVirtual.ShiftLeft, KeysVirtual.Tab);
                            return;
                        }
                        else if (vTabTargetListsFirstLastItem.Contains(parentListbox.Name))
                        {
                            if (parentListbox.SelectedIndex == 0)
                            {
                                KeyPressReleaseCombo(KeysVirtual.ShiftLeft, KeysVirtual.Tab);
                                return;
                            }
                        }
                        else if (vTabTargetListsFirstLastColumn.Contains(parentListbox.Name))
                        {
                            if (ListViewItemColumnPosition(parentListbox, (ListViewItem)frameworkElement, true))
                            {
                                KeyPressReleaseCombo(KeysVirtual.ShiftLeft, KeysVirtual.Tab);
                                return;
                            }
                        }
                    }
                    else if (frameworkElement.GetType() == typeof(Button) || (frameworkElement.GetType() == typeof(TextBox) || frameworkElement.GetType() == typeof(Slider) || frameworkElement.GetType() == typeof(SliderDelay)))
                    {
                        KeyPressReleaseCombo(KeysVirtual.ShiftLeft, KeysVirtual.Tab);
                        return;
                    }
                }

                FocusManager.TryMoveFocus(FocusNavigationDirection.Up);
            }
            catch { }
        }
    }
}