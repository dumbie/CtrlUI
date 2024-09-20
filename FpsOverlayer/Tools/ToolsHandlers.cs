using System.Windows;
using static ArnoldVinkCode.AVSettings;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowTools : Window
    {
        //Switch browser visibility
        private void button_ShowHide_Browser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (border_Browser.Visibility == Visibility.Visible)
                {
                    //Switch visibility
                    border_Browser.Visibility = Visibility.Collapsed;

                    //Update setting
                    SettingSave(vConfigurationFpsOverlayer, "ToolsShowBrowser", "False");
                }
                else
                {
                    //Switch visibility
                    border_Browser.Visibility = Visibility.Visible;

                    //Update setting
                    SettingSave(vConfigurationFpsOverlayer, "ToolsShowBrowser", "True");
                }
            }
            catch { }
        }

        //Switch notes visibility
        private void button_ShowHide_Notes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (border_Notes.Visibility == Visibility.Visible)
                {
                    //Switch visibility
                    border_Notes.Visibility = Visibility.Collapsed;

                    //Update setting
                    SettingSave(vConfigurationFpsOverlayer, "ToolsShowNotes", "False");
                }
                else
                {
                    //Switch visibility
                    border_Notes.Visibility = Visibility.Visible;

                    //Update setting
                    SettingSave(vConfigurationFpsOverlayer, "ToolsShowNotes", "True");
                }
            }
            catch { }
        }
    }
}