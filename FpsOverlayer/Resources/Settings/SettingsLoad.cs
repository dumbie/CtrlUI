using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Media;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowSettings
    {
        //Load - CtrlUI Settings
        public static void Settings_Load_CtrlUI()
        {
            try
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = "CtrlUI.exe.Config";
                vConfigurationCtrlUI = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            }
            catch (Exception Ex)
            {
                Debug.WriteLine("Failed to load the CtrlUI settings: " + Ex.Message);
            }
        }

        //Load - Application Settings
        void Settings_Load()
        {
            try
            {
                checkbox_CategoryTitles.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["CategoryTitles"]);
                checkbox_DisplayBackground.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["DisplayBackground"]);

                textblock_DisplayOpacity.Text = textblock_DisplayOpacity.Tag + ": " + ConfigurationManager.AppSettings["DisplayOpacity"].ToString() + "%";
                slider_DisplayOpacity.Value = Convert.ToDouble(ConfigurationManager.AppSettings["DisplayOpacity"]);

                textblock_MarginHorizontal.Text = textblock_MarginHorizontal.Tag + ": " + ConfigurationManager.AppSettings["MarginHorizontal"].ToString() + "px";
                slider_MarginHorizontal.Value = Convert.ToDouble(ConfigurationManager.AppSettings["MarginHorizontal"]);

                textblock_MarginVertical.Text = textblock_MarginVertical.Tag + ": " + ConfigurationManager.AppSettings["MarginVertical"].ToString() + "px";
                slider_MarginVertical.Value = Convert.ToDouble(ConfigurationManager.AppSettings["MarginVertical"]);

                //Select the current font name
                try
                {
                    combobox_TextFontName.SelectedItem = ConfigurationManager.AppSettings["TextFontName"].ToString();
                }
                catch { }

                combobox_TextPosition.SelectedIndex = Convert.ToInt32(ConfigurationManager.AppSettings["TextPosition"]);
                combobox_TextDirection.SelectedIndex = Convert.ToInt32(ConfigurationManager.AppSettings["TextDirection"]);

                textblock_TextSize.Text = textblock_TextSize.Tag + ": " + ConfigurationManager.AppSettings["TextSize"].ToString() + "px";
                slider_TextSize.Value = Convert.ToDouble(ConfigurationManager.AppSettings["TextSize"]);

                checkbox_TextColorSingle.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["TextColorSingle"]);

                checkbox_GpuShowPercentage.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["GpuShowPercentage"]);
                checkbox_GpuShowMemoryUsed.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["GpuShowMemoryUsed"]);
                checkbox_GpuShowTemperature.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["GpuShowTemperature"]);
                checkbox_GpuShowCoreFrequency.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["GpuShowCoreFrequency"]);
                checkbox_GpuShowFanSpeed.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["GpuShowFanSpeed"]);

                checkbox_CpuShowPercentage.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["CpuShowPercentage"]);
                checkbox_CpuShowTemperature.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["CpuShowTemperature"]);
                checkbox_CpuShowCoreFrequency.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["CpuShowCoreFrequency"]);
                checkbox_CpuShowPowerUsage.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["CpuShowPowerUsage"]);

                checkbox_MemShowPercentage.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["MemShowPercentage"]);
                checkbox_MemShowUsed.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["MemShowUsed"]);
                checkbox_MemShowFree.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["MemShowFree"]);
                checkbox_MemShowTotal.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["MemShowTotal"]);

                checkbox_NetShowCurrentUsage.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["NetShowCurrentUsage"]);
                checkbox_AppShowName.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["AppShowName"]);
                checkbox_TimeShowCurrentTime.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["TimeShowCurrentTime"]);

                checkbox_FpsShowCurrentFps.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["FpsShowCurrentFps"]);
                checkbox_FpsShowCurrentLatency.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["FpsShowCurrentLatency"]);
                checkbox_FpsShowAverageFps.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["FpsShowAverageFps"]);

                string ColorBackground = ConfigurationManager.AppSettings["ColorBackground"].ToString();
                colorpicker_ColorBackground.Background = new BrushConverter().ConvertFrom(ColorBackground) as SolidColorBrush;

                string ColorSingle = ConfigurationManager.AppSettings["ColorSingle"].ToString();
                colorpicker_ColorSingle.Background = new BrushConverter().ConvertFrom(ColorSingle) as SolidColorBrush;

                string ColorGpu = ConfigurationManager.AppSettings["ColorGpu"].ToString();
                colorpicker_ColorGpu.Background = new BrushConverter().ConvertFrom(ColorGpu) as SolidColorBrush;

                string ColorCpu = ConfigurationManager.AppSettings["ColorCpu"].ToString();
                colorpicker_ColorCpu.Background = new BrushConverter().ConvertFrom(ColorCpu) as SolidColorBrush;

                string ColorMem = ConfigurationManager.AppSettings["ColorMem"].ToString();
                colorpicker_ColorMem.Background = new BrushConverter().ConvertFrom(ColorMem) as SolidColorBrush;

                string ColorNet = ConfigurationManager.AppSettings["ColorNet"].ToString();
                colorpicker_ColorNet.Background = new BrushConverter().ConvertFrom(ColorNet) as SolidColorBrush;

                string ColorApp = ConfigurationManager.AppSettings["ColorApp"].ToString();
                colorpicker_ColorApp.Background = new BrushConverter().ConvertFrom(ColorApp) as SolidColorBrush;

                string ColorTime = ConfigurationManager.AppSettings["ColorTime"].ToString();
                colorpicker_ColorTime.Background = new BrushConverter().ConvertFrom(ColorTime) as SolidColorBrush;

                string ColorFps = ConfigurationManager.AppSettings["ColorFps"].ToString();
                colorpicker_ColorFps.Background = new BrushConverter().ConvertFrom(ColorFps) as SolidColorBrush;
            }
            catch (Exception Ex)
            {
                Debug.WriteLine("Failed to load the application settings: " + Ex.Message);
            }
        }
    }
}