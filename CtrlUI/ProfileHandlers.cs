using ArnoldVinkCode;
using System.Windows;
using System.Windows.Input;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Change the edit profile category
        async void Grid_Popup_ProfileManager_button_ChangeProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ChangeProfileCategory();
            }
            catch { }
        }

        //Add new profile value
        async void grid_Popup_ProfileManager_textbox_ProfileString_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    await AddSaveNewProfileValue();
                }
            }
            catch { }
        }

        //Add new profile value
        async void Grid_Popup_ProfileManager_button_ProfileAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await AddSaveNewProfileValue();
            }
            catch { }
        }

        //Handle profile manager keyboard/controller tapped
        async void ListBox_ProfileManager_KeyPressUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    await ProfileManager_DeleteProfile();
                }
            }
            catch { }
        }

        //Handle profile manager mouse/touch tapped
        async void ListBox_ProfileManager_MousePressUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Check if an actual ListBoxItem is clicked
                if (!AVFunctions.ListBoxItemClickCheck((DependencyObject)e.OriginalSource)) { return; }

                //Check which mouse button is pressed
                if (e.ClickCount == 1)
                {
                    if (vMousePressDownLeft)
                    {
                        await ProfileManager_DeleteProfile();
                    }
                }
            }
            catch { }
        }
    }
}