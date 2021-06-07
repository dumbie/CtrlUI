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
using static LibraryShared.Classes;

namespace LibraryShared
{
    public partial class FocusFunctions
    {
        //Focus framework element
        public static async Task FrameworkElementFocus(FrameworkElement focusElement, bool mouseCapture, IntPtr windowHandle)
        {
            try
            {
                if (focusElement != null && focusElement.Focusable && focusElement.Visibility == Visibility.Visible)
                {
                    int whileLoopCount = 0;
                    while (Keyboard.FocusedElement != focusElement)
                    {
                        //Update the element layout
                        focusElement.UpdateLayout();

                        //Logical focus on the element
                        focusElement.Focus();

                        //Keyboard focus on the element
                        Keyboard.Focus(focusElement);

                        //Mouse capture the element
                        if (mouseCapture)
                        {
                            Mouse.Capture(focusElement);
                        }

                        //Press tab key when no element is focused
                        if (whileLoopCount >= 30)
                        {
                            if (Keyboard.FocusedElement == null)
                            {
                                Debug.WriteLine("Failed focusing on the element after " + whileLoopCount + " times, pressing tab key.");
                                await KeySendSingle(KeysVirtual.Tab, windowHandle);
                            }
                            return;
                        }

                        whileLoopCount++;
                        await Task.Delay(10);
                    }

                    //Debug.WriteLine("Forced keyboard focus on: " + focusElement);
                }
                else
                {
                    Debug.WriteLine("Focus element cannot be focused on, pressing tab key.");
                    await KeySendSingle(KeysVirtual.Tab, windowHandle);
                }
            }
            catch
            {
                Debug.WriteLine("Failed focusing on the element, pressing tab key.");
                await KeySendSingle(KeysVirtual.Tab, windowHandle);
            }
        }

        //Save framework focus element
        public static void FrameworkElementFocusSave(FrameworkElementFocus saveElement, FrameworkElement focusedElement)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Get the currently focused element
                    if (focusedElement == null)
                    {
                        focusedElement = (FrameworkElement)Keyboard.FocusedElement;
                    }

                    //Check the currently focused element
                    if (focusedElement != null)
                    {
                        //Get focus type
                        Type focusType = focusedElement.GetType().BaseType;

                        //Validate focus type
                        if (focusType == typeof(Window))
                        {
                            Debug.WriteLine("Invalid element focus type: " + focusType);
                            saveElement = null;
                            return;
                        }

                        //Check if save element is null
                        if (saveElement == null)
                        {
                            Debug.WriteLine("Save element is null creating new.");
                            saveElement = new FrameworkElementFocus();
                        }

                        //Save focused element
                        saveElement.FocusElement = focusedElement;
                        if (focusType == typeof(ListBoxItem))
                        {
                            saveElement.FocusListBox = AVFunctions.FindVisualParent<ListBox>(saveElement.FocusElement);
                            saveElement.FocusIndex = saveElement.FocusListBox.SelectedIndex;
                        }

                        Debug.WriteLine("Saved element focus: " + focusedElement + " / index: " + saveElement.FocusIndex);
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed saving element focus: " + ex.Message);
            }
        }

        //Focus framework focus element
        public static async Task FrameworkElementFocusFocus(FrameworkElementFocus focusElement, IntPtr windowHandle)
        {
            try
            {
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    //Check if focus element is disconnected
                    bool disconnectedSource = false;
                    if (focusElement.FocusElement != null)
                    {
                        disconnectedSource = focusElement.FocusElement.DataContext == BindingOperations.DisconnectedSource;
                    }

                    //Force focus on element
                    if (focusElement.FocusElement != null && !disconnectedSource)
                    {
                        Debug.WriteLine("Focusing on previous element: " + focusElement.FocusElement);
                        await FrameworkElementFocus(focusElement.FocusElement, false, windowHandle);
                    }
                    else if (focusElement.FocusListBox != null && !disconnectedSource)
                    {
                        Debug.WriteLine("Focusing on previous listbox: " + focusElement.FocusListBox);
                        await ListboxFocusIndex(focusElement.FocusListBox, false, false, focusElement.FocusIndex, windowHandle);
                    }
                    else
                    {
                        Debug.WriteLine("No previous focus element, pressing tab key.");
                        await KeySendSingle(KeysVirtual.Tab, windowHandle);
                    }

                    //Reset the previous focus
                    focusElement.Reset();
                });
            }
            catch { }
        }
    }
}