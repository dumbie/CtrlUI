using ArnoldVinkCode;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVWindowFunctions;
using static ScreenCapture.AppClasses;
using static ScreenCapture.AppVariables;

namespace ScreenCapture
{
    public partial class WindowOverlay : Window
    {
        //Window Variables
        private IntPtr vInteropWindowHandle = IntPtr.Zero;
        private DispatcherTimer vDispatcherTimerDelay = new DispatcherTimer();
        private CaptureTypes vCaptureType = CaptureTypes.None;
        private int vRecordingTime = 0;

        //Window Initialize
        public WindowOverlay()
        {
            InitializeComponent();
        }

        //Window Show Overlay
        public void ShowOverlay(CaptureTypes captureType)
        {
            try
            {
                vCaptureType = captureType;
                Show();
            }
            catch { }
        }

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

                //Update window visibility
                WindowUpdateVisibility(vInteropWindowHandle, true);

                //Update window style
                WindowUpdateStyle(vInteropWindowHandle, true, true, true);

                //Set window display affinity
                SetWindowDisplayAffinity(vInteropWindowHandle, DisplayAffinityFlags.WDA_EXCLUDEFROMCAPTURE);

                //Update window position
                UpdateWindowPosition();

                //Update overlay capture type
                if (vCaptureType == CaptureTypes.Video)
                {
                    //Show video overlay
                    border_Overlay_Image.Visibility = Visibility.Collapsed;
                    border_Overlay_Video.Visibility = Visibility.Visible;
                    border_Overlay_Failed.Visibility = Visibility.Collapsed;

                    //Start timing update timer
                    vRecordingTime = 0;
                    AVFunctions.TimerRenew(ref vDispatcherTimerDelay);
                    vDispatcherTimerDelay.Interval = TimeSpan.FromMilliseconds(1000);
                    vDispatcherTimerDelay.Tick += VDispatcherTimerDelay_Tick;
                    vDispatcherTimerDelay.Start();
                }
                else if (vCaptureType == CaptureTypes.Image)
                {
                    //Show image overlay
                    border_Overlay_Image.Visibility = Visibility.Visible;
                    border_Overlay_Video.Visibility = Visibility.Collapsed;
                    border_Overlay_Failed.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //Show failed overlay
                    border_Overlay_Image.Visibility = Visibility.Collapsed;
                    border_Overlay_Video.Visibility = Visibility.Collapsed;
                    border_Overlay_Failed.Visibility = Visibility.Visible;
                }
            }
            catch { }
        }

        //Update window position
        private void UpdateWindowPosition()
        {
            try
            {
                string overlayPosition = SettingLoad(vConfiguration, "OverlayPosition", typeof(string));
                if (overlayPosition == "TopLeft")
                {
                    grid_Overlay.VerticalAlignment = VerticalAlignment.Top;
                    grid_Overlay.HorizontalAlignment = HorizontalAlignment.Left;
                }
                else if (overlayPosition == "TopLeft")
                {
                    grid_Overlay.VerticalAlignment = VerticalAlignment.Top;
                    grid_Overlay.HorizontalAlignment = HorizontalAlignment.Left;
                }
                else if (overlayPosition == "TopCenter")
                {
                    grid_Overlay.VerticalAlignment = VerticalAlignment.Top;
                    grid_Overlay.HorizontalAlignment = HorizontalAlignment.Center;
                }
                else if (overlayPosition == "TopRight")
                {
                    grid_Overlay.VerticalAlignment = VerticalAlignment.Top;
                    grid_Overlay.HorizontalAlignment = HorizontalAlignment.Right;
                }
                else if (overlayPosition == "RightCenter")
                {
                    grid_Overlay.VerticalAlignment = VerticalAlignment.Center;
                    grid_Overlay.HorizontalAlignment = HorizontalAlignment.Right;
                }
                else if (overlayPosition == "BottomRight")
                {
                    grid_Overlay.VerticalAlignment = VerticalAlignment.Bottom;
                    grid_Overlay.HorizontalAlignment = HorizontalAlignment.Right;
                }
                else if (overlayPosition == "BottomCenter")
                {
                    grid_Overlay.VerticalAlignment = VerticalAlignment.Bottom;
                    grid_Overlay.HorizontalAlignment = HorizontalAlignment.Center;
                }
                else if (overlayPosition == "BottomLeft")
                {
                    grid_Overlay.VerticalAlignment = VerticalAlignment.Bottom;
                    grid_Overlay.HorizontalAlignment = HorizontalAlignment.Left;
                }
                else if (overlayPosition == "LeftCenter")
                {
                    grid_Overlay.VerticalAlignment = VerticalAlignment.Center;
                    grid_Overlay.HorizontalAlignment = HorizontalAlignment.Left;
                }
            }
            catch { }
        }

        //Update recording time
        private void VDispatcherTimerDelay_Tick(object sender, EventArgs e)
        {
            try
            {
                vRecordingTime++;
                textblock_Timing.Text = AVFunctions.SecondsToHms(vRecordingTime, true, false);
            }
            catch { }
        }
    }
}