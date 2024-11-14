using System.Windows;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Update drivers buttons
        async void btn_Settings_InstallDrivers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Message_UpdateDrivers();
            }
            catch { }
        }
    }
}