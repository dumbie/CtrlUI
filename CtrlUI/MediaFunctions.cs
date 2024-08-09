using ArnoldVinkCode;
using System.Windows;
using static ArnoldVinkCode.AVAudioDevice;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Update the current volume information
        void UpdateCurrentVolumeInformation()
        {
            try
            {
                //Check if application is activated
                if (!vAppActivated)
                {
                    return;
                }

                //Check if volume is currently muted
                bool currentOutputVolumeMuted = AudioMuteGetStatus(false);
                bool currentInputVolumeMuted = AudioMuteGetStatus(true);
                AVActions.DispatcherInvoke(delegate
                {
                    img_Main_VolumeMute.Visibility = currentOutputVolumeMuted ? Visibility.Visible : Visibility.Collapsed;
                    img_Main_MicrophoneMute.Visibility = currentInputVolumeMuted ? Visibility.Visible : Visibility.Collapsed;
                });
            }
            catch { }
        }
    }
}