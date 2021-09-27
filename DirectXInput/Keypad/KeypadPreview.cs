using ArnoldVinkCode;
using System.Windows;
using System.Windows.Media;
using static ArnoldVinkCode.AVActions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

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
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        try
                        {
                            SolidColorBrush targetSolidColorBrushDarkLight = null;
                            SolidColorBrush targetSolidColorBrushAccent = (SolidColorBrush)Application.Current.Resources["ApplicationAccentLightBrush"];
                            if (vKeypadMappingProfile.KeypadDisplayStyle == 0)
                            {
                                targetSolidColorBrushDarkLight = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFFFF");
                            }
                            else
                            {
                                targetSolidColorBrushDarkLight = (SolidColorBrush)new BrushConverter().ConvertFrom("#000000");
                            }

                            //DPad
                            if (controllerInput.DPadLeft.PressedRaw) { textblock_DPadLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_DPadLeft.Foreground = targetSolidColorBrushDarkLight; }
                            if (controllerInput.DPadUp.PressedRaw) { textblock_DPadUp.Foreground = targetSolidColorBrushAccent; } else { textblock_DPadUp.Foreground = targetSolidColorBrushDarkLight; }
                            if (controllerInput.DPadRight.PressedRaw) { textblock_DPadRight.Foreground = targetSolidColorBrushAccent; } else { textblock_DPadRight.Foreground = targetSolidColorBrushDarkLight; }
                            if (controllerInput.DPadDown.PressedRaw) { textblock_DPadDown.Foreground = targetSolidColorBrushAccent; } else { textblock_DPadDown.Foreground = targetSolidColorBrushDarkLight; }

                            //Thumb Left
                            if (controllerInput.ThumbLeftX < -vControllerOffsetMedium) { textblock_ThumbLeftLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbLeftLeft.Foreground = targetSolidColorBrushDarkLight; }
                            if (controllerInput.ThumbLeftY > vControllerOffsetMedium) { textblock_ThumbLeftUp.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbLeftUp.Foreground = targetSolidColorBrushDarkLight; }
                            if (controllerInput.ThumbLeftX > vControllerOffsetMedium) { textblock_ThumbLeftRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbLeftRight.Foreground = targetSolidColorBrushDarkLight; }
                            if (controllerInput.ThumbLeftY < -vControllerOffsetMedium) { textblock_ThumbLeftDown.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbLeftDown.Foreground = targetSolidColorBrushDarkLight; }

                            //Thumb Right
                            if (controllerInput.ThumbRightX < -vControllerOffsetMedium) { textblock_ThumbRightLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbRightLeft.Foreground = targetSolidColorBrushDarkLight; }
                            if (controllerInput.ThumbRightY > vControllerOffsetMedium) { textblock_ThumbRightUp.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbRightUp.Foreground = targetSolidColorBrushDarkLight; }
                            if (controllerInput.ThumbRightX > vControllerOffsetMedium) { textblock_ThumbRightRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbRightRight.Foreground = targetSolidColorBrushDarkLight; }
                            if (controllerInput.ThumbRightY < -vControllerOffsetMedium) { textblock_ThumbRightDown.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbRightDown.Foreground = targetSolidColorBrushDarkLight; }

                            //Buttons
                            if (controllerInput.ButtonA.PressedRaw) { textblock_ButtonA.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonA.Foreground = targetSolidColorBrushDarkLight; }
                            if (controllerInput.ButtonB.PressedRaw) { textblock_ButtonB.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonB.Foreground = targetSolidColorBrushDarkLight; }
                            if (controllerInput.ButtonX.PressedRaw) { textblock_ButtonX.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonX.Foreground = targetSolidColorBrushDarkLight; }
                            if (controllerInput.ButtonY.PressedRaw) { textblock_ButtonY.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonY.Foreground = targetSolidColorBrushDarkLight; }
                            if (controllerInput.ButtonBack.PressedRaw) { textblock_ButtonBack.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonBack.Foreground = targetSolidColorBrushDarkLight; }
                            if (controllerInput.ButtonStart.PressedRaw) { textblock_ButtonStart.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonStart.Foreground = targetSolidColorBrushDarkLight; }

                            //Shoulder
                            if (controllerInput.ButtonShoulderLeft.PressedRaw) { textblock_ButtonShoulderLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonShoulderLeft.Foreground = targetSolidColorBrushDarkLight; }
                            if (controllerInput.ButtonShoulderRight.PressedRaw) { textblock_ButtonShoulderRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonShoulderRight.Foreground = targetSolidColorBrushDarkLight; }

                            //Trigger
                            if (controllerInput.ButtonTriggerLeft.PressedRaw) { textblock_ButtonTriggerLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonTriggerLeft.Foreground = targetSolidColorBrushDarkLight; }
                            if (controllerInput.ButtonTriggerRight.PressedRaw) { textblock_ButtonTriggerRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonTriggerRight.Foreground = targetSolidColorBrushDarkLight; }

                            //Thumb
                            if (controllerInput.ButtonThumbLeft.PressedRaw) { textblock_ThumbLeftButton.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbLeftButton.Foreground = targetSolidColorBrushDarkLight; }
                            if (controllerInput.ButtonThumbRight.PressedRaw) { textblock_ThumbRightButton.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbRightButton.Foreground = targetSolidColorBrushDarkLight; }
                        }
                        catch { }
                    });

                    vControllerDelay_KeypadPreview = GetSystemTicksMs() + vControllerDelayMicroTicks;
                }
            }
            catch { }
        }
    }
}