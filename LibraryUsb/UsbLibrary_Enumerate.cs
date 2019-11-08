using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static LibraryUsb.NativeMethods_Hid;
using static LibraryUsb.NativeMethods_SetupApi;

namespace LibraryUsb
{
    public partial class HidDevices
    {
        public class EnumerateInfo
        {
            public string DevicePath { get; set; }
            public string Description { get; set; }
            public string HardwareId { get; set; }
        }

        public static List<EnumerateInfo> EnumerateDevices(Guid EnumerateGuid)
        {
            List<EnumerateInfo> EnumeratedInfo = new List<EnumerateInfo>();
            IntPtr deviceInfoSet = SetupDiGetClassDevs(ref EnumerateGuid, null, 0, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
            if (deviceInfoSet.ToInt64() != INVALID_HANDLE_VALUE)
            {
                int deviceIndex = 0;
                SP_DEVINFO_DATA deviceInfoData = CreateDeviceInfoData();

                while (SetupDiEnumDeviceInfo(deviceInfoSet, deviceIndex, ref deviceInfoData))
                {
                    deviceIndex++;
                    int deviceInterfaceIndex = 0;
                    SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
                    deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);

                    while (SetupDiEnumDeviceInterfaces(deviceInfoSet, ref deviceInfoData, ref EnumerateGuid, deviceInterfaceIndex, ref deviceInterfaceData))
                    {
                        deviceInterfaceIndex++;
                        string devicePath = GetDevicePath(deviceInfoSet, deviceInterfaceData);
                        string description = GetBusReportedDeviceDescription(deviceInfoSet, ref deviceInfoData) ?? GetDeviceDescription(deviceInfoSet, ref deviceInfoData);
                        string hardwareId = GetDeviceHardwareId(deviceInfoSet, ref deviceInfoData);
                        EnumeratedInfo.Add(new EnumerateInfo { DevicePath = devicePath, Description = description, HardwareId = hardwareId });
                    }
                }
                SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }

            return EnumeratedInfo;
        }

        private static SP_DEVINFO_DATA CreateDeviceInfoData()
        {
            SP_DEVINFO_DATA deviceInfoData = new SP_DEVINFO_DATA();

            deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);
            deviceInfoData.DevInst = 0;
            deviceInfoData.ClassGuid = Guid.Empty;
            deviceInfoData.Reserved = IntPtr.Zero;

            return deviceInfoData;
        }

        private static string GetDevicePath(IntPtr deviceInfoSet, SP_DEVICE_INTERFACE_DATA deviceInterfaceData)
        {
            try
            {
                int bufferSize = 0;
                SP_DEVICE_INTERFACE_DETAIL_DATA interfaceDetail = new SP_DEVICE_INTERFACE_DETAIL_DATA { Size = IntPtr.Size == 4 ? 4 + Marshal.SystemDefaultCharSize : 8 };

                SetupDiGetDeviceInterfaceDetailBuffer(deviceInfoSet, ref deviceInterfaceData, IntPtr.Zero, 0, ref bufferSize, IntPtr.Zero);
                bool SuccessDetail = SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData, ref interfaceDetail, bufferSize, ref bufferSize, IntPtr.Zero);
                if (SuccessDetail) { return interfaceDetail.DevicePath; }
            }
            catch { }
            return null;
        }

        private static string GetDeviceDescription(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA devinfoData)
        {
            try
            {
                byte[] descriptionBuffer = new byte[1024];
                int propertyType = 0;
                int requiredSize = 0;

                bool Success = SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref devinfoData, SPDRP_DEVICEDESC, ref propertyType, descriptionBuffer, descriptionBuffer.Length, ref requiredSize);
                if (Success) { return descriptionBuffer.ToUTF8String(); }
            }
            catch { }
            return null;
        }

        private static string GetBusReportedDeviceDescription(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA devinfoData)
        {
            try
            {
                byte[] descriptionBuffer = new byte[1024];
                ulong propertyType = 0;
                int requiredSize = 0;

                bool Success = SetupDiGetDeviceProperty(deviceInfoSet, ref devinfoData, ref DEVPKEY_Device_BusReportedDeviceDesc, ref propertyType, descriptionBuffer, descriptionBuffer.Length, ref requiredSize, 0);
                if (Success) { return descriptionBuffer.ToUTF16String(); }
            }
            catch { }
            return null;
        }

        private static string GetDeviceHardwareId(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA devinfoData)
        {
            try
            {
                byte[] hardwareBuffer = new byte[1024];
                int propertyType = 0;
                int requiredSize = 0;

                bool Success = SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref devinfoData, SPDRP_HARDWAREID, ref propertyType, hardwareBuffer, hardwareBuffer.Length, ref requiredSize);
                if (Success) { return hardwareBuffer.ToUTF8String(); }
            }
            catch { }
            return null;
        }
    }
}