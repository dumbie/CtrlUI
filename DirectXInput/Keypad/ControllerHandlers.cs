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

                //Press arrow left key
                bool thumbLeftLeft = controllerInput.ThumbLeftX < -vControllerOffsetMedium;
                PressKeypadKey(controllerInput.DPadLeft.PressedRaw, ref vKeypadDownStatus.DPadLeft, directKeypadMappingProfile.DPadLeftMod, directKeypadMappingProfile.DPadLeft);
                PressKeypadKey(thumbLeftLeft, ref vKeypadDownStatus.ThumbLeftLeft, directKeypadMappingProfile.ThumbLeftLeftMod, directKeypadMappingProfile.ThumbLeftLeft);

                //Press arrow right key
                bool thumbLeftRight = controllerInput.ThumbLeftX > vControllerOffsetMedium;
                PressKeypadKey(controllerInput.DPadRight.PressedRaw, ref vKeypadDownStatus.DPadRight, directKeypadMappingProfile.DPadRightMod, directKeypadMappingProfile.DPadRight);
                PressKeypadKey(thumbLeftRight, ref vKeypadDownStatus.ThumbLeftRight, directKeypadMappingProfile.ThumbLeftRightMod, directKeypadMappingProfile.ThumbLeftRight);

                //Press arrow up key
                bool thumbLeftUp = controllerInput.ThumbLeftY > vControllerOffsetMedium;
                PressKeypadKey(controllerInput.DPadUp.PressedRaw, ref vKeypadDownStatus.DPadUp, directKeypadMappingProfile.DPadUpMod, directKeypadMappingProfile.DPadUp);
                PressKeypadKey(thumbLeftUp, ref vKeypadDownStatus.ThumbLeftUp, directKeypadMappingProfile.ThumbLeftUpMod, directKeypadMappingProfile.ThumbLeftUp);

                //Press arrow down key
                bool thumbLeftDown = controllerInput.ThumbLeftY < -vControllerOffsetMedium;
                PressKeypadKey(controllerInput.DPadDown.PressedRaw, ref vKeypadDownStatus.DPadDown, directKeypadMappingProfile.DPadDownMod, directKeypadMappingProfile.DPadDown);
                PressKeypadKey(thumbLeftDown, ref vKeypadDownStatus.ThumbLeftDown, directKeypadMappingProfile.ThumbLeftDownMod, directKeypadMappingProfile.ThumbLeftDown);

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