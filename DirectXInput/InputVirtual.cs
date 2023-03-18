using System;
using System.Diagnostics;
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
                        int targetTimeMs = SettingLoad(vConfigurationDirectXInput, "ControllerIdleDisconnectMin", typeof(int)) * 60000;
                        if (targetTimeMs > 0 && idleTimeMs > targetTimeMs)
                        {
                            Debug.WriteLine("Controller " + Controller.NumberId + " is idle for: " + idleTimeMs + "/" + targetTimeMs + "ms");
                            Controller.LastActiveTicks = currentTimeMs;
                            await StopController(Controller, "idle", "Disconnected idle controller " + Controller.NumberId + ".");
                            return;
                        }
                    }
                    else
                    {
                        Controller.LastActiveTicks = currentTimeMs;
                    }
                }

                //Check if controller output needs to be blocked
                if (vAppActivated && !vAppMinimized && vShowDebugInformation)
                {
                    //Send empty input to the virtual device
                    SendInputVirtualEmpty(Controller);
                    return;
                }
                else if (Controller.Disconnecting)
                {
                    //Send empty input to the virtual device
                    SendInputVirtualEmpty(Controller);
                    return;
                }

                //Ignore controller timeout
                Controller.TimeoutIgnore = true;

                //Check if controller shortcut is pressed
                bool blockOutputShortcut = await ControllerShortcut(Controller);

                //Check if controller output needs to be forwarded
                bool blockOutputApplication = await ControllerOutputApps(Controller);

                //Allow controller timeout 
                Controller.TimeoutIgnore = false;

                //Check if controller output needs to be blocked
                if (blockOutputShortcut || blockOutputApplication)
                {
                    //Send empty input to the virtual device
                    SendInputVirtualEmpty(Controller);
                    return;
                }

                //Check if guide button needs to be blocked
                if (Controller.InputCurrent.ButtonGuide.PressedRaw && SettingLoad(vConfigurationDirectXInput, "ExclusiveGuide", typeof(bool)))
                {
                    Controller.InputCurrent.ButtonGuide.PressedRaw = false;
                }

                //Prepare current xinput data
                PrepareXInputDataCurrent(Controller);

                //Send input to the virtual bus
                vVirtualBusDevice.VirtualInput(ref Controller);
            }
            catch { }
        }
    }
}