using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static DirectXInput.AppVariables;
using static DirectXInput.SettingsNotify;
using static DirectXInput.WindowMain;
using static LibraryShared.Classes;

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
                        SetKeypadMappingProfile();

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
                    GetMouseMovementAmountFromThumb(vKeypadMappingProfile.KeypadMouseMoveSensitivity, controllerInput.ThumbRightX, controllerInput.ThumbRightY, true, out int moveHorizontalRight, out int moveVerticalRight);

                    //Update button press status
                    vKeypadDownStatus.ThumbRightLeft.Pressed = controllerInput.ThumbRightX < -vControllerOffsetMedium;
                    vKeypadDownStatus.ThumbRightRight.Pressed = controllerInput.ThumbRightX > vControllerOffsetMedium;
                    vKeypadDownStatus.ThumbRightUp.Pressed = controllerInput.ThumbRightY > vControllerOffsetMedium;
                    vKeypadDownStatus.ThumbRightDown.Pressed = controllerInput.ThumbRightY < -vControllerOffsetMedium;

                    //Move the mouse cursor
                    vVirtualHidDevice.movRel(moveHorizontalRight, moveVerticalRight);

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
                KeypadKeyPress(controllerInput.DPadLeft.PressedRaw, vKeypadDownStatus.DPadLeft, vKeypadMappingProfile.DPadLeftMod, vKeypadMappingProfile.DPadLeft, vKeypadMappingProfile);

                //Press dpad right
                KeypadKeyPress(controllerInput.DPadRight.PressedRaw, vKeypadDownStatus.DPadRight, vKeypadMappingProfile.DPadRightMod, vKeypadMappingProfile.DPadRight, vKeypadMappingProfile);

                //Press dpad up
                KeypadKeyPress(controllerInput.DPadUp.PressedRaw, vKeypadDownStatus.DPadUp, vKeypadMappingProfile.DPadUpMod, vKeypadMappingProfile.DPadUp, vKeypadMappingProfile);

                //Press dpad down
                KeypadKeyPress(controllerInput.DPadDown.PressedRaw, vKeypadDownStatus.DPadDown, vKeypadMappingProfile.DPadDownMod, vKeypadMappingProfile.DPadDown, vKeypadMappingProfile);

                //Press thumb left left
                bool thumbLeftLeft = controllerInput.ThumbLeftX < -vControllerOffsetMedium;
                KeypadKeyPress(thumbLeftLeft, vKeypadDownStatus.ThumbLeftLeft, vKeypadMappingProfile.ThumbLeftLeftMod, vKeypadMappingProfile.ThumbLeftLeft, vKeypadMappingProfile);

                //Press thumb left right
                bool thumbLeftRight = controllerInput.ThumbLeftX > vControllerOffsetMedium;
                KeypadKeyPress(thumbLeftRight, vKeypadDownStatus.ThumbLeftRight, vKeypadMappingProfile.ThumbLeftRightMod, vKeypadMappingProfile.ThumbLeftRight, vKeypadMappingProfile);

                //Press thumb left up
                bool thumbLeftUp = controllerInput.ThumbLeftY > vControllerOffsetMedium;
                KeypadKeyPress(thumbLeftUp, vKeypadDownStatus.ThumbLeftUp, vKeypadMappingProfile.ThumbLeftUpMod, vKeypadMappingProfile.ThumbLeftUp, vKeypadMappingProfile);

                //Press thumb left down
                bool thumbLeftDown = controllerInput.ThumbLeftY < -vControllerOffsetMedium;
                KeypadKeyPress(thumbLeftDown, vKeypadDownStatus.ThumbLeftDown, vKeypadMappingProfile.ThumbLeftDownMod, vKeypadMappingProfile.ThumbLeftDown, vKeypadMappingProfile);

                //Check if mouse movement is enabled
                if (!vKeypadMappingProfile.KeypadMouseMoveEnabled)
                {
                    //Press thumb right left
                    bool thumbRightLeft = controllerInput.ThumbRightX < -vControllerOffsetMedium;
                    KeypadKeyPress(thumbRightLeft, vKeypadDownStatus.ThumbRightLeft, vKeypadMappingProfile.ThumbRightLeftMod, vKeypadMappingProfile.ThumbRightLeft, vKeypadMappingProfile);

                    //Press thumb right right
                    bool thumbRightRight = controllerInput.ThumbRightX > vControllerOffsetMedium;
                    KeypadKeyPress(thumbRightRight, vKeypadDownStatus.ThumbRightRight, vKeypadMappingProfile.ThumbRightRightMod, vKeypadMappingProfile.ThumbRightRight, vKeypadMappingProfile);

                    //Press thumb right up
                    bool thumbRightUp = controllerInput.ThumbRightY > vControllerOffsetMedium;
                    KeypadKeyPress(thumbRightUp, vKeypadDownStatus.ThumbRightUp, vKeypadMappingProfile.ThumbRightUpMod, vKeypadMappingProfile.ThumbRightUp, vKeypadMappingProfile);

                    //Press thumb right down
                    bool thumbRightDown = controllerInput.ThumbRightY < -vControllerOffsetMedium;
                    KeypadKeyPress(thumbRightDown, vKeypadDownStatus.ThumbRightDown, vKeypadMappingProfile.ThumbRightDownMod, vKeypadMappingProfile.ThumbRightDown, vKeypadMappingProfile);
                }

                //Press button a key
                KeypadKeyPress(controllerInput.ButtonA.PressedRaw, vKeypadDownStatus.ButtonA, vKeypadMappingProfile.ButtonAMod, vKeypadMappingProfile.ButtonA, vKeypadMappingProfile);

                //Press button b key
                KeypadKeyPress(controllerInput.ButtonB.PressedRaw, vKeypadDownStatus.ButtonB, vKeypadMappingProfile.ButtonBMod, vKeypadMappingProfile.ButtonB, vKeypadMappingProfile);

                //Press button x key
                KeypadKeyPress(controllerInput.ButtonX.PressedRaw, vKeypadDownStatus.ButtonX, vKeypadMappingProfile.ButtonXMod, vKeypadMappingProfile.ButtonX, vKeypadMappingProfile);

                //Press button y key
                KeypadKeyPress(controllerInput.ButtonY.PressedRaw, vKeypadDownStatus.ButtonY, vKeypadMappingProfile.ButtonYMod, vKeypadMappingProfile.ButtonY, vKeypadMappingProfile);

                //Press button back key
                KeypadKeyPress(controllerInput.ButtonBack.PressedRaw, vKeypadDownStatus.ButtonBack, vKeypadMappingProfile.ButtonBackMod, vKeypadMappingProfile.ButtonBack, vKeypadMappingProfile);

                //Press button start key
                KeypadKeyPress(controllerInput.ButtonStart.PressedRaw, vKeypadDownStatus.ButtonStart, vKeypadMappingProfile.ButtonStartMod, vKeypadMappingProfile.ButtonStart, vKeypadMappingProfile);

                //Press button shoulder left key
                KeypadKeyPress(controllerInput.ButtonShoulderLeft.PressedRaw, vKeypadDownStatus.ButtonShoulderLeft, vKeypadMappingProfile.ButtonShoulderLeftMod, vKeypadMappingProfile.ButtonShoulderLeft, vKeypadMappingProfile);

                //Press button trigger left key
                KeypadKeyPress(controllerInput.ButtonTriggerLeft.PressedRaw, vKeypadDownStatus.ButtonTriggerLeft, vKeypadMappingProfile.ButtonTriggerLeftMod, vKeypadMappingProfile.ButtonTriggerLeft, vKeypadMappingProfile);

                //Press button thumb left key
                KeypadKeyPress(controllerInput.ButtonThumbLeft.PressedRaw, vKeypadDownStatus.ButtonThumbLeft, vKeypadMappingProfile.ButtonThumbLeftMod, vKeypadMappingProfile.ButtonThumbLeft, vKeypadMappingProfile);

                //Press button shoulder right key
                KeypadKeyPress(controllerInput.ButtonShoulderRight.PressedRaw, vKeypadDownStatus.ButtonShoulderRight, vKeypadMappingProfile.ButtonShoulderRightMod, vKeypadMappingProfile.ButtonShoulderRight, vKeypadMappingProfile);

                //Press button trigger right key
                KeypadKeyPress(controllerInput.ButtonTriggerRight.PressedRaw, vKeypadDownStatus.ButtonTriggerRight, vKeypadMappingProfile.ButtonTriggerRightMod, vKeypadMappingProfile.ButtonTriggerRight, vKeypadMappingProfile);

                //Press button thumb right key
                KeypadKeyPress(controllerInput.ButtonThumbRight.PressedRaw, vKeypadDownStatus.ButtonThumbRight, vKeypadMappingProfile.ButtonThumbRightMod, vKeypadMappingProfile.ButtonThumbRight, vKeypadMappingProfile);
            }
            catch { }
        }

        //Keypad release keyboard buttons
        public void ControllerInteractionKeypadRelease()
        {
            try
            {
                //Release dpad left
                KeypadKeyRelease(vKeypadDownStatus.DPadLeft, vKeypadMappingProfile.DPadLeftMod, vKeypadMappingProfile.DPadLeft);

                //Release dpad right
                KeypadKeyRelease(vKeypadDownStatus.DPadRight, vKeypadMappingProfile.DPadRightMod, vKeypadMappingProfile.DPadRight);

                //Release dpad up
                KeypadKeyRelease(vKeypadDownStatus.DPadUp, vKeypadMappingProfile.DPadUpMod, vKeypadMappingProfile.DPadUp);

                //Release dpad down
                KeypadKeyRelease(vKeypadDownStatus.DPadDown, vKeypadMappingProfile.DPadDownMod, vKeypadMappingProfile.DPadDown);

                //Release thumb left left
                KeypadKeyRelease(vKeypadDownStatus.ThumbLeftLeft, vKeypadMappingProfile.ThumbLeftLeftMod, vKeypadMappingProfile.ThumbLeftLeft);

                //Release thumb left right
                KeypadKeyRelease(vKeypadDownStatus.ThumbLeftRight, vKeypadMappingProfile.ThumbLeftRightMod, vKeypadMappingProfile.ThumbLeftRight);

                //Release thumb left up
                KeypadKeyRelease(vKeypadDownStatus.ThumbLeftUp, vKeypadMappingProfile.ThumbLeftUpMod, vKeypadMappingProfile.ThumbLeftUp);

                //Release thumb left down
                KeypadKeyRelease(vKeypadDownStatus.ThumbLeftDown, vKeypadMappingProfile.ThumbLeftDownMod, vKeypadMappingProfile.ThumbLeftDown);

                //Release thumb right left
                KeypadKeyRelease(vKeypadDownStatus.ThumbRightLeft, vKeypadMappingProfile.ThumbRightLeftMod, vKeypadMappingProfile.ThumbRightLeft);

                //Release thumb right right
                KeypadKeyRelease(vKeypadDownStatus.ThumbRightRight, vKeypadMappingProfile.ThumbRightRightMod, vKeypadMappingProfile.ThumbRightRight);

                //Release thumb right up
                KeypadKeyRelease(vKeypadDownStatus.ThumbRightUp, vKeypadMappingProfile.ThumbRightUpMod, vKeypadMappingProfile.ThumbRightUp);

                //Release thumb right down
                KeypadKeyRelease(vKeypadDownStatus.ThumbRightDown, vKeypadMappingProfile.ThumbRightDownMod, vKeypadMappingProfile.ThumbRightDown);

                //Release button a key
                KeypadKeyRelease(vKeypadDownStatus.ButtonA, vKeypadMappingProfile.ButtonAMod, vKeypadMappingProfile.ButtonA);

                //Release button b key
                KeypadKeyRelease(vKeypadDownStatus.ButtonB, vKeypadMappingProfile.ButtonBMod, vKeypadMappingProfile.ButtonB);

                //Release button x key
                KeypadKeyRelease(vKeypadDownStatus.ButtonX, vKeypadMappingProfile.ButtonXMod, vKeypadMappingProfile.ButtonX);

                //Release button y key
                KeypadKeyRelease(vKeypadDownStatus.ButtonY, vKeypadMappingProfile.ButtonYMod, vKeypadMappingProfile.ButtonY);

                //Release button back key
                KeypadKeyRelease(vKeypadDownStatus.ButtonBack, vKeypadMappingProfile.ButtonBackMod, vKeypadMappingProfile.ButtonBack);

                //Release button start key
                KeypadKeyRelease(vKeypadDownStatus.ButtonStart, vKeypadMappingProfile.ButtonStartMod, vKeypadMappingProfile.ButtonStart);

                //Release button shoulder left key
                KeypadKeyRelease(vKeypadDownStatus.ButtonShoulderLeft, vKeypadMappingProfile.ButtonShoulderLeftMod, vKeypadMappingProfile.ButtonShoulderLeft);

                //Release button trigger left key
                KeypadKeyRelease(vKeypadDownStatus.ButtonTriggerLeft, vKeypadMappingProfile.ButtonTriggerLeftMod, vKeypadMappingProfile.ButtonTriggerLeft);

                //Release button thumb left key
                KeypadKeyRelease(vKeypadDownStatus.ButtonThumbLeft, vKeypadMappingProfile.ButtonThumbLeftMod, vKeypadMappingProfile.ButtonThumbLeft);

                //Release button shoulder right key
                KeypadKeyRelease(vKeypadDownStatus.ButtonShoulderRight, vKeypadMappingProfile.ButtonShoulderRightMod, vKeypadMappingProfile.ButtonShoulderRight);

                //Release button trigger right key
                KeypadKeyRelease(vKeypadDownStatus.ButtonTriggerRight, vKeypadMappingProfile.ButtonTriggerRightMod, vKeypadMappingProfile.ButtonTriggerRight);

                //Release button thumb right key
                KeypadKeyRelease(vKeypadDownStatus.ButtonThumbRight, vKeypadMappingProfile.ButtonThumbRightMod, vKeypadMappingProfile.ButtonThumbRight);
            }
            catch { }
        }

        //Press keyboard key binded to keypad
        void KeypadKeyPress(bool buttonPressed, KeypadDownStatus keypadDownStatus, KeysVirtual? modifierKey, KeysVirtual? virtualKey, KeypadMapping keypadMapping)
        {
            try
            {
                if (buttonPressed)
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
                            KeyToggleComboAuto((KeysVirtual)modifierKey, (KeysVirtual)virtualKey, true);
                        }
                        else
                        {
                            KeyToggleSingleAuto((KeysVirtual)virtualKey, true);
                        }
                    }
                }
                else if (keypadDownStatus.Pressed)
                {
                    keypadDownStatus.RepeatCount = 0;
                    keypadDownStatus.Pressed = false;
                    if (modifierKey != null)
                    {
                        KeyToggleComboAuto((KeysVirtual)modifierKey, (KeysVirtual)virtualKey, false);
                    }
                    else
                    {
                        KeyToggleSingleAuto((KeysVirtual)virtualKey, false);
                    }
                }
            }
            catch { }
        }

        //Release keyboard key binded to keypad
        void KeypadKeyRelease(KeypadDownStatus keypadDownStatus, KeysVirtual? modifierKey, KeysVirtual? virtualKey)
        {
            try
            {
                keypadDownStatus.RepeatCount = 0;
                keypadDownStatus.Pressed = false;
                if (modifierKey != null)
                {
                    KeyToggleComboAuto((KeysVirtual)modifierKey, (KeysVirtual)virtualKey, false);
                }
                else
                {
                    KeyToggleSingleAuto((KeysVirtual)virtualKey, false);
                }
            }
            catch { }
        }
    }
}