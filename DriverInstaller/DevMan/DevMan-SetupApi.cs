using System;
using System.Runtime.InteropServices;

namespace DriverInstaller.Driver
{
    public static partial class DevMan
    {
        public static bool Install(string driverPackageInfPath, DiirFlags flag, ref bool rebootRequired)
        {
            try
            {
                return DiInstallDriver(IntPtr.Zero, driverPackageInfPath, (uint)flag, ref rebootRequired);
            }
            catch { return false; }
        }

        public static bool Uninstall(string driverPackageInfPath, DiirFlags flag, ref bool rebootRequired)
        {
            try
            {
                return DiUninstallDriver(IntPtr.Zero, driverPackageInfPath, (uint)flag, ref rebootRequired);
            }
            catch { return false; }
        }

        public static bool Create(string className, Guid classGuid, string node)
        {
            IntPtr deviceInfoSet = (IntPtr)(-1);
            SP_DEVINFO_DATA deviceInfoData = new SP_DEVINFO_DATA();
            deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);

            try
            {
                deviceInfoSet = SetupDiCreateDeviceInfoList(ref classGuid, IntPtr.Zero);
                if (deviceInfoSet == (IntPtr)(-1))
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
            }
            finally
            {
                if (deviceInfoSet != (IntPtr)(-1))
                {
                    SetupDiDestroyDeviceInfoList(deviceInfoSet);
                }
            }
            return true;
        }

        public static bool Find(Guid target, ref string path, ref string instanceId, int instance = 0)
        {
            IntPtr deviceInfoSet = IntPtr.Zero;
            IntPtr detailDataBuffer = IntPtr.Zero;

            try
            {
                int bufferSize = 0;
                int memberIndex = 0;

                SP_DEVINFO_DATA deviceInterfaceData1 = new SP_DEVINFO_DATA();
                SP_DEVINFO_DATA deviceInterfaceData2 = new SP_DEVINFO_DATA();
                deviceInterfaceData1.cbSize = deviceInterfaceData2.cbSize = Marshal.SizeOf(deviceInterfaceData1);
                deviceInfoSet = SetupDiGetClassDevs(ref target, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);

                while (SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref target, memberIndex, ref deviceInterfaceData1))
                {
                    SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData1, IntPtr.Zero, 0, ref bufferSize, ref deviceInterfaceData2);
                    {
                        detailDataBuffer = Marshal.AllocHGlobal(bufferSize);
                        Marshal.WriteInt32(detailDataBuffer, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);

                        if (SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData1, detailDataBuffer, bufferSize, ref bufferSize, ref deviceInterfaceData2))
                        {
                            IntPtr pDevicePathName = detailDataBuffer + 4;
                            path = (Marshal.PtrToStringAuto(pDevicePathName) ?? string.Empty).ToUpper();

                            if (memberIndex == instance)
                            {
                                int nBytes = 256;
                                IntPtr ptrInstanceBuf = Marshal.AllocHGlobal(nBytes);

                                CM_Get_Device_ID(deviceInterfaceData2.Flags, ptrInstanceBuf, nBytes, 0);
                                instanceId = (Marshal.PtrToStringAuto(ptrInstanceBuf) ?? string.Empty).ToUpper();

                                Marshal.FreeHGlobal(ptrInstanceBuf);
                                return true;
                            }
                        }
                        else
                        {
                            Marshal.FreeHGlobal(detailDataBuffer);
                        }
                    }
                    memberIndex++;
                }
            }
            finally
            {
                if (deviceInfoSet != IntPtr.Zero)
                {
                    SetupDiDestroyDeviceInfoList(deviceInfoSet);
                }
            }
            return false;
        }

        public static bool Remove(Guid classGuid, string path, string instanceId)
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
                    props.ClassInstallHeader.InstallFunction = DIF_REMOVE;
                    props.Scope = DI_REMOVEDEVICE_GLOBAL;
                    props.HwProfile = 0x00;

                    if (SetupDiSetClassInstallParams(deviceInfoSet, ref deviceInterfaceData, ref props, Marshal.SizeOf(props)))
                    {
                        return SetupDiCallClassInstaller(DIF_REMOVE, deviceInfoSet, ref deviceInterfaceData);
                    }
                }
            }
            finally
            {
                if (deviceInfoSet != IntPtr.Zero)
                {
                    SetupDiDestroyDeviceInfoList(deviceInfoSet);
                }
            }
            return false;
        }
    }
}