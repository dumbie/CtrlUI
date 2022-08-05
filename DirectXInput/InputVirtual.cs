using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.Settings;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Send empty input to the virtual device
        void SendInputVirtualEmpty(ControllerStatus Controller)
        {
            try
            {
                //Prepare empty xinput data
                PrepareXInputDataEmpty(Controller);

                //Send input to the virtual bus
                vVirtualBusDevice.VirtualInput(ref Controller);
            }
            catch { }
        }

        //Send controller input to the virtual device
        async Task SendInputVirtualController(ControllerStatus Controller)
        {
            try
            {
                //Update and check button press times
                UpdateCheckButtonPressTimes(Controller.InputCurrent.ButtonGuide);

                //Check if the controller is currently idle
                if (Controller.Details.Wireless)
                {
                    long currentTimeMs = GetSystemTicksMs();
                    if (Controller.BatteryCurrent.BatteryStatus != BatteryStatus.Charging && CheckControllerIdle(Controller))
                    {
                        long idleTimeMs = currentTimeMs - Controller.LastActiveTicks;
                        int targetTimeMs = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "ControllerIdleDisconnectMin")) * 60000;
                        if (targetTimeMs > 0 && idleTimeMs > targetTimeMs)
                        {
                            Debug.WriteLine("Controller " + Controller.NumberId + " is idle for: " + idleTimeMs + "/" + targetTimeMs + "ms");
                            Controller.LastActiveTicks = currentTimeMs;
                            StopControllerTask(Controller, "idle", "Disconnected idle controller " + Controller.NumberId + ".");
                            return;
                        }
                    }
                    else
                    {
                        Controller.LastActiveTicks = currentTimeMs;
                    }
                }

                //Check if controller output needs to be blocked
                if (vAppActivated && !vAppMinimized && vShowDebugInformation) { return; }

                //Check if controller shortcut is pressed
                bool blockOutputShortcut = await ControllerShortcut(Controller);

                //Check if controller output needs to be forwarded
                bool blockOutputApplication = await ControllerOutputApps(Controller);

                //Check if output or guide button needs to be blocked
                if (blockOutputApplication || blockOutputShortcut || Controller.Disconnecting)
                {
                    //Prepare empty xinput data
                    PrepareXInputDataEmpty(Controller);
                }
                else
                {
                    //Check if guide button is CtrlUI exclusive
                    if (Controller.InputCurrent.ButtonGuide.PressedRaw && Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ExclusiveGuide")))
                    {
                        Controller.InputCurrent.ButtonGuide.PressedRaw = false;
                    }

                    //Prepare current xinput data
                    PrepareXInputDataCurrent(Controller);
                }

                //Send input to the virtual bus
                vVirtualBusDevice.VirtualInput(ref Controller);
            }
            catch { }
        }
    }
}