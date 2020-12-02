using System.Runtime.InteropServices;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryUsb.WinUsbDevice;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Receive output from the virtual bus
        void VirtualBusOutput(ControllerStatus Controller)
        {
            try
            {
                //Set receive structure
                Controller.XOutputData = new XUSB_OUTPUT_REPORT();
                Controller.XOutputData.Size = Marshal.SizeOf(Controller.XOutputData);
                Controller.XOutputData.SerialNo = Controller.NumberId + 1;

                //Receive output from the virtual bus driver
                vVirtualBusDevice.VirtualOutput(ref Controller.XOutputData);

                //Send output to the controller
                ControllerOutput(Controller, false, false, false);
            }
            catch { }
        }
    }
}