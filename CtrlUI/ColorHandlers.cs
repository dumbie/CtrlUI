using ArnoldVinkCode;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.Styles.MainColors;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle color picker mouse/touch tapped
        async void ListBox_ColorPicker_MousePressUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Check if an actual ListBoxItem is clicked
                if (!AVFunctions.ListBoxItemClickCheck((DependencyObject)e.OriginalSource)) { return; }

                //Check which mouse button is pressed
                if (e.ClickCount == 1)
                {
                    if (vMousePressDownLeftClick)
                    {
                        await lb_ColorPicker_LeftClick();
                    }
                }
            }
            catch { }
        }

        //Handle color picker keyboard/controller tapped
        async void ListBox_ColorPicker_KeyPressUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
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
                if (lb_ColorPicker.SelectedItems.Count > 0 && lb_ColorPicker.SelectedIndex != -1)
                {
                    //Save the new accent color
                    SolidColorBrush selectedSolidColorBrush = (SolidColorBrush)lb_ColorPicker.SelectedItem;
                    string colorLightHex = selectedSolidColorBrush.ToString();
                    SettingSave(vConfigurationCtrlUI, "ColorAccentLight", colorLightHex);

                    //Change application accent color
                    ChangeApplicationAccentColor(colorLightHex);

                    //Notify applications setting changed
                    await NotifyDirectXInputSettingChanged("ColorAccentLight");
                    await NotifyFpsOverlayerSettingChanged("ColorAccentLight");

                    //Close the color picker
                    await Popup_Close_ColorPicker();
                }
            }
            catch { }
        }
    }
}