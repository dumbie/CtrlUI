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

                        //D-Pad
                        if (vKeypadDownStatus.DPadLeft || vKeypadDownStatus.ThumbLeftLeft) { textblock_LeftLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_LeftLeft.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.DPadUp || vKeypadDownStatus.ThumbLeftUp) { textblock_LeftUp.Foreground = targetSolidColorBrushAccent; } else { textblock_LeftUp.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.DPadRight || vKeypadDownStatus.ThumbLeftRight) { textblock_LeftRight.Foreground = targetSolidColorBrushAccent; } else { textblock_LeftRight.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.DPadDown || vKeypadDownStatus.ThumbLeftDown) { textblock_LeftDown.Foreground = targetSolidColorBrushAccent; } else { textblock_LeftDown.Foreground = targetSolidColorBrushWhite; }

                        //Buttons
                        if (vKeypadDownStatus.ButtonA) { textblock_ButtonA.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonA.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.ButtonB) { textblock_ButtonB.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonB.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.ButtonX) { textblock_ButtonX.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonX.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.ButtonY) { textblock_ButtonY.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonY.Foreground = targetSolidColorBrushWhite; }

                        if (vKeypadDownStatus.ButtonBack) { textblock_ButtonBack.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonBack.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.ButtonStart) { textblock_ButtonStart.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonStart.Foreground = targetSolidColorBrushWhite; }

                        if (vKeypadDownStatus.ButtonShoulderLeft) { textblock_ButtonShoulderLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonShoulderLeft.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.ButtonTriggerLeft) { textblock_ButtonTriggerLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonTriggerLeft.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.ButtonThumbLeft) { textblock_ButtonThumbLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonThumbLeft.Foreground = targetSolidColorBrushWhite; }

                        if (vKeypadDownStatus.ButtonShoulderRight) { textblock_ButtonShoulderRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonShoulderRight.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.ButtonTriggerRight) { textblock_ButtonTriggerRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonTriggerRight.Foreground = targetSolidColorBrushWhite; }
                        if (vKeypadDownStatus.ButtonThumbRight) { textblock_ButtonThumbRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonThumbRight.Foreground = targetSolidColorBrushWhite; }
                    }
                    catch { }
                });
            }
            catch { }
        }
    }
}