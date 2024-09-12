using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVWindowFunctions;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowCrosshair : Window
    {
        //Window Initialize
        public WindowCrosshair() { InitializeComponent(); }

        //Window Variables
        private IntPtr vInteropWindowHandle = IntPtr.Zero;
        public bool vWindowVisible = false;

        //Window Initialized
        protected override void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Get interop window handle
                vInteropWindowHandle = new WindowInteropHelper(this).EnsureHandle();

                //Set render mode to software
                HwndSource hwndSource = HwndSource.FromHwnd(vInteropWindowHandle);
                HwndTarget hwndTarget = hwndSource.CompositionTarget;
                hwndTarget.RenderMode = RenderMode.SoftwareOnly;

                //Update window style
                WindowUpdateStyle(vInteropWindowHandle, true, true, true);

                //Update window display affinity
                UpdateWindowAffinity();

                //Update window position
                UpdateWindowPosition();

                //Update the crosshair overlay style
                UpdateCrosshairOverlayStyle();

                //Show crosshair when enabled
                if (SettingLoad(vConfigurationFpsOverlayer, "CrosshairLaunch", typeof(bool)))
                {
                    SwitchCrosshairVisibility(true);
                }
            }
            catch { }
        }

        //Hide the window
        public new void Hide()
        {
            try
            {
                //Update the window visibility
                UpdateWindowVisibility(false);
            }
            catch { }
        }

        //Show the window
        public new void Show()
        {
            try
            {
                //Update the window visibility
                UpdateWindowVisibility(true);
            }
            catch { }
        }

        //Update window display affinity
        public void UpdateWindowAffinity()
        {
            try
            {
                if (SettingLoad(vConfigurationFpsOverlayer, "HideScreenCapture", typeof(bool)))
                {
                    SetWindowDisplayAffinity(vInteropWindowHandle, DisplayAffinityFlags.WDA_EXCLUDEFROMCAPTURE);
                }
                else
                {
                    SetWindowDisplayAffinity(vInteropWindowHandle, DisplayAffinityFlags.WDA_NONE);
                }
            }
            catch { }
        }

        //Update the window visibility
        public void UpdateWindowVisibility(bool visible)
        {
            try
            {
                if (visible)
                {
                    if (!vWindowVisible)
                    {
                        //Create and show the window
                        base.Show();

                        //Update window visibility
                        WindowUpdateVisibility(vInteropWindowHandle, true);

                        //Update window style
                        WindowUpdateStyle(vInteropWindowHandle, true, true, true);

                        this.Title = "Cross Overlayer (Visible)";
                        vWindowVisible = true;
                        Debug.WriteLine("Showing the window.");
                    }
                }
                else
                {
                    if (vWindowVisible)
                    {
                        //Update window visibility
                        WindowUpdateVisibility(vInteropWindowHandle, false);

                        this.Title = "Cross Overlayer (Hidden)";
                        vWindowVisible = false;
                        Debug.WriteLine("Hiding the window.");
                    }
                }
            }
            catch { }
        }

        //Update window position
        public void UpdateWindowPosition()
        {
            try
            {
                //Get the current active screen
                int monitorNumber = SettingLoad(vConfigurationCtrlUI, "DisplayMonitor", typeof(int));

                //Move the window position
                WindowUpdatePosition(monitorNumber, vInteropWindowHandle, AVWindowPosition.FullScreen);
            }
            catch { }
        }
    }
}