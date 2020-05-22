using System.Configuration;

namespace LibraryShared
{
    public partial class Settings
    {
        //Save - Application Setting
        public static void SettingSave(Configuration settingConfiguration, string settingName, string settingValue)
        {
            try
            {
                settingConfiguration.AppSettings.Settings.Remove(settingName);
                settingConfiguration.AppSettings.Settings.Add(settingName, settingValue);
                settingConfiguration.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch { }
        }
    }
}