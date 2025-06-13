using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using static ArnoldVinkCode.AVSettings;

namespace LibraryShared
{
    public partial class SoundPlayer
    {
        //Play interface sound
        public static void PlayInterfaceSound(Configuration sourceConfig, string soundName, bool forceSound, bool forceMaxVolume)
        {
            try
            {
                if (forceSound || SettingLoad(sourceConfig, "InterfaceSound", typeof(bool)))
                {
                    double soundVolume = SettingLoad(sourceConfig, "InterfaceSoundVolume", typeof(double)) / 100;
                    if (forceMaxVolume)
                    {
                        soundVolume = 1.00;
                    }
                    else if (forceSound && soundVolume <= 0.20)
                    {
                        soundVolume = 0.75;
                    }

                    string soundPackName = SettingLoad(sourceConfig, "InterfaceSoundPackName", typeof(string));
                    string soundFileName = "Assets/Default/Sounds/" + soundPackName + "/" + soundName + ".mp3";
                    string soundFileNameUser = "Assets/User/Sounds/" + soundPackName + "/" + soundName + ".mp3";
                    if (File.Exists(soundFileNameUser))
                    {
                        soundFileName = soundFileNameUser;
                    }

                    AVSoundPlayer.PlaySound(soundFileName, soundVolume);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to play sound: " + soundName + " / " + ex.Message);
            }
        }
    }
}