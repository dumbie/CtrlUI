using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace KeyboardController
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
                TrayContextMenu.MenuItems.Add("Website", NotifyIcon_Website);
                TrayContextMenu.MenuItems.Add("Exit", NotifyIcon_Exit);

                //Initialize the tray notify icon. 
                TrayNotifyIcon.Text = "Keyboard Controller";
                TrayNotifyIcon.Icon = new Icon(Assembly.GetEntryAssembly().GetManifestResourceStream("KeyboardController.Assets.AppIcon.ico"));

                //Handle Single Click event
                TrayNotifyIcon.MouseUp += NotifyIcon_MouseUp;

                //Add menu to tray icon and show it.  
                TrayNotifyIcon.ContextMenu = TrayContextMenu;
                TrayNotifyIcon.Visible = true;
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

        async void Mouse_Single_Click(MouseEventArgs args)
        {
            try
            {
                if (args.Button == MouseButtons.Middle)
                {
                    await Application_Exit();
                }
            }
            catch { }
        }

        void NotifyIcon_MouseUp(object sender, MouseEventArgs args) { Mouse_Single_Click(args); }
        void NotifyIcon_Settings(object sender, EventArgs args) { Application_ShowHideSettings(); }
        void NotifyIcon_Website(object sender, EventArgs args) { Process.Start("https://projects.arnoldvink.com"); }
        async void NotifyIcon_Exit(object sender, EventArgs args) { await Application_Exit(); }
    }
}