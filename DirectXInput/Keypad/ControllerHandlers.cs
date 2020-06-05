using System;
using System.Diagnostics;
using System.Linq;
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
                //Update interface controller preview
                if (Environment.TickCount >= vControllerDelay_KeypadPreview)
                {
                    UpdateKeypadPreview(controllerInput);

                    vControllerDelay_KeypadPreview = Environment.TickCount + vControllerDelayMicroTicks;
                }

                //Get keypad mapping profile
                string processNameLower = vProcessForeground.Name.ToLower();
                KeypadMapping directKeypadMappingProfile = vDirectKeypadMapping.Where(x => x.Name.ToLower() == processNameLower).FirstOrDefault();
                if (directKeypadMappingProfile == null) { directKeypadMappingProfile = vDirectKeypadMapping.Where(x => x.Name == "Default").FirstOrDefault(); }

                //Update the key names
                if (processNameLower != vKeypadPreviousProcess)
                {
                    Debug.WriteLine("Keypad process changed to: " + processNameLower);
                    vKeypadPreviousProcess = processNameLower;
                    UpdateKeypadNames();
                }

                //Press thumb left left
                bool thumbLeftLeft = controllerInput.ThumbLeftX < -vControllerOffsetMedium;
                PressKeypadKey(controllerInput.DPadLeft.PressedRaw, vKeypadDownStatus.DPadLeft, directKeypadMappingProfile.DPadLeftMod, directKeypadMappingProfile.DPadLeft, directKeypadMappingProfile.ButtonRepeatIntervalMs);
                PressKeypadKey(thumbLeftLeft, vKeypadDownStatus.ThumbLeftLeft, directKeypadMappingProfile.ThumbLeftLeftMod, directKeypadMappingProfile.ThumbLeftLeft, directKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press thumb left right
                bool thumbLeftRight = controllerInput.ThumbLeftX > vControllerOffsetMedium;
                PressKeypadKey(controllerInput.DPadRight.PressedRaw, vKeypadDownStatus.DPadRight, directKeypadMappingProfile.DPadRightMod, directKeypadMappingProfile.DPadRight, directKeypadMappingProfile.ButtonRepeatIntervalMs);
                PressKeypadKey(thumbLeftRight, vKeypadDownStatus.ThumbLeftRight, directKeypadMappingProfile.ThumbLeftRightMod, directKeypadMappingProfile.ThumbLeftRight, directKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press thumb left up
                bool thumbLeftUp = controllerInput.ThumbLeftY > vControllerOffsetMedium;
                PressKeypadKey(controllerInput.DPadUp.PressedRaw, vKeypadDownStatus.DPadUp, directKeypadMappingProfile.DPadUpMod, directKeypadMappingProfile.DPadUp, directKeypadMappingProfile.ButtonRepeatIntervalMs);
                PressKeypadKey(thumbLeftUp, vKeypadDownStatus.ThumbLeftUp, directKeypadMappingProfile.ThumbLeftUpMod, directKeypadMappingProfile.ThumbLeftUp, directKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press thumb left down
                bool thumbLeftDown = controllerInput.ThumbLeftY < -vControllerOffsetMedium;
                PressKeypadKey(controllerInput.DPadDown.PressedRaw, vKeypadDownStatus.DPadDown, directKeypadMappingProfile.DPadDownMod, directKeypadMappingProfile.DPadDown, directKeypadMappingProfile.ButtonRepeatIntervalMs);
                PressKeypadKey(thumbLeftDown, vKeypadDownStatus.ThumbLeftDown, directKeypadMappingProfile.ThumbLeftDownMod, directKeypadMappingProfile.ThumbLeftDown, directKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press thumb right left
                bool thumbRightLeft = controllerInput.ThumbRightX < -vControllerOffsetMedium;
                PressKeypadKey(thumbRightLeft, vKeypadDownStatus.ThumbRightLeft, directKeypadMappingProfile.ThumbRightLeftMod, directKeypadMappingProfile.ThumbRightLeft, directKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press thumb right right
                bool thumbRightRight = controllerInput.ThumbRightX > vControllerOffsetMedium;
                PressKeypadKey(thumbRightRight, vKeypadDownStatus.ThumbRightRight, directKeypadMappingProfile.ThumbRightRightMod, directKeypadMappingProfile.ThumbRightRight, directKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press thumb right up
                bool thumbRightUp = controllerInput.ThumbRightY > vControllerOffsetMedium;
                PressKeypadKey(thumbRightUp, vKeypadDownStatus.ThumbRightUp, directKeypadMappingProfile.ThumbRightUpMod, directKeypadMappingProfile.ThumbRightUp, directKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press thumb right down
                bool thumbRightDown = controllerInput.ThumbRightY < -vControllerOffsetMedium;
                PressKeypadKey(thumbRightDown, vKeypadDownStatus.ThumbRightDown, directKeypadMappingProfile.ThumbRightDownMod, directKeypadMappingProfile.ThumbRightDown, directKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button a key
                PressKeypadKey(controllerInput.ButtonA.PressedRaw, vKeypadDownStatus.ButtonA, directKeypadMappingProfile.ButtonAMod, directKeypadMappingProfile.ButtonA, directKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button b key
                PressKeypadKey(controllerInput.ButtonB.PressedRaw, vKeypadDownStatus.ButtonB, directKeypadMappingProfile.ButtonBMod, directKeypadMappingProfile.ButtonB, directKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button x key
                PressKeypadKey(controllerInput.ButtonX.PressedRaw, vKeypadDownStatus.ButtonX, directKeypadMappingProfile.ButtonXMod, directKeypadMappingProfile.ButtonX, directKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button y key
                PressKeypadKey(controllerInput.ButtonY.PressedRaw, vKeypadDownStatus.ButtonY, directKeypadMappingProfile.ButtonYMod, directKeypadMappingProfile.ButtonY, directKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button back key
                PressKeypadKey(controllerInput.ButtonBack.PressedRaw, vKeypadDownStatus.ButtonBack, directKeypadMappingProfile.ButtonBackMod, directKeypadMappingProfile.ButtonBack, directKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button start key
                PressKeypadKey(controllerInput.ButtonStart.PressedRaw, vKeypadDownStatus.ButtonStart, directKeypadMappingProfile.ButtonStartMod, directKeypadMappingProfile.ButtonStart, directKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button shoulder left key
                PressKeypadKey(controllerInput.ButtonShoulderLeft.PressedRaw, vKeypadDownStatus.ButtonShoulderLeft, directKeypadMappingProfile.ButtonShoulderLeftMod, directKeypadMappingProfile.ButtonShoulderLeft, directKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button trigger left key
                PressKeypadKey(controllerInput.ButtonTriggerLeft.PressedRaw, vKeypadDownStatus.ButtonTriggerLeft, directKeypadMappingProfile.ButtonTriggerLeftMod, directKeypadMappingProfile.ButtonTriggerLeft, directKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button thumb left key
                PressKeypadKey(controllerInput.ButtonThumbLeft.PressedRaw, vKeypadDownStatus.ButtonThumbLeft, directKeypadMappingProfile.ButtonThumbLeftMod, directKeypadMappingProfile.ButtonThumbLeft, directKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button shoulder right key
                PressKeypadKey(controllerInput.ButtonShoulderRight.PressedRaw, vKeypadDownStatus.ButtonShoulderRight, directKeypadMappingProfile.ButtonShoulderRightMod, directKeypadMappingProfile.ButtonShoulderRight, directKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button trigger right key
                PressKeypadKey(controllerInput.ButtonTriggerRight.PressedRaw, vKeypadDownStatus.ButtonTriggerRight, directKeypadMappingProfile.ButtonTriggerRightMod, directKeypadMappingProfile.ButtonTriggerRight, directKeypadMappingProfile.ButtonRepeatIntervalMs);

                //Press button thumb right key
                PressKeypadKey(controllerInput.ButtonThumbRight.PressedRaw, vKeypadDownStatus.ButtonThumbRight, directKeypadMappingProfile.ButtonThumbRightMod, directKeypadMappingProfile.ButtonThumbRight, directKeypadMappingProfile.ButtonRepeatIntervalMs);
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