using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static DirectXInput.AppVariables;
using static LibraryShared.SoundPlayer;

namespace DirectXInput.KeyboardCode
{
    partial class WindowKeyboard
    {
        //Show or hide the emoji popup
        async Task ShowHideEmojiListPopup()
        {
            try
            {
                if (border_EmojiListPopup.Visibility == Visibility.Collapsed && border_TextListPopup.Visibility == Visibility.Collapsed)
                {
                    await ShowEmojiPopup();
                }
                else
                {
                    await HideTextEmojiPopup();
                }
            }
            catch { }
        }

        //Show the emoji popup
        async Task ShowEmojiPopup()
        {
            //Play window open sound
            PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false);

            //Show the emoji menu
            border_EmojiListPopup.Visibility = Visibility.Visible;
            grid_Keyboard.Opacity = 0.60;
            grid_Keyboard.IsEnabled = false;

            //Update the help bar
            textblock_LeftTriggerOff.Text = string.Empty;
            textblock_RightTriggerOff.Text = "Switch emoji";

            //Store close focus button
            FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;
            if (frameworkElement != null && frameworkElement.GetType() == typeof(Button))
            {
                vEmojiFocusedButtonClose = (Button)frameworkElement;
            }

            //Focus on keyboard button
            if (vEmojiFocusedButtonOpen == null)
            {
                await FocusPopupButton(true, key_EmojiClose);
            }
            else
            {
                await FocusPopupButton(true, vEmojiFocusedButtonOpen);
            }
        }

        //Hide the emoji popup
        async Task HideEmojiPopup()
        {
            try
            {
                //Play window close sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false);

                //Hide the emoji menu
                border_EmojiListPopup.Visibility = Visibility.Collapsed;
                grid_Keyboard.Opacity = 1;
                grid_Keyboard.IsEnabled = true;
                vLastPopupListType = "Emoji";

                //Update the help bar
                textblock_LeftTriggerOff.Text = "Caps";
                textblock_RightTriggerOff.Text = "Tab";

                //Store open focus button
                FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;
                if (frameworkElement != null && frameworkElement.GetType() == typeof(Button))
                {
                    vEmojiFocusedButtonOpen = (Button)frameworkElement;
                }

                //Focus on keyboard button
                if (vEmojiFocusedButtonClose == null)
                {
                    await FocusPopupButton(true, key_Space);
                }
                else
                {
                    await FocusPopupButton(true, vEmojiFocusedButtonClose);
                }
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
                    await ShowHideEmojiListPopup();
                }
            }
            catch { }
        }
        async void ButtonCloseEmoji_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                await ShowHideEmojiListPopup();
            }
            catch { }
        }

        //Handle emoji click
        void listbox_EmojiList_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                KeyTypeStringSenderShared(sender);
            }
            catch { }
        }

        void listbox_EmojiList_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    KeyTypeStringSenderShared(sender);
                }
            }
            catch { }
        }
    }
}