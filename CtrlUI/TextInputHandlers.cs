using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static ArnoldVinkCode.AVInterface;
using static CtrlUI.AppVariables;

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
                    await ClosePopupSetText();
                }
            }
            catch { }
        }

        //Close the popup and store text
        async void Button_TextInputConfirmText_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ClosePopupSetText();
            }
            catch { }
        }

        //Close the popup and store text
        async Task ClosePopupSetText()
        {
            try
            {
                if (grid_Popup_TextInput_textbox.Text != "Enter text...")
                {
                    //Enter text and mouse selection
                    vTextInputTargetTextBox.Text = grid_Popup_TextInput_textbox.Text;
                    vTextInputTargetTextBox.SelectionStart = grid_Popup_TextInput_textbox.Text.Length;

                    //Close the text input popup
                    await Popup_Close_TextInput();
                }
            }
            catch { }
        }

        //Open the keyboard controller
        async void Button_TextInputKeyboardController_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CloseShowKeyboardController();
                await FocusOnElement(grid_Popup_TextInput_textbox, false, vProcessCurrent.MainWindowHandle);
            }
            catch { }
        }
    }
}