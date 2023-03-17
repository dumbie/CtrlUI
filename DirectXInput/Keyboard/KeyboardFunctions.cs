using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.Styles.AVColors;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.FocusFunctions;
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
                    else if (sendKeyName == "ShortcutListPopup")
                    {
                        await ShowHideShortcutListPopup();
                    }
                }
                else
                {
                    if (sendKeyType == typeof(KeyboardAction))
                    {
                        KeyboardAction sendKey = (KeyboardAction)sendButton.Tag;
                        Debug.WriteLine("Sending Keyboard action: " + sendKey);
                        SendKeyAction(sendKey);
                    }
                    else if (sendKeyType == typeof(KeyboardMultimedia))
                    {
                        KeyboardMultimedia sendKey = (KeyboardMultimedia)sendButton.Tag;
                        Debug.WriteLine("Sending multimedia key: " + sendKey);
                        if (sendKey == KeyboardMultimedia.VolumeMute)
                        {
                            VolumeOutputMute();
                        }
                        else if (sendKey == KeyboardMultimedia.VolumeUp)
                        {
                            VolumeUp();
                        }
                        else if (sendKey == KeyboardMultimedia.VolumeDown)
                        {
                            VolumeDown();
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

        void SendKeyAction(KeyboardAction sendKey)
        {
            try
            {
                vFakerInputDevice.KeyboardPressRelease(sendKey);
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