using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
                //Try to open the device exclusively
                DeviceShareMode = 0;
                DeviceHandle = DeviceOpen(DevicePath, DeviceAccessMode, DeviceReadWriteMode, DeviceShareMode);
                IsExclusive = true;

                //If failed to open exclusively open normally
                if (DeviceHandle.ToInt32() == NativeMethods_Hid.INVALID_HANDLE_VALUE)
                {
                    //Debug.WriteLine("Failed to open device exclusively, opening normally.");
                    DeviceShareMode = NativeMethods_Hid.FILE_SHARE_READ | NativeMethods_Hid.FILE_SHARE_WRITE;
                    DeviceHandle = DeviceOpen(DevicePath, DeviceAccessMode, DeviceReadWriteMode, DeviceShareMode);
                    IsExclusive = false;
                }

                return DeviceHandle.ToInt32() != NativeMethods_Hid.INVALID_HANDLE_VALUE;
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
                for (int i = 0; i < 6; i++) { MacAddressBytes[5 - i] = Convert.ToByte(MacAddressSplit[i], 16); }
                Debug.WriteLine("Disconnecting bluetooth device: " + MacAddressRaw);

                //Disconnect the device from bluetooth
                NativeMethods_Bluetooth.BLUETOOTH_FIND_RADIO_PARAMS RadioFindParams = new NativeMethods_Bluetooth.BLUETOOTH_FIND_RADIO_PARAMS();
                RadioFindParams.dwSize = Marshal.SizeOf(RadioFindParams);

                int Transferred = 0;
                IntPtr BluetoothHandle = IntPtr.Zero;
                IntPtr RadioHandle = NativeMethods_Bluetooth.BluetoothFindFirstRadio(ref RadioFindParams, ref BluetoothHandle);

                bool ControllerDisconnected = false;
                while (!ControllerDisconnected)
                {
                    ControllerDisconnected = NativeMethods_DevMan.DeviceIoControl(BluetoothHandle, NativeMethods_Bluetooth.IOCTL_BTH_DISCONNECT_DEVICE, MacAddressBytes, MacAddressBytes.Length, null, 0, ref Transferred, IntPtr.Zero);
                    NativeMethods_Hid.CloseHandle(BluetoothHandle);
                    if (!ControllerDisconnected)
                    {
                        if (!NativeMethods_Bluetooth.BluetoothFindNextRadio(RadioHandle, ref BluetoothHandle))
                        {
                            ControllerDisconnected = true;
                        }
                    }
                }

                NativeMethods_Bluetooth.BluetoothFindRadioClose(RadioHandle);
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
                NativeMethods_Hid.HidD_GetHidGuid(ref HidGuid);

                IntPtr DeviceClass = NativeMethods_Hid.SetupDiGetClassDevs(ref HidGuid, ConvertPathToInstanceId(), 0, NativeMethods_Hid.DIGCF_DEVICEINTERFACE);
                NativeMethods_Hid.SP_DEVINFO_DATA DeviceInfoData = new NativeMethods_Hid.SP_DEVINFO_DATA();
                DeviceInfoData.cbSize = Marshal.SizeOf(DeviceInfoData);

                bool DeviceStatus = NativeMethods_Hid.SetupDiEnumDeviceInfo(DeviceClass, 0, ref DeviceInfoData);
                if (!DeviceStatus)
                {
                    Debug.WriteLine("SetupDi: Failed getting device info.");
                    NativeMethods_Hid.SetupDiDestroyDeviceInfoList(DeviceClass);
                    return false;
                }

                NativeMethods_SetupDi.SP_PROPCHANGE_PARAMS PropertyParams = new NativeMethods_SetupDi.SP_PROPCHANGE_PARAMS();
                PropertyParams.classInstallHeader.cbSize = Marshal.SizeOf(PropertyParams.classInstallHeader);
                PropertyParams.classInstallHeader.installFunction = NativeMethods_SetupDi.DIF_PROPERTYCHANGE;
                PropertyParams.Scope = NativeMethods_SetupDi.DICS_FLAG_GLOBAL;

                //Set disable param
                PropertyParams.stateChange = NativeMethods_SetupDi.DICS_DISABLE;

                //Prepare the device
                DeviceStatus = NativeMethods_SetupDi.SetupDiSetClassInstallParams(DeviceClass, ref DeviceInfoData, ref PropertyParams, Marshal.SizeOf(PropertyParams));
                if (!DeviceStatus)
                {
                    Debug.WriteLine("SetupDi: Failed to set install params.");
                    NativeMethods_Hid.SetupDiDestroyDeviceInfoList(DeviceClass);
                    return false;
                }

                //Disable the device
                DeviceStatus = NativeMethods_SetupDi.SetupDiCallClassInstaller(NativeMethods_SetupDi.DIF_PROPERTYCHANGE, DeviceClass, ref DeviceInfoData);
                if (!DeviceStatus)
                {
                    Debug.WriteLine("SetupDi: Failed to disable the device.");
                    NativeMethods_Hid.SetupDiDestroyDeviceInfoList(DeviceClass);
                    return false;
                }

                NativeMethods_Hid.SetupDiDestroyDeviceInfoList(DeviceClass);
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
                NativeMethods_Hid.HidD_GetHidGuid(ref HidGuid);

                IntPtr DeviceClass = NativeMethods_Hid.SetupDiGetClassDevs(ref HidGuid, ConvertPathToInstanceId(), 0, NativeMethods_Hid.DIGCF_DEVICEINTERFACE);
                NativeMethods_Hid.SP_DEVINFO_DATA DeviceInfoData = new NativeMethods_Hid.SP_DEVINFO_DATA();
                DeviceInfoData.cbSize = Marshal.SizeOf(DeviceInfoData);

                bool DeviceStatus = NativeMethods_Hid.SetupDiEnumDeviceInfo(DeviceClass, 0, ref DeviceInfoData);
                if (!DeviceStatus)
                {
                    Debug.WriteLine("SetupDi: Failed getting device info.");
                    NativeMethods_Hid.SetupDiDestroyDeviceInfoList(DeviceClass);
                    return false;
                }

                NativeMethods_SetupDi.SP_PROPCHANGE_PARAMS PropertyParams = new NativeMethods_SetupDi.SP_PROPCHANGE_PARAMS();
                PropertyParams.classInstallHeader.cbSize = Marshal.SizeOf(PropertyParams.classInstallHeader);
                PropertyParams.classInstallHeader.installFunction = NativeMethods_SetupDi.DIF_PROPERTYCHANGE;
                PropertyParams.Scope = NativeMethods_SetupDi.DICS_FLAG_GLOBAL;

                //Set enable param
                PropertyParams.stateChange = NativeMethods_SetupDi.DICS_ENABLE;

                //Prepare the device
                DeviceStatus = NativeMethods_SetupDi.SetupDiSetClassInstallParams(DeviceClass, ref DeviceInfoData, ref PropertyParams, Marshal.SizeOf(PropertyParams));
                if (!DeviceStatus)
                {
                    Debug.WriteLine("SetupDi: Failed to set install params.");
                    NativeMethods_Hid.SetupDiDestroyDeviceInfoList(DeviceClass);
                    return false;
                }

                //Enable the device
                DeviceStatus = NativeMethods_SetupDi.SetupDiCallClassInstaller(NativeMethods_SetupDi.DIF_PROPERTYCHANGE, DeviceClass, ref DeviceInfoData);
                if (!DeviceStatus)
                {
                    Debug.WriteLine("SetupDi: Failed to enable the device.");
                    NativeMethods_Hid.SetupDiDestroyDeviceInfoList(DeviceClass);
                    return false;
                }

                NativeMethods_Hid.SetupDiDestroyDeviceInfoList(DeviceClass);
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
            catch { return string.Empty; }
        }
    }
}