using System;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVInteropDll;
using static DirectXInput.AppVariables;
using static LibraryShared.Settings;

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
                WindowRectangle positionRect = new WindowRectangle();
                GetWindowRect(vInteropWindowHandle, ref positionRect);
                int moveLeft = positionRect.Left + mouseHorizontal;
                int moveTop = positionRect.Top + mouseVertical;
                int moveRight = positionRect.Right + mouseHorizontal;
                int moveBottom = positionRect.Bottom + mouseVertical;

                //Get the current active screen
                int monitorNumber = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "DisplayMonitor"));
                DisplayMonitorSettings displayMonitorSettings = GetScreenSettings(monitorNumber);

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
    }
}