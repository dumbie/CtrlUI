using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.FocusFunctions;
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
        async void ButtonMode_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    await SwitchKeyboardMode();
                }
            }
            catch { }
        }
        async void ButtonMode_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                await SwitchKeyboardMode();
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

        //Set current keyboard mode
        async Task SetKeyboardMode()
        {
            try
            {
                KeyboardMode keyboardMode = (KeyboardMode)Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "KeyboardMode"));
                if (keyboardMode == KeyboardMode.Media)
                {
                    await SetModeMedia();
                }
                else if (keyboardMode == KeyboardMode.Keyboard)
                {
                    await SetModeKeyboard();
                }
            }
            catch { }
        }

        //Switch between keyboard modes
        async Task SwitchKeyboardMode()
        {
            try
            {
                KeyboardMode keyboardMode = (KeyboardMode)Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "KeyboardMode"));
                if (keyboardMode == KeyboardMode.Media)
                {
                    await SetModeKeyboard();
                }
                else if (keyboardMode == KeyboardMode.Keyboard)
                {
                    await SetModeMedia();
                }
            }
            catch { }
        }

        public async Task SetModeKeyboard()
        {
            try
            {
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    //Update help bar
                    stackpanel_DPad.Visibility = Visibility.Collapsed;
                    textblock_ButtonLeft.Text = "Backspace";
                    textblock_ButtonRight.Text = "Enter";
                    textblock_ButtonUp.Text = "Space";
                    textblock_ThumbRightOff.Text = "Scroll";
                    textblock_LeftTriggerOff.Text = "Caps";
                    textblock_RightTriggerOff.Text = "Tab";
                    textblock_ThumbPress.Text = "Arrows";
                    textblock_BackOff.Text = "Emoji/Text";
                    textblock_StartOff.Text = "Media";

                    //Show keyboard interface
                    grid_Keyboard.Visibility = Visibility.Visible;
                    grid_Media.Visibility = Visibility.Collapsed;

                    //Update border color
                    SolidColorBrush backgroundColor = new SolidColorBrush(Colors.Transparent);
                    border_ControllerHelp_Accent.Background = backgroundColor;
                    border_Header_Accent.Background = backgroundColor;
                    border_Media_Accent.Background = backgroundColor;

                    //Focus on keyboard button
                    if (vFocusedButtonKeyboard.FocusElement == null)
                    {
                        await FrameworkElementFocus(key_h, false, vInteropWindowHandle);
                    }
                    else
                    {
                        await FrameworkElementFocusFocus(vFocusedButtonKeyboard, vInteropWindowHandle);
                    }

                    //Play sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);

                    //Show notification
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Keyboard";
                    notificationDetails.Text = "Switched to keyboard mode";
                    await App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                    //Update settings
                    Setting_Save(vConfigurationDirectXInput, "KeyboardMode", Convert.ToInt32(KeyboardMode.Keyboard).ToString());
                });
            }
            catch { }
        }

        public async Task SetModeMedia()
        {
            try
            {
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    //Update help bar
                    stackpanel_DPad.Visibility = Visibility.Visible;
                    textblock_ButtonLeft.Text = "Media Prev";
                    textblock_ButtonRight.Text = "Media Next";
                    textblock_ButtonUp.Text = "Play/Pause";
                    textblock_ThumbRightOff.Text = "Move";
                    textblock_LeftTriggerOff.Text = string.Empty;
                    textblock_RightTriggerOff.Text = "Volume";
                    textblock_ThumbPress.Text = "Mute";
                    textblock_BackOff.Text = "Fullscreen";
                    textblock_StartOff.Text = "Keyboard";

                    //Save the previous focus element
                    FrameworkElementFocusSave(vFocusedButtonKeyboard, null);

                    //Show media interface
                    grid_Keyboard.Visibility = Visibility.Collapsed;
                    grid_Media.Visibility = Visibility.Visible;

                    //Update border color
                    SolidColorBrush backgroundBrush = (SolidColorBrush)Application.Current.Resources["ApplicationAccentLightBrush"];
                    SolidColorBrush backgroundBrushOpacity = AdjustColorOpacity(backgroundBrush, 0.20);
                    border_ControllerHelp_Accent.Background = backgroundBrushOpacity;
                    border_Header_Accent.Background = backgroundBrushOpacity;
                    border_Media_Accent.Background = backgroundBrushOpacity;

                    //Focus on media button
                    await FrameworkElementFocus(key_ModeMedia, false, vInteropWindowHandle);

                    //Play sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);

                    //Show notification
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Keyboard";
                    notificationDetails.Text = "Switched to media mode";
                    await App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                    //Update settings
                    Setting_Save(vConfigurationDirectXInput, "KeyboardMode", Convert.ToInt32(KeyboardMode.Media).ToString());
                });
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