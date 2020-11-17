using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static LibraryUsb.NativeMethods_SetupApi;

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
            IntPtr deviceInfoList = IntPtr.Zero;
            List<EnumerateInfo> enumeratedInfo = new List<EnumerateInfo>();
            try
            {
                int deviceIndex = 0;
                SP_DEVICE_INFO_DATA deviceInfoData = new SP_DEVICE_INFO_DATA();
                deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);

                deviceInfoList = SetupDiGetClassDevs(ref enumerateGuid, null, IntPtr.Zero, DiGetClassFlag.DIGCF_PRESENT | DiGetClassFlag.DIGCF_DEVICEINTERFACE);
                while (SetupDiEnumDeviceInfo(deviceInfoList, deviceIndex, ref deviceInfoData))
                {
                    try
                    {
                        deviceIndex++;
                        int deviceMemberIndex = 0;
                        SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
                        deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);

                        while (SetupDiEnumDeviceInterfaces(deviceInfoList, deviceInfoData, ref enumerateGuid, deviceMemberIndex, ref deviceInterfaceData))
                        {
                            try
                            {
                                deviceMemberIndex++;
                                string devicePath = GetDevicePath(deviceInfoList, deviceInterfaceData);
                                string description = GetBusReportedDeviceDescription(deviceInfoList, ref deviceInfoData);
                                if (string.IsNullOrWhiteSpace(description))
                                {
                                    description = GetDeviceDescription(deviceInfoList, ref deviceInfoData);
                                }
                                string hardwareId = GetDeviceHardwareId(deviceInfoList, ref deviceInfoData);
                                enumeratedInfo.Add(new EnumerateInfo { DevicePath = devicePath, Description = description, HardwareId = hardwareId });
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed enumerating devices: " + ex.Message);
            }
            finally
            {
                if (deviceInfoList != IntPtr.Zero)
                {
                    SetupDiDestroyDeviceInfoList(deviceInfoList);
                }
            }
            return enumeratedInfo;
        }

        public static string GetDevicePath(IntPtr deviceInfoSet, SP_DEVICE_INTERFACE_DATA deviceInterfaceData)
        {
            try
            {
                //Get the buffer size
                int bufferSize = 0;
                SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData, IntPtr.Zero, 0, ref bufferSize, IntPtr.Zero);
                SP_DEVICE_INTERFACE_DETAIL_DATA interfaceDetail = new SP_DEVICE_INTERFACE_DETAIL_DATA
                {
                    Size = IntPtr.Size == 4 ? 4 + Marshal.SystemDefaultCharSize : 8
                };

                //Read device details
                if (SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData, ref interfaceDetail, bufferSize, ref bufferSize, IntPtr.Zero))
                {
                    if (!string.IsNullOrWhiteSpace(interfaceDetail.DevicePath))
                    {
                        return interfaceDetail.DevicePath.ToLower();
                    }
                    else
                    {
                        Debug.WriteLine("Failed to get device path, empty string.");
                        return string.Empty;
                    }
                }
                else
                {
                    Debug.WriteLine("Failed to get device path, detail missing.");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get device path: " + ex.Message);
                return string.Empty;
            }
        }

        public static string GetDeviceDescription(IntPtr deviceInfoSet, ref SP_DEVICE_INFO_DATA devinfoData)
        {
            try
            {
                byte[] descriptionBuffer = new byte[1024];
                int propertyType = 0;
                int requiredSize = 0;

                if (SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref devinfoData, DiDeviceRegistryProperty.SPDRP_DEVICEDESC, ref propertyType, descriptionBuffer, descriptionBuffer.Length, ref requiredSize))
                {
                    return descriptionBuffer.ToUTF8String();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get device description: " + ex.Message);
                return string.Empty;
            }
        }

        public static string GetBusReportedDeviceDescription(IntPtr deviceInfoSet, ref SP_DEVICE_INFO_DATA devinfoData)
        {
            try
            {
                byte[] descriptionBuffer = new byte[1024];
                ulong propertyType = 0;
                int requiredSize = 0;

                if (SetupDiGetDeviceProperty(deviceInfoSet, ref devinfoData, ref DEVPKEY_Device_BusReportedDeviceDesc, ref propertyType, descriptionBuffer, descriptionBuffer.Length, ref requiredSize, 0))
                {
                    return descriptionBuffer.ToUTF16String();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get bus device description: " + ex.Message);
                return string.Empty;
            }
        }

        public static string GetDeviceHardwareId(IntPtr deviceInfoSet, ref SP_DEVICE_INFO_DATA devinfoData)
        {
            try
            {
                byte[] hardwareBuffer = new byte[1024];
                int propertyType = 0;
                int requiredSize = 0;

                if (SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref devinfoData, DiDeviceRegistryProperty.SPDRP_HARDWAREID, ref propertyType, hardwareBuffer, hardwareBuffer.Length, ref requiredSize))
                {
                    return hardwareBuffer.ToUTF8String();
                }
                else
                {
                    Debug.WriteLine("Failed to get hardware id.");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get hardware id: " + ex.Message);
                return string.Empty;
            }
        }
    }
}