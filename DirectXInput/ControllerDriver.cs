using LibraryUsb;
using System;
using System.Diagnostics;
using System.IO;
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
                bool hidHideDriver = EnumerateDevicesStore("HidHide.inf").Any();
                bool ds3ControllerDriver = EnumerateDevicesStore("Ds3Controller.inf").Any();
                bool fakerInputDriver = EnumerateDevicesStore("FakerInput.inf").Any();
                return virtualBusDriver && hidHideDriver && ds3ControllerDriver && fakerInputDriver;
            }
            catch
            {
                Debug.WriteLine("Failed to check installed drivers.");
                return false;
            }
        }

        //Check driver version
        bool CheckDriversVersion()
        {
            try
            {
                foreach (FileInfo infNames in EnumerateDevicesStore("ViGEmBus.inf"))
                {
                    string availableVersion = File.ReadAllLines(@"Drivers\ViGEmBus\x64\ViGEmBus.inf").FirstOrDefault(x => x.StartsWith("DriverVer"));
                    string installedVersion = File.ReadAllLines(infNames.FullName).FirstOrDefault(x => x.StartsWith("DriverVer"));
                    //Debug.WriteLine("ViGEmBus: " + installedVersion + " / " + availableVersion);
                    if (availableVersion != installedVersion) { return false; } else { break; }
                }

                foreach (FileInfo infNames in EnumerateDevicesStore("HidHide.inf"))
                {
                    string availableVersion = File.ReadAllLines(@"Drivers\HidHide\x64\HidHide.inf").FirstOrDefault(x => x.StartsWith("DriverVer"));
                    string installedVersion = File.ReadAllLines(infNames.FullName).FirstOrDefault(x => x.StartsWith("DriverVer"));
                    //Debug.WriteLine("HidHide: " + installedVersion + " / " + availableVersion);
                    if (availableVersion != installedVersion) { return false; } else { break; }
                }

                foreach (FileInfo infNames in EnumerateDevicesStore("Ds3Controller.inf"))
                {
                    string availableVersion = File.ReadAllLines(@"Drivers\Ds3Controller\Ds3Controller.inf").FirstOrDefault(x => x.StartsWith("DriverVer"));
                    string installedVersion = File.ReadAllLines(infNames.FullName).FirstOrDefault(x => x.StartsWith("DriverVer"));
                    //Debug.WriteLine("Ds3Controller: " + installedVersion + " / " + availableVersion);
                    if (availableVersion != installedVersion) { return false; } else { break; }
                }

                foreach (FileInfo infNames in EnumerateDevicesStore("FakerInput.inf"))
                {
                    string availableVersion = File.ReadAllLines(@"Drivers\FakerInput\x64\FakerInput.inf").FirstOrDefault(x => x.StartsWith("DriverVer"));
                    string installedVersion = File.ReadAllLines(infNames.FullName).FirstOrDefault(x => x.StartsWith("DriverVer"));
                    //Debug.WriteLine("FakerInput: " + installedVersion + " / " + availableVersion);
                    if (availableVersion != installedVersion) { return false; } else { break; }
                }

                Debug.WriteLine("Drivers seem to be up to date.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check driver version: " + ex.Message);
                return true;
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

        //Open the hid hide device
        bool OpenHidHideDevice()
        {
            try
            {
                vHidHideDevice = new HidHideDevice();
                if (vHidHideDevice.Connected)
                {
                    Debug.WriteLine("HidHide device is installed.");
                    return true;
                }
                else
                {
                    Debug.WriteLine("HidHide device not installed.");
                    return false;
                }
            }
            catch
            {
                Debug.WriteLine("Failed to open HidHide device.");
                return false;
            }
        }

        //Open the FakerInput device
        bool OpenFakerInputDevice()
        {
            try
            {
                vFakerInputDevice = new FakerInputDevice();
                if (vFakerInputDevice.Connected)
                {
                    Debug.WriteLine("FakerInput device is installed.");
                    return true;
                }
                else
                {
                    Debug.WriteLine("FakerInput device not installed.");
                    return false;
                }
            }
            catch
            {
                Debug.WriteLine("Failed to open FakerInput device.");
                return false;
            }
        }
    }
}