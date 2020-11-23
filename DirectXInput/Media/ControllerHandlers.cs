using System;
using System.Threading.Tasks;
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
                if (Environment.TickCount >= vControllerDelay_Media)
                {
                    //Left stick movement
                    if (ControllerInput.ThumbLeftX < -10000 && Math.Abs(ControllerInput.ThumbLeftY) < 13000)
                    {
                        await KeySendSingle(KeysVirtual.Left, vInteropWindowHandle);

                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ThumbLeftY > 10000 && Math.Abs(ControllerInput.ThumbLeftX) < 13000)
                    {
                        await KeySendSingle(KeysVirtual.Up, vInteropWindowHandle);

                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ThumbLeftX > 10000 && Math.Abs(ControllerInput.ThumbLeftY) < 13000)
                    {
                        await KeySendSingle(KeysVirtual.Right, vInteropWindowHandle);

                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ThumbLeftY < -10000 && Math.Abs(ControllerInput.ThumbLeftX) < 13000)
                    {
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

                        ControllerDelayShort = true;
                    }
                    //Send external media next key
                    else if (ControllerInput.ButtonB.PressedRaw || ControllerInput.ButtonShoulderRight.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        App.vWindowOverlay.Notification_Show_Status("MediaNext", "Going to next media item");
                        await KeyPressSingleAuto(KeysVirtual.MediaNextTrack);

                        ControllerDelayShort = true;
                    }
                    //Send external media playpause key
                    else if (ControllerInput.ButtonY.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        App.vWindowOverlay.Notification_Show_Status("MediaPlayPause", "Resuming or pausing media");
                        await KeyPressSingleAuto(KeysVirtual.MediaPlayPause);

                        ControllerDelayShort = true;
                    }
                    //Send external media previous key
                    else if (ControllerInput.ButtonX.PressedRaw || ControllerInput.ButtonShoulderLeft.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        App.vWindowOverlay.Notification_Show_Status("MediaPrevious", "Going to previous media item");
                        await KeyPressSingleAuto(KeysVirtual.MediaPreviousTrack);

                        ControllerDelayShort = true;
                    }

                    //Change the system volume
                    else if (ControllerInput.TriggerLeft > 0 && ControllerInput.TriggerRight > 0)
                    {
                        App.vWindowOverlay.Notification_Show_Status("VolumeMute", "Toggling mute");
                        await KeyPressSingleAuto(KeysVirtual.VolumeMute);

                        ControllerDelayLonger = true;
                    }
                    else if (ControllerInput.ButtonStart.PressedRaw)
                    {
                        App.vWindowOverlay.Notification_Show_Status("VolumeMute", "Toggling mute");
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

                    if (ControllerDelayShort)
                    {
                        vControllerDelay_Media = Environment.TickCount + vControllerDelayShortTicks;
                    }
                    else if (ControllerDelayMedium)
                    {
                        vControllerDelay_Media = Environment.TickCount + vControllerDelayMediumTicks;
                    }
                    else if (ControllerDelayLonger)
                    {
                        vControllerDelay_Media = Environment.TickCount + vControllerDelayLongerTicks;
                    }
                }
            }
            catch { }
        }
    }
}