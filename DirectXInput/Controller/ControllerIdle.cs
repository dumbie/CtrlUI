using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVSettings;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

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
                    if (Controller.Details.Wireless)
                    {
                        long lastMs = GetSystemTicksMs() - Controller.TicksActiveLast;
                        int targetTimeMs = SettingLoad(vConfigurationDirectXInput, "ControllerIdleDisconnectMin", typeof(int)) * 60000;
                        //Debug.WriteLine("Controller " + Controller.NumberId + " idle check: " + lastMs + "/" + targetTimeMs + "ms.");
                        if (targetTimeMs > 0 && lastMs > targetTimeMs)
                        {
                            await StopController(Controller, "idle", "Disconnected idle controller " + Controller.NumberDisplay());
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
                if (controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.DPadLeft].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.DPadUp].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.DPadRight].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.DPadDown].PressedRaw)
                {
                    return false;
                }

                //Buttons
                if (controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.A].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.B].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.X].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.Y].PressedRaw)
                {
                    return false;
                }

                if (controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.Back].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.Start].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.Guide].PressedRaw)
                {
                    return false;
                }

                if (controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.One].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.Two].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.Three].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.Four].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.Five].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.Six].PressedRaw)
                {
                    return false;
                }

                if (controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.ShoulderLeft].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.ShoulderRight].PressedRaw)
                {
                    return false;
                }

                if (controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.ThumbLeft].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.ThumbRight].PressedRaw)
                {
                    return false;
                }

                //Triggers
                if (controllerStatus.InputCurrent.TriggerLeft >= 50 || controllerStatus.InputCurrent.TriggerRight >= 50)
                {
                    return false;
                }

                //Left stick movement
                if (controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.ThumbLeftLeft].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.ThumbLeftUp].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.ThumbLeftRight].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.ThumbLeftDown].PressedRaw)
                {
                    return false;
                }

                //Right stick movement
                if (controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.ThumbRightLeft].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.ThumbRightUp].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.ThumbRightRight].PressedRaw || controllerStatus.InputCurrent.Buttons[(byte)ControllerButtons.ThumbRightDown].PressedRaw)
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