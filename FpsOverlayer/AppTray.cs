﻿using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using static ArnoldVinkCode.AVFunctions;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class AppTray
    {
        //Tray Menu Variables
        public NotifyIcon TrayNotifyIcon = new NotifyIcon();
        public ContextMenuStrip TrayContextMenu = new ContextMenuStrip();

        public AppTray()
        {
            try
            {
                Debug.WriteLine("Creating application tray menu.");

                //Create a context menu for systray.
                TrayContextMenu.Items.Add("Show or hide stats", null, NotifyIcon_ShowHide_Stats);
                TrayContextMenu.Items.Add("Show or hide crosshair", null, NotifyIcon_ShowHide_Crosshair);
                TrayContextMenu.Items.Add("Show or hide browser", null, NotifyIcon_ShowHide_Browser);
                TrayContextMenu.Items.Add("Change stats position", null, NotifyIcon_Position_Stats);
                TrayContextMenu.Items.Add("-");
                TrayContextMenu.Items.Add("Settings", null, NotifyIcon_Settings);
                TrayContextMenu.Items.Add("Website", null, NotifyIcon_Website);
                TrayContextMenu.Items.Add("Exit", null, NotifyIcon_Exit);

                //Initialize the tray notify icon. 
                TrayNotifyIcon.Text = "Fps Overlayer";
                TrayNotifyIcon.Icon = new Icon(AVEmbedded.EmbeddedResourceToStream(null, "FpsOverlayer.Assets.AppIcon.ico"));

                //Handle Single Click event
                TrayNotifyIcon.MouseUp += NotifyIcon_MouseUp;

                //Add menu to tray icon and show it.  
                TrayNotifyIcon.ContextMenuStrip = TrayContextMenu;
                TrayNotifyIcon.Visible = true;
            }
            catch { }
        }

        //Show and hide settings
        void Application_ShowHideSettings()
        {
            try
            {
                vWindowSettings.Show();
            }
            catch { }
        }

        void Mouse_Single_Click(MouseEventArgs args)
        {
            try
            {
                if (args.Button == MouseButtons.Left)
                {
                    vWindowMain.SwitchFpsOverlayVisibility();
                }
                else if (args.Button == MouseButtons.Middle)
                {
                    vWindowMain.ChangeFpsOverlayPosition();
                }
            }
            catch { }
        }

        void NotifyIcon_MouseUp(object sender, MouseEventArgs args) { Mouse_Single_Click(args); }
        void NotifyIcon_ShowHide_Stats(object sender, EventArgs args) { vWindowMain.SwitchFpsOverlayVisibility(); }
        void NotifyIcon_ShowHide_Crosshair(object sender, EventArgs args) { vWindowMain.SwitchCrosshairVisibility(true); }
        void NotifyIcon_ShowHide_Browser(object sender, EventArgs args) { vWindowBrowser.Browser_Switch_Visibility(); }
        void NotifyIcon_Position_Stats(object sender, EventArgs args) { vWindowMain.ChangeFpsOverlayPosition(); }
        void NotifyIcon_Settings(object sender, EventArgs args) { Application_ShowHideSettings(); }
        void NotifyIcon_Website(object sender, EventArgs args) { OpenWebsiteBrowser("https://projects.arnoldvink.com"); }
        async void NotifyIcon_Exit(object sender, EventArgs args) { await AppExit.Exit(); }
    }
}