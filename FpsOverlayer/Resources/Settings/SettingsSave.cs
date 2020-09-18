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
                checkbox_DisplayBackground.Click += (sender, e) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "DisplayBackground", checkbox_DisplayBackground.IsChecked.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                slider_DisplayOpacity.ValueChanged += (sender, e) =>
                {
                    textblock_DisplayOpacity.Text = textblock_DisplayOpacity.Tag + ": " + slider_DisplayOpacity.Value.ToString("0.00") + "%";
                    Setting_Save(vConfigurationFpsOverlayer, "DisplayOpacity", slider_DisplayOpacity.Value.ToString("0.00"));
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                slider_HardwareUpdateRateMs.ValueChanged += (sender, e) =>
                {
                    textblock_HardwareUpdateRateMs.Text = textblock_HardwareUpdateRateMs.Tag + ": " + slider_HardwareUpdateRateMs.Value.ToString() + "ms";
                    Setting_Save(vConfigurationFpsOverlayer, "HardwareUpdateRateMs", slider_HardwareUpdateRateMs.Value.ToString());
                };

                slider_MarginHorizontal.ValueChanged += (sender, e) =>
                {
                    textblock_MarginHorizontal.Text = textblock_MarginHorizontal.Tag + ": " + slider_MarginHorizontal.Value.ToString("0") + "px";
                    Setting_Save(vConfigurationFpsOverlayer, "MarginHorizontal", slider_MarginHorizontal.Value.ToString("0"));
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                slider_MarginVertical.ValueChanged += (sender, e) =>
                {
                    textblock_MarginVertical.Text = textblock_MarginVertical.Tag + ": " + slider_MarginVertical.Value.ToString("0") + "px";
                    Setting_Save(vConfigurationFpsOverlayer, "MarginVertical", slider_MarginVertical.Value.ToString("0"));
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                combobox_InterfaceFontStyleName.SelectionChanged += (sender, e) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "InterfaceFontStyleName", combobox_InterfaceFontStyleName.SelectedItem.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                combobox_TextPosition.SelectionChanged += async (sender, e) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "TextPosition", combobox_TextPosition.SelectedIndex.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                    await NotifyDirectXInputSettingChanged("TextPosition");
                };

                combobox_TextDirection.SelectionChanged += (sender, e) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "TextDirection", combobox_TextDirection.SelectedIndex.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                slider_TextSize.ValueChanged += (sender, e) =>
                {
                    textblock_TextSize.Text = textblock_TextSize.Tag + ": " + slider_TextSize.Value.ToString("0") + "px";
                    Setting_Save(vConfigurationFpsOverlayer, "TextSize", slider_TextSize.Value.ToString("0"));
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                checkbox_TextColorSingle.Click += (sender, e) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "TextColorSingle", checkbox_TextColorSingle.IsChecked.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                textbox_GpuCategoryTitle.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "GpuCategoryTitle", senderTextbox.Text);
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_GpuShowCategoryTitle.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "GpuShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_GpuShowName.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "GpuShowName", checkbox_GpuShowName.IsChecked.ToString()); };
                checkbox_GpuShowPercentage.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "GpuShowPercentage", checkbox_GpuShowPercentage.IsChecked.ToString()); };
                checkbox_GpuShowMemoryUsed.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "GpuShowMemoryUsed", checkbox_GpuShowMemoryUsed.IsChecked.ToString()); };
                checkbox_GpuShowTemperature.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "GpuShowTemperature", checkbox_GpuShowTemperature.IsChecked.ToString()); };
                checkbox_GpuShowCoreFrequency.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "GpuShowCoreFrequency", checkbox_GpuShowCoreFrequency.IsChecked.ToString()); };
                checkbox_GpuShowFanSpeed.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "GpuShowFanSpeed", checkbox_GpuShowFanSpeed.IsChecked.ToString()); };
                checkbox_GpuShowPowerUsage.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "GpuShowPowerUsage", checkbox_GpuShowPowerUsage.IsChecked.ToString()); };

                textbox_CpuCategoryTitle.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "CpuCategoryTitle", senderTextbox.Text);
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_CpuShowCategoryTitle.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "CpuShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_CpuShowName.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "CpuShowName", checkbox_CpuShowName.IsChecked.ToString()); };
                checkbox_CpuShowPercentage.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "CpuShowPercentage", checkbox_CpuShowPercentage.IsChecked.ToString()); };
                checkbox_CpuShowTemperature.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "CpuShowTemperature", checkbox_CpuShowTemperature.IsChecked.ToString()); };
                checkbox_CpuShowCoreFrequency.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "CpuShowCoreFrequency", checkbox_CpuShowCoreFrequency.IsChecked.ToString()); };
                checkbox_CpuShowPowerUsage.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "CpuShowPowerUsage", checkbox_CpuShowPowerUsage.IsChecked.ToString()); };

                textbox_MemCategoryTitle.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "MemCategoryTitle", senderTextbox.Text);
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_MemShowCategoryTitle.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "MemShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_MemShowPercentage.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "MemShowPercentage", checkbox_MemShowPercentage.IsChecked.ToString()); };
                checkbox_MemShowUsed.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "MemShowUsed", checkbox_MemShowUsed.IsChecked.ToString()); };
                checkbox_MemShowFree.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "MemShowFree", checkbox_MemShowFree.IsChecked.ToString()); };
                checkbox_MemShowTotal.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "MemShowTotal", checkbox_MemShowTotal.IsChecked.ToString()); };

                textbox_NetCategoryTitle.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "NetCategoryTitle", senderTextbox.Text);
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_NetShowCategoryTitle.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "NetShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_NetShowCurrentUsage.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "NetShowCurrentUsage", checkbox_NetShowCurrentUsage.IsChecked.ToString()); };

                checkbox_AppShowName.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "AppShowName", checkbox_AppShowName.IsChecked.ToString()); };
                checkbox_TimeShowCurrentTime.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "TimeShowCurrentTime", checkbox_TimeShowCurrentTime.IsChecked.ToString()); };

                textbox_MonCategoryTitle.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "MonCategoryTitle", senderTextbox.Text);
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_MonShowCategoryTitle.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "MonShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_MonShowResolution.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "MonShowResolution", checkbox_MonShowResolution.IsChecked.ToString()); };
                checkbox_MonShowDpiResolution.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "MonShowDpiResolution", checkbox_MonShowDpiResolution.IsChecked.ToString()); };
                checkbox_MonShowColorBitDepth.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "MonShowColorBitDepth", checkbox_MonShowColorBitDepth.IsChecked.ToString()); };
                checkbox_MonShowRefreshRate.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "MonShowRefreshRate", checkbox_MonShowRefreshRate.IsChecked.ToString()); };

                textbox_FpsCategoryTitle.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "FpsCategoryTitle", senderTextbox.Text);
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_FpsShowCategoryTitle.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    Setting_Save(vConfigurationFpsOverlayer, "FpsShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_FpsShowCurrentFps.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "FpsShowCurrentFps", checkbox_FpsShowCurrentFps.IsChecked.ToString()); };
                checkbox_FpsShowCurrentLatency.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "FpsShowCurrentLatency", checkbox_FpsShowCurrentLatency.IsChecked.ToString()); };
                checkbox_FpsShowAverageFps.Click += (sender, e) => { Setting_Save(vConfigurationFpsOverlayer, "FpsShowAverageFps", checkbox_FpsShowAverageFps.IsChecked.ToString()); };

                colorpicker_ColorSingle.SelectedColorChanged += (Color color) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "ColorSingle", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorBackground.SelectedColorChanged += (Color color) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "ColorBackground", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorGpu.SelectedColorChanged += (Color color) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "ColorGpu", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorCpu.SelectedColorChanged += (Color color) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "ColorCpu", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorMem.SelectedColorChanged += (Color color) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "ColorMem", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorNet.SelectedColorChanged += (Color color) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "ColorNet", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorApp.SelectedColorChanged += (Color color) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "ColorApp", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorTime.SelectedColorChanged += (Color color) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "ColorTime", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorMon.SelectedColorChanged += (Color color) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "ColorMon", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorFps.SelectedColorChanged += (Color color) =>
                {
                    Setting_Save(vConfigurationFpsOverlayer, "ColorFps", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save the application settings: " + ex.Message);
            }
        }
    }
}