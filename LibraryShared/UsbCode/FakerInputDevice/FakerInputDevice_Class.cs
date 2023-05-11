using System.Runtime.InteropServices;

namespace LibraryUsb
{
    public partial class FakerInputDevice
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct FAKERINPUT_CONTROL_REPORT_HEADER
        {
            public byte ReportID;
            public byte ReportLength;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FAKERINPUT_KEYBOARD_REPORT
        {
            public byte ReportID;
            public byte ModifierCodes;
            public byte Reserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = KBD_KEY_CODES)]
            public byte[] KeyCodes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FAKERINPUT_MULTIMEDIA_REPORT
        {
            public byte ReportID;
            public byte MultimediaKey0;
            public byte MultimediaKey1;
            public byte MultimediaKey2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FAKERINPUT_RELATIVE_MOUSE_REPORT
        {
            public byte ReportID;
            public byte Button;
            public short XValue;
            public short YValue;
            public byte VWheelPosition;
            public byte HWheelPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FAKERINPUT_ABSOLUTE_MOUSE_REPORT
        {
            public byte ReportID;
            public byte Button;
            public ushort XValue;
            public ushort YValue;
            public byte VWheelPosition;
            public byte HWheelPosition;
        }

        public enum FAKERINPUT_REPORT_ID : byte
        {
            REPORTID_KEYBOARD = 0x01,
            REPORTID_MULTIMEDIA = 0x02,
            REPORTID_RELATIVE_MOUSE = 0x03,
            REPORTID_ABSOLUTE_MOUSE = 0x04,
            REPORTID_CONTROL = 0x40
        }
    }
}