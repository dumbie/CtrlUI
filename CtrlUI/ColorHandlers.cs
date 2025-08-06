using ArnoldVinkStyles;
using System.Diagnostics;
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
                //Check which mouse button is pressed
                if (vMousePressDownLeft)
                {
                    //Get clicked item
                    SolidColorBrush clickedObject = AVListView.GetRoutedListViewItemObject<SolidColorBrush>(e);

                    await ListView_ColorPicker_Click(clickedObject);
                }
            }
            catch { }
        }

        //Handle color picker keyboard/controller tapped
        async void ListView_ColorPicker_KeyPressUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                //Check which key is pressed
                if (e.Key == VirtualKey.Space)
                {
                    //Get clicked item
                    SolidColorBrush clickedObject = AVListView.GetRoutedListViewItemObject<SolidColorBrush>(e);

                    await ListView_ColorPicker_Click(clickedObject);
                }
            }
            catch { }
        }

        //Handle color picker click
        async Task ListView_ColorPicker_Click(SolidColorBrush solidColorBrush)
        {
            try
            {
                //Check clicked object
                if (solidColorBrush == null)
                {
                    Debug.WriteLine("Clicked ListView object is null.");
                    return;
                }

                //Save clicked color
                string colorLightHex = solidColorBrush.ToString();
                SettingSave(vConfigurationCtrlUI, "ColorAccentLight", colorLightHex);

                //Change application accent color
                ChangeApplicationAccentColor(colorLightHex);

                //Close the color picker
                await Popup_Close_ColorPicker();
            }
            catch { }
        }
    }
}