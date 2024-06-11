using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVSettings;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowSettings
    {
        //Check - Application Settings
        public void Settings_Check()
        {
            try
            {
                if (!SettingCheck(vConfigurationFpsOverlayer, "DisplayBackground")) { SettingSave(vConfigurationFpsOverlayer, "DisplayBackground", "False"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "DisplayOpacity")) { SettingSave(vConfigurationFpsOverlayer, "DisplayOpacity", "0,90"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "MarginHorizontal")) { SettingSave(vConfigurationFpsOverlayer, "MarginHorizontal", "5"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "MarginVertical")) { SettingSave(vConfigurationFpsOverlayer, "MarginVertical", "5"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "CheckTaskbarVisible")) { SettingSave(vConfigurationFpsOverlayer, "CheckTaskbarVisible", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "StatsFlipBottom")) { SettingSave(vConfigurationFpsOverlayer, "StatsFlipBottom", "False"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "HideScreenCapture")) { SettingSave(vConfigurationFpsOverlayer, "HideScreenCapture", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "InterfaceFontStyleName")) { SettingSave(vConfigurationFpsOverlayer, "InterfaceFontStyleName", "Segoe UI"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "TextPosition")) { SettingSave(vConfigurationFpsOverlayer, "TextPosition", "0"); } //Shared
                if (!SettingCheck(vConfigurationFpsOverlayer, "TextDirection")) { SettingSave(vConfigurationFpsOverlayer, "TextDirection", "1"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "TextSize")) { SettingSave(vConfigurationFpsOverlayer, "TextSize", "18"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "HardwareUpdateRateMs")) { SettingSave(vConfigurationFpsOverlayer, "HardwareUpdateRateMs", "1000"); }

                if (!SettingCheck(vConfigurationFpsOverlayer, "BrowserShowStartup")) { SettingSave(vConfigurationFpsOverlayer, "BrowserShowStartup", "False"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "BrowserUnload")) { SettingSave(vConfigurationFpsOverlayer, "BrowserUnload", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "BrowserOpacity")) { SettingSave(vConfigurationFpsOverlayer, "BrowserOpacity", "0,70"); }

                if (!SettingCheck(vConfigurationFpsOverlayer, "TextColorSingle")) { SettingSave(vConfigurationFpsOverlayer, "TextColorSingle", "False"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "ColorBackground")) { SettingSave(vConfigurationFpsOverlayer, "ColorBackground", "#1D1D1D"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "ColorSingle")) { SettingSave(vConfigurationFpsOverlayer, "ColorSingle", "#F1F1F1"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "ColorGpu")) { SettingSave(vConfigurationFpsOverlayer, "ColorGpu", "#A3FF39"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "ColorCpu")) { SettingSave(vConfigurationFpsOverlayer, "ColorCpu", "#00EAFF"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "ColorMem")) { SettingSave(vConfigurationFpsOverlayer, "ColorMem", "#FFA200"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "ColorFan")) { SettingSave(vConfigurationFpsOverlayer, "ColorFan", "#E7CEB5"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "ColorFps")) { SettingSave(vConfigurationFpsOverlayer, "ColorFps", "#FF0505"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "ColorFrametime")) { SettingSave(vConfigurationFpsOverlayer, "ColorFrametime", "#F1F1F1"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "ColorNet")) { SettingSave(vConfigurationFpsOverlayer, "ColorNet", "#FF00A8"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "ColorApp")) { SettingSave(vConfigurationFpsOverlayer, "ColorApp", "#C000FF"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "ColorBat")) { SettingSave(vConfigurationFpsOverlayer, "ColorBat", "#FFE115"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "ColorTime")) { SettingSave(vConfigurationFpsOverlayer, "ColorTime", "#21AFFF"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "ColorCustomText")) { SettingSave(vConfigurationFpsOverlayer, "ColorCustomText", "#F1F1F1"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "ColorMon")) { SettingSave(vConfigurationFpsOverlayer, "ColorMon", "#21A000"); }

                if (!SettingCheck(vConfigurationFpsOverlayer, "GpuId")) { SettingSave(vConfigurationFpsOverlayer, "GpuId", "4"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "GpuCategoryTitle")) { SettingSave(vConfigurationFpsOverlayer, "GpuCategoryTitle", "GPU"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "GpuShowCategoryTitle")) { SettingSave(vConfigurationFpsOverlayer, "GpuShowCategoryTitle", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "GpuShowName")) { SettingSave(vConfigurationFpsOverlayer, "GpuShowName", "False"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "GpuShowPercentage")) { SettingSave(vConfigurationFpsOverlayer, "GpuShowPercentage", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "GpuShowMemoryUsed")) { SettingSave(vConfigurationFpsOverlayer, "GpuShowMemoryUsed", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "GpuShowTemperature")) { SettingSave(vConfigurationFpsOverlayer, "GpuShowTemperature", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "GpuShowTemperatureHotspot")) { SettingSave(vConfigurationFpsOverlayer, "GpuShowTemperatureHotspot", "False"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "GpuShowMemorySpeed")) { SettingSave(vConfigurationFpsOverlayer, "GpuShowMemorySpeed", "False"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "GpuShowCoreFrequency")) { SettingSave(vConfigurationFpsOverlayer, "GpuShowCoreFrequency", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "GpuShowFanSpeed")) { SettingSave(vConfigurationFpsOverlayer, "GpuShowFanSpeed", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "GpuShowPowerWatt")) { SettingSave(vConfigurationFpsOverlayer, "GpuShowPowerWatt", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "GpuShowPowerVolt")) { SettingSave(vConfigurationFpsOverlayer, "GpuShowPowerVolt", "False"); }

                if (!SettingCheck(vConfigurationFpsOverlayer, "CpuId")) { SettingSave(vConfigurationFpsOverlayer, "CpuId", "3"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "CpuCategoryTitle")) { SettingSave(vConfigurationFpsOverlayer, "CpuCategoryTitle", "CPU"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "CpuShowCategoryTitle")) { SettingSave(vConfigurationFpsOverlayer, "CpuShowCategoryTitle", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "CpuShowName")) { SettingSave(vConfigurationFpsOverlayer, "CpuShowName", "False"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "BoardShowName")) { SettingSave(vConfigurationFpsOverlayer, "BoardShowName", "False"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "CpuShowPercentage")) { SettingSave(vConfigurationFpsOverlayer, "CpuShowPercentage", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "CpuShowTemperature")) { SettingSave(vConfigurationFpsOverlayer, "CpuShowTemperature", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "CpuShowCoreFrequency")) { SettingSave(vConfigurationFpsOverlayer, "CpuShowCoreFrequency", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "CpuShowPowerWatt")) { SettingSave(vConfigurationFpsOverlayer, "CpuShowPowerWatt", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "CpuShowPowerVolt")) { SettingSave(vConfigurationFpsOverlayer, "CpuShowPowerVolt", "False"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "CpuShowFanSpeed")) { SettingSave(vConfigurationFpsOverlayer, "CpuShowFanSpeed", "True"); }

                if (!SettingCheck(vConfigurationFpsOverlayer, "MemId")) { SettingSave(vConfigurationFpsOverlayer, "MemId", "5"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "MemCategoryTitle")) { SettingSave(vConfigurationFpsOverlayer, "MemCategoryTitle", "MEM"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "MemShowCategoryTitle")) { SettingSave(vConfigurationFpsOverlayer, "MemShowCategoryTitle", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "MemShowName")) { SettingSave(vConfigurationFpsOverlayer, "MemShowName", "False"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "MemShowSpeed")) { SettingSave(vConfigurationFpsOverlayer, "MemShowSpeed", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "MemShowPowerVolt")) { SettingSave(vConfigurationFpsOverlayer, "MemShowPowerVolt", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "MemShowPercentage")) { SettingSave(vConfigurationFpsOverlayer, "MemShowPercentage", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "MemShowUsed")) { SettingSave(vConfigurationFpsOverlayer, "MemShowUsed", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "MemShowFree")) { SettingSave(vConfigurationFpsOverlayer, "MemShowFree", "False"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "MemShowTotal")) { SettingSave(vConfigurationFpsOverlayer, "MemShowTotal", "True"); }

                if (!SettingCheck(vConfigurationFpsOverlayer, "NetId")) { SettingSave(vConfigurationFpsOverlayer, "NetId", "2"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "NetCategoryTitle")) { SettingSave(vConfigurationFpsOverlayer, "NetCategoryTitle", "NET"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "NetShowCategoryTitle")) { SettingSave(vConfigurationFpsOverlayer, "NetShowCategoryTitle", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "NetShowCurrentUsage")) { SettingSave(vConfigurationFpsOverlayer, "NetShowCurrentUsage", "False"); }

                if (!SettingCheck(vConfigurationFpsOverlayer, "AppId")) { SettingSave(vConfigurationFpsOverlayer, "AppId", "0"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "AppShowName")) { SettingSave(vConfigurationFpsOverlayer, "AppShowName", "False"); }

                if (!SettingCheck(vConfigurationFpsOverlayer, "MonId")) { SettingSave(vConfigurationFpsOverlayer, "MonId", "7"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "MonCategoryTitle")) { SettingSave(vConfigurationFpsOverlayer, "MonCategoryTitle", "MON"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "MonShowCategoryTitle")) { SettingSave(vConfigurationFpsOverlayer, "MonShowCategoryTitle", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "MonShowResolution")) { SettingSave(vConfigurationFpsOverlayer, "MonShowResolution", "False"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "MonShowDpiResolution")) { SettingSave(vConfigurationFpsOverlayer, "MonShowDpiResolution", "False"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "MonShowColorBitDepth")) { SettingSave(vConfigurationFpsOverlayer, "MonShowColorBitDepth", "False"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "MonShowRefreshRate")) { SettingSave(vConfigurationFpsOverlayer, "MonShowRefreshRate", "False"); }

                if (!SettingCheck(vConfigurationFpsOverlayer, "BatId")) { SettingSave(vConfigurationFpsOverlayer, "BatId", "8"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "BatCategoryTitle")) { SettingSave(vConfigurationFpsOverlayer, "BatCategoryTitle", "BAT"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "BatShowCategoryTitle")) { SettingSave(vConfigurationFpsOverlayer, "BatShowCategoryTitle", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "BatShowPercentage")) { SettingSave(vConfigurationFpsOverlayer, "BatShowPercentage", "True"); }

                if (!SettingCheck(vConfigurationFpsOverlayer, "TimeId")) { SettingSave(vConfigurationFpsOverlayer, "TimeId", "9"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "TimeShowCurrentTime")) { SettingSave(vConfigurationFpsOverlayer, "TimeShowCurrentTime", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "TimeShowCurrentDate")) { SettingSave(vConfigurationFpsOverlayer, "TimeShowCurrentDate", "True"); }

                if (!SettingCheck(vConfigurationFpsOverlayer, "CustomTextId")) { SettingSave(vConfigurationFpsOverlayer, "CustomTextId", "10"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "CustomTextString")) { SettingSave(vConfigurationFpsOverlayer, "CustomTextString", ""); }

                //Frames per second
                if (!SettingCheck(vConfigurationFpsOverlayer, "FpsId")) { SettingSave(vConfigurationFpsOverlayer, "FpsId", "1"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "FpsCategoryTitle")) { SettingSave(vConfigurationFpsOverlayer, "FpsCategoryTitle", "FPS"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "FpsShowCategoryTitle")) { SettingSave(vConfigurationFpsOverlayer, "FpsShowCategoryTitle", "False"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "FpsShowCurrentFps")) { SettingSave(vConfigurationFpsOverlayer, "FpsShowCurrentFps", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "FpsShowCurrentLatency")) { SettingSave(vConfigurationFpsOverlayer, "FpsShowCurrentLatency", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "FpsShowAverageFps")) { SettingSave(vConfigurationFpsOverlayer, "FpsShowAverageFps", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "FpsAverageSeconds")) { SettingSave(vConfigurationFpsOverlayer, "FpsAverageSeconds", "10"); }

                //Frame times
                if (!SettingCheck(vConfigurationFpsOverlayer, "FrametimeId")) { SettingSave(vConfigurationFpsOverlayer, "FrametimeId", "11"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "FrametimeGraphShow")) { SettingSave(vConfigurationFpsOverlayer, "FrametimeGraphShow", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "FrametimeAccuracy")) { SettingSave(vConfigurationFpsOverlayer, "FrametimeAccuracy", "2"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "FrametimeWidth")) { SettingSave(vConfigurationFpsOverlayer, "FrametimeWidth", "400"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "FrametimeHeight")) { SettingSave(vConfigurationFpsOverlayer, "FrametimeHeight", "40"); }

                //Fan
                if (!SettingCheck(vConfigurationFpsOverlayer, "FanId")) { SettingSave(vConfigurationFpsOverlayer, "FanId", "6"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "FanCategoryTitle")) { SettingSave(vConfigurationFpsOverlayer, "FanCategoryTitle", "FAN"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "FanShowCategoryTitle")) { SettingSave(vConfigurationFpsOverlayer, "FanShowCategoryTitle", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "FanShowCpu")) { SettingSave(vConfigurationFpsOverlayer, "FanShowCpu", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "FanShowGpu")) { SettingSave(vConfigurationFpsOverlayer, "FanShowGpu", "True"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "FanShowSystem")) { SettingSave(vConfigurationFpsOverlayer, "FanShowSystem", "True"); }

                //Crosshair
                if (!SettingCheck(vConfigurationFpsOverlayer, "CrosshairLaunch")) { SettingSave(vConfigurationFpsOverlayer, "CrosshairLaunch", "False"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "CrosshairColor")) { SettingSave(vConfigurationFpsOverlayer, "CrosshairColor", "#FFFFFF"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "CrosshairOpacity")) { SettingSave(vConfigurationFpsOverlayer, "CrosshairOpacity", "0,80"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "CrosshairVerticalPosition")) { SettingSave(vConfigurationFpsOverlayer, "CrosshairVerticalPosition", "0"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "CrosshairHorizontalPosition")) { SettingSave(vConfigurationFpsOverlayer, "CrosshairHorizontalPosition", "0"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "CrosshairSize")) { SettingSave(vConfigurationFpsOverlayer, "CrosshairSize", "10"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "CrosshairStyle")) { SettingSave(vConfigurationFpsOverlayer, "CrosshairStyle", "0"); }
                if (!SettingCheck(vConfigurationFpsOverlayer, "CrosshairThickness")) { SettingSave(vConfigurationFpsOverlayer, "CrosshairThickness", "1"); }

                Debug.WriteLine("Checked the application settings.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check the application settings: " + ex.Message);
            }
        }
    }
}