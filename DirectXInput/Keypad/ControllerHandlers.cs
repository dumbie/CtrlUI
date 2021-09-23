using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVInputOutputMouse;
using static DirectXInput.AppVariables;
using static DirectXInput.SettingsNotify;
using static DirectXInput.WindowMain;
using static LibraryShared.Classes;
using static LibraryUsb.FakerInputDevice;

namespace DirectXInput.KeypadCode
{
    partial class WindowKeypad
    {
        //Update keypad preview and profile
        public async Task ControllerInteractionKeypadUpdate()
        {
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_Keypad)
                {
                    //Update interface controller preview
                    UpdateKeypadPreview();

                    //Check if the keypad process changed
                    string processNameLower = vProcessForeground.Name.ToLower();
                    string processTitleLower = vProcessForeground.Title.ToLower();
                    if (processNameLower != vKeypadPreviousProcessName || processTitleLower != vKeypadPreviousProcessTitle)
                    {
                        Debug.WriteLine("Keypad process changed to: " + processNameLower + "/" + processTitleLower);
                        vKeypadPreviousProcessName = processNameLower;
                        vKeypadPreviousProcessTitle = processTitleLower;

                        //Set the keypad mapping profile
                        await SetKeypadMappingProfile();

                        //Update the key names
                        UpdateKeypadNames();

                        //Update the keypad opacity
                        UpdatePopupOpacity();

                        //Update the keypad style
                        UpdateKeypadStyle();

                        //Update the keypad size
                        double keypadHeight = UpdateKeypadSize();

                        //Notify - Fps Overlayer keypad size changed
                        await NotifyFpsOverlayerKeypadSizeChanged(Convert.ToInt32(keypadHeight));
                    }

                    vControllerDelay_Keypad = GetSystemTicksMs() + vControllerDelayMicroTicks;
                }
            }
            catch { }
        }

        //Process controller input for mouse
        public void ControllerInteractionMouse(ControllerInput controllerInput)
        {
            try
            {
                //Check if mouse movement is enabled
                if (vKeypadMappingProfile.KeypadMouseMoveEnabled && GetSystemTicksMs() >= vControllerDelay_Mouse)
                {
                    //Get the mouse move amount
                    GetMouseMovementAmountFromThumbGame(vKeypadMappingProfile.KeypadMouseMoveSensitivity, controllerInput.ThumbRightX, controllerInput.ThumbRightY, true, out int moveHorizontalRight, out int moveVerticalRight);

                    //Update button press status
                    vKeypadDownStatus.ThumbRightLeft.Pressed = controllerInput.ThumbRightX < -vControllerOffsetMedium;
                    vKeypadDownStatus.ThumbRightRight.Pressed = controllerInput.ThumbRightX > vControllerOffsetMedium;
                    vKeypadDownStatus.ThumbRightUp.Pressed = controllerInput.ThumbRightY > vControllerOffsetMedium;
                    vKeypadDownStatus.ThumbRightDown.Pressed = controllerInput.ThumbRightY < -vControllerOffsetMedium;

                    //Move the mouse cursor
                    MouseMoveCursor(moveHorizontalRight, moveVerticalRight);

                    //Delay input to prevent repeat
                    vControllerDelay_Mouse = GetSystemTicksMs() + vControllerDelayNanoTicks;
                }
            }
            catch { }
        }

        //Process controller input for keyboard
        public void ControllerInteractionKeyboard(ControllerInput controllerInput)
        {
            try
            {
                //Press dpad left
                KeypadKeyCheck(controllerInput.DPadLeft.PressedRaw, vKeypadDownStatus.DPadLeft, vKeypadMappingProfile.DPadLeftMod0, vKeypadMappingProfile.DPadLeft, vKeypadMappingProfile);

                //Press dpad right
                KeypadKeyCheck(controllerInput.DPadRight.PressedRaw, vKeypadDownStatus.DPadRight, vKeypadMappingProfile.DPadRightMod0, vKeypadMappingProfile.DPadRight, vKeypadMappingProfile);

                //Press dpad up
                KeypadKeyCheck(controllerInput.DPadUp.PressedRaw, vKeypadDownStatus.DPadUp, vKeypadMappingProfile.DPadUpMod0, vKeypadMappingProfile.DPadUp, vKeypadMappingProfile);

                //Press dpad down
                KeypadKeyCheck(controllerInput.DPadDown.PressedRaw, vKeypadDownStatus.DPadDown, vKeypadMappingProfile.DPadDownMod0, vKeypadMappingProfile.DPadDown, vKeypadMappingProfile);

                //Press thumb left left
                bool thumbLeftLeft = controllerInput.ThumbLeftX < -vControllerOffsetMedium;
                KeypadKeyCheck(thumbLeftLeft, vKeypadDownStatus.ThumbLeftLeft, vKeypadMappingProfile.ThumbLeftLeftMod0, vKeypadMappingProfile.ThumbLeftLeft, vKeypadMappingProfile);

                //Press thumb left right
                bool thumbLeftRight = controllerInput.ThumbLeftX > vControllerOffsetMedium;
                KeypadKeyCheck(thumbLeftRight, vKeypadDownStatus.ThumbLeftRight, vKeypadMappingProfile.ThumbLeftRightMod0, vKeypadMappingProfile.ThumbLeftRight, vKeypadMappingProfile);

                //Press thumb left up
                bool thumbLeftUp = controllerInput.ThumbLeftY > vControllerOffsetMedium;
                KeypadKeyCheck(thumbLeftUp, vKeypadDownStatus.ThumbLeftUp, vKeypadMappingProfile.ThumbLeftUpMod0, vKeypadMappingProfile.ThumbLeftUp, vKeypadMappingProfile);

                //Press thumb left down
                bool thumbLeftDown = controllerInput.ThumbLeftY < -vControllerOffsetMedium;
                KeypadKeyCheck(thumbLeftDown, vKeypadDownStatus.ThumbLeftDown, vKeypadMappingProfile.ThumbLeftDownMod0, vKeypadMappingProfile.ThumbLeftDown, vKeypadMappingProfile);

                //Check if mouse movement is enabled
                if (!vKeypadMappingProfile.KeypadMouseMoveEnabled)
                {
                    //Press thumb right left
                    bool thumbRightLeft = controllerInput.ThumbRightX < -vControllerOffsetMedium;
                    KeypadKeyCheck(thumbRightLeft, vKeypadDownStatus.ThumbRightLeft, vKeypadMappingProfile.ThumbRightLeftMod0, vKeypadMappingProfile.ThumbRightLeft, vKeypadMappingProfile);

                    //Press thumb right right
                    bool thumbRightRight = controllerInput.ThumbRightX > vControllerOffsetMedium;
                    KeypadKeyCheck(thumbRightRight, vKeypadDownStatus.ThumbRightRight, vKeypadMappingProfile.ThumbRightRightMod0, vKeypadMappingProfile.ThumbRightRight, vKeypadMappingProfile);

                    //Press thumb right up
                    bool thumbRightUp = controllerInput.ThumbRightY > vControllerOffsetMedium;
                    KeypadKeyCheck(thumbRightUp, vKeypadDownStatus.ThumbRightUp, vKeypadMappingProfile.ThumbRightUpMod0, vKeypadMappingProfile.ThumbRightUp, vKeypadMappingProfile);

                    //Press thumb right down
                    bool thumbRightDown = controllerInput.ThumbRightY < -vControllerOffsetMedium;
                    KeypadKeyCheck(thumbRightDown, vKeypadDownStatus.ThumbRightDown, vKeypadMappingProfile.ThumbRightDownMod0, vKeypadMappingProfile.ThumbRightDown, vKeypadMappingProfile);
                }

                //Press button a key
                KeypadKeyCheck(controllerInput.ButtonA.PressedRaw, vKeypadDownStatus.ButtonA, vKeypadMappingProfile.ButtonAMod0, vKeypadMappingProfile.ButtonA, vKeypadMappingProfile);

                //Press button b key
                KeypadKeyCheck(controllerInput.ButtonB.PressedRaw, vKeypadDownStatus.ButtonB, vKeypadMappingProfile.ButtonBMod0, vKeypadMappingProfile.ButtonB, vKeypadMappingProfile);

                //Press button x key
                KeypadKeyCheck(controllerInput.ButtonX.PressedRaw, vKeypadDownStatus.ButtonX, vKeypadMappingProfile.ButtonXMod0, vKeypadMappingProfile.ButtonX, vKeypadMappingProfile);

                //Press button y key
                KeypadKeyCheck(controllerInput.ButtonY.PressedRaw, vKeypadDownStatus.ButtonY, vKeypadMappingProfile.ButtonYMod0, vKeypadMappingProfile.ButtonY, vKeypadMappingProfile);

                //Press button back key
                KeypadKeyCheck(controllerInput.ButtonBack.PressedRaw, vKeypadDownStatus.ButtonBack, vKeypadMappingProfile.ButtonBackMod0, vKeypadMappingProfile.ButtonBack, vKeypadMappingProfile);

                //Press button start key
                KeypadKeyCheck(controllerInput.ButtonStart.PressedRaw, vKeypadDownStatus.ButtonStart, vKeypadMappingProfile.ButtonStartMod0, vKeypadMappingProfile.ButtonStart, vKeypadMappingProfile);

                //Press button shoulder left key
                KeypadKeyCheck(controllerInput.ButtonShoulderLeft.PressedRaw, vKeypadDownStatus.ButtonShoulderLeft, vKeypadMappingProfile.ButtonShoulderLeftMod0, vKeypadMappingProfile.ButtonShoulderLeft, vKeypadMappingProfile);

                //Press button trigger left key
                KeypadKeyCheck(controllerInput.ButtonTriggerLeft.PressedRaw, vKeypadDownStatus.ButtonTriggerLeft, vKeypadMappingProfile.ButtonTriggerLeftMod0, vKeypadMappingProfile.ButtonTriggerLeft, vKeypadMappingProfile);

                //Press button thumb left key
                KeypadKeyCheck(controllerInput.ButtonThumbLeft.PressedRaw, vKeypadDownStatus.ButtonThumbLeft, vKeypadMappingProfile.ButtonThumbLeftMod0, vKeypadMappingProfile.ButtonThumbLeft, vKeypadMappingProfile);

                //Press button shoulder right key
                KeypadKeyCheck(controllerInput.ButtonShoulderRight.PressedRaw, vKeypadDownStatus.ButtonShoulderRight, vKeypadMappingProfile.ButtonShoulderRightMod0, vKeypadMappingProfile.ButtonShoulderRight, vKeypadMappingProfile);

                //Press button trigger right key
                KeypadKeyCheck(controllerInput.ButtonTriggerRight.PressedRaw, vKeypadDownStatus.ButtonTriggerRight, vKeypadMappingProfile.ButtonTriggerRightMod0, vKeypadMappingProfile.ButtonTriggerRight, vKeypadMappingProfile);

                //Press button thumb right key
                KeypadKeyCheck(controllerInput.ButtonThumbRight.PressedRaw, vKeypadDownStatus.ButtonThumbRight, vKeypadMappingProfile.ButtonThumbRightMod0, vKeypadMappingProfile.ButtonThumbRight, vKeypadMappingProfile);
            }
            catch { }
        }

        //Keypad release keyboard buttons
        public void ControllerInteractionKeypadRelease()
        {
            try
            {
                //Release dpad left
                KeypadKeyRelease(vKeypadDownStatus.DPadLeft, vKeypadMappingProfile.DPadLeftMod0, vKeypadMappingProfile.DPadLeft);

                //Release dpad right
                KeypadKeyRelease(vKeypadDownStatus.DPadRight, vKeypadMappingProfile.DPadRightMod0, vKeypadMappingProfile.DPadRight);

                //Release dpad up
                KeypadKeyRelease(vKeypadDownStatus.DPadUp, vKeypadMappingProfile.DPadUpMod0, vKeypadMappingProfile.DPadUp);

                //Release dpad down
                KeypadKeyRelease(vKeypadDownStatus.DPadDown, vKeypadMappingProfile.DPadDownMod0, vKeypadMappingProfile.DPadDown);

                //Release thumb left left
                KeypadKeyRelease(vKeypadDownStatus.ThumbLeftLeft, vKeypadMappingProfile.ThumbLeftLeftMod0, vKeypadMappingProfile.ThumbLeftLeft);

                //Release thumb left right
                KeypadKeyRelease(vKeypadDownStatus.ThumbLeftRight, vKeypadMappingProfile.ThumbLeftRightMod0, vKeypadMappingProfile.ThumbLeftRight);

                //Release thumb left up
                KeypadKeyRelease(vKeypadDownStatus.ThumbLeftUp, vKeypadMappingProfile.ThumbLeftUpMod0, vKeypadMappingProfile.ThumbLeftUp);

                //Release thumb left down
                KeypadKeyRelease(vKeypadDownStatus.ThumbLeftDown, vKeypadMappingProfile.ThumbLeftDownMod0, vKeypadMappingProfile.ThumbLeftDown);

                //Release thumb right left
                KeypadKeyRelease(vKeypadDownStatus.ThumbRightLeft, vKeypadMappingProfile.ThumbRightLeftMod0, vKeypadMappingProfile.ThumbRightLeft);

                //Release thumb right right
                KeypadKeyRelease(vKeypadDownStatus.ThumbRightRight, vKeypadMappingProfile.ThumbRightRightMod0, vKeypadMappingProfile.ThumbRightRight);

                //Release thumb right up
                KeypadKeyRelease(vKeypadDownStatus.ThumbRightUp, vKeypadMappingProfile.ThumbRightUpMod0, vKeypadMappingProfile.ThumbRightUp);

                //Release thumb right down
                KeypadKeyRelease(vKeypadDownStatus.ThumbRightDown, vKeypadMappingProfile.ThumbRightDownMod0, vKeypadMappingProfile.ThumbRightDown);

                //Release button a key
                KeypadKeyRelease(vKeypadDownStatus.ButtonA, vKeypadMappingProfile.ButtonAMod0, vKeypadMappingProfile.ButtonA);

                //Release button b key
                KeypadKeyRelease(vKeypadDownStatus.ButtonB, vKeypadMappingProfile.ButtonBMod0, vKeypadMappingProfile.ButtonB);

                //Release button x key
                KeypadKeyRelease(vKeypadDownStatus.ButtonX, vKeypadMappingProfile.ButtonXMod0, vKeypadMappingProfile.ButtonX);

                //Release button y key
                KeypadKeyRelease(vKeypadDownStatus.ButtonY, vKeypadMappingProfile.ButtonYMod0, vKeypadMappingProfile.ButtonY);

                //Release button back key
                KeypadKeyRelease(vKeypadDownStatus.ButtonBack, vKeypadMappingProfile.ButtonBackMod0, vKeypadMappingProfile.ButtonBack);

                //Release button start key
                KeypadKeyRelease(vKeypadDownStatus.ButtonStart, vKeypadMappingProfile.ButtonStartMod0, vKeypadMappingProfile.ButtonStart);

                //Release button shoulder left key
                KeypadKeyRelease(vKeypadDownStatus.ButtonShoulderLeft, vKeypadMappingProfile.ButtonShoulderLeftMod0, vKeypadMappingProfile.ButtonShoulderLeft);

                //Release button trigger left key
                KeypadKeyRelease(vKeypadDownStatus.ButtonTriggerLeft, vKeypadMappingProfile.ButtonTriggerLeftMod0, vKeypadMappingProfile.ButtonTriggerLeft);

                //Release button thumb left key
                KeypadKeyRelease(vKeypadDownStatus.ButtonThumbLeft, vKeypadMappingProfile.ButtonThumbLeftMod0, vKeypadMappingProfile.ButtonThumbLeft);

                //Release button shoulder right key
                KeypadKeyRelease(vKeypadDownStatus.ButtonShoulderRight, vKeypadMappingProfile.ButtonShoulderRightMod0, vKeypadMappingProfile.ButtonShoulderRight);

                //Release button trigger right key
                KeypadKeyRelease(vKeypadDownStatus.ButtonTriggerRight, vKeypadMappingProfile.ButtonTriggerRightMod0, vKeypadMappingProfile.ButtonTriggerRight);

                //Release button thumb right key
                KeypadKeyRelease(vKeypadDownStatus.ButtonThumbRight, vKeypadMappingProfile.ButtonThumbRightMod0, vKeypadMappingProfile.ButtonThumbRight);
            }
            catch { }
        }

        //Check keyboard key binded to keypad
        void KeypadKeyCheck(bool buttonPressed, KeypadDownStatus keypadDownStatus, KeyboardModifiers? modifierKey, KeyboardKeys? virtualKey, KeypadMapping keypadMapping)
        {
            try
            {
                if (buttonPressed)
                {
                    KeypadKeyPress(keypadDownStatus, modifierKey, virtualKey, keypadMapping);
                }
                else if (keypadDownStatus.Pressed)
                {
                    KeypadKeyRelease(keypadDownStatus, modifierKey, virtualKey);
                }
            }
            catch { }
        }

        //Press keyboard key binded to keypad
        void KeypadKeyPress(KeypadDownStatus keypadDownStatus, KeyboardModifiers? modifierKey, KeyboardKeys? virtualKey, KeypadMapping keypadMapping)
        {
            try
            {
                long currentPressMs = GetSystemTicksMs();
                if (!keypadDownStatus.Pressed || currentPressMs >= keypadDownStatus.DelayPressMs)
                {
                    keypadDownStatus.Pressed = true;
                    if (keypadDownStatus.RepeatCount == 0)
                    {
                        keypadDownStatus.DelayPressMs = currentPressMs + keypadMapping.ButtonDelayFirstMs;
                    }
                    else
                    {
                        keypadDownStatus.DelayPressMs = currentPressMs + keypadMapping.ButtonDelayRepeatMs;
                    }
                    keypadDownStatus.RepeatCount++;
                    if (modifierKey != null)
                    {
                        //Fix add support for multiple modifiers
                        vFakerInputDevice.KeyboardPress((KeyboardModifiers)modifierKey, KeyboardModifiers.None, (KeyboardKeys)virtualKey, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);
                    }
                    else
                    {
                        vFakerInputDevice.KeyboardPress(KeyboardModifiers.None, KeyboardModifiers.None, (KeyboardKeys)virtualKey, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);
                    }
                }
            }
            catch { }
        }

        //Release keyboard key binded to keypad
        void KeypadKeyRelease(KeypadDownStatus keypadDownStatus, KeyboardModifiers? modifierKey, KeyboardKeys? virtualKey)
        {
            try
            {
                keypadDownStatus.RepeatCount = 0;
                keypadDownStatus.Pressed = false;
                if (modifierKey != null)
                {
                    vFakerInputDevice.KeyboardReset();
                }
                else
                {
                    vFakerInputDevice.KeyboardReset();
                }
            }
            catch { }
        }
    }
}