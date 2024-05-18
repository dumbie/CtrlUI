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
        private async Task ControllerInputSend(ControllerStatus controller)
        {
            try
            {
                //Read data from the controller
                if (controller.Details.Type == ControllerType.HidDevice)
                {
                    if (!controller.HidDevice.ReadBytesFile(controller.ControllerDataInput))
                    {
                        Debug.WriteLine("Failed to read input data from hid controller: " + controller.NumberId);
                        AVHighResDelay.Delay(0.1F);
                        return;
                    }
                }
                else
                {
                    if (!controller.WinUsbDevice.ReadBytesIntPipe(controller.ControllerDataInput))
                    {
                        Debug.WriteLine("Failed to read input data from win controller: " + controller.NumberId);
                        AVHighResDelay.Delay(0.1F);
                        return;
                    }
                }

                //Validate controller input data
                if (!InputValidateData(controller))
                {
                    Debug.WriteLine("Invalid input data read from controller: " + controller.NumberId);
                    AVHighResDelay.Delay(0.1F);
                    return;
                }

                //Update read status
                controller.ControllerDataRead = true;

                //Update Thumbsticks
                InputUpdateThumbsticks(controller);

                //Update DPad
                InputUpdateDirectionalPad(controller);

                //Update Buttons
                InputUpdateButtons(controller);

                //Update Triggers
                InputUpdateTriggers(controller);

                //Update Touchpad
                InputUpdateTouchpad(controller);

                //Update Gyroscope
                InputUpdateGyroscope(controller);

                //Update Accelerometer
                InputUpdateAccelerometer(controller);

                //Save controller button mapping
                if (ControllerMappingSave(controller))
                {
                    AVHighResDelay.Delay(0.1F);
                    return;
                }

                //Lookup controller button mapping
                ControllerMappingLookup(controller);

                //Fake Guide or Media button press with LB and Back
                if (controller.Details.Profile.FakeGuideButton && controller.InputCurrent.ButtonShoulderLeft.PressedRaw && controller.InputCurrent.ButtonBack.PressedRaw)
                {
                    controller.InputCurrent.ButtonShoulderLeft.PressedRaw = false;
                    controller.InputCurrent.ButtonBack.PressedRaw = false;
                    controller.InputCurrent.ButtonGuide.PressedRaw = true;
                }
                else if (controller.Details.Profile.FakeOneButton && controller.InputCurrent.ButtonShoulderLeft.PressedRaw && controller.InputCurrent.ButtonBack.PressedRaw)
                {
                    controller.InputCurrent.ButtonShoulderLeft.PressedRaw = false;
                    controller.InputCurrent.ButtonBack.PressedRaw = false;
                    controller.InputCurrent.ButtonTwo.PressedRaw = true;
                }

                //Fake Touchpad button press with RB and Back
                if (controller.Details.Profile.FakeTwoButton && controller.InputCurrent.ButtonShoulderRight.PressedRaw && controller.InputCurrent.ButtonBack.PressedRaw)
                {
                    controller.InputCurrent.ButtonShoulderRight.PressedRaw = false;
                    controller.InputCurrent.ButtonBack.PressedRaw = false;
                    controller.InputCurrent.ButtonOne.PressedRaw = true;
                }

                //Check if alt tab is active and buttons need to be blocked
                if (vAltTabDownStatus)
                {
                    controller.InputCurrent.ButtonStart.PressedRaw = false;
                    controller.InputCurrent.ButtonShoulderLeft.PressedRaw = false;
                }

                //Check controller button press times
                CheckControllerButtonPressTimes(controller);

                //Check if controller output needs to be blocked
                bool blockOutput = await CheckControllerBlockInteraction(controller);

                //Check if guide button is exclusive and needs to be blocked
                if (controller.InputCurrent.ButtonGuide.PressedRaw && SettingLoad(vConfigurationDirectXInput, "ExclusiveGuide", typeof(bool)))
                {
                    controller.InputCurrent.ButtonGuide.PressedRaw = false;
                }

                //Update controller input time
                long ticksSystem = GetSystemTicksMs();
                controller.TicksInputPrev = controller.TicksInputLast;
                controller.TicksInputLast = ticksSystem;

                //Check if controller is idle and update active time
                if (!CheckControllerIdlePress(controller))
                {
                    controller.TicksActiveLast = ticksSystem;
                }

                //Update and prepare virtual input data
                if (blockOutput)
                {
                    PrepareVirtualInputDataEmpty(controller);
                }
                else
                {
                    PrepareVirtualInputDataCurrent(controller);
                }

                //Send input to virtual device
                vVirtualBusDevice.VirtualReadWrite(ref controller);

                //Update controller rumble status
                if (controller.VirtualDataOutput[1] == 0x08)
                {
                    controller.RumbleCurrentHeavy = controller.VirtualDataOutput[3];
                    controller.RumbleCurrentLight = controller.VirtualDataOutput[4];
                }
            }
            catch
            {
                Debug.WriteLine("DirectInput " + controller.Details.Type + " data report is out of range or empty, skipping.");
            }
        }
    }
}