using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVInteropDll;
using static DirectXInput.AppVariables;
using static LibraryShared.ImageFunctions;
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

                //Update the battery icon and text position
                UpdateBatteryPosition();

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
                Screen targetScreen = GetScreenByNumber(monitorNumber, out bool monitorSuccess);

                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Set the window size
                    this.Width = targetScreen.Bounds.Width;
                    this.Height = targetScreen.Bounds.Height;

                    //Move the window top left
                    this.Left = targetScreen.Bounds.Left;
                    this.Top = targetScreen.Bounds.Top;
                });
            }
            catch { }
        }

        //Update the battery icon and text position
        public void UpdateBatteryPosition()
        {
            try
            {
                //Load current fps overlay settings
                Settings_Load_FpsOverlayer(ref vConfigurationFpsOverlayer);

                //Check current fps overlay position
                int fpsTextPosition = Convert.ToInt32(vConfigurationFpsOverlayer.AppSettings.Settings["TextPosition"].Value);
                //Debug.WriteLine("Fps overlayer text position: " + fpsTextPosition);

                //Move the battery icon and text
                AVActions.ActionDispatcherInvoke(delegate
                {
                    if (fpsTextPosition == 0 || fpsTextPosition == 1 || fpsTextPosition == 2 || fpsTextPosition == 3 || fpsTextPosition == 7)
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

        //Show the status overlay
        public void Overlay_Show_Status(string IconName, string Message)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    grid_Message_Status_Image.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/" + IconName + ".png" }, IntPtr.Zero, -1, 0);
                    grid_Message_Status_Text.Text = Message;
                    grid_Message_Status.Visibility = Visibility.Visible;
                });

                vDispatcherTimerOverlay.Stop();
                vDispatcherTimerOverlay.Interval = TimeSpan.FromSeconds(3);
                vDispatcherTimerOverlay.Tick += delegate
                {
                    grid_Message_Status.Visibility = Visibility.Collapsed;
                    vDispatcherTimerOverlay.Stop();
                };
                vDispatcherTimerOverlay.Start();
            }
            catch { }
        }
    }
}
