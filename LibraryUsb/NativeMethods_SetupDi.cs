using System;
using System.Runtime.InteropServices;
using static LibraryUsb.NativeMethods_Hid;

namespace LibraryUsb
{
    internal static class NativeMethods_SetupDi
    {
        internal const int DICS_ENABLE = 1;
        internal const int DICS_DISABLE = 2;
        internal const int DICS_PROPCHANGE = 3;
        internal const int DICS_FLAG_GLOBAL = 1;
        internal const int DIF_PROPERTYCHANGE = 0x12;

        [StructLayout(LayoutKind.Sequential)]
        internal struct SP_CLASSINSTALL_HEADER
        {
            internal int cbSize;
            internal int installFunction;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SP_PROPCHANGE_PARAMS
        {
            internal SP_CLASSINSTALL_HEADER classInstallHeader;
            internal int stateChange;
            internal int Scope;
            internal int hwProfile;
        }

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        internal static extern int CM_Get_Device_ID(int dnDevInst, IntPtr Buffer, int BufferLen, int ulFlags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        internal static extern bool SetupDiOpenDeviceInfo(IntPtr DeviceInfoSet, string DeviceInstanceId, IntPtr hWndParent, int Flags, ref SP_DEVICE_INTERFACE_DATA DeviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        internal static extern bool SetupDiChangeState(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        internal static extern bool SetupDiSetClassInstallParams(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, ref SP_PROPCHANGE_PARAMS ClassInstallParams, int ClassInstallParamsSize);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        internal static extern bool SetupDiSetClassInstallParams(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, ref SP_PROPCHANGE_PARAMS classInstallParams, int classInstallParamsSize);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        internal static extern bool SetupDiCallClassInstaller(int installFunction, IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        internal static extern bool SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, ref Guid InterfaceClassGuid, int MemberIndex, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hWndParent, int Flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        internal static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, ref int RequiredSize, ref SP_DEVICE_INTERFACE_DATA DeviceInfoData);
    }
}