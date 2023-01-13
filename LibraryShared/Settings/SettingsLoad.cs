using System;
using System.Configuration;
using System.Diagnostics;

namespace LibraryShared
{
    public partial class Settings
    {
        //Load - Application Setting Value
        public static object Setting_Load(Configuration sourceConfig, string settingName)
        {
            try
            {
                return sourceConfig.AppSettings.Settings[settingName].Value;
            }
            catch
            {
                return null;
            }
        }

        //Load - CtrlUI Settings
        public static Configuration Settings_Load_CtrlUI()
        {
            try
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = "CtrlUI.exe.csettings";
                Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                Debug.WriteLine("Loaded the CtrlUI settings.");
                return configuration;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load the CtrlUI settings: " + ex.Message);
                return null;
            }
        }

        //Load - DirectXInput Settings
        public static Configuration Settings_Load_DirectXInput()
        {
            try
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = "DirectXInput.exe.csettings";
                Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                Debug.WriteLine("Loaded the DirectXInput settings.");
                return configuration;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load the DirectXInput settings: " + ex.Message);
                return null;
            }
        }

        //Load - Fps Overlayer Settings
        public static Configuration Settings_Load_FpsOverlayer()
        {
            try
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = "FpsOverlayer.exe.csettings";
                Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                Debug.WriteLine("Loaded the Fps Overlayer settings.");
                return configuration;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load the Fps Overlayer settings: " + ex.Message);
                return null;
            }
        }
    }
}