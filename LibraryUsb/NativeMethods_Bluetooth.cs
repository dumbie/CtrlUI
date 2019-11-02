using System;
using System.Runtime.InteropServices;

namespace LibraryUsb
{
    internal static class NativeMethods_Bluetooth
    {
        //internal static readonly Guid HumanInterfaceDeviceServiceClass_UUID = new Guid(0x00001124, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5F, 0x9B, 0x34, 0xFB);

        internal const int IOCTL_BTH_DISCONNECT_DEVICE = 0x41000c;
        internal const int IOCTL_HID_ACTIVATE_DEVICE = 0xb001f;
        internal const int IOCTL_HID_DEACTIVATE_DEVICE = 0xb0023;

        internal const int BLUETOOTH_MAX_NAME_SIZE = 248;
        internal const int BLUETOOTH_SERVICE_DISABLE = 0;
        internal const int BLUETOOTH_SERVICE_ENABLE = 1;

        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct BLUETOOTH_ADDRESS
        {
            public byte byte1;
            public byte byte2;
            public byte byte3;
            public byte byte4;
            public byte byte5;
            public byte byte6;
            public byte bytex1;
            public byte bytex2;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct BLUETOOTH_DEVICE_INFO
        {
            public int dwSize;
            public BLUETOOTH_ADDRESS Address;
            public uint ulClassofDevice;
            public bool fConnected;
            public bool fRemembered;
            public bool fAuthenticated;
            public SYSTEMTIME stLastSeen;
            public SYSTEMTIME stLastUsed;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BLUETOOTH_MAX_NAME_SIZE)]
            public string szName;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct BLUETOOTH_DEVICE_SEARCH_PARAMS
        {
            public int dwSize;
            public bool fReturnAuthenticated;
            public bool fReturnRemembered;
            public bool fReturnUnknown;
            public bool fReturnConnected;
            public bool fIssueInquiry;
            public byte cTimeoutMultiplier;
            public IntPtr hRadio;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct BLUETOOTH_FIND_RADIO_PARAMS
        {
            public int dwSize;
        }

        [DllImport("bthprops.cpl")]
        internal static extern IntPtr BluetoothFindFirstRadio(ref BLUETOOTH_FIND_RADIO_PARAMS pBtRadioParam, ref IntPtr phRadio);

        [DllImport("bthprops.cpl")]
        internal static extern bool BluetoothFindNextRadio(IntPtr hFind, ref IntPtr phRadio);

        [DllImport("bthprops.cpl")]
        internal static extern bool BluetoothFindRadioClose(IntPtr hFind);

        [DllImport("bthprops.cpl")]
        internal static extern IntPtr BluetoothFindFirstDevice(ref BLUETOOTH_DEVICE_SEARCH_PARAMS SearchParams, ref BLUETOOTH_DEVICE_INFO DeviceInfo);

        [DllImport("bthprops.cpl")]
        internal static extern bool BluetoothFindNextDevice(IntPtr hFind, ref BLUETOOTH_DEVICE_INFO DeviceInfo);

        [DllImport("bthprops.cpl")]
        internal static extern bool BluetoothFindDeviceClose(IntPtr hFind);

        [DllImport("bthprops.cpl")]
        internal static extern uint BluetoothSetServiceState(IntPtr hRadio, ref BLUETOOTH_DEVICE_INFO DeviceInfo, ref Guid guid, int ServiceFlags);

        [DllImport("bthprops.cpl")]
        internal static extern uint BluetoothRemoveDevice(ref BLUETOOTH_ADDRESS Address);
    }
}