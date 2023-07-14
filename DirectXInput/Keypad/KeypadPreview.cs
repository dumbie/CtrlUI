using ArnoldVinkCode;
using static ArnoldVinkCode.AVActions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.ControllerTimings;

namespace DirectXInput.KeypadCode
{
    public partial class WindowKeypad
    {
        //Update keypad preview
        public void ControllerInteractionKeypadPreview(ControllerInput controllerInput)
        {
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_KeypadPreview)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        try
                        {
                            //DPad
                            if (controllerInput.DPadLeft.PressedRaw) { textblock_DPadLeft.Foreground = vApplicationAccentLightBrush; } else { textblock_DPadLeft.Foreground = vKeypadNormalBrush; }
                            if (controllerInput.DPadUp.PressedRaw) { textblock_DPadUp.Foreground = vApplicationAccentLightBrush; } else { textblock_DPadUp.Foreground = vKeypadNormalBrush; }
                            if (controllerInput.DPadRight.PressedRaw) { textblock_DPadRight.Foreground = vApplicationAccentLightBrush; } else { textblock_DPadRight.Foreground = vKeypadNormalBrush; }
                            if (controllerInput.DPadDown.PressedRaw) { textblock_DPadDown.Foreground = vApplicationAccentLightBrush; } else { textblock_DPadDown.Foreground = vKeypadNormalBrush; }

                            //Thumb Left
                            if (controllerInput.ButtonThumbLeftLeft.PressedRaw) { textblock_ThumbLeftLeft.Foreground = vApplicationAccentLightBrush; } else { textblock_ThumbLeftLeft.Foreground = vKeypadNormalBrush; }
                            if (controllerInput.ButtonThumbLeftUp.PressedRaw) { textblock_ThumbLeftUp.Foreground = vApplicationAccentLightBrush; } else { textblock_ThumbLeftUp.Foreground = vKeypadNormalBrush; }
                            if (controllerInput.ButtonThumbLeftRight.PressedRaw) { textblock_ThumbLeftRight.Foreground = vApplicationAccentLightBrush; } else { textblock_ThumbLeftRight.Foreground = vKeypadNormalBrush; }
                            if (controllerInput.ButtonThumbLeftDown.PressedRaw) { textblock_ThumbLeftDown.Foreground = vApplicationAccentLightBrush; } else { textblock_ThumbLeftDown.Foreground = vKeypadNormalBrush; }

                            //Thumb Right
                            if (controllerInput.ButtonThumbRightLeft.PressedRaw) { textblock_ThumbRightLeft.Foreground = vApplicationAccentLightBrush; } else { textblock_ThumbRightLeft.Foreground = vKeypadNormalBrush; }
                            if (controllerInput.ButtonThumbRightUp.PressedRaw) { textblock_ThumbRightUp.Foreground = vApplicationAccentLightBrush; } else { textblock_ThumbRightUp.Foreground = vKeypadNormalBrush; }
                            if (controllerInput.ButtonThumbRightRight.PressedRaw) { textblock_ThumbRightRight.Foreground = vApplicationAccentLightBrush; } else { textblock_ThumbRightRight.Foreground = vKeypadNormalBrush; }
                            if (controllerInput.ButtonThumbRightDown.PressedRaw) { textblock_ThumbRightDown.Foreground = vApplicationAccentLightBrush; } else { textblock_ThumbRightDown.Foreground = vKeypadNormalBrush; }

                            //Buttons
                            if (controllerInput.ButtonA.PressedRaw) { textblock_ButtonA.Foreground = vApplicationAccentLightBrush; } else { textblock_ButtonA.Foreground = vKeypadNormalBrush; }
                            if (controllerInput.ButtonB.PressedRaw) { textblock_ButtonB.Foreground = vApplicationAccentLightBrush; } else { textblock_ButtonB.Foreground = vKeypadNormalBrush; }
                            if (controllerInput.ButtonX.PressedRaw) { textblock_ButtonX.Foreground = vApplicationAccentLightBrush; } else { textblock_ButtonX.Foreground = vKeypadNormalBrush; }
                            if (controllerInput.ButtonY.PressedRaw) { textblock_ButtonY.Foreground = vApplicationAccentLightBrush; } else { textblock_ButtonY.Foreground = vKeypadNormalBrush; }
                            if (controllerInput.ButtonBack.PressedRaw) { textblock_ButtonBack.Foreground = vApplicationAccentLightBrush; } else { textblock_ButtonBack.Foreground = vKeypadNormalBrush; }
                            if (controllerInput.ButtonStart.PressedRaw) { textblock_ButtonStart.Foreground = vApplicationAccentLightBrush; } else { textblock_ButtonStart.Foreground = vKeypadNormalBrush; }

                            //Shoulder
                            if (controllerInput.ButtonShoulderLeft.PressedRaw) { textblock_ButtonShoulderLeft.Foreground = vApplicationAccentLightBrush; } else { textblock_ButtonShoulderLeft.Foreground = vKeypadNormalBrush; }
                            if (controllerInput.ButtonShoulderRight.PressedRaw) { textblock_ButtonShoulderRight.Foreground = vApplicationAccentLightBrush; } else { textblock_ButtonShoulderRight.Foreground = vKeypadNormalBrush; }

                            //Trigger
                            if (controllerInput.ButtonTriggerLeft.PressedRaw) { textblock_ButtonTriggerLeft.Foreground = vApplicationAccentLightBrush; } else { textblock_ButtonTriggerLeft.Foreground = vKeypadNormalBrush; }
                            if (controllerInput.ButtonTriggerRight.PressedRaw) { textblock_ButtonTriggerRight.Foreground = vApplicationAccentLightBrush; } else { textblock_ButtonTriggerRight.Foreground = vKeypadNormalBrush; }

                            //Thumb
                            if (controllerInput.ButtonThumbLeft.PressedRaw) { textblock_ThumbLeftButton.Foreground = vApplicationAccentLightBrush; } else { textblock_ThumbLeftButton.Foreground = vKeypadNormalBrush; }
                            if (controllerInput.ButtonThumbRight.PressedRaw) { textblock_ThumbRightButton.Foreground = vApplicationAccentLightBrush; } else { textblock_ThumbRightButton.Foreground = vKeypadNormalBrush; }
                        }
                        catch { }
                    });

                    vControllerDelay_KeypadPreview = GetSystemTicksMs() + vControllerDelayTicks125;
                }
            }
            catch { }
        }
    }
}