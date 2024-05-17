using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVSettings;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Check for idle controllers
        async Task CheckAllControllersIdle()
        {
            try
            {
                await CheckControllerIdle(vController0);
                await CheckControllerIdle(vController1);
                await CheckControllerIdle(vController2);
                await CheckControllerIdle(vController3);
            }
            catch { }
        }

        //Check if controller is idle
        async Task<bool> CheckControllerIdle(ControllerStatus Controller)
        {
            try
            {
                if (Controller.Connected() && Controller.ControllerDataInput != null && Controller.TicksActiveLast != 0)
                {
                    if (Controller.Details.Wireless && Controller.BatteryCurrent.BatteryStatus != BatteryStatus.Charging)
                    {
                        long lastMs = GetSystemTicksMs() - Controller.TicksActiveLast;
                        int targetTimeMs = SettingLoad(vConfigurationDirectXInput, "ControllerIdleDisconnectMin", typeof(int)) * 60000;
                        //Debug.WriteLine("Controller " + Controller.NumberId + " idle check: " + lastMs + "/" + targetTimeMs + "ms.");
                        if (targetTimeMs > 0 && lastMs > targetTimeMs)
                        {
                            await StopController(Controller, "idle", "Disconnected idle controller " + Controller.NumberId);
                            return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        //Check if controller is currently pressed
        bool CheckControllerIdlePress(ControllerStatus controllerStatus)
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

                if (controllerStatus.InputCurrent.ButtonOne.PressedRaw || controllerStatus.InputCurrent.ButtonTwo.PressedRaw || controllerStatus.InputCurrent.ButtonThree.PressedRaw || controllerStatus.InputCurrent.ButtonFour.PressedRaw || controllerStatus.InputCurrent.ButtonFive.PressedRaw || controllerStatus.InputCurrent.ButtonSix.PressedRaw)
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
                if (controllerStatus.InputCurrent.TriggerLeft >= 50 || controllerStatus.InputCurrent.TriggerRight >= 50)
                {
                    return false;
                }

                //Left stick movement
                if (controllerStatus.InputCurrent.ButtonThumbLeftLeft.PressedRaw || controllerStatus.InputCurrent.ButtonThumbLeftUp.PressedRaw || controllerStatus.InputCurrent.ButtonThumbLeftRight.PressedRaw || controllerStatus.InputCurrent.ButtonThumbLeftDown.PressedRaw)
                {
                    return false;
                }

                //Right stick movement
                if (controllerStatus.InputCurrent.ButtonThumbRightLeft.PressedRaw || controllerStatus.InputCurrent.ButtonThumbRightUp.PressedRaw || controllerStatus.InputCurrent.ButtonThumbRightRight.PressedRaw || controllerStatus.InputCurrent.ButtonThumbRightDown.PressedRaw)
                {
                    return false;
                }

                //Debug.WriteLine("Controller " + controllerStatus.NumberId + " is currently idle.");
            }
            catch { }
            return true;
        }
    }
}