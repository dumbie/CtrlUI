using ArnoldVinkCode;
using ArnoldVinkCode.Styles;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using static ArnoldVinkCode.AVSettings;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowSettings
    {
        //Save - Monitor Application Settings
        void Settings_Save()
        {
            try
            {
                cb_SettingsWindowsStartup.Click += (sender, e) =>
                {
                    AVSettings.StartupShortcutManage("FpsOverlayer-Launcher.exe", false);
                };

                checkbox_DisplayBackground.Click += (sender, e) =>
                {
                    SettingSave(vConfigurationFpsOverlayer, "DisplayBackground", checkbox_DisplayBackground.IsChecked.ToString());
                    vWindowStats.UpdateFpsOverlayStyle();
                };

                slider_DisplayOpacity.ValueChanged += (sender, e) =>
                {
                    textblock_DisplayOpacity.Text = textblock_DisplayOpacity.Tag + ": " + slider_DisplayOpacity.Value.ToString("0.00") + "%";
                    SettingSave(vConfigurationFpsOverlayer, "DisplayOpacity", slider_DisplayOpacity.Value);
                    vWindowStats.UpdateFpsOverlayStyle();
                };

                slider_HardwareUpdateRateMs.ValueChanged += (sender, e) =>
                {
                    textblock_HardwareUpdateRateMs.Text = textblock_HardwareUpdateRateMs.Tag + ": " + slider_HardwareUpdateRateMs.Value.ToString() + "ms";
                    SettingSave(vConfigurationFpsOverlayer, "HardwareUpdateRateMs", slider_HardwareUpdateRateMs.Value);
                };

                slider_MarginHorizontal.ValueChanged += (sender, e) =>
                {
                    textblock_MarginHorizontal.Text = textblock_MarginHorizontal.Tag + ": " + slider_MarginHorizontal.Value.ToString("0") + "px";
                    SettingSave(vConfigurationFpsOverlayer, "MarginHorizontal", slider_MarginHorizontal.Value);
                    vWindowStats.UpdateFpsOverlayStyle();
                };

                slider_MarginVertical.ValueChanged += (sender, e) =>
                {
                    textblock_MarginVertical.Text = textblock_MarginVertical.Tag + ": " + slider_MarginVertical.Value.ToString("0") + "px";
                    SettingSave(vConfigurationFpsOverlayer, "MarginVertical", slider_MarginVertical.Value);
                    vWindowStats.UpdateFpsOverlayStyle();
                };

                checkbox_CheckTaskbarVisible.Click += (sender, e) =>
                {
                    SettingSave(vConfigurationFpsOverlayer, "CheckTaskbarVisible", checkbox_CheckTaskbarVisible.IsChecked.ToString());
                    vWindowStats.UpdateFpsOverlayStyle();
                };

                checkbox_StatsFlipBottom.Click += (sender, e) =>
                {
                    SettingSave(vConfigurationFpsOverlayer, "StatsFlipBottom", checkbox_StatsFlipBottom.IsChecked.ToString());
                    vWindowStats.UpdateFpsOverlayStyle();
                };

                checkbox_HideScreenCapture.Click += (sender, e) =>
                {
                    SettingSave(vConfigurationFpsOverlayer, "HideScreenCapture", checkbox_HideScreenCapture.IsChecked.ToString());
                    vWindowStats.UpdateWindowAffinity();
                    vWindowCrosshair.UpdateWindowAffinity();
                };

                combobox_InterfaceFontStyleName.SelectionChanged += (sender, e) =>
                {
                    SettingSave(vConfigurationFpsOverlayer, "InterfaceFontStyleName", combobox_InterfaceFontStyleName.SelectedItem.ToString());
                    vWindowStats.UpdateFpsOverlayStyle();
                };

                combobox_TextPosition.SelectionChanged += (sender, e) =>
                {
                    SettingSave(vConfigurationFpsOverlayer, "TextPosition", combobox_TextPosition.SelectedIndex.ToString());
                    vWindowStats.UpdateFpsOverlayStyle();
                };

                combobox_TextDirection.SelectionChanged += (sender, e) =>
                {
                    SettingSave(vConfigurationFpsOverlayer, "TextDirection", combobox_TextDirection.SelectedIndex.ToString());
                    vWindowStats.UpdateFpsOverlayStyle();
                };

                slider_TextSize.ValueChanged += (sender, e) =>
                {
                    textblock_TextSize.Text = textblock_TextSize.Tag + ": " + slider_TextSize.Value.ToString("0") + "px";
                    SettingSave(vConfigurationFpsOverlayer, "TextSize", slider_TextSize.Value);
                    vWindowStats.UpdateFpsOverlayStyle();
                };

                textbox_CustomText.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    SettingSave(vConfigurationFpsOverlayer, "CustomTextString", senderTextbox.Text);
                };

                checkbox_TextColorSingle.Click += (sender, e) =>
                {
                    SettingSave(vConfigurationFpsOverlayer, "TextColorSingle", checkbox_TextColorSingle.IsChecked.ToString());
                    vWindowStats.UpdateFpsOverlayStyle();
                };

                textbox_GpuCategoryTitle.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    SettingSave(vConfigurationFpsOverlayer, "GpuCategoryTitle", senderTextbox.Text);
                    vWindowStats.UpdateFpsOverlayStyle();
                };
                checkbox_GpuShowCategoryTitle.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    SettingSave(vConfigurationFpsOverlayer, "GpuShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    vWindowStats.UpdateFpsOverlayStyle();
                };
                checkbox_GpuShowName.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "GpuShowName", checkbox_GpuShowName.IsChecked.ToString()); };
                checkbox_GpuShowPercentage.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "GpuShowPercentage", checkbox_GpuShowPercentage.IsChecked.ToString()); };
                checkbox_GpuShowMemoryUsed.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "GpuShowMemoryUsed", checkbox_GpuShowMemoryUsed.IsChecked.ToString()); };
                checkbox_GpuShowTemperature.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "GpuShowTemperature", checkbox_GpuShowTemperature.IsChecked.ToString()); };
                checkbox_GpuShowTemperatureHotspot.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "GpuShowTemperatureHotspot", checkbox_GpuShowTemperatureHotspot.IsChecked.ToString()); };
                checkbox_GpuShowMemorySpeed.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "GpuShowMemorySpeed", checkbox_GpuShowMemorySpeed.IsChecked.ToString()); };
                checkbox_GpuShowCoreFrequency.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "GpuShowCoreFrequency", checkbox_GpuShowCoreFrequency.IsChecked.ToString()); };
                checkbox_GpuShowFanSpeed.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "GpuShowFanSpeed", checkbox_GpuShowFanSpeed.IsChecked.ToString()); };
                checkbox_GpuShowPowerWatt.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "GpuShowPowerWatt", checkbox_GpuShowPowerWatt.IsChecked.ToString()); };
                checkbox_GpuShowPowerVolt.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "GpuShowPowerVolt", checkbox_GpuShowPowerVolt.IsChecked.ToString()); };

                textbox_CpuCategoryTitle.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    SettingSave(vConfigurationFpsOverlayer, "CpuCategoryTitle", senderTextbox.Text);
                    vWindowStats.UpdateFpsOverlayStyle();
                };
                checkbox_CpuShowCategoryTitle.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    SettingSave(vConfigurationFpsOverlayer, "CpuShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    vWindowStats.UpdateFpsOverlayStyle();
                };
                checkbox_CpuShowName.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "CpuShowName", checkbox_CpuShowName.IsChecked.ToString()); };
                checkbox_BoardShowName.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "BoardShowName", checkbox_BoardShowName.IsChecked.ToString()); };
                checkbox_CpuShowPercentage.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "CpuShowPercentage", checkbox_CpuShowPercentage.IsChecked.ToString()); };
                checkbox_CpuShowTemperature.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "CpuShowTemperature", checkbox_CpuShowTemperature.IsChecked.ToString()); };
                checkbox_CpuShowCoreFrequency.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "CpuShowCoreFrequency", checkbox_CpuShowCoreFrequency.IsChecked.ToString()); };
                checkbox_CpuShowPowerWatt.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "CpuShowPowerWatt", checkbox_CpuShowPowerWatt.IsChecked.ToString()); };
                checkbox_CpuShowPowerVolt.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "CpuShowPowerVolt", checkbox_CpuShowPowerVolt.IsChecked.ToString()); };
                checkbox_CpuShowFanSpeed.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "CpuShowFanSpeed", checkbox_CpuShowFanSpeed.IsChecked.ToString()); };

                textbox_MemCategoryTitle.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    SettingSave(vConfigurationFpsOverlayer, "MemCategoryTitle", senderTextbox.Text);
                    vWindowStats.UpdateFpsOverlayStyle();
                };
                checkbox_MemShowCategoryTitle.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    SettingSave(vConfigurationFpsOverlayer, "MemShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    vWindowStats.UpdateFpsOverlayStyle();
                };
                checkbox_MemShowName.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "MemShowName", checkbox_MemShowName.IsChecked.ToString()); };
                checkbox_MemShowSpeed.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "MemShowSpeed", checkbox_MemShowSpeed.IsChecked.ToString()); };
                checkbox_MemShowPowerVolt.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "MemShowPowerVolt", checkbox_MemShowPowerVolt.IsChecked.ToString()); };
                checkbox_MemShowPercentage.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "MemShowPercentage", checkbox_MemShowPercentage.IsChecked.ToString()); };
                checkbox_MemShowUsed.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "MemShowUsed", checkbox_MemShowUsed.IsChecked.ToString()); };
                checkbox_MemShowFree.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "MemShowFree", checkbox_MemShowFree.IsChecked.ToString()); };
                checkbox_MemShowTotal.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "MemShowTotal", checkbox_MemShowTotal.IsChecked.ToString()); };

                textbox_NetCategoryTitle.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    SettingSave(vConfigurationFpsOverlayer, "NetCategoryTitle", senderTextbox.Text);
                    vWindowStats.UpdateFpsOverlayStyle();
                };
                checkbox_NetShowCategoryTitle.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    SettingSave(vConfigurationFpsOverlayer, "NetShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    vWindowStats.UpdateFpsOverlayStyle();
                };
                checkbox_NetShowCurrentUsage.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "NetShowCurrentUsage", checkbox_NetShowCurrentUsage.IsChecked.ToString()); };

                checkbox_AppShowName.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "AppShowName", checkbox_AppShowName.IsChecked.ToString()); };

                textbox_BatCategoryTitle.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    SettingSave(vConfigurationFpsOverlayer, "BatCategoryTitle", senderTextbox.Text);
                    vWindowStats.UpdateFpsOverlayStyle();
                };
                checkbox_BatShowCategoryTitle.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    SettingSave(vConfigurationFpsOverlayer, "BatShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    vWindowStats.UpdateFpsOverlayStyle();
                };
                checkbox_BatShowPercentage.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "BatShowPercentage", checkbox_BatShowPercentage.IsChecked.ToString()); };

                checkbox_TimeShowCurrentTime.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "TimeShowCurrentTime", checkbox_TimeShowCurrentTime.IsChecked.ToString()); };
                checkbox_TimeShowCurrentDate.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "TimeShowCurrentDate", checkbox_TimeShowCurrentDate.IsChecked.ToString()); };

                textbox_MonCategoryTitle.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    SettingSave(vConfigurationFpsOverlayer, "MonCategoryTitle", senderTextbox.Text);
                    vWindowStats.UpdateFpsOverlayStyle();
                };
                checkbox_MonShowCategoryTitle.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    SettingSave(vConfigurationFpsOverlayer, "MonShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    vWindowStats.UpdateFpsOverlayStyle();
                };
                checkbox_MonShowResolution.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "MonShowResolution", checkbox_MonShowResolution.IsChecked.ToString()); };
                checkbox_MonShowDpiResolution.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "MonShowDpiResolution", checkbox_MonShowDpiResolution.IsChecked.ToString()); };
                checkbox_MonShowColorBitDepth.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "MonShowColorBitDepth", checkbox_MonShowColorBitDepth.IsChecked.ToString()); };
                checkbox_MonShowColorFormat.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "MonShowColorFormat", checkbox_MonShowColorFormat.IsChecked.ToString()); };
                checkbox_MonShowColorHdr.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "MonShowColorHdr", checkbox_MonShowColorHdr.IsChecked.ToString()); };
                checkbox_MonShowRefreshRate.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "MonShowRefreshRate", checkbox_MonShowRefreshRate.IsChecked.ToString()); };

                //Frames
                textbox_FpsCategoryTitle.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    SettingSave(vConfigurationFpsOverlayer, "FpsCategoryTitle", senderTextbox.Text);
                    vWindowStats.UpdateFpsOverlayStyle();
                };
                checkbox_FpsShowCategoryTitle.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    SettingSave(vConfigurationFpsOverlayer, "FpsShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    vWindowStats.UpdateFpsOverlayStyle();
                };
                checkbox_FpsShowCurrentFps.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "FpsShowCurrentFps", checkbox_FpsShowCurrentFps.IsChecked.ToString()); };
                checkbox_FpsShowCurrentLatency.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "FpsShowCurrentLatency", checkbox_FpsShowCurrentLatency.IsChecked.ToString()); };
                checkbox_FpsShowAverageFps.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "FpsShowAverageFps", checkbox_FpsShowAverageFps.IsChecked.ToString()); };

                slider_FpsAverageSeconds.ValueChanged += (sender, e) =>
                {
                    textblock_FpsAverageSeconds.Text = textblock_FpsAverageSeconds.Tag + ": " + slider_FpsAverageSeconds.Value.ToString("0") + " seconds";
                    SettingSave(vConfigurationFpsOverlayer, "FpsAverageSeconds", slider_FpsAverageSeconds.Value);
                };

                //Fan
                textbox_FanCategoryTitle.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    SettingSave(vConfigurationFpsOverlayer, "FanCategoryTitle", senderTextbox.Text);
                    vWindowStats.UpdateFpsOverlayStyle();
                };
                checkbox_FanShowCategoryTitle.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    SettingSave(vConfigurationFpsOverlayer, "FanShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    vWindowStats.UpdateFpsOverlayStyle();
                };
                checkbox_FanShowCpu.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "FanShowCpu", checkbox_FanShowCpu.IsChecked.ToString()); };
                checkbox_FanShowGpu.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "FanShowGpu", checkbox_FanShowGpu.IsChecked.ToString()); };
                checkbox_FanShowSystem.Click += (sender, e) => { SettingSave(vConfigurationFpsOverlayer, "FanShowSystem", checkbox_FanShowSystem.IsChecked.ToString()); };

                //Colors
                colorpicker_ColorSingle.Click += async (sender, e) =>
                {
                    Color? newColor = await new ColorPickerPreset().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorSingle.Background = newBrush;
                        SettingSave(vConfigurationFpsOverlayer, "ColorSingle", newColor.ToString());
                        vWindowStats.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorBackground.Click += async (sender, e) =>
                {
                    Color? newColor = await new ColorPickerPreset().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorBackground.Background = newBrush;
                        SettingSave(vConfigurationFpsOverlayer, "ColorBackground", newColor.ToString());
                        vWindowStats.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorGpu.Click += async (sender, e) =>
                {
                    Color? newColor = await new ColorPickerPreset().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorGpu.Background = newBrush;
                        SettingSave(vConfigurationFpsOverlayer, "ColorGpu", newColor.ToString());
                        vWindowStats.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorCpu.Click += async (sender, e) =>
                {
                    Color? newColor = await new ColorPickerPreset().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorCpu.Background = newBrush;
                        SettingSave(vConfigurationFpsOverlayer, "ColorCpu", newColor.ToString());
                        vWindowStats.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorMem.Click += async (sender, e) =>
                {
                    Color? newColor = await new ColorPickerPreset().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorMem.Background = newBrush;
                        SettingSave(vConfigurationFpsOverlayer, "ColorMem", newColor.ToString());
                        vWindowStats.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorFan.Click += async (sender, e) =>
                {
                    Color? newColor = await new ColorPickerPreset().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorFan.Background = newBrush;
                        SettingSave(vConfigurationFpsOverlayer, "ColorFan", newColor.ToString());
                        vWindowStats.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorNet.Click += async (sender, e) =>
                {
                    Color? newColor = await new ColorPickerPreset().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorNet.Background = newBrush;
                        SettingSave(vConfigurationFpsOverlayer, "ColorNet", newColor.ToString());
                        vWindowStats.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorApp.Click += async (sender, e) =>
                {
                    Color? newColor = await new ColorPickerPreset().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorApp.Background = newBrush;
                        SettingSave(vConfigurationFpsOverlayer, "ColorApp", newColor.ToString());
                        vWindowStats.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorBat.Click += async (sender, e) =>
                {
                    Color? newColor = await new ColorPickerPreset().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorBat.Background = newBrush;
                        SettingSave(vConfigurationFpsOverlayer, "ColorBat", newColor.ToString());
                        vWindowStats.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorTime.Click += async (sender, e) =>
                {
                    Color? newColor = await new ColorPickerPreset().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorTime.Background = newBrush;
                        SettingSave(vConfigurationFpsOverlayer, "ColorTime", newColor.ToString());
                        vWindowStats.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorCustomText.Click += async (sender, e) =>
                {
                    Color? newColor = await new ColorPickerPreset().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorCustomText.Background = newBrush;
                        SettingSave(vConfigurationFpsOverlayer, "ColorCustomText", newColor.ToString());
                        vWindowStats.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorMon.Click += async (sender, e) =>
                {
                    Color? newColor = await new ColorPickerPreset().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorMon.Background = newBrush;
                        SettingSave(vConfigurationFpsOverlayer, "ColorMon", newColor.ToString());
                        vWindowStats.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorFps.Click += async (sender, e) =>
                {
                    Color? newColor = await new ColorPickerPreset().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorFps.Background = newBrush;
                        SettingSave(vConfigurationFpsOverlayer, "ColorFps", newColor.ToString());
                        vWindowStats.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorFrametime.Click += async (sender, e) =>
                {
                    Color? newColor = await new ColorPickerPreset().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorFrametime.Background = newBrush;
                        SettingSave(vConfigurationFpsOverlayer, "ColorFrametime", newColor.ToString());
                        vWindowStats.UpdateFpsOverlayStyle();
                    }
                };

                //Frametime
                checkbox_FrametimeGraphShow.Click += (sender, e) =>
                {
                    SettingSave(vConfigurationFpsOverlayer, "FrametimeGraphShow", checkbox_FrametimeGraphShow.IsChecked.ToString());
                };

                slider_FrametimeAccuracy.ValueChanged += (sender, e) =>
                {
                    textblock_FrametimeAccuracy.Text = textblock_FrametimeAccuracy.Tag + ": " + slider_FrametimeAccuracy.Value.ToString("0") + "px";
                    SettingSave(vConfigurationFpsOverlayer, "FrametimeAccuracy", slider_FrametimeAccuracy.Value);
                };

                slider_FrametimeWidth.ValueChanged += (sender, e) =>
                {
                    textblock_FrametimeWidth.Text = textblock_FrametimeWidth.Tag + ": " + slider_FrametimeWidth.Value.ToString("0") + "px";
                    SettingSave(vConfigurationFpsOverlayer, "FrametimeWidth", slider_FrametimeWidth.Value);
                    vWindowStats.UpdateFpsOverlayStyle();
                };

                slider_FrametimeHeight.ValueChanged += (sender, e) =>
                {
                    textblock_FrametimeHeight.Text = textblock_FrametimeHeight.Tag + ": " + slider_FrametimeHeight.Value.ToString("0") + "px";
                    SettingSave(vConfigurationFpsOverlayer, "FrametimeHeight", slider_FrametimeHeight.Value);
                    vWindowStats.UpdateFpsOverlayStyle();
                };

                //Crosshair
                checkbox_CrosshairLaunch.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    SettingSave(vConfigurationFpsOverlayer, "CrosshairLaunch", senderCheckBox.IsChecked.ToString());
                };

                colorpicker_CrosshairColor.Click += async (sender, e) =>
                {
                    Color? newColor = await new ColorPickerPreset().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_CrosshairColor.Background = newBrush;
                        SettingSave(vConfigurationFpsOverlayer, "CrosshairColor", newColor.ToString());
                        vWindowCrosshair.UpdateCrosshairOverlayStyle();
                    }
                };

                slider_CrosshairOpacity.ValueChanged += (sender, e) =>
                {
                    textblock_CrosshairOpacity.Text = textblock_CrosshairOpacity.Tag + ": " + slider_CrosshairOpacity.Value.ToString("0.00") + "%";
                    SettingSave(vConfigurationFpsOverlayer, "CrosshairOpacity", slider_CrosshairOpacity.Value);
                    vWindowCrosshair.UpdateCrosshairOverlayStyle();
                };

                slider_CrosshairVerticalPosition.ValueChanged += (sender, e) =>
                {
                    textblock_CrosshairVerticalPosition.Text = textblock_CrosshairVerticalPosition.Tag + ": " + slider_CrosshairVerticalPosition.Value.ToString("0") + "px";
                    SettingSave(vConfigurationFpsOverlayer, "CrosshairVerticalPosition", slider_CrosshairVerticalPosition.Value);
                    vWindowCrosshair.UpdateCrosshairOverlayStyle();
                };

                slider_CrosshairHorizontalPosition.ValueChanged += (sender, e) =>
                {
                    textblock_CrosshairHorizontalPosition.Text = textblock_CrosshairHorizontalPosition.Tag + ": " + slider_CrosshairHorizontalPosition.Value.ToString("0") + "px";
                    SettingSave(vConfigurationFpsOverlayer, "CrosshairHorizontalPosition", slider_CrosshairHorizontalPosition.Value);
                    vWindowCrosshair.UpdateCrosshairOverlayStyle();
                };

                slider_CrosshairSize.ValueChanged += (sender, e) =>
                {
                    textblock_CrosshairSize.Text = textblock_CrosshairSize.Tag + ": " + slider_CrosshairSize.Value.ToString("0") + "px";
                    SettingSave(vConfigurationFpsOverlayer, "CrosshairSize", slider_CrosshairSize.Value);
                    vWindowCrosshair.UpdateCrosshairOverlayStyle();
                };

                slider_CrosshairThickness.ValueChanged += (sender, e) =>
                {
                    textblock_CrosshairThickness.Text = textblock_CrosshairThickness.Tag + ": " + slider_CrosshairThickness.Value.ToString("0") + "px";
                    SettingSave(vConfigurationFpsOverlayer, "CrosshairThickness", slider_CrosshairThickness.Value);
                    vWindowCrosshair.UpdateCrosshairOverlayStyle();
                };

                combobox_CrosshairStyle.SelectionChanged += (sender, e) =>
                {
                    SettingSave(vConfigurationFpsOverlayer, "CrosshairStyle", combobox_CrosshairStyle.SelectedIndex.ToString());
                    vWindowCrosshair.UpdateCrosshairOverlayStyle();
                };

                //Tools
                checkbox_ToolsLaunch.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    SettingSave(vConfigurationFpsOverlayer, "ToolsLaunch", senderCheckBox.IsChecked.ToString());
                };

                //Browser
                checkbox_BrowserUnload.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    SettingSave(vConfigurationFpsOverlayer, "BrowserUnload", senderCheckBox.IsChecked.ToString());
                };

                slider_BrowserOpacity.ValueChanged += async (sender, e) =>
                {
                    textblock_BrowserOpacity.Text = textblock_BrowserOpacity.Tag + ": " + slider_BrowserOpacity.Value.ToString("0.00") + "%";
                    SettingSave(vConfigurationFpsOverlayer, "BrowserOpacity", slider_BrowserOpacity.Value);
                    await vWindowTools.Browser_Update_Opacity();
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save the application settings: " + ex.Message);
            }
        }
    }
}