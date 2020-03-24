using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Media;
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
                checkbox_CategoryTitles.Click += (sender, e) =>
                {
                    SettingSave("CategoryTitles", checkbox_CategoryTitles.IsChecked.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                checkbox_DisplayBackground.Click += (sender, e) =>
                {
                    SettingSave("DisplayBackground", checkbox_DisplayBackground.IsChecked.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                slider_DisplayOpacity.ValueChanged += (sender, e) =>
                {
                    textblock_DisplayOpacity.Text = textblock_DisplayOpacity.Tag + ": " + slider_DisplayOpacity.Value.ToString("0.00") + "%";
                    SettingSave("DisplayOpacity", slider_DisplayOpacity.Value.ToString("0.00"));
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                slider_MarginHorizontal.ValueChanged += (sender, e) =>
                {
                    textblock_MarginHorizontal.Text = textblock_MarginHorizontal.Tag + ": " + slider_MarginHorizontal.Value.ToString("0") + "px";
                    SettingSave("MarginHorizontal", slider_MarginHorizontal.Value.ToString("0"));
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                slider_MarginVertical.ValueChanged += (sender, e) =>
                {
                    textblock_MarginVertical.Text = textblock_MarginVertical.Tag + ": " + slider_MarginVertical.Value.ToString("0") + "px";
                    SettingSave("MarginVertical", slider_MarginVertical.Value.ToString("0"));
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                combobox_TextFont.SelectionChanged += (sender, e) =>
                {
                    SettingSave("TextFont", combobox_TextFont.SelectedIndex.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                combobox_TextPosition.SelectionChanged += (sender, e) =>
                {
                    SettingSave("TextPosition", combobox_TextPosition.SelectedIndex.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                combobox_TextDirection.SelectionChanged += (sender, e) =>
                {
                    SettingSave("TextDirection", combobox_TextDirection.SelectedIndex.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                slider_TextSize.ValueChanged += (sender, e) =>
                {
                    textblock_TextSize.Text = textblock_TextSize.Tag + ": " + slider_TextSize.Value.ToString("0") + "px";
                    SettingSave("TextSize", slider_TextSize.Value.ToString("0"));
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                checkbox_TextColorSingle.Click += (sender, e) =>
                {
                    SettingSave("TextColorSingle", checkbox_TextColorSingle.IsChecked.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                checkbox_GpuShowPercentage.Click += (sender, e) => { SettingSave("GpuShowPercentage", checkbox_GpuShowPercentage.IsChecked.ToString()); };
                checkbox_GpuShowMemoryUsed.Click += (sender, e) => { SettingSave("GpuShowMemoryUsed", checkbox_GpuShowMemoryUsed.IsChecked.ToString()); };
                checkbox_GpuShowTemperature.Click += (sender, e) => { SettingSave("GpuShowTemperature", checkbox_GpuShowTemperature.IsChecked.ToString()); };
                checkbox_GpuShowCoreFrequency.Click += (sender, e) => { SettingSave("GpuShowCoreFrequency", checkbox_GpuShowCoreFrequency.IsChecked.ToString()); };
                checkbox_GpuShowFanSpeed.Click += (sender, e) => { SettingSave("GpuShowFanSpeed", checkbox_GpuShowFanSpeed.IsChecked.ToString()); };

                checkbox_CpuShowPercentage.Click += (sender, e) => { SettingSave("CpuShowPercentage", checkbox_CpuShowPercentage.IsChecked.ToString()); };
                checkbox_CpuShowTemperature.Click += (sender, e) => { SettingSave("CpuShowTemperature", checkbox_CpuShowTemperature.IsChecked.ToString()); };
                checkbox_CpuShowCoreFrequency.Click += (sender, e) => { SettingSave("CpuShowCoreFrequency", checkbox_CpuShowCoreFrequency.IsChecked.ToString()); };
                checkbox_CpuShowPowerUsage.Click += (sender, e) => { SettingSave("CpuShowPowerUsage", checkbox_CpuShowPowerUsage.IsChecked.ToString()); };

                checkbox_MemShowPercentage.Click += (sender, e) => { SettingSave("MemShowPercentage", checkbox_MemShowPercentage.IsChecked.ToString()); };
                checkbox_MemShowUsed.Click += (sender, e) => { SettingSave("MemShowUsed", checkbox_MemShowUsed.IsChecked.ToString()); };
                checkbox_MemShowFree.Click += (sender, e) => { SettingSave("MemShowFree", checkbox_MemShowFree.IsChecked.ToString()); };
                checkbox_MemShowTotal.Click += (sender, e) => { SettingSave("MemShowTotal", checkbox_MemShowTotal.IsChecked.ToString()); };

                checkbox_NetShowCurrentUsage.Click += (sender, e) => { SettingSave("NetShowCurrentUsage", checkbox_NetShowCurrentUsage.IsChecked.ToString()); };
                checkbox_AppShowName.Click += (sender, e) => { SettingSave("AppShowName", checkbox_AppShowName.IsChecked.ToString()); };
                checkbox_TimeShowCurrentTime.Click += (sender, e) => { SettingSave("TimeShowCurrentTime", checkbox_TimeShowCurrentTime.IsChecked.ToString()); };

                checkbox_FpsShowCurrentFps.Click += (sender, e) => { SettingSave("FpsShowCurrentFps", checkbox_FpsShowCurrentFps.IsChecked.ToString()); };
                checkbox_FpsShowCurrentLatency.Click += (sender, e) => { SettingSave("FpsShowCurrentLatency", checkbox_FpsShowCurrentLatency.IsChecked.ToString()); };
                checkbox_FpsShowAverageFps.Click += (sender, e) => { SettingSave("FpsShowAverageFps", checkbox_FpsShowAverageFps.IsChecked.ToString()); };

                colorpicker_ColorSingle.SelectedColorChanged += (Color color) =>
                {
                    SettingSave("ColorSingle", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorBackground.SelectedColorChanged += (Color color) =>
                {
                    SettingSave("ColorBackground", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorGpu.SelectedColorChanged += (Color color) =>
                {
                    SettingSave("ColorGpu", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorCpu.SelectedColorChanged += (Color color) =>
                {
                    SettingSave("ColorCpu", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorMem.SelectedColorChanged += (Color color) =>
                {
                    SettingSave("ColorMem", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorNet.SelectedColorChanged += (Color color) =>
                {
                    SettingSave("ColorNet", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorApp.SelectedColorChanged += (Color color) =>
                {
                    SettingSave("ColorApp", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorTime.SelectedColorChanged += (Color color) =>
                {
                    SettingSave("ColorTime", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };

                colorpicker_ColorFps.SelectedColorChanged += (Color color) =>
                {
                    SettingSave("ColorFps", color.ToString());
                    App.vWindowMain.UpdateFpsOverlayStyle();
                };
            }
            catch (Exception Ex)
            {
                Debug.WriteLine("Failed to save the application settings: " + Ex.Message);
            }
        }

        //Save - Application Setting
        public void SettingSave(string Name, string Value)
        {
            try
            {
                vConfiguration.AppSettings.Settings.Remove(Name);
                vConfiguration.AppSettings.Settings.Add(Name, Value);
                vConfiguration.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch { }
        }
    }
}