using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVInteropDll;
using static DirectXInput.AppVariables;
using static LibraryShared.FocusFunctions;
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

        //Hide the window
        public new async Task Hide()
        {
            try
            {
                if (vWindowVisible)
                {
                    //Stop the update tasks
                    await TasksBackgroundStop();

                    //Delay CtrlUI output
                    vController0.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayMediumTicks;
                    vController1.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayMediumTicks;
                    vController2.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayMediumTicks;
                    vController3.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayMediumTicks;

                    //Play window close sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false);

                    //Update the window visibility
                    UpdateWindowVisibility(false);
                }
            }
            catch { }
        }

        //Show the window
        public new async Task Show()
        {
            try
            {
                if (vInteropWindowHandle == IntPtr.Zero)
                {
                    //Get interop window handle
                    vInteropWindowHandle = new WindowInteropHelper(this).EnsureHandle();

                    //Make window able to drag from border
                    this.MouseDown += Window_MouseDown;

                    //Check if resolution has changed
                    SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

                    //Show the window
                    base.Show();

                    //Update the window style
                    UpdateWindowStyle();

                    //Update the user interface clock style
                    UpdateClockStyle();
                }

                //Close other popups
                App.vWindowKeyboard.Hide();
                await App.vWindowKeypad.Hide();

                //Delay media input
                vControllerDelay_Media = GetSystemTicksMs() + vControllerDelayMediumTicks;

                //Play window open sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false);

                //Start the update tasks
                TasksBackgroundStart();

                //Check mouse cursor position
                CheckMousePosition();

                //Focus on keyboard button
                await FocusPopupButton(false, button_PlayPause);

                //Update the window position
                UpdateWindowPosition();

                //Update the window visibility
                UpdateWindowVisibility(true);
            }
            catch { }
        }

        //Drag the window around
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
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
        public async void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                //Wait for change to complete
                await Task.Delay(500);

                //Check mouse cursor position
                CheckMousePosition();

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

                    this.Title = "DirectXInput Media (Visible)";
                    vWindowVisible = true;
                    Debug.WriteLine("Showing the window.");
                }
                else
                {
                    IntPtr updatedStyle = IntPtr.Zero;
                    SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_STYLE, updatedStyle);

                    this.Title = "DirectXInput Media (Hidden)";
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
                IntPtr updatedExStyle = new IntPtr((uint)(WindowStylesEx.WS_EX_TOPMOST | WindowStylesEx.WS_EX_NOACTIVATE));
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
                if (forceFocus || Keyboard.FocusedElement == null || Keyboard.FocusedElement == this)
                {
                    await FrameworkElementFocus(targetButton, false, vInteropWindowHandle);
                }
            }
            catch { }
        }
    }
}