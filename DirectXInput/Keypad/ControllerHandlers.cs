using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVInputOutputMouse;
using static DirectXInput.AppVariables;
using static DirectXInput.WindowMain;
using static LibraryShared.Classes;
using static LibraryUsb.FakerInputDevice;

namespace DirectXInput.KeypadCode
{
    partial class WindowKeypad
    {
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

                    //Move the mouse cursor
                    MouseMoveCursor(moveHorizontalRight, moveVerticalRight);

                    //Delay input to prevent repeat
                    vControllerDelay_Mouse = GetSystemTicksMs() + vControllerDelayTicks10;
                }
            }
            catch { }
        }

        //Process controller input for keyboard
        public void ControllerInteractionKeyboard(ControllerInput controllerInput)
        {
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_KeypadControl)
                {
                    KeyboardAction keyboardAction = new KeyboardAction();
                    KeyboardModifiers pressedModifiers = KeyboardModifiers.None;

                    //DPad
                    if (controllerInput.DPadLeft.PressedRaw)
                    {
                        if (vKeypadMappingProfile.DPadLeftMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.DPadLeftMod0; }
                        if (vKeypadMappingProfile.DPadLeftMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.DPadLeftMod1; }
                        if (vKeypadMappingProfile.DPadLeft != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.DPadLeft); }
                    }
                    if (controllerInput.DPadRight.PressedRaw)
                    {
                        if (vKeypadMappingProfile.DPadRightMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.DPadRightMod0; }
                        if (vKeypadMappingProfile.DPadRightMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.DPadRightMod1; }
                        if (vKeypadMappingProfile.DPadRight != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.DPadRight); }
                    }
                    if (controllerInput.DPadUp.PressedRaw)
                    {
                        if (vKeypadMappingProfile.DPadUpMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.DPadUpMod0; }
                        if (vKeypadMappingProfile.DPadUpMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.DPadUpMod1; }
                        if (vKeypadMappingProfile.DPadUp != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.DPadUp); }
                    }
                    if (controllerInput.DPadDown.PressedRaw)
                    {
                        if (vKeypadMappingProfile.DPadDownMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.DPadDownMod0; }
                        if (vKeypadMappingProfile.DPadDownMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.DPadDownMod1; }
                        if (vKeypadMappingProfile.DPadDown != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.DPadDown); }
                    }

                    //Thumb Left
                    if (controllerInput.ThumbLeftX < -vControllerThumbOffset7500)
                    {
                        if (vKeypadMappingProfile.ThumbLeftLeftMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ThumbLeftLeftMod0; }
                        if (vKeypadMappingProfile.ThumbLeftLeftMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ThumbLeftLeftMod1; }
                        if (vKeypadMappingProfile.ThumbLeftLeft != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ThumbLeftLeft); }
                    }
                    if (controllerInput.ThumbLeftY > vControllerThumbOffset7500)
                    {
                        if (vKeypadMappingProfile.ThumbLeftUpMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ThumbLeftUpMod0; }
                        if (vKeypadMappingProfile.ThumbLeftUpMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ThumbLeftUpMod1; }
                        if (vKeypadMappingProfile.ThumbLeftUp != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ThumbLeftUp); }
                    }
                    if (controllerInput.ThumbLeftX > vControllerThumbOffset7500)
                    {
                        if (vKeypadMappingProfile.ThumbLeftRightMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ThumbLeftRightMod0; }
                        if (vKeypadMappingProfile.ThumbLeftRightMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ThumbLeftRightMod1; }
                        if (vKeypadMappingProfile.ThumbLeftRight != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ThumbLeftRight); }
                    }
                    if (controllerInput.ThumbLeftY < -vControllerThumbOffset7500)
                    {
                        if (vKeypadMappingProfile.ThumbLeftDownMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ThumbLeftDownMod0; }
                        if (vKeypadMappingProfile.ThumbLeftDownMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ThumbLeftDownMod1; }
                        if (vKeypadMappingProfile.ThumbLeftDown != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ThumbLeftDown); }
                    }

                    //Thumb Right
                    if (!vKeypadMappingProfile.KeypadMouseMoveEnabled)
                    {
                        if (controllerInput.ThumbRightX < -vControllerThumbOffset7500)
                        {
                            if (vKeypadMappingProfile.ThumbRightLeftMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ThumbRightLeftMod0; }
                            if (vKeypadMappingProfile.ThumbRightLeftMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ThumbRightLeftMod1; }
                            if (vKeypadMappingProfile.ThumbRightLeft != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ThumbRightLeft); }
                        }
                        if (controllerInput.ThumbRightX > vControllerThumbOffset7500)
                        {
                            if (vKeypadMappingProfile.ThumbRightRightMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ThumbRightRightMod0; }
                            if (vKeypadMappingProfile.ThumbRightRightMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ThumbRightRightMod1; }
                            if (vKeypadMappingProfile.ThumbRightRight != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ThumbRightRight); }
                        }
                        if (controllerInput.ThumbRightY > vControllerThumbOffset7500)
                        {
                            if (vKeypadMappingProfile.ThumbRightUpMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ThumbRightUpMod0; }
                            if (vKeypadMappingProfile.ThumbRightUpMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ThumbRightUpMod1; }
                            if (vKeypadMappingProfile.ThumbRightUp != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ThumbRightUp); }
                        }
                        if (controllerInput.ThumbRightY < -vControllerThumbOffset7500)
                        {
                            if (vKeypadMappingProfile.ThumbRightDownMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ThumbRightDownMod0; }
                            if (vKeypadMappingProfile.ThumbRightDownMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ThumbRightDownMod1; }
                            if (vKeypadMappingProfile.ThumbRightDown != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ThumbRightDown); }
                        }
                    }

                    //Buttons
                    if (controllerInput.ButtonA.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonAMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonAMod0; }
                        if (vKeypadMappingProfile.ButtonAMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonAMod1; }
                        if (vKeypadMappingProfile.ButtonA != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonA); }
                    }
                    if (controllerInput.ButtonB.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonBMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonBMod0; }
                        if (vKeypadMappingProfile.ButtonBMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonBMod1; }
                        if (vKeypadMappingProfile.ButtonB != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonB); }
                    }
                    if (controllerInput.ButtonX.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonXMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonXMod0; }
                        if (vKeypadMappingProfile.ButtonXMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonXMod1; }
                        if (vKeypadMappingProfile.ButtonX != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonX); }
                    }
                    if (controllerInput.ButtonY.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonYMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonYMod0; }
                        if (vKeypadMappingProfile.ButtonYMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonYMod1; }
                        if (vKeypadMappingProfile.ButtonY != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonY); }
                    }
                    if (controllerInput.ButtonBack.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonBackMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonBackMod0; }
                        if (vKeypadMappingProfile.ButtonBackMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonBackMod1; }
                        if (vKeypadMappingProfile.ButtonBack != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonBack); }
                    }
                    if (controllerInput.ButtonStart.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonStartMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonStartMod0; }
                        if (vKeypadMappingProfile.ButtonStartMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonStartMod1; }
                        if (vKeypadMappingProfile.ButtonStart != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonStart); }
                    }

                    //Shoulder
                    if (controllerInput.ButtonShoulderLeft.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonShoulderLeftMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonShoulderLeftMod0; }
                        if (vKeypadMappingProfile.ButtonShoulderLeftMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonShoulderLeftMod1; }
                        if (vKeypadMappingProfile.ButtonShoulderLeft != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonShoulderLeft); }
                    }
                    if (controllerInput.ButtonShoulderRight.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonShoulderRightMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonShoulderRightMod0; }
                        if (vKeypadMappingProfile.ButtonShoulderRightMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonShoulderRightMod1; }
                        if (vKeypadMappingProfile.ButtonShoulderRight != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonShoulderRight); }
                    }

                    //Trigger
                    if (controllerInput.ButtonTriggerLeft.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonTriggerLeftMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonTriggerLeftMod0; }
                        if (vKeypadMappingProfile.ButtonTriggerLeftMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonTriggerLeftMod1; }
                        if (vKeypadMappingProfile.ButtonTriggerLeft != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonTriggerLeft); }
                    }
                    if (controllerInput.ButtonTriggerRight.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonTriggerRightMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonTriggerRightMod0; }
                        if (vKeypadMappingProfile.ButtonTriggerRightMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonTriggerRightMod1; }
                        if (vKeypadMappingProfile.ButtonTriggerRight != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonTriggerRight); }
                    }

                    //Thumb
                    if (controllerInput.ButtonThumbLeft.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonThumbLeftMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonThumbLeftMod0; }
                        if (vKeypadMappingProfile.ButtonThumbLeftMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonThumbLeftMod1; }
                        if (vKeypadMappingProfile.ButtonThumbLeft != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonThumbLeft); }
                    }
                    if (controllerInput.ButtonThumbRight.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonThumbRightMod0 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonThumbRightMod0; }
                        if (vKeypadMappingProfile.ButtonThumbRightMod1 != KeyboardModifiers.None) { pressedModifiers |= vKeypadMappingProfile.ButtonThumbRightMod1; }
                        if (vKeypadMappingProfile.ButtonThumbRight != KeyboardKeys.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonThumbRight); }
                    }

                    //Send key presses to keyboard
                    keyboardAction.Modifiers = pressedModifiers;
                    vFakerInputDevice.KeyboardPress(keyboardAction);

                    //Delay input to prevent repeat
                    vControllerDelay_KeypadControl = GetSystemTicksMs() + vControllerDelayTicks10;
                }
            }
            catch { }
        }

        //Update key press byte array
        void UpdateKeyPressByteArray(ref KeyboardAction keyboardAction, KeyboardKeys pressedKey)
        {
            try
            {
                if (keyboardAction.Key0 == KeyboardKeys.None)
                {
                    keyboardAction.Key0 = pressedKey;
                }
                else if (keyboardAction.Key1 == KeyboardKeys.None)
                {
                    keyboardAction.Key1 = pressedKey;
                }
                else if (keyboardAction.Key2 == KeyboardKeys.None)
                {
                    keyboardAction.Key2 = pressedKey;
                }
                else if (keyboardAction.Key3 == KeyboardKeys.None)
                {
                    keyboardAction.Key3 = pressedKey;
                }
                else if (keyboardAction.Key4 == KeyboardKeys.None)
                {
                    keyboardAction.Key4 = pressedKey;
                }
                else if (keyboardAction.Key5 == KeyboardKeys.None)
                {
                    keyboardAction.Key5 = pressedKey;
                }
                else if (keyboardAction.Key5 == KeyboardKeys.None)
                {
                    keyboardAction.Key5 = pressedKey;
                }
                else if (keyboardAction.Key6 == KeyboardKeys.None)
                {
                    keyboardAction.Key6 = pressedKey;
                }
                else if (keyboardAction.Key7 == KeyboardKeys.None)
                {
                    keyboardAction.Key7 = pressedKey;
                }
            }
            catch { }
        }
    }
}