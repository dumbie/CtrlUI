using ArnoldVinkCode;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVInterface;
using static CtrlUI.AppVariables;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Hide or show the TextInput
        async Task<string> Popup_ShowHide_TextInput(string textTitle, string textDefault, string buttonTitle)
        {
            try
            {
                //Check if the popup is open
                if (vTextInputOpen)
                {
                    await Popup_Close_Top();
                    return string.Empty;
                }

                //Reset text input variables
                vTextInputCancelled = false;
                vTextInputResult = string.Empty;
                vTextInputOpen = true;

                //Update the text input title
                if (!string.IsNullOrWhiteSpace(textTitle))
                {
                    grid_Popup_TextInput_textblock_Title.Text = textTitle;
                }
                else
                {
                    grid_Popup_TextInput_textblock_Title.Text = "Text Input";
                }

                //Update the button title
                if (!string.IsNullOrWhiteSpace(buttonTitle))
                {
                    grid_Popup_TextInput_button_ConfirmText.Content = buttonTitle;
                }
                else
                {
                    grid_Popup_TextInput_button_ConfirmText.Content = "Return and use the entered text";
                }

                //Reset the popup to defaults
                await Popup_Reset_TextInput(false, textDefault);

                //Play the opening sound
                PlayInterfaceSound(vInterfaceSoundVolume, "PopupOpen", false);

                //Save previous focused element
                if (Keyboard.FocusedElement != null)
                {
                    vTextInputElementFocus.FocusPrevious = (FrameworkElement)Keyboard.FocusedElement;
                    if (vTextInputElementFocus.FocusPrevious.GetType() == typeof(ListBoxItem))
                    {
                        vTextInputElementFocus.FocusListBox = AVFunctions.FindVisualParent<ListBox>(vTextInputElementFocus.FocusPrevious);
                        vTextInputElementFocus.FocusIndex = vTextInputElementFocus.FocusListBox.SelectedIndex;
                    }
                }

                //Show the popup with animation
                AVAnimations.Ani_Visibility(grid_Popup_TextInput, true, true, 0.10);
                AVAnimations.Ani_Opacity(grid_Main, 0.08, true, false, 0.10);

                //if (vTextInputOpen) { AVAnimations.Ani_Opacity(grid_Popup_TextInput, 0.02, true, false, 0.10); }
                if (vMessageBoxOpen) { AVAnimations.Ani_Opacity(grid_Popup_MessageBox, 0.02, true, false, 0.10); }
                if (vFilePickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_FilePicker, 0.02, true, false, 0.10); }
                if (vPopupOpen) { AVAnimations.Ani_Opacity(vPopupElementTarget, 0.02, true, false, 0.10); }
                if (vColorPickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_ColorPicker, 0.02, true, false, 0.10); }
                if (vSearchOpen) { AVAnimations.Ani_Opacity(grid_Popup_Search, 0.02, true, false, 0.10); }
                if (vMainMenuOpen) { AVAnimations.Ani_Opacity(grid_Popup_MainMenu, 0.02, true, false, 0.10); }

                //Force focus on an element
                await FocusOnElement(grid_Popup_TextInput_textbox, false, vProcessCurrent.MainWindowHandle);

                //Launch the keyboard controller
                if (vAppActivated && vControllerAnyConnected())
                {
                    LaunchKeyboardController(false);
                }

                //Wait for user input
                while (vTextInputResult == string.Empty && !vTextInputCancelled) { await Task.Delay(500); }
                if (vTextInputCancelled) { return string.Empty; }

                //Close and reset the popup
                await Popup_Close_TextInput();
            }
            catch { }
            return vTextInputResult;
        }

        //Reset the popup to defaults
        async Task Popup_Reset_TextInput(bool focusTextbox, string textDefault)
        {
            try
            {
                //Reset the text input
                if (!string.IsNullOrWhiteSpace(textDefault))
                {
                    //Enter text and mouse selection
                    grid_Popup_TextInput_textbox.Text = textDefault;
                    grid_Popup_TextInput_textbox.SelectionStart = grid_Popup_TextInput_textbox.Text.Length;

                    //Force focus on an element
                    await FocusOnElement(grid_Popup_TextInput_textbox, false, vProcessCurrent.MainWindowHandle);
                }
                else if (focusTextbox)
                {
                    //Empty the textbox text
                    grid_Popup_TextInput_textbox.Text = string.Empty;

                    //Force focus on an element
                    await FocusOnElement(grid_Popup_TextInput_textbox, false, vProcessCurrent.MainWindowHandle);
                }
                else
                {
                    grid_Popup_TextInput_textbox.Text = "Enter text...";
                }

                //Show or hide the keyboard icon
                if (CheckKeyboardEnabled())
                {
                    grid_Popup_TextInput_button_KeyboardControllerIcon.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_Popup_TextInput_button_KeyboardControllerIcon.Visibility = Visibility.Collapsed;
                }
            }
            catch { }
        }

        async Task Popup_Close_TextInput()
        {
            try
            {
                if (vTextInputOpen)
                {
                    //Play the closing sound
                    PlayInterfaceSound(vInterfaceSoundVolume, "PopupClose", false);

                    //Reset the popup variables
                    vTextInputCancelled = true;
                    //vTextInputResult = string.Empty;
                    vTextInputOpen = false;

                    //Hide the popup with animation
                    AVAnimations.Ani_Visibility(grid_Popup_TextInput, false, false, 0.10);

                    if (!Popup_Any_Open()) { AVAnimations.Ani_Opacity(grid_Main, 1, true, true, 0.10); }
                    else if (vTextInputOpen) { AVAnimations.Ani_Opacity(grid_Popup_TextInput, 1, true, true, 0.10); }
                    else if (vMessageBoxOpen) { AVAnimations.Ani_Opacity(grid_Popup_MessageBox, 1, true, true, 0.10); }
                    else if (vFilePickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_FilePicker, 1, true, true, 0.10); }
                    else if (vPopupOpen) { AVAnimations.Ani_Opacity(vPopupElementTarget, 1, true, true, 0.10); }
                    else if (vColorPickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_ColorPicker, 1, true, true, 0.10); }
                    else if (vSearchOpen) { AVAnimations.Ani_Opacity(grid_Popup_Search, 1, true, true, 0.10); }
                    else if (vMainMenuOpen) { AVAnimations.Ani_Opacity(grid_Popup_MainMenu, 1, true, true, 0.10); }

                    while (grid_Popup_TextInput.Visibility == Visibility.Visible)
                    {
                        await Task.Delay(10);
                    }

                    //Force focus on an element
                    if (vTextInputElementFocus.FocusTarget != null)
                    {
                        await FocusOnElement(vTextInputElementFocus.FocusTarget, false, vProcessCurrent.MainWindowHandle);
                    }
                    else if (vTextInputElementFocus.FocusListBox != null)
                    {
                        await FocusOnListbox(vTextInputElementFocus.FocusListBox, false, false, vTextInputElementFocus.FocusIndex);
                    }
                    else
                    {
                        await FocusOnElement(vTextInputElementFocus.FocusPrevious, false, vProcessCurrent.MainWindowHandle);
                    }

                    //Reset previous focus
                    vTextInputElementFocus.Reset();
                }
            }
            catch { }
        }
    }
}