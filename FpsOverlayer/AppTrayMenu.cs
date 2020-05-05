using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
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
                TrayContextMenu.MenuItems.Add("Settings", NotifyIcon_Settings);
                TrayContextMenu.MenuItems.Add("Applications", NotifyIcon_Applications);
                TrayContextMenu.MenuItems.Add("Website", NotifyIcon_Website);
                TrayContextMenu.MenuItems.Add("Exit", NotifyIcon_Exit);

                //Initialize the tray notify icon. 
                TrayNotifyIcon.Text = "Fps Overlayer";
                TrayNotifyIcon.Icon = new Icon(Assembly.GetEntryAssembly().GetManifestResourceStream("FpsOverlayer.Assets.AppIcon.ico"));

                //Handle Single Click event
                TrayNotifyIcon.MouseUp += NotifyIcon_MouseUp;

                //Add menu to tray icon and show it.  
                TrayNotifyIcon.ContextMenu = TrayContextMenu;
                TrayNotifyIcon.Visible = true;
            }
            catch { }
        }

        //Show and hide the fps overlay
        void ShowHideFpsOverlayerManual()
        {
            try
            {
                if (grid_FpsOverlayer.Visibility == Visibility.Visible)
                {
                    Debug.WriteLine("Hiding application.");
                    grid_FpsOverlayer.Visibility = Visibility.Collapsed;
                    vManualHidden = true;
                }
                else
                {
                    Debug.WriteLine("Showing application.");
                    grid_FpsOverlayer.Visibility = Visibility.Visible;
                    vManualHidden = false;

                    //Update the fps overlay style
                    UpdateFpsOverlayStyle();
                }
            }
            catch { }
        }

        //Show and hide settings
        void Application_ShowHideSettings()
        {
            try
            {
                App.vWindowSettings.Show();
            }
            catch { }
        }

        //Show and hide applications
        void Application_ShowHideApplications()
        {
            try
            {
                App.vWindowApplications.Show();
            }
            catch { }
        }

        async void Mouse_Single_Click(MouseEventArgs args)
        {
            try
            {
                if (args.Button == MouseButtons.Left)
                {
                    ShowHideFpsOverlayerManual();
                }
                else if (args.Button == MouseButtons.Middle)
                {
                    await ChangeWindowPosition();
                }
            }
            catch { }
        }

        void NotifyIcon_MouseUp(object sender, MouseEventArgs args) { Mouse_Single_Click(args); }
        void NotifyIcon_Settings(object sender, EventArgs args) { Application_ShowHideSettings(); }
        void NotifyIcon_Applications(object sender, EventArgs args) { Application_ShowHideApplications(); }
        void NotifyIcon_Website(object sender, EventArgs args) { Process.Start("https://projects.arnoldvink.com"); }
        async void NotifyIcon_Exit(object sender, EventArgs args) { await Application_Exit(); }
    }
}