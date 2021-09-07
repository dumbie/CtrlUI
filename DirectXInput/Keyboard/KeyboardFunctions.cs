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
using static LibraryUsb.VirtualHidDevice;

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
                    if (sendKeyType == typeof(KeysVirtual))
                    {
                        KeysVirtual sendKeyVirtual = (KeysVirtual)sendButton.Tag;
                        Debug.WriteLine("Sending Virtual key: " + sendKeyVirtual);
                        SendKeyVirtual(sendKeyVirtual);
                    }
                    else
                    {
                        KeysDDCode sendKeyDDCode = (KeysDDCode)sendButton.Tag;
                        Debug.WriteLine("Sending DDCode key: " + sendKeyDDCode);
                        SendKeyDDCode(sendKeyDDCode);
                    }
                }
            }
            catch { }
        }

        void SendKeyVirtual(KeysVirtual sendKeyVirtual)
        {
            try
            {
                KeyPressReleaseSingle(sendKeyVirtual);
            }
            catch { }
        }

        void SendKeyDDCode(KeysDDCode sendKeyDDCode)
        {
            try
            {
                //Check if the caps lock is enabled
                if (vCapsEnabled)
                {
                    if (sendKeyDDCode == KeysDDCode.ShiftLeft || sendKeyDDCode == KeysDDCode.ShiftRight)
                    {
                        vVirtualHidDevice.KeyPressReleaseCombo(KeysDDCode.ControlLeft, KeysDDCode.X);
                    }
                    else if (sendKeyDDCode == KeysDDCode.ControlLeft || sendKeyDDCode == KeysDDCode.ControlRight)
                    {
                        vVirtualHidDevice.KeyPressReleaseCombo(KeysDDCode.ControlLeft, KeysDDCode.C);
                    }
                    else if (sendKeyDDCode == KeysDDCode.AltLeft || sendKeyDDCode == KeysDDCode.AltRight)
                    {
                        vVirtualHidDevice.KeyPressReleaseCombo(KeysDDCode.ControlLeft, KeysDDCode.V);
                    }
                    else if (sendKeyDDCode == KeysDDCode.Enter)
                    {
                        vVirtualHidDevice.KeyPressReleaseCombo(KeysDDCode.ControlLeft, KeysDDCode.Z);
                    }
                    else if (sendKeyDDCode == KeysDDCode.LeftWindows)
                    {
                        vVirtualHidDevice.KeyPressReleaseCombo(KeysDDCode.ControlLeft, KeysDDCode.A);
                    }
                    else if (sendKeyDDCode == KeysDDCode.Home)
                    {
                        vVirtualHidDevice.KeyPressReleaseSingle(KeysDDCode.Home);
                    }
                    else
                    {
                        vVirtualHidDevice.KeyPressReleaseCombo(KeysDDCode.ShiftLeft, sendKeyDDCode);
                    }
                }
                else
                {
                    vVirtualHidDevice.KeyPressReleaseSingle(sendKeyDDCode);
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