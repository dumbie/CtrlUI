using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using static ArnoldVinkCode.AVInteropDll;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer.OverlayCode
{
    public partial class WindowBrowser : Window
    {
        //Window Initialize
        public WindowBrowser() { InitializeComponent(); }

        //Window Variables
        private IntPtr vInteropWindowHandle = IntPtr.Zero;
        public bool vWindowVisible = false;
        public bool vHideAdded = false;

        //Window Initialized
        protected override async void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Get interop window handle
                vInteropWindowHandle = new WindowInteropHelper(this).EnsureHandle();

                //Set render mode to software
                HwndSource hwndSource = HwndSource.FromHwnd(vInteropWindowHandle);
                HwndTarget hwndTarget = hwndSource.CompositionTarget;
                hwndTarget.RenderMode = RenderMode.SoftwareOnly;

                //Add browser to grid
                await Browser_Add_Grid();
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
                    //Create and show the window
                    base.Show();

                    //Update the window style
                    await UpdateWindowStyleVisible(vBrowserClickThrough);

                    this.Title = "FpsOverlayer Browser (Visible)";
                    vWindowVisible = true;
                    Debug.WriteLine("Showing the window.");
                }
                else
                {
                    //Hide the window
                    base.Hide();

                    //Update the window style
                    await UpdateWindowStyleHidden();

                    this.Title = "FpsOverlayer Browser (Hidden)";
                    vWindowVisible = false;
                    Debug.WriteLine("Hiding the window.");
                }
            }
            catch { }
        }

        //Update the window style
        public async Task UpdateWindowStyleVisible(bool clickThrough)
        {
            try
            {
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    //Set the window alpha
                    //SetLayeredWindowAttributes(vInteropWindowHandle, 0, 250, LayeredWindowAttributes.LWA_ALPHA);

                    //Set the window style
                    IntPtr updatedStyle = new IntPtr((uint)WindowStyles.WS_VISIBLE);
                    await SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_STYLE, updatedStyle);

                    //Set the window style ex
                    IntPtr updatedExStyle = IntPtr.Zero;
                    if (clickThrough)
                    {
                        updatedExStyle = new IntPtr((uint)(WindowStylesEx.WS_EX_TOPMOST | WindowStylesEx.WS_EX_TRANSPARENT));
                    }
                    else
                    {
                        updatedExStyle = new IntPtr((uint)WindowStylesEx.WS_EX_TOPMOST);
                    }

                    //IntPtr updatedExStyle = new IntPtr((uint)(WindowStylesEx.WS_EX_TOPMOST | WindowStylesEx.WS_EX_NOACTIVATE | WindowStylesEx.WS_EX_TRANSPARENT));
                    await SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_EXSTYLE, updatedExStyle);

                    //Set the window as top most (focus workaround)
                    SetWindowPos(vInteropWindowHandle, (IntPtr)WindowPosition.TopMost, 0, 0, 0, 0, (int)(WindowSWP.NOMOVE | WindowSWP.NOSIZE));
                });
            }
            catch { }
        }

        //Update the window style
        async Task UpdateWindowStyleHidden()
        {
            try
            {
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    //Set the window style
                    IntPtr updatedStyle = new IntPtr((uint)WindowStyles.WS_NONE);
                    await SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_STYLE, updatedStyle);
                });
            }
            catch { }
        }
    }
}