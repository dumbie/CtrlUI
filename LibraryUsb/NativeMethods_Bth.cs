using System;
using System.Runtime.InteropServices;

namespace LibraryUsb
{
    public class NativeMethods_Bth
    {
        public const int BLUETOOTH_MAX_NAME_SIZE = 248;

        public enum SERVICE_FLAGS : uint
        {
            BLUETOOTH_SERVICE_DISABLE = 0x00,
            BLUETOOTH_SERVICE_ENABLE = 0x01
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
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
        public struct BLUETOOTH_ADDRESS
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
        public struct BLUETOOTH_DEVICE_INFO
        {
            public int dwSize;
            public BLUETOOTH_ADDRESS address;
            public uint ulClassofDevice;
            public bool fConnected;
            public bool fRemembered;
            public bool fAuthenticated;
            public SYSTEMTIME stLastSeen;
            public SYSTEMTIME stLastUsed;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BLUETOOTH_MAX_NAME_SIZE)]
            public string szName;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct BLUETOOTH_RADIO_INFO
        {
            public int dwSize;
            public BLUETOOTH_ADDRESS address;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BLUETOOTH_MAX_NAME_SIZE)]
            public string szName;
            public uint ulClassOfDevice;
            public ushort lmpSubversion;
            public ushort manufacturer;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BLUETOOTH_DEVICE_SEARCH_PARAMS
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
        public struct BLUETOOTH_FIND_RADIO_PARAMS
        {
            public int dwSize;
        }

        [DllImport("bthprops.cpl")]
        public static extern bool BluetoothGetRadioInfo(IntPtr hRadio, ref BLUETOOTH_RADIO_INFO pRadioInfo);

        [DllImport("bthprops.cpl")]
        public static extern IntPtr BluetoothFindFirstRadio(ref BLUETOOTH_FIND_RADIO_PARAMS pBtRadioParam, ref IntPtr phRadio);

        [DllImport("bthprops.cpl")]
        public static extern bool BluetoothFindNextRadio(IntPtr hFind, ref IntPtr phRadio);

        [DllImport("bthprops.cpl")]
        public static extern bool BluetoothFindRadioClose(IntPtr hFind);

        [DllImport("bthprops.cpl")]
        public static extern IntPtr BluetoothFindFirstDevice(ref BLUETOOTH_DEVICE_SEARCH_PARAMS SearchParams, ref BLUETOOTH_DEVICE_INFO DeviceInfo);

        [DllImport("bthprops.cpl")]
        public static extern bool BluetoothFindNextDevice(IntPtr hFind, ref BLUETOOTH_DEVICE_INFO DeviceInfo);

        [DllImport("bthprops.cpl")]
        public static extern bool BluetoothFindDeviceClose(IntPtr hFind);

        [DllImport("bthprops.cpl")]
        public static extern uint BluetoothSetServiceState(IntPtr hRadio, ref BLUETOOTH_DEVICE_INFO DeviceInfo, ref Guid guid, SERVICE_FLAGS ServiceFlags);

        [DllImport("bthprops.cpl")]
        public static extern uint BluetoothRemoveDevice(ref BLUETOOTH_ADDRESS Address);
    }
}