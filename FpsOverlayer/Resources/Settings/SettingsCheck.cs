using System;
using System.Configuration;
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
                if (ConfigurationManager.AppSettings["DisplayBackground"] == null) { SettingSave(vConfigurationApplication, "DisplayBackground", "False"); }
                if (ConfigurationManager.AppSettings["DisplayOpacity"] == null) { SettingSave(vConfigurationApplication, "DisplayOpacity", "0,90"); }
                if (ConfigurationManager.AppSettings["MarginHorizontal"] == null) { SettingSave(vConfigurationApplication, "MarginHorizontal", "0"); }
                if (ConfigurationManager.AppSettings["MarginVertical"] == null) { SettingSave(vConfigurationApplication, "MarginVertical", "40"); }
                if (ConfigurationManager.AppSettings["InterfaceFontStyleName"] == null) { SettingSave(vConfigurationApplication, "InterfaceFontStyleName", "Segoe UI"); }
                if (ConfigurationManager.AppSettings["TextPosition"] == null) { SettingSave(vConfigurationApplication, "TextPosition", "0"); } //Shared
                if (ConfigurationManager.AppSettings["TextDirection"] == null) { SettingSave(vConfigurationApplication, "TextDirection", "1"); }
                if (ConfigurationManager.AppSettings["TextSize"] == null) { SettingSave(vConfigurationApplication, "TextSize", "18"); }
                if (ConfigurationManager.AppSettings["HardwareUpdateRateMs"] == null) { SettingSave(vConfigurationApplication, "HardwareUpdateRateMs", "1000"); }

                if (ConfigurationManager.AppSettings["TextColorSingle"] == null) { SettingSave(vConfigurationApplication, "TextColorSingle", "False"); }
                if (ConfigurationManager.AppSettings["ColorBackground"] == null) { SettingSave(vConfigurationApplication, "ColorBackground", "#101010"); }
                if (ConfigurationManager.AppSettings["ColorSingle"] == null) { SettingSave(vConfigurationApplication, "ColorSingle", "#F1F1F1"); }
                if (ConfigurationManager.AppSettings["ColorGpu"] == null) { SettingSave(vConfigurationApplication, "ColorGpu", "#A3FF39"); }
                if (ConfigurationManager.AppSettings["ColorCpu"] == null) { SettingSave(vConfigurationApplication, "ColorCpu", "#00EAFF"); }
                if (ConfigurationManager.AppSettings["ColorMem"] == null) { SettingSave(vConfigurationApplication, "ColorMem", "#FFA200"); }
                if (ConfigurationManager.AppSettings["ColorFps"] == null) { SettingSave(vConfigurationApplication, "ColorFps", "#FF0505"); }
                if (ConfigurationManager.AppSettings["ColorNet"] == null) { SettingSave(vConfigurationApplication, "ColorNet", "#FF05F0"); }
                if (ConfigurationManager.AppSettings["ColorApp"] == null) { SettingSave(vConfigurationApplication, "ColorApp", "#FFE115"); }
                if (ConfigurationManager.AppSettings["ColorTime"] == null) { SettingSave(vConfigurationApplication, "ColorTime", "#21AFFF"); }
                if (ConfigurationManager.AppSettings["ColorMon"] == null) { SettingSave(vConfigurationApplication, "ColorMon", "#21A000"); }

                if (ConfigurationManager.AppSettings["GpuId"] == null) { SettingSave(vConfigurationApplication, "GpuId", "4"); }
                if (ConfigurationManager.AppSettings["GpuCategoryTitle"] == null) { SettingSave(vConfigurationApplication, "GpuCategoryTitle", "GPU"); }
                if (ConfigurationManager.AppSettings["GpuShowCategoryTitle"] == null) { SettingSave(vConfigurationApplication, "GpuShowCategoryTitle", "True"); }
                if (ConfigurationManager.AppSettings["GpuShowName"] == null) { SettingSave(vConfigurationApplication, "GpuShowName", "False"); }
                if (ConfigurationManager.AppSettings["GpuShowPercentage"] == null) { SettingSave(vConfigurationApplication, "GpuShowPercentage", "True"); }
                if (ConfigurationManager.AppSettings["GpuShowMemoryUsed"] == null) { SettingSave(vConfigurationApplication, "GpuShowMemoryUsed", "True"); }
                if (ConfigurationManager.AppSettings["GpuShowTemperature"] == null) { SettingSave(vConfigurationApplication, "GpuShowTemperature", "True"); }
                if (ConfigurationManager.AppSettings["GpuShowCoreFrequency"] == null) { SettingSave(vConfigurationApplication, "GpuShowCoreFrequency", "True"); }
                if (ConfigurationManager.AppSettings["GpuShowFanSpeed"] == null) { SettingSave(vConfigurationApplication, "GpuShowFanSpeed", "True"); }

                if (ConfigurationManager.AppSettings["CpuId"] == null) { SettingSave(vConfigurationApplication, "CpuId", "3"); }
                if (ConfigurationManager.AppSettings["CpuCategoryTitle"] == null) { SettingSave(vConfigurationApplication, "CpuCategoryTitle", "CPU"); }
                if (ConfigurationManager.AppSettings["CpuShowCategoryTitle"] == null) { SettingSave(vConfigurationApplication, "CpuShowCategoryTitle", "True"); }
                if (ConfigurationManager.AppSettings["CpuShowName"] == null) { SettingSave(vConfigurationApplication, "CpuShowName", "False"); }
                if (ConfigurationManager.AppSettings["CpuShowPercentage"] == null) { SettingSave(vConfigurationApplication, "CpuShowPercentage", "True"); }
                if (ConfigurationManager.AppSettings["CpuShowTemperature"] == null) { SettingSave(vConfigurationApplication, "CpuShowTemperature", "True"); }
                if (ConfigurationManager.AppSettings["CpuShowCoreFrequency"] == null) { SettingSave(vConfigurationApplication, "CpuShowCoreFrequency", "True"); }
                if (ConfigurationManager.AppSettings["CpuShowPowerUsage"] == null) { SettingSave(vConfigurationApplication, "CpuShowPowerUsage", "True"); }

                if (ConfigurationManager.AppSettings["MemId"] == null) { SettingSave(vConfigurationApplication, "MemId", "5"); }
                if (ConfigurationManager.AppSettings["MemCategoryTitle"] == null) { SettingSave(vConfigurationApplication, "MemCategoryTitle", "MEM"); }
                if (ConfigurationManager.AppSettings["MemShowCategoryTitle"] == null) { SettingSave(vConfigurationApplication, "MemShowCategoryTitle", "True"); }
                if (ConfigurationManager.AppSettings["MemShowPercentage"] == null) { SettingSave(vConfigurationApplication, "MemShowPercentage", "True"); }
                if (ConfigurationManager.AppSettings["MemShowUsed"] == null) { SettingSave(vConfigurationApplication, "MemShowUsed", "True"); }
                if (ConfigurationManager.AppSettings["MemShowFree"] == null) { SettingSave(vConfigurationApplication, "MemShowFree", "True"); }
                if (ConfigurationManager.AppSettings["MemShowTotal"] == null) { SettingSave(vConfigurationApplication, "MemShowTotal", "True"); }

                if (ConfigurationManager.AppSettings["NetId"] == null) { SettingSave(vConfigurationApplication, "NetId", "2"); }
                if (ConfigurationManager.AppSettings["NetCategoryTitle"] == null) { SettingSave(vConfigurationApplication, "NetCategoryTitle", "NET"); }
                if (ConfigurationManager.AppSettings["NetShowCategoryTitle"] == null) { SettingSave(vConfigurationApplication, "NetShowCategoryTitle", "True"); }
                if (ConfigurationManager.AppSettings["NetShowCurrentUsage"] == null) { SettingSave(vConfigurationApplication, "NetShowCurrentUsage", "False"); }

                if (ConfigurationManager.AppSettings["AppId"] == null) { SettingSave(vConfigurationApplication, "AppId", "0"); }
                if (ConfigurationManager.AppSettings["AppShowName"] == null) { SettingSave(vConfigurationApplication, "AppShowName", "False"); }

                if (ConfigurationManager.AppSettings["MonId"] == null) { SettingSave(vConfigurationApplication, "MonId", "6"); }
                if (ConfigurationManager.AppSettings["MonCategoryTitle"] == null) { SettingSave(vConfigurationApplication, "MonCategoryTitle", "MON"); }
                if (ConfigurationManager.AppSettings["MonShowCategoryTitle"] == null) { SettingSave(vConfigurationApplication, "MonShowCategoryTitle", "False"); }
                if (ConfigurationManager.AppSettings["MonShowResolution"] == null) { SettingSave(vConfigurationApplication, "MonShowResolution", "False"); }
                if (ConfigurationManager.AppSettings["MonShowDpiResolution"] == null) { SettingSave(vConfigurationApplication, "MonShowDpiResolution", "False"); }
                if (ConfigurationManager.AppSettings["MonShowColorBitDepth"] == null) { SettingSave(vConfigurationApplication, "MonShowColorBitDepth", "False"); }
                if (ConfigurationManager.AppSettings["MonShowRefreshRate"] == null) { SettingSave(vConfigurationApplication, "MonShowRefreshRate", "False"); }

                if (ConfigurationManager.AppSettings["TimeId"] == null) { SettingSave(vConfigurationApplication, "TimeId", "7"); }
                if (ConfigurationManager.AppSettings["TimeShowCurrentTime"] == null) { SettingSave(vConfigurationApplication, "TimeShowCurrentTime", "True"); }

                if (ConfigurationManager.AppSettings["FpsId"] == null) { SettingSave(vConfigurationApplication, "FpsId", "1"); }
                if (ConfigurationManager.AppSettings["FpsCategoryTitle"] == null) { SettingSave(vConfigurationApplication, "FpsCategoryTitle", "FPS"); }
                if (ConfigurationManager.AppSettings["FpsShowCategoryTitle"] == null) { SettingSave(vConfigurationApplication, "FpsShowCategoryTitle", "False"); }
                if (ConfigurationManager.AppSettings["FpsShowCurrentFps"] == null) { SettingSave(vConfigurationApplication, "FpsShowCurrentFps", "True"); }
                if (ConfigurationManager.AppSettings["FpsShowCurrentLatency"] == null) { SettingSave(vConfigurationApplication, "FpsShowCurrentLatency", "True"); }
                if (ConfigurationManager.AppSettings["FpsShowAverageFps"] == null) { SettingSave(vConfigurationApplication, "FpsShowAverageFps", "True"); }

                Debug.WriteLine("Checked the application settings.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check the application settings: " + ex.Message);
            }
        }
    }
}