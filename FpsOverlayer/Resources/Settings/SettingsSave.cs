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
                    SettingSave(vConfigurationApplication, "DisplayBackground", checkbox_DisplayBackground.IsChecked.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                slider_DisplayOpacity.ValueChanged += (sender, e) =>
                {
                    textblock_DisplayOpacity.Text = textblock_DisplayOpacity.Tag + ": " + slider_DisplayOpacity.Value.ToString("0.00") + "%";
                    SettingSave(vConfigurationApplication, "DisplayOpacity", slider_DisplayOpacity.Value.ToString("0.00"));
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                slider_HardwareUpdateRateMs.ValueChanged += (sender, e) =>
                {
                    textblock_HardwareUpdateRateMs.Text = textblock_HardwareUpdateRateMs.Tag + ": " + slider_HardwareUpdateRateMs.Value.ToString() + "ms";
                    SettingSave(vConfigurationApplication, "HardwareUpdateRateMs", slider_HardwareUpdateRateMs.Value.ToString());
                };

                slider_MarginHorizontal.ValueChanged += (sender, e) =>
                {
                    textblock_MarginHorizontal.Text = textblock_MarginHorizontal.Tag + ": " + slider_MarginHorizontal.Value.ToString("0") + "px";
                    SettingSave(vConfigurationApplication, "MarginHorizontal", slider_MarginHorizontal.Value.ToString("0"));
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                slider_MarginVertical.ValueChanged += (sender, e) =>
                {
                    textblock_MarginVertical.Text = textblock_MarginVertical.Tag + ": " + slider_MarginVertical.Value.ToString("0") + "px";
                    SettingSave(vConfigurationApplication, "MarginVertical", slider_MarginVertical.Value.ToString("0"));
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                combobox_InterfaceFontStyleName.SelectionChanged += (sender, e) =>
                {
                    SettingSave(vConfigurationApplication, "InterfaceFontStyleName", combobox_InterfaceFontStyleName.SelectedItem.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                combobox_TextPosition.SelectionChanged += async (sender, e) =>
                {
                    SettingSave(vConfigurationApplication, "TextPosition", combobox_TextPosition.SelectedIndex.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                    await NotifyDirectXInputSettingChanged("TextPosition");
                };

                combobox_TextDirection.SelectionChanged += (sender, e) =>
                {
                    SettingSave(vConfigurationApplication, "TextDirection", combobox_TextDirection.SelectedIndex.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                slider_TextSize.ValueChanged += (sender, e) =>
                {
                    textblock_TextSize.Text = textblock_TextSize.Tag + ": " + slider_TextSize.Value.ToString("0") + "px";
                    SettingSave(vConfigurationApplication, "TextSize", slider_TextSize.Value.ToString("0"));
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                checkbox_TextColorSingle.Click += (sender, e) =>
                {
                    SettingSave(vConfigurationApplication, "TextColorSingle", checkbox_TextColorSingle.IsChecked.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                textbox_GpuCategoryTitle.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    SettingSave(vConfigurationApplication, "GpuCategoryTitle", senderTextbox.Text);
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_GpuShowCategoryTitle.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    SettingSave(vConfigurationApplication, "GpuShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_GpuShowName.Click += (sender, e) => { SettingSave(vConfigurationApplication, "GpuShowName", checkbox_GpuShowName.IsChecked.ToString()); };
                checkbox_GpuShowPercentage.Click += (sender, e) => { SettingSave(vConfigurationApplication, "GpuShowPercentage", checkbox_GpuShowPercentage.IsChecked.ToString()); };
                checkbox_GpuShowMemoryUsed.Click += (sender, e) => { SettingSave(vConfigurationApplication, "GpuShowMemoryUsed", checkbox_GpuShowMemoryUsed.IsChecked.ToString()); };
                checkbox_GpuShowTemperature.Click += (sender, e) => { SettingSave(vConfigurationApplication, "GpuShowTemperature", checkbox_GpuShowTemperature.IsChecked.ToString()); };
                checkbox_GpuShowCoreFrequency.Click += (sender, e) => { SettingSave(vConfigurationApplication, "GpuShowCoreFrequency", checkbox_GpuShowCoreFrequency.IsChecked.ToString()); };
                checkbox_GpuShowFanSpeed.Click += (sender, e) => { SettingSave(vConfigurationApplication, "GpuShowFanSpeed", checkbox_GpuShowFanSpeed.IsChecked.ToString()); };

                textbox_CpuCategoryTitle.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    SettingSave(vConfigurationApplication, "CpuCategoryTitle", senderTextbox.Text);
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_CpuShowCategoryTitle.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    SettingSave(vConfigurationApplication, "CpuShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_CpuShowName.Click += (sender, e) => { SettingSave(vConfigurationApplication, "CpuShowName", checkbox_CpuShowName.IsChecked.ToString()); };
                checkbox_CpuShowPercentage.Click += (sender, e) => { SettingSave(vConfigurationApplication, "CpuShowPercentage", checkbox_CpuShowPercentage.IsChecked.ToString()); };
                checkbox_CpuShowTemperature.Click += (sender, e) => { SettingSave(vConfigurationApplication, "CpuShowTemperature", checkbox_CpuShowTemperature.IsChecked.ToString()); };
                checkbox_CpuShowCoreFrequency.Click += (sender, e) => { SettingSave(vConfigurationApplication, "CpuShowCoreFrequency", checkbox_CpuShowCoreFrequency.IsChecked.ToString()); };
                checkbox_CpuShowPowerUsage.Click += (sender, e) => { SettingSave(vConfigurationApplication, "CpuShowPowerUsage", checkbox_CpuShowPowerUsage.IsChecked.ToString()); };

                textbox_MemCategoryTitle.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    SettingSave(vConfigurationApplication, "MemCategoryTitle", senderTextbox.Text);
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_MemShowCategoryTitle.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    SettingSave(vConfigurationApplication, "MemShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_MemShowPercentage.Click += (sender, e) => { SettingSave(vConfigurationApplication, "MemShowPercentage", checkbox_MemShowPercentage.IsChecked.ToString()); };
                checkbox_MemShowUsed.Click += (sender, e) => { SettingSave(vConfigurationApplication, "MemShowUsed", checkbox_MemShowUsed.IsChecked.ToString()); };
                checkbox_MemShowFree.Click += (sender, e) => { SettingSave(vConfigurationApplication, "MemShowFree", checkbox_MemShowFree.IsChecked.ToString()); };
                checkbox_MemShowTotal.Click += (sender, e) => { SettingSave(vConfigurationApplication, "MemShowTotal", checkbox_MemShowTotal.IsChecked.ToString()); };

                textbox_NetCategoryTitle.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    SettingSave(vConfigurationApplication, "NetCategoryTitle", senderTextbox.Text);
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_NetShowCategoryTitle.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    SettingSave(vConfigurationApplication, "NetShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_NetShowCurrentUsage.Click += (sender, e) => { SettingSave(vConfigurationApplication, "NetShowCurrentUsage", checkbox_NetShowCurrentUsage.IsChecked.ToString()); };

                checkbox_AppShowName.Click += (sender, e) => { SettingSave(vConfigurationApplication, "AppShowName", checkbox_AppShowName.IsChecked.ToString()); };
                checkbox_TimeShowCurrentTime.Click += (sender, e) => { SettingSave(vConfigurationApplication, "TimeShowCurrentTime", checkbox_TimeShowCurrentTime.IsChecked.ToString()); };

                textbox_MonCategoryTitle.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    SettingSave(vConfigurationApplication, "MonCategoryTitle", senderTextbox.Text);
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_MonShowCategoryTitle.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    SettingSave(vConfigurationApplication, "MonShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_MonShowResolution.Click += (sender, e) => { SettingSave(vConfigurationApplication, "MonShowResolution", checkbox_MonShowResolution.IsChecked.ToString()); };
                checkbox_MonShowDpiResolution.Click += (sender, e) => { SettingSave(vConfigurationApplication, "MonShowDpiResolution", checkbox_MonShowDpiResolution.IsChecked.ToString()); };
                checkbox_MonShowColorBitDepth.Click += (sender, e) => { SettingSave(vConfigurationApplication, "MonShowColorBitDepth", checkbox_MonShowColorBitDepth.IsChecked.ToString()); };
                checkbox_MonShowRefreshRate.Click += (sender, e) => { SettingSave(vConfigurationApplication, "MonShowRefreshRate", checkbox_MonShowRefreshRate.IsChecked.ToString()); };

                textbox_FpsCategoryTitle.TextChanged += (sender, e) =>
                {
                    TextBox senderTextbox = (TextBox)sender;
                    SettingSave(vConfigurationApplication, "FpsCategoryTitle", senderTextbox.Text);
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_FpsShowCategoryTitle.Click += (sender, e) =>
                {
                    CheckBox senderCheckBox = (CheckBox)sender;
                    SettingSave(vConfigurationApplication, "FpsShowCategoryTitle", senderCheckBox.IsChecked.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
                checkbox_FpsShowCurrentFps.Click += (sender, e) => { SettingSave(vConfigurationApplication, "FpsShowCurrentFps", checkbox_FpsShowCurrentFps.IsChecked.ToString()); };
                checkbox_FpsShowCurrentLatency.Click += (sender, e) => { SettingSave(vConfigurationApplication, "FpsShowCurrentLatency", checkbox_FpsShowCurrentLatency.IsChecked.ToString()); };
                checkbox_FpsShowAverageFps.Click += (sender, e) => { SettingSave(vConfigurationApplication, "FpsShowAverageFps", checkbox_FpsShowAverageFps.IsChecked.ToString()); };

                colorpicker_ColorSingle.SelectedColorChanged += (Color color) =>
                {
                    SettingSave(vConfigurationApplication, "ColorSingle", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorBackground.SelectedColorChanged += (Color color) =>
                {
                    SettingSave(vConfigurationApplication, "ColorBackground", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorGpu.SelectedColorChanged += (Color color) =>
                {
                    SettingSave(vConfigurationApplication, "ColorGpu", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorCpu.SelectedColorChanged += (Color color) =>
                {
                    SettingSave(vConfigurationApplication, "ColorCpu", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorMem.SelectedColorChanged += (Color color) =>
                {
                    SettingSave(vConfigurationApplication, "ColorMem", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorNet.SelectedColorChanged += (Color color) =>
                {
                    SettingSave(vConfigurationApplication, "ColorNet", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorApp.SelectedColorChanged += (Color color) =>
                {
                    SettingSave(vConfigurationApplication, "ColorApp", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorTime.SelectedColorChanged += (Color color) =>
                {
                    SettingSave(vConfigurationApplication, "ColorTime", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorMon.SelectedColorChanged += (Color color) =>
                {
                    SettingSave(vConfigurationApplication, "ColorMon", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorFps.SelectedColorChanged += (Color color) =>
                {
                    SettingSave(vConfigurationApplication, "ColorFps", color.ToString());
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