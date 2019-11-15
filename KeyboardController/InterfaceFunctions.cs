using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static KeyboardController.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.OutputKeyboard;

namespace KeyboardController
{
    partial class WindowMain
    {
        //Force focus on an element
        async Task FocusOnElement(FrameworkElement focusElement)
        {
            try
            {
                if (focusElement != null && focusElement.Focusable && focusElement.Visibility == Visibility.Visible)
                {
                    int WhileLoop = 0;
                    while (Keyboard.FocusedElement != focusElement)
                    {
                        //Update the element layout
                        focusElement.UpdateLayout();

                        //Logical focus on the element
                        focusElement.Focus();

                        //Keyboard focus on the element
                        Keyboard.Focus(focusElement);

                        //Mouse capture the element
                        Mouse.Capture(focusElement);

                        if (WhileLoop >= 30)
                        {
                            Debug.WriteLine("Failed focusing on the element after " + WhileLoop + " times, pressing tab key.");
                            KeySendSingle((byte)KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                            return;
                        }

                        WhileLoop++;
                        await Task.Delay(10);
                    }

                    //Debug.WriteLine("Forced keyboard focus on: " + focusElement);
                }
                else
                {
                    Debug.WriteLine("Focus element cannot be focused on, pressing tab key.");
                    KeySendSingle((byte)KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                }
            }
            catch
            {
                Debug.WriteLine("Failed focusing on the element, pressing tab key.");
                KeySendSingle((byte)KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
            }
        }
    }
}