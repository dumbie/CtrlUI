using System;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVSettings;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

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
                //Check if controller output needs to be blocked
                if (vAppActivated && !vAppMinimized && (vShowControllerDebug || vShowControllerPreview))
                {
                    //Send empty input to the virtual device
                    SendInputVirtualEmpty(Controller);
                    return;
                }
                //Check if controller is currently disconnecting
                else if (Controller.Disconnecting)
                {
                    //Send empty input to the virtual device
                    SendInputVirtualEmpty(Controller);
                    return;
                }

                //Update and check button press times
                UpdateCheckButtonPressTimes(Controller);

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

                //Check if guide button is exclusive and needs to be blocked
                if (Controller.InputCurrent.ButtonGuide.PressedRaw && SettingLoad(vConfigurationDirectXInput, "ExclusiveGuide", typeof(bool)))
                {
                    Controller.InputCurrent.ButtonGuide.PressedRaw = false;
                }

                //Check if alt tab is active and buttons need to be blocked
                if (vAltTabDownStatus)
                {
                    Controller.InputCurrent.ButtonStart.PressedRaw = false;
                    Controller.InputCurrent.ButtonShoulderLeft.PressedRaw = false;
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