using ArnoldVinkCode;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
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

        //Switch the emoji type list by button
        void SwitchEmojiTypeListButton(object sender)
        {
            try
            {
                //Shadow effect variable
                DropShadowEffect dropShadowEffect = new DropShadowEffect
                {
                    Color = new Color { A = 255, R = 0, G = 0, B = 0 },
                    Direction = 360,
                    ShadowDepth = 0,
                    BlurRadius = 15,
                    Opacity = 1
                };

                //Reset shadow effects
                key_EmojiSmileyText.Effect = null;
                key_EmojiActivityText.Effect = null;
                key_EmojiFoodText.Effect = null;
                key_EmojiNatureText.Effect = null;
                key_EmojiOtherText.Effect = null;
                key_EmojiPeopleText.Effect = null;
                key_EmojiSymbolText.Effect = null;
                key_EmojiTravelText.Effect = null;

                if (sender == key_EmojiSmiley)
                {
                    listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListSmiley;
                    key_EmojiSmileyText.Effect = dropShadowEffect;
                }
                else if (sender == key_EmojiActivity)
                {
                    listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListActivity;
                    key_EmojiActivityText.Effect = dropShadowEffect;
                }
                else if (sender == key_EmojiFood)
                {
                    listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListFood;
                    key_EmojiFoodText.Effect = dropShadowEffect;
                }
                else if (sender == key_EmojiNature)
                {
                    listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListNature;
                    key_EmojiNatureText.Effect = dropShadowEffect;
                }
                else if (sender == key_EmojiOther)
                {
                    listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListOther;
                    key_EmojiOtherText.Effect = dropShadowEffect;
                }
                else if (sender == key_EmojiPeople)
                {
                    listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListPeople;
                    key_EmojiPeopleText.Effect = dropShadowEffect;
                }
                else if (sender == key_EmojiSymbol)
                {
                    listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListSymbol;
                    key_EmojiSymbolText.Effect = dropShadowEffect;
                }
                else if (sender == key_EmojiTravel)
                {
                    listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListTravel;
                    key_EmojiTravelText.Effect = dropShadowEffect;
                }
            }
            catch { }
        }

        //Switch the emoji type list by trigger
        void SwitchEmojiTypeListTrigger(bool previous)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Shadow effect variable
                    DropShadowEffect dropShadowEffect = new DropShadowEffect
                    {
                        Color = new Color { A = 255, R = 0, G = 0, B = 0 },
                        Direction = 360,
                        ShadowDepth = 0,
                        BlurRadius = 15,
                        Opacity = 1
                    };

                    //Reset shadow effects
                    key_EmojiSmileyText.Effect = null;
                    key_EmojiActivityText.Effect = null;
                    key_EmojiFoodText.Effect = null;
                    key_EmojiNatureText.Effect = null;
                    key_EmojiOtherText.Effect = null;
                    key_EmojiPeopleText.Effect = null;
                    key_EmojiSymbolText.Effect = null;
                    key_EmojiTravelText.Effect = null;

                    if (!previous)
                    {
                        if (listbox_EmojiList.ItemsSource == vDirectKeyboardEmojiListSmiley)
                        {
                            listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListActivity;
                            key_EmojiActivityText.Effect = dropShadowEffect;
                        }
                        else if (listbox_EmojiList.ItemsSource == vDirectKeyboardEmojiListActivity)
                        {
                            listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListFood;
                            key_EmojiFoodText.Effect = dropShadowEffect;
                        }
                        else if (listbox_EmojiList.ItemsSource == vDirectKeyboardEmojiListFood)
                        {
                            listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListNature;
                            key_EmojiNatureText.Effect = dropShadowEffect;
                        }
                        else if (listbox_EmojiList.ItemsSource == vDirectKeyboardEmojiListNature)
                        {
                            listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListOther;
                            key_EmojiOtherText.Effect = dropShadowEffect;
                        }
                        else if (listbox_EmojiList.ItemsSource == vDirectKeyboardEmojiListOther)
                        {
                            listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListPeople;
                            key_EmojiPeopleText.Effect = dropShadowEffect;
                        }
                        else if (listbox_EmojiList.ItemsSource == vDirectKeyboardEmojiListPeople)
                        {
                            listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListSymbol;
                            key_EmojiSymbolText.Effect = dropShadowEffect;
                        }
                        else if (listbox_EmojiList.ItemsSource == vDirectKeyboardEmojiListSymbol)
                        {
                            listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListTravel;
                            key_EmojiTravelText.Effect = dropShadowEffect;
                        }
                        else if (listbox_EmojiList.ItemsSource == vDirectKeyboardEmojiListTravel)
                        {
                            listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListSmiley;
                            key_EmojiSmileyText.Effect = dropShadowEffect;
                        }
                    }
                    else
                    {
                        if (listbox_EmojiList.ItemsSource == vDirectKeyboardEmojiListSmiley)
                        {
                            listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListTravel;
                            key_EmojiTravelText.Effect = dropShadowEffect;
                        }
                        else if (listbox_EmojiList.ItemsSource == vDirectKeyboardEmojiListTravel)
                        {
                            listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListSymbol;
                            key_EmojiSymbolText.Effect = dropShadowEffect;
                        }
                        else if (listbox_EmojiList.ItemsSource == vDirectKeyboardEmojiListSymbol)
                        {
                            listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListPeople;
                            key_EmojiPeopleText.Effect = dropShadowEffect;
                        }
                        else if (listbox_EmojiList.ItemsSource == vDirectKeyboardEmojiListPeople)
                        {
                            listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListOther;
                            key_EmojiOtherText.Effect = dropShadowEffect;
                        }
                        else if (listbox_EmojiList.ItemsSource == vDirectKeyboardEmojiListOther)
                        {
                            listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListNature;
                            key_EmojiNatureText.Effect = dropShadowEffect;
                        }
                        else if (listbox_EmojiList.ItemsSource == vDirectKeyboardEmojiListNature)
                        {
                            listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListFood;
                            key_EmojiFoodText.Effect = dropShadowEffect;
                        }
                        else if (listbox_EmojiList.ItemsSource == vDirectKeyboardEmojiListFood)
                        {
                            listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListActivity;
                            key_EmojiActivityText.Effect = dropShadowEffect;
                        }
                        else if (listbox_EmojiList.ItemsSource == vDirectKeyboardEmojiListActivity)
                        {
                            listbox_EmojiList.ItemsSource = vDirectKeyboardEmojiListSmiley;
                            key_EmojiSmileyText.Effect = dropShadowEffect;
                        }
                    }
                });
            }
            catch { }
        }

        //Switch emoji button
        void ButtonSwitchEmoji_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    SwitchEmojiTypeListButton(sender);
                }
            }
            catch { }
        }
        void ButtonSwitchEmoji_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SwitchEmojiTypeListButton(sender);
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