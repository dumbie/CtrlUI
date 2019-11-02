using System;
using System.Runtime.InteropServices;

namespace DriverInstaller.Driver
{
    public static partial class DevMan
    {
        internal const short DIGCF_PRESENT = 0x2;
        internal const short DIGCF_DEVICEINTERFACE = 0x10;
        internal const short DIGCF_ALLCLASSES = 0x4;
        internal const int DICD_GENERATE_ID = 0x0001;
        internal const int SPDRP_HARDWAREID = 0x0001;
        internal const int DIF_REMOVE = 0x0005;
        internal const int DIF_REGISTERDEVICE = 0x0019;
        internal const int DI_REMOVEDEVICE_GLOBAL = 0x0001;

        [Flags]
        public enum DiirFlags : uint
        {
            DIIRFLAG_INF_ALREADY_COPIED = 0x00000001,
            DIIRFLAG_FORCE_INF = 0x00000002,
            DIIRFLAG_HW_USING_THE_INF = 0x00000004,
            DIIRFLAG_HOTPATCH = 0x00000008,
            DIIRFLAG_NOBACKUP = 0x00000010
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVINFO_DATA
        {
            internal int cbSize;
            internal readonly Guid ClassGuid;
            internal readonly int Flags;
            internal readonly IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_CLASSINSTALL_HEADER
        {
            internal int cbSize;
            internal int InstallFunction;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_REMOVEDEVICE_PARAMS
        {
            internal SP_CLASSINSTALL_HEADER ClassInstallHeader;
            internal int Scope;
            internal int HwProfile;
        }

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern int CM_Get_Device_ID(int DevInst, IntPtr Buffer, int BufferLen, int Flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hWndParent, int Flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, ref Guid InterfaceClassGuid, int MemberIndex, ref SP_DEVINFO_DATA DeviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, ref int RequiredSize, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiOpenDeviceInfo(IntPtr DeviceInfoSet, string DeviceInstanceId, IntPtr hWndParent, int Flags, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiSetClassInstallParams(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInterfaceData, ref SP_REMOVEDEVICE_PARAMS ClassInstallParams, int ClassInstallParamsSize);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiCreateDeviceInfoList(ref Guid ClassGuid, IntPtr hWndParent);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiCreateDeviceInfo(IntPtr DeviceInfoSet, string DeviceName, ref Guid ClassGuid, string DeviceDescription, IntPtr hWndParent, int CreationFlags, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiSetDeviceRegistryProperty(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, int Property, [MarshalAs(UnmanagedType.LPWStr)] string PropertyBuffer, int PropertyBufferSize);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiCallClassInstaller(int InstallFunction, IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("newdev.dll", CharSet = CharSet.Auto)]
        public static extern bool DiInstallDriver(IntPtr hWndParent, string DriverPackageInfPath, uint Flags, ref bool RebootRequired);

        [DllImport("newdev.dll", CharSet = CharSet.Auto)]
        public static extern bool DiUninstallDriver(IntPtr hWndParent, string DriverPackageInfPath, uint Flags, ref bool RebootRequired);
    }
}