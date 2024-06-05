using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Loop controller input
        async Task LoopInputController(ControllerStatus controller)
        {
            try
            {
                Debug.WriteLine("Handle controller input data " + controller.Details.Type + " for: " + controller.Details.DisplayName);

                //Receive input from the selected controller
                while (TaskCheckLoop(controller.InputControllerTask) && controller.Connected())
                {
                    try
                    {
                        await ControllerInputSend(controller);
                    }
                    catch { }
                }
            }
            catch { }
        }

        //Loop controller output
        void LoopOutputController(ControllerStatus controller)
        {
            try
            {
                Debug.WriteLine("Handle controller output data for: " + controller.Details.DisplayName);

                while (TaskCheckLoop(controller.OutputControllerTask) && controller.Connected())
                {
                    try
                    {
                        //Check if output values have changed
                        bool ledRChanged = controller.ColorLedCurrentR == controller.ColorLedPreviousR;
                        bool ledGChanged = controller.ColorLedCurrentG == controller.ColorLedPreviousG;
                        bool ledBChanged = controller.ColorLedCurrentB == controller.ColorLedPreviousB;
                        bool ledMuteChanged = vControllerMuteLedCurrent == vControllerMuteLedPrevious;
                        bool heavyRumbleChanged = controller.RumbleCurrentHeavy == controller.RumblePreviousHeavy;
                        bool lightRumbleChanged = controller.RumbleCurrentLight == controller.RumblePreviousLight;
                        if ((ledRChanged && ledGChanged && ledBChanged && ledMuteChanged && heavyRumbleChanged && lightRumbleChanged) == false)
                        {
                            //Update the previous output values
                            controller.ColorLedPreviousR = controller.ColorLedCurrentR;
                            controller.ColorLedPreviousG = controller.ColorLedCurrentG;
                            controller.ColorLedPreviousB = controller.ColorLedCurrentB;
                            vControllerMuteLedPrevious = vControllerMuteLedCurrent;
                            controller.RumblePreviousHeavy = controller.RumbleCurrentHeavy;
                            controller.RumblePreviousLight = controller.RumbleCurrentLight;

                            //Send received output to controller
                            ControllerOutputSend(controller);
                        }
                    }
                    catch { }
                    finally
                    {
                        //Delay task to prevent high cpu usage
                        AVHighResDelay.Delay(0.1F);
                    }
                }
            }
            catch { }
        }

        //Loop virtual output
        void LoopOutputVirtual(ControllerStatus controller)
        {
            try
            {
                Debug.WriteLine("Handle virtual output data for: " + controller.Details.DisplayName);

                while (TaskCheckLoop(controller.OutputVirtualTask) && controller.Connected())
                {
                    try
                    {
                        //Read output from virtual device
                        if (vVirtualBusDevice.VirtualOutput(ref controller))
                        {
                            controller.RumbleCurrentHeavy = controller.VirtualDataOutput[8];
                            controller.RumbleCurrentLight = controller.VirtualDataOutput[9];
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        //Loop gyroscope output
        async Task LoopOutputGyro(ControllerStatus controller)
        {
            try
            {
                Debug.WriteLine("Handle controller gyroscope data for: " + controller.Details.DisplayName);

                //Send gyro motion to dsu client
                while (TaskCheckLoop(controller.OutputGyroscopeTask) && controller.Connected())
                {
                    try
                    {
                        await SendGyroMotionController(controller);
                    }
                    catch { }
                    finally
                    {
                        //Delay task to prevent high cpu usage
                        AVHighResDelay.Delay(0.1F);
                    }
                }
            }
            catch { }
        }
    }
}