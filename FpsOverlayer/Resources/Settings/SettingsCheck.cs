using System;
using System.Diagnostics;
using static FpsOverlayer.AppVariables;
using static LibraryShared.Settings;

namespace FpsOverlayer
{
    public partial class WindowSettings
    {
        //Check - Application Settings
        public void Settings_Check()
        {
            try
            {
                if (Setting_Load(vConfigurationFpsOverlayer, "DisplayBackground") == null) { Setting_Save(vConfigurationFpsOverlayer, "DisplayBackground", "False"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "DisplayOpacity") == null) { Setting_Save(vConfigurationFpsOverlayer, "DisplayOpacity", "0,90"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "MarginHorizontal") == null) { Setting_Save(vConfigurationFpsOverlayer, "MarginHorizontal", "5"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "MarginVertical") == null) { Setting_Save(vConfigurationFpsOverlayer, "MarginVertical", "5"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "CheckTaskbarVisible") == null) { Setting_Save(vConfigurationFpsOverlayer, "CheckTaskbarVisible", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "InterfaceFontStyleName") == null) { Setting_Save(vConfigurationFpsOverlayer, "InterfaceFontStyleName", "Segoe UI"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "TextPosition") == null) { Setting_Save(vConfigurationFpsOverlayer, "TextPosition", "0"); } //Shared
                if (Setting_Load(vConfigurationFpsOverlayer, "TextDirection") == null) { Setting_Save(vConfigurationFpsOverlayer, "TextDirection", "1"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "TextSize") == null) { Setting_Save(vConfigurationFpsOverlayer, "TextSize", "18"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "HardwareUpdateRateMs") == null) { Setting_Save(vConfigurationFpsOverlayer, "HardwareUpdateRateMs", "1000"); }

                if (Setting_Load(vConfigurationFpsOverlayer, "TextColorSingle") == null) { Setting_Save(vConfigurationFpsOverlayer, "TextColorSingle", "False"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "ColorBackground") == null) { Setting_Save(vConfigurationFpsOverlayer, "ColorBackground", "#1D1D1D"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "ColorSingle") == null) { Setting_Save(vConfigurationFpsOverlayer, "ColorSingle", "#F1F1F1"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "ColorGpu") == null) { Setting_Save(vConfigurationFpsOverlayer, "ColorGpu", "#A3FF39"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "ColorCpu") == null) { Setting_Save(vConfigurationFpsOverlayer, "ColorCpu", "#00EAFF"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "ColorMem") == null) { Setting_Save(vConfigurationFpsOverlayer, "ColorMem", "#FFA200"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "ColorFps") == null) { Setting_Save(vConfigurationFpsOverlayer, "ColorFps", "#FF0505"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "ColorNet") == null) { Setting_Save(vConfigurationFpsOverlayer, "ColorNet", "#FF00A8"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "ColorApp") == null) { Setting_Save(vConfigurationFpsOverlayer, "ColorApp", "#C000FF"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "ColorBat") == null) { Setting_Save(vConfigurationFpsOverlayer, "ColorBat", "#FFE115"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "ColorTime") == null) { Setting_Save(vConfigurationFpsOverlayer, "ColorTime", "#21AFFF"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "ColorMon") == null) { Setting_Save(vConfigurationFpsOverlayer, "ColorMon", "#21A000"); }

                if (Setting_Load(vConfigurationFpsOverlayer, "GpuId") == null) { Setting_Save(vConfigurationFpsOverlayer, "GpuId", "4"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "GpuCategoryTitle") == null) { Setting_Save(vConfigurationFpsOverlayer, "GpuCategoryTitle", "GPU"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "GpuShowCategoryTitle") == null) { Setting_Save(vConfigurationFpsOverlayer, "GpuShowCategoryTitle", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "GpuShowName") == null) { Setting_Save(vConfigurationFpsOverlayer, "GpuShowName", "False"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "GpuShowPercentage") == null) { Setting_Save(vConfigurationFpsOverlayer, "GpuShowPercentage", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "GpuShowMemoryUsed") == null) { Setting_Save(vConfigurationFpsOverlayer, "GpuShowMemoryUsed", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "GpuShowTemperature") == null) { Setting_Save(vConfigurationFpsOverlayer, "GpuShowTemperature", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "GpuShowCoreFrequency") == null) { Setting_Save(vConfigurationFpsOverlayer, "GpuShowCoreFrequency", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "GpuShowFanSpeed") == null) { Setting_Save(vConfigurationFpsOverlayer, "GpuShowFanSpeed", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "GpuShowPowerWatt") == null) { Setting_Save(vConfigurationFpsOverlayer, "GpuShowPowerWatt", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "GpuShowPowerVolt") == null) { Setting_Save(vConfigurationFpsOverlayer, "GpuShowPowerVolt", "False"); }

                if (Setting_Load(vConfigurationFpsOverlayer, "CpuId") == null) { Setting_Save(vConfigurationFpsOverlayer, "CpuId", "3"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "CpuCategoryTitle") == null) { Setting_Save(vConfigurationFpsOverlayer, "CpuCategoryTitle", "CPU"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "CpuShowCategoryTitle") == null) { Setting_Save(vConfigurationFpsOverlayer, "CpuShowCategoryTitle", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "CpuShowName") == null) { Setting_Save(vConfigurationFpsOverlayer, "CpuShowName", "False"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "CpuShowPercentage") == null) { Setting_Save(vConfigurationFpsOverlayer, "CpuShowPercentage", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "CpuShowTemperature") == null) { Setting_Save(vConfigurationFpsOverlayer, "CpuShowTemperature", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "CpuShowCoreFrequency") == null) { Setting_Save(vConfigurationFpsOverlayer, "CpuShowCoreFrequency", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "CpuShowPowerWatt") == null) { Setting_Save(vConfigurationFpsOverlayer, "CpuShowPowerWatt", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "CpuShowPowerVolt") == null) { Setting_Save(vConfigurationFpsOverlayer, "CpuShowPowerVolt", "False"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "CpuShowFanSpeed") == null) { Setting_Save(vConfigurationFpsOverlayer, "CpuShowFanSpeed", "True"); }

                if (Setting_Load(vConfigurationFpsOverlayer, "MemId") == null) { Setting_Save(vConfigurationFpsOverlayer, "MemId", "5"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "MemCategoryTitle") == null) { Setting_Save(vConfigurationFpsOverlayer, "MemCategoryTitle", "MEM"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "MemShowCategoryTitle") == null) { Setting_Save(vConfigurationFpsOverlayer, "MemShowCategoryTitle", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "MemShowPercentage") == null) { Setting_Save(vConfigurationFpsOverlayer, "MemShowPercentage", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "MemShowUsed") == null) { Setting_Save(vConfigurationFpsOverlayer, "MemShowUsed", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "MemShowFree") == null) { Setting_Save(vConfigurationFpsOverlayer, "MemShowFree", "False"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "MemShowTotal") == null) { Setting_Save(vConfigurationFpsOverlayer, "MemShowTotal", "True"); }

                if (Setting_Load(vConfigurationFpsOverlayer, "NetId") == null) { Setting_Save(vConfigurationFpsOverlayer, "NetId", "2"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "NetCategoryTitle") == null) { Setting_Save(vConfigurationFpsOverlayer, "NetCategoryTitle", "NET"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "NetShowCategoryTitle") == null) { Setting_Save(vConfigurationFpsOverlayer, "NetShowCategoryTitle", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "NetShowCurrentUsage") == null) { Setting_Save(vConfigurationFpsOverlayer, "NetShowCurrentUsage", "False"); }

                if (Setting_Load(vConfigurationFpsOverlayer, "AppId") == null) { Setting_Save(vConfigurationFpsOverlayer, "AppId", "0"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "AppShowName") == null) { Setting_Save(vConfigurationFpsOverlayer, "AppShowName", "False"); }

                if (Setting_Load(vConfigurationFpsOverlayer, "MonId") == null) { Setting_Save(vConfigurationFpsOverlayer, "MonId", "6"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "MonCategoryTitle") == null) { Setting_Save(vConfigurationFpsOverlayer, "MonCategoryTitle", "MON"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "MonShowCategoryTitle") == null) { Setting_Save(vConfigurationFpsOverlayer, "MonShowCategoryTitle", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "MonShowResolution") == null) { Setting_Save(vConfigurationFpsOverlayer, "MonShowResolution", "False"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "MonShowDpiResolution") == null) { Setting_Save(vConfigurationFpsOverlayer, "MonShowDpiResolution", "False"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "MonShowColorBitDepth") == null) { Setting_Save(vConfigurationFpsOverlayer, "MonShowColorBitDepth", "False"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "MonShowRefreshRate") == null) { Setting_Save(vConfigurationFpsOverlayer, "MonShowRefreshRate", "False"); }

                if (Setting_Load(vConfigurationFpsOverlayer, "BatId") == null) { Setting_Save(vConfigurationFpsOverlayer, "BatId", "7"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "BatCategoryTitle") == null) { Setting_Save(vConfigurationFpsOverlayer, "BatCategoryTitle", "BAT"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "BatShowCategoryTitle") == null) { Setting_Save(vConfigurationFpsOverlayer, "BatShowCategoryTitle", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "BatShowPercentage") == null) { Setting_Save(vConfigurationFpsOverlayer, "BatShowPercentage", "True"); }

                if (Setting_Load(vConfigurationFpsOverlayer, "TimeId") == null) { Setting_Save(vConfigurationFpsOverlayer, "TimeId", "8"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "TimeShowCurrentTime") == null) { Setting_Save(vConfigurationFpsOverlayer, "TimeShowCurrentTime", "True"); }

                if (Setting_Load(vConfigurationFpsOverlayer, "FpsId") == null) { Setting_Save(vConfigurationFpsOverlayer, "FpsId", "1"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "FpsCategoryTitle") == null) { Setting_Save(vConfigurationFpsOverlayer, "FpsCategoryTitle", "FPS"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "FpsShowCategoryTitle") == null) { Setting_Save(vConfigurationFpsOverlayer, "FpsShowCategoryTitle", "False"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "FpsShowCurrentFps") == null) { Setting_Save(vConfigurationFpsOverlayer, "FpsShowCurrentFps", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "FpsShowCurrentLatency") == null) { Setting_Save(vConfigurationFpsOverlayer, "FpsShowCurrentLatency", "True"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "FpsShowAverageFps") == null) { Setting_Save(vConfigurationFpsOverlayer, "FpsShowAverageFps", "True"); }

                //Crosshair
                if (Setting_Load(vConfigurationFpsOverlayer, "CrosshairLaunch") == null) { Setting_Save(vConfigurationFpsOverlayer, "CrosshairLaunch", "False"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "CrosshairColor") == null) { Setting_Save(vConfigurationFpsOverlayer, "CrosshairColor", "#FFFFFF"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "CrosshairOpacity") == null) { Setting_Save(vConfigurationFpsOverlayer, "CrosshairOpacity", "0,80"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "CrosshairSize") == null) { Setting_Save(vConfigurationFpsOverlayer, "CrosshairSize", "10"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "CrosshairStyle") == null) { Setting_Save(vConfigurationFpsOverlayer, "CrosshairStyle", "0"); }
                if (Setting_Load(vConfigurationFpsOverlayer, "CrosshairThickness") == null) { Setting_Save(vConfigurationFpsOverlayer, "CrosshairThickness", "1"); }

                Debug.WriteLine("Checked the application settings.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check the application settings: " + ex.Message);
            }
        }
    }
}