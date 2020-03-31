using System.Configuration;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class AppSettings
    {
        //Save - Application Setting
        public static void SettingSave(string name, string value)
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