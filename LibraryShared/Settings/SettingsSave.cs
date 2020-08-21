using System.Configuration;

namespace LibraryShared
{
    public partial class Settings
    {
        //Save - Application Setting Value
        public static void Setting_Save(Configuration sourceConfig, string settingName, string settingValue)
        {
            try
            {
                sourceConfig.AppSettings.Settings.Remove(settingName);
                sourceConfig.AppSettings.Settings.Add(settingName, settingValue);
                sourceConfig.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch { }
        }
    }
}