using System;
using System.Runtime.InteropServices;
using System.Security;

namespace LibraryUsb
{
    [SuppressUnmanagedCodeSecurity]
    public class NativeMethods_SetupApi
    {
        public enum DiRemoveDevice : int
        {
            DI_REMOVEDEVICE_GLOBAL = 0x00000001,
            DI_REMOVEDEVICE_CONFIGSPECIFIC = 0x00000002
        }

        public enum DiFunction : int
        {
            DIF_SELECTDEVICE = 0x0001,
            DIF_INSTALLDEVICE = 0x0002,
            DIF_ASSIGNRESOURCES = 0x0003,
            DIF_PROPERTIES = 0x0004,
            DIF_REMOVE = 0x0005,
            DIF_FIRSTTIMESETUP = 0x0006,
            DIF_FOUNDDEVICE = 0x0007,
            DIF_SELECTCLASSDRIVERS = 0x0008,
            DIF_VALIDATECLASSDRIVERS = 0x0009,
            DIF_INSTALLCLASSDRIVERS = 0x000A,
            DIF_CALCDISKSPACE = 0x000B,
            DIF_DESTROYPRIVATEDATA = 0x000C,
            DIF_VALIDATEDRIVER = 0x000D,
            DIF_MOVEDEVICE = 0x000E,
            DIF_DETECT = 0x000F,
            DIF_INSTALLWIZARD = 0x0010,
            DIF_DESTROYWIZARDDATA = 0x0011,
            DIF_PROPERTYCHANGE = 0x0012,
            DIF_ENABLECLASS = 0x0013,
            DIF_DETECTVERIFY = 0x0014,
            DIF_INSTALLDEVICEFILES = 0x0015,
            DIF_UNREMOVE = 0x0016,
            DIF_SELECTBESTCOMPATDRV = 0x0017,
            DIF_ALLOW_INSTALL = 0x0018,
            DIF_REGISTERDEVICE = 0x0019
        }

        public enum DiChangeState : int
        {
            DICS_ENABLE = 0x00000001,
            DICS_DISABLE = 0x00000002,
            DICS_PROPCHANGE = 0x00000003,
            DICS_START = 0x00000004,
            DICS_STOP = 0x00000005
        }

        public enum DiChangeStateFlag : int
        {
            DICS_FLAG_GLOBAL = 0x00000001,
            DICS_FLAG_CONFIGSPECIFIC = 0x00000002,
            DICS_FLAG_CONFIGGENERAL = 0x00000004
        }

        public enum DiGetClassFlag : int
        {
            DIGCF_DEFAULT = 0x0001,
            DIGCF_PRESENT = 0x0002,
            DIGCF_ALLCLASSES = 0x0004,
            DIGCF_PROFILE = 0x0008,
            DIGCF_DEVICEINTERFACE = 0x0010
        }

        public enum DiOpenDevice : int
        {
            DIOD_NONE = 0x00000000,
            DIOD_INHERIT_CLASSDRVS = 0x00000002,
            DIOD_CANCEL_REMOVE = 0x00000004
        }

        public enum DiCreateDevice : int
        {
            DICD_GENERATE_ID = 0x00000001,
            DICD_INHERIT_CLASSDRVS = 0x00000002
        }

        public enum DiDeviceRegistryProperty : int
        {
            SPDRP_DEVICEDESC = 0x00000000,
            SPDRP_HARDWAREID = 0x00000001,
            SPDRP_COMPATIBLEIDS = 0x00000002,
            SPDRP_UNUSED0 = 0x00000003,
            SPDRP_SERVICE = 0x00000004,
            SPDRP_UNUSED1 = 0x00000005,
            SPDRP_UNUSED2 = 0x00000006,
            SPDRP_CLASS = 0x00000007,
            SPDRP_CLASSGUID = 0x00000008,
            SPDRP_DRIVER = 0x00000009,
            SPDRP_CONFIGFLAGS = 0x0000000A,
            SPDRP_MFG = 0x0000000B,
            SPDRP_FRIENDLYNAME = 0x0000000C,
            SPDRP_LOCATION_INFORMATION = 0x0000000D,
            SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 0x0000000E,
            SPDRP_CAPABILITIES = 0x0000000F,
            SPDRP_UI_NUMBER = 0x00000010,
            SPDRP_UPPERFILTERS = 0x00000011,
            SPDRP_LOWERFILTERS = 0x00000012,
            SPDRP_BUSTYPEGUID = 0x00000013,
            SPDRP_LEGACYBUSTYPE = 0x00000014,
            SPDRP_BUSNUMBER = 0x00000015,
            SPDRP_ENUMERATOR_NAME = 0x00000016,
            SPDRP_SECURITY = 0x00000017,
            SPDRP_SECURITY_SDS = 0x00000018,
            SPDRP_DEVTYPE = 0x00000019,
            SPDRP_EXCLUSIVE = 0x0000001A,
            SPDRP_CHARACTERISTICS = 0x0000001B,
            SPDRP_ADDRESS = 0x0000001C,
            SPDRP_UI_NUMBER_DESC_FORMAT = 0X0000001D,
            SPDRP_DEVICE_POWER_DATA = 0x0000001E,
            SPDRP_REMOVAL_POLICY = 0x0000001F,
            SPDRP_REMOVAL_POLICY_HW_DEFAULT = 0x00000020,
            SPDRP_REMOVAL_POLICY_OVERRIDE = 0x00000021,
            SPDRP_INSTALL_STATE = 0x00000022,
            SPDRP_LOCATION_PATHS = 0x00000023,
            SPDRP_BASE_CONTAINERID = 0x00000024,
            SPDRP_MAXIMUM_PROPERTY = 0x00000025
        }

        public enum DIIRFLAG : uint
        {
            DIIRFLAG_INF_ALREADY_COPIED = 0x00000001,
            DIIRFLAG_FORCE_INF = 0x00000002,
            DIIRFLAG_HW_USING_THE_INF = 0x00000004,
            DIIRFLAG_HOTPATCH = 0x00000008,
            DIIRFLAG_NOBACKUP = 0x00000010
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_CLASSINSTALL_HEADER
        {
            public int cbSize;
            public DiFunction installFunction;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_PROPCHANGE_PARAMS
        {
            public SP_CLASSINSTALL_HEADER classInstallHeader;
            public DiChangeState stateChange;
            public DiChangeStateFlag changeStateFlag;
            public int hwProfile;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVICE_INFO_DATA
        {
            public int cbSize;
            public Guid ClassGuid;
            public int DevInst;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public Guid InterfaceClassGuid;
            public int Flags;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public int cbSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }

        public static DEVPROPKEY DEVPKEY_Device_BusReportedDeviceDesc = new DEVPROPKEY { fmtId = new Guid("540B947E-8B40-45BC-A8A2-6A0B894CBDA2"), pId = 4 };
        [StructLayout(LayoutKind.Sequential)]
        public struct DEVPROPKEY
        {
            public Guid fmtId;
            public uint pId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_REMOVEDEVICE_PARAMS
        {
            public SP_CLASSINSTALL_HEADER classInstallHeader;
            public DiRemoveDevice removeDevice;
            public int hwProfile;
        }

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiSetClassInstallParams(IntPtr deviceInfoList, ref SP_DEVICE_INFO_DATA deviceInfoData, ref SP_PROPCHANGE_PARAMS classInstallParams, int classInstallParamsSize);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiCallClassInstaller(DiFunction installFunction, IntPtr deviceInfoList, ref SP_DEVICE_INFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiGetClassDevs(Guid classGuid, string enumerator, IntPtr hWndParent, DiGetClassFlag diFlags);

        [DllImport("setupapi.dll", EntryPoint = "SetupDiGetDeviceRegistryProperty")]
        public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoList, ref SP_DEVICE_INFO_DATA deviceInfoData, DiDeviceRegistryProperty propertyVal, ref int propertyRegDataType, byte[] propertyBuffer, int propertyBufferSize, ref int requiredSize);

        [DllImport("setupapi.dll", EntryPoint = "SetupDiGetDevicePropertyW")]
        public static extern bool SetupDiGetDeviceProperty(IntPtr deviceInfo, ref SP_DEVICE_INFO_DATA deviceInfoData, ref DEVPROPKEY propKey, ref ulong propertyDataType, byte[] propertyBuffer, int propertyBufferSize, ref int requiredSize, uint flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiEnumDeviceInfo(IntPtr deviceInfoList, int memberIndex, ref SP_DEVICE_INFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern int SetupDiDestroyDeviceInfoList(IntPtr deviceInfoList);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoList, IntPtr deviceInfoData, Guid interfaceClassGuid, int memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoList, SP_DEVICE_INFO_DATA deviceInfoData, Guid interfaceClassGuid, int memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoList, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, IntPtr deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoList, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, IntPtr deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoList, ref SP_DEVICE_INFO_DATA deviceInfoData, byte[] DeviceInstanceId, int DeviceInstanceIdSize, out int RequiredSize);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiOpenDeviceInfo(IntPtr deviceInfoList, string DeviceInstanceId, IntPtr hWndParent, DiOpenDevice flags, ref SP_DEVICE_INFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiSetClassInstallParams(IntPtr deviceInfoList, ref SP_DEVICE_INFO_DATA deviceInfoData, ref SP_REMOVEDEVICE_PARAMS ClassInstallParams, int ClassInstallParamsSize);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiCreateDeviceInfoList(ref Guid ClassGuid, IntPtr hWndParent);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiCreateDeviceInfo(IntPtr deviceInfoList, string DeviceName, ref Guid ClassGuid, string DeviceDescription, IntPtr hWndParent, DiCreateDevice flags, ref SP_DEVICE_INFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiSetDeviceRegistryProperty(IntPtr deviceInfoList, ref SP_DEVICE_INFO_DATA deviceInfoData, DiDeviceRegistryProperty Property, [MarshalAs(UnmanagedType.LPWStr)] string PropertyBuffer, int PropertyBufferSize);

        [DllImport("newdev.dll", CharSet = CharSet.Auto)]
        public static extern bool DiInstallDriver(IntPtr hWndParent, string DriverPackageInfPath, DIIRFLAG Flags, ref bool RebootRequired);

        [DllImport("newdev.dll", CharSet = CharSet.Auto)]
        public static extern bool DiUninstallDriver(IntPtr hWndParent, string DriverPackageInfPath, DIIRFLAG Flags, ref bool RebootRequired);
    }
}