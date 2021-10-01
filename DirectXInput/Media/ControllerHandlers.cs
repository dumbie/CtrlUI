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
            bool ControllerDelay500 = false;
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
                        ControllerDelay500 = true;
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
                    if (ControllerDelay500)
                    {
                        vControllerDelay_Mouse = GetSystemTicksMs() + vControllerDelayTicks500;
                    }
                    else
                    {
                        vControllerDelay_Mouse = GetSystemTicksMs() + vControllerDelayTicks10;
                    }
                }
            }
            catch { }
        }

        //Process controller input for keyboard
        public async Task ControllerInteractionKeyboard(ControllerInput ControllerInput)
        {
            bool ControllerDelay125 = false;
            bool ControllerDelay250 = false;
            bool ControllerDelay750 = false;
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_Media)
                {
                    //Send internal arrow left key
                    if (ControllerInput.DPadLeft.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        KeySendSingle(KeysVirtual.Left, vInteropWindowHandle);

                        ControllerDelay125 = true;
                    }
                    //Send internal arrow right key
                    else if (ControllerInput.DPadRight.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        KeySendSingle(KeysVirtual.Right, vInteropWindowHandle);

                        ControllerDelay125 = true;
                    }
                    //Send internal arrow up key
                    else if (ControllerInput.DPadUp.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        KeySendSingle(KeysVirtual.Up, vInteropWindowHandle);

                        ControllerDelay125 = true;
                    }
                    //Send internal arrow down key
                    else if (ControllerInput.DPadDown.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        KeySendSingle(KeysVirtual.Down, vInteropWindowHandle);

                        ControllerDelay125 = true;
                    }

                    //Send internal space key
                    else if (ControllerInput.ButtonA.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        KeySendSingle(KeysVirtual.Space, vInteropWindowHandle);

                        ControllerDelay250 = true;
                    }
                    //Send external media next key
                    else if (ControllerInput.ButtonB.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await App.vWindowOverlay.Notification_Show_Status("MediaNext", "Going to next media item");
                        vFakerInputDevice.MultimediaPressRelease(KeyboardMultimedia.Next);

                        ControllerDelay250 = true;
                    }
                    //Send external media playpause key
                    else if (ControllerInput.ButtonY.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await App.vWindowOverlay.Notification_Show_Status("MediaPlayPause", "Resuming or pausing media");
                        vFakerInputDevice.MultimediaPressRelease(KeyboardMultimedia.PlayPause);

                        ControllerDelay250 = true;
                    }
                    //Send external media previous key
                    else if (ControllerInput.ButtonX.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await App.vWindowOverlay.Notification_Show_Status("MediaPrevious", "Going to previous media item");
                        vFakerInputDevice.MultimediaPressRelease(KeyboardMultimedia.Previous);

                        ControllerDelay250 = true;
                    }

                    //Send external arrow keys
                    else if (ControllerInput.ButtonThumbLeft.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await App.vWindowOverlay.Notification_Show_Status("ArrowLeft", "Moving left");
                        vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.None, KeyboardModifiers.None, KeyboardKeys.ArrowLeft, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);

                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.ButtonThumbRight.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await App.vWindowOverlay.Notification_Show_Status("ArrowRight", "Moving right");
                        vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.None, KeyboardModifiers.None, KeyboardKeys.ArrowRight, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);

                        ControllerDelay125 = true;
                    }

                    //Send external alt+enter keys
                    else if (ControllerInput.ButtonBack.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await App.vWindowOverlay.Notification_Show_Status("MediaFullscreen", "Toggling fullscreen");
                        vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.AltLeft, KeyboardModifiers.None, KeyboardKeys.Enter, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);

                        ControllerDelay250 = true;
                    }

                    //Change the system volume
                    else if (ControllerInput.TriggerLeft > 0 && ControllerInput.TriggerRight > 0)
                    {
                        await App.vWindowOverlay.Notification_Show_Status("VolumeMute", "Toggling output mute");
                        vFakerInputDevice.MultimediaPressRelease(KeyboardMultimedia.VolumeMute);

                        ControllerDelay750 = true;
                    }
                    else if (ControllerInput.ButtonStart.PressedRaw)
                    {
                        await App.vWindowOverlay.Notification_Show_Status("VolumeMute", "Toggling output mute");
                        vFakerInputDevice.MultimediaPressRelease(KeyboardMultimedia.VolumeMute);

                        ControllerDelay250 = true;
                    }
                    else if (ControllerInput.TriggerLeft > 0)
                    {
                        await App.vWindowOverlay.Notification_Show_Status("VolumeDown", "Decreasing volume");
                        vFakerInputDevice.MultimediaPressRelease(KeyboardMultimedia.VolumeDown);

                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.TriggerRight > 0)
                    {
                        await App.vWindowOverlay.Notification_Show_Status("VolumeUp", "Increasing volume");
                        vFakerInputDevice.MultimediaPressRelease(KeyboardMultimedia.VolumeUp);

                        ControllerDelay125 = true;
                    }

                    //Delay input to prevent repeat
                    if (ControllerDelay125)
                    {
                        vControllerDelay_Media = GetSystemTicksMs() + vControllerDelayTicks125;
                    }
                    else if (ControllerDelay250)
                    {
                        vControllerDelay_Media = GetSystemTicksMs() + vControllerDelayTicks250;
                    }
                    else if (ControllerDelay750)
                    {
                        vControllerDelay_Media = GetSystemTicksMs() + vControllerDelayTicks750;
                    }
                }
            }
            catch { }
        }
    }
}