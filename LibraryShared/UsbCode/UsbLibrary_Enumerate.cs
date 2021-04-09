using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
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
            public string ModelId { get; set; }
        }

        public static List<string> EnumerateDevicesStore(string infFileName)
        {
            List<string> infPaths = new List<string>();
            try
            {
                string windowsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                string driverStoreFileRepository = Path.Combine(windowsFolderPath, @"System32\DriverStore\FileRepository");
                DirectoryInfo driverStoreDirectory = new DirectoryInfo(driverStoreFileRepository);
                FileInfo[] driverStoreFiles = driverStoreDirectory.GetFiles("*.inf", SearchOption.AllDirectories);
                foreach (FileInfo fileInfo in driverStoreFiles)
                {
                    try
                    {
                        if (fileInfo.Name.ToLower() == infFileName.ToLower())
                        {
                            infPaths.Add(fileInfo.FullName);
                        }
                    }
                    catch { }
                }
                return infPaths;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed enumerating devices store: " + ex.Message);
                return infPaths;
            }
        }

        public static List<EnumerateInfo> EnumerateDevicesWmi()
        {
            List<EnumerateInfo> enumeratedInfoList = new List<EnumerateInfo>();
            try
            {
                using (ManagementObjectSearcher objSearcher = new ManagementObjectSearcher("Select * from Win32_PnPSignedDriver"))
                {
                    using (ManagementObjectCollection objCollection = objSearcher.Get())
                    {
                        foreach (ManagementObject objDriver in objCollection)
                        {
                            try
                            {
                                EnumerateInfo foundDevice = new EnumerateInfo();
                                enumeratedInfoList.Add(foundDevice);
                            }
                            catch { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed enumerating devices wmi: " + ex.Message);
            }
            return enumeratedInfoList;
        }

        public static List<EnumerateInfo> EnumerateDevicesDi(Guid enumerateGuid, bool isPresent)
        {
            IntPtr deviceInfoList = IntPtr.Zero;
            List<EnumerateInfo> enumeratedInfoList = new List<EnumerateInfo>();
            try
            {
                int deviceIndexInfo = 0;
                SP_DEVICE_INFO_DATA deviceInfoData = new SP_DEVICE_INFO_DATA();
                deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);

                //Get device information
                if (isPresent)
                {
                    deviceInfoList = SetupDiGetClassDevs(enumerateGuid, null, IntPtr.Zero, DiGetClassFlag.DIGCF_PRESENT | DiGetClassFlag.DIGCF_DEVICEINTERFACE);
                }
                else
                {
                    deviceInfoList = SetupDiGetClassDevs(enumerateGuid, null, IntPtr.Zero, DiGetClassFlag.DIGCF_DEVICEINTERFACE);
                }

                while (SetupDiEnumDeviceInfo(deviceInfoList, deviceIndexInfo, ref deviceInfoData))
                {
                    try
                    {
                        deviceIndexInfo++;
                        int deviceIndexInterfaces = 0;
                        SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
                        deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);

                        while (SetupDiEnumDeviceInterfaces(deviceInfoList, deviceInfoData, enumerateGuid, deviceIndexInterfaces, ref deviceInterfaceData))
                        {
                            try
                            {
                                deviceIndexInterfaces++;
                                string devicePath = GetDevicePath(deviceInfoList, deviceInterfaceData);
                                string description = GetBusReportedDeviceDescription(deviceInfoList, ref deviceInfoData);
                                if (string.IsNullOrWhiteSpace(description))
                                {
                                    description = GetDeviceDescription(deviceInfoList, ref deviceInfoData);
                                }
                                string hardwareId = GetDeviceHardwareId(deviceInfoList, ref deviceInfoData);
                                string modelId = GetDeviceModelId(devicePath);

                                EnumerateInfo foundDevice = new EnumerateInfo();
                                foundDevice.DevicePath = devicePath;
                                foundDevice.Description = description;
                                foundDevice.HardwareId = hardwareId;
                                foundDevice.ModelId = modelId;
                                enumeratedInfoList.Add(foundDevice);
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed enumerating devices di: " + ex.Message);
            }
            finally
            {
                if (deviceInfoList != IntPtr.Zero)
                {
                    SetupDiDestroyDeviceInfoList(deviceInfoList);
                }
            }
            return enumeratedInfoList;
        }

        private static string GetDevicePath(IntPtr deviceInfoList, SP_DEVICE_INTERFACE_DATA deviceInterfaceData)
        {
            try
            {
                //Get the buffer size
                int bufferSize = 0;
                SetupDiGetDeviceInterfaceDetail(deviceInfoList, ref deviceInterfaceData, IntPtr.Zero, 0, ref bufferSize, IntPtr.Zero);
                SP_DEVICE_INTERFACE_DETAIL_DATA interfaceDetail = new SP_DEVICE_INTERFACE_DETAIL_DATA
                {
                    Size = IntPtr.Size == 4 ? 4 + Marshal.SystemDefaultCharSize : 8
                };

                //Read device details
                if (SetupDiGetDeviceInterfaceDetail(deviceInfoList, ref deviceInterfaceData, ref interfaceDetail, bufferSize, ref bufferSize, IntPtr.Zero))
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

        private static string GetDeviceDescription(IntPtr deviceInfoList, ref SP_DEVICE_INFO_DATA devinfoData)
        {
            try
            {
                byte[] descriptionBuffer = new byte[1024];
                int propertyType = 0;
                int requiredSize = 0;

                if (SetupDiGetDeviceRegistryProperty(deviceInfoList, ref devinfoData, DiDeviceRegistryProperty.SPDRP_DEVICEDESC, ref propertyType, descriptionBuffer, descriptionBuffer.Length, ref requiredSize))
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

        private static string GetBusReportedDeviceDescription(IntPtr deviceInfoList, ref SP_DEVICE_INFO_DATA devinfoData)
        {
            try
            {
                byte[] descriptionBuffer = new byte[1024];
                ulong propertyType = 0;
                int requiredSize = 0;

                if (SetupDiGetDeviceProperty(deviceInfoList, ref devinfoData, ref DEVPKEY_Device_BusReportedDeviceDesc, ref propertyType, descriptionBuffer, descriptionBuffer.Length, ref requiredSize, 0))
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

        private static string GetDeviceHardwareId(IntPtr deviceInfoList, ref SP_DEVICE_INFO_DATA devinfoData)
        {
            try
            {
                byte[] hardwareBuffer = new byte[1024];
                int propertyType = 0;
                int requiredSize = 0;

                if (SetupDiGetDeviceRegistryProperty(deviceInfoList, ref devinfoData, DiDeviceRegistryProperty.SPDRP_HARDWAREID, ref propertyType, hardwareBuffer, hardwareBuffer.Length, ref requiredSize))
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

        private static string GetDeviceModelId(string devicePath)
        {
            try
            {
                string modelId = devicePath.Replace("#", @"\");
                modelId = modelId.Replace(@"\\?\hid", @"hid");

                int slashCount = modelId.Count(x => x == '\\');
                if (slashCount > 2)
                {
                    modelId = modelId.Substring(0, modelId.LastIndexOf(@"\"));
                }

                return modelId;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get model id: " + ex.Message);
                return string.Empty;
            }
        }
    }
}