using System;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVInteropDll;
using static DirectXInput.AppVariables;

namespace DirectXInput.Keyboard
{
    partial class WindowKeyboard
    {
        //Get the mouse movement amount based on thumb movement
        public void GetMouseMovementAmountFromThumb(int thumbSensitivity, int thumbHorizontal, int thumbVertical, bool flipVertical, out int mouseHorizontal, out int mouseVertical)
        {
            mouseHorizontal = 0;
            mouseVertical = 0;
            try
            {
                //Check the thumb movement
                if (flipVertical) { thumbVertical = -thumbVertical; }
                int absHorizontal = Math.Abs(thumbHorizontal);
                int absVertical = Math.Abs(thumbVertical);

                if (absHorizontal > vControllerOffsetNormal || absVertical > vControllerOffsetNormal)
                {
                    double mouseSensitivity = (double)thumbSensitivity / (double)15000;
                    mouseHorizontal = Convert.ToInt32(thumbHorizontal * mouseSensitivity);
                    mouseVertical = Convert.ToInt32(thumbVertical * mouseSensitivity);
                }
                else if (absHorizontal > vControllerOffsetSmall || absVertical > vControllerOffsetSmall)
                {
                    double mouseSensitivity = (double)thumbSensitivity / (double)30000;
                    mouseHorizontal = Convert.ToInt32(thumbHorizontal * mouseSensitivity);
                    mouseVertical = Convert.ToInt32(thumbVertical * mouseSensitivity);
                }
            }
            catch { }
        }

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
                int monitorNumber = Convert.ToInt32(vConfigurationCtrlUI.AppSettings.Settings["DisplayMonitor"].Value);
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