using System.Configuration;
using System.IO;
using System.Media;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Play interface sounds
        void PlayInterfaceSound(string SoundName, bool ForceSound)
        {
            try
            {
                if (ForceSound || ConfigurationManager.AppSettings["InterfaceSound"] == "True")
                {
                    if (File.Exists("Assets\\Sounds\\" + SoundName + ".wav"))
                    {
                        using (SoundPlayer soundPlayer = new SoundPlayer())
                        {
                            soundPlayer.SoundLocation = "Assets/Sounds/" + SoundName + ".wav";
                            soundPlayer.Play();
                        }
                    }
                }
            }
            catch { }
        }
    }
}