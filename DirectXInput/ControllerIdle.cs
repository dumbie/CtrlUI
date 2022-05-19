using System;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Check if the controller is currently idle
        bool CheckControllerIdle(ControllerStatus controllerStatus)
        {
            try
            {
                //DPad
                if (controllerStatus.InputCurrent.DPadLeft.PressedRaw || controllerStatus.InputCurrent.DPadUp.PressedRaw || controllerStatus.InputCurrent.DPadRight.PressedRaw || controllerStatus.InputCurrent.DPadDown.PressedRaw)
                {
                    return false;
                }

                //Buttons
                if (controllerStatus.InputCurrent.ButtonA.PressedRaw || controllerStatus.InputCurrent.ButtonB.PressedRaw || controllerStatus.InputCurrent.ButtonX.PressedRaw || controllerStatus.InputCurrent.ButtonY.PressedRaw)
                {
                    return false;
                }

                if (controllerStatus.InputCurrent.ButtonBack.PressedRaw || controllerStatus.InputCurrent.ButtonStart.PressedRaw || controllerStatus.InputCurrent.ButtonGuide.PressedRaw)
                {
                    return false;
                }

                if (controllerStatus.InputCurrent.ButtonTouchpad.PressedRaw || controllerStatus.InputCurrent.ButtonMedia.PressedRaw)
                {
                    return false;
                }

                if (controllerStatus.InputCurrent.ButtonShoulderLeft.PressedRaw || controllerStatus.InputCurrent.ButtonShoulderRight.PressedRaw)
                {
                    return false;
                }

                if (controllerStatus.InputCurrent.ButtonThumbLeft.PressedRaw || controllerStatus.InputCurrent.ButtonThumbRight.PressedRaw)
                {
                    return false;
                }

                //Triggers
                if (!controllerStatus.Details.Profile.UseButtonTriggers)
                {
                    if (controllerStatus.InputCurrent.TriggerLeft >= 10 || controllerStatus.InputCurrent.TriggerRight >= 10)
                    {
                        return false;
                    }
                }
                else
                {
                    if (controllerStatus.InputCurrent.ButtonTriggerLeft.PressedRaw || controllerStatus.InputCurrent.ButtonTriggerRight.PressedRaw)
                    {
                        return false;
                    }
                }

                //Left stick movement
                if (Math.Abs(controllerStatus.InputCurrent.ThumbLeftY) > vControllerThumbOffset2500 || Math.Abs(controllerStatus.InputCurrent.ThumbLeftX) > vControllerThumbOffset2500)
                {
                    return false;
                }

                //Right stick movement
                if (Math.Abs(controllerStatus.InputCurrent.ThumbRightY) > vControllerThumbOffset2500 || Math.Abs(controllerStatus.InputCurrent.ThumbRightX) > vControllerThumbOffset2500)
                {
                    return false;
                }

                //Debug.WriteLine("Controller " + Controller.NumberId + " is currently idle.");
            }
            catch { }
            return true;
        }
    }
}