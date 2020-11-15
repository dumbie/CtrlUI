using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static LibraryUsb.NativeMethods_DeviceManager;
using static LibraryUsb.NativeMethods_Variables;

namespace LibraryUsb
{
    public class Enumerate
    {
        public class EnumerateInfo
        {
            public string DevicePath { get; set; }
            public string Description { get; set; }
            public string HardwareId { get; set; }
        }

        public static List<EnumerateInfo> EnumerateDevices(Guid enumerateGuid)
        {
            IntPtr deviceInfoSet = IntPtr.Zero;
            List<EnumerateInfo> enumeratedInfo = new List<EnumerateInfo>();
            try
            {
                deviceInfoSet = SetupDiGetClassDevs(ref enumerateGuid, null, 0, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
                if (deviceInfoSet != (IntPtr)INVALID_HANDLE_VALUE)
                {
                    int deviceIndex = 0;
                    SP_DEVINFO_DATA deviceInfoData = new SP_DEVINFO_DATA();
                    deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);

                    while (SetupDiEnumDeviceInfo(deviceInfoSet, deviceIndex, ref deviceInfoData))
                    {
                        deviceIndex++;
                        int deviceInterfaceIndex = 0;
                        SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
                        deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);

                        while (SetupDiEnumDeviceInterfaces(deviceInfoSet, ref deviceInfoData, ref enumerateGuid, deviceInterfaceIndex, ref deviceInterfaceData))
                        {
                            deviceInterfaceIndex++;
                            string devicePath = GetDevicePath(deviceInfoSet, deviceInterfaceData);
                            string description = GetBusReportedDeviceDescription(deviceInfoSet, ref deviceInfoData);
                            if (string.IsNullOrWhiteSpace(description))
                            {
                                description = GetDeviceDescription(deviceInfoSet, ref deviceInfoData);
                            }
                            string hardwareId = GetDeviceHardwareId(deviceInfoSet, ref deviceInfoData);
                            enumeratedInfo.Add(new EnumerateInfo { DevicePath = devicePath, Description = description, HardwareId = hardwareId });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed enumerating devices: " + ex.Message);
            }
            finally
            {
                if (deviceInfoSet != IntPtr.Zero)
                {
                    SetupDiDestroyDeviceInfoList(deviceInfoSet);
                }
            }
            return enumeratedInfo;
        }

        public static string GetDevicePath(IntPtr deviceInfoSet, SP_DEVICE_INTERFACE_DATA deviceInterfaceData)
        {
            try
            {
                int bufferSize = 0;
                SP_DEVICE_INTERFACE_DETAIL_DATA interfaceDetail = new SP_DEVICE_INTERFACE_DETAIL_DATA
                {
                    Size = IntPtr.Size == 4 ? 4 + Marshal.SystemDefaultCharSize : 8
                };

                SetupDiGetDeviceInterfaceDetailBuffer(deviceInfoSet, ref deviceInterfaceData, IntPtr.Zero, 0, ref bufferSize, IntPtr.Zero);
                bool success = SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData, ref interfaceDetail, bufferSize, ref bufferSize, IntPtr.Zero);
                if (success) { return interfaceDetail.DevicePath; }
            }
            catch { }
            return string.Empty;
        }

        public static string GetDeviceDescription(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA devinfoData)
        {
            try
            {
                byte[] descriptionBuffer = new byte[1024];
                int propertyType = 0;
                int requiredSize = 0;

                bool success = SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref devinfoData, SPDRP_DEVICEDESC, ref propertyType, descriptionBuffer, descriptionBuffer.Length, ref requiredSize);
                if (success) { return descriptionBuffer.ToUTF8String(); }
            }
            catch { }
            return string.Empty;
        }

        public static string GetBusReportedDeviceDescription(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA devinfoData)
        {
            try
            {
                byte[] descriptionBuffer = new byte[1024];
                ulong propertyType = 0;
                int requiredSize = 0;

                bool success = SetupDiGetDeviceProperty(deviceInfoSet, ref devinfoData, ref DEVPKEY_Device_BusReportedDeviceDesc, ref propertyType, descriptionBuffer, descriptionBuffer.Length, ref requiredSize, 0);
                if (success) { return descriptionBuffer.ToUTF16String(); }
            }
            catch { }
            return string.Empty;
        }

        public static string GetDeviceHardwareId(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA devinfoData)
        {
            try
            {
                byte[] hardwareBuffer = new byte[1024];
                int propertyType = 0;
                int requiredSize = 0;

                bool success = SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref devinfoData, SPDRP_HARDWAREID, ref propertyType, hardwareBuffer, hardwareBuffer.Length, ref requiredSize);
                if (success) { return hardwareBuffer.ToUTF8String(); }
            }
            catch { }
            return string.Empty;
        }
    }
}