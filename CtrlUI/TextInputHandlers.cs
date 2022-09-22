using System.Windows;
using System.Windows.Input;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Reset the popup to defaults
        async void Grid_Popup_TextInput_button_Reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Popup_Reset_TextInput(true, string.Empty);
            }
            catch { }
        }

        //Check text input key presses
        async void Grid_Popup_TextInput_textbox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    await ValidateSetTextInput();
                }
            }
            catch { }
        }

        //Close the popup and store text
        async void Button_TextInputConfirmText_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ValidateSetTextInput();
            }
            catch { }
        }
    }
}