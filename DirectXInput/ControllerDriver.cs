using LibraryUsb;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static DirectXInput.AppVariables;
using static LibraryUsb.Enumerate;
using static LibraryUsb.NativeMethods_Guid;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Check if drivers are installed
        bool CheckInstalledDrivers()
        {
            try
            {
                bool virtualBusDriver = EnumerateDevicesStore("ViGEmBus.inf").Any();
                bool hidGuardianDriver = EnumerateDevicesStore("HidGuardian.inf").Any();
                bool ds3ControllerDriver = EnumerateDevicesStore("Ds3Controller.inf").Any();
                return virtualBusDriver && hidGuardianDriver && ds3ControllerDriver;
            }
            catch
            {
                Debug.WriteLine("Failed to check installed drivers.");
                return false;
            }
        }

        //Open the virtual bus driver
        async Task<bool> OpenVirtualBusDriver()
        {
            try
            {
                vVirtualBusDevice = new WinUsbDevice(GuidClassVigemVirtualBus, string.Empty, false, false);
                if (vVirtualBusDevice.Connected)
                {
                    Debug.WriteLine("Virtual bus driver is installed.");
                    vVirtualBusDevice.VirtualUnplugAll();
                    await Task.Delay(500);
                    return true;
                }
                else
                {
                    Debug.WriteLine("Virtual bus driver not installed.");
                    return false;
                }
            }
            catch
            {
                Debug.WriteLine("Failed to open virtual bus driver.");
                return false;
            }
        }
    }
}