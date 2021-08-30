using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.FocusFunctions;
using static LibraryShared.SoundPlayer;

namespace DirectXInput.KeyboardCode
{
    partial class WindowKeyboard
    {
        //Show or hide the textlist menu
        async Task ShowHideTextListPopup()
        {
            try
            {
                if (border_EmojiListPopup.Visibility == Visibility.Collapsed && border_TextListPopup.Visibility == Visibility.Collapsed)
                {
                    await ShowTextPopup();
                }
                else
                {
                    await HideTextEmojiPopup();
                }
            }
            catch { }
        }

        //Show the text popup
        async Task ShowTextPopup()
        {
            try
            {
                //Play window open sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false);

                //Show the textlist menu
                border_TextListPopup.Visibility = Visibility.Visible;
                grid_Keyboard.Opacity = 0.60;
                grid_Keyboard.IsEnabled = false;

                //Store close focus button
                FrameworkElementFocusSave(vTextFocusedButtonClose, null);

                //Focus on popup button
                if (vTextFocusedButtonOpen.FocusListBox == null)
                {
                    if (vDirectKeyboardTextList.Any())
                    {
                        FrameworkElementFocus focusListbox = new FrameworkElementFocus();
                        focusListbox.FocusListBox = listbox_TextList;
                        focusListbox.FocusIndex = vLastPopupListTextIndex;
                        await FrameworkElementFocusFocus(focusListbox, vProcessCurrent.MainWindowHandle);
                    }
                    else
                    {
                        await FrameworkElementFocus(key_TextListClose, false, vInteropWindowHandle);
                    }
                }
                else
                {
                    await FrameworkElementFocusFocus(vTextFocusedButtonOpen, vProcessCurrent.MainWindowHandle);
                }
            }
            catch { }
        }

        //Hide the text popup
        async Task HideTextPopup()
        {
            try
            {
                //Play window close sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false);

                //Store open focus button
                FrameworkElementFocusSave(vTextFocusedButtonOpen, null);

                //Hide the textlist menu
                border_TextListPopup.Visibility = Visibility.Collapsed;
                grid_Keyboard.Opacity = 1;
                grid_Keyboard.IsEnabled = true;
                vLastPopupListType = "Text";
                vLastPopupListTextIndex = listbox_TextList.SelectedIndex;

                //Focus on keyboard button
                if (vTextFocusedButtonClose.FocusElement == null)
                {
                    await FrameworkElementFocus(key_TextList, false, vInteropWindowHandle);
                }
                else
                {
                    await FrameworkElementFocusFocus(vTextFocusedButtonClose, vProcessCurrent.MainWindowHandle);
                }
            }
            catch { }
        }

        //Hide the text or emoji popup
        async Task HideTextEmojiPopup()
        {
            try
            {
                if (border_EmojiListPopup.Visibility == Visibility.Visible)
                {
                    await HideEmojiPopup();
                }
                else if (border_TextListPopup.Visibility == Visibility.Visible)
                {
                    await HideTextPopup();
                }
            }
            catch { }
        }

        //Handle textlist close
        async void ButtonCloseTextList_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    await ShowHideTextListPopup();
                }
            }
            catch { }
        }
        async void ButtonCloseTextList_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                await ShowHideTextListPopup();
            }
            catch { }
        }

        //Handle textlist click
        void listbox_TextList_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                KeyTypeStringSenderShared(sender);
            }
            catch { }
        }

        void listbox_TextList_PreviewKeyUp(object sender, KeyEventArgs e)
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

        void KeyTypeStringSenderShared(object sender)
        {
            try
            {
                ListBox ListboxSender = (ListBox)sender;
                if (ListboxSender.SelectedItems.Count > 0 && ListboxSender.SelectedIndex != -1)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                    ProfileShared SelectedItem = (ProfileShared)ListboxSender.SelectedItem;
                    KeyTypeStringSend(SelectedItem.String1);
                }
            }
            catch { }
        }
    }
}