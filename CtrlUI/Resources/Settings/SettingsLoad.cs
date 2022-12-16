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
                cb_SettingsLaunchMinimized.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "LaunchMinimized"));

                cb_SettingsShowLibrarySteam.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibrarySteam"));
                cb_SettingsShowLibraryEADesktop.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryEADesktop"));
                cb_SettingsShowLibraryEpic.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryEpic"));
                cb_SettingsShowLibraryUbisoft.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryUbisoft"));
                cb_SettingsShowLibraryGoG.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryGoG"));
                cb_SettingsShowLibraryBattleNet.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryBattleNet"));
                cb_SettingsShowLibraryRockstar.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryRockstar"));
                cb_SettingsShowLibraryAmazon.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryAmazon"));
                cb_SettingsShowLibraryUwp.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryUwp"));

                cb_SettingsHideBatteryLevel.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "HideBatteryLevel"));
                cb_SettingsHideControllerHelp.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "HideControllerHelp"));

                cb_SettingsShowHiddenFilesFolders.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowHiddenFilesFolders"));
                cb_SettingsHideNetworkDrives.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "HideNetworkDrives"));
                cb_SettingsNotReadyNetworkDrives.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "NotReadyNetworkDrives"));

                cb_SettingsInterfaceSound.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "InterfaceSound"));
                cb_SettingsLaunchFpsOverlayer.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "LaunchFpsOverlayer"));
                cb_SettingsLaunchDirectXInput.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "LaunchDirectXInput"));

                //Load the socket used ports
                txt_SettingsSocketClientPortStart.Text = Convert.ToString(Setting_Load(vConfigurationCtrlUI, "ServerPort"));
                txt_SettingsSocketClientPortRange.Text = Convert.ToString(Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "ServerPort")) + 2);

                //Load the application font size
                textblock_SettingsFontSize.Text = "Adjust the application font size: " + Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "AppFontSize"));
                slider_SettingsFontSize.Value = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "AppFontSize"));

                textblock_SettingsAppWindowSize.Text = textblock_SettingsAppWindowSize.Tag + ": " + Setting_Load(vConfigurationCtrlUI, "AppWindowSize").ToString() + "%";
                slider_SettingsAppWindowSize.Value = Convert.ToDouble(Setting_Load(vConfigurationCtrlUI, "AppWindowSize"));

                //Load the display monitor
                int monitorNumber = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "DisplayMonitor"));
                textblock_SettingsDisplayMonitor.Text = "Monitor to display the applications on: " + monitorNumber;
                slider_SettingsDisplayMonitor.Value = monitorNumber;
                slider_SettingsDisplayMonitor.Maximum = Screen.AllScreens.Count();

                //Load display settings
                cb_SettingsMonitorPreventSleep.IsChecked = Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "MonitorPreventSleep"));

                textblock_SettingsAdjustChromiumDpi.Text = textblock_SettingsAdjustChromiumDpi.Tag + ": +" + Setting_Load(vConfigurationCtrlUI, "AdjustChromiumDpi").ToString() + "%";
                slider_SettingsAdjustChromiumDpi.Value = Convert.ToDouble(Setting_Load(vConfigurationCtrlUI, "AdjustChromiumDpi"));

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

                //Check if ctrlui is added to GeForce Experience
                string targetFileGeforceCtrlUI = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/NVIDIA Corporation/Shield Apps/", targetName + ".url");
                if (File.Exists(targetFileGeforceCtrlUI))
                {
                    btn_Settings_AddGeforceExperience_TextBlock.Text = "Remove CtrlUI from GeForce Experience";
                }

                //Check if remote desktop is added to GeForce Experience
                string targetFileGeforceRemote = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/NVIDIA Corporation/Shield Apps/", "Remote Desktop.url");
                if (File.Exists(targetFileGeforceRemote))
                {
                    btn_Settings_AddRemoteDesktop_TextBlock.Text = "Remove Remote Desktop from GeForce Experience";
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