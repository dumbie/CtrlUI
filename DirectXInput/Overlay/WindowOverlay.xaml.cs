using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Diagnostics;
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
        public static IntPtr vInteropWindowHandle = IntPtr.Zero;

        //Window Initialized
        protected override void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Get interop window handle
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
                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            }
            catch { }
        }

        //Update the window position on resolution change
        public void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
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
                vConfigurationCtrlUI = Settings_Load_CtrlUI();

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

        //Update the battery status position
        public void UpdateBatteryPosition()
        {
            try
            {
                //Check current fps overlay position
                OverlayPosition fpsTextPosition = (OverlayPosition)Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "TextPosition"));
                //Debug.WriteLine("Fps overlayer text position: " + fpsTextPosition);

                //Move the battery status position
                AVActions.ActionDispatcherInvoke(delegate
                {
                    if (vProcessFpsOverlayer == null || (vProcessFpsOverlayer != null && fpsTextPosition != OverlayPosition.BottomLeft))
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