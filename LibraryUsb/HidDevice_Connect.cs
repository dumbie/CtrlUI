using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static LibraryUsb.NativeMethods_Bluetooth;
using static LibraryUsb.NativeMethods_DeviceManager;
using static LibraryUsb.NativeMethods_Hid;
using static LibraryUsb.NativeMethods_SetupApi;

namespace LibraryUsb
{
    public partial class HidDevice
    {
        public bool IsExclusive { get; private set; }

        //Try to open the device exclusively
        public bool OpenDeviceExclusively()
        {
            try
            {
                //Debug.WriteLine("Opening device: " + DevicePath);

                //Try to open the device exclusively
                uint DeviceShareMode = (uint)FILE_SHARE.FILE_SHARE_NONE;
                DeviceHandle = DeviceOpen(DevicePath, DeviceShareMode);
                IsExclusive = true;

                //If failed to open exclusively open normally
                if (DeviceHandle.ToInt32() == INVALID_HANDLE_VALUE)
                {
                    //Debug.WriteLine("Failed to open device exclusively, opening normally.");
                    DeviceShareMode = (uint)FILE_SHARE.FILE_SHARE_READ | (uint)FILE_SHARE.FILE_SHARE_WRITE;
                    DeviceHandle = DeviceOpen(DevicePath, DeviceShareMode);
                    IsExclusive = false;
                }

                return DeviceHandle.ToInt32() != INVALID_HANDLE_VALUE;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed opening HID device: " + ex.Message);
                return false;
            }
        }

        //Disconnect the device from bluetooth
        public void DisconnectBluetooth()
        {
            try
            {
                Debug.WriteLine("Attempting to disconnect bluetooth device.");

                //Get the device serial number
                string MacAddressRaw = Attributes.SerialNumber;

                //Set and parse the mac address
                byte[] MacAddressBytes = new byte[8];
                string[] MacAddressSplit = { $"{MacAddressRaw[0]}{MacAddressRaw[1]}", $"{MacAddressRaw[2]}{MacAddressRaw[3]}", $"{MacAddressRaw[4]}{MacAddressRaw[5]}", $"{MacAddressRaw[6]}{MacAddressRaw[7]}", $"{MacAddressRaw[8]}{MacAddressRaw[9]}", $"{MacAddressRaw[10]}{MacAddressRaw[11]}" };
                for (int i = 0; i < 6; i++)
                {
                    MacAddressBytes[5 - i] = Convert.ToByte(MacAddressSplit[i], 16);
                }

                Debug.WriteLine("Disconnecting bluetooth device: " + MacAddressRaw);

                //Disconnect the device from bluetooth
                BLUETOOTH_FIND_RADIO_PARAMS RadioFindParams = new BLUETOOTH_FIND_RADIO_PARAMS();
                RadioFindParams.dwSize = Marshal.SizeOf(RadioFindParams);

                IntPtr BluetoothHandle = IntPtr.Zero;
                IntPtr RadioHandle = BluetoothFindFirstRadio(ref RadioFindParams, ref BluetoothHandle);

                bool ControllerDisconnected = false;
                while (!ControllerDisconnected)
                {
                    ControllerDisconnected = DeviceIoControl(BluetoothHandle, IoControlCodes.IOCTL_BTH_DISCONNECT_DEVICE, MacAddressBytes, MacAddressBytes.Length, null, 0, out uint Transferred, IntPtr.Zero);
                    CloseHandle(BluetoothHandle);
                    if (!ControllerDisconnected)
                    {
                        if (!BluetoothFindNextRadio(RadioHandle, ref BluetoothHandle))
                        {
                            ControllerDisconnected = true;
                        }
                    }
                }

                BluetoothFindRadioClose(RadioHandle);
                Debug.WriteLine("Bluetooth disconnected succesfully: " + ControllerDisconnected);
            }
            catch { }
        }

        //Try to disable the device
        public bool DisableDevice()
        {
            try
            {
                Guid HidGuid = Guid.Empty;
                HidD_GetHidGuid(ref HidGuid);

                IntPtr DeviceClass = SetupDiGetClassDevs(ref HidGuid, ConvertPathToInstanceId(), 0, DIGCF_DEVICEINTERFACE);
                SP_DEVINFO_DATA DeviceInfoData = new SP_DEVINFO_DATA();
                DeviceInfoData.cbSize = Marshal.SizeOf(DeviceInfoData);

                bool DeviceStatus = SetupDiEnumDeviceInfo(DeviceClass, 0, ref DeviceInfoData);
                if (!DeviceStatus)
                {
                    Debug.WriteLine("SetupDi: Failed getting device info.");
                    SetupDiDestroyDeviceInfoList(DeviceClass);
                    return false;
                }

                SP_PROPCHANGE_PARAMS PropertyParams = new SP_PROPCHANGE_PARAMS();
                PropertyParams.classInstallHeader.cbSize = Marshal.SizeOf(PropertyParams.classInstallHeader);
                PropertyParams.classInstallHeader.installFunction = DIF_PROPERTYCHANGE;
                PropertyParams.Scope = DICS_FLAG_GLOBAL;

                //Set disable param
                PropertyParams.stateChange = DICS_DISABLE;

                //Prepare the device
                DeviceStatus = SetupDiSetClassInstallParams(DeviceClass, ref DeviceInfoData, ref PropertyParams, Marshal.SizeOf(PropertyParams));
                if (!DeviceStatus)
                {
                    Debug.WriteLine("SetupDi: Failed to set install params.");
                    SetupDiDestroyDeviceInfoList(DeviceClass);
                    return false;
                }

                //Disable the device
                DeviceStatus = SetupDiCallClassInstaller(DIF_PROPERTYCHANGE, DeviceClass, ref DeviceInfoData);
                if (!DeviceStatus)
                {
                    Debug.WriteLine("SetupDi: Failed to disable the device.");
                    SetupDiDestroyDeviceInfoList(DeviceClass);
                    return false;
                }

                SetupDiDestroyDeviceInfoList(DeviceClass);
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to disable the device.");
                return false;
            }
        }

        //Try to enable the device
        public bool EnableDevice()
        {
            try
            {
                Guid HidGuid = Guid.Empty;
                HidD_GetHidGuid(ref HidGuid);

                IntPtr DeviceClass = SetupDiGetClassDevs(ref HidGuid, ConvertPathToInstanceId(), 0, DIGCF_DEVICEINTERFACE);
                SP_DEVINFO_DATA DeviceInfoData = new SP_DEVINFO_DATA();
                DeviceInfoData.cbSize = Marshal.SizeOf(DeviceInfoData);

                bool DeviceStatus = SetupDiEnumDeviceInfo(DeviceClass, 0, ref DeviceInfoData);
                if (!DeviceStatus)
                {
                    Debug.WriteLine("SetupDi: Failed getting device info.");
                    SetupDiDestroyDeviceInfoList(DeviceClass);
                    return false;
                }

                SP_PROPCHANGE_PARAMS PropertyParams = new SP_PROPCHANGE_PARAMS();
                PropertyParams.classInstallHeader.cbSize = Marshal.SizeOf(PropertyParams.classInstallHeader);
                PropertyParams.classInstallHeader.installFunction = DIF_PROPERTYCHANGE;
                PropertyParams.Scope = DICS_FLAG_GLOBAL;

                //Set enable param
                PropertyParams.stateChange = DICS_ENABLE;

                //Prepare the device
                DeviceStatus = SetupDiSetClassInstallParams(DeviceClass, ref DeviceInfoData, ref PropertyParams, Marshal.SizeOf(PropertyParams));
                if (!DeviceStatus)
                {
                    Debug.WriteLine("SetupDi: Failed to set install params.");
                    SetupDiDestroyDeviceInfoList(DeviceClass);
                    return false;
                }

                //Enable the device
                DeviceStatus = SetupDiCallClassInstaller(DIF_PROPERTYCHANGE, DeviceClass, ref DeviceInfoData);
                if (!DeviceStatus)
                {
                    Debug.WriteLine("SetupDi: Failed to enable the device.");
                    SetupDiDestroyDeviceInfoList(DeviceClass);
                    return false;
                }

                SetupDiDestroyDeviceInfoList(DeviceClass);
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to enable the device.");
                return false;
            }
        }

        //Convert device path to InstanceId
        public string ConvertPathToInstanceId()
        {
            try
            {
                string InstanceId = DevicePath.Remove(0, DevicePath.LastIndexOf("\\") + 1);
                InstanceId = InstanceId.Remove(InstanceId.LastIndexOf("{"));
                InstanceId = InstanceId.Replace('#', '\\');
                if (InstanceId.EndsWith("\\")) { InstanceId = InstanceId.Remove(InstanceId.Length - 1); }
                return InstanceId;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}