using System;
using System.Configuration;
using System.Diagnostics;

namespace KeyboardController
{
    public partial class WindowSettings
    {
        //Check - Application Settings
        public static void Settings_Check()
        {
            try
            {
                if (ConfigurationManager.AppSettings["InterfaceSound"] == null) { SettingSave("InterfaceSound", "True"); }
                if (ConfigurationManager.AppSettings["SoundVolume"] == null) { SettingSave("SoundVolume", "90"); }

                if (ConfigurationManager.AppSettings["KeyboardLayout"] == null) { SettingSave("KeyboardLayout", "0"); }
                if (ConfigurationManager.AppSettings["KeyboardMode"] == null) { SettingSave("KeyboardMode", "0"); }
                if (ConfigurationManager.AppSettings["KeyboardOpacity"] == null) { SettingSave("KeyboardOpacity", "0,95"); }

                if (ConfigurationManager.AppSettings["DomainExtension"] == null) { SettingSave("DomainExtension", ".nl"); }

                Debug.WriteLine("Checked the application settings.");
            }
            catch (Exception Ex)
            {
                Debug.WriteLine("Failed to check the application settings: " + Ex.Message);
            }
        }
    }
}