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

        //Hide the window
        public new void Hide()
        {
            try
            {
                if (vWindowVisible)
                {
                    //Update the window visibility
                    UpdateWindowVisibility(false);
                }
            }
            catch { }
        }

        //Show the window
        public new void Show()
        {
            try
            {
                if (vInteropWindowHandle == IntPtr.Zero)
                {
                    //Get interop window handle
                    vInteropWindowHandle = new WindowInteropHelper(this).EnsureHandle();

                    //Check if resolution has changed
                    SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

                    //Show the window
                    base.Show();

                    //Update the window style
                    UpdateWindowStyle();

                    //Update the window position
                    UpdateWindowPosition();

                    //Update notification position
                    UpdateNotificationPosition();
                }

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
                await Task.Delay(500);

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
                    IntPtr updatedStyle = new IntPtr((uint)WindowStyles.WS_VISIBLE);
                    SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_STYLE, updatedStyle);

                    this.Title = "DirectXInput Overlay (Visible)";
                    vWindowVisible = true;
                    Debug.WriteLine("Showing the window.");
                }
                else
                {
                    IntPtr updatedStyle = IntPtr.Zero;
                    SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_STYLE, updatedStyle);

                    this.Title = "DirectXInput Overlay (Hidden)";
                    vWindowVisible = false;
                    Debug.WriteLine("Hiding the window.");
                }
            }
            catch { }
        }

        //Update the window style
        public void UpdateWindowStyle()
        {
            try
            {
                //Set the window style ex
                IntPtr updatedExStyle = new IntPtr((uint)(WindowStylesEx.WS_EX_TOPMOST | WindowStylesEx.WS_EX_NOACTIVATE | WindowStylesEx.WS_EX_TRANSPARENT));
                SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_EXSTYLE, updatedExStyle);

                //Set the window as top most
                SetWindowPos(vInteropWindowHandle, (IntPtr)WindowPosition.TopMost, 0, 0, 0, 0, (int)(WindowSWP.NOMOVE | WindowSWP.NOSIZE));

                Debug.WriteLine("Window style has been updated.");
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
                DisplayMonitorSettings displayMonitorSettings = GetScreenSettings(monitorNumber);

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