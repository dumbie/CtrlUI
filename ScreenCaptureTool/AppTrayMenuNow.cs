using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using static ScreenCapture.AppVariables;

namespace ScreenCapture
{
    public partial class AppTrayMenuNow
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
                TrayContextMenu.Items.Add("Stop screen capture", null, NotifyIcon_Stop);

                //Initialize the tray notify icon
                TrayNotifyIcon.Text = AVFunctions.StringCut("Capturing " + vCaptureFileName, 59, "...");
                TrayNotifyIcon.Icon = new Icon(AVEmbedded.EmbeddedResourceToStream(null, "ScreenCaptureTool.Assets.AppIconRecording.ico"));

                //Add menu to tray icon and show it
                TrayNotifyIcon.ContextMenuStrip = TrayContextMenu;
                TrayNotifyIcon.Visible = true;
            }
            catch { }
        }

        private static async void NotifyIcon_Stop(object sender, EventArgs args)
        {
            try
            {
                await AppClose.Application_Exit();
            }
            catch { }
        }
    }
}