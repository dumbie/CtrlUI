using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static LibraryUsb.Enumerate;
using static LibraryUsb.NativeMethods_SetupApi;

namespace LibraryUsb
{
    public class DeviceManager
    {
        public static bool DriverInstallInf(string driverPackageInfPath, DIIRFLAG flag, ref bool rebootRequired)
        {
            try
            {
                return DiInstallDriver(IntPtr.Zero, driverPackageInfPath, flag, ref rebootRequired);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to install inf driver: " + ex.Message);
                return false;
            }
        }

        public static bool DriverUninstallInf(string driverPackageInfPath, DIIRFLAG flag, ref bool rebootRequired)
        {
            try
            {
                return DiUninstallDriver(IntPtr.Zero, driverPackageInfPath, flag, ref rebootRequired);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to uninstall inf driver: " + ex.Message);
                return false;
            }
        }

        public static bool DeviceCreate(string className, Guid classGuid, string propertyNode)
        {
            IntPtr deviceInfoSet = IntPtr.Zero;
            try
            {
                SP_DEVICE_INFO_DATA deviceInfoData = new SP_DEVICE_INFO_DATA();
                deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);
                deviceInfoSet = SetupDiCreateDeviceInfoList(ref classGuid, IntPtr.Zero);
                if (deviceInfoSet == IntPtr.Zero)
                {
                    return false;
                }

                if (!SetupDiCreateDeviceInfo(deviceInfoSet, className, ref classGuid, null, IntPtr.Zero, DiCreateDevice.DICD_GENERATE_ID, ref deviceInfoData))
                {
                    return false;
                }

                if (!SetupDiSetDeviceRegistryProperty(deviceInfoSet, ref deviceInfoData, DiDeviceRegistryProperty.SPDRP_HARDWAREID, propertyNode, propertyNode.Length * 2))
                {
                    return false;
                }

                if (!SetupDiCallClassInstaller(DiFunction.DIF_REGISTERDEVICE, deviceInfoSet, ref deviceInfoData))
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
            IntPtr deviceInfoList = IntPtr.Zero;
            try
            {
                int memberIndex = 0;
                SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
                deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);

                deviceInfoList = SetupDiGetClassDevs(ref deviceGuid, string.Empty, IntPtr.Zero, DiGetClassFlag.DIGCF_PRESENT | DiGetClassFlag.DIGCF_DEVICEINTERFACE);
                while (SetupDiEnumDeviceInterfaces(deviceInfoList, IntPtr.Zero, ref deviceGuid, memberIndex, ref deviceInterfaceData))
                {
                    try
                    {
                        if (memberIndex == instance)
                        {
                            devicePath = GetDevicePath(deviceInfoList, deviceInterfaceData);
                            return true;
                        }
                        memberIndex++;
                    }
                    catch { }
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
                if (deviceInfoList != IntPtr.Zero)
                {
                    SetupDiDestroyDeviceInfoList(deviceInfoList);
                }
            }
        }

        public static bool DeviceRemove(Guid classGuid, string instanceId)
        {
            IntPtr deviceInfoList = IntPtr.Zero;
            try
            {
                SP_DEVICE_INFO_DATA deviceInterfaceData = new SP_DEVICE_INFO_DATA();
                deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);

                deviceInfoList = SetupDiGetClassDevs(ref classGuid, string.Empty, IntPtr.Zero, DiGetClassFlag.DIGCF_PRESENT | DiGetClassFlag.DIGCF_DEVICEINTERFACE);
                if (SetupDiOpenDeviceInfo(deviceInfoList, instanceId, IntPtr.Zero, 0, ref deviceInterfaceData))
                {
                    SP_REMOVEDEVICE_PARAMS props = new SP_REMOVEDEVICE_PARAMS();
                    props.classInstallHeader = new SP_CLASSINSTALL_HEADER();
                    props.classInstallHeader.cbSize = Marshal.SizeOf(props.classInstallHeader);
                    props.classInstallHeader.installFunction = DiFunction.DIF_REMOVE;
                    props.removeDevice = DiRemoveDevice.DI_REMOVEDEVICE_GLOBAL;
                    props.hwProfile = 0x00;

                    if (SetupDiSetClassInstallParams(deviceInfoList, ref deviceInterfaceData, ref props, Marshal.SizeOf(props)))
                    {
                        return SetupDiCallClassInstaller(DiFunction.DIF_REMOVE, deviceInfoList, ref deviceInterfaceData);
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
                if (deviceInfoList != IntPtr.Zero)
                {
                    SetupDiDestroyDeviceInfoList(deviceInfoList);
                }
            }
        }

        public static bool ChangePropertyDevice(Guid guidClass, string deviceInstanceId, DiChangeState changeState)
        {
            IntPtr deviceInfoList = IntPtr.Zero;
            try
            {
                SP_DEVICE_INFO_DATA deviceInfoData = new SP_DEVICE_INFO_DATA();
                deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);

                //Get device information
                deviceInfoList = SetupDiGetClassDevs(ref guidClass, deviceInstanceId, IntPtr.Zero, DiGetClassFlag.DIGCF_DEVICEINTERFACE);
                if (!SetupDiEnumDeviceInfo(deviceInfoList, 0, ref deviceInfoData))
                {
                    Debug.WriteLine("SetupDi: Failed getting device info.");
                    return false;
                }

                //Set property change param
                SP_PROPCHANGE_PARAMS propertyParams = new SP_PROPCHANGE_PARAMS();
                propertyParams.classInstallHeader.cbSize = Marshal.SizeOf(propertyParams.classInstallHeader);
                propertyParams.classInstallHeader.installFunction = DiFunction.DIF_PROPERTYCHANGE;
                propertyParams.changeStateFlag = DiChangeStateFlag.DICS_FLAG_GLOBAL;
                propertyParams.stateChange = changeState;

                //Prepare the device
                if (!SetupDiSetClassInstallParams(deviceInfoList, ref deviceInfoData, ref propertyParams, Marshal.SizeOf(propertyParams)))
                {
                    Debug.WriteLine("SetupDi: Failed to set install params.");
                    return false;
                }

                //Change the property
                if (!SetupDiCallClassInstaller(DiFunction.DIF_PROPERTYCHANGE, deviceInfoList, ref deviceInfoData))
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
                if (deviceInfoList != IntPtr.Zero)
                {
                    SetupDiDestroyDeviceInfoList(deviceInfoList);
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