using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;

namespace LibraryShared
{
    public partial class SoundPlayer
    {
        //Player Variables
        private static MediaPlayer windowsMediaPlayer = new MediaPlayer();

        //Get sound volume from settings
        public static double ConvertSoundVolumeSetting(Configuration appConfiguration)
        {
            double soundVolume = 0.70;
            try
            {
                int soundVolumeInt = Convert.ToInt32(appConfiguration.AppSettings.Settings["InterfaceSoundVolume"].Value);
                soundVolume = (double)soundVolumeInt / 100;
            }
            catch { }
            return soundVolume;
        }

        //Play interface sound
        public static void PlayInterfaceSound(string soundName, bool forceSound)
        {
            try
            {
                if (forceSound || Convert.ToBoolean(ConfigurationManager.AppSettings["InterfaceSound"]))
                {
                    double soundVolume = (double)Convert.ToInt32(ConfigurationManager.AppSettings["InterfaceSoundVolume"]) / 100;
                    string soundPackName = ConfigurationManager.AppSettings["InterfaceSoundPackName"].ToString();
                    string soundFileName = @"Assets\\Sounds\\" + soundPackName + "\\" + soundName + ".mp3";
                    if (File.Exists(soundFileName))
                    {
                        Uri soundFileUri = new Uri(soundFileName, UriKind.RelativeOrAbsolute);
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            windowsMediaPlayer.Volume = soundVolume;
                            windowsMediaPlayer.Open(soundFileUri);
                            windowsMediaPlayer.Play();
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to play sound: " + soundName + " / " + ex.Message);
            }
        }
    }
}