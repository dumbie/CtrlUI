using ArnoldVinkCode;
using System.Windows;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVWindowFunctions;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer.ToolsOverlay
{
    public partial class WindowTools : Window
    {
        //Show or hide tools
        private void ShowHide_Tools()
        {
            try
            {
                bool showNotes = SettingLoad(vConfigurationFpsOverlayer, "ToolsShowNotes", typeof(bool));
                bool showBrowser = SettingLoad(vConfigurationFpsOverlayer, "ToolsShowBrowser", typeof(bool));

                //Switch visibility
                if (showNotes)
                {
                    border_Notes.Visibility = Visibility.Visible;
                }
                else
                {
                    border_Notes.Visibility = Visibility.Collapsed;
                }

                if (showBrowser)
                {
                    border_Browser.Visibility = Visibility.Visible;
                }
                else
                {
                    border_Browser.Visibility = Visibility.Collapsed;
                }
            }
            catch { }
        }

        //Switch tools visibility
        public void SwitchToolsVisibility()
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    bool overlayVisible = grid_ToolsOverlayer.Visibility == Visibility.Visible;
                    if (overlayVisible && vToolsBlockInteract)
                    {
                        SwitchToolsClickthrough(false);
                    }
                    else if (overlayVisible)
                    {
                        grid_ToolsOverlayer.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        grid_ToolsOverlayer.Visibility = Visibility.Visible;
                    }
                });
            }
            catch { }
        }

        //Switch clickthrough mode
        public void SwitchToolsClickthrough(bool forceVisible)
        {
            try
            {
                if (forceVisible || vToolsBlockInteract)
                {
                    //Show bars
                    border_Menu.Visibility = Visibility.Visible;
                    grid_Browser_Menu.Visibility = Visibility.Visible;
                    grid_Notes_Menu.Visibility = Visibility.Visible;

                    //Update window style
                    vToolsBlockInteract = false;
                    WindowUpdateStyle(vInteropWindowHandle, true, vToolsBlockInteract, vToolsBlockInteract);
                }
                else
                {
                    //Hide bars
                    border_Menu.Visibility = Visibility.Collapsed;
                    grid_Browser_Menu.Visibility = Visibility.Collapsed;
                    grid_Browser_Manage.Visibility = Visibility.Collapsed;
                    grid_Notes_Menu.Visibility = Visibility.Collapsed;
                    grid_Notes_Manage.Visibility = Visibility.Collapsed;

                    //Update window style
                    vToolsBlockInteract = true;
                    WindowUpdateStyle(vInteropWindowHandle, true, vToolsBlockInteract, vToolsBlockInteract);
                }
            }
            catch { }
        }
    }
}