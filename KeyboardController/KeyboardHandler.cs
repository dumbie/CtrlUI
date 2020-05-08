using System;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVInteropDll;
using static KeyboardController.AppVariables;

namespace KeyboardController
{
    partial class WindowMain
    {
        void MoveKeyboardWindow(int thumbHorizontal, int thumbVertical)
        {
            try
            {
                //Check the thumb movement
                int smallOffset = 2500;

                thumbVertical = -thumbVertical;
                int absHorizontal = Math.Abs(thumbHorizontal);
                int absVertical = Math.Abs(thumbVertical);

                if (absHorizontal > smallOffset || absVertical > smallOffset)
                {
                    double mouseSensitivity = 0.0007;
                    int mouseHorizontal = Convert.ToInt32(thumbHorizontal * mouseSensitivity);
                    int mouseVertical = Convert.ToInt32(thumbVertical * mouseSensitivity);

                    //Get the current window position
                    WindowRectangle positionRect = new WindowRectangle();
                    GetWindowRect(vInteropWindowHandle, ref positionRect);
                    int moveLeft = positionRect.Left + mouseHorizontal;
                    int moveTop = positionRect.Top + mouseVertical;
                    int moveRight = positionRect.Right + mouseHorizontal;
                    int moveBottom = positionRect.Bottom + mouseVertical;

                    //Get the current active screen
                    int monitorNumber = Convert.ToInt32(vConfigurationCtrlUI.AppSettings.Settings["DisplayMonitor"].Value);
                    DisplayMonitorSettings displayMonitorSettings = GetScreenSettings(monitorNumber);

                    //Check if window leaves screen
                    double screenEdgeLeft = moveLeft + this.ActualWidth;
                    double screenLimitLeft = displayMonitorSettings.BoundsLeft + 20;
                    double screenEdgeTop = moveTop + this.ActualHeight;
                    double screenLimitTop = displayMonitorSettings.BoundsTop + 20;
                    double screenEdgeRight = moveRight - this.ActualWidth;
                    double screenLimitRight = displayMonitorSettings.BoundsRight - 20;
                    double screenEdgeBottom = moveBottom - this.ActualHeight;
                    double screenLimitBottom = displayMonitorSettings.BoundsBottom - 20;
                    if (screenEdgeLeft > screenLimitLeft && screenEdgeTop > screenLimitTop && screenEdgeRight < screenLimitRight && screenEdgeBottom < screenLimitBottom)
                    {
                        SetWindowPos(vInteropWindowHandle, IntPtr.Zero, moveLeft, moveTop, 0, 0, (int)WindowSWP.NOSIZE);
                    }
                }
            }
            catch { }
        }
    }
}