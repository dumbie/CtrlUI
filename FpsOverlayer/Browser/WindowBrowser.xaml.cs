using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using static ArnoldVinkCode.AVWindowFunctions;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer.OverlayCode
{
    public partial class WindowBrowser : Window
    {
        //Window Initialize
        public WindowBrowser() { InitializeComponent(); }

        //Window Variables
        public IntPtr vInteropWindowHandle = IntPtr.Zero;
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
            }
            catch { }
        }

        //Hide the window
        public new async Task Hide()
        {
            try
            {
                //Update the window visibility
                await UpdateWindowVisibility(false);

                //Reset browser default values
                Browser_Unload();
            }
            catch { }
        }

        //Show the window
        public new async Task Show()
        {
            try
            {
                //Update the window visibility
                await UpdateWindowVisibility(true);

                //Set default browser values
                await Browser_Setup();
            }
            catch { }
        }

        //Update the window visibility
        async Task UpdateWindowVisibility(bool visible)
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
                        await UpdateWindowStyleVisible(vInteropWindowHandle, true, vBrowserWindowNoActivate, vBrowserWindowClickThrough);

                        this.Title = "FpsOverlayer Browser (Visible)";
                        vWindowVisible = true;
                        Debug.WriteLine("Showing the window.");
                    }
                }
                else
                {
                    if (vWindowVisible)
                    {
                        //Update the window style
                        await UpdateWindowStyleHidden(vInteropWindowHandle);

                        this.Title = "FpsOverlayer Browser (Hidden)";
                        vWindowVisible = false;
                        Debug.WriteLine("Hiding the window.");
                    }
                }
            }
            catch { }
        }
    }
}