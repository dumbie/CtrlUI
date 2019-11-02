using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static KeyboardController.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.OutputKeyboard;

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
                //Check if the first click happend
                if (vMouseClicks < 2)
                {
                    vMouseClicks += 1;
                    return;
                }

                //Send the clicked button
                KeyButtonClick(sender);
            }
            catch { }
        }

        //Send the clicked button
        void KeyButtonClick(object sender)
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

                //Check for keys that are not caps capable
                if (sendKeyName == "DotCom")
                {
                    KeyPressSingle((byte)KeysVirtual.OEMPeriod, false);
                    KeyPressSingle((byte)KeysVirtual.C, false);
                    KeyPressSingle((byte)KeysVirtual.O, false);
                    KeyPressSingle((byte)KeysVirtual.M, false);
                    return;
                }
                else if (sendKeyVirtual == (byte)KeysVirtual.LeftWindows) { KeyPressSingle(sendKeyVirtual, false); return; }
                else if (sendKeyVirtual == (byte)KeysVirtual.Up) { KeyPressSingle(sendKeyVirtual, true); return; }
                else if (sendKeyVirtual == (byte)KeysVirtual.Down) { KeyPressSingle(sendKeyVirtual, true); return; }
                else if (sendKeyVirtual == (byte)KeysVirtual.Left) { KeyPressSingle(sendKeyVirtual, true); return; }
                else if (sendKeyVirtual == (byte)KeysVirtual.Right) { KeyPressSingle(sendKeyVirtual, true); return; }

                //Check if the caps lock is enabled
                if (vCapsEnabled)
                {
                    if (sendKeyVirtual == (byte)KeysVirtual.Control) { KeyPressCombo((byte)KeysVirtual.Control, (byte)KeysVirtual.C, false); }
                    else if (sendKeyVirtual == (byte)KeysVirtual.Menu) { KeyPressCombo((byte)KeysVirtual.Control, (byte)KeysVirtual.V, false); }
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
                //Check if the first click happend
                if (vMouseClicks < 2)
                {
                    vMouseClicks += 1;
                    return;
                }

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
                //Check if the first click happend
                if (vMouseClicks < 2)
                {
                    vMouseClicks += 1;
                    return;
                }

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
                        image_ScrollMove.Source = new BitmapImage(new Uri(@"Assets/Icons/Scroll.png", UriKind.Relative));
                        //ToolTip newTooltip = new ToolTip() { Content = "Switch to mouse wheel mode" };
                        //key_ScrollMove.ToolTip = newTooltip;
                    });
                }
                else
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        textblock_ThumbRightOff.Text = "Scroll";
                        image_ScrollMove.Source = new BitmapImage(new Uri(@"Assets/Icons/Move.png", UriKind.Relative));
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
                //Check if the first click happend
                if (vMouseClicks < 2)
                {
                    vMouseClicks += 1;
                    return;
                }

                await Application_Exit();
            }
            catch { }
        }
    }
}