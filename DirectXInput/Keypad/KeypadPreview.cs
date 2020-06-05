using ArnoldVinkCode;
using System.Windows;
using System.Windows.Media;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput.Keypad
{
    public partial class WindowKeypad
    {
        //Update interface keypad preview
        void UpdateKeypadPreview(ControllerInput controllerInput)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    try
                    {
                        SolidColorBrush targetSolidColorBrushWhite = (SolidColorBrush)new BrushConverter().ConvertFrom("#F1F1F1");
                        SolidColorBrush targetSolidColorBrushAccent = (SolidColorBrush)Application.Current.Resources["ApplicationAccentLightBrush"];

                        //Thumb Left / DPad
                        if (vKeypadDownStatus.DPadLeft.Pressed || vKeypadDownStatus.ThumbLeftLeft.Pressed) { textblock_ThumbLeftLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbLeftLeft.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.DPadUp.Pressed || vKeypadDownStatus.ThumbLeftUp.Pressed) { textblock_ThumbLeftUp.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbLeftUp.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.DPadRight.Pressed || vKeypadDownStatus.ThumbLeftRight.Pressed) { textblock_ThumbLeftRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbLeftRight.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.DPadDown.Pressed || vKeypadDownStatus.ThumbLeftDown.Pressed) { textblock_ThumbLeftDown.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbLeftDown.Foreground = targetSolidColorBrushWhite; }

                        //Thumb Right
                        if (vKeypadDownStatus.ThumbRightLeft.Pressed) { textblock_ThumbRightLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbRightLeft.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.ThumbRightUp.Pressed) { textblock_ThumbRightUp.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbRightUp.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.ThumbRightRight.Pressed) { textblock_ThumbRightRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbRightRight.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.ThumbRightDown.Pressed) { textblock_ThumbRightDown.Foreground = targetSolidColorBrushAccent; } else { textblock_ThumbRightDown.Foreground = targetSolidColorBrushWhite; }

                        //Buttons
                        if (vKeypadDownStatus.ButtonA.Pressed) { textblock_ButtonA.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonA.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.ButtonB.Pressed) { textblock_ButtonB.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonB.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.ButtonX.Pressed) { textblock_ButtonX.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonX.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.ButtonY.Pressed) { textblock_ButtonY.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonY.Foreground = targetSolidColorBrushWhite; }

                        if (vKeypadDownStatus.ButtonBack.Pressed) { textblock_ButtonBack.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonBack.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.ButtonStart.Pressed) { textblock_ButtonStart.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonStart.Foreground = targetSolidColorBrushWhite; }

                        if (vKeypadDownStatus.ButtonShoulderLeft.Pressed) { textblock_ButtonShoulderLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonShoulderLeft.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.ButtonTriggerLeft.Pressed) { textblock_ButtonTriggerLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonTriggerLeft.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.ButtonThumbLeft.Pressed) { textblock_ButtonThumbLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonThumbLeft.Foreground = targetSolidColorBrushWhite; }

                        if (vKeypadDownStatus.ButtonShoulderRight.Pressed) { textblock_ButtonShoulderRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonShoulderRight.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.ButtonTriggerRight.Pressed) { textblock_ButtonTriggerRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonTriggerRight.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.ButtonThumbRight.Pressed) { textblock_ButtonThumbRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonThumbRight.Foreground = targetSolidColorBrushWhite; }
                    }
                    catch { }
                });
            }
            catch { }
        }
    }
}