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
                    //Set the text input result
                    if (grid_Popup_TextInput_textbox.Text == "Enter text...")
                    {
                        await Notification_Send_Status("Rename", "Invalid text");
                        vTextInputResult = string.Empty;
                    }
                    else
                    {
                        vTextInputResult = grid_Popup_TextInput_textbox.Text;
                    }
                }
            }
            catch { }
        }

        //Close the popup and store text
        async void Button_TextInputConfirmText_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Set the text input result
                if (grid_Popup_TextInput_textbox.Text == "Enter text...")
                {
                    await Notification_Send_Status("Rename", "Invalid text");
                    vTextInputResult = string.Empty;
                }
                else
                {
                    vTextInputResult = grid_Popup_TextInput_textbox.Text;
                }
            }
            catch { }
        }

        //Open the keyboard controller
        async void Button_TextInputKeyboardController_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await CloseShowKeyboardController();
                await FocusOnElement(grid_Popup_TextInput_textbox, false, vProcessCurrent.MainWindowHandle);
            }
            catch { }
        }
    }
}