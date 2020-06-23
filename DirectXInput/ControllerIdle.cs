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
                //D-Pad
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
                if (Math.Abs(controllerStatus.InputCurrent.ThumbLeftY) > vControllerOffsetSmall || Math.Abs(controllerStatus.InputCurrent.ThumbLeftX) > vControllerOffsetSmall)
                {
                    return false;
                }

                //Right stick movement
                if (Math.Abs(controllerStatus.InputCurrent.ThumbRightY) > vControllerOffsetSmall || Math.Abs(controllerStatus.InputCurrent.ThumbRightX) > vControllerOffsetSmall)
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