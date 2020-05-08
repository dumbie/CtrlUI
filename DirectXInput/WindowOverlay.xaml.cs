using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVInteropDll;
using static DirectXInput.AppVariables;
using static LibraryShared.Settings;

namespace DirectXInput
{
    public partial class WindowOverlay : Window
    {
        //Window Initialize
        public WindowOverlay() { InitializeComponent(); }

        //Window Variables
        public static IntPtr vInteropWindowHandle = IntPtr.Zero;

        //Window Initialized
        protected override void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Get application interop window handle
                vInteropWindowHandle = new WindowInteropHelper(this).EnsureHandle();

                //Update the window style
                UpdateWindowStyle();

                //Update the window and text position
                UpdateWindowPosition();

                //Update the battery status position
                UpdateBatteryPosition();

                //Update the notification position
                UpdateNotificationPosition();

                //Check if resolution has changed
                SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);
            }
            catch { }
        }

        //Update the window position on resolution change
        public void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                //Update the window and text position
                UpdateWindowPosition();
            }
            catch { }
        }

        //Update the window style
        void UpdateWindowStyle()
        {
            try
            {
                //Set the window style
                IntPtr UpdatedStyle = new IntPtr((uint)WindowStyles.WS_VISIBLE);
                SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_STYLE, UpdatedStyle);

                //Set the window style ex
                IntPtr UpdatedExStyle = new IntPtr((uint)(WindowStylesEx.WS_EX_TOPMOST | WindowStylesEx.WS_EX_NOACTIVATE | WindowStylesEx.WS_EX_TRANSPARENT));
                SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_EXSTYLE, UpdatedExStyle);

                //Set the window as top most
                SetWindowPos(vInteropWindowHandle, (IntPtr)WindowPosition.TopMost, 0, 0, 0, 0, (int)(WindowSWP.NOMOVE | WindowSWP.NOSIZE));

                Debug.WriteLine("The window style has been updated.");
            }
            catch { }
        }

        //Update the window position
        public void UpdateWindowPosition()
        {
            try
            {
                //Load current CtrlUI settings
                Settings_Load_CtrlUI(ref vConfigurationCtrlUI);

                //Get the current active screen
                int monitorNumber = Convert.ToInt32(vConfigurationCtrlUI.AppSettings.Settings["DisplayMonitor"].Value);
                DisplayMonitorResolution displayResolution = GetScreenResolutionBounds(monitorNumber);

                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Set the window size
                    this.Width = displayResolution.ScreenWidth;
                    this.Height = displayResolution.ScreenHeight;

                    //Move the window top left
                    this.Left = displayResolution.BoundsLeft;
                    this.Top = displayResolution.BoundsTop;
                });
            }
            catch { }
        }

        //Update the notification position
        public void UpdateNotificationPosition()
        {
            try
            {
                //Check current fps overlay position
                int fpsTextPosition = Convert.ToInt32(vConfigurationFpsOverlayer.AppSettings.Settings["TextPosition"].Value);
                //Debug.WriteLine("Fps overlayer text position: " + fpsTextPosition);

                //Move the notification position
                AVActions.ActionDispatcherInvoke(delegate
                {
                    if (vProcessFpsOverlayer == null || (vProcessFpsOverlayer != null && fpsTextPosition != 0))
                    {
                        grid_Message_Status.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        grid_Message_Status_Stackpanel.SetValue(Grid.ColumnProperty, 0);
                        grid_Message_Status_Rectangle.SetValue(Grid.ColumnProperty, 1);
                    }
                    else
                    {
                        grid_Message_Status.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                        grid_Message_Status_Stackpanel.SetValue(Grid.ColumnProperty, 1);
                        grid_Message_Status_Rectangle.SetValue(Grid.ColumnProperty, 0);
                    }
                });
            }
            catch { }
        }

        //Update the battery status position
        public void UpdateBatteryPosition()
        {
            try
            {
                //Check current fps overlay position
                int fpsTextPosition = Convert.ToInt32(vConfigurationFpsOverlayer.AppSettings.Settings["TextPosition"].Value);
                //Debug.WriteLine("Fps overlayer text position: " + fpsTextPosition);

                //Move the battery status position
                AVActions.ActionDispatcherInvoke(delegate
                {
                    if (vProcessFpsOverlayer == null || (vProcessFpsOverlayer != null && fpsTextPosition != 6))
                    {
                        stackpanel_Battery_Warning.VerticalAlignment = VerticalAlignment.Bottom;
                    }
                    else
                    {
                        stackpanel_Battery_Warning.VerticalAlignment = VerticalAlignment.Top;
                    }
                });
            }
            catch { }
        }
    }
}