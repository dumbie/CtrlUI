using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVInteropDll;
using static DirectXInput.AppVariables;
using static LibraryShared.Enums;
using static LibraryShared.Settings;

namespace DirectXInput.OverlayCode
{
    public partial class WindowOverlay : Window
    {
        //Window Initialize
        public WindowOverlay() { InitializeComponent(); }

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

                //Update the window style
                await UpdateWindowStyleVisible();

                //Update the window and text position
                UpdateWindowPosition();

                //Update the notification position
                UpdateNotificationPosition();

                //Check if resolution has changed
                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
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

        //Update the window position on resolution change
        public async void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                //Wait for change to complete
                await Task.Delay(500);

                //Update the window position
                UpdateWindowPosition();
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
                    await UpdateWindowStyleVisible();

                    this.Title = "DirectXInput Overlay (Visible)";
                    vWindowVisible = true;
                    Debug.WriteLine("Showing the window.");
                }
                else
                {
                    //Update the window style
                    await UpdateWindowStyleHidden();

                    this.Title = "DirectXInput Overlay (Hidden)";
                    vWindowVisible = false;
                    Debug.WriteLine("Hiding the window.");
                }
            }
            catch { }
        }

        //Update the window style
        async Task UpdateWindowStyleVisible()
        {
            try
            {
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    //Set the window style
                    IntPtr updatedStyle = new IntPtr((uint)WindowStyles.WS_VISIBLE);
                    await SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_STYLE, updatedStyle);

                    //Set the window style ex
                    IntPtr updatedExStyle = new IntPtr((uint)(WindowStylesEx.WS_EX_TOPMOST | WindowStylesEx.WS_EX_NOACTIVATE | WindowStylesEx.WS_EX_TRANSPARENT));
                    await SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_EXSTYLE, updatedExStyle);

                    //Set the window as top most (focus workaround)
                    SetWindowPos(vInteropWindowHandle, (IntPtr)WindowPosition.TopMost, 0, 0, 0, 0, (int)(WindowSWP.NOMOVE | WindowSWP.NOSIZE | WindowSWP.FRAMECHANGED | WindowSWP.NOCOPYBITS));
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

                    //Move window to force style
                    WindowRectangle positionRect = new WindowRectangle();
                    GetWindowRect(vInteropWindowHandle, ref positionRect);
                    if (vHideAdded)
                    {
                        WindowMove(vInteropWindowHandle, positionRect.Left + 1, positionRect.Top + 1);
                        vHideAdded = false;
                    }
                    else
                    {
                        WindowMove(vInteropWindowHandle, positionRect.Left - 1, positionRect.Top - 1);
                        vHideAdded = true;
                    }
                });
            }
            catch { }
        }

        //Update the window position
        public void UpdateWindowPosition()
        {
            try
            {
                //Get the current active screen
                int monitorNumber = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "DisplayMonitor"));
                DisplayMonitor displayMonitorSettings = GetSingleMonitorEnumDisplay(monitorNumber);

                //Move and resize the window
                WindowMove(vInteropWindowHandle, displayMonitorSettings.BoundsLeft, displayMonitorSettings.BoundsTop);
                WindowResize(vInteropWindowHandle, displayMonitorSettings.WidthNative, displayMonitorSettings.HeightNative);
            }
            catch { }
        }

        //Update the notification position
        public void UpdateNotificationPosition()
        {
            try
            {
                //Check current fps overlay position
                OverlayPosition fpsTextPosition = (OverlayPosition)Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "TextPosition"));
                //Debug.WriteLine("Fps overlayer text position: " + fpsTextPosition);

                //Move the notification position
                AVActions.ActionDispatcherInvoke(delegate
                {
                    if (vProcessFpsOverlayer == null || (vProcessFpsOverlayer != null && fpsTextPosition != OverlayPosition.TopLeft))
                    {
                        grid_Message_Status.HorizontalAlignment = HorizontalAlignment.Left;
                        grid_Message_Status_Grid.SetValue(Grid.ColumnProperty, 0);
                        grid_Message_Status_Border.SetValue(Grid.ColumnProperty, 1);
                        grid_Message_Status_Border.SetValue(Border.CornerRadiusProperty, new CornerRadius(0, 2, 2, 0));
                    }
                    else
                    {
                        grid_Message_Status.HorizontalAlignment = HorizontalAlignment.Right;
                        grid_Message_Status_Grid.SetValue(Grid.ColumnProperty, 1);
                        grid_Message_Status_Border.SetValue(Grid.ColumnProperty, 0);
                        grid_Message_Status_Border.SetValue(Border.CornerRadiusProperty, new CornerRadius(2, 0, 0, 2));
                    }
                });
            }
            catch { }
        }
    }
}