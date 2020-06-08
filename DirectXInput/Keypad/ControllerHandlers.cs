using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput.Keypad
{
    partial class WindowKeypad
    {
        //Process controller input for keyboard
        public void ControllerInteractionKeyboard(ControllerInput controllerInput)
        {
            try
            {
                if (Environment.TickCount >= vControllerDelay_Keypad)
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
                        UpdateKeypadOpacity();

                        //Update the keypad style
                        UpdateKeypadStyle();
                    }

                    vControllerDelay_Keypad = Environment.TickCount + vControllerDelayMicroTicks;
                }

                //Press thumb left left
                bool thumbLeftLeft = controllerInput.ThumbLeftX < -vControllerOffsetMedium;
                PressKeypadKey(controllerInput.DPadLeft.PressedRaw, vKeypadDownStatus.DPadLeft, vKeypadMappingProfile.DPadLeftMod, vKeypadMappingProfile.DPadLeft, vKeypadMappingProfile.ButtonRepeatIntervalMs);
                PressKeypadKey(thumbLeftLeft, vKeypadDownStatus.ThumbLeftLeft, vKeypadMappingProfile.ThumbLeftLeftMod, vKeypadMappingProfile.ThumbLeftLeft, vKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press thumb left right
                bool thumbLeftRight = controllerInput.ThumbLeftX > vControllerOffsetMedium;
                PressKeypadKey(controllerInput.DPadRight.PressedRaw, vKeypadDownStatus.DPadRight, vKeypadMappingProfile.DPadRightMod, vKeypadMappingProfile.DPadRight, vKeypadMappingProfile.ButtonRepeatIntervalMs);
                PressKeypadKey(thumbLeftRight, vKeypadDownStatus.ThumbLeftRight, vKeypadMappingProfile.ThumbLeftRightMod, vKeypadMappingProfile.ThumbLeftRight, vKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press thumb left up
                bool thumbLeftUp = controllerInput.ThumbLeftY > vControllerOffsetMedium;
                PressKeypadKey(controllerInput.DPadUp.PressedRaw, vKeypadDownStatus.DPadUp, vKeypadMappingProfile.DPadUpMod, vKeypadMappingProfile.DPadUp, vKeypadMappingProfile.ButtonRepeatIntervalMs);
                PressKeypadKey(thumbLeftUp, vKeypadDownStatus.ThumbLeftUp, vKeypadMappingProfile.ThumbLeftUpMod, vKeypadMappingProfile.ThumbLeftUp, vKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press thumb left down
                bool thumbLeftDown = controllerInput.ThumbLeftY < -vControllerOffsetMedium;
                PressKeypadKey(controllerInput.DPadDown.PressedRaw, vKeypadDownStatus.DPadDown, vKeypadMappingProfile.DPadDownMod, vKeypadMappingProfile.DPadDown, vKeypadMappingProfile.ButtonRepeatIntervalMs);
                PressKeypadKey(thumbLeftDown, vKeypadDownStatus.ThumbLeftDown, vKeypadMappingProfile.ThumbLeftDownMod, vKeypadMappingProfile.ThumbLeftDown, vKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press thumb right left
                bool thumbRightLeft = controllerInput.ThumbRightX < -vControllerOffsetMedium;
                PressKeypadKey(thumbRightLeft, vKeypadDownStatus.ThumbRightLeft, vKeypadMappingProfile.ThumbRightLeftMod, vKeypadMappingProfile.ThumbRightLeft, vKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press thumb right right
                bool thumbRightRight = controllerInput.ThumbRightX > vControllerOffsetMedium;
                PressKeypadKey(thumbRightRight, vKeypadDownStatus.ThumbRightRight, vKeypadMappingProfile.ThumbRightRightMod, vKeypadMappingProfile.ThumbRightRight, vKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press thumb right up
                bool thumbRightUp = controllerInput.ThumbRightY > vControllerOffsetMedium;
                PressKeypadKey(thumbRightUp, vKeypadDownStatus.ThumbRightUp, vKeypadMappingProfile.ThumbRightUpMod, vKeypadMappingProfile.ThumbRightUp, vKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press thumb right down
                bool thumbRightDown = controllerInput.ThumbRightY < -vControllerOffsetMedium;
                PressKeypadKey(thumbRightDown, vKeypadDownStatus.ThumbRightDown, vKeypadMappingProfile.ThumbRightDownMod, vKeypadMappingProfile.ThumbRightDown, vKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button a key
                PressKeypadKey(controllerInput.ButtonA.PressedRaw, vKeypadDownStatus.ButtonA, vKeypadMappingProfile.ButtonAMod, vKeypadMappingProfile.ButtonA, vKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button b key
                PressKeypadKey(controllerInput.ButtonB.PressedRaw, vKeypadDownStatus.ButtonB, vKeypadMappingProfile.ButtonBMod, vKeypadMappingProfile.ButtonB, vKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button x key
                PressKeypadKey(controllerInput.ButtonX.PressedRaw, vKeypadDownStatus.ButtonX, vKeypadMappingProfile.ButtonXMod, vKeypadMappingProfile.ButtonX, vKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button y key
                PressKeypadKey(controllerInput.ButtonY.PressedRaw, vKeypadDownStatus.ButtonY, vKeypadMappingProfile.ButtonYMod, vKeypadMappingProfile.ButtonY, vKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button back key
                PressKeypadKey(controllerInput.ButtonBack.PressedRaw, vKeypadDownStatus.ButtonBack, vKeypadMappingProfile.ButtonBackMod, vKeypadMappingProfile.ButtonBack, vKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button start key
                PressKeypadKey(controllerInput.ButtonStart.PressedRaw, vKeypadDownStatus.ButtonStart, vKeypadMappingProfile.ButtonStartMod, vKeypadMappingProfile.ButtonStart, vKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button shoulder left key
                PressKeypadKey(controllerInput.ButtonShoulderLeft.PressedRaw, vKeypadDownStatus.ButtonShoulderLeft, vKeypadMappingProfile.ButtonShoulderLeftMod, vKeypadMappingProfile.ButtonShoulderLeft, vKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button trigger left key
                PressKeypadKey(controllerInput.ButtonTriggerLeft.PressedRaw, vKeypadDownStatus.ButtonTriggerLeft, vKeypadMappingProfile.ButtonTriggerLeftMod, vKeypadMappingProfile.ButtonTriggerLeft, vKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button thumb left key
                PressKeypadKey(controllerInput.ButtonThumbLeft.PressedRaw, vKeypadDownStatus.ButtonThumbLeft, vKeypadMappingProfile.ButtonThumbLeftMod, vKeypadMappingProfile.ButtonThumbLeft, vKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button shoulder right key
                PressKeypadKey(controllerInput.ButtonShoulderRight.PressedRaw, vKeypadDownStatus.ButtonShoulderRight, vKeypadMappingProfile.ButtonShoulderRightMod, vKeypadMappingProfile.ButtonShoulderRight, vKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button trigger right key
                PressKeypadKey(controllerInput.ButtonTriggerRight.PressedRaw, vKeypadDownStatus.ButtonTriggerRight, vKeypadMappingProfile.ButtonTriggerRightMod, vKeypadMappingProfile.ButtonTriggerRight, vKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button thumb right key
                PressKeypadKey(controllerInput.ButtonThumbRight.PressedRaw, vKeypadDownStatus.ButtonThumbRight, vKeypadMappingProfile.ButtonThumbRightMod, vKeypadMappingProfile.ButtonThumbRight, vKeypadMappingProfile.ButtonRepeatIntervalMs);
            }
            catch { }
        }

        //Press keyboard key binded to keypad
        void PressKeypadKey(bool buttonPressed, KeypadDownStatus keypadDownStatus, KeysVirtual? keyModifier, KeysVirtual? key, int buttonRepeatIntervalMs)
        {
            try
            {
                if (buttonPressed)
                {
                    if (!keypadDownStatus.Pressed || (buttonRepeatIntervalMs > 0 && Environment.TickCount >= keypadDownStatus.LastPress + buttonRepeatIntervalMs))
                    {
                        keypadDownStatus.Pressed = true;
                        keypadDownStatus.LastPress = Environment.TickCount;
                        if (keyModifier != null)
                        {
                            KeyToggleCombo((byte)keyModifier, (byte)key, false, true);
                        }
                        else
                        {
                            KeyToggleSingle((byte)key, false, true);
                        }
                    }
                }
                else if (keypadDownStatus.Pressed)
                {
                    keypadDownStatus.Pressed = false;
                    if (keyModifier != null)
                    {
                        KeyToggleCombo((byte)keyModifier, (byte)key, false, false);
                    }
                    else
                    {
                        KeyToggleSingle((byte)key, false, false);
                    }
                }
            }
            catch { }
        }
    }
}