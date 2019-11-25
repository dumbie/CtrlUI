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
                int soundVolumeInt = Convert.ToInt32(appConfiguration.AppSettings.Settings["SoundVolume"].Value);
                soundVolume = (double)soundVolumeInt / 100;
            }
            catch { }
            return soundVolume;
        }

        //Play interface sound
        public static void PlayInterfaceSound(double soundVolume, string soundName, bool forceSound)
        {
            try
            {
                if (forceSound || ConfigurationManager.AppSettings["InterfaceSound"] == "True")
                {
                    if (File.Exists(@"Assets\\Sounds\\" + soundName + ".mp3"))
                    {
                        Uri soundFileUri = new Uri(@"Assets\\Sounds\\" + soundName + ".mp3", UriKind.Relative);
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