using ArnoldVinkCode;
using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkCode.AVSettings;
using static ScreenCapture.AppVariables;

namespace ScreenCapture
{
    partial class WindowMain
    {
        //Save - Application Settings
        public void Settings_Save()
        {
            try
            {
                //General
                checkbox_CaptureSoundEffect.Click += (sender, e) =>
                {
                    SettingSave(vConfiguration, "CaptureSoundEffect", checkbox_CaptureSoundEffect.IsChecked);
                };

                checkbox_CaptureDrawBorder.Click += (sender, e) =>
                {
                    SettingSave(vConfiguration, "CaptureDrawBorder", checkbox_CaptureDrawBorder.IsChecked);
                };

                checkbox_CaptureDrawMouseCursor.Click += (sender, e) =>
                {
                    SettingSave(vConfiguration, "CaptureDrawMouseCursor", checkbox_CaptureDrawMouseCursor.IsChecked);
                };

                slider_CaptureMonitorId.ValueChanged += (sender, e) =>
                {
                    textblock_CaptureMonitorId.Text = textblock_CaptureMonitorId.Tag + slider_CaptureMonitorId.Value.ToString();
                    SettingSave(vConfiguration, "CaptureMonitorId", slider_CaptureMonitorId.Value);
                };

                //Screenshot
                combobox_ScreenshotSaveFormat.SelectionChanged += (sender, e) =>
                {
                    SettingSave(vConfiguration, "ScreenshotSaveFormat", combobox_ScreenshotSaveFormat.SelectedIndex);
                };

                slider_ScreenshotSaveQuality.ValueChanged += (sender, e) =>
                {
                    textblock_ScreenshotSaveQuality.Text = textblock_ScreenshotSaveQuality.Tag + slider_ScreenshotSaveQuality.Value.ToString() + "%";
                    SettingSave(vConfiguration, "ScreenshotSaveQuality", slider_ScreenshotSaveQuality.Value);
                };

                slider_ScreenshotMaxPixelDimension.ValueChanged += (sender, e) =>
                {
                    textblock_ScreenshotMaxPixelDimension.Text = textblock_ScreenshotMaxPixelDimension.Tag + slider_ScreenshotMaxPixelDimension.Value.ToString() + "px";
                    SettingSave(vConfiguration, "ScreenshotMaxPixelDimension", slider_ScreenshotMaxPixelDimension.Value);
                };

                //Recording
                combobox_VideoSaveFormat.SelectionChanged += (sender, e) =>
                {
                    SettingSave(vConfiguration, "VideoSaveFormat", combobox_VideoSaveFormat.SelectedIndex);
                };

                combobox_VideoFrameRate.SelectionChanged += (sender, e) =>
                {
                    ComboBoxItemValue saveValue = (ComboBoxItemValue)combobox_VideoFrameRate.SelectedItem;
                    SettingSave(vConfiguration, "VideoFrameRate", saveValue.Value);
                };

                combobox_VideoRateControl.SelectionChanged += (sender, e) =>
                {
                    SettingSave(vConfiguration, "VideoRateControl", combobox_VideoRateControl.SelectedIndex);
                };

                slider_VideoBitRate.ValueChanged += (sender, e) =>
                {
                    textblock_VideoBitRate.Text = textblock_VideoBitRate.Tag + slider_VideoBitRate.Value.ToString() + " Kbps";
                    SettingSave(vConfiguration, "VideoBitRate", slider_VideoBitRate.Value);
                };

                slider_VideoMaxPixelDimension.ValueChanged += (sender, e) =>
                {
                    textblock_VideoMaxPixelDimension.Text = textblock_VideoMaxPixelDimension.Tag + slider_VideoMaxPixelDimension.Value.ToString() + "px";
                    SettingSave(vConfiguration, "VideoMaxPixelDimension", slider_VideoMaxPixelDimension.Value);
                };

                //Audio
                combobox_AudioSaveFormat.SelectionChanged += (sender, e) =>
                {
                    SettingSave(vConfiguration, "AudioSaveFormat", combobox_AudioSaveFormat.SelectedIndex);
                };

                combobox_AudioChannels.SelectionChanged += (sender, e) =>
                {
                    ComboBoxItemValue saveValue = (ComboBoxItemValue)combobox_AudioChannels.SelectedItem;
                    SettingSave(vConfiguration, "AudioChannels", saveValue.Value);
                };

                slider_AudioBitRate.ValueChanged += (sender, e) =>
                {
                    textblock_AudioBitRate.Text = textblock_AudioBitRate.Tag + slider_AudioBitRate.Value.ToString() + " kbps";
                    SettingSave(vConfiguration, "AudioBitRate", slider_AudioBitRate.Value);
                };

                combobox_AudioBitDepth.SelectionChanged += (sender, e) =>
                {
                    ComboBoxItemValue saveValue = (ComboBoxItemValue)combobox_AudioBitDepth.SelectedItem;
                    SettingSave(vConfiguration, "AudioBitDepth", saveValue.Value);
                };

                combobox_AudioSampleRate.SelectionChanged += (sender, e) =>
                {
                    ComboBoxItemValue saveValue = (ComboBoxItemValue)combobox_AudioSampleRate.SelectedItem;
                    SettingSave(vConfiguration, "AudioSampleRate", saveValue.Value);
                };

                //Overlay
                checkbox_OverlayShowScreenshot.Click += (sender, e) =>
                {
                    SettingSave(vConfiguration, "OverlayShowScreenshot", checkbox_OverlayShowScreenshot.IsChecked);
                };

                checkbox_OverlayShowRecording.Click += (sender, e) =>
                {
                    SettingSave(vConfiguration, "OverlayShowRecording", checkbox_OverlayShowRecording.IsChecked);
                };

                combobox_OverlayPosition.SelectionChanged += (sender, e) =>
                {
                    ComboBoxItemValue saveValue = (ComboBoxItemValue)combobox_OverlayPosition.SelectedItem;
                    SettingSave(vConfiguration, "OverlayPosition", saveValue.Value);
                };

                //Launch on Windows startup
                checkbox_WindowsStartup.Click += (sender, e) =>
                {
                    try
                    {
                        AVSettings.StartupShortcutManage(string.Empty, true);
                    }
                    catch { }
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save the application settings: " + ex.Message);
            }
        }
    }
}