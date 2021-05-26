using System;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

namespace DirectXInput.MediaCode
{
    partial class WindowMedia
    {
        //Process controller input
        public async Task ControllerInteraction(ControllerInput ControllerInput)
        {
            bool ControllerDelayShort = false;
            bool ControllerDelayMedium = false;
            bool ControllerDelayLonger = false;
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_Media)
                {
                    //Left stick movement
                    if (ControllerInput.ThumbLeftX < -10000 && Math.Abs(ControllerInput.ThumbLeftY) < 13000)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        await KeySendSingle(KeysVirtual.Left, vInteropWindowHandle);

                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ThumbLeftY > 10000 && Math.Abs(ControllerInput.ThumbLeftX) < 13000)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        await KeySendSingle(KeysVirtual.Up, vInteropWindowHandle);

                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ThumbLeftX > 10000 && Math.Abs(ControllerInput.ThumbLeftY) < 13000)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        await KeySendSingle(KeysVirtual.Right, vInteropWindowHandle);

                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ThumbLeftY < -10000 && Math.Abs(ControllerInput.ThumbLeftX) < 13000)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        await KeySendSingle(KeysVirtual.Down, vInteropWindowHandle);

                        ControllerDelayShort = true;
                    }

                    //Send internal arrow left key
                    else if (ControllerInput.DPadLeft.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        await KeySendSingle(KeysVirtual.Left, vInteropWindowHandle);

                        ControllerDelayShort = true;
                    }
                    //Send internal arrow right key
                    else if (ControllerInput.DPadRight.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        await KeySendSingle(KeysVirtual.Right, vInteropWindowHandle);

                        ControllerDelayShort = true;
                    }
                    //Send internal arrow up key
                    else if (ControllerInput.DPadUp.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        await KeySendSingle(KeysVirtual.Up, vInteropWindowHandle);

                        ControllerDelayShort = true;
                    }
                    //Send internal arrow down key
                    else if (ControllerInput.DPadDown.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        await KeySendSingle(KeysVirtual.Down, vInteropWindowHandle);

                        ControllerDelayShort = true;
                    }

                    //Send internal space key
                    else if (ControllerInput.ButtonA.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await KeySendSingle(KeysVirtual.Space, vInteropWindowHandle);

                        ControllerDelayMedium = true;
                    }
                    //Send external media next key
                    else if (ControllerInput.ButtonB.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        App.vWindowOverlay.Notification_Show_Status("MediaNext", "Going to next media item");
                        await KeyPressSingleAuto(KeysVirtual.MediaNextTrack);

                        ControllerDelayMedium = true;
                    }
                    //Send external media playpause key
                    else if (ControllerInput.ButtonY.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        App.vWindowOverlay.Notification_Show_Status("MediaPlayPause", "Resuming or pausing media");
                        await KeyPressSingleAuto(KeysVirtual.MediaPlayPause);

                        ControllerDelayMedium = true;
                    }
                    //Send external media previous key
                    else if (ControllerInput.ButtonX.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        App.vWindowOverlay.Notification_Show_Status("MediaPrevious", "Going to previous media item");
                        await KeyPressSingleAuto(KeysVirtual.MediaPreviousTrack);

                        ControllerDelayMedium = true;
                    }

                    //Send external arrow keys
                    else if (ControllerInput.ButtonShoulderLeft.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        App.vWindowOverlay.Notification_Show_Status("ArrowLeft", "Moving left");
                        await KeyPressSingleAuto(KeysVirtual.Left);

                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ButtonShoulderRight.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        App.vWindowOverlay.Notification_Show_Status("ArrowRight", "Moving right");
                        await KeyPressSingleAuto(KeysVirtual.Right);

                        ControllerDelayShort = true;
                    }

                    //Send external alt+enter keys
                    else if (ControllerInput.ButtonBack.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        App.vWindowOverlay.Notification_Show_Status("MediaFullscreen", "Toggling fullscreen");
                        await KeyPressComboAuto(KeysVirtual.Alt, KeysVirtual.Enter);

                        ControllerDelayMedium = true;
                    }

                    //Change the system volume
                    else if (ControllerInput.TriggerLeft > 0 && ControllerInput.TriggerRight > 0)
                    {
                        App.vWindowOverlay.Notification_Show_Status("VolumeMute", "Toggling output mute");
                        await KeyPressSingleAuto(KeysVirtual.VolumeMute);

                        ControllerDelayLonger = true;
                    }
                    else if (ControllerInput.ButtonStart.PressedRaw)
                    {
                        App.vWindowOverlay.Notification_Show_Status("VolumeMute", "Toggling output mute");
                        await KeyPressSingleAuto(KeysVirtual.VolumeMute);

                        ControllerDelayMedium = true;
                    }
                    else if (ControllerInput.TriggerLeft > 0)
                    {
                        App.vWindowOverlay.Notification_Show_Status("VolumeDown", "Decreasing volume");
                        await KeyPressSingleAuto(KeysVirtual.VolumeDown);

                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.TriggerRight > 0)
                    {
                        App.vWindowOverlay.Notification_Show_Status("VolumeUp", "Increasing volume");
                        await KeyPressSingleAuto(KeysVirtual.VolumeUp);

                        ControllerDelayShort = true;
                    }

                    //Delay input to prevent repeat
                    if (ControllerDelayShort)
                    {
                        vControllerDelay_Media = GetSystemTicksMs() + vControllerDelayShortTicks;
                    }
                    else if (ControllerDelayMedium)
                    {
                        vControllerDelay_Media = GetSystemTicksMs() + vControllerDelayMediumTicks;
                    }
                    else if (ControllerDelayLonger)
                    {
                        vControllerDelay_Media = GetSystemTicksMs() + vControllerDelayLongerTicks;
                    }

                    //Update the window style (focus workaround)
                    if (ControllerDelayShort || ControllerDelayMedium || ControllerDelayLonger)
                    {
                        UpdateWindowStyle();
                    }
                }
            }
            catch { }
        }
    }
}