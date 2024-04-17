using System.Diagnostics;
using static ArnoldVinkCode.AVActions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Receive rumble
        void LoopOutputVirtual(ControllerStatus Controller)
        {
            try
            {
                Debug.WriteLine("Receive rumble for: " + Controller.Details.DisplayName);

                //Receive output from the virtual bus
                while (TaskCheckLoop(Controller.OutputVirtualTask) && Controller.Connected())
                {
                    try
                    {
                        vVirtualBusDevice.VirtualOutput(ref Controller);
                        Controller.RumbleCurrentHeavy = Controller.VirtualDataOutput[8];
                        Controller.RumbleCurrentLight = Controller.VirtualDataOutput[9];
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}