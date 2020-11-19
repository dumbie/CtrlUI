using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using static CtrlUI.AppVariables;
using static LibraryShared.Settings;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Load - Application Settings
        bool Settings_Load()
        {
            try
            {
                cb_SettingsLaunchFullscreen.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "LaunchFullscreen"));
                cb_SettingsLaunchMinimized.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "LaunchMinimized"));

                cb_SettingsShowOtherShortcuts.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowOtherShortcuts"));
                cb_SettingsShowOtherProcesses.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowOtherProcesses"));
                cb_SettingsHideAppProcesses.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "HideAppProcesses"));
                cb_SettingsShowLibrarySteam.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibrarySteam"));
                cb_SettingsShowLibraryEADesktop.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryEADesktop"));
                cb_SettingsShowLibraryEpic.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryEpic"));
                cb_SettingsShowLibraryUbisoft.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryUbisoft"));
                cb_SettingsShowLibraryGoG.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryGoG"));
                cb_SettingsShowLibraryBattleNet.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryBattleNet"));
                cb_SettingsShowLibraryBethesda.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryBethesda"));
                cb_SettingsShowLibraryRockstar.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryRockstar"));
                cb_SettingsShowLibraryUwp.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryUwp"));

                cb_SettingsHideBatteryLevel.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "HideBatteryLevel"));
                cb_SettingsHideMouseCursor.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "HideMouseCursor"));
                cb_SettingsHideControllerHelp.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "HideControllerHelp"));
                cb_SettingsShowHiddenFilesFolders.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowHiddenFilesFolders"));
                cb_SettingsHideNetworkDrives.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "HideNetworkDrives"));
                cb_SettingsInterfaceSound.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "InterfaceSound"));
                cb_SettingsShowMediaMain.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowMediaMain"));
                cb_SettingsMinimizeAppOnShow.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "MinimizeAppOnShow"));
                cb_SettingsLaunchFpsOverlayer.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "LaunchFpsOverlayer"));

                //Load the socket used ports
                txt_SettingsSocketClientPortStart.Text = Convert.ToString(Setting_Load(vConfigurationCtrlUI, "ServerPort"));
                txt_SettingsSocketClientPortRange.Text = Convert.ToString(Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "ServerPort")) + 3);

                //Load the application font size
                textblock_SettingsFontSize.Text = "Adjust the application font size: " + Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "AppFontSize"));
                slider_SettingsFontSize.Value = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "AppFontSize"));

                //Load the display monitor
                int monitorNumber = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "DisplayMonitor"));
                textblock_SettingsDisplayMonitor.Text = "Monitor to display the applications on: " + monitorNumber;
                slider_SettingsDisplayMonitor.Value = monitorNumber;
                slider_SettingsDisplayMonitor.Maximum = Screen.AllScreens.Count();

                //Load the sound volume
                textblock_SettingsSoundVolume.Text = "User interface sound volume: " + Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "InterfaceSoundVolume")) + "%";
                slider_SettingsSoundVolume.Value = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "InterfaceSoundVolume"));

                //Set the application name to string to check shortcuts
                string targetName = Assembly.GetEntryAssembly().GetName().Name;

                //Check if application is set to launch on Windows startup
                string targetFileStartup = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), targetName + ".url");
                if (File.Exists(targetFileStartup))
                {
                    cb_SettingsWindowsStartup.IsChecked = true;
                }

                //Check if application is added to GeForce Experience
                string targetFileGeforce = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/NVIDIA Corporation/Shield Apps/", targetName + ".url");
                if (File.Exists(targetFileGeforce))
                {
                    btn_Settings_AddGeforceExperience_TextBlock.Text = "Remove CtrlUI from GeForce Experience";
                }

                //Background settings
                cb_SettingsVideoBackground.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "VideoBackground"));
                cb_SettingsDesktopBackground.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "DesktopBackground"));

                textblock_SettingsBackgroundBrightness.Text = "Background brightness: " + Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "BackgroundBrightness")) + "%";
                slider_SettingsBackgroundBrightness.Value = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "BackgroundBrightness"));

                textblock_SettingsBackgroundPlayVolume.Text = "Video playback volume: " + Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "BackgroundPlayVolume")) + "%";
                slider_SettingsBackgroundPlayVolume.Value = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "BackgroundPlayVolume"));

                textblock_SettingsBackgroundPlaySpeed.Text = "Video playback speed: " + Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "BackgroundPlaySpeed")) + "%";
                slider_SettingsBackgroundPlaySpeed.Value = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "BackgroundPlaySpeed"));

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