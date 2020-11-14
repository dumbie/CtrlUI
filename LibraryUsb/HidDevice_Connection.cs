using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static LibraryUsb.NativeMethods_Hid;
using static LibraryUsb.NativeMethods_SetupApi;

namespace LibraryUsb
{
    public partial class HidDevice
    {
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
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to disable the device: " + ex.Message);
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
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to enable the device: " + ex.Message);
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