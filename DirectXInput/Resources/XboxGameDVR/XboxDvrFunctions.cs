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
                if (!CaptureIsAvailable())
                {
                    //Play interface sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "RecycleBinEmpty", false, true);
                    return;
                }

                //Play interface sound
                PlayInterfaceSound(vConfigurationCtrlUI, "Move", false, true);

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
                if (!CaptureIsAvailable())
                {
                    //Play interface sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "RecycleBinEmpty", false, true);
                    return;
                }

                //Play interface sound
                if (CaptureIsRecording())
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false, true);
                }
                else
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false, true);
                }

                //Capture keyboard shortcut
                vFakerInputDevice.KeyboardPressRelease(GetKeysHidAction_ToggleRecording());
            }
            catch { }
        }
    }
}