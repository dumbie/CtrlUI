using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using static ArnoldVinkCode.AVSettings;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowSettings
    {
        //Load - Application Settings
        async Task Settings_Load()
        {
            try
            {
                checkbox_DisplayBackground.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "DisplayBackground", typeof(bool));

                textblock_DisplayOpacity.Text = textblock_DisplayOpacity.Tag + ": " + SettingLoad(vConfigurationFpsOverlayer, "DisplayOpacity", typeof(string)) + "%";
                slider_DisplayOpacity.Value = SettingLoad(vConfigurationFpsOverlayer, "DisplayOpacity", typeof(double));

                textblock_HardwareUpdateRateMs.Text = textblock_HardwareUpdateRateMs.Tag + ": " + SettingLoad(vConfigurationFpsOverlayer, "HardwareUpdateRateMs", typeof(string)) + "ms";
                slider_HardwareUpdateRateMs.Value = SettingLoad(vConfigurationFpsOverlayer, "HardwareUpdateRateMs", typeof(double));

                textblock_MarginHorizontal.Text = textblock_MarginHorizontal.Tag + ": " + SettingLoad(vConfigurationFpsOverlayer, "MarginHorizontal", typeof(string)) + "px";
                slider_MarginHorizontal.Value = SettingLoad(vConfigurationFpsOverlayer, "MarginHorizontal", typeof(double));

                textblock_MarginVertical.Text = textblock_MarginVertical.Tag + ": " + SettingLoad(vConfigurationFpsOverlayer, "MarginVertical", typeof(string)) + "px";
                slider_MarginVertical.Value = SettingLoad(vConfigurationFpsOverlayer, "MarginVertical", typeof(double));

                checkbox_CheckTaskbarVisible.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "CheckTaskbarVisible", typeof(bool));
                checkbox_StatsFlipBottom.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "StatsFlipBottom", typeof(bool));
                checkbox_HideScreenCapture.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "HideScreenCapture", typeof(bool));

                //Select the current font name
                try
                {
                    combobox_InterfaceFontStyleName.SelectedItem = SettingLoad(vConfigurationFpsOverlayer, "InterfaceFontStyleName", typeof(string));
                }
                catch { }

                combobox_TextPosition.SelectedIndex = SettingLoad(vConfigurationFpsOverlayer, "TextPosition", typeof(int));
                combobox_TextDirection.SelectedIndex = SettingLoad(vConfigurationFpsOverlayer, "TextDirection", typeof(int));

                textblock_TextSize.Text = textblock_TextSize.Tag + ": " + SettingLoad(vConfigurationFpsOverlayer, "TextSize", typeof(string)) + "px";
                slider_TextSize.Value = SettingLoad(vConfigurationFpsOverlayer, "TextSize", typeof(double));

                textbox_CustomText.Text = SettingLoad(vConfigurationFpsOverlayer, "CustomTextString", typeof(string));

                checkbox_TextColorSingle.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "TextColorSingle", typeof(bool));

                textbox_GpuCategoryTitle.Text = SettingLoad(vConfigurationFpsOverlayer, "GpuCategoryTitle", typeof(string));
                checkbox_GpuShowCategoryTitle.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "GpuShowCategoryTitle", typeof(bool));
                checkbox_GpuShowName.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "GpuShowName", typeof(bool));
                checkbox_GpuShowPercentage.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "GpuShowPercentage", typeof(bool));
                checkbox_GpuShowMemoryUsed.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "GpuShowMemoryUsed", typeof(bool));
                checkbox_GpuShowTemperature.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "GpuShowTemperature", typeof(bool));
                checkbox_GpuShowTemperatureHotspot.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "GpuShowTemperatureHotspot", typeof(bool));
                checkbox_GpuShowMemorySpeed.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "GpuShowMemorySpeed", typeof(bool));
                checkbox_GpuShowCoreFrequency.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "GpuShowCoreFrequency", typeof(bool));
                checkbox_GpuShowFanSpeed.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "GpuShowFanSpeed", typeof(bool));
                checkbox_GpuShowPowerWatt.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "GpuShowPowerWatt", typeof(bool));
                checkbox_GpuShowPowerVolt.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "GpuShowPowerVolt", typeof(bool));

                textbox_CpuCategoryTitle.Text = SettingLoad(vConfigurationFpsOverlayer, "CpuCategoryTitle", typeof(string));
                checkbox_CpuShowCategoryTitle.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "CpuShowCategoryTitle", typeof(bool));
                checkbox_CpuShowName.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "CpuShowName", typeof(bool));
                checkbox_BoardShowName.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "BoardShowName", typeof(bool));
                checkbox_CpuShowPercentage.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "CpuShowPercentage", typeof(bool));
                checkbox_CpuShowTemperature.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "CpuShowTemperature", typeof(bool));
                checkbox_CpuShowCoreFrequency.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "CpuShowCoreFrequency", typeof(bool));
                checkbox_CpuShowPowerWatt.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "CpuShowPowerWatt", typeof(bool));
                checkbox_CpuShowPowerVolt.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "CpuShowPowerVolt", typeof(bool));
                checkbox_CpuShowFanSpeed.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "CpuShowFanSpeed", typeof(bool));

                textbox_MemCategoryTitle.Text = SettingLoad(vConfigurationFpsOverlayer, "MemCategoryTitle", typeof(string));
                checkbox_MemShowCategoryTitle.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "MemShowCategoryTitle", typeof(bool));
                checkbox_MemShowName.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "MemShowName", typeof(bool));
                checkbox_MemShowSpeed.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "MemShowSpeed", typeof(bool));
                checkbox_MemShowPowerVolt.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "MemShowPowerVolt", typeof(bool));
                checkbox_MemShowPercentage.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "MemShowPercentage", typeof(bool));
                checkbox_MemShowUsed.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "MemShowUsed", typeof(bool));
                checkbox_MemShowFree.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "MemShowFree", typeof(bool));
                checkbox_MemShowTotal.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "MemShowTotal", typeof(bool));

                textbox_NetCategoryTitle.Text = SettingLoad(vConfigurationFpsOverlayer, "NetCategoryTitle", typeof(string));
                checkbox_NetShowCategoryTitle.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "NetShowCategoryTitle", typeof(bool));
                checkbox_NetShowCurrentUsage.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "NetShowCurrentUsage", typeof(bool));

                checkbox_AppShowName.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "AppShowName", typeof(bool));

                textbox_BatCategoryTitle.Text = SettingLoad(vConfigurationFpsOverlayer, "BatCategoryTitle", typeof(string));
                checkbox_BatShowCategoryTitle.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "BatShowCategoryTitle", typeof(bool));
                checkbox_BatShowPercentage.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "BatShowPercentage", typeof(bool));

                checkbox_TimeShowCurrentTime.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "TimeShowCurrentTime", typeof(bool));
                checkbox_TimeShowCurrentDate.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "TimeShowCurrentDate", typeof(bool));

                textbox_MonCategoryTitle.Text = SettingLoad(vConfigurationFpsOverlayer, "MonCategoryTitle", typeof(string));
                checkbox_MonShowCategoryTitle.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "MonShowCategoryTitle", typeof(bool));
                checkbox_MonShowResolution.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "MonShowResolution", typeof(bool));
                checkbox_MonShowDpiResolution.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "MonShowDpiResolution", typeof(bool));
                checkbox_MonShowColorBitDepth.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "MonShowColorBitDepth", typeof(bool));
                checkbox_MonShowColorFormat.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "MonShowColorFormat", typeof(bool));
                checkbox_MonShowColorHdr.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "MonShowColorHdr", typeof(bool));
                checkbox_MonShowRefreshRate.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "MonShowRefreshRate", typeof(bool));

                //Frames
                textbox_FpsCategoryTitle.Text = SettingLoad(vConfigurationFpsOverlayer, "FpsCategoryTitle", typeof(string));
                checkbox_FpsShowCategoryTitle.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "FpsShowCategoryTitle", typeof(bool));
                checkbox_FpsShowCurrentFps.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "FpsShowCurrentFps", typeof(bool));
                checkbox_FpsShowCurrentLatency.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "FpsShowCurrentLatency", typeof(bool));
                checkbox_FpsShowAverageFps.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "FpsShowAverageFps", typeof(bool));

                textblock_FpsAverageSeconds.Text = textblock_FpsAverageSeconds.Tag + ": " + SettingLoad(vConfigurationFpsOverlayer, "FpsAverageSeconds", typeof(string)) + " seconds";
                slider_FpsAverageSeconds.Value = SettingLoad(vConfigurationFpsOverlayer, "FpsAverageSeconds", typeof(double));

                //Fan
                textbox_FanCategoryTitle.Text = SettingLoad(vConfigurationFpsOverlayer, "FanCategoryTitle", typeof(string));
                checkbox_FanShowCategoryTitle.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "FanShowCategoryTitle", typeof(bool));
                checkbox_FanShowCpu.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "FanShowCpu", typeof(bool));
                checkbox_FanShowGpu.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "FanShowGpu", typeof(bool));
                checkbox_FanShowSystem.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "FanShowSystem", typeof(bool));

                //Colors
                string ColorBackground = SettingLoad(vConfigurationFpsOverlayer, "ColorBackground", typeof(string));
                colorpicker_ColorBackground.Background = new BrushConverter().ConvertFrom(ColorBackground) as SolidColorBrush;

                string ColorSingle = SettingLoad(vConfigurationFpsOverlayer, "ColorSingle", typeof(string));
                colorpicker_ColorSingle.Background = new BrushConverter().ConvertFrom(ColorSingle) as SolidColorBrush;

                string ColorGpu = SettingLoad(vConfigurationFpsOverlayer, "ColorGpu", typeof(string));
                colorpicker_ColorGpu.Background = new BrushConverter().ConvertFrom(ColorGpu) as SolidColorBrush;

                string ColorCpu = SettingLoad(vConfigurationFpsOverlayer, "ColorCpu", typeof(string));
                colorpicker_ColorCpu.Background = new BrushConverter().ConvertFrom(ColorCpu) as SolidColorBrush;

                string ColorMem = SettingLoad(vConfigurationFpsOverlayer, "ColorMem", typeof(string));
                colorpicker_ColorMem.Background = new BrushConverter().ConvertFrom(ColorMem) as SolidColorBrush;

                string ColorNet = SettingLoad(vConfigurationFpsOverlayer, "ColorNet", typeof(string));
                colorpicker_ColorNet.Background = new BrushConverter().ConvertFrom(ColorNet) as SolidColorBrush;

                string ColorApp = SettingLoad(vConfigurationFpsOverlayer, "ColorApp", typeof(string));
                colorpicker_ColorApp.Background = new BrushConverter().ConvertFrom(ColorApp) as SolidColorBrush;

                string ColorBat = SettingLoad(vConfigurationFpsOverlayer, "ColorBat", typeof(string));
                colorpicker_ColorBat.Background = new BrushConverter().ConvertFrom(ColorBat) as SolidColorBrush;

                string ColorTime = SettingLoad(vConfigurationFpsOverlayer, "ColorTime", typeof(string));
                colorpicker_ColorTime.Background = new BrushConverter().ConvertFrom(ColorTime) as SolidColorBrush;

                string ColorCustomText = SettingLoad(vConfigurationFpsOverlayer, "ColorCustomText", typeof(string));
                colorpicker_ColorCustomText.Background = new BrushConverter().ConvertFrom(ColorCustomText) as SolidColorBrush;

                string ColorMon = SettingLoad(vConfigurationFpsOverlayer, "ColorMon", typeof(string));
                colorpicker_ColorMon.Background = new BrushConverter().ConvertFrom(ColorMon) as SolidColorBrush;

                string ColorFps = SettingLoad(vConfigurationFpsOverlayer, "ColorFps", typeof(string));
                colorpicker_ColorFps.Background = new BrushConverter().ConvertFrom(ColorFps) as SolidColorBrush;

                string ColorFrametime = SettingLoad(vConfigurationFpsOverlayer, "ColorFrametime", typeof(string));
                colorpicker_ColorFrametime.Background = new BrushConverter().ConvertFrom(ColorFrametime) as SolidColorBrush;

                //Frametime
                checkbox_FrametimeGraphShow.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "FrametimeGraphShow", typeof(bool));

                textblock_FrametimeAccuracy.Text = textblock_FrametimeAccuracy.Tag + ": " + SettingLoad(vConfigurationFpsOverlayer, "FrametimeAccuracy", typeof(string)) + "px";
                slider_FrametimeAccuracy.Value = SettingLoad(vConfigurationFpsOverlayer, "FrametimeAccuracy", typeof(double));

                textblock_FrametimeWidth.Text = textblock_FrametimeWidth.Tag + ": " + SettingLoad(vConfigurationFpsOverlayer, "FrametimeWidth", typeof(string)) + "px";
                slider_FrametimeWidth.Value = SettingLoad(vConfigurationFpsOverlayer, "FrametimeWidth", typeof(double));

                textblock_FrametimeHeight.Text = textblock_FrametimeHeight.Tag + ": " + SettingLoad(vConfigurationFpsOverlayer, "FrametimeHeight", typeof(string)) + "px";
                slider_FrametimeHeight.Value = SettingLoad(vConfigurationFpsOverlayer, "FrametimeHeight", typeof(double));

                //Crosshair
                checkbox_CrosshairLaunch.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "CrosshairLaunch", typeof(bool));

                string CrosshairColor = SettingLoad(vConfigurationFpsOverlayer, "CrosshairColor", typeof(string));
                colorpicker_CrosshairColor.Background = new BrushConverter().ConvertFrom(CrosshairColor) as SolidColorBrush;

                textblock_CrosshairOpacity.Text = textblock_CrosshairOpacity.Tag + ": " + SettingLoad(vConfigurationFpsOverlayer, "CrosshairOpacity", typeof(string)) + "%";
                slider_CrosshairOpacity.Value = SettingLoad(vConfigurationFpsOverlayer, "CrosshairOpacity", typeof(double));

                textblock_CrosshairVerticalPosition.Text = textblock_CrosshairVerticalPosition.Tag + ": " + SettingLoad(vConfigurationFpsOverlayer, "CrosshairVerticalPosition", typeof(string)) + "px";
                slider_CrosshairVerticalPosition.Value = SettingLoad(vConfigurationFpsOverlayer, "CrosshairVerticalPosition", typeof(double));

                textblock_CrosshairHorizontalPosition.Text = textblock_CrosshairHorizontalPosition.Tag + ": " + SettingLoad(vConfigurationFpsOverlayer, "CrosshairHorizontalPosition", typeof(string)) + "px";
                slider_CrosshairHorizontalPosition.Value = SettingLoad(vConfigurationFpsOverlayer, "CrosshairHorizontalPosition", typeof(double));

                textblock_CrosshairSize.Text = textblock_CrosshairSize.Tag + ": " + SettingLoad(vConfigurationFpsOverlayer, "CrosshairSize", typeof(string)) + "px";
                slider_CrosshairSize.Value = SettingLoad(vConfigurationFpsOverlayer, "CrosshairSize", typeof(double));

                textblock_CrosshairThickness.Text = textblock_CrosshairThickness.Tag + ": " + SettingLoad(vConfigurationFpsOverlayer, "CrosshairThickness", typeof(string)) + "px";
                slider_CrosshairThickness.Value = SettingLoad(vConfigurationFpsOverlayer, "CrosshairThickness", typeof(double));

                combobox_CrosshairStyle.SelectedIndex = SettingLoad(vConfigurationFpsOverlayer, "CrosshairStyle", typeof(int));

                //Tools
                checkbox_ToolsShowStartup.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "ToolsShowStartup", typeof(bool));

                //Browser
                checkbox_BrowserUnload.IsChecked = SettingLoad(vConfigurationFpsOverlayer, "BrowserUnload", typeof(bool));

                textblock_BrowserOpacity.Text = textblock_BrowserOpacity.Tag + ": " + SettingLoad(vConfigurationFpsOverlayer, "BrowserOpacity", typeof(string)) + "%";
                slider_BrowserOpacity.Value = SettingLoad(vConfigurationFpsOverlayer, "BrowserOpacity", typeof(double));

                //Update stats position text
                UpdateStatsPositionText();

                //Set the application name to string to check shortcuts
                string targetName = AVFunctions.ApplicationName();

                //Check if application is set to launch on Windows startup
                string targetFileStartup = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), targetName + ".url");
                if (File.Exists(targetFileStartup))
                {
                    cb_SettingsWindowsStartup.IsChecked = true;
                }

                //Wait for settings to have loaded
                await Task.Delay(1500);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load application settings: " + ex.Message);
            }
        }
    }
}