using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVInteropDll;
using static KeyboardController.AppVariables;

namespace KeyboardController
{
    partial class WindowMain
    {
        void MoveKeyboardWindow(int ThumbHorizontal, int ThumbVertical)
        {
            try
            {
                //Check the thumb movement
                int SmallOffset = 2500;

                ThumbVertical = -ThumbVertical;
                int AbsHorizontal = Math.Abs(ThumbHorizontal);
                int AbsVertical = Math.Abs(ThumbVertical);

                if (AbsHorizontal > SmallOffset || AbsVertical > SmallOffset)
                {
                    double MouseSensitivity = 0.0007;
                    int MouseHorizontal = Convert.ToInt32(ThumbHorizontal * MouseSensitivity);
                    int MouseVertical = Convert.ToInt32(ThumbVertical * MouseSensitivity);

                    //Get the current window handle
                    IntPtr WindowHandle = Process.GetCurrentProcess().MainWindowHandle;

                    //Get the current window position
                    WindowRectangle PositionRect = new WindowRectangle();
                    GetWindowRect(WindowHandle, ref PositionRect);
                    int HorizontalCenter = PositionRect.Left + MouseHorizontal;
                    int VerticalCenter = PositionRect.Top + MouseVertical;

                    //Get the current active screen
                    int monitorNumber = Convert.ToInt32(vConfigurationCtrlUI.AppSettings.Settings["DisplayMonitor"].Value);
                    Screen targetScreen = GetScreenByNumber(monitorNumber, out bool monitorSuccess);

                    //Check if window leaves screen
                    Rectangle targetRectangle = targetScreen.WorkingArea;
                    double ScreenEdgeLeft = HorizontalCenter + this.ActualWidth;
                    double ScreenEdgeTop = VerticalCenter + this.ActualHeight;
                    double ScreenEdgeRight = targetRectangle.Right - HorizontalCenter;
                    double ScreenEdgeBottom = targetRectangle.Bottom - VerticalCenter;
                    if (ScreenEdgeLeft > 20 && ScreenEdgeTop > 20 && ScreenEdgeRight > 20 && ScreenEdgeBottom > 40)
                    {
                        SetWindowPos(WindowHandle, IntPtr.Zero, HorizontalCenter, VerticalCenter, 0, 0, (int)WindowSWP.NOSIZE);
                    }
                }
            }
            catch { }
        }
    }
}