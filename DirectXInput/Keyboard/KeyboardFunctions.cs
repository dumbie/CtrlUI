using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static DirectXInput.AppVariables;
using static LibraryShared.Enums;
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
                            await VolumeOutputMute();
                        }
                        else if (sendKey == KeyboardMultimedia.VolumeUp)
                        {
                            await VolumeUp();
                        }
                        else if (sendKey == KeyboardMultimedia.VolumeDown)
                        {
                            await VolumeDown();
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

        //Handle Mode Switch
        void ButtonMode_PreviewKeyUp(object sender, KeyEventArgs e)
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
        void ButtonMode_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SwitchKeyboardMode();
            }
            catch { }
        }

        //Handle CtrlUI and Fps Overlayer
        async void ButtonCtrlUI_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                await ProcessFunctions.LaunchShowCtrlUI();
            }
            catch { }
        }
        async void ButtonCtrlUI_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    await ProcessFunctions.LaunchShowCtrlUI();
                }
            }
            catch { }
        }

        async void ButtonFpsOverlayer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                await ProcessFunctions.LaunchCloseFpsOverlayer();
            }
            catch { }
        }
        async void ButtonFpsOverlayer_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    await ProcessFunctions.LaunchCloseFpsOverlayer();
                }
            }
            catch { }
        }

        //Update the keyboard mode
        void UpdateKeyboardMode()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Check keyboard mode
                    KeyboardMode keyboardMode = (KeyboardMode)Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "KeyboardMode"));
                    if (keyboardMode == KeyboardMode.Move)
                    {
                        textblock_ButtonLeft.Text = "Backspace";
                        textblock_ButtonRight.Text = "Enter";
                        textblock_ButtonUp.Text = "Space";
                        textblock_ThumbRightOff.Text = "Move";
                        textblock_LeftTriggerOff.Text = "Caps";
                        textblock_RightTriggerOff.Text = "Tab";
                        textblock_ThumbPress.Text = "Left/Right arrow";
                        textblock_BackOff.Text = "Emoji/Text";
                        image_Mode.Source = vImagePreloadIconKeyboardMove;
                    }
                    else if (keyboardMode == KeyboardMode.Scroll)
                    {
                        textblock_ButtonLeft.Text = "Backspace";
                        textblock_ButtonRight.Text = "Enter";
                        textblock_ButtonUp.Text = "Space";
                        textblock_ThumbRightOff.Text = "Scroll";
                        textblock_LeftTriggerOff.Text = "Caps";
                        textblock_RightTriggerOff.Text = "Tab";
                        textblock_ThumbPress.Text = "Left/Right arrow";
                        textblock_BackOff.Text = "Emoji/Text";
                        image_Mode.Source = vImagePreloadIconKeyboardScroll;
                    }
                    else if (keyboardMode == KeyboardMode.Media)
                    {
                        textblock_ButtonLeft.Text = "Media Prev";
                        textblock_ButtonRight.Text = "Media Next";
                        textblock_ButtonUp.Text = "Play/Pause";
                        textblock_ThumbRightOff.Text = "Arrows";
                        textblock_LeftTriggerOff.Text = "VolDown";
                        textblock_RightTriggerOff.Text = "VolUp";
                        textblock_ThumbPress.Text = "Mute";
                        textblock_BackOff.Text = "Fullscreen";
                        image_Mode.Source = vImagePreloadIconKeyboardMedia;
                    }
                });
            }
            catch { }
        }

        //Switch between keyboard modes
        void SwitchKeyboardMode()
        {
            try
            {
                //Check keyboard mode
                KeyboardMode keyboardMode = (KeyboardMode)Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "KeyboardMode"));
                if (keyboardMode == KeyboardMode.Move)
                {
                    Setting_Save(vConfigurationDirectXInput, "KeyboardMode", Convert.ToInt32(KeyboardMode.Scroll).ToString());
                    UpdateKeyboardMode();
                }
                else if (keyboardMode == KeyboardMode.Scroll)
                {
                    Setting_Save(vConfigurationDirectXInput, "KeyboardMode", Convert.ToInt32(KeyboardMode.Media).ToString());
                    UpdateKeyboardMode();
                }
                else if (keyboardMode == KeyboardMode.Media)
                {
                    Setting_Save(vConfigurationDirectXInput, "KeyboardMode", Convert.ToInt32(KeyboardMode.Move).ToString());
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