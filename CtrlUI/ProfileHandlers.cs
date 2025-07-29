using ArnoldVinkStyles;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
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
        async void grid_Popup_ProfileManager_textbox_ProfileString_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                if (e.Key == VirtualKey.Enter)
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
        async void ListView_ProfileManager_KeyPressUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                if (e.Key == VirtualKey.Space)
                {
                    await ProfileManager_DeleteProfile();
                }
            }
            catch { }
        }

        //Handle profile manager mouse/touch tapped
        async void ListView_ProfileManager_MousePressUp(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                //Check if an actual ListViewItem is clicked
                if (!AVInterface.CheckClickedListViewItem(e))
                {
                    return;
                }

                //Check which mouse button is pressed
                if (vMousePressDownLeft)
                {
                    await ProfileManager_DeleteProfile();
                }
            }
            catch { }
        }
    }
}