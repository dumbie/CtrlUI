using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using static FpsOverlayer.AppVariables;
using static LibraryShared.Settings;

namespace FpsOverlayer
{
    public partial class WindowSettings
    {
        //Save - Monitor Application Settings
        void Settings_Save()
        {
            try
            {
                cb_SettingsWindowsStartup.Click += (sender, e) => { ManageShortcutStartup(); };

                checkbox_DisplayBackground.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "DisplayBackground", checkbox_DisplayBackground.IsChecked.ToString());
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };

                slider_DisplayOpacity.ValueChanged += async (sender, e) =>
                {
                    textblock_DisplayOpacity.Text = textblock_DisplayOpacity.Tag + ": " + slider_DisplayOpacity.Value.ToString("0.00") + "%";
                    Setting_Save(vConfigurationFpsOverlayer, "DisplayOpacity", slider_DisplayOpacity.Value.ToString("0.00"));
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };

                slider_HardwareUpdateRateMs.ValueChanged += (sender, e) =>
                {
                    textblock_HardwareUpdateRateMs.Text = textblock_HardwareUpdateRateMs.Tag + ": " + slider_HardwareUpdateRateMs.Value.ToString() + "ms";
                    Setting_Save(vConfigurationFpsOverlayer, "HardwareUpdateRateMs", slider_HardwareUpdateRateMs.Value.ToString());
                };

                slider_MarginHorizontal.ValueChanged += async (sender, e) =>
                {
                    textblock_MarginHorizontal.Text = textblock_MarginHorizontal.Tag + ": " + slider_MarginHorizontal.Value.ToString("0") + "px";
                    Setting_Save(vConfigurationFpsOverlayer, "MarginHorizontal", slider_MarginHorizontal.Value.ToString("0"));
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };

                slider_MarginVertical.ValueChanged += async (sender, e) =>
                {
                    textblock_MarginVertical.Text = textblock_MarginVertical.Tag + ": " + slider_MarginVertical.Value.ToString("0") + "px";
                    Setting_Save(vConfigurationFpsOverlayer, "MarginVertical", slider_MarginVertical.Value.ToString("0"));
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };

                checkbox_CheckTaskbarVisible.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "CheckTaskbarVisible", checkbox_CheckTaskbarVisible.IsChecked.ToString());
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };

                combobox_InterfaceFontStyleName.SelectionChanged += async (sender, e) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "InterfaceFontStyleName", combobox_InterfaceFontStyleName.SelectedItem.ToString());
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };

                combobox_TextPosition.SelectionChanged += async (sender, e) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "TextPosition", combobox_TextPosition.SelectedIndex.ToString());
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                    await NotifyDirectXInputSettingChanged("TextPosition");
                };

                combobox_TextDirection.SelectionChanged += async (sender, e) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "TextDirection", combobox_TextDirection.SelectedIndex.ToString());
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };

                slider_TextSize.ValueChanged += async (sender, e) =>
                {
                    textblock_TextSize.Text = textblock_TextSize.Tag + ": " + slider_TextSize.Value.ToString("0") + "px";
                    Setting_Save(vConfigurationFpsOverlayer, "TextSize", slider_TextSize.Value.ToString("0"));
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };

                checkbox_TextColorSingle.Click += async (sender, e) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "TextColorSingle", checkbox_TextColorSingle.IsChecked.ToString());
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };

                textbox_GpuCategoryTitle.TextChanged += async (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "GpuCategoryTitle", senderTextbox.Text);
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_GpuShowCategoryTitle.Click += async (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "GpuShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_GpuShowName.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "GpuShowName", checkbox_GpuShowName.IsChecked.ToString()); };
                checkbox_GpuShowPercentage.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "GpuShowPercentage", checkbox_GpuShowPercentage.IsChecked.ToString()); };
                checkbox_GpuShowMemoryUsed.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "GpuShowMemoryUsed", checkbox_GpuShowMemoryUsed.IsChecked.ToString()); };
                checkbox_GpuShowTemperature.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "GpuShowTemperature", checkbox_GpuShowTemperature.IsChecked.ToString()); };
                checkbox_GpuShowCoreFrequency.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "GpuShowCoreFrequency", checkbox_GpuShowCoreFrequency.IsChecked.ToString()); };
                checkbox_GpuShowFanSpeed.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "GpuShowFanSpeed", checkbox_GpuShowFanSpeed.IsChecked.ToString()); };
                checkbox_GpuShowPowerWatt.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "GpuShowPowerWatt", checkbox_GpuShowPowerWatt.IsChecked.ToString()); };
                checkbox_GpuShowPowerVolt.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "GpuShowPowerVolt", checkbox_GpuShowPowerVolt.IsChecked.ToString()); };

                textbox_CpuCategoryTitle.TextChanged += async (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "CpuCategoryTitle", senderTextbox.Text);
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_CpuShowCategoryTitle.Click += async (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "CpuShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_CpuShowName.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "CpuShowName", checkbox_CpuShowName.IsChecked.ToString()); };
                checkbox_CpuShowPercentage.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "CpuShowPercentage", checkbox_CpuShowPercentage.IsChecked.ToString()); };
                checkbox_CpuShowTemperature.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "CpuShowTemperature", checkbox_CpuShowTemperature.IsChecked.ToString()); };
                checkbox_CpuShowCoreFrequency.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "CpuShowCoreFrequency", checkbox_CpuShowCoreFrequency.IsChecked.ToString()); };
                checkbox_CpuShowPowerWatt.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "CpuShowPowerWatt", checkbox_CpuShowPowerWatt.IsChecked.ToString()); };
                checkbox_CpuShowPowerVolt.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "CpuShowPowerVolt", checkbox_CpuShowPowerVolt.IsChecked.ToString()); };
                checkbox_CpuShowFanSpeed.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "CpuShowFanSpeed", checkbox_CpuShowFanSpeed.IsChecked.ToString()); };

                textbox_MemCategoryTitle.TextChanged += async (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "MemCategoryTitle", senderTextbox.Text);
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_MemShowCategoryTitle.Click += async (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "MemShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_MemShowPercentage.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "MemShowPercentage", checkbox_MemShowPercentage.IsChecked.ToString()); };
                checkbox_MemShowUsed.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "MemShowUsed", checkbox_MemShowUsed.IsChecked.ToString()); };
                checkbox_MemShowFree.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "MemShowFree", checkbox_MemShowFree.IsChecked.ToString()); };
                checkbox_MemShowTotal.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "MemShowTotal", checkbox_MemShowTotal.IsChecked.ToString()); };

                textbox_NetCategoryTitle.TextChanged += async (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "NetCategoryTitle", senderTextbox.Text);
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_NetShowCategoryTitle.Click += async (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "NetShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_NetShowCurrentUsage.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "NetShowCurrentUsage", checkbox_NetShowCurrentUsage.IsChecked.ToString()); };

                checkbox_AppShowName.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "AppShowName", checkbox_AppShowName.IsChecked.ToString()); };
                checkbox_TimeShowCurrentTime.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "TimeShowCurrentTime", checkbox_TimeShowCurrentTime.IsChecked.ToString()); };

                textbox_MonCategoryTitle.TextChanged += async (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "MonCategoryTitle", senderTextbox.Text);
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_MonShowCategoryTitle.Click += async (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "MonShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_MonShowResolution.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "MonShowResolution", checkbox_MonShowResolution.IsChecked.ToString()); };
                checkbox_MonShowDpiResolution.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "MonShowDpiResolution", checkbox_MonShowDpiResolution.IsChecked.ToString()); };
                checkbox_MonShowColorBitDepth.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "MonShowColorBitDepth", checkbox_MonShowColorBitDepth.IsChecked.ToString()); };
                checkbox_MonShowRefreshRate.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "MonShowRefreshRate", checkbox_MonShowRefreshRate.IsChecked.ToString()); };

                textbox_FpsCategoryTitle.TextChanged += async (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "FpsCategoryTitle", senderTextbox.Text);
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_FpsShowCategoryTitle.Click += async (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "FpsShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    await App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_FpsShowCurrentFps.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "FpsShowCurrentFps", checkbox_FpsShowCurrentFps.IsChecked.ToString()); };
                checkbox_FpsShowCurrentLatency.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "FpsShowCurrentLatency", checkbox_FpsShowCurrentLatency.IsChecked.ToString()); };
                checkbox_FpsShowAverageFps.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "FpsShowAverageFps", checkbox_FpsShowAverageFps.IsChecked.ToString()); };

                colorpicker_ColorSingle.Click += async (sender, e) =>
                {
                    Color? newColor = await new AVColorPicker().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorSingle.Background = newBrush;
                        Setting_Save(vConfigurationFpsOverlayer, "ColorSingle", newColor.ToString());
                        await App.vWindowMain.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorBackground.Click += async (sender, e) =>
                {
                    Color? newColor = await new AVColorPicker().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorBackground.Background = newBrush;
                        Setting_Save(vConfigurationFpsOverlayer, "ColorBackground", newColor.ToString());
                        await App.vWindowMain.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorGpu.Click += async (sender, e) =>
                {
                    Color? newColor = await new AVColorPicker().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorGpu.Background = newBrush;
                        Setting_Save(vConfigurationFpsOverlayer, "ColorGpu", newColor.ToString());
                        await App.vWindowMain.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorCpu.Click += async (sender, e) =>
                {
                    Color? newColor = await new AVColorPicker().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorCpu.Background = newBrush;
                        Setting_Save(vConfigurationFpsOverlayer, "ColorCpu", newColor.ToString());
                        await App.vWindowMain.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorMem.Click += async (sender, e) =>
                {
                    Color? newColor = await new AVColorPicker().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorMem.Background = newBrush;
                        Setting_Save(vConfigurationFpsOverlayer, "ColorMem", newColor.ToString());
                        await App.vWindowMain.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorNet.Click += async (sender, e) =>
                {
                    Color? newColor = await new AVColorPicker().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorNet.Background = newBrush;
                        Setting_Save(vConfigurationFpsOverlayer, "ColorNet", newColor.ToString());
                        await App.vWindowMain.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorApp.Click += async (sender, e) =>
                {
                    Color? newColor = await new AVColorPicker().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorApp.Background = newBrush;
                        Setting_Save(vConfigurationFpsOverlayer, "ColorApp", newColor.ToString());
                        await App.vWindowMain.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorTime.Click += async (sender, e) =>
                {
                    Color? newColor = await new AVColorPicker().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorTime.Background = newBrush;
                        Setting_Save(vConfigurationFpsOverlayer, "ColorTime", newColor.ToString());
                        await App.vWindowMain.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorMon.Click += async (sender, e) =>
                {
                    Color? newColor = await new AVColorPicker().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorMon.Background = newBrush;
                        Setting_Save(vConfigurationFpsOverlayer, "ColorMon", newColor.ToString());
                        await App.vWindowMain.UpdateFpsOverlayStyle();
                    }
                };

                colorpicker_ColorFps.Click += async (sender, e) =>
                {
                    Color? newColor = await new AVColorPicker().Popup(null);
                    if (newColor != null)
                    {
                        SolidColorBrush newBrush = new SolidColorBrush((Color)newColor);
                        colorpicker_ColorFps.Background = newBrush;
                        Setting_Save(vConfigurationFpsOverlayer, "ColorFps", newColor.ToString());
                        await App.vWindowMain.UpdateFpsOverlayStyle();
                    }
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save the application settings: " + ex.Message);
            }
        }
    }
}