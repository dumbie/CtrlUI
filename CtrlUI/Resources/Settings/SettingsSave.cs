using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Media;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Save - Monitor Application Settings
        void Settings_Save()
        {
            try
            {
                cb_SettingsLaunchFullscreen.Click += (sender, e) =>
                {
                    SettingSave("LaunchFullscreen", cb_SettingsLaunchFullscreen.IsChecked.ToString());
                    if ((bool)cb_SettingsLaunchFullscreen.IsChecked)
                    {
                        cb_SettingsLaunchMinimized.IsChecked = false;
                        SettingSave("LaunchMinimized", cb_SettingsLaunchMinimized.IsChecked.ToString());
                    }
                };

                cb_SettingsLaunchMinimized.Click += (sender, e) =>
                {
                    SettingSave("LaunchMinimized", cb_SettingsLaunchMinimized.IsChecked.ToString());
                    if ((bool)cb_SettingsLaunchMinimized.IsChecked)
                    {
                        cb_SettingsLaunchFullscreen.IsChecked = false;
                        SettingSave("LaunchFullscreen", cb_SettingsLaunchFullscreen.IsChecked.ToString());
                    }
                };

                cb_SettingsCloseMediaScreen.Click += (sender, e) => { SettingSave("CloseMediaScreen", cb_SettingsCloseMediaScreen.IsChecked.ToString()); };
                cb_SettingsShowMediaMain.Click += (sender, e) => { SettingSave("ShowMediaMain", cb_SettingsShowMediaMain.IsChecked.ToString()); };
                cb_SettingsMinimizeAppOnShow.Click += (sender, e) => { SettingSave("MinimizeAppOnShow", cb_SettingsMinimizeAppOnShow.IsChecked.ToString()); };

                cb_SettingsLaunchFpsOverlayer.Click += (sender, e) => { SettingSave("LaunchFpsOverlayer", cb_SettingsLaunchFpsOverlayer.IsChecked.ToString()); };

                cb_SettingsShortcutVolume.Click += (sender, e) =>
                {
                    SettingSave("ShortcutVolume", cb_SettingsShortcutVolume.IsChecked.ToString());
                    UpdateControllerHelp();
                };

                cb_SettingsShowOtherShortcuts.Click += (sender, e) =>
                {
                    SettingSave("ShowOtherShortcuts", cb_SettingsShowOtherShortcuts.IsChecked.ToString());
                };

                cb_SettingsShowOtherProcesses.Click += (sender, e) =>
                {
                    SettingSave("ShowOtherProcesses", cb_SettingsShowOtherProcesses.IsChecked.ToString());
                };

                cb_SettingsHideAppProcesses.Click += (sender, e) =>
                {
                    SettingSave("HideAppProcesses", cb_SettingsHideAppProcesses.IsChecked.ToString());
                };

                cb_SettingsHideBatteryLevel.Click += (sender, e) =>
                {
                    SettingSave("HideBatteryLevel", cb_SettingsHideBatteryLevel.IsChecked.ToString());
                    if ((bool)cb_SettingsHideBatteryLevel.IsChecked)
                    {
                        HideBatteryStatus(true);
                    }
                };

                cb_SettingsHideMouseCursor.Click += async (sender, e) =>
                {
                    SettingSave("HideMouseCursor", cb_SettingsHideMouseCursor.IsChecked.ToString());
                    if ((bool)cb_SettingsHideMouseCursor.IsChecked)
                    {
                        TaskStart_ShowHideMouseCursor();
                        MouseCursorShow();
                    }
                    else
                    {
                        await AVActions.TaskStop(vTask_ShowHideMouse, vTaskToken_ShowHideMouse);
                        MouseCursorShow();
                    }
                };

                cb_SettingsHideControllerHelp.Click += (sender, e) =>
                {
                    SettingSave("HideControllerHelp", cb_SettingsHideControllerHelp.IsChecked.ToString());
                    UpdateControllerHelp();
                };

                cb_SettingsShowHiddenFilesFolders.Click += (sender, e) => { SettingSave("ShowHiddenFilesFolders", cb_SettingsShowHiddenFilesFolders.IsChecked.ToString()); };
                cb_SettingsHideNetworkDrives.Click += (sender, e) => { SettingSave("HideNetworkDrives", cb_SettingsHideNetworkDrives.IsChecked.ToString()); };
                cb_SettingsInterfaceSound.Click += (sender, e) => { SettingSave("InterfaceSound", cb_SettingsInterfaceSound.IsChecked.ToString()); };
                cb_SettingsWindowsStartup.Click += (sender, e) => { ManageShortcutStartup(); };

                slider_SettingsFontSize.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsFontSize.Text = "Adjust the application font size: " + Convert.ToInt32(slider_SettingsFontSize.Value);
                    SettingSave("AppFontSize", Convert.ToInt32(slider_SettingsFontSize.Value).ToString());
                    AdjustApplicationFontSize();
                };

                slider_SettingsDisplayMonitor.ValueChanged += async (sender, e) =>
                {
                    textblock_SettingsDisplayMonitor.Text = "Monitor to display the applications on: " + Convert.ToInt32(slider_SettingsDisplayMonitor.Value);
                    SettingSave("DisplayMonitor", Convert.ToInt32(slider_SettingsDisplayMonitor.Value).ToString());

                    int monitorNumber = Convert.ToInt32(ConfigurationManager.AppSettings["DisplayMonitor"]);
                    await UpdateWindowPosition(monitorNumber, true, false);
                };

                slider_SettingsSoundVolume.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsSoundVolume.Text = "User interface sound volume: " + Convert.ToInt32(slider_SettingsSoundVolume.Value) + "%";
                    SettingSave("InterfaceSoundVolume", Convert.ToInt32(slider_SettingsSoundVolume.Value).ToString());
                };

                //Background Settings
                cb_SettingsVideoBackground.Click += (sender, e) =>
                {
                    SettingSave("VideoBackground", cb_SettingsVideoBackground.IsChecked.ToString());
                    UpdateBackgroundMedia();
                };

                cb_SettingsDesktopBackground.Click += (sender, e) =>
                {
                    SettingSave("DesktopBackground", cb_SettingsDesktopBackground.IsChecked.ToString());
                    UpdateBackgroundMedia();
                };

                slider_SettingsBackgroundBrightness.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsBackgroundBrightness.Text = "Background brightness: " + Convert.ToInt32(slider_SettingsBackgroundBrightness.Value) + "%";
                    SettingSave("BackgroundBrightness", Convert.ToInt32(slider_SettingsBackgroundBrightness.Value).ToString());
                    UpdateBackgroundBrightness();
                };

                slider_SettingsBackgroundPlayVolume.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsBackgroundPlayVolume.Text = "Video playback volume: " + Convert.ToInt32(slider_SettingsBackgroundPlayVolume.Value) + "%";
                    SettingSave("BackgroundPlayVolume", Convert.ToInt32(slider_SettingsBackgroundPlayVolume.Value).ToString());
                    UpdateBackgroundPlayVolume();
                };

                slider_SettingsBackgroundPlaySpeed.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsBackgroundPlaySpeed.Text = "Video playback speed: " + Convert.ToInt32(slider_SettingsBackgroundPlaySpeed.Value) + "%";
                    SettingSave("BackgroundPlaySpeed", Convert.ToInt32(slider_SettingsBackgroundPlaySpeed.Value).ToString());
                    UpdateBackgroundPlaySpeed();
                };

                //Save - Socket Client Port
                txt_SettingsSocketClientPortStart.TextChanged += (sender, e) =>
                {
                    //Color brushes
                    BrushConverter BrushConvert = new BrushConverter();
                    Brush BrushInvalid = BrushConvert.ConvertFromString("#CD1A2B") as Brush;
                    Brush BrushValid = BrushConvert.ConvertFromString("#1DB954") as Brush;

                    if (string.IsNullOrWhiteSpace(txt_SettingsSocketClientPortStart.Text))
                    {
                        txt_SettingsSocketClientPortStart.BorderBrush = BrushInvalid;
                        txt_SettingsSocketClientPortRange.BorderBrush = BrushInvalid;
                        return;
                    }

                    if (Regex.IsMatch(txt_SettingsSocketClientPortStart.Text, "(\\D+)"))
                    {
                        txt_SettingsSocketClientPortStart.BorderBrush = BrushInvalid;
                        txt_SettingsSocketClientPortRange.BorderBrush = BrushInvalid;
                        return;
                    }

                    int NewServerPort = Convert.ToInt32(txt_SettingsSocketClientPortStart.Text);
                    if (NewServerPort < 100 || NewServerPort > 65500)
                    {
                        txt_SettingsSocketClientPortStart.BorderBrush = BrushInvalid;
                        txt_SettingsSocketClientPortRange.BorderBrush = BrushInvalid;
                        return;
                    }

                    txt_SettingsSocketClientPortStart.BorderBrush = BrushValid;
                    txt_SettingsSocketClientPortRange.BorderBrush = BrushValid;
                    txt_SettingsSocketClientPortRange.Text = Convert.ToString(NewServerPort + 2);
                    SettingSave("ServerPort", txt_SettingsSocketClientPortStart.Text);
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save the application settings: " + ex.Message);
            }
        }

        //Save - Application Setting
        void SettingSave(string name, string value)
        {
            try
            {
                vConfiguration.AppSettings.Settings.Remove(name);
                vConfiguration.AppSettings.Settings.Add(name, value);
                vConfiguration.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch { }
        }
    }
}