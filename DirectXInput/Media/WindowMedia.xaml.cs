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

                //Update the window position
                UpdateWindowPosition();

                //Focus on popup button
                await FocusPopupButton(true, button_PlayPause);

                //Update the user interface clock style
                UpdateClockStyle();

                //Make window able to drag from border
                this.MouseDown += Window_MouseDown;

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
                if (vWindowVisible)
                {
                    //Play window close sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false);

                    //Stop the update tasks
                    await TasksBackgroundStop();

                    //Update the window visibility
                    await UpdateWindowVisibility(false);

                    //Release keyboard and mouse
                    vFakerInputDevice.KeyboardReset();
                    vFakerInputDevice.MouseResetAbsolute();
                    vFakerInputDevice.MouseResetRelative();

                    //Delay CtrlUI output
                    vController0.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayMediumTicks;
                    vController1.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayMediumTicks;
                    vController2.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayMediumTicks;
                    vController3.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayMediumTicks;
                }
            }
            catch { }
        }

        //Show the window
        public new async Task Show()
        {
            try
            {
                //Close other popups
                await App.vWindowKeyboard.Hide();
                await App.vWindowKeypad.Hide();

                //Delay media input
                vControllerDelay_Media = GetSystemTicksMs() + vControllerDelayMediumTicks;

                //Play window open sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false);

                //Start the update tasks
                TasksBackgroundStart();

                //Focus on keyboard button
                await FocusPopupButton(false, button_PlayPause);

                //Update the window visibility
                await UpdateWindowVisibility(true);

                //Move mouse cursor to target
                MoveMousePosition();
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

                    this.Title = "DirectXInput Media (Visible)";
                    vWindowVisible = true;
                    Debug.WriteLine("Showing the window.");
                }
                else
                {
                    //Update the window style
                    await UpdateWindowStyleHidden();

                    this.Title = "DirectXInput Media (Hidden)";
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
                    IntPtr updatedExStyle = new IntPtr((uint)(WindowStylesEx.WS_EX_TOPMOST | WindowStylesEx.WS_EX_NOACTIVATE));
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

        //Move mouse cursor to target
        void MoveMousePosition()
        {
            try
            {
                //Get the current active screen
                int monitorNumber = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "DisplayMonitor"));
                DisplayMonitor displayMonitorSettings = GetSingleMonitorEnumDisplay(monitorNumber);

                //Calculate target mouse position
                int windowTop = (int)(this.Top * displayMonitorSettings.DpiScaleVertical);
                int windowLeft = (int)(this.Left * displayMonitorSettings.DpiScaleHorizontal);
                int windowWidth = (int)(this.ActualWidth * displayMonitorSettings.DpiScaleHorizontal);
                int windowHeight = (int)(this.ActualHeight * displayMonitorSettings.DpiScaleVertical);
                int targetWidth = windowLeft + (windowWidth / 2);
                int targetHeight = windowTop - 30;

                //Check if target is outside screen
                if (targetHeight < 0)
                {
                    targetHeight = windowTop + windowHeight + 30;
                }
                if (targetWidth < 0)
                {
                    targetWidth = 30;
                }
                else if (targetWidth > displayMonitorSettings.WidthNative)
                {
                    targetWidth = displayMonitorSettings.WidthNative - 30;
                }

                //Move mouse cursor to target
                SetCursorPos(targetWidth, targetHeight);
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