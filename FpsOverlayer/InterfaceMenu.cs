using ArnoldVinkCode;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    partial class WindowSettings
    {
        //Handle main menu mouse/touch tapped
        async void lb_Menu_MousePressUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Check if an actual ListBoxItem is clicked
                if (!AVFunctions.ListBoxItemClickCheck((DependencyObject)e.OriginalSource)) { return; }

                //Check which mouse button is pressed
                if (e.ClickCount == 1)
                {
                    vSingleTappedEvent = true;
                    await Task.Delay(500);
                    if (vSingleTappedEvent) { lb_Menu_SingleTap(); }
                }
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
        async void lb_Menu_SingleTap()
        {
            try
            {
                if (lb_Menu.SelectedIndex >= 0)
                {
                    StackPanel SelStackPanel = (StackPanel)lb_Menu.SelectedItem;
                    if (SelStackPanel.Name == "menuButtonText") { ShowGridPage(stackpanel_Text); }
                    else if (SelStackPanel.Name == "menuButtonColors") { ShowGridPage(stackpanel_Colors); }
                    else if (SelStackPanel.Name == "menuButtonDisplay") { ShowGridPage(stackpanel_Display); }
                    else if (SelStackPanel.Name == "menuButtonCrosshair") { ShowGridPage(stackpanel_Crosshair); }
                    else if (SelStackPanel.Name == "menuButtonBrowser") { ShowGridPage(stackpanel_Browser); }
                    else if (SelStackPanel.Name == "menuButtonShortcuts") { ShowGridPage(stackpanel_Shortcuts); }
                    else if (SelStackPanel.Name == "menuButtonFps") { ShowGridPage(stackpanel_Fps); }
                    else if (SelStackPanel.Name == "menuButtonTime") { ShowGridPage(stackpanel_Time); }
                    else if (SelStackPanel.Name == "menuButtonProcessor") { ShowGridPage(stackpanel_Processor); }
                    else if (SelStackPanel.Name == "menuButtonVideocard") { ShowGridPage(stackpanel_Videocard); }
                    else if (SelStackPanel.Name == "menuButtonMemory") { ShowGridPage(stackpanel_Memory); }
                    else if (SelStackPanel.Name == "menuButtonNetwork") { ShowGridPage(stackpanel_Network); }
                    else if (SelStackPanel.Name == "menuButtonMonitor") { ShowGridPage(stackpanel_Monitor); }
                    else if (SelStackPanel.Name == "menuButtonBattery") { ShowGridPage(stackpanel_Battery); }
                    else if (SelStackPanel.Name == "menuButtonApplications") { ShowGridPage(stackpanel_Applications); }
                    else if (SelStackPanel.Name == "menuButtonSettings") { ShowGridPage(stackpanel_Settings); }
                    else if (SelStackPanel.Name == "menuButtonClose") { this.Close(); }
                    else if (SelStackPanel.Name == "menuButtonExit") { await vWindowMain.Application_Exit(); }
                }
            }
            catch { }
        }

        //Display a certain grid page
        void ShowGridPage(FrameworkElement elementTarget)
        {
            try
            {
                stackpanel_Text.Visibility = Visibility.Collapsed;
                stackpanel_Colors.Visibility = Visibility.Collapsed;
                stackpanel_Display.Visibility = Visibility.Collapsed;
                stackpanel_Crosshair.Visibility = Visibility.Collapsed;
                stackpanel_Browser.Visibility = Visibility.Collapsed;
                stackpanel_Shortcuts.Visibility = Visibility.Collapsed;
                stackpanel_Fps.Visibility = Visibility.Collapsed;
                stackpanel_Time.Visibility = Visibility.Collapsed;
                stackpanel_Processor.Visibility = Visibility.Collapsed;
                stackpanel_Videocard.Visibility = Visibility.Collapsed;
                stackpanel_Memory.Visibility = Visibility.Collapsed;
                stackpanel_Network.Visibility = Visibility.Collapsed;
                stackpanel_Monitor.Visibility = Visibility.Collapsed;
                stackpanel_Battery.Visibility = Visibility.Collapsed;
                stackpanel_Applications.Visibility = Visibility.Collapsed;
                stackpanel_Settings.Visibility = Visibility.Collapsed;
                elementTarget.Visibility = Visibility.Visible;
            }
            catch { }
        }
    }
}