using System.Windows;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Close all popup windows
        async void Button_Popup_Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Popup_Close_Top();
            }
            catch { }
        }
    }
}