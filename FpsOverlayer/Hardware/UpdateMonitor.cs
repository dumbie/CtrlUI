using ArnoldVinkCode;
using System.Windows;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVSettings;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowMain
    {
        void UpdateMonitorInformation()
        {
            try
            {
                //Check if the information is visible
                bool showResolution = SettingLoad(vConfigurationFpsOverlayer, "MonShowResolution", typeof(bool));
                bool showDpiResolution = SettingLoad(vConfigurationFpsOverlayer, "MonShowDpiResolution", typeof(bool));
                bool showColorBitDepth = SettingLoad(vConfigurationFpsOverlayer, "MonShowColorBitDepth", typeof(bool));
                bool showRefreshRate = SettingLoad(vConfigurationFpsOverlayer, "MonShowRefreshRate", typeof(bool));
                if (!showResolution && !showColorBitDepth && !showRefreshRate)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        stackpanel_CurrentMon.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                //Get the current active screen
                int monitorNumber = SettingLoad(vConfigurationCtrlUI, "DisplayMonitor", typeof(int));

                //Get the screen resolution
                DisplayMonitor displayMonitorSettings = GetSingleMonitorEnumDisplay(monitorNumber);
                string screenResolutionString = string.Empty;
                if (showResolution)
                {
                    if (showDpiResolution)
                    {
                        screenResolutionString = " " + displayMonitorSettings.WidthDpi + "x" + displayMonitorSettings.HeightDpi;
                    }
                    else
                    {
                        screenResolutionString = " " + displayMonitorSettings.WidthNative + "x" + displayMonitorSettings.HeightNative;
                    }
                }

                //Get the screen color bit depth
                string screenColorBitDepthString = string.Empty;
                if (showColorBitDepth)
                {
                    screenColorBitDepthString = " " + displayMonitorSettings.BitDepth + "Bits";
                }

                //Get the screen refresh rate
                string screenRefreshRateString = string.Empty;
                if (showRefreshRate)
                {
                    int screenRefreshRateInt = displayMonitorSettings.RefreshRate;
                    if (screenRefreshRateInt > 0)
                    {
                        if (showResolution || showColorBitDepth)
                        {
                            screenRefreshRateString = " @ " + screenRefreshRateInt + "Hz";
                        }
                        else
                        {
                            screenRefreshRateString = " " + screenRefreshRateInt + "Hz";
                        }
                    }
                }

                //Update the screen resolution
                string stringDisplay = AVFunctions.StringRemoveStart(vTitleMON + screenResolutionString + screenColorBitDepthString + screenRefreshRateString, " ");
                AVActions.DispatcherInvoke(delegate
                {
                    textblock_CurrentMon.Text = stringDisplay;
                    stackpanel_CurrentMon.Visibility = Visibility.Visible;
                });
            }
            catch { }
        }
    }
}