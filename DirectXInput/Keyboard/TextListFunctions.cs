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
                if (!CheckTextPopupsOpen())
                {
                    await ShowTextPopup();
                }
                else
                {
                    await HideTextPopups();
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
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false, false);

                //Store keyboard focus button
                FrameworkElementFocusSave(vFocusedButtonKeyboard, null);

                //Show the textlist menu
                border_TextListPopup.Visibility = Visibility.Visible;
                grid_Keyboard_Keys.IsEnabled = false;

                //Focus on popup button
                if (vFocusedButtonText.FocusListBox == null)
                {
                    if (vDirectKeyboardTextList.Any())
                    {
                        FrameworkElementFocus focusListbox = new FrameworkElementFocus();
                        focusListbox.FocusListBox = listbox_TextList;
                        focusListbox.FocusIndex = vLastPopupListTextIndex;
                        await FrameworkElementFocusFocus(focusListbox, vInteropWindowHandle);
                    }
                    else
                    {
                        await FrameworkElementFocus(key_TextListClose, false, vInteropWindowHandle);
                    }
                }
                else
                {
                    await FrameworkElementFocusFocus(vFocusedButtonText, vInteropWindowHandle);
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
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false, false);

                //Store open focus button
                FrameworkElementFocusSave(vFocusedButtonText, null);

                //Hide the textlist menu
                border_TextListPopup.Visibility = Visibility.Collapsed;
                grid_Keyboard_Keys.IsEnabled = true;
                vLastPopupListType = "Text";
                vLastPopupListTextIndex = listbox_TextList.SelectedIndex;

                //Focus on keyboard button
                if (vFocusedButtonKeyboard.FocusElement == null)
                {
                    await FrameworkElementFocus(key_TextList, false, vInteropWindowHandle);
                }
                else
                {
                    await FrameworkElementFocusFocus(vFocusedButtonKeyboard, vInteropWindowHandle);
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
        void Listbox_TextList_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                KeyTypeStringSenderShared(sender);
            }
            catch { }
        }

        void Listbox_TextList_PreviewKeyUp(object sender, KeyEventArgs e)
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
                    PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                    ProfileShared SelectedItem = (ProfileShared)ListboxSender.SelectedItem;
                    KeyTypeStringSend(SelectedItem.String1);
                }
            }
            catch { }
        }
    }
}