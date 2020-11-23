using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVInterface;
using static ArnoldVinkCode.AVInteropDll;
using static DirectXInput.AppVariables;
using static LibraryShared.Settings;
using static LibraryShared.SoundPlayer;

namespace DirectXInput.MediaCode
{
    public partial class WindowMedia : Window
    {
        //Window Initialize
        public WindowMedia() { InitializeComponent(); }

        //Window Variables
        private IntPtr vInteropWindowHandle = IntPtr.Zero;
        public bool vWindowVisible = false;

        //Window Initialized
        protected override async void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Get interop window handle
                vInteropWindowHandle = new WindowInteropHelper(this).EnsureHandle();

                //Update the window style
                UpdateWindowStyle();

                //Update the window position
                UpdateWindowPosition();

                //Check mouse cursor position
                CheckMousePosition();

                //Focus on popup button
                await FocusPopupButton(true, button_PlayPause);

                //Make window able to drag from border
                this.MouseDown += WindowMedia_MouseDown;

                //Check if resolution has changed
                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            }
            catch { }
        }

        //Hide the keyboard window
        async public new Task Hide()
        {
            try
            {
                if (vWindowVisible)
                {
                    //Stop the media update task
                    await AVActions.TaskStopLoop(vTask_UpdateMediaInformation);

                    //Delay CtrlUI output
                    vController0.Delay_CtrlUIOutput = Environment.TickCount + vControllerDelayMediumTicks;
                    vController1.Delay_CtrlUIOutput = Environment.TickCount + vControllerDelayMediumTicks;
                    vController2.Delay_CtrlUIOutput = Environment.TickCount + vControllerDelayMediumTicks;
                    vController3.Delay_CtrlUIOutput = Environment.TickCount + vControllerDelayMediumTicks;

                    //Play window close sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false);

                    //Update the popup opacity
                    this.Opacity = 0;

                    //Update the popup visibility
                    this.Title = "DirectXInput Media (Hidden)";
                    vWindowVisible = false;
                    Debug.WriteLine("Hiding the media window.");
                }
            }
            catch { }
        }

        //Show the keyboard window
        public new async Task Show()
        {
            try
            {
                //Close other popups
                App.vWindowKeyboard.Hide();
                await App.vWindowKeypad.Hide();

                //Delay media input
                vControllerDelay_Media = Environment.TickCount + vControllerDelayMediumTicks;

                //Play window open sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false);

                //Start the media update task
                AVActions.TaskStartLoop(vTaskLoop_UpdateMediaInformation, vTask_UpdateMediaInformation);

                //Check mouse cursor position
                CheckMousePosition();

                //Focus on keyboard button
                await FocusPopupButton(false, button_Close);

                //Update the window style (focus workaround)
                UpdateWindowStyle();

                //Update the popup opacity
                UpdatePopupOpacity(true);

                //Update the window position
                UpdateWindowPosition();

                //Update the keyboard visibility
                this.Title = "DirectXInput Media (Visible)";
                this.Visibility = Visibility.Visible;
                vWindowVisible = true;
                Debug.WriteLine("Showing the media window.");
            }
            catch { }
        }

        //Update popup opacity
        public void UpdatePopupOpacity(bool forceUpdate)
        {
            try
            {
                if (forceUpdate || this.Opacity != 0)
                {
                    this.Opacity = 1;
                }
            }
            catch { }
        }

        //Drag the window around
        private void WindowMedia_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
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
                IntPtr UpdatedExStyle = new IntPtr((uint)(WindowStylesEx.WS_EX_TOPMOST | WindowStylesEx.WS_EX_NOACTIVATE));
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
                //Get the current active screen
                int monitorNumber = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "DisplayMonitor"));
                DisplayMonitorSettings displayMonitorSettings = GetScreenSettings(monitorNumber);

                //Get the current window size
                int windowWidth = (int)(this.ActualWidth * displayMonitorSettings.DpiScaleHorizontal);
                int windowHeight = (int)(this.ActualHeight * displayMonitorSettings.DpiScaleVertical);

                //Move the window to screen center
                int horizontalLeft = (int)(displayMonitorSettings.BoundsLeft + (displayMonitorSettings.WidthNative - windowWidth) / 2);
                int verticalTop = (int)(displayMonitorSettings.BoundsTop + (displayMonitorSettings.HeightNative - windowHeight) / 2);
                WindowMove(vInteropWindowHandle, horizontalLeft, verticalTop);
            }
            catch { }
        }

        //Activate keyboard window
        void CheckMousePosition()
        {
            try
            {
                //Get the current active screen
                int monitorNumber = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "DisplayMonitor"));
                DisplayMonitorSettings displayMonitorSettings = GetScreenSettings(monitorNumber);

                //Get the current mouse position
                GetCursorPos(out PointWin previousCursorPosition);

                //Check if mouse cursor is in window
                int windowHeight = (int)this.Height;
                int displayCenterTop = (int)(displayMonitorSettings.BoundsTop + (displayMonitorSettings.HeightNative - windowHeight) / 2);
                int displayCenterBottom = displayCenterTop + windowHeight;
                if (AVFunctions.BetweenNumbers(previousCursorPosition.Y, displayCenterTop, displayCenterBottom, true))
                {
                    previousCursorPosition.Y = displayCenterTop - 20;
                    SetCursorPos(previousCursorPosition.X, previousCursorPosition.Y);
                }
            }
            catch { }
        }

        //Focus on popup button
        async Task FocusPopupButton(bool forceFocus, Button targetButton)
        {
            try
            {
                if (forceFocus || Keyboard.FocusedElement == null)
                {
                    await FocusOnElement(targetButton, false, vInteropWindowHandle);
                }
            }
            catch { }
        }
    }
}