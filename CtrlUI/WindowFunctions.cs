using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static CtrlUI.AppVariables;
using static LibraryShared.Settings;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Update window on resolution change
        async void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                //Update the window position
                await UpdateWindowPosition(false, true);
            }
            catch { }
        }

        //Update the window position
        async Task UpdateWindowPosition(bool notifyApps, bool silent)
        {
            try
            {
                //Get the current active screen
                int monitorNumber = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "DisplayMonitor"));
                DisplayMonitor displayMonitorSettings = GetSingleMonitorEnumDisplay(monitorNumber);

                //Resize the window size
                double appWindowSize = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "AppWindowSize"));
                appWindowSize /= 100;
                int windowWidth = Convert.ToInt32(displayMonitorSettings.WidthNative * appWindowSize);
                int windowHeight = Convert.ToInt32(displayMonitorSettings.HeightNative * appWindowSize);
                WindowResize(vInteropWindowHandle, windowWidth, windowHeight);

                //Center the window on target screen
                int horizontalLeft = (int)(displayMonitorSettings.BoundsLeft + (displayMonitorSettings.WidthNative - windowWidth) / 2);
                int verticalTop = (int)(displayMonitorSettings.BoundsTop + (displayMonitorSettings.HeightNative - windowHeight) / 2);
                WindowMove(vInteropWindowHandle, horizontalLeft, verticalTop);

                //Set the window as top most
                SetWindowPos(vInteropWindowHandle, (IntPtr)WindowPosition.TopMost, 0, 0, 0, 0, (int)(WindowSWP.NOMOVE | WindowSWP.NOSIZE));

                //Notify apps the monitor changed
                if (notifyApps)
                {
                    await NotifyDirectXInputSettingChanged("DisplayMonitor");
                    await NotifyFpsOverlayerSettingChanged("DisplayMonitor");
                }

                //Show monitor change notification
                if (!silent)
                {
                    await Notification_Send_Status("MonitorNext", "Moved to monitor " + monitorNumber);
                }

                Debug.WriteLine("Moved the application to monitor: " + monitorNumber);
            }
            catch { }
        }

        //Update the window status
        void UpdateWindowStatus()
        {
            try
            {
                vProcessDirectXInput = GetProcessByNameOrTitle("DirectXInput", false, true);
                int focusedAppId = GetProcessMultiFromWindowHandle(GetForegroundWindow()).Identifier;

                AVActions.ActionDispatcherInvoke(delegate
                {
                    try
                    {
                        if (WindowState == WindowState.Minimized) { vAppMinimized = true; } else { vAppMinimized = false; }
                        if (vProcessCurrent.Id == focusedAppId)
                        {
                            AppWindowActivated();
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
        void AppWindowActivated()
        {
            try
            {
                if (!vAppActivated)
                {
                    vAppActivated = true;
                    Debug.WriteLine("Activated the application.");

                    //Enable application window
                    AppWindowEnable();

                    //Resume ScrollViewerLoops
                    PauseResumeScrollviewerLoops(false);

                    //Prevent monitor sleep
                    UpdateMonitorSleepAuto();
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
        void AppWindowEnable()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Update window status message
                    textblock_DisableMain.Text = string.Empty;

                    //Enable the application window
                    grid_DisableHeader.Visibility = Visibility.Collapsed;
                    grid_DisableHelp.Visibility = Visibility.Collapsed;
                    grid_DisableMain.Visibility = Visibility.Collapsed;
                });
            }
            catch { }
        }

        //Disable application window
        void AppWindowDisable(string windowText)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Update window status message
                    textblock_DisableMain.Text = windowText;

                    //Disable the application window
                    grid_DisableHeader.Visibility = Visibility.Visible;
                    grid_DisableHelp.Visibility = Visibility.Visible;
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

                //Get the current focused application
                ProcessMulti foregroundProcess = GetProcessMultiFromWindowHandle(GetForegroundWindow());

                if (vAppMinimized || !vAppActivated)
                {
                    //Show the CtrlUI window
                    await AppWindowShow(false);
                }
                else
                {
                    //Minimize the CtrlUI window
                    await AppWindowMinimize(false, false);
                }
            }
            catch
            {
                await Notification_Send_Status("Close", "Failed to minimize or show CtrlUI");
                Debug.WriteLine("Failed to minimize or show CtrlUI.");
            }
        }

        //Show the CtrlUI window
        async Task AppWindowShow(bool silentShow)
        {
            try
            {
                Debug.WriteLine("Showing the CtrlUI window.");

                //Play maximize sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false, false);

                //Force focus on CtrlUI
                await PrepareFocusProcessWindow("CtrlUI", vProcessCurrent.Id, vProcessCurrent.MainWindowHandle, 0, false, false, silentShow, false);

                //Update the window position
                await UpdateWindowPosition(false, true);

                //Move mouse cursor to target
                MoveMousePosition();
            }
            catch { }
        }

        //Minimize the CtrlUI window
        async Task AppWindowMinimize(bool minimizeDelay, bool silentMinimize)
        {
            try
            {
                Debug.WriteLine("Minimizing the CtrlUI window.");

                //Save the CtrlUI window state
                vAppMinimized = true;

                //Minimize the CtrlUI application
                WindowState = WindowState.Minimized;

                //Play minimize sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false, false);

                //Show minimize notification
                if (!silentMinimize)
                {
                    await Notification_Send_Status("AppMinimize", "Hiding CtrlUI");
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