using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Load - Application Settings
        async Task Settings_Load()
        {
            try
            {
                cb_SettingsLaunchFullscreen.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["LaunchFullscreen"]);
                cb_SettingsLaunchMinimized.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["LaunchMinimized"]);
                cb_SettingsShowOtherShortcuts.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShowOtherShortcuts"]);
                cb_SettingsShowOtherProcesses.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShowOtherProcesses"]);
                cb_SettingsHideAppProcesses.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["HideAppProcesses"]);
                cb_SettingsHideBatteryLevel.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["HideBatteryLevel"]);
                cb_SettingsHideMouseCursor.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["HideMouseCursor"]);
                cb_SettingsHideControllerHelp.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["HideControllerHelp"]);
                cb_SettingsShowHiddenFilesFolders.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShowHiddenFilesFolders"]);
                cb_SettingsHideNetworkDrives.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["HideNetworkDrives"]);
                cb_SettingsInterfaceSound.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["InterfaceSound"]);
                cb_SettingsCloseMediaScreen.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["CloseMediaScreen"]);
                cb_SettingsShowMediaMain.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShowMediaMain"]);
                cb_SettingsMinimizeAppOnShow.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["MinimizeAppOnShow"]);
                cb_SettingsShortcutVolume.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutVolume"]);
                cb_SettingsShortcutAltEnter.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutAltEnter"]);
                cb_SettingsShortcutAltF4.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutAltF4"]);
                cb_SettingsShortcutAltTab.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutAltTab"]);
                cb_SettingsShortcutWinTab.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutWinTab"]);
                cb_SettingsShortcutScreenshot.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutScreenshot"]);
                cb_SettingsLaunchDirectXInput.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["LaunchDirectXInput"]);
                cb_SettingsLaunchFpsOverlayer.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["LaunchFpsOverlayer"]);

                //Load the socket used ports
                txt_SettingsSocketClientPortStart.Text = Convert.ToString(ConfigurationManager.AppSettings["ServerPort"]);
                txt_SettingsSocketClientPortRange.Text = Convert.ToString(Convert.ToInt32(ConfigurationManager.AppSettings["ServerPort"]) + 2);

                //Load the application font size
                textblock_SettingsFontSize.Text = "Adjust the application font size: " + Convert.ToInt32(ConfigurationManager.AppSettings["AppFontSize"]);
                slider_SettingsFontSize.Value = Convert.ToInt32(ConfigurationManager.AppSettings["AppFontSize"]);

                //Load the display monitor
                textblock_SettingsDisplayMonitor.Text = "Default monitor to launch CtrlUI on: " + Convert.ToInt32(ConfigurationManager.AppSettings["DisplayMonitor"]);
                slider_SettingsDisplayMonitor.Value = Convert.ToInt32(ConfigurationManager.AppSettings["DisplayMonitor"]);
                slider_SettingsDisplayMonitor.Maximum = Screen.AllScreens.Count() - 1;

                //Load the sound volume
                textblock_SettingsSoundVolume.Text = "User interface sound volume: " + Convert.ToInt32(ConfigurationManager.AppSettings["InterfaceSoundVolume"]) + "%";
                slider_SettingsSoundVolume.Value = Convert.ToInt32(ConfigurationManager.AppSettings["InterfaceSoundVolume"]);

                //Set the application name to string to check shortcuts
                string targetName = Assembly.GetEntryAssembly().GetName().Name;

                //Check if application is set to launch on Windows startup
                string targetFileStartup = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), targetName + ".url");
                if (File.Exists(targetFileStartup))
                {
                    cb_SettingsWindowsStartup.IsChecked = true;
                }

                //Check if application is added to GeForce Experience
                string targetFileGeforce = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\NVIDIA Corporation\\Shield Apps\\", targetName + ".url");
                if (File.Exists(targetFileGeforce))
                {
                    btn_Settings_AddGeforceExperience_TextBlock.Text = "Remove CtrlUI from GeForce Experience";
                }

                //Background settings
                cb_SettingsVideoBackground.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["VideoBackground"]);
                cb_SettingsDesktopBackground.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["DesktopBackground"]);

                textblock_SettingsBackgroundBrightness.Text = "Background brightness: " + Convert.ToInt32(ConfigurationManager.AppSettings["BackgroundBrightness"]) + "%";
                slider_SettingsBackgroundBrightness.Value = Convert.ToInt32(ConfigurationManager.AppSettings["BackgroundBrightness"]);

                textblock_SettingsBackgroundPlayVolume.Text = "Video playback volume: " + Convert.ToInt32(ConfigurationManager.AppSettings["BackgroundPlayVolume"]) + "%";
                slider_SettingsBackgroundPlayVolume.Value = Convert.ToInt32(ConfigurationManager.AppSettings["BackgroundPlayVolume"]);

                textblock_SettingsBackgroundPlaySpeed.Text = "Video playback speed: " + Convert.ToInt32(ConfigurationManager.AppSettings["BackgroundPlaySpeed"]) + "%";
                slider_SettingsBackgroundPlaySpeed.Value = Convert.ToInt32(ConfigurationManager.AppSettings["BackgroundPlaySpeed"]);
            }
            catch (Exception Ex)
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                Answer1.Name = "Alright";
                Answers.Add(Answer1);

                await Popup_Show_MessageBox("Failed to load the application settings", "", Ex.Message, Answers);
            }
        }
    }
}