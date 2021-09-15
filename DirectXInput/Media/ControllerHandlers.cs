using System;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static DirectXInput.AppVariables;
using static DirectXInput.WindowMain;
using static LibraryShared.Classes;
using static LibraryShared.Settings;
using static LibraryShared.SoundPlayer;
using static LibraryUsb.FakerInputDevice;

namespace DirectXInput.MediaCode
{
    partial class WindowMedia
    {
        //Process controller input for mouse
        public void ControllerInteractionMouse(ControllerInput ControllerInput)
        {
            bool ControllerDelayLong = false;
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_Mouse)
                {
                    int scrollHorizontalRight = 0;
                    int scrollVerticalRight = 0;
                    MouseButtons buttonPress = MouseButtons.None;

                    //Get the mouse move amount
                    double moveSensitivity = Convert.ToDouble(Setting_Load(vConfigurationDirectXInput, "KeyboardMouseMoveSensitivity"));
                    GetMouseMovementAmountFromThumbDesktop(moveSensitivity, ControllerInput.ThumbLeftX, ControllerInput.ThumbLeftY, true, out int moveHorizontalLeft, out int moveVerticalLeft);
                    GetMouseMovementAmountFromThumbDesktop(moveSensitivity, ControllerInput.ThumbRightX, ControllerInput.ThumbRightY, true, out int moveHorizontalRight, out int moveVerticalRight);

                    //Move the keyboard window
                    MoveMediaWindow(moveHorizontalRight, moveVerticalRight);

                    //Emulate mouse button press
                    if (ControllerInput.ButtonShoulderLeft.PressedRaw && ControllerInput.ButtonShoulderRight.PressedRaw)
                    {
                        buttonPress = MouseButtons.MiddleButton;
                        ControllerDelayLong = true;
                    }
                    else if (ControllerInput.ButtonShoulderLeft.PressedRaw)
                    {
                        buttonPress = MouseButtons.LeftButton;
                    }
                    else if (ControllerInput.ButtonShoulderRight.PressedRaw)
                    {
                        buttonPress = MouseButtons.RightButton;
                    }
                    else
                    {
                        buttonPress = MouseButtons.None;
                    }

                    //Update current mouse input
                    vFakerInputDevice.MouseRelative(moveHorizontalLeft, moveVerticalLeft, scrollHorizontalRight, scrollVerticalRight, buttonPress);

                    //Delay input to prevent repeat
                    if (ControllerDelayLong)
                    {
                        vControllerDelay_Mouse = GetSystemTicksMs() + vControllerDelayLongTicks;
                    }
                    else
                    {
                        vControllerDelay_Mouse = GetSystemTicksMs() + vControllerDelayNanoTicks;
                    }
                }
            }
            catch { }
        }

        //Process controller input for keyboard
        public async Task ControllerInteractionKeyboard(ControllerInput ControllerInput)
        {
            bool ControllerDelayShort = false;
            bool ControllerDelayMedium = false;
            bool ControllerDelayLonger = false;
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_Media)
                {
                    //Send internal arrow left key
                    if (ControllerInput.DPadLeft.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        KeySendSingle(KeysVirtual.Left, vInteropWindowHandle);

                        ControllerDelayShort = true;
                    }
                    //Send internal arrow right key
                    else if (ControllerInput.DPadRight.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        KeySendSingle(KeysVirtual.Right, vInteropWindowHandle);

                        ControllerDelayShort = true;
                    }
                    //Send internal arrow up key
                    else if (ControllerInput.DPadUp.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        KeySendSingle(KeysVirtual.Up, vInteropWindowHandle);

                        ControllerDelayShort = true;
                    }
                    //Send internal arrow down key
                    else if (ControllerInput.DPadDown.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        KeySendSingle(KeysVirtual.Down, vInteropWindowHandle);

                        ControllerDelayShort = true;
                    }

                    //Send internal space key
                    else if (ControllerInput.ButtonA.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        KeySendSingle(KeysVirtual.Space, vInteropWindowHandle);

                        ControllerDelayMedium = true;
                    }
                    //Send external media next key
                    else if (ControllerInput.ButtonB.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await App.vWindowOverlay.Notification_Show_Status("MediaNext", "Going to next media item");
                        vFakerInputDevice.MultimediaPressRelease(KeyboardMultimedia.Next);

                        ControllerDelayMedium = true;
                    }
                    //Send external media playpause key
                    else if (ControllerInput.ButtonY.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await App.vWindowOverlay.Notification_Show_Status("MediaPlayPause", "Resuming or pausing media");
                        vFakerInputDevice.MultimediaPressRelease(KeyboardMultimedia.PlayPause);

                        ControllerDelayMedium = true;
                    }
                    //Send external media previous key
                    else if (ControllerInput.ButtonX.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await App.vWindowOverlay.Notification_Show_Status("MediaPrevious", "Going to previous media item");
                        vFakerInputDevice.MultimediaPressRelease(KeyboardMultimedia.Previous);

                        ControllerDelayMedium = true;
                    }

                    //Send external arrow keys
                    else if (ControllerInput.ButtonThumbLeft.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await App.vWindowOverlay.Notification_Show_Status("ArrowLeft", "Moving left");
                        vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.None, KeyboardKeys.ArrowLeft, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);

                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ButtonThumbRight.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await App.vWindowOverlay.Notification_Show_Status("ArrowRight", "Moving right");
                        vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.None, KeyboardKeys.ArrowRight, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);

                        ControllerDelayShort = true;
                    }

                    //Send external alt+enter keys
                    else if (ControllerInput.ButtonBack.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await App.vWindowOverlay.Notification_Show_Status("MediaFullscreen", "Toggling fullscreen");
                        vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.AltLeft, KeyboardKeys.Enter, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);

                        ControllerDelayMedium = true;
                    }

                    //Change the system volume
                    else if (ControllerInput.TriggerLeft > 0 && ControllerInput.TriggerRight > 0)
                    {
                        await App.vWindowOverlay.Notification_Show_Status("VolumeMute", "Toggling output mute");
                        vFakerInputDevice.MultimediaPressRelease(KeyboardMultimedia.VolumeMute);

                        ControllerDelayLonger = true;
                    }
                    else if (ControllerInput.ButtonStart.PressedRaw)
                    {
                        await App.vWindowOverlay.Notification_Show_Status("VolumeMute", "Toggling output mute");
                        vFakerInputDevice.MultimediaPressRelease(KeyboardMultimedia.VolumeMute);

                        ControllerDelayMedium = true;
                    }
                    else if (ControllerInput.TriggerLeft > 0)
                    {
                        await App.vWindowOverlay.Notification_Show_Status("VolumeDown", "Decreasing volume");
                        vFakerInputDevice.MultimediaPressRelease(KeyboardMultimedia.VolumeDown);

                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.TriggerRight > 0)
                    {
                        await App.vWindowOverlay.Notification_Show_Status("VolumeUp", "Increasing volume");
                        vFakerInputDevice.MultimediaPressRelease(KeyboardMultimedia.VolumeUp);

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
                }
            }
            catch { }
        }
    }
}