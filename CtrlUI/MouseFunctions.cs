using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVInputOutputInterop;
using static ArnoldVinkCode.AVSettings;
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