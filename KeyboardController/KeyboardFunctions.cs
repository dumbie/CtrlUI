using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessWin32Functions;
using static KeyboardController.AppVariables;
using static LibraryShared.SoundPlayer;

namespace KeyboardController
{
    partial class WindowMain
    {
        //Handle key press
        void ButtonKey_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    //Send the clicked button
                    KeyButtonClick(sender);
                }
            }
            catch { }
        }
        void ButtonKey_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Send the clicked button
                KeyButtonClick(sender);
            }
            catch { }
        }

        //Keyboard type string
        void KeyboardTypeString(string typeString)
        {
            try
            {
                foreach (char charString in typeString)
                {
                    short virtualKeyScan = VkKeyScanEx(charString, IntPtr.Zero);
                    bool shiftPressed = (virtualKeyScan & 0x100) == 0x100;
                    if (shiftPressed)
                    {
                        KeyPressCombo((byte)KeysVirtual.Shift, (byte)virtualKeyScan, false);
                    }
                    else
                    {
                        KeyPressSingle((byte)virtualKeyScan, false);
                    }
                }
            }
            catch { }
        }

        //Send the clicked button
        async void KeyButtonClick(object sender)
        {
            try
            {
                Button sendButton = sender as Button;
                string sendKeyName = sendButton.Tag.ToString();
                byte sendKeyVirtual = 0;
                try
                {
                    sendKeyVirtual = (byte)(KeysVirtual)Enum.Parse(typeof(KeysVirtual), sendKeyName, true);
                }
                catch { }
                Debug.WriteLine("Sending key: " + sendKeyName + "/" + sendKeyVirtual);
                PlayInterfaceSound("KeyboardPress", false);

                //Check for keys that are not caps capable or require extended
                if (sendKeyName == "DotCom")
                {
                    KeyboardTypeString(key_DotCom.Content.ToString());
                    return;
                }
                else if (sendKeyVirtual == (byte)KeysVirtual.Up)
                {
                    KeyPressSingle(sendKeyVirtual, true);
                    return;
                }
                else if (sendKeyVirtual == (byte)KeysVirtual.Down)
                {
                    KeyPressSingle(sendKeyVirtual, true);
                    return;
                }
                else if (sendKeyVirtual == (byte)KeysVirtual.Left)
                {
                    KeyPressSingle(sendKeyVirtual, true);
                    return;
                }
                else if (sendKeyVirtual == (byte)KeysVirtual.Right)
                {
                    KeyPressSingle(sendKeyVirtual, true);
                    return;
                }
                else if (sendKeyVirtual == (byte)KeysVirtual.Delete)
                {
                    KeyPressSingle(sendKeyVirtual, true);
                    return;
                }
                else if (sendKeyVirtual == (byte)KeysVirtual.Home)
                {
                    KeyPressSingle(sendKeyVirtual, false);
                    return;
                }
                else if (sendKeyVirtual == (byte)KeysVirtual.End)
                {
                    KeyPressSingle(sendKeyVirtual, false);
                    return;
                }

                //Check if the caps lock is enabled
                if (vCapsEnabled)
                {
                    if (sendKeyVirtual == (byte)KeysVirtual.Shift)
                    {
                        KeyPressCombo((byte)KeysVirtual.Control, (byte)KeysVirtual.X, false);
                    }
                    else if (sendKeyVirtual == (byte)KeysVirtual.Control)
                    {
                        KeyPressCombo((byte)KeysVirtual.Control, (byte)KeysVirtual.C, false);
                    }
                    else if (sendKeyVirtual == (byte)KeysVirtual.Menu)
                    {
                        KeyPressCombo((byte)KeysVirtual.Control, (byte)KeysVirtual.V, false);
                    }
                    else if (sendKeyVirtual == (byte)KeysVirtual.Space)
                    {
                        await ProcessLauncherWin32Async(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\Taskmgr.exe", "", "", false, false);
                    }
                    else if (sendKeyVirtual == (byte)KeysVirtual.Return)
                    {
                        KeyPressCombo((byte)KeysVirtual.Control, (byte)KeysVirtual.Z, false);
                    }
                    else if (sendKeyVirtual == (byte)KeysVirtual.LeftWindows)
                    {
                        KeyPressCombo((byte)KeysVirtual.Control, (byte)KeysVirtual.A, false);
                    }
                    else
                    {
                        KeyPressCombo((byte)KeysVirtual.Shift, sendKeyVirtual, false);
                    }
                }
                else
                {
                    KeyPressSingle(sendKeyVirtual, false);
                }
            }
            catch { }
        }

        //Handle capslock
        void ButtonCaps_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    SwitchCapsLock();
                }
            }
            catch { }
        }
        void ButtonCaps_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SwitchCapsLock();
            }
            catch { }
        }

        //Handle ScrollMove
        void ButtonScrollMove_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    SwitchKeyboardMode();
                }
            }
            catch { }
        }
        void ButtonScrollMove_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SwitchKeyboardMode();
            }
            catch { }
        }

        //Update the keyboard mode
        void UpdateKeyboardMode()
        {
            try
            {
                if (Convert.ToInt32(ConfigurationManager.AppSettings["KeyboardMode"]) == 0)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        textblock_ThumbRightOff.Text = "Move";
                        image_ScrollMove.Source = FileToBitmapImage(new string[] { "Assets/Icons/KeyboardScroll.png" }, IntPtr.Zero, -1, 0);
                        //ToolTip newTooltip = new ToolTip() { Content = "Switch to mouse wheel mode" };
                        //key_ScrollMove.ToolTip = newTooltip;
                    });
                }
                else
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        textblock_ThumbRightOff.Text = "Scroll";
                        image_ScrollMove.Source = FileToBitmapImage(new string[] { "Assets/Icons/KeyboardMove.png" }, IntPtr.Zero, -1, 0);
                        //ToolTip newTooltip = new ToolTip() { Content = "Switch to window move mode" };
                        //key_ScrollMove.ToolTip = newTooltip;
                    });
                }
            }
            catch { }
        }

        //Switch between keyboard modes
        void SwitchKeyboardMode()
        {
            try
            {
                if (Convert.ToInt32(ConfigurationManager.AppSettings["KeyboardMode"]) == 0)
                {
                    WindowSettings.SettingSave("KeyboardMode", "1");
                    UpdateKeyboardMode();
                }
                else
                {
                    WindowSettings.SettingSave("KeyboardMode", "0");
                    UpdateKeyboardMode();
                }
            }
            catch { }
        }

        //Handle close
        async void ButtonClose_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    await Application_Exit();
                }
            }
            catch { }
        }
        async void ButtonClose_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                await Application_Exit();
            }
            catch { }
        }
    }
}