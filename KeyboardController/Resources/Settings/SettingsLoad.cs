using System;
using System.Configuration;
using System.Diagnostics;
using static KeyboardController.AppVariables;

namespace KeyboardController
{
    public partial class WindowSettings
    {
        //Load - Socket server settings
        public static void Settings_LoadSocket()
        {
            try
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = "CtrlUI.exe.Config";

                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                string SocketServerIp = Convert.ToString(config.AppSettings.Settings["SocketClientIp"].Value);
                int SocketServerPort = Convert.ToInt32(config.AppSettings.Settings["SocketClientPort"].Value);

                vSocketServer.vTcpListenerIp = SocketServerIp;
                vSocketServer.vTcpListenerPort = SocketServerPort + 2;
            }
            catch { }
        }

        //Load - Application Settings
        void Settings_Load()
        {
            try
            {
                textblock_KeyboardOpacity.Text = textblock_KeyboardOpacity.Tag + ": " + ConfigurationManager.AppSettings["KeyboardOpacity"].ToString() + "%";
                slider_KeyboardOpacity.Value = Convert.ToDouble(ConfigurationManager.AppSettings["KeyboardOpacity"]);

                combobox_KeyboardLayout.SelectedIndex = Convert.ToInt32(ConfigurationManager.AppSettings["KeyboardLayout"]);
                cb_SettingsInterfaceSound.IsChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["InterfaceSound"]);
            }
            catch (Exception Ex) { Debug.WriteLine("Failed to load the application settings: " + Ex.Message); }
        }
    }
}