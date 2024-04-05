﻿using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVInputOutputInterop;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVWindowFunctions;
using static DirectXInput.AppVariables;

namespace DirectXInput.KeyboardCode
{
    partial class WindowKeyboard
    {
        //Move the keyboard window based on thumb movement
        public void MoveKeyboardWindow(int mouseHorizontal, int mouseVertical)
        {
            try
            {
                //Check the mouse movement position
                if (mouseHorizontal == 0 && mouseVertical == 0) { return; }

                //Get the current window position
                GetWindowRect(vInteropWindowHandle, out WindowRectangle positionRect);
                int moveLeft = positionRect.Left + mouseHorizontal;
                int moveTop = positionRect.Top + mouseVertical;
                int moveRight = positionRect.Right + mouseHorizontal;
                int moveBottom = positionRect.Bottom + mouseVertical;

                //Get the current active screen
                int monitorNumber = SettingLoad(vConfigurationCtrlUI, "DisplayMonitor", typeof(int));
                DisplayMonitor displayMonitorSettings = GetSingleMonitorEnumDisplay(monitorNumber);

                //Get the current window size
                int windowWidth = (int)(this.ActualWidth * displayMonitorSettings.DpiScaleHorizontal);
                int windowHeight = (int)(this.ActualHeight * displayMonitorSettings.DpiScaleVertical);

                //Check if window leaves screen
                double screenEdgeLeft = moveLeft + windowWidth;
                double screenLimitLeft = displayMonitorSettings.BoundsLeft + 20;
                double screenEdgeTop = moveTop + windowHeight;
                double screenLimitTop = displayMonitorSettings.BoundsTop + 20;
                double screenEdgeRight = moveRight - windowWidth;
                double screenLimitRight = displayMonitorSettings.BoundsRight - 20;
                double screenEdgeBottom = moveBottom - windowHeight;
                double screenLimitBottom = displayMonitorSettings.BoundsBottom - 20;
                if (screenEdgeLeft > screenLimitLeft && screenEdgeTop > screenLimitTop && screenEdgeRight < screenLimitRight && screenEdgeBottom < screenLimitBottom)
                {
                    WindowMove(vInteropWindowHandle, moveLeft, moveTop);
                }
            }
            catch { }
        }

        //Move mouse cursor to target
        void MoveMousePosition()
        {
            try
            {
                //Get the current active screen
                int monitorNumber = SettingLoad(vConfigurationCtrlUI, "DisplayMonitor", typeof(int));
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
    }
}