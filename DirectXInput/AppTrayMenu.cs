using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Tray Menu Variables
        public static NotifyIcon TrayNotifyIcon = new NotifyIcon();
        public static ContextMenu TrayContextMenu = new ContextMenu();

        //Create the application tray menu
        void Application_CreateTrayMenu()
        {
            try
            {
                Debug.WriteLine("Creating application tray menu.");

                //Create a context menu for systray.
                TrayContextMenu.MenuItems.Add("Open Settings", NotifyIcon_Settings);
                TrayContextMenu.MenuItems.Add("Open Keyboard", NotifyIcon_Keyboard);
                TrayContextMenu.MenuItems.Add("Website", NotifyIcon_Website);
                TrayContextMenu.MenuItems.Add("Exit", NotifyIcon_Exit);

                //Initialize the tray notify icon. 
                TrayNotifyIcon.Text = "DirectXInput";
                TrayNotifyIcon.Icon = new Icon(Assembly.GetEntryAssembly().GetManifestResourceStream("DirectXInput.Assets.AppIcon.ico"));

                //Handle Middle Click event
                TrayNotifyIcon.MouseUp += NotifyIcon_MouseUp;

                //Handle Double Click event
                TrayNotifyIcon.DoubleClick += NotifyIcon_DoubleClick;

                //Add menu to tray icon and show it.  
                TrayNotifyIcon.ContextMenu = TrayContextMenu;
                TrayNotifyIcon.Visible = true;
            }
            catch { }
        }

        //Show and hide the application window
        void Application_ShowHideWindow()
        {
            try
            {
                if (ShowInTaskbar)
                {
                    Debug.WriteLine("Minimizing application to tray.");
                    ShowInTaskbar = false;
                    Visibility = Visibility.Collapsed;
                    WindowState = WindowState.Normal;
                }
                else
                {
                    Debug.WriteLine("Show the application from tray.");
                    ShowInTaskbar = true;
                    Visibility = Visibility.Visible;
                    WindowState = WindowState.Normal;
                }
            }
            catch { }
        }

        void NotifyIcon_DoubleClick(object sender, EventArgs args)
        {
            try
            {
                Application_ShowHideWindow();
            }
            catch { }
        }

        void NotifyIcon_Settings(object sender, EventArgs args)
        {
            try
            {
                Application_ShowHideWindow();
            }
            catch { }
        }

        async void NotifyIcon_Keyboard(object sender, EventArgs args)
        {
            try
            {
                await KeyboardPopupHideShow(true);
            }
            catch { }
        }

        void NotifyIcon_Website(object sender, EventArgs args)
        {
            try
            {
                Process.Start("https://projects.arnoldvink.com");
            }
            catch { }
        }

        void NotifyIcon_MouseUp(object sender, MouseEventArgs args)
        {
            try
            {
                if (args.Button == MouseButtons.Middle)
                {
                    StopAllControllers(false);
                }
            }
            catch { }
        }

        async void NotifyIcon_Exit(object sender, EventArgs args)
        {
            try
            {
                await Application_Exit_Prompt();
            }
            catch { }
        }
    }
}