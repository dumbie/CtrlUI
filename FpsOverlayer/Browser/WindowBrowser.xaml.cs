using Microsoft.Win32;
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

                //Bind lists to the listbox elements
                ListBoxBindLists();

                //Reset browser interface to defaults
                Browser_Reset_Interface(string.Empty);

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

                //Remove browser from grid
                Browser_Remove_Grid(string.Empty);
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
                        WindowUpdateStyleVisible(vInteropWindowHandle, true, true, vBrowserWindowClickThrough);

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
                        WindowUpdateStyleHidden(vInteropWindowHandle);

                        this.Title = "FpsOverlayer Browser (Hidden)";
                        vWindowVisible = false;
                        Debug.WriteLine("Hiding the window.");
                    }
                }
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

                //Check if window is out of screen
                WindowCheckScreenBounds(null, vInteropWindowHandle, 10);
            }
            catch { }
        }

        //Bind the lists to the listbox elements
        void ListBoxBindLists()
        {
            try
            {
                listbox_Link.ItemsSource = vFpsBrowserLinks;

                Debug.WriteLine("Lists bound to interface.");
            }
            catch { }
        }
    }
}