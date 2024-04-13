namespace LibraryUsb
{
    public partial class VigemBusDevice : WinUsbDevice
    {
        public enum IO_FUNCTION : uint
        {
            IOCTL_BUSENUM_BASE = 0x801,
            IOCTL_BUSENUM_PLUGIN_HARDWARE = IOCTL_BUSENUM_BASE + 0x0,
            IOCTL_BUSENUM_UNPLUG_HARDWARE = IOCTL_BUSENUM_BASE + 0x1,
            IOCTL_XUSB_REQUEST_NOTIFICATION = IOCTL_BUSENUM_BASE + 0x200,
            IOCTL_XUSB_SUBMIT_REPORT = IOCTL_BUSENUM_BASE + 0x201
        }

        public enum IO_METHOD : uint
        {
            METHOD_BUFFERED = 0,
            METHOD_IN_DIRECT = 1,
            METHOD_OUT_DIRECT = 2,
            METHOD_NEITHER = 3
        }

        public enum FILE_DEVICE_TYPE : uint
        {
            FILE_DEVICE_BUS_EXTENDER = 0x0000002A
        }

        public enum FILE_ACCESS_DATA : uint
        {
            FILE_READ_DATA = 0x0001,
            FILE_WRITE_DATA = 0x0002,
            FILE_APPEND_DATA = 0x0004
        }

        public uint CTL_CODE(FILE_DEVICE_TYPE DeviceType, FILE_ACCESS_DATA Access, IO_FUNCTION Function, IO_METHOD Method)
        {
            return (((uint)DeviceType) << 16) | (((uint)Access) << 14) | (((uint)Function) << 2) | ((uint)Method);
        }

        public enum VIGEM_TARGET_TYPE : uint
        {
            Xbox360Wired = 0,
            XboxOneWired = 1,
            DualShock4Wired = 2
        }

        public enum XUSB_BUTTON : ushort
        {
            XUSB_GAMEPAD_DPAD_UP = 0x0001,
            XUSB_GAMEPAD_DPAD_DOWN = 0x0002,
            XUSB_GAMEPAD_DPAD_LEFT = 0x0004,
            XUSB_GAMEPAD_DPAD_RIGHT = 0x0008,
            XUSB_GAMEPAD_START = 0x0010,
            XUSB_GAMEPAD_BACK = 0x0020,
            XUSB_GAMEPAD_LEFT_THUMB = 0x0040,
            XUSB_GAMEPAD_RIGHT_THUMB = 0x0080,
            XUSB_GAMEPAD_LEFT_SHOULDER = 0x0100,
            XUSB_GAMEPAD_RIGHT_SHOULDER = 0x0200,
            XUSB_GAMEPAD_GUIDE = 0x0400,
            XUSB_GAMEPAD_A = 0x1000,
            XUSB_GAMEPAD_B = 0x2000,
            XUSB_GAMEPAD_X = 0x4000,
            XUSB_GAMEPAD_Y = 0x8000
        }

        public struct BUSENUM_PLUGIN_HARDWARE
        {
            public int Size;
            public int SerialNo;
            public VIGEM_TARGET_TYPE TargetType;
            public ushort VendorId;
            public ushort ProductId;
        }

        public struct BUSENUM_UNPLUG_HARDWARE
        {
            public int Size;
            public int SerialNo;
        }

        public struct XUSB_INPUT_REPORT
        {
            public int Size;
            public int SerialNo;
            public XUSB_REPORT Report;
        }

        public struct XUSB_REPORT
        {
            public XUSB_BUTTON wButtons;
            public byte bLeftTrigger;
            public byte bRightTrigger;
            public short sThumbLX;
            public short sThumbLY;
            public short sThumbRX;
            public short sThumbRY;
        }

        public struct XUSB_OUTPUT_REPORT
        {
            public int Size;
            public int SerialNo;
            public byte RumbleHeavy;
            public byte RumbleLight;
            public byte LedNumber;
        }
    }
}