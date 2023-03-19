using ArnoldVinkCode;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVFocus;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
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
                AVFocusDetailsSave(vFocusedButtonKeyboard, null);

                //Show the textlist menu
                border_TextListPopup.Visibility = Visibility.Visible;
                grid_Keyboard_Keys.IsEnabled = false;

                //Focus on popup button
                if (vFocusedButtonText.FocusListBox == null)
                {
                    if (vDirectKeyboardTextList.Any())
                    {
                        AVFocusDetails focusListbox = new AVFocusDetails();
                        focusListbox.FocusListBox = listbox_TextList;
                        focusListbox.FocusIndex = vLastPopupListTextIndex;
                        await AVFocusDetailsFocus(focusListbox, vInteropWindowHandle);
                    }
                    else
                    {
                        await FocusElement(key_TextListClose, vInteropWindowHandle);
                    }
                }
                else
                {
                    await AVFocusDetailsFocus(vFocusedButtonText, vInteropWindowHandle);
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
                AVFocusDetailsSave(vFocusedButtonText, null);

                //Hide the textlist menu
                border_TextListPopup.Visibility = Visibility.Collapsed;
                grid_Keyboard_Keys.IsEnabled = true;
                vLastPopupListType = "Text";
                vLastPopupListTextIndex = listbox_TextList.SelectedIndex;

                //Focus on keyboard button
                if (vFocusedButtonKeyboard.FocusElement == null)
                {
                    await FocusElement(key_TextList, vInteropWindowHandle);
                }
                else
                {
                    await AVFocusDetailsFocus(vFocusedButtonKeyboard, vInteropWindowHandle);
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
                //Check if an actual ListBoxItem is clicked
                if (!AVFunctions.ListBoxItemClickCheck((DependencyObject)e.OriginalSource)) { return; }

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