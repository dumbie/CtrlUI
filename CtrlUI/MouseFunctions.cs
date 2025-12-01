using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVInputOutputInterop;
using static ArnoldVinkCode.AVInteropDll;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Move mouse cursor to target
        void MoveMousePosition()
        {
            try
            {
                //Get current active screen
                int monitorNumber = vSettings.Load("DisplayMonitor", typeof(int));
                DisplayMonitor displayMonitorSettings = GetSingleMonitorEnumDisplay(monitorNumber);

                //Get current window location and size
                WindowRectangle windowLocation = vWindowMain.GetWindowLocationSize();

                //Calculate target mouse position
                int targetWidth = windowLocation.Left + (windowLocation.Width / 2);
                int targetHeight = windowLocation.Top - 30;

                //Check if target is outside screen
                if (targetHeight < 0)
                {
                    targetHeight = windowLocation.Top + windowLocation.Height + 30;
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