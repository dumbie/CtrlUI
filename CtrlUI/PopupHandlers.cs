using Windows.UI.Xaml;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Close the top popup window
        async void Button_Popup_Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Popup_Close_Top(false);
            }
            catch { }
        }
    }
}