using ArnoldVinkCode;
using System;
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
                    int horizontalCenter = positionRect.Left + mouseHorizontal;
                    int verticalCenter = positionRect.Top + mouseVertical;

                    //Get the current active screen
                    int monitorNumber = Convert.ToInt32(vConfigurationCtrlUI.AppSettings.Settings["DisplayMonitor"].Value);
                    AVDisplayMonitor.GetScreenResolution(monitorNumber, out int screenWidth, out int screenHeight, out float dpiScale);
                    AVDisplayMonitor.GetScreenBounds(monitorNumber, out int boundsLeft, out int boundsTop);

                    //Check if window leaves screen
                    double screenEdgeLeft = horizontalCenter + this.ActualWidth;
                    double screenEdgeTop = verticalCenter + this.ActualHeight;
                    double screenEdgeRight = screenWidth - horizontalCenter;
                    double screenEdgeBottom = screenHeight - verticalCenter;

                    if (screenEdgeLeft > (boundsLeft + 20) && screenEdgeTop > (boundsTop + 20) && screenEdgeRight > 20 && screenEdgeBottom > 20)
                    {
                        SetWindowPos(vInteropWindowHandle, IntPtr.Zero, horizontalCenter, verticalCenter, 0, 0, (int)WindowSWP.NOSIZE);
                    }
                }
            }
            catch { }
        }
    }
}