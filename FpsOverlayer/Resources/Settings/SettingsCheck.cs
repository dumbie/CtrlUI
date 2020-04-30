using System;
using System.Configuration;
using System.Diagnostics;

namespace FpsOverlayer
{
    public partial class WindowSettings
    {
        //Check - Application Settings
        public void Settings_Check()
        {
            try
            {
                if (ConfigurationManager.AppSettings["CategoryTitles"] == null) { SettingSave("CategoryTitles", "True"); }
                if (ConfigurationManager.AppSettings["DisplayBackground"] == null) { SettingSave("DisplayBackground", "False"); }
                if (ConfigurationManager.AppSettings["DisplayOpacity"] == null) { SettingSave("DisplayOpacity", "0,90"); }
                if (ConfigurationManager.AppSettings["MarginHorizontal"] == null) { SettingSave("MarginHorizontal", "0"); }
                if (ConfigurationManager.AppSettings["MarginVertical"] == null) { SettingSave("MarginVertical", "40"); }
                if (ConfigurationManager.AppSettings["InterfaceFontStyleName"] == null) { SettingSave("InterfaceFontStyleName", "Segoe UI"); }
                if (ConfigurationManager.AppSettings["TextPosition"] == null) { SettingSave("TextPosition", "0"); } //Shared
                if (ConfigurationManager.AppSettings["TextDirection"] == null) { SettingSave("TextDirection", "1"); }
                if (ConfigurationManager.AppSettings["TextSize"] == null) { SettingSave("TextSize", "18"); }

                if (ConfigurationManager.AppSettings["TextColorSingle"] == null) { SettingSave("TextColorSingle", "False"); }
                if (ConfigurationManager.AppSettings["ColorBackground"] == null) { SettingSave("ColorBackground", "#101010"); }
                if (ConfigurationManager.AppSettings["ColorSingle"] == null) { SettingSave("ColorSingle", "#F1F1F1"); }
                if (ConfigurationManager.AppSettings["ColorGpu"] == null) { SettingSave("ColorGpu", "#A3FF39"); }
                if (ConfigurationManager.AppSettings["ColorCpu"] == null) { SettingSave("ColorCpu", "#00EAFF"); }
                if (ConfigurationManager.AppSettings["ColorMem"] == null) { SettingSave("ColorMem", "#FFA200"); }
                if (ConfigurationManager.AppSettings["ColorFps"] == null) { SettingSave("ColorFps", "#FF0505"); }
                if (ConfigurationManager.AppSettings["ColorNet"] == null) { SettingSave("ColorNet", "#FF05F0"); }
                if (ConfigurationManager.AppSettings["ColorApp"] == null) { SettingSave("ColorApp", "#FFE115"); }
                if (ConfigurationManager.AppSettings["ColorTime"] == null) { SettingSave("ColorTime", "#21AFFF"); }
                if (ConfigurationManager.AppSettings["ColorMon"] == null) { SettingSave("ColorMon", "#21A000"); }

                if (ConfigurationManager.AppSettings["GpuId"] == null) { SettingSave("GpuId", "4"); }
                if (ConfigurationManager.AppSettings["GpuShowPercentage"] == null) { SettingSave("GpuShowPercentage", "True"); }
                if (ConfigurationManager.AppSettings["GpuShowMemoryUsed"] == null) { SettingSave("GpuShowMemoryUsed", "True"); }
                if (ConfigurationManager.AppSettings["GpuShowTemperature"] == null) { SettingSave("GpuShowTemperature", "True"); }
                if (ConfigurationManager.AppSettings["GpuShowCoreFrequency"] == null) { SettingSave("GpuShowCoreFrequency", "True"); }
                if (ConfigurationManager.AppSettings["GpuShowFanSpeed"] == null) { SettingSave("GpuShowFanSpeed", "True"); }

                if (ConfigurationManager.AppSettings["CpuId"] == null) { SettingSave("CpuId", "3"); }
                if (ConfigurationManager.AppSettings["CpuShowPercentage"] == null) { SettingSave("CpuShowPercentage", "True"); }
                if (ConfigurationManager.AppSettings["CpuShowTemperature"] == null) { SettingSave("CpuShowTemperature", "True"); }
                if (ConfigurationManager.AppSettings["CpuShowCoreFrequency"] == null) { SettingSave("CpuShowCoreFrequency", "True"); }
                if (ConfigurationManager.AppSettings["CpuShowPowerUsage"] == null) { SettingSave("CpuShowPowerUsage", "True"); }

                if (ConfigurationManager.AppSettings["MemId"] == null) { SettingSave("MemId", "5"); }
                if (ConfigurationManager.AppSettings["MemShowPercentage"] == null) { SettingSave("MemShowPercentage", "True"); }
                if (ConfigurationManager.AppSettings["MemShowUsed"] == null) { SettingSave("MemShowUsed", "True"); }
                if (ConfigurationManager.AppSettings["MemShowFree"] == null) { SettingSave("MemShowFree", "True"); }
                if (ConfigurationManager.AppSettings["MemShowTotal"] == null) { SettingSave("MemShowTotal", "True"); }

                if (ConfigurationManager.AppSettings["NetId"] == null) { SettingSave("NetId", "2"); }
                if (ConfigurationManager.AppSettings["NetShowCurrentUsage"] == null) { SettingSave("NetShowCurrentUsage", "False"); }

                if (ConfigurationManager.AppSettings["AppId"] == null) { SettingSave("AppId", "0"); }
                if (ConfigurationManager.AppSettings["AppShowName"] == null) { SettingSave("AppShowName", "False"); }

                if (ConfigurationManager.AppSettings["MonId"] == null) { SettingSave("MonId", "6"); }
                if (ConfigurationManager.AppSettings["MonShowResolution"] == null) { SettingSave("MonShowResolution", "False"); }

                if (ConfigurationManager.AppSettings["TimeId"] == null) { SettingSave("TimeId", "7"); }
                if (ConfigurationManager.AppSettings["TimeShowCurrentTime"] == null) { SettingSave("TimeShowCurrentTime", "True"); }

                if (ConfigurationManager.AppSettings["FpsId"] == null) { SettingSave("FpsId", "1"); }
                if (ConfigurationManager.AppSettings["FpsShowCurrentFps"] == null) { SettingSave("FpsShowCurrentFps", "True"); }
                if (ConfigurationManager.AppSettings["FpsShowCurrentLatency"] == null) { SettingSave("FpsShowCurrentLatency", "True"); }
                if (ConfigurationManager.AppSettings["FpsShowAverageFps"] == null) { SettingSave("FpsShowAverageFps", "True"); }

                Debug.WriteLine("Checked the application settings.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check the application settings: " + ex.Message);
            }
        }
    }
}