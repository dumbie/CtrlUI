using static DirectXInput.AppVariables;
using static LibraryShared.SoundPlayer;

namespace DirectXInput
{
    partial class XboxGameDVR
    {
        //Capture screenshot
        public static void CaptureImage()
        {
            try
            {
                //Check if capture is available
                if (!CaptureIsAvailable(false))
                {
                    //Play interface sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "CaptureFailed", false, true);
                    return;
                }

                //Play interface sound
                PlayInterfaceSound(vConfigurationCtrlUI, "CaptureScreenshot", false, true);

                //Capture keyboard shortcut
                vFakerInputDevice.KeyboardPressRelease(GetKeysHidAction_TakeScreenshot());
            }
            catch { }
        }

        //Capture video
        public static void CaptureVideo()
        {
            try
            {
                //Check if capture is available
                if (!CaptureIsAvailable(true))
                {
                    //Play interface sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "CaptureFailed", false, true);
                    return;
                }

                //Play interface sound
                if (CaptureIsRecording())
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "CaptureVideoStop", false, true);
                }
                else
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "CaptureVideoStart", false, true);
                }

                //Capture keyboard shortcut
                vFakerInputDevice.KeyboardPressRelease(GetKeysHidAction_ToggleRecording());
            }
            catch { }
        }
    }
}