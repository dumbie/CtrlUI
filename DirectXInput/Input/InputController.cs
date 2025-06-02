using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
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
                //Check if controller is connected
                if (!controller.Connected())
                {
                    controller.ReadFailureCount++;
                    Debug.WriteLine("Read input controller is not connected: " + controller.NumberId);
                    AVHighResDelay.Delay(0.1F);
                    return;
                }

                //Read data from the controller
                if (controller.Details.Type == ControllerType.HidDevice)
                {
                    if (!controller.HidDevice.ReadBytesFile(controller.ControllerDataInput))
                    {
                        controller.ReadFailureCount++;
                        Debug.WriteLine("Failed to read input data from hid controller: " + controller.NumberId);
                        AVHighResDelay.Delay(0.1F);
                        return;
                    }
                }
                else
                {
                    if (!controller.WinUsbDevice.ReadBytesIntPipe(controller.ControllerDataInput))
                    {
                        controller.ReadFailureCount++;
                        Debug.WriteLine("Failed to read input data from win controller: " + controller.NumberId);
                        AVHighResDelay.Delay(0.1F);
                        return;
                    }
                }

                //Validate controller input data
                if (!InputValidateData(controller))
                {
                    controller.ReadFailureCount++;
                    Debug.WriteLine("Invalid input data read from controller: " + controller.NumberId);
                    AVHighResDelay.Delay(0.1F);
                    return;
                }

                //Update read status
                controller.ControllerDataRead = true;

                //Update read failure count
                controller.ReadFailureCount = 0;

                //Update Identifiers
                InputUpdateIdentifiers(controller);

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

                //Check controller button press times
                CheckControllerButtonPressTimes(controller);

                //Check if controller output needs to be blocked
                bool blockOutput = await CheckControllerBlockInteraction(controller);

                //Update controller input time
                long ticksSystem = GetSystemTicksMs();
                controller.TicksInputPrev = controller.TicksInputLast;
                controller.TicksInputLast = ticksSystem;

                //Check if controller is idle and update active time
                if (controller.BatteryCurrent.BatteryStatus == BatteryStatus.Charging || !CheckControllerIdlePress(controller))
                {
                    controller.TicksActiveLast = ticksSystem;
                }

                if (blockOutput)
                {
                    //Update and prepare empty input data
                    PrepareVirtualInputDataEmpty(controller);
                }
                else
                {
                    //Check and overwrite controller button presses
                    CheckControllerButtonOverwrite(controller);

                    //Update and prepare virtual input data
                    PrepareVirtualInputDataCurrent(controller);
                }

                //Send input to virtual device
                vVirtualBusDevice.VirtualInput(ref controller);
            }
            catch
            {
                Debug.WriteLine("DirectInput " + controller.Details.Type + " data report is out of range or empty, skipping.");
            }
        }
    }
}