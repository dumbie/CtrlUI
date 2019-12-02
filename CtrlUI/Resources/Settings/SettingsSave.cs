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
                    if ((bool)cb_SettingsLaunchFullscreen.IsChecked) { cb_SettingsLaunchMinimized.IsChecked = false; SettingSave("LaunchMinimized", cb_SettingsLaunchMinimized.IsChecked.ToString()); }
                };

                cb_SettingsLaunchMinimized.Click += (sender, e) =>
                {
                    SettingSave("LaunchMinimized", cb_SettingsLaunchMinimized.IsChecked.ToString());
                    if ((bool)cb_SettingsLaunchMinimized.IsChecked) { cb_SettingsLaunchFullscreen.IsChecked = false; SettingSave("LaunchFullscreen", cb_SettingsLaunchFullscreen.IsChecked.ToString()); }
                };

                cb_SettingsCloseMediaScreen.Click += (sender, e) => { SettingSave("CloseMediaScreen", cb_SettingsCloseMediaScreen.IsChecked.ToString()); };
                cb_SettingsMinimizeAppOnShow.Click += (sender, e) => { SettingSave("MinimizeAppOnShow", cb_SettingsMinimizeAppOnShow.IsChecked.ToString()); };

                cb_SettingsLaunchDirectXInput.Click += (sender, e) => { SettingSave("LaunchDirectXInput", cb_SettingsLaunchDirectXInput.IsChecked.ToString()); };
                cb_SettingsLaunchFpsOverlayer.Click += (sender, e) => { SettingSave("LaunchFpsOverlayer", cb_SettingsLaunchFpsOverlayer.IsChecked.ToString()); };

                cb_SettingsShortcutVolume.Click += (sender, e) =>
                {
                    SettingSave("ShortcutVolume", cb_SettingsShortcutVolume.IsChecked.ToString());
                    UpdateControllerHelp();
                };

                cb_SettingsShortcutAltEnter.Click += (sender, e) =>
                {
                    SettingSave("ShortcutAltEnter", cb_SettingsShortcutAltEnter.IsChecked.ToString());
                    UpdateControllerHelp();
                };

                cb_SettingsShortcutAltF4.Click += (sender, e) =>
                {
                    SettingSave("ShortcutAltF4", cb_SettingsShortcutAltF4.IsChecked.ToString());
                    UpdateControllerHelp();
                };

                cb_SettingsShortcutAltTab.Click += (sender, e) =>
                {
                    SettingSave("ShortcutAltTab", cb_SettingsShortcutAltTab.IsChecked.ToString());

                    if (cb_SettingsShortcutAltTab.IsChecked == true)
                    {
                        SettingSave("ShortcutWinTab", "False");
                        cb_SettingsShortcutWinTab.IsChecked = false;
                    }

                    UpdateControllerHelp();
                };

                cb_SettingsShortcutWinTab.Click += (sender, e) =>
                {
                    SettingSave("ShortcutWinTab", cb_SettingsShortcutWinTab.IsChecked.ToString());

                    if (cb_SettingsShortcutWinTab.IsChecked == true)
                    {
                        SettingSave("ShortcutAltTab", "False");
                        cb_SettingsShortcutAltTab.IsChecked = false;
                    }

                    UpdateControllerHelp();
                };

                cb_SettingsShortcutScreenshot.Click += (sender, e) =>
                {
                    SettingSave("ShortcutScreenshot", cb_SettingsShortcutScreenshot.IsChecked.ToString());
                    UpdateControllerHelp();
                };

                cb_SettingsShowOtherShortcuts.Click += async (sender, e) =>
                {
                    SettingSave("ShowOtherShortcuts", cb_SettingsShowOtherShortcuts.IsChecked.ToString());
                    await RefreshApplicationLists(false, false, false, false, false, false, false);
                };

                cb_SettingsShowOtherProcesses.Click += async (sender, e) =>
                {
                    SettingSave("ShowOtherProcesses", cb_SettingsShowOtherProcesses.IsChecked.ToString());
                    await RefreshApplicationLists(false, false, false, false, false, false, false);
                };

                cb_SettingsHideAppProcesses.Click += async (sender, e) =>
                {
                    SettingSave("HideAppProcesses", cb_SettingsHideAppProcesses.IsChecked.ToString());
                    await RefreshApplicationLists(false, false, false, false, false, false, false);
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

                cb_SettingsDesktopBackground.Click += (sender, e) =>
                {
                    SettingSave("DesktopBackground", cb_SettingsDesktopBackground.IsChecked.ToString());
                    UpdateBackgroundImage();
                };

                cb_SettingsWindowsStartup.Click += (sender, e) => { ManageShortcutStartup(); };

                slider_SettingsFontSize.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsFontSize.Text = "Adjust the application font size: " + Convert.ToInt32(slider_SettingsFontSize.Value);
                    SettingSave("AppFontSize", Convert.ToInt32(slider_SettingsFontSize.Value).ToString());
                    AdjustApplicationFontSize();
                };

                slider_SettingsDisplayMonitor.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsDisplayMonitor.Text = "Default monitor to launch CtrlUI on: " + Convert.ToInt32(slider_SettingsDisplayMonitor.Value);
                    SettingSave("DisplayMonitor", Convert.ToInt32(slider_SettingsDisplayMonitor.Value).ToString());
                };

                slider_SettingsSoundVolume.ValueChanged += (sender, e) =>
                {
                    textblock_SettingsSoundVolume.Text = "User interface sound volume: " + Convert.ToInt32(slider_SettingsSoundVolume.Value) + "%";
                    SettingSave("SoundVolume", Convert.ToInt32(slider_SettingsSoundVolume.Value).ToString());
                    vInterfaceSoundVolume = (double)Convert.ToInt32(ConfigurationManager.AppSettings["SoundVolume"]) / 100;
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
            catch (Exception Ex)
            {
                Debug.WriteLine("Failed to save the application settings: " + Ex.Message);
            }
        }

        //Save - Application Setting
        void SettingSave(string Name, string Value)
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