using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static LibraryUsb.Enumerate;
using static LibraryUsb.NativeMethods_DeviceManager;
using static LibraryUsb.NativeMethods_Variables;

namespace LibraryUsb
{
    public class DeviceManager
    {
        public static bool DriverInstall(string driverPackageInfPath, DIIRFLAG flag, ref bool rebootRequired)
        {
            try
            {
                return DiInstallDriver(IntPtr.Zero, driverPackageInfPath, (uint)flag, ref rebootRequired);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to install driver: " + ex.Message);
                return false;
            }
        }

        public static bool DriverUninstall(string driverPackageInfPath, DIIRFLAG flag, ref bool rebootRequired)
        {
            try
            {
                return DiUninstallDriver(IntPtr.Zero, driverPackageInfPath, (uint)flag, ref rebootRequired);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to uninstall driver: " + ex.Message);
                return false;
            }
        }

        public static bool DeviceCreate(string className, Guid classGuid, string node)
        {
            IntPtr deviceInfoSet = IntPtr.Zero;
            try
            {
                SP_DEVINFO_DATA deviceInfoData = new SP_DEVINFO_DATA();
                deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);
                deviceInfoSet = SetupDiCreateDeviceInfoList(ref classGuid, IntPtr.Zero);
                if (deviceInfoSet == IntPtr.Zero)
                {
                    return false;
                }

                if (!SetupDiCreateDeviceInfo(deviceInfoSet, className, ref classGuid, null, IntPtr.Zero, DICD_GENERATE_ID, ref deviceInfoData))
                {
                    return false;
                }

                if (!SetupDiSetDeviceRegistryProperty(deviceInfoSet, ref deviceInfoData, SPDRP_HARDWAREID, node, node.Length * 2))
                {
                    return false;
                }

                if (!SetupDiCallClassInstaller(DIF_REGISTERDEVICE, deviceInfoSet, ref deviceInfoData))
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to create device: " + ex.Message);
                return false;
            }
            finally
            {
                if (deviceInfoSet != IntPtr.Zero)
                {
                    SetupDiDestroyDeviceInfoList(deviceInfoSet);
                }
            }
        }

        public static bool DeviceFind(Guid deviceGuid, ref string devicePath, int instance)
        {
            IntPtr deviceInfoSet = IntPtr.Zero;
            try
            {
                int bufferSize = 0;
                int memberIndex = 0;
                SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
                deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);
                SP_DEVICE_INTERFACE_DATA deviceInterfaceDataDetail = new SP_DEVICE_INTERFACE_DATA();
                deviceInterfaceDataDetail.cbSize = Marshal.SizeOf(deviceInterfaceDataDetail);
                deviceInfoSet = SetupDiGetClassDevs(ref deviceGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);

                while (SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref deviceGuid, memberIndex, ref deviceInterfaceData))
                {
                    SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData, IntPtr.Zero, 0, ref bufferSize, ref deviceInterfaceDataDetail);
                    {
                        if (memberIndex == instance)
                        {
                            devicePath = GetDevicePath(deviceInfoSet, deviceInterfaceData);
                            return true;
                        }
                    }
                    memberIndex++;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to find device: " + ex.Message);
                return false;
            }
            finally
            {
                if (deviceInfoSet != IntPtr.Zero)
                {
                    SetupDiDestroyDeviceInfoList(deviceInfoSet);
                }
            }
        }

        public static bool DeviceRemove(Guid classGuid, string instanceId)
        {
            IntPtr deviceInfoSet = IntPtr.Zero;
            try
            {
                SP_DEVINFO_DATA deviceInterfaceData = new SP_DEVINFO_DATA();
                deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);
                deviceInfoSet = SetupDiGetClassDevs(ref classGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);

                if (SetupDiOpenDeviceInfo(deviceInfoSet, instanceId, IntPtr.Zero, 0, ref deviceInterfaceData))
                {
                    SP_REMOVEDEVICE_PARAMS props = new SP_REMOVEDEVICE_PARAMS();
                    props.ClassInstallHeader = new SP_CLASSINSTALL_HEADER();
                    props.ClassInstallHeader.cbSize = Marshal.SizeOf(props.ClassInstallHeader);
                    props.ClassInstallHeader.installFunction = DIF_REMOVE;
                    props.Scope = DI_REMOVEDEVICE_GLOBAL;
                    props.HwProfile = 0x00;

                    if (SetupDiSetClassInstallParams(deviceInfoSet, ref deviceInterfaceData, ref props, Marshal.SizeOf(props)))
                    {
                        return SetupDiCallClassInstaller(DIF_REMOVE, deviceInfoSet, ref deviceInterfaceData);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to remove device: " + ex.Message);
                return false;
            }
            finally
            {
                if (deviceInfoSet != IntPtr.Zero)
                {
                    SetupDiDestroyDeviceInfoList(deviceInfoSet);
                }
            }
        }

        public static bool ChangePropertyDevice(string devicePath, int targetProperty)
        {
            IntPtr deviceInfoSet = IntPtr.Zero;
            try
            {
                string instanceId = ConvertPathToInstanceId(devicePath);
                deviceInfoSet = SetupDiGetClassDevs(ref GuidClassHidDevice, instanceId, 0, DIGCF_DEVICEINTERFACE);
                SP_DEVINFO_DATA deviceInfoData = new SP_DEVINFO_DATA();
                deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);

                //Get device information
                if (!SetupDiEnumDeviceInfo(deviceInfoSet, 0, ref deviceInfoData))
                {
                    Debug.WriteLine("SetupDi: Failed getting device info.");
                    return false;
                }

                //Set property change param
                SP_PROPCHANGE_PARAMS propertyParams = new SP_PROPCHANGE_PARAMS();
                propertyParams.classInstallHeader.cbSize = Marshal.SizeOf(propertyParams.classInstallHeader);
                propertyParams.classInstallHeader.installFunction = DIF_PROPERTYCHANGE;
                propertyParams.Scope = DICS_FLAG_GLOBAL;
                propertyParams.stateChange = targetProperty;

                //Prepare the device
                if (!SetupDiSetClassInstallParams(deviceInfoSet, ref deviceInfoData, ref propertyParams, Marshal.SizeOf(propertyParams)))
                {
                    Debug.WriteLine("SetupDi: Failed to set install params.");
                    return false;
                }

                //Change the property
                if (!SetupDiCallClassInstaller(DIF_PROPERTYCHANGE, deviceInfoSet, ref deviceInfoData))
                {
                    Debug.WriteLine("SetupDi: Failed to change property.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to change property: " + ex.Message);
                return false;
            }
            finally
            {
                if (deviceInfoSet != IntPtr.Zero)
                {
                    SetupDiDestroyDeviceInfoList(deviceInfoSet);
                }
            }
        }

        //Convert device path to InstanceId
        public static string ConvertPathToInstanceId(string devicePath)
        {
            try
            {
                string InstanceId = devicePath.Remove(0, devicePath.LastIndexOf("\\") + 1);
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