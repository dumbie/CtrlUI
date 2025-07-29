using Windows.System;
using Windows.UI.Xaml.Input;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Prevent ScrollViewer from moving in ListView
        private void ListView_ArrowScrollBlock(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                if (e.Key == VirtualKey.Down)
                {
                    FocusManager.TryMoveFocus(FocusNavigationDirection.Down);
                    e.Handled = true;
                }
                else if (e.Key == VirtualKey.Up)
                {
                    FocusManager.TryMoveFocus(FocusNavigationDirection.Up);
                    e.Handled = true;
                }
            }
            catch { }
        }
    }
}