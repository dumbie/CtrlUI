using LibraryUsb;
using System;
using System.Diagnostics;
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
                bool virtualBusDriver = CheckDevicesStore("ViGEmBus.inf");
                bool hidGuardianDriver = CheckDevicesStore("HidGuardian.inf");
                bool ds3ControllerDriver = CheckDevicesStore("Ds3Controller.inf");
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