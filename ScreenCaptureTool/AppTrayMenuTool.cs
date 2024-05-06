using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using static ScreenCapture.AppVariables;

namespace ScreenCapture
{
    public partial class AppTrayMenuTool
    {
        //Tray Menu Variables
        public static NotifyIcon TrayNotifyIcon = new NotifyIcon();
        public static ContextMenuStrip TrayContextMenu = new ContextMenuStrip();

        //Create the application tray menu
        public static void Application_CreateTrayMenu()
        {
            try
            {
                Debug.WriteLine("Creating application tray menu.");

                //Create a context menu for system tray
                TrayContextMenu.Items.Add("Screen image capture", null, NotifyIcon_ImageCapture);
                TrayContextMenu.Items.Add("Start/stop video capture", null, NotifyIcon_StartVideoCapture);
                TrayContextMenu.Items.Add("Open capture location", null, NotifyIcon_OpenCapture);
                TrayContextMenu.Items.Add("-");
                TrayContextMenu.Items.Add("Settings", null, NotifyIcon_Settings);
                TrayContextMenu.Items.Add("Website", null, NotifyIcon_Website);
                TrayContextMenu.Items.Add("Exit", null, NotifyIcon_Exit);

                //Initialize the tray notify icon
                TrayNotifyIcon.Text = "Screen Capture Tool";
                TrayNotifyIcon.Icon = new Icon(AVEmbedded.EmbeddedResourceToStream(null, "ScreenCaptureTool.Assets.AppIcon.ico"));

                //Handle Double Click event
                TrayNotifyIcon.DoubleClick += NotifyIcon_DoubleClick;

                //Add menu to tray icon and show it
                TrayNotifyIcon.ContextMenuStrip = TrayContextMenu;
                TrayNotifyIcon.Visible = true;
            }
            catch { }
        }

        //Change tray icon
        public static void Application_ChangeTrayIcon(string iconName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(iconName))
                {
                    iconName = "AppIcon";
                }

                TrayNotifyIcon.Icon = new Icon(AVEmbedded.EmbeddedResourceToStream(null, "ScreenCaptureTool.Assets." + iconName + ".ico"));
            }
            catch { }
        }

        public static void NotifyIcon_DoubleClick(object sender, EventArgs args)
        {
            try
            {
                vWindowMain.Application_ShowHideWindow();
            }
            catch { }
        }

        public static async void NotifyIcon_ImageCapture(object sender, EventArgs args)
        {
            try
            {
                await CaptureScreen.CaptureImageProcess(1000);
            }
            catch { }
        }

        public static async void NotifyIcon_StartVideoCapture(object sender, EventArgs args)
        {
            try
            {
                await CaptureScreen.CaptureVideoProcess(1000);
            }
            catch { }
        }

        public static void NotifyIcon_OpenCapture(object sender, EventArgs args)
        {
            try
            {
                vWindowMain.OpenCaptureLocation();
            }
            catch { }
        }

        public static void NotifyIcon_Settings(object sender, EventArgs args)
        {
            try
            {
                vWindowMain.Application_ShowHideWindow();
            }
            catch { }
        }

        public static void NotifyIcon_Website(object sender, EventArgs args)
        {
            try
            {
                AVFunctions.OpenWebsiteBrowser("https://projects.arnoldvink.com");
            }
            catch { }
        }

        public static async void NotifyIcon_Exit(object sender, EventArgs args)
        {
            try
            {
                await AppClose.Application_Exit_Prompt();
            }
            catch { }
        }
    }
}