using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static DirectXInput.AppVariables;
using static LibraryShared.Settings;
using static LibraryShared.SoundPlayer;

namespace DirectXInput.KeyboardCode
{
    partial class WindowKeyboard
    {
        //Handle key press
        async void ButtonKey_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    //Send the clicked button
                    await KeyButtonClick(sender);
                }
            }
            catch { }
        }
        async void ButtonKey_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Send the clicked button
                await KeyButtonClick(sender);
            }
            catch { }
        }

        //Send the clicked button
        async Task KeyButtonClick(object sender)
        {
            try
            {
                PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);

                Button sendButton = sender as Button;
                Type sendKeyType = sendButton.Tag.GetType();
                string sendKeyName = sendButton.Tag.ToString();
                if (sendKeyType == typeof(string))
                {
                    if (sendKeyName == "EmojiPopup")
                    {
                        await ShowHideEmojiListPopup();
                    }
                    else if (sendKeyName == "TextListPopup")
                    {
                        await ShowHideTextListPopup();
                    }
                }
                else
                {
                    KeysVirtual sendKeyVirtual = (KeysVirtual)sendButton.Tag;
                    Debug.WriteLine("Sending Virtual key: " + sendKeyVirtual);
                    SendKeyVirtual(sendKeyVirtual);
                }
            }
            catch { }
        }

        void SendKeyVirtual(KeysVirtual sendKeyVirtual)
        {
            try
            {
                //Check if the caps lock is enabled
                if (vCapsEnabled)
                {
                    if (sendKeyVirtual == KeysVirtual.Shift || sendKeyVirtual == KeysVirtual.ShiftLeft || sendKeyVirtual == KeysVirtual.ShiftRight)
                    {
                        KeyPressReleaseCombo(KeysVirtual.Control, KeysVirtual.X);
                    }
                    else if (sendKeyVirtual == KeysVirtual.Control || sendKeyVirtual == KeysVirtual.ControlLeft || sendKeyVirtual == KeysVirtual.ControlRight)
                    {
                        KeyPressReleaseCombo(KeysVirtual.Control, KeysVirtual.C);
                    }
                    else if (sendKeyVirtual == KeysVirtual.Alt || sendKeyVirtual == KeysVirtual.AltLeft || sendKeyVirtual == KeysVirtual.AltRight)
                    {
                        KeyPressReleaseCombo(KeysVirtual.Control, KeysVirtual.V);
                    }
                    else if (sendKeyVirtual == KeysVirtual.WindowsLeft || sendKeyVirtual == KeysVirtual.WindowsRight)
                    {
                        KeyPressReleaseCombo(KeysVirtual.Control, KeysVirtual.A);
                    }
                    else if (sendKeyVirtual == KeysVirtual.Enter)
                    {
                        KeyPressReleaseCombo(KeysVirtual.Control, KeysVirtual.Z);
                    }
                    else if (sendKeyVirtual == KeysVirtual.Home)
                    {
                        KeyPressReleaseSingle(KeysVirtual.Home);
                    }
                    else if (sendKeyVirtual == KeysVirtual.End)
                    {
                        KeyPressReleaseSingle(KeysVirtual.End);
                    }
                    else
                    {
                        KeyPressReleaseCombo(KeysVirtual.Shift, sendKeyVirtual);
                    }
                }
                else
                {
                    KeyPressReleaseSingle(sendKeyVirtual);
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
                if (Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "KeyboardMode")) == 0)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        textblock_ThumbRightOff.Text = "Move";
                        image_ScrollMove.Source = vImagePreloadIconKeyboardScroll;
                        //ToolTip newTooltip = new ToolTip() { Content = "Switch to mouse wheel mode" };
                        //key_ScrollMove.ToolTip = newTooltip;
                    });
                }
                else
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        textblock_ThumbRightOff.Text = "Scroll";
                        image_ScrollMove.Source = vImagePreloadIconKeyboardMove;
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
                if (Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "KeyboardMode")) == 0)
                {
                    Setting_Save(vConfigurationDirectXInput, "KeyboardMode", "1");
                    UpdateKeyboardMode();
                }
                else
                {
                    Setting_Save(vConfigurationDirectXInput, "KeyboardMode", "0");
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
                    await this.Hide();
                }
            }
            catch { }
        }
        async void ButtonClose_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                await this.Hide();
            }
            catch { }
        }
    }
}