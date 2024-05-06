using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using static ArnoldVinkCode.AVSettings;
using static ScreenCapture.AppVariables;

namespace ScreenCapture
{
    public partial class WindowMain : Window
    {
        //Window Initialize
        public WindowMain() { InitializeComponent(); }

        //Window Initialized
        protected override void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Update first launch setting
                SettingSave(vConfiguration, "AppFirstLaunch", "False");

                //Make sure the correct window style is set
                StateChanged += CheckWindowStateAndStyle;

                //Main menu functions
                lb_Menu.PreviewKeyUp += lb_Menu_KeyPressUp;
                lb_Menu.PreviewMouseUp += lb_Menu_MousePressUp;
            }
            catch { }
        }

        //Window Close Handler
        protected async override void OnClosing(CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                await AppClose.Application_Exit_Prompt();
            }
            catch { }
        }

        //Make sure the correct window style is set
        void CheckWindowStateAndStyle(object sender, EventArgs e)
        {
            try
            {
                if (WindowState == WindowState.Minimized) { Application_ShowHideWindow(); }
            }
            catch { }
        }

        //Show or hide the application window
        public void Application_ShowHideWindow()
        {
            try
            {
                if (ShowInTaskbar)
                {
                    Application_HideWindow();
                }
                else
                {
                    Application_ShowWindow();
                }
            }
            catch { }
        }

        //Show the application window
        public void Application_ShowWindow()
        {
            try
            {
                Debug.WriteLine("Show application from tray.");
                ShowInTaskbar = true;
                Visibility = Visibility.Visible;
                WindowState = WindowState.Normal;
            }
            catch { }
        }

        //Hide the application window
        public void Application_HideWindow()
        {
            try
            {
                Debug.WriteLine("Minimizing application to tray.");
                ShowInTaskbar = false;
                Visibility = Visibility.Collapsed;
                WindowState = WindowState.Normal;
            }
            catch { }
        }
    }
}