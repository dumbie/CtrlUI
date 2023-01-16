using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using static FpsOverlayer.AppVariables;
using static LibraryShared.Settings;

namespace FpsOverlayer
{
    public partial class WindowSettings
    {
        //Load - Application Settings
        bool Settings_Load()
        {
            try
            {
                checkbox_DisplayBackground.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "DisplayBackground"));

                textblock_DisplayOpacity.Text = textblock_DisplayOpacity.Tag + ": " + Setting_Load(vConfigurationFpsOverlayer, "DisplayOpacity").ToString() + "%";
                slider_DisplayOpacity.Value = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "DisplayOpacity"));

                textblock_HardwareUpdateRateMs.Text = textblock_HardwareUpdateRateMs.Tag + ": " + Setting_Load(vConfigurationFpsOverlayer, "HardwareUpdateRateMs").ToString() + "ms";
                slider_HardwareUpdateRateMs.Value = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "HardwareUpdateRateMs"));

                textblock_MarginHorizontal.Text = textblock_MarginHorizontal.Tag + ": " + Setting_Load(vConfigurationFpsOverlayer, "MarginHorizontal").ToString() + "px";
                slider_MarginHorizontal.Value = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginHorizontal"));

                textblock_MarginVertical.Text = textblock_MarginVertical.Tag + ": " + Setting_Load(vConfigurationFpsOverlayer, "MarginVertical").ToString() + "px";
                slider_MarginVertical.Value = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "MarginVertical"));
                checkbox_CheckTaskbarVisible.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "CheckTaskbarVisible"));

                //Select the current font name
                try
                {
                    combobox_InterfaceFontStyleName.SelectedItem = Setting_Load(vConfigurationFpsOverlayer, "InterfaceFontStyleName").ToString();
                }
                catch { }

                combobox_TextPosition.SelectedIndex = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "TextPosition"));
                combobox_TextDirection.SelectedIndex = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "TextDirection"));

                textblock_TextSize.Text = textblock_TextSize.Tag + ": " + Setting_Load(vConfigurationFpsOverlayer, "TextSize").ToString() + "px";
                slider_TextSize.Value = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "TextSize"));

                textbox_CustomText.Text = Convert.ToString(Setting_Load(vConfigurationFpsOverlayer, "CustomTextString"));

                checkbox_TextColorSingle.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "TextColorSingle"));

                textbox_GpuCategoryTitle.Text = Convert.ToString(Setting_Load(vConfigurationFpsOverlayer, "GpuCategoryTitle"));
                checkbox_GpuShowCategoryTitle.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "GpuShowCategoryTitle"));
                checkbox_GpuShowName.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "GpuShowName"));
                checkbox_GpuShowPercentage.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "GpuShowPercentage"));
                checkbox_GpuShowMemoryUsed.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "GpuShowMemoryUsed"));
                checkbox_GpuShowTemperature.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "GpuShowTemperature"));
                checkbox_GpuShowCoreFrequency.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "GpuShowCoreFrequency"));
                checkbox_GpuShowFanSpeed.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "GpuShowFanSpeed"));
                checkbox_GpuShowPowerWatt.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "GpuShowPowerWatt"));
                checkbox_GpuShowPowerVolt.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "GpuShowPowerVolt"));

                textbox_CpuCategoryTitle.Text = Convert.ToString(Setting_Load(vConfigurationFpsOverlayer, "CpuCategoryTitle"));
                checkbox_CpuShowCategoryTitle.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "CpuShowCategoryTitle"));
                checkbox_CpuShowName.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "CpuShowName"));
                checkbox_BoardShowName.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "BoardShowName"));
                checkbox_CpuShowPercentage.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "CpuShowPercentage"));
                checkbox_CpuShowTemperature.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "CpuShowTemperature"));
                checkbox_CpuShowCoreFrequency.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "CpuShowCoreFrequency"));
                checkbox_CpuShowPowerWatt.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "CpuShowPowerWatt"));
                checkbox_CpuShowPowerVolt.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "CpuShowPowerVolt"));
                checkbox_CpuShowFanSpeed.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "CpuShowFanSpeed"));

                textbox_MemCategoryTitle.Text = Convert.ToString(Setting_Load(vConfigurationFpsOverlayer, "MemCategoryTitle"));
                checkbox_MemShowCategoryTitle.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MemShowCategoryTitle"));
                checkbox_MemShowName.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MemShowName"));
                checkbox_MemShowSpeed.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MemShowSpeed"));
                checkbox_MemShowPowerVolt.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MemShowPowerVolt"));
                checkbox_MemShowPercentage.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MemShowPercentage"));
                checkbox_MemShowUsed.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MemShowUsed"));
                checkbox_MemShowFree.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MemShowFree"));
                checkbox_MemShowTotal.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MemShowTotal"));

                textbox_NetCategoryTitle.Text = Convert.ToString(Setting_Load(vConfigurationFpsOverlayer, "NetCategoryTitle"));
                checkbox_NetShowCategoryTitle.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "NetShowCategoryTitle"));
                checkbox_NetShowCurrentUsage.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "NetShowCurrentUsage"));

                checkbox_AppShowName.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "AppShowName"));

                textbox_BatCategoryTitle.Text = Convert.ToString(Setting_Load(vConfigurationFpsOverlayer, "BatCategoryTitle"));
                checkbox_BatShowCategoryTitle.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "BatShowCategoryTitle"));
                checkbox_BatShowPercentage.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "BatShowPercentage"));

                checkbox_TimeShowCurrentTime.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "TimeShowCurrentTime"));

                textbox_MonCategoryTitle.Text = Convert.ToString(Setting_Load(vConfigurationFpsOverlayer, "MonCategoryTitle"));
                checkbox_MonShowCategoryTitle.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MonShowCategoryTitle"));
                checkbox_MonShowResolution.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MonShowResolution"));
                checkbox_MonShowDpiResolution.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MonShowDpiResolution"));
                checkbox_MonShowColorBitDepth.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MonShowColorBitDepth"));
                checkbox_MonShowRefreshRate.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MonShowRefreshRate"));

                textbox_FpsCategoryTitle.Text = Convert.ToString(Setting_Load(vConfigurationFpsOverlayer, "FpsCategoryTitle"));
                checkbox_FpsShowCategoryTitle.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "FpsShowCategoryTitle"));
                checkbox_FpsShowCurrentFps.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "FpsShowCurrentFps"));
                checkbox_FpsShowCurrentLatency.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "FpsShowCurrentLatency"));
                checkbox_FpsShowAverageFps.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "FpsShowAverageFps"));

                string ColorBackground = Setting_Load(vConfigurationFpsOverlayer, "ColorBackground").ToString();
                colorpicker_ColorBackground.Background = new BrushConverter().ConvertFrom(ColorBackground) as SolidColorBrush;

                string ColorSingle = Setting_Load(vConfigurationFpsOverlayer, "ColorSingle").ToString();
                colorpicker_ColorSingle.Background = new BrushConverter().ConvertFrom(ColorSingle) as SolidColorBrush;

                string ColorGpu = Setting_Load(vConfigurationFpsOverlayer, "ColorGpu").ToString();
                colorpicker_ColorGpu.Background = new BrushConverter().ConvertFrom(ColorGpu) as SolidColorBrush;

                string ColorCpu = Setting_Load(vConfigurationFpsOverlayer, "ColorCpu").ToString();
                colorpicker_ColorCpu.Background = new BrushConverter().ConvertFrom(ColorCpu) as SolidColorBrush;

                string ColorMem = Setting_Load(vConfigurationFpsOverlayer, "ColorMem").ToString();
                colorpicker_ColorMem.Background = new BrushConverter().ConvertFrom(ColorMem) as SolidColorBrush;

                string ColorNet = Setting_Load(vConfigurationFpsOverlayer, "ColorNet").ToString();
                colorpicker_ColorNet.Background = new BrushConverter().ConvertFrom(ColorNet) as SolidColorBrush;

                string ColorApp = Setting_Load(vConfigurationFpsOverlayer, "ColorApp").ToString();
                colorpicker_ColorApp.Background = new BrushConverter().ConvertFrom(ColorApp) as SolidColorBrush;

                string ColorBat = Setting_Load(vConfigurationFpsOverlayer, "ColorBat").ToString();
                colorpicker_ColorBat.Background = new BrushConverter().ConvertFrom(ColorBat) as SolidColorBrush;

                string ColorTime = Setting_Load(vConfigurationFpsOverlayer, "ColorTime").ToString();
                colorpicker_ColorTime.Background = new BrushConverter().ConvertFrom(ColorTime) as SolidColorBrush;

                string ColorCustomText = Setting_Load(vConfigurationFpsOverlayer, "ColorCustomText").ToString();
                colorpicker_ColorCustomText.Background = new BrushConverter().ConvertFrom(ColorCustomText) as SolidColorBrush;

                string ColorMon = Setting_Load(vConfigurationFpsOverlayer, "ColorMon").ToString();
                colorpicker_ColorMon.Background = new BrushConverter().ConvertFrom(ColorMon) as SolidColorBrush;

                string ColorFps = Setting_Load(vConfigurationFpsOverlayer, "ColorFps").ToString();
                colorpicker_ColorFps.Background = new BrushConverter().ConvertFrom(ColorFps) as SolidColorBrush;

                //Crosshair
                checkbox_CrosshairLaunch.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "CrosshairLaunch"));

                string CrosshairColor = Setting_Load(vConfigurationFpsOverlayer, "CrosshairColor").ToString();
                colorpicker_CrosshairColor.Background = new BrushConverter().ConvertFrom(CrosshairColor) as SolidColorBrush;

                textblock_CrosshairOpacity.Text = textblock_CrosshairOpacity.Tag + ": " + Setting_Load(vConfigurationFpsOverlayer, "CrosshairOpacity").ToString() + "%";
                slider_CrosshairOpacity.Value = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "CrosshairOpacity"));

                textblock_CrosshairVerticalPosition.Text = textblock_CrosshairVerticalPosition.Tag + ": " + Setting_Load(vConfigurationFpsOverlayer, "CrosshairVerticalPosition").ToString() + "px";
                slider_CrosshairVerticalPosition.Value = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "CrosshairVerticalPosition"));

                textblock_CrosshairSize.Text = textblock_CrosshairSize.Tag + ": " + Setting_Load(vConfigurationFpsOverlayer, "CrosshairSize").ToString() + "px";
                slider_CrosshairSize.Value = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "CrosshairSize"));

                textblock_CrosshairThickness.Text = textblock_CrosshairThickness.Tag + ": " + Setting_Load(vConfigurationFpsOverlayer, "CrosshairThickness").ToString() + "px";
                slider_CrosshairThickness.Value = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "CrosshairThickness"));

                combobox_CrosshairStyle.SelectedIndex = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "CrosshairStyle"));

                //Browser
                checkbox_BrowserShowStartup.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "BrowserShowStartup"));
                checkbox_BrowserUnload.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "BrowserUnload"));

                textblock_BrowserOpacity.Text = textblock_BrowserOpacity.Tag + ": " + Setting_Load(vConfigurationFpsOverlayer, "BrowserOpacity").ToString() + "%";
                slider_BrowserOpacity.Value = Convert.ToDouble(Setting_Load(vConfigurationFpsOverlayer, "BrowserOpacity"));

                //Update stats position text
                UpdateStatsPositionText();

                //Set the application name to string to check shortcuts
                string targetName = Assembly.GetEntryAssembly().GetName().Name;

                //Check if application is set to launch on Windows startup
                string targetFileStartup = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), targetName + ".url");
                if (File.Exists(targetFileStartup))
                {
                    cb_SettingsWindowsStartup.IsChecked = true;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load the application settings: " + ex.Message);
                return false;
            }
        }
    }
}