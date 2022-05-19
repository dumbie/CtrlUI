using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVAudioDevice;
using static DirectXInput.AppVariables;
using static LibraryShared.Settings;
using static LibraryShared.SoundPlayer;
using static LibraryUsb.FakerInputDevice;

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
                PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);

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
                    if (sendKeyType == typeof(KeyboardKeys))
                    {
                        KeyboardKeys sendKey = (KeyboardKeys)sendButton.Tag;
                        Debug.WriteLine("Sending Keyboard key: " + sendKey);
                        SendKeyKeyboard(sendKey);
                    }
                    else if (sendKeyType == typeof(KeyboardModifiers))
                    {
                        KeyboardModifiers sendKey = (KeyboardModifiers)sendButton.Tag;
                        Debug.WriteLine("Sending Modifier key: " + sendKey);
                        SendKeyModifier(sendKey);
                    }
                    else if (sendKeyType == typeof(KeyboardMultimedia))
                    {
                        KeyboardMultimedia sendKey = (KeyboardMultimedia)sendButton.Tag;
                        Debug.WriteLine("Sending Multimedia key: " + sendKey);
                        if (sendKey == KeyboardMultimedia.VolumeMute)
                        {
                            if (AudioMuteSwitch(false))
                            {
                                await App.vWindowOverlay.Notification_Show_Status("VolumeMute", "Output muted");
                            }
                            else
                            {
                                await App.vWindowOverlay.Notification_Show_Status("VolumeMute", "Output unmuted");
                            }
                        }
                        else if (sendKey == KeyboardMultimedia.VolumeUp)
                        {
                            int volumeStep = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "MediaVolumeStep"));
                            int newVolume = AudioVolumeUp(volumeStep, false);
                            await App.vWindowOverlay.Notification_Show_Status("VolumeUp", "Increased volume to " + newVolume);
                        }
                        else if (sendKey == KeyboardMultimedia.VolumeDown)
                        {
                            int volumeStep = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "MediaVolumeStep"));
                            int newVolume = AudioVolumeDown(volumeStep, false);
                            await App.vWindowOverlay.Notification_Show_Status("VolumeDown", "Decreased volume to " + newVolume);
                        }
                        else
                        {
                            SendKeyMultimedia(sendKey);
                        }
                    }
                }
            }
            catch { }
        }

        void SendKeyKeyboard(KeyboardKeys sendKey)
        {
            try
            {
                //Check if the caps lock is enabled
                if (vCapsEnabled)
                {
                    if (sendKey == KeyboardKeys.Enter)
                    {
                        vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.ControlLeft, KeyboardModifiers.None, KeyboardKeys.Z, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);
                    }
                    else if (sendKey == KeyboardKeys.Home)
                    {
                        vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.None, KeyboardModifiers.None, KeyboardKeys.Home, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);
                    }
                    else if (sendKey == KeyboardKeys.End)
                    {
                        vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.None, KeyboardModifiers.None, KeyboardKeys.End, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);
                    }
                    else
                    {
                        vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.ShiftLeft, KeyboardModifiers.None, sendKey, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);
                    }
                }
                else
                {
                    vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.None, KeyboardModifiers.None, sendKey, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);
                }
            }
            catch { }
        }

        void SendKeyModifier(KeyboardModifiers sendKey)
        {
            try
            {
                //Check if the caps lock is enabled
                if (vCapsEnabled)
                {
                    if (sendKey == KeyboardModifiers.ShiftLeft || sendKey == KeyboardModifiers.ShiftRight)
                    {
                        vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.ControlLeft, KeyboardModifiers.None, KeyboardKeys.X, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);
                    }
                    else if (sendKey == KeyboardModifiers.ControlLeft || sendKey == KeyboardModifiers.ControlRight)
                    {
                        vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.ControlLeft, KeyboardModifiers.None, KeyboardKeys.C, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);
                    }
                    else if (sendKey == KeyboardModifiers.AltLeft || sendKey == KeyboardModifiers.AltRight)
                    {
                        vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.ControlLeft, KeyboardModifiers.None, KeyboardKeys.V, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);
                    }
                    else if (sendKey == KeyboardModifiers.WindowsLeft || sendKey == KeyboardModifiers.WindowsRight)
                    {
                        vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.ControlLeft, KeyboardModifiers.None, KeyboardKeys.A, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);
                    }
                }
                else
                {
                    vFakerInputDevice.KeyboardPressRelease(sendKey, KeyboardModifiers.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);
                }
            }
            catch { }
        }

        void SendKeyMultimedia(KeyboardMultimedia sendKey)
        {
            try
            {
                vFakerInputDevice.MultimediaPressRelease(sendKey);
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