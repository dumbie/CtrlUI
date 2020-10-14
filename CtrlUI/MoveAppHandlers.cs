using System.Windows;

namespace CtrlUI
{
    partial class WindowMain
    {
        async void Btn_MoveAppRight_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await MoveApplicationList_Right();
            }
            catch { }
        }

        async void Btn_MoveAppLeft_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await MoveApplicationList_Left();
            }
            catch { }
        }
    }
}