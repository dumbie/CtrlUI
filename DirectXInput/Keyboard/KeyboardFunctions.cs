using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static DirectXInput.AppVariables;
using static LibraryShared.Settings;
using static LibraryShared.SoundPlayer;

namespace DirectXInput.Keyboard
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
                    if (sendKeyName == "DotCom")
                    {
                        if (vCapsEnabled)
                        {
                            KeyTypeStringSend(Setting_Load(vConfigurationDirectXInput, "KeyboardDomainExtension").ToString());
                        }
                        else
                        {
                            KeyTypeStringSend(Setting_Load(vConfigurationDirectXInput, "KeyboardDomainExtensionDefault").ToString());
                        }
                    }
                    else if (sendKeyName == "Emoji")
                    {
                        Emoji.Wpf.TextBlock emojiTextblock = FindVisualChild<Emoji.Wpf.TextBlock>(sendButton);
                        if (emojiTextblock != null)
                        {
                            KeyTypeStringSend(emojiTextblock.Text);
                        }
                    }
                }
                else
                {
                    KeysVirtual sendKeyVirtual = (KeysVirtual)sendButton.Tag;
                    Debug.WriteLine("Sending key: " + sendKeyVirtual);

                    //Check if the caps lock is enabled
                    if (vCapsEnabled)
                    {
                        if (sendKeyVirtual == KeysVirtual.Shift)
                        {
                            await KeyPressComboAuto(KeysVirtual.Control, KeysVirtual.X);
                        }
                        else if (sendKeyVirtual == KeysVirtual.Control)
                        {
                            await KeyPressComboAuto(KeysVirtual.Control, KeysVirtual.C);
                        }
                        else if (sendKeyVirtual == KeysVirtual.Alt)
                        {
                            await KeyPressComboAuto(KeysVirtual.Control, KeysVirtual.V);
                        }
                        else if (sendKeyVirtual == KeysVirtual.Space)
                        {
                            await ShowHideEmojiMenu();
                        }
                        else if (sendKeyVirtual == KeysVirtual.Enter)
                        {
                            await KeyPressComboAuto(KeysVirtual.Control, KeysVirtual.Z);
                        }
                        else if (sendKeyVirtual == KeysVirtual.LeftWindows)
                        {
                            await KeyPressComboAuto(KeysVirtual.Control, KeysVirtual.A);
                        }
                        else if (sendKeyVirtual == KeysVirtual.Home)
                        {
                            await KeyPressSingleAuto(KeysVirtual.Home);
                        }
                        else if (sendKeyVirtual == KeysVirtual.End)
                        {
                            await KeyPressSingleAuto(KeysVirtual.End);
                        }
                        else
                        {
                            await KeyPressComboAuto(KeysVirtual.Shift, sendKeyVirtual);
                        }
                    }
                    else
                    {
                        await KeyPressSingleAuto(sendKeyVirtual);
                    }
                }
            }
            catch { }
        }

        //Handle capslock
        async void ButtonCaps_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    await SwitchCapsLock();
                }
            }
            catch { }
        }
        async void ButtonCaps_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                await SwitchCapsLock();
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
        void ButtonClose_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    this.Hide();
                }
            }
            catch { }
        }
        void ButtonClose_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.Hide();
            }
            catch { }
        }

        //Handle emoji close
        async void ButtonCloseEmoji_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    await ShowHideEmojiMenu();
                }
            }
            catch { }
        }
        async void ButtonCloseEmoji_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                await ShowHideEmojiMenu();
            }
            catch { }
        }
    }
}