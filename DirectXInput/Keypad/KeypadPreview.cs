using ArnoldVinkCode;
using System.Windows;
using System.Windows.Media;
using static LibraryShared.Classes;

namespace DirectXInput.Keypad
{
    public partial class WindowKeypad
    {
        //Update interface controller preview
        void ControllerPreview(ControllerInput controllerInput)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    try
                    {
                        SolidColorBrush targetSolidColorBrushWhite = new BrushConverter().ConvertFrom("#F1F1F1") as SolidColorBrush;
                        SolidColorBrush targetSolidColorBrushAccent = (SolidColorBrush)Application.Current.Resources["ApplicationAccentLightBrush"];

                        //D-Pad
                        if (controllerInput.DPadLeft.PressedRaw) { textblock_ArrowLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ArrowLeft.Foreground = targetSolidColorBrushWhite; }
                        if (controllerInput.DPadUp.PressedRaw) { textblock_ArrowUp.Foreground = targetSolidColorBrushAccent; } else { textblock_ArrowUp.Foreground = targetSolidColorBrushWhite; }
                        if (controllerInput.DPadRight.PressedRaw) { textblock_ArrowRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ArrowRight.Foreground = targetSolidColorBrushWhite; }
                        if (controllerInput.DPadDown.PressedRaw) { textblock_ArrowDown.Foreground = targetSolidColorBrushAccent; } else { textblock_ArrowDown.Foreground = targetSolidColorBrushWhite; }

                        //Buttons
                        if (controllerInput.ButtonA.PressedRaw) { textblock_ButtonA.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonA.Foreground = targetSolidColorBrushWhite; }
                        if (controllerInput.ButtonB.PressedRaw) { textblock_ButtonB.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonB.Foreground = targetSolidColorBrushWhite; }
                        if (controllerInput.ButtonX.PressedRaw) { textblock_ButtonX.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonX.Foreground = targetSolidColorBrushWhite; }
                        if (controllerInput.ButtonY.PressedRaw) { textblock_ButtonY.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonY.Foreground = targetSolidColorBrushWhite; }

                        if (controllerInput.ButtonBack.PressedRaw) { textblock_ButtonBack.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonBack.Foreground = targetSolidColorBrushWhite; }
                        if (controllerInput.ButtonStart.PressedRaw) { textblock_ButtonStart.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonStart.Foreground = targetSolidColorBrushWhite; }

                        if (controllerInput.ButtonShoulderLeft.PressedRaw) { textblock_ButtonShoulderLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonShoulderLeft.Foreground = targetSolidColorBrushWhite; }
                        if (controllerInput.ButtonTriggerLeft.PressedRaw) { textblock_ButtonTriggerLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonTriggerLeft.Foreground = targetSolidColorBrushWhite; }
                        if (controllerInput.ButtonThumbLeft.PressedRaw) { textblock_ButtonThumbLeft.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonThumbLeft.Foreground = targetSolidColorBrushWhite; }

                        if (controllerInput.ButtonShoulderRight.PressedRaw) { textblock_ButtonShoulderRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonShoulderRight.Foreground = targetSolidColorBrushWhite; }
                        if (controllerInput.ButtonTriggerRight.PressedRaw) { textblock_ButtonTriggerRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonTriggerRight.Foreground = targetSolidColorBrushWhite; }
                        if (controllerInput.ButtonThumbRight.PressedRaw) { textblock_ButtonThumbRight.Foreground = targetSolidColorBrushAccent; } else { textblock_ButtonThumbRight.Foreground = targetSolidColorBrushWhite; }
                    }
                    catch { }
                });
            }
            catch { }
        }
    }
}