using System;
using System.Diagnostics;
using static LibraryUsb.DeviceManager;
using static LibraryUsb.NativeMethods_SetupApi;

namespace LibraryUsb
{
    public partial class HidDevice
    {
        public bool DisableDevice()
        {
            try
            {
                return ChangePropertyDevice(DevicePath, DiChangeState.DICS_DISABLE);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to disable the device: " + ex.Message);
                return false;
            }
        }

        public bool EnableDevice()
        {
            try
            {
                return ChangePropertyDevice(DevicePath, DiChangeState.DICS_ENABLE);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to enable the device: " + ex.Message);
                return false;
            }
        }

        public bool BluetoothDisconnect()
        {
            try
            {
                return Bluetooth.BluetoothDisconnect(Attributes.SerialNumber);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed disconnecting bluetooth: " + ex.Message);
                return false;
            }
        }
    }
}