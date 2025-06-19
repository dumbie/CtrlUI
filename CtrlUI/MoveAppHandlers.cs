using System.Windows;

namespace CtrlUI
{
    partial class WindowMain
    {
        void Btn_MoveAppRight_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MoveApplicationList_Right();
            }
            catch { }
        }

        void Btn_MoveAppLeft_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MoveApplicationList_Left();
            }
            catch { }
        }
    }
}