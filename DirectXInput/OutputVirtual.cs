using System.Diagnostics;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVActions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryUsb.WinUsbDevice;

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

                //Set receive structure
                Controller.XOutputData = new XUSB_OUTPUT_REPORT();
                Controller.XOutputData.Size = Marshal.SizeOf(Controller.XOutputData);
                Controller.XOutputData.SerialNo = Controller.NumberId + 1;

                //Receive output from the virtual bus
                while (TaskCheckLoop(Controller.OutputVirtualTask) && Controller.Connected())
                {
                    vVirtualBusDevice.VirtualOutput(ref Controller);
                }
            }
            catch { }
        }
    }
}