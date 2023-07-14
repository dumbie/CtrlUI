using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputMouse;
using static DirectXInput.AppVariables;
using static DirectXInput.WindowMain;
using static LibraryShared.Classes;
using static LibraryShared.ControllerTimings;

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
                    KeysHidAction keyboardAction = new KeysHidAction();
                    KeysModifierHid pressedModifiers = KeysModifierHid.None;

                    //DPad
                    if (controllerInput.DPadLeft.PressedRaw)
                    {
                        if (vKeypadMappingProfile.DPadLeftMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.DPadLeftMod0; }
                        if (vKeypadMappingProfile.DPadLeftMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.DPadLeftMod1; }
                        if (vKeypadMappingProfile.DPadLeft != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.DPadLeft); }
                    }
                    if (controllerInput.DPadRight.PressedRaw)
                    {
                        if (vKeypadMappingProfile.DPadRightMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.DPadRightMod0; }
                        if (vKeypadMappingProfile.DPadRightMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.DPadRightMod1; }
                        if (vKeypadMappingProfile.DPadRight != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.DPadRight); }
                    }
                    if (controllerInput.DPadUp.PressedRaw)
                    {
                        if (vKeypadMappingProfile.DPadUpMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.DPadUpMod0; }
                        if (vKeypadMappingProfile.DPadUpMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.DPadUpMod1; }
                        if (vKeypadMappingProfile.DPadUp != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.DPadUp); }
                    }
                    if (controllerInput.DPadDown.PressedRaw)
                    {
                        if (vKeypadMappingProfile.DPadDownMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.DPadDownMod0; }
                        if (vKeypadMappingProfile.DPadDownMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.DPadDownMod1; }
                        if (vKeypadMappingProfile.DPadDown != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.DPadDown); }
                    }

                    //Thumb Left
                    if (controllerInput.ButtonThumbLeftLeft.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ThumbLeftLeftMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ThumbLeftLeftMod0; }
                        if (vKeypadMappingProfile.ThumbLeftLeftMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ThumbLeftLeftMod1; }
                        if (vKeypadMappingProfile.ThumbLeftLeft != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ThumbLeftLeft); }
                    }
                    if (controllerInput.ButtonThumbLeftUp.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ThumbLeftUpMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ThumbLeftUpMod0; }
                        if (vKeypadMappingProfile.ThumbLeftUpMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ThumbLeftUpMod1; }
                        if (vKeypadMappingProfile.ThumbLeftUp != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ThumbLeftUp); }
                    }
                    if (controllerInput.ButtonThumbLeftRight.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ThumbLeftRightMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ThumbLeftRightMod0; }
                        if (vKeypadMappingProfile.ThumbLeftRightMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ThumbLeftRightMod1; }
                        if (vKeypadMappingProfile.ThumbLeftRight != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ThumbLeftRight); }
                    }
                    if (controllerInput.ButtonThumbLeftDown.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ThumbLeftDownMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ThumbLeftDownMod0; }
                        if (vKeypadMappingProfile.ThumbLeftDownMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ThumbLeftDownMod1; }
                        if (vKeypadMappingProfile.ThumbLeftDown != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ThumbLeftDown); }
                    }

                    //Thumb Right
                    if (!vKeypadMappingProfile.KeypadMouseMoveEnabled)
                    {
                        if (controllerInput.ButtonThumbRightLeft.PressedRaw)
                        {
                            if (vKeypadMappingProfile.ThumbRightLeftMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ThumbRightLeftMod0; }
                            if (vKeypadMappingProfile.ThumbRightLeftMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ThumbRightLeftMod1; }
                            if (vKeypadMappingProfile.ThumbRightLeft != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ThumbRightLeft); }
                        }
                        if (controllerInput.ButtonThumbRightUp.PressedRaw)
                        {
                            if (vKeypadMappingProfile.ThumbRightUpMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ThumbRightUpMod0; }
                            if (vKeypadMappingProfile.ThumbRightUpMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ThumbRightUpMod1; }
                            if (vKeypadMappingProfile.ThumbRightUp != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ThumbRightUp); }
                        }
                        if (controllerInput.ButtonThumbRightRight.PressedRaw)
                        {
                            if (vKeypadMappingProfile.ThumbRightRightMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ThumbRightRightMod0; }
                            if (vKeypadMappingProfile.ThumbRightRightMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ThumbRightRightMod1; }
                            if (vKeypadMappingProfile.ThumbRightRight != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ThumbRightRight); }
                        }
                        if (controllerInput.ButtonThumbRightDown.PressedRaw)
                        {
                            if (vKeypadMappingProfile.ThumbRightDownMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ThumbRightDownMod0; }
                            if (vKeypadMappingProfile.ThumbRightDownMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ThumbRightDownMod1; }
                            if (vKeypadMappingProfile.ThumbRightDown != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ThumbRightDown); }
                        }
                    }

                    //Buttons
                    if (controllerInput.ButtonA.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonAMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonAMod0; }
                        if (vKeypadMappingProfile.ButtonAMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonAMod1; }
                        if (vKeypadMappingProfile.ButtonA != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonA); }
                    }
                    if (controllerInput.ButtonB.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonBMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonBMod0; }
                        if (vKeypadMappingProfile.ButtonBMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonBMod1; }
                        if (vKeypadMappingProfile.ButtonB != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonB); }
                    }
                    if (controllerInput.ButtonX.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonXMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonXMod0; }
                        if (vKeypadMappingProfile.ButtonXMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonXMod1; }
                        if (vKeypadMappingProfile.ButtonX != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonX); }
                    }
                    if (controllerInput.ButtonY.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonYMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonYMod0; }
                        if (vKeypadMappingProfile.ButtonYMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonYMod1; }
                        if (vKeypadMappingProfile.ButtonY != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonY); }
                    }
                    if (controllerInput.ButtonBack.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonBackMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonBackMod0; }
                        if (vKeypadMappingProfile.ButtonBackMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonBackMod1; }
                        if (vKeypadMappingProfile.ButtonBack != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonBack); }
                    }
                    if (controllerInput.ButtonStart.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonStartMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonStartMod0; }
                        if (vKeypadMappingProfile.ButtonStartMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonStartMod1; }
                        if (vKeypadMappingProfile.ButtonStart != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonStart); }
                    }

                    //Shoulder
                    if (controllerInput.ButtonShoulderLeft.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonShoulderLeftMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonShoulderLeftMod0; }
                        if (vKeypadMappingProfile.ButtonShoulderLeftMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonShoulderLeftMod1; }
                        if (vKeypadMappingProfile.ButtonShoulderLeft != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonShoulderLeft); }
                    }
                    if (controllerInput.ButtonShoulderRight.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonShoulderRightMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonShoulderRightMod0; }
                        if (vKeypadMappingProfile.ButtonShoulderRightMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonShoulderRightMod1; }
                        if (vKeypadMappingProfile.ButtonShoulderRight != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonShoulderRight); }
                    }

                    //Trigger
                    if (controllerInput.ButtonTriggerLeft.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonTriggerLeftMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonTriggerLeftMod0; }
                        if (vKeypadMappingProfile.ButtonTriggerLeftMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonTriggerLeftMod1; }
                        if (vKeypadMappingProfile.ButtonTriggerLeft != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonTriggerLeft); }
                    }
                    if (controllerInput.ButtonTriggerRight.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonTriggerRightMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonTriggerRightMod0; }
                        if (vKeypadMappingProfile.ButtonTriggerRightMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonTriggerRightMod1; }
                        if (vKeypadMappingProfile.ButtonTriggerRight != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonTriggerRight); }
                    }

                    //Thumb
                    if (controllerInput.ButtonThumbLeft.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonThumbLeftMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonThumbLeftMod0; }
                        if (vKeypadMappingProfile.ButtonThumbLeftMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonThumbLeftMod1; }
                        if (vKeypadMappingProfile.ButtonThumbLeft != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonThumbLeft); }
                    }
                    if (controllerInput.ButtonThumbRight.PressedRaw)
                    {
                        if (vKeypadMappingProfile.ButtonThumbRightMod0 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonThumbRightMod0; }
                        if (vKeypadMappingProfile.ButtonThumbRightMod1 != KeysModifierHid.None) { pressedModifiers |= vKeypadMappingProfile.ButtonThumbRightMod1; }
                        if (vKeypadMappingProfile.ButtonThumbRight != KeysHid.None) { UpdateKeyPressByteArray(ref keyboardAction, vKeypadMappingProfile.ButtonThumbRight); }
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
        void UpdateKeyPressByteArray(ref KeysHidAction keyboardAction, KeysHid pressedKey)
        {
            try
            {
                if (keyboardAction.Key0 == KeysHid.None)
                {
                    keyboardAction.Key0 = pressedKey;
                }
                else if (keyboardAction.Key1 == KeysHid.None)
                {
                    keyboardAction.Key1 = pressedKey;
                }
                else if (keyboardAction.Key2 == KeysHid.None)
                {
                    keyboardAction.Key2 = pressedKey;
                }
                else if (keyboardAction.Key3 == KeysHid.None)
                {
                    keyboardAction.Key3 = pressedKey;
                }
                else if (keyboardAction.Key4 == KeysHid.None)
                {
                    keyboardAction.Key4 = pressedKey;
                }
                else if (keyboardAction.Key5 == KeysHid.None)
                {
                    keyboardAction.Key5 = pressedKey;
                }
                else if (keyboardAction.Key5 == KeysHid.None)
                {
                    keyboardAction.Key5 = pressedKey;
                }
                else if (keyboardAction.Key6 == KeysHid.None)
                {
                    keyboardAction.Key6 = pressedKey;
                }
                else if (keyboardAction.Key7 == KeysHid.None)
                {
                    keyboardAction.Key7 = pressedKey;
                }
            }
            catch { }
        }
    }
}