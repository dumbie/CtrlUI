using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Settings;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Send the prepared controller data
        async Task SendControllerData(ControllerStatus Controller)
        {
            try
            {
                //Update and check button press times
                UpdateCheckButtonPressTimes(Controller.InputCurrent.ButtonGuide);

                //Check if the controller is currently idle
                if (Controller.Details.Wireless)
                {
                    if (CheckControllerIdle(Controller))
                    {
                        int idleTimeMs = Environment.TickCount - Controller.LastActiveTicks;
                        int targetTimeMs = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "ControllerIdleDisconnectMin")) * 60000;
                        if (targetTimeMs > 0 && idleTimeMs > targetTimeMs)
                        {
                            Debug.WriteLine("Controller " + Controller.NumberId + " is idle for: " + idleTimeMs + "/" + targetTimeMs + "ms");
                            Controller.LastActiveTicks = Environment.TickCount;
                            StopControllerTask(Controller, false, "idle");
                            return;
                        }
                    }
                    else
                    {
                        Controller.LastActiveTicks = Environment.TickCount;
                    }
                }

                //Update interface controller preview
                UpdateControllerPreview(Controller);

                //Check if controller shortcut is pressed
                bool blockOutputShortcut = await ControllerShortcut(Controller);

                //Check if controller output needs to be forwarded
                bool blockOutputApplication = await ControllerOutput(Controller);

                //Check if output or guide button needs to be blocked
                if (blockOutputApplication || blockOutputShortcut || Controller.BlockOutput)
                {
                    //Prepare empty XOutput device data
                    PrepareXInputData(Controller, true);
                }
                else
                {
                    //Check if guide button is CtrlUI exclusive
                    if (Controller.InputCurrent.ButtonGuide.PressedRaw && Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ExclusiveGuide")))
                    {
                        Controller.InputCurrent.ButtonGuide.PressedRaw = false;
                    }

                    //Prepare current XOutput device data
                    PrepareXInputData(Controller, false);
                }

                //Send XOutput device data
                vVirtualBusDevice.VirtualReadWrite(Controller.XInputData, Controller.XOutputData);

                //Send XInput device data
                SendXRumbleData(Controller, false, false, false);
            }
            catch { }
        }
    }
}