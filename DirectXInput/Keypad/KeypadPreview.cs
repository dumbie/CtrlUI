using ArnoldVinkCode;
using System.Windows;
using System.Windows.Media;
using static DirectXInput.AppVariables;

namespace DirectXInput.Keypad
{
    public partial class WindowKeypad
    {
        //Update interface keypad preview
        void UpdateKeypadPreview()
        {
            try
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

                        //Thumb Left / DPad
                        if (vKeypadDownStatus.DPadLeft.Pressed || vKeypadDownStatus.ThumbLeftLeft.Pressed) { textblock_ThumbLeftLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbLeftLeft.Foreground = targetSolidColorBrushDarkLight; }
                        if (vKeypadDownStatus.DPadUp.Pressed || vKeypadDownStatus.ThumbLeftUp.Pressed) { textblock_ThumbLeftUp.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbLeftUp.Foreground = targetSolidColorBrushDarkLight; }
                        if (vKeypadDownStatus.DPadRight.Pressed || vKeypadDownStatus.ThumbLeftRight.Pressed) { textblock_ThumbLeftRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbLeftRight.Foreground = targetSolidColorBrushDarkLight; }
                        if (vKeypadDownStatus.DPadDown.Pressed || vKeypadDownStatus.ThumbLeftDown.Pressed) { textblock_ThumbLeftDown.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbLeftDown.Foreground = targetSolidColorBrushDarkLight; }

                        //Thumb Right
                        if (vKeypadDownStatus.ThumbRightLeft.Pressed) { textblock_ThumbRightLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbRightLeft.Foreground = targetSolidColorBrushDarkLight; }
                        if (vKeypadDownStatus.ThumbRightUp.Pressed) { textblock_ThumbRightUp.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbRightUp.Foreground = targetSolidColorBrushDarkLight; }
                        if (vKeypadDownStatus.ThumbRightRight.Pressed) { textblock_ThumbRightRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbRightRight.Foreground = targetSolidColorBrushDarkLight; }
                        if (vKeypadDownStatus.ThumbRightDown.Pressed) { textblock_ThumbRightDown.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbRightDown.Foreground = targetSolidColorBrushDarkLight; }

                        //Buttons
                        if (vKeypadDownStatus.ButtonA.Pressed) { textblock_ButtonA.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonA.Foreground = targetSolidColorBrushDarkLight; }
                        if (vKeypadDownStatus.ButtonB.Pressed) { textblock_ButtonB.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonB.Foreground = targetSolidColorBrushDarkLight; }
                        if (vKeypadDownStatus.ButtonX.Pressed) { textblock_ButtonX.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonX.Foreground = targetSolidColorBrushDarkLight; }
                        if (vKeypadDownStatus.ButtonY.Pressed) { textblock_ButtonY.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonY.Foreground = targetSolidColorBrushDarkLight; }

                        if (vKeypadDownStatus.ButtonBack.Pressed) { textblock_ButtonBack.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonBack.Foreground = targetSolidColorBrushDarkLight; }
                        if (vKeypadDownStatus.ButtonStart.Pressed) { textblock_ButtonStart.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonStart.Foreground = targetSolidColorBrushDarkLight; }

                        if (vKeypadDownStatus.ButtonShoulderLeft.Pressed) { textblock_ButtonShoulderLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonShoulderLeft.Foreground = targetSolidColorBrushDarkLight; }
                        if (vKeypadDownStatus.ButtonTriggerLeft.Pressed) { textblock_ButtonTriggerLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonTriggerLeft.Foreground = targetSolidColorBrushDarkLight; }
                        if (vKeypadDownStatus.ButtonThumbLeft.Pressed) { textblock_ButtonThumbLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonThumbLeft.Foreground = targetSolidColorBrushDarkLight; }

                        if (vKeypadDownStatus.ButtonShoulderRight.Pressed) { textblock_ButtonShoulderRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonShoulderRight.Foreground = targetSolidColorBrushDarkLight; }
                        if (vKeypadDownStatus.ButtonTriggerRight.Pressed) { textblock_ButtonTriggerRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonTriggerRight.Foreground = targetSolidColorBrushDarkLight; }
                        if (vKeypadDownStatus.ButtonThumbRight.Pressed) { textblock_ButtonThumbRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonThumbRight.Foreground = targetSolidColorBrushDarkLight; }
                    }
                    catch { }
                });
            }
            catch { }
        }
    }
}