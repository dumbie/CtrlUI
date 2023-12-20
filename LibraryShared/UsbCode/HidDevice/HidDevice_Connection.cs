using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVDevices.Enumerate;
using static ArnoldVinkCode.AVDevices.Interop;
using static LibraryUsb.NativeMethods_Guid;

namespace LibraryUsb
{
    public partial class HidDevice
    {
        public bool DisableDevice()
        {
            try
            {
                return ChangePropertyDevice(GuidClassHidDevice, DeviceInstanceId, DiChangeState.DICS_DISABLE);
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
                return ChangePropertyDevice(GuidClassHidDevice, DeviceInstanceId, DiChangeState.DICS_ENABLE);
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
                return BthDevice.BluetoothDisconnect(Attributes.SerialNumber);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed disconnecting bluetooth: " + ex.Message);
                return false;
            }
        }
    }
}