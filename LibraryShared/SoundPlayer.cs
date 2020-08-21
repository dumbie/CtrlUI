using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using static LibraryShared.Settings;

namespace LibraryShared
{
    public partial class SoundPlayer
    {
        //Player Variables
        private static MediaPlayer windowsMediaPlayer = new MediaPlayer();

        //Play interface sound
        public static void PlayInterfaceSound(Configuration sourceConfig, string soundName, bool forceSound)
        {
            try
            {
                if (forceSound || Convert.ToBoolean(Setting_Load(sourceConfig, "InterfaceSound")))
                {
                    double soundVolume = (double)Convert.ToInt32(Setting_Load(sourceConfig, "InterfaceSoundVolume")) / 100;
                    if (forceSound && soundVolume <= 0.20) { soundVolume = 0.70; }
                    string soundPackName = Setting_Load(sourceConfig, "InterfaceSoundPackName").ToString();
                    string soundFileName = "Assets/Sounds/" + soundPackName + "/" + soundName + ".mp3";
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