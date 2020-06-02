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
        public bool ControllerInteractionKeyboard(ControllerInput controllerInput)
        {
            bool ControllerUsed = false;
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
                    if (!vKeyboardStatus.Contains(KeysVirtual.Left))
                    {
                        vKeyboardStatus.Add(KeysVirtual.Left);
                        KeyToggleSingle((byte)directKeypadMappingProfile.DPadLeft, false, true);

                        ControllerUsed = true;
                    }
                }
                else
                {
                    if (vKeyboardStatus.Contains(KeysVirtual.Left))
                    {
                        vKeyboardStatus.RemoveAll(x => x == KeysVirtual.Left);
                        KeyToggleSingle((byte)directKeypadMappingProfile.DPadLeft, false, false);

                        ControllerUsed = true;
                    }
                }

                //Press arrow right key
                if (controllerInput.DPadRight.PressedRaw)
                {
                    if (!vKeyboardStatus.Contains(KeysVirtual.Right))
                    {
                        vKeyboardStatus.Add(KeysVirtual.Right);
                        KeyToggleSingle((byte)directKeypadMappingProfile.DPadRight, false, true);

                        ControllerUsed = true;
                    }
                }
                else
                {
                    if (vKeyboardStatus.Contains(KeysVirtual.Right))
                    {
                        vKeyboardStatus.RemoveAll(x => x == KeysVirtual.Right);
                        KeyToggleSingle((byte)directKeypadMappingProfile.DPadRight, false, false);

                        ControllerUsed = true;
                    }
                }

                //Press arrow up key
                if (controllerInput.DPadUp.PressedRaw)
                {
                    if (!vKeyboardStatus.Contains(KeysVirtual.Up))
                    {
                        vKeyboardStatus.Add(KeysVirtual.Up);
                        KeyToggleSingle((byte)directKeypadMappingProfile.DPadUp, false, true);

                        ControllerUsed = true;
                    }
                }
                else
                {
                    if (vKeyboardStatus.Contains(KeysVirtual.Up))
                    {
                        vKeyboardStatus.RemoveAll(x => x == KeysVirtual.Up);
                        KeyToggleSingle((byte)directKeypadMappingProfile.DPadUp, false, false);

                        ControllerUsed = true;
                    }
                }

                //Press arrow down key
                if (controllerInput.DPadDown.PressedRaw)
                {
                    if (!vKeyboardStatus.Contains(KeysVirtual.Down))
                    {
                        vKeyboardStatus.Add(KeysVirtual.Down);
                        KeyToggleSingle((byte)directKeypadMappingProfile.DPadDown, false, true);

                        ControllerUsed = true;
                    }
                }
                else
                {
                    if (vKeyboardStatus.Contains(KeysVirtual.Down))
                    {
                        vKeyboardStatus.RemoveAll(x => x == KeysVirtual.Down);
                        KeyToggleSingle((byte)directKeypadMappingProfile.DPadDown, false, false);

                        ControllerUsed = true;
                    }
                }

                //Press button a key
                if (controllerInput.ButtonA.PressedRaw)
                {
                    if (!vKeyboardStatus.Contains(KeysVirtual.Control))
                    {
                        vKeyboardStatus.Add(KeysVirtual.Control);
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonA, false, true);

                        ControllerUsed = true;
                    }
                }
                else
                {
                    if (vKeyboardStatus.Contains(KeysVirtual.Control))
                    {
                        vKeyboardStatus.RemoveAll(x => x == KeysVirtual.Control);
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonA, false, false);

                        ControllerUsed = true;
                    }
                }

                //Press button b key
                if (controllerInput.ButtonB.PressedRaw)
                {
                    if (!vKeyboardStatus.Contains(KeysVirtual.Alt))
                    {
                        vKeyboardStatus.Add(KeysVirtual.Alt);
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonB, false, true);

                        ControllerUsed = true;
                    }
                }
                else
                {
                    if (vKeyboardStatus.Contains(KeysVirtual.Alt))
                    {
                        vKeyboardStatus.RemoveAll(x => x == KeysVirtual.Alt);
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonB, false, false);

                        ControllerUsed = true;
                    }
                }

                //Press button y key
                if (controllerInput.ButtonY.PressedRaw)
                {
                    KeysVirtual comboKey = KeysVirtual.Control | KeysVirtual.Alt;
                    if (!vKeyboardStatus.Contains(comboKey))
                    {
                        vKeyboardStatus.Add(comboKey);
                        KeyToggleCombo((byte)KeysVirtual.Control, (byte)KeysVirtual.Alt, false, true);
                        //Fix add support for combo key presses

                        ControllerUsed = true;
                    }
                }
                else
                {
                    KeysVirtual comboKey = KeysVirtual.Control | KeysVirtual.Alt;
                    if (vKeyboardStatus.Contains(comboKey))
                    {
                        vKeyboardStatus.RemoveAll(x => x == comboKey);
                        KeyToggleCombo((byte)KeysVirtual.Control, (byte)KeysVirtual.Alt, false, false);
                        //Fix add support for combo key presses

                        ControllerUsed = true;
                    }
                }

                //Press button x key
                if (controllerInput.ButtonX.PressedRaw)
                {
                    if (!vKeyboardStatus.Contains(KeysVirtual.Space))
                    {
                        vKeyboardStatus.Add(KeysVirtual.Space);
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonX, false, true);

                        ControllerUsed = true;
                    }
                }
                else
                {
                    if (vKeyboardStatus.Contains(KeysVirtual.Space))
                    {
                        vKeyboardStatus.RemoveAll(x => x == KeysVirtual.Space);
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonX, false, false);

                        ControllerUsed = true;
                    }
                }

                //Press button back key
                if (controllerInput.ButtonBack.PressedRaw)
                {
                    if (!vKeyboardStatus.Contains(KeysVirtual.Escape))
                    {
                        vKeyboardStatus.Add(KeysVirtual.Escape);
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonBack, false, true);

                        ControllerUsed = true;
                    }
                }
                else
                {
                    if (vKeyboardStatus.Contains(KeysVirtual.Escape))
                    {
                        vKeyboardStatus.RemoveAll(x => x == KeysVirtual.Escape);
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonBack, false, false);

                        ControllerUsed = true;
                    }
                }

                //Press button start key
                if (controllerInput.ButtonStart.PressedRaw)
                {
                    if (!vKeyboardStatus.Contains(KeysVirtual.Enter))
                    {
                        vKeyboardStatus.Add(KeysVirtual.Enter);
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonStart, false, true);

                        ControllerUsed = true;
                    }
                }
                else
                {
                    if (vKeyboardStatus.Contains(KeysVirtual.Enter))
                    {
                        vKeyboardStatus.RemoveAll(x => x == KeysVirtual.Enter);
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonStart, false, false);

                        ControllerUsed = true;
                    }
                }

                //Press button shoulder left key
                if (controllerInput.ButtonShoulderLeft.PressedRaw)
                {
                    if (!vKeyboardStatus.Contains(KeysVirtual.Shift))
                    {
                        vKeyboardStatus.Add(KeysVirtual.Shift);
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonShoulderLeft, false, true);

                        ControllerUsed = true;
                    }
                }
                else
                {
                    if (vKeyboardStatus.Contains(KeysVirtual.Shift))
                    {
                        vKeyboardStatus.RemoveAll(x => x == KeysVirtual.Shift);
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonShoulderLeft, false, false);

                        ControllerUsed = true;
                    }
                }

                //Press button shoulder right key
                if (controllerInput.ButtonShoulderRight.PressedRaw)
                {
                    if (!vKeyboardStatus.Contains(KeysVirtual.Y))
                    {
                        vKeyboardStatus.Add(KeysVirtual.Y);
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonShoulderRight, false, true);

                        ControllerUsed = true;
                    }
                }
                else
                {
                    if (vKeyboardStatus.Contains(KeysVirtual.Y))
                    {
                        vKeyboardStatus.RemoveAll(x => x == KeysVirtual.Y);
                        KeyToggleSingle((byte)directKeypadMappingProfile.ButtonShoulderRight, false, false);

                        ControllerUsed = true;
                    }
                }
            }
            catch { }
            return ControllerUsed;
        }
    }
}