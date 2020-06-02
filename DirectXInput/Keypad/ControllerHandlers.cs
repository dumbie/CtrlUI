using System;
using System.Linq;
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

                //Fix some applications dont handle pressdown as repeat / add 30ms repeating key presses

                //Get keypad mapping profile
                KeypadMapping directKeypadMappingProfile = vDirectKeypadMapping.Where(x => x.Name == "Default").FirstOrDefault();

                //Press arrow left key
                if (controllerInput.DPadLeft.PressedRaw)
                {
                    if (!vKeypadDownStatus.DPadLeft)
                    {
                        vKeypadDownStatus.DPadLeft = true;
                        if (directKeypadMappingProfile.DPadLeftMod != null)
                        {
                            KeyToggleCombo((byte)directKeypadMappingProfile.DPadLeftMod, (byte)directKeypadMappingProfile.DPadLeft, false, vKeypadDownStatus.DPadLeft);
                        }
                        else
                        {
                            KeyToggleSingle((byte)directKeypadMappingProfile.DPadLeft, false, vKeypadDownStatus.DPadLeft);
                        }
                    }
                }
                else if (vKeypadDownStatus.DPadLeft)
                {
                    vKeypadDownStatus.DPadLeft = false;
                    if (directKeypadMappingProfile.DPadLeftMod != null)
                    {
                        KeyToggleCombo((byte)directKeypadMappingProfile.DPadLeftMod, (byte)directKeypadMappingProfile.DPadLeft, false, vKeypadDownStatus.DPadLeft);
                    }
                    else
                    {
                        KeyToggleSingle((byte)directKeypadMappingProfile.DPadLeft, false, vKeypadDownStatus.DPadLeft);
                    }
                }

                //Press arrow right key
                if (controllerInput.DPadRight.PressedRaw)
                {
                    if (!vKeypadDownStatus.DPadRight)
                    {
                        vKeypadDownStatus.DPadRight = true;
                        if (directKeypadMappingProfile.DPadRightMod != null)
                        {
                            KeyToggleCombo((byte)directKeypadMappingProfile.DPadRightMod, (byte)directKeypadMappingProfile.DPadRight, false, vKeypadDownStatus.DPadRight);
                        }
                        else
                        {
                            KeyToggleSingle((byte)directKeypadMappingProfile.DPadRight, false, vKeypadDownStatus.DPadRight);
                        }
                    }
                }
                else if (vKeypadDownStatus.DPadRight)
                {
                    vKeypadDownStatus.DPadRight = false;
                    if (directKeypadMappingProfile.DPadRightMod != null)
                    {
                        KeyToggleCombo((byte)directKeypadMappingProfile.DPadRightMod, (byte)directKeypadMappingProfile.DPadRight, false, vKeypadDownStatus.DPadRight);
                    }
                    else
                    {
                        KeyToggleSingle((byte)directKeypadMappingProfile.DPadRight, false, vKeypadDownStatus.DPadRight);
                    }
                }

                //Press arrow up key
                if (controllerInput.DPadUp.PressedRaw)
                {
                    if (!vKeypadDownStatus.DPadUp)
                    {
                        vKeypadDownStatus.DPadUp = true;
                        if (directKeypadMappingProfile.DPadUpMod != null)
                        {
                            KeyToggleCombo((byte)directKeypadMappingProfile.DPadUpMod, (byte)directKeypadMappingProfile.DPadUp, false, vKeypadDownStatus.DPadUp);
                        }
                        else
                        {
                            KeyToggleSingle((byte)directKeypadMappingProfile.DPadUp, false, vKeypadDownStatus.DPadUp);
                        }
                    }
                }
                else if (vKeypadDownStatus.DPadUp)
                {
                    vKeypadDownStatus.DPadUp = false;
                    if (directKeypadMappingProfile.DPadUpMod != null)
                    {
                        KeyToggleCombo((byte)directKeypadMappingProfile.DPadUpMod, (byte)directKeypadMappingProfile.DPadUp, false, vKeypadDownStatus.DPadUp);
                    }
                    else
                    {
                        KeyToggleSingle((byte)directKeypadMappingProfile.DPadUp, false, vKeypadDownStatus.DPadUp);
                    }
                }

                //Press arrow down key
                if (controllerInput.DPadDown.PressedRaw)
                {
                    if (!vKeypadDownStatus.DPadDown)
                    {
                        vKeypadDownStatus.DPadDown = true;
                        if (directKeypadMappingProfile.DPadDownMod != null)
                        {
                            KeyToggleCombo((byte)directKeypadMappingProfile.DPadDownMod, (byte)directKeypadMappingProfile.DPadDown, false, vKeypadDownStatus.DPadDown);
                        }
                        else
                        {
                            KeyToggleSingle((byte)directKeypadMappingProfile.DPadDown, false, vKeypadDownStatus.DPadDown);
                        }
                    }
                }
                else if (vKeypadDownStatus.DPadDown)
                {
                    vKeypadDownStatus.DPadDown = false;
                    if (directKeypadMappingProfile.DPadDownMod != null)
                    {
                        KeyToggleCombo((byte)directKeypadMappingProfile.DPadDownMod, (byte)directKeypadMappingProfile.DPadDown, false, vKeypadDownStatus.DPadDown);
                    }
                    else
                    {
                        KeyToggleSingle((byte)directKeypadMappingProfile.DPadDown, false, vKeypadDownStatus.DPadDown);
                    }
                }

                //Press button a key
                if (controllerInput.ButtonA.PressedRaw)
                {
                    if (!vKeypadDownStatus.ButtonA)
                    {
                        vKeypadDownStatus.ButtonA = true;
                        if (directKeypadMappingProfile.ButtonAMod != null)
                        {
                            KeyToggleCombo((byte)directKeypadMappingProfile.ButtonAMod, (byte)directKeypadMappingProfile.ButtonA, false, vKeypadDownStatus.ButtonA);
                        }
                        else
                        {
                            KeyToggleSingle((byte)directKeypadMappingProfile.ButtonA, false, vKeypadDownStatus.ButtonA);
                        }
                    }
                }
                else if (vKeypadDownStatus.ButtonA)
                {
                    vKeypadDownStatus.ButtonA = false;
                    if (directKeypadMappingProfile.ButtonAMod != null)
                    {
                        KeyToggleCombo((byte)directKeypadMappingProfile.ButtonAMod, (byte)directKeypadMappingProfile.ButtonA, false, vKeypadDownStatus.ButtonA);
                    }
                    else
                    {
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonA, false, vKeypadDownStatus.ButtonA);
                    }
                }

                //Press button b key
                if (controllerInput.ButtonB.PressedRaw)
                {
                    if (!vKeypadDownStatus.ButtonB)
                    {
                        vKeypadDownStatus.ButtonB = true;
                        if (directKeypadMappingProfile.ButtonBMod != null)
                        {
                            KeyToggleCombo((byte)directKeypadMappingProfile.ButtonBMod, (byte)directKeypadMappingProfile.ButtonB, false, vKeypadDownStatus.ButtonB);
                        }
                        else
                        {
                            KeyToggleSingle((byte)directKeypadMappingProfile.ButtonB, false, vKeypadDownStatus.ButtonB);
                        }
                    }
                }
                else if (vKeypadDownStatus.ButtonB)
                {
                    vKeypadDownStatus.ButtonB = false;
                    if (directKeypadMappingProfile.ButtonBMod != null)
                    {
                        KeyToggleCombo((byte)directKeypadMappingProfile.ButtonBMod, (byte)directKeypadMappingProfile.ButtonB, false, vKeypadDownStatus.ButtonB);
                    }
                    else
                    {
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonB, false, vKeypadDownStatus.ButtonB);
                    }
                }

                //Press button y key
                if (controllerInput.ButtonY.PressedRaw)
                {
                    if (!vKeypadDownStatus.ButtonY)
                    {
                        vKeypadDownStatus.ButtonY = true;
                        if (directKeypadMappingProfile.ButtonYMod != null)
                        {
                            KeyToggleCombo((byte)directKeypadMappingProfile.ButtonYMod, (byte)directKeypadMappingProfile.ButtonY, false, vKeypadDownStatus.ButtonY);
                        }
                        else
                        {
                            KeyToggleSingle((byte)directKeypadMappingProfile.ButtonY, false, vKeypadDownStatus.ButtonY);
                        }
                    }
                }
                else if (vKeypadDownStatus.ButtonY)
                {
                    vKeypadDownStatus.ButtonY = false;
                    if (directKeypadMappingProfile.ButtonYMod != null)
                    {
                        KeyToggleCombo((byte)directKeypadMappingProfile.ButtonYMod, (byte)directKeypadMappingProfile.ButtonY, false, vKeypadDownStatus.ButtonY);
                    }
                    else
                    {
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonY, false, vKeypadDownStatus.ButtonY);
                    }
                }

                //Press button x key
                if (controllerInput.ButtonX.PressedRaw)
                {
                    if (!vKeypadDownStatus.ButtonX)
                    {
                        vKeypadDownStatus.ButtonX = true;
                        if (directKeypadMappingProfile.ButtonXMod != null)
                        {
                            KeyToggleCombo((byte)directKeypadMappingProfile.ButtonXMod, (byte)directKeypadMappingProfile.ButtonX, false, vKeypadDownStatus.ButtonX);
                        }
                        else
                        {
                            KeyToggleSingle((byte)directKeypadMappingProfile.ButtonX, false, vKeypadDownStatus.ButtonX);
                        }
                    }
                }
                else if (vKeypadDownStatus.ButtonX)
                {
                    vKeypadDownStatus.ButtonX = false;
                    if (directKeypadMappingProfile.ButtonXMod != null)
                    {
                        KeyToggleCombo((byte)directKeypadMappingProfile.ButtonXMod, (byte)directKeypadMappingProfile.ButtonX, false, vKeypadDownStatus.ButtonX);
                    }
                    else
                    {
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonX, false, vKeypadDownStatus.ButtonX);
                    }
                }

                //Press button back key
                if (controllerInput.ButtonBack.PressedRaw)
                {
                    if (!vKeypadDownStatus.ButtonBack)
                    {
                        vKeypadDownStatus.ButtonBack = true;
                        if (directKeypadMappingProfile.ButtonBackMod != null)
                        {
                            KeyToggleCombo((byte)directKeypadMappingProfile.ButtonBackMod, (byte)directKeypadMappingProfile.ButtonBack, false, vKeypadDownStatus.ButtonBack);
                        }
                        else
                        {
                            KeyToggleSingle((byte)directKeypadMappingProfile.ButtonBack, false, vKeypadDownStatus.ButtonBack);
                        }
                    }
                }
                else if (vKeypadDownStatus.ButtonBack)
                {
                    vKeypadDownStatus.ButtonBack = false;
                    if (directKeypadMappingProfile.ButtonBackMod != null)
                    {
                        KeyToggleCombo((byte)directKeypadMappingProfile.ButtonBackMod, (byte)directKeypadMappingProfile.ButtonBack, false, vKeypadDownStatus.ButtonBack);
                    }
                    else
                    {
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonBack, false, vKeypadDownStatus.ButtonBack);
                    }
                }

                //Press button start key
                if (controllerInput.ButtonStart.PressedRaw)
                {
                    if (!vKeypadDownStatus.ButtonStart)
                    {
                        vKeypadDownStatus.ButtonStart = true;
                        if (directKeypadMappingProfile.ButtonStartMod != null)
                        {
                            KeyToggleCombo((byte)directKeypadMappingProfile.ButtonStartMod, (byte)directKeypadMappingProfile.ButtonStart, false, vKeypadDownStatus.ButtonStart);
                        }
                        else
                        {
                            KeyToggleSingle((byte)directKeypadMappingProfile.ButtonStart, false, vKeypadDownStatus.ButtonStart);
                        }
                    }
                }
                else if (vKeypadDownStatus.ButtonStart)
                {
                    vKeypadDownStatus.ButtonStart = false;
                    if (directKeypadMappingProfile.ButtonStartMod != null)
                    {
                        KeyToggleCombo((byte)directKeypadMappingProfile.ButtonStartMod, (byte)directKeypadMappingProfile.ButtonStart, false, vKeypadDownStatus.ButtonStart);
                    }
                    else
                    {
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonStart, false, vKeypadDownStatus.ButtonStart);
                    }
                }

                //Press button shoulder left key
                if (controllerInput.ButtonShoulderLeft.PressedRaw)
                {
                    if (!vKeypadDownStatus.ButtonShoulderLeft)
                    {
                        vKeypadDownStatus.ButtonShoulderLeft = true;
                        if (directKeypadMappingProfile.ButtonShoulderLeftMod != null)
                        {
                            KeyToggleCombo((byte)directKeypadMappingProfile.ButtonShoulderLeftMod, (byte)directKeypadMappingProfile.ButtonShoulderLeft, false, vKeypadDownStatus.ButtonShoulderLeft);
                        }
                        else
                        {
                            KeyToggleSingle((byte)directKeypadMappingProfile.ButtonShoulderLeft, false, vKeypadDownStatus.ButtonShoulderLeft);
                        }
                    }
                }
                else if (vKeypadDownStatus.ButtonShoulderLeft)
                {
                    vKeypadDownStatus.ButtonShoulderLeft = false;
                    if (directKeypadMappingProfile.ButtonShoulderLeftMod != null)
                    {
                        KeyToggleCombo((byte)directKeypadMappingProfile.ButtonShoulderLeftMod, (byte)directKeypadMappingProfile.ButtonShoulderLeft, false, vKeypadDownStatus.ButtonShoulderLeft);
                    }
                    else
                    {
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonShoulderLeft, false, vKeypadDownStatus.ButtonShoulderLeft);
                    }
                }

                //Press button shoulder right key
                if (controllerInput.ButtonShoulderRight.PressedRaw)
                {
                    if (!vKeypadDownStatus.ButtonShoulderRight)
                    {
                        vKeypadDownStatus.ButtonShoulderRight = true;
                        if (directKeypadMappingProfile.ButtonShoulderRightMod != null)
                        {
                            KeyToggleCombo((byte)directKeypadMappingProfile.ButtonShoulderRightMod, (byte)directKeypadMappingProfile.ButtonShoulderRight, false, vKeypadDownStatus.ButtonShoulderRight);
                        }
                        else
                        {
                            KeyToggleSingle((byte)directKeypadMappingProfile.ButtonShoulderRight, false, vKeypadDownStatus.ButtonShoulderRight);
                        }
                    }
                }
                else if (vKeypadDownStatus.ButtonShoulderRight)
                {
                    vKeypadDownStatus.ButtonShoulderRight = false;
                    if (directKeypadMappingProfile.ButtonShoulderRightMod != null)
                    {
                        KeyToggleCombo((byte)directKeypadMappingProfile.ButtonShoulderRightMod, (byte)directKeypadMappingProfile.ButtonShoulderRight, false, vKeypadDownStatus.ButtonShoulderRight);
                    }
                    else
                    {
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonShoulderRight, false, vKeypadDownStatus.ButtonShoulderRight);
                    }
                }
            }
            catch { }
        }
    }
}