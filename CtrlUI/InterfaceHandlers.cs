using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static ArnoldVinkCode.ProcessClasses;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

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
        void Button_MenuSorting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SortAppLists(false, false);
            }
            catch { }
        }

        //Handle refresh mouse presses
        async void Button_MenuRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await RefreshApplicationLists(false, false, false, false, false, true, true);
            }
            catch { }
        }

        //Open the keyboard controller
        void Button_KeyboardController_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CloseShowKeyboardController();
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

        //Handle exe file getting dropped into the window
        async void Application_DragDropFile(object sender, DragEventArgs e)
        {
            try
            {
                string[] DroppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
                string DroppedFile = DroppedFiles.FirstOrDefault();
                if (DroppedFile.EndsWith(".exe"))
                {
                    DataBindApp DropApp = new DataBindApp() { Type = ProcessType.Win32, PathExe = DroppedFile, PathLaunch = Path.GetDirectoryName(DroppedFile) };
                    await Popup_Show_AppDrop(DropApp);
                }
            }
            catch { }
        }
    }
}