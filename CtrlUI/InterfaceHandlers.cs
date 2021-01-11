using System.Windows;
using System.Windows.Input;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle hamburger mouse presses
        async void Button_MenuHamburger_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Popup_ShowHide_MainMenu(false);
            }
            catch { }
        }

        //Handle search mouse presses
        async void Button_MenuSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Popup_ShowHide_Search(false);
            }
            catch { }
        }

        //Handle sorting mouse presses
        async void Button_MenuSorting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SortAppListsSwitch(false);
            }
            catch { }
        }

        //Monitor application mouse movement
        void WindowMain_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                MouseCursorShow();
            }
            catch { }
        }

        //Monitor application mouse down
        void WindowMain_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Reset the previous mouse click states
                vMousePressDownLeftClick = false;
                vMousePressDownRightClick = false;
                vMousePressDownXButton1 = false;

                //Check which mouse button is pressed
                if (e.ClickCount == 1)
                {
                    if (e.LeftButton == MouseButtonState.Pressed) { vMousePressDownLeftClick = true; }
                    else if (e.RightButton == MouseButtonState.Pressed) { vMousePressDownRightClick = true; }
                    else if (e.XButton1 == MouseButtonState.Pressed) { vMousePressDownXButton1 = true; }
                }
            }
            catch { }
        }
    }
}