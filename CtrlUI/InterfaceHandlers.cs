using Windows.Devices.Input;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
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
                await Popup_Show_Sorting();
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
                await Exit_Prompt();
            }
            catch { }
        }

        //Monitor application mouse down
        void WindowMain_PreviewMouseDown(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
                {
                    //Get pointer properties
                    PointerPointProperties pointerProps = e.GetCurrentPoint(null).Properties;

                    //Check which mouse button is pressed
                    vMousePressDownLeft = pointerProps.IsLeftButtonPressed;
                    vMousePressDownRight = pointerProps.IsRightButtonPressed;
                    vMousePressDownMiddle = pointerProps.IsMiddleButtonPressed;
                    vMousePressDownXButton1 = pointerProps.IsXButton1Pressed;
                    vMousePressDownXButton2 = pointerProps.IsXButton2Pressed;
                }
            }
            catch { }
        }
    }
}