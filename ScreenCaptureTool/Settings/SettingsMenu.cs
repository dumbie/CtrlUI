using ArnoldVinkCode;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ScreenCapture
{
    partial class WindowMain
    {
        //Handle main menu mouse/touch tapped
        void lb_Menu_MousePressUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Check if an actual ListBoxItem is clicked
                if (!AVFunctions.ListBoxItemClickCheck((DependencyObject)e.OriginalSource)) { return; }

                //Check which mouse button is pressed
                lb_Menu_SingleTap();
            }
            catch { }
        }

        //Handle main menu keyboard/controller tapped
        void lb_Menu_KeyPressUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space) { lb_Menu_SingleTap(); }
            }
            catch { }
        }

        //Handle main menu single tap
        void lb_Menu_SingleTap()
        {
            try
            {
                if (lb_Menu.SelectedIndex >= 0)
                {
                    StackPanel SelStackPanel = (StackPanel)lb_Menu.SelectedItem;
                    if (SelStackPanel.Name == "menuButtonGeneral") { ShowGridPage(grid_General); }
                    else if (SelStackPanel.Name == "menuButtonScreenshot") { ShowGridPage(grid_Screenshot); }
                    else if (SelStackPanel.Name == "menuButtonRecording") { ShowGridPage(grid_Recording); }
                }
            }
            catch { }
        }

        //Display a certain grid page
        void ShowGridPage(FrameworkElement elementTarget)
        {
            try
            {
                grid_General.Visibility = Visibility.Collapsed;
                grid_Screenshot.Visibility = Visibility.Collapsed;
                grid_Recording.Visibility = Visibility.Collapsed;
                elementTarget.Visibility = Visibility.Visible;
            }
            catch { }
        }
    }
}