using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;

namespace LibraryShared
{
    public partial class FocusFunctions
    {
        //Listbox focus or select an index
        public static async Task ListBoxFocusOrSelectIndex(ListBox focusListBox, bool firstIndex, bool lastIndex, int indexNumber, IntPtr windowHandle)
        {
            try
            {
                await AVActions.DispatcherInvoke(async delegate
                {
                    //Get the currently focused element
                    FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;

                    //Check if focused element is disconnected
                    bool disconnectedSource = frameworkElement == null || frameworkElement.DataContext == BindingOperations.DisconnectedSource;

                    //Focus on the listbox or select index
                    if (disconnectedSource || frameworkElement == focusListBox)
                    {
                        await ListboxFocusIndex(focusListBox, firstIndex, lastIndex, indexNumber, windowHandle);
                    }
                    else
                    {
                        ListBoxSelectIndex(focusListBox, firstIndex, lastIndex, indexNumber);
                    }
                });
            }
            catch { }
        }

        //Focus on listbox index
        public static async Task ListboxFocusIndex(ListBox focusListBox, bool firstIndex, bool lastIndex, int indexNumber, IntPtr windowHandle)
        {
            try
            {
                await AVActions.DispatcherInvoke(async delegate
                {
                    //Select first available listbox
                    if (focusListBox != null && focusListBox.IsEnabled && focusListBox.Visibility == Visibility.Visible && focusListBox.Items.Count > 0)
                    {
                        //Update the listbox layout
                        focusListBox.UpdateLayout();

                        //Select a listbox item index
                        ListBoxSelectIndex(focusListBox, firstIndex, lastIndex, indexNumber);

                        //Focus on the listbox and item
                        int selectedIndex = focusListBox.SelectedIndex;

                        //Scroll to the listbox item
                        object scrollListBoxItem = focusListBox.Items[selectedIndex];
                        focusListBox.ScrollIntoView(scrollListBoxItem);

                        //Force focus on element
                        ListBoxItem focusListBoxItem = (ListBoxItem)focusListBox.ItemContainerGenerator.ContainerFromInd‌​ex(selectedIndex);
                        await FrameworkElementFocus(focusListBoxItem, false, windowHandle);

                        Debug.WriteLine("Focusing on listbox index: " + selectedIndex);
                    }
                    else
                    {
                        Debug.WriteLine("Listbox cannot be focused on, pressing tab key.");
                        KeySendSingle(KeysVirtual.Tab, windowHandle);
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed focusing on the listbox, pressing tab key: " + ex.Message);
                KeySendSingle(KeysVirtual.Tab, windowHandle);
            }
        }

        //Select a listbox index
        public static void ListBoxSelectIndex(ListBox focusListBox, bool firstIndex, bool lastIndex, int indexNumber)
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    //Update the listbox layout
                    focusListBox.UpdateLayout();

                    //Select the requested index
                    if (firstIndex)
                    {
                        focusListBox.SelectedIndex = 0;
                        Debug.WriteLine("Selecting first listbox index.");
                    }
                    else if (lastIndex)
                    {
                        focusListBox.SelectedIndex = focusListBox.Items.Count - 1;
                        Debug.WriteLine("Selecting last listbox index.");
                    }
                    else if (indexNumber != -1)
                    {
                        if (indexNumber >= focusListBox.Items.Count)
                        {
                            focusListBox.SelectedIndex = focusListBox.Items.Count - 1;
                            Debug.WriteLine("Selecting last listbox index.");
                        }
                        else
                        {
                            focusListBox.SelectedIndex = indexNumber;
                            Debug.WriteLine("Selecting listbox index: " + indexNumber);
                        }
                    }

                    //Check the selected index
                    if (focusListBox.SelectedIndex == -1)
                    {
                        focusListBox.SelectedIndex = 0;
                        Debug.WriteLine("No selection, selecting first listbox index.");
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed selecting the listbox index: " + ex.Message);
            }
        }

        //Listbox focus or select an item
        public static async Task ListBoxFocusOrSelectItem(ListBox focusListBox, object selectItem, IntPtr windowHandle)
        {
            try
            {
                await AVActions.DispatcherInvoke(async delegate
                {
                    //Get the currently focused element
                    FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;

                    //Check if focused element is disconnected
                    bool disconnectedSource = frameworkElement == null || frameworkElement.DataContext == BindingOperations.DisconnectedSource;

                    //Focus on the listbox or select index
                    if (disconnectedSource || frameworkElement == focusListBox)
                    {
                        await ListBoxFocusItem(focusListBox, selectItem, windowHandle);
                    }
                    else
                    {
                        ListBoxSelectItem(focusListBox, selectItem);
                    }
                });
            }
            catch { }
        }

        //Focus on listbox item
        public static async Task ListBoxFocusItem(ListBox focusListBox, object selectItem, IntPtr windowHandle)
        {
            try
            {
                await AVActions.DispatcherInvoke(async delegate
                {
                    //Select first available listbox
                    if (focusListBox != null && focusListBox.IsEnabled && focusListBox.Visibility == Visibility.Visible && focusListBox.Items.Count > 0)
                    {
                        //Update the listbox layout
                        focusListBox.UpdateLayout();

                        //Select a listbox item
                        ListBoxSelectItem(focusListBox, selectItem);

                        //Focus on the listbox and item
                        int selectedIndex = focusListBox.SelectedIndex;

                        //Scroll to the listbox item
                        object scrollListBoxItem = focusListBox.Items[selectedIndex];
                        focusListBox.ScrollIntoView(scrollListBoxItem);

                        //Force focus on element
                        ListBoxItem focusListBoxItem = (ListBoxItem)focusListBox.ItemContainerGenerator.ContainerFromInd‌​ex(selectedIndex);
                        await FrameworkElementFocus(focusListBoxItem, false, windowHandle);

                        Debug.WriteLine("Focusing on listbox item index: " + selectedIndex);
                    }
                    else
                    {
                        Debug.WriteLine("Listbox cannot be focused on, pressing tab key.");
                        KeySendSingle(KeysVirtual.Tab, windowHandle);
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed focusing on the listbox, pressing tab key: " + ex.Message);
                KeySendSingle(KeysVirtual.Tab, windowHandle);
            }
        }

        //Select a listbox item
        public static void ListBoxSelectItem(ListBox focusListBox, object selectItem)
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    //Update the listbox layout
                    focusListBox.UpdateLayout();

                    //Select the listbox item
                    if (selectItem != null)
                    {
                        try
                        {
                            focusListBox.SelectedItem = selectItem;
                        }
                        catch { }
                    }

                    //Check the selected index
                    if (focusListBox.SelectedIndex == -1)
                    {
                        focusListBox.SelectedIndex = 0;
                        Debug.WriteLine("No selection, selecting first listbox index.");
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed selecting the listbox item: " + ex.Message);
            }
        }
    }
}