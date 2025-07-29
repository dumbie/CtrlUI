using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVWindowFunctions;
using static ArnoldVinkStyles.AVDispatcherInvoke;
using static ArnoldVinkStyles.AVFocus;
using static CtrlUI.AppVariables;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Update window on resolution change
        public async void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                //Wait for resolution change
                await Task.Delay(2000);

                //Update window style
                WindowUpdateStyle(vWindowMain.GetHandle(), true, false, false, false);

                //Update window position
                await UpdateWindowPosition(true);
            }
            catch { }
        }

        //Update window position
        async Task UpdateWindowPosition(bool skipNotification)
        {
            try
            {
                //Get the current active screen
                int monitorNumber = SettingLoad(vConfigurationCtrlUI, "DisplayMonitor", typeof(int));
                DisplayMonitor displayMonitorSettings = GetSingleMonitorEnumDisplay(monitorNumber);

                //Resize the window size
                double appWindowSize = SettingLoad(vConfigurationCtrlUI, "AppWindowSize", typeof(double)) / 100;
                int windowWidth = Convert.ToInt32(displayMonitorSettings.WidthNative * appWindowSize);
                int windowHeight = Convert.ToInt32(displayMonitorSettings.HeightNative * appWindowSize);
                WindowResize(vWindowMain.GetHandle(), windowWidth, windowHeight);

                //Center the window on target screen
                int horizontalLeft = (int)(displayMonitorSettings.BoundsLeft + (displayMonitorSettings.WidthNative - windowWidth) / 2);
                int verticalTop = (int)(displayMonitorSettings.BoundsTop + (displayMonitorSettings.HeightNative - windowHeight) / 2);
                WindowMove(vWindowMain.GetHandle(), horizontalLeft, verticalTop);

                //Show monitor change notification
                if (!skipNotification)
                {
                    await Notification_Show_Status("MonitorNext", "Moved to monitor " + monitorNumber);
                }

                Debug.WriteLine("Moved the application to monitor: " + monitorNumber);
            }
            catch { }
        }

        //Update the window status
        async Task UpdateWindowStatus()
        {
            try
            {
                vProcessDirectXInput = Get_ProcessesMultiByName("DirectXInput", true).FirstOrDefault();
                int focusedProcessId = Detail_ProcessIdByWindowHandle(GetForegroundWindow());

                await DispatcherInvoke(this.Dispatcher, async delegate
                {
                    try
                    {
                        if (vWindowMain.IsMinimized) { vAppMinimized = true; } else { vAppMinimized = false; }
                        if (vProcessCurrent.Identifier == focusedProcessId)
                        {
                            await AppWindowActivated();
                        }
                        else
                        {
                            AppWindowDeactivated();
                        }
                    }
                    catch { }
                });
            }
            catch { }
        }

        //Application window activated event
        async Task AppWindowActivated()
        {
            try
            {
                if (!vAppActivated)
                {
                    vAppActivated = true;
                    Debug.WriteLine("Activated the application.");

                    //Enable application window
                    AppWindowEnable();

                    //Update window position
                    await UpdateWindowPosition(true);

                    //Resume ScrollViewerLoops
                    PauseResumeScrollviewerLoops(false);

                    //Prevent monitor sleep
                    UpdateMonitorSleepAuto();

                    //Check keyboard focus
                    await FocusCheckKeyboard(this, vProcessCurrent.WindowHandleMain);
                }
            }
            catch { }
        }

        //Application window deactivated event
        void AppWindowDeactivated()
        {
            try
            {
                if (vAppActivated)
                {
                    vAppActivated = false;
                    Debug.WriteLine("Deactivated the application.");

                    //Disable application window
                    AppWindowDisable("Application window is not activated.");

                    //Pause ScrollViewerLoops
                    PauseResumeScrollviewerLoops(true);

                    //Allow monitor sleep
                    UpdateMonitorSleepAllow();
                }
            }
            catch { }
        }

        //Enable application window
        public void AppWindowEnable()
        {
            try
            {
                DispatcherInvoke(this.Dispatcher, delegate
                {
                    //Update window status message
                    textblock_DisableMain.Text = string.Empty;

                    //Enable main menu buttons
                    MainMenuButtonsEnable(false);

                    //Enable the application window
                    grid_ControllerHelp_Content.Opacity = 1.00;
                    grid_DisableMain.Visibility = Visibility.Collapsed;
                });
            }
            catch { }
        }

        //Disable application window
        public void AppWindowDisable(string windowText)
        {
            try
            {
                DispatcherInvoke(this.Dispatcher, delegate
                {
                    //Update window status message
                    textblock_DisableMain.Text = windowText;

                    //Disable main menu buttons
                    MainMenuButtonsDisable();

                    //Disable the application window
                    grid_ControllerHelp_Content.Opacity = 0.10;
                    grid_DisableMain.Visibility = Visibility.Visible;
                });
            }
            catch { }
        }

        //Hide or recover the CtrlUI application
        async Task AppWindow_HideShow()
        {
            try
            {
                Debug.WriteLine("Show or hide the CtrlUI window.");
                if (vAppMinimized || !vAppActivated)
                {
                    //Focus on CtrlUI window
                    await AppWindowShow(false, false);
                }
                else
                {
                    //Minimize CtrlUI window
                    await AppWindowMinimize(false, false);
                }
            }
            catch
            {
                await Notification_Show_Status("Close", "Failed to minimize or show CtrlUI");
                Debug.WriteLine("Failed to minimize or show CtrlUI.");
            }
        }

        //Show the CtrlUI window
        async Task AppWindowShow(bool skipNotification, bool skipSound)
        {
            try
            {
                Debug.WriteLine("Showing the CtrlUI window.");

                //Play maximize sound
                if (!skipSound)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false, false);
                }

                //Hide foreground window
                //await Hide_ProcessByWindowHandle(GetForegroundWindow());

                //Focus on CtrlUI window
                await ShowProcessWindow("CtrlUI", vProcessCurrent.WindowHandleMain, false, skipNotification, false);

                //Update window style
                WindowUpdateStyle(vWindowMain.GetHandle(), true, false, false, false);

                //Update window position
                await UpdateWindowPosition(true);

                //Move mouse cursor to target
                MoveMousePosition();
            }
            catch { }
        }

        //Minimize the CtrlUI window
        async Task AppWindowMinimize(bool minimizeDelay, bool skipNotification)
        {
            try
            {
                Debug.WriteLine("Minimizing CtrlUI window.");

                //Save window state
                vAppMinimized = true;

                //Minimize application
                vWindowMain.Minimize();

                //Play minimize sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false, false);

                //Show minimize notification
                if (!skipNotification)
                {
                    await Notification_Show_Status("AppMinimize", "Hiding CtrlUI");
                }

                //Wait for application to minimize
                if (minimizeDelay)
                {
                    await Task.Delay(500);
                }
            }
            catch { }
        }
    }
}