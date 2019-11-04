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
        //Load - Socket server settings
        void Settings_LoadSocket()
        {
            try
            {
                string SocketServerIp = Convert.ToString(ConfigurationManager.AppSettings["SocketClientIp"]);
                int SocketServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["SocketClientPort"]);

                vSocketServer.vTcpListenerIp = SocketServerIp;
                vSocketServer.vTcpListenerPort = SocketServerPort;
            }
            catch { }
        }

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
                cb_SettingsDesktopBackground.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["DesktopBackground"]);
                cb_SettingsCloseMediaScreen.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["CloseMediaScreen"]);
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
                txt_SettingsSocketClientPortStart.Text = Convert.ToString(ConfigurationManager.AppSettings["SocketClientPort"]);
                txt_SettingsSocketClientPortRange.Text = Convert.ToString(Convert.ToInt32(ConfigurationManager.AppSettings["SocketClientPort"]) + 2);

                //Load the application font size
                textblock_SettingsFontSize.Text = "Adjust the application font size: " + Convert.ToInt32(ConfigurationManager.AppSettings["AppFontSize"]);
                slider_SettingsFontSize.Value = Convert.ToInt32(ConfigurationManager.AppSettings["AppFontSize"]);

                //Load the display monitor
                textblock_SettingsDisplayMonitor.Text = "Default monitor to launch CtrlUI on: " + Convert.ToInt32(ConfigurationManager.AppSettings["DisplayMonitor"]);
                slider_SettingsDisplayMonitor.Value = Convert.ToInt32(ConfigurationManager.AppSettings["DisplayMonitor"]);
                slider_SettingsDisplayMonitor.Maximum = Screen.AllScreens.Count() - 1;

                //Set the application name to string to check shortcuts
                string TargetName_Admin = Assembly.GetEntryAssembly().GetName().Name;

                //Check if application is set to launch on Windows startup
                string TargetFileStartup_Admin = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + TargetName_Admin + ".url";
                if (File.Exists(TargetFileStartup_Admin)) { cb_SettingsWindowsStartup.IsChecked = true; }

                //Check if application is added to GeForce Experience
                string TargetFileGeforce_Admin = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\NVIDIA Corporation\\Shield Apps\\" + TargetName_Admin + ".url";
                if (File.Exists(TargetFileGeforce_Admin)) { btn_Settings_AddGeforceExperience.Content = "Remove CtrlUI from GeForce Experience"; }
            }
            catch (Exception Ex)
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                Answer1.Name = "Alright";
                Answers.Add(Answer1);

                await Popup_Show_MessageBox("Failed to load the application settings", "", Ex.Message, Answers);
            }
        }
    }
}