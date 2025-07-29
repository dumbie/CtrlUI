using ArnoldVinkStyles;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkStyles.MainColors;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle color picker mouse/touch tapped
        async void ListView_ColorPicker_MousePressUp(object sender, PointerRoutedEventArgs e)
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
                    await lb_ColorPicker_LeftClick();
                }
            }
            catch { }
        }

        //Handle color picker keyboard/controller tapped
        async void ListView_ColorPicker_KeyPressUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                if (e.Key == VirtualKey.Space)
                {
                    await lb_ColorPicker_LeftClick();
                }
            }
            catch { }
        }

        //Handle color picker left click
        async Task lb_ColorPicker_LeftClick()
        {
            try
            {
                if (listView_ColorPicker.SelectedItems.Count > 0 && listView_ColorPicker.SelectedIndex != -1)
                {
                    //Save the new accent color
                    SolidColorBrush selectedSolidColorBrush = (SolidColorBrush)listView_ColorPicker.SelectedItem;
                    string colorLightHex = selectedSolidColorBrush.ToString();
                    SettingSave(vConfigurationCtrlUI, "ColorAccentLight", colorLightHex);

                    //Change application accent color
                    ChangeApplicationAccentColor(colorLightHex);

                    //Close the color picker
                    await Popup_Close_ColorPicker();
                }
            }
            catch { }
        }
    }
}