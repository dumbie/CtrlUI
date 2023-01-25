using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVWindowFunctions;
using static DirectXInput.AppVariables;

namespace DirectXInput.OverlayCode
{
    public partial class WindowOverlay : Window
    {
        //Window Initialize
        public WindowOverlay() { InitializeComponent(); }

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

                //Update the window style
                WindowUpdateStyleVisible(vInteropWindowHandle, true, true, true);

                //Update the window and text position
                UpdateWindowPosition();

                //Check if resolution has changed
                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
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

        //Update the window position on resolution change
        public async void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                //Wait for change to complete
                await Task.Delay(1000);

                //Update the window position
                UpdateWindowPosition();
            }
            catch { }
        }

        //Update the window visibility
        void UpdateWindowVisibility(bool visible)
        {
            try
            {
                if (visible)
                {
                    if (!vWindowVisible)
                    {
                        //Create and show the window
                        base.Show();

                        //Update the window style
                        WindowUpdateStyleVisible(vInteropWindowHandle, true, true, true);

                        this.Title = "DirectXInput Overlay (Visible)";
                        vWindowVisible = true;
                        Debug.WriteLine("Showing the window.");
                    }
                }
                else
                {
                    if (vWindowVisible)
                    {
                        //Update the window style
                        WindowUpdateStyleHidden(vInteropWindowHandle);

                        this.Title = "DirectXInput Overlay (Hidden)";
                        vWindowVisible = false;
                        Debug.WriteLine("Hiding the window.");
                    }
                }
            }
            catch { }
        }

        //Update the window position
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