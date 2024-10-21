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

        //Handle sorting mouse presses
        async void Button_MenuSorting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SortListsAuto();
            }
            catch { }
        }

        //Handle minimize mouse presses
        private async void Button_MenuMinimize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Minimize CtrlUI window
                await AppWindowMinimize(false, true);
            }
            catch { }
        }

        //Handle close mouse presses
        private async void Button_MenuClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await AppExit.Exit_Prompt();
            }
            catch { }
        }

        //Monitor application mouse down
        void WindowMain_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Reset previous mouse click states
                vMousePressDownLeft = false;
                vMousePressDownRight = false;
                vMousePressDownMiddle = false;
                vMousePressDownXButton1 = false;

                //Check which mouse button is pressed
                if (e.ClickCount == 1)
                {
                    if (e.LeftButton == MouseButtonState.Pressed) { vMousePressDownLeft = true; }
                    else if (e.RightButton == MouseButtonState.Pressed) { vMousePressDownRight = true; }
                    else if (e.MiddleButton == MouseButtonState.Pressed) { vMousePressDownMiddle = true; }
                    else if (e.XButton1 == MouseButtonState.Pressed) { vMousePressDownXButton1 = true; }
                }
            }
            catch { }
        }
    }
}