using System;
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
                    ControllerPreview(controllerInput);

                    vControllerDelay_KeypadPreview = Environment.TickCount + vControllerDelayNanoTicks;
                }

                //Get keypad mapping profile
                KeypadMapping directKeypadMappingProfile = vDirectKeypadMapping.Where(x => x.Name == "Default").FirstOrDefault();

                //Press arrow left key
                PressKeypadKey(controllerInput.DPadLeft.PressedRaw, ref vKeypadDownStatus.DPadLeft, directKeypadMappingProfile.DPadLeftMod, directKeypadMappingProfile.DPadLeft);

                //Press arrow right key
                PressKeypadKey(controllerInput.DPadRight.PressedRaw, ref vKeypadDownStatus.DPadRight, directKeypadMappingProfile.DPadRightMod, directKeypadMappingProfile.DPadRight);

                //Press arrow up key
                PressKeypadKey(controllerInput.DPadUp.PressedRaw, ref vKeypadDownStatus.DPadUp, directKeypadMappingProfile.DPadUpMod, directKeypadMappingProfile.DPadUp);

                //Press arrow down key
                PressKeypadKey(controllerInput.DPadDown.PressedRaw, ref vKeypadDownStatus.DPadDown, directKeypadMappingProfile.DPadDownMod, directKeypadMappingProfile.DPadDown);

                //Press button a key
                PressKeypadKey(controllerInput.ButtonA.PressedRaw, ref vKeypadDownStatus.ButtonA, directKeypadMappingProfile.ButtonAMod, directKeypadMappingProfile.ButtonA);

                //Press button b key
                PressKeypadKey(controllerInput.ButtonB.PressedRaw, ref vKeypadDownStatus.ButtonB, directKeypadMappingProfile.ButtonBMod, directKeypadMappingProfile.ButtonB);

                //Press button x key
                PressKeypadKey(controllerInput.ButtonX.PressedRaw, ref vKeypadDownStatus.ButtonX, directKeypadMappingProfile.ButtonXMod, directKeypadMappingProfile.ButtonX);

                //Press button y key
                PressKeypadKey(controllerInput.ButtonY.PressedRaw, ref vKeypadDownStatus.ButtonY, directKeypadMappingProfile.ButtonYMod, directKeypadMappingProfile.ButtonY);

                //Press button back key
                PressKeypadKey(controllerInput.ButtonBack.PressedRaw, ref vKeypadDownStatus.ButtonBack, directKeypadMappingProfile.ButtonBackMod, directKeypadMappingProfile.ButtonBack);

                //Press button start key
                PressKeypadKey(controllerInput.ButtonStart.PressedRaw, ref vKeypadDownStatus.ButtonStart, directKeypadMappingProfile.ButtonStartMod, directKeypadMappingProfile.ButtonStart);

                //Press button shoulder left key
                PressKeypadKey(controllerInput.ButtonShoulderLeft.PressedRaw, ref vKeypadDownStatus.ButtonShoulderLeft, directKeypadMappingProfile.ButtonShoulderLeftMod, directKeypadMappingProfile.ButtonShoulderLeft);

                //Press button trigger left key
                PressKeypadKey(controllerInput.ButtonTriggerLeft.PressedRaw, ref vKeypadDownStatus.ButtonTriggerLeft, directKeypadMappingProfile.ButtonTriggerLeftMod, directKeypadMappingProfile.ButtonTriggerLeft);

                //Press button thumb left key
                PressKeypadKey(controllerInput.ButtonThumbLeft.PressedRaw, ref vKeypadDownStatus.ButtonThumbLeft, directKeypadMappingProfile.ButtonThumbLeftMod, directKeypadMappingProfile.ButtonThumbLeft);

                //Press button shoulder right key
                PressKeypadKey(controllerInput.ButtonShoulderRight.PressedRaw, ref vKeypadDownStatus.ButtonShoulderRight, directKeypadMappingProfile.ButtonShoulderRightMod, directKeypadMappingProfile.ButtonShoulderRight);

                //Press button trigger right key
                PressKeypadKey(controllerInput.ButtonTriggerRight.PressedRaw, ref vKeypadDownStatus.ButtonTriggerRight, directKeypadMappingProfile.ButtonTriggerRightMod, directKeypadMappingProfile.ButtonTriggerRight);

                //Press button thumb right key
                PressKeypadKey(controllerInput.ButtonThumbRight.PressedRaw, ref vKeypadDownStatus.ButtonThumbRight, directKeypadMappingProfile.ButtonThumbRightMod, directKeypadMappingProfile.ButtonThumbRight);
            }
            catch { }
        }

        //Press keyboard key binded to keypad
        void PressKeypadKey(bool buttonPressed, ref bool keypadDownStatus, KeysVirtual? keyModifier, KeysVirtual? key)
        {
            try
            {
                //Fix some applications dont handle pressdown as repeat / add 30ms repeating key presses
                if (buttonPressed)
                {
                    if (!keypadDownStatus)
                    {
                        keypadDownStatus = true;
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
                else if (keypadDownStatus)
                {
                    keypadDownStatus = false;
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