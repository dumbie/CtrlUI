using System.Runtime.InteropServices;

namespace LibraryUsb
{
    public partial class TetherScriptDevice
    {
        //Driver product identifiers
        public enum DriverProductIds : ushort
        {
            TTC_PRODUCTID_JOYSTICK = 0x00000001,
            TTC_PRODUCTID_MOUSEABS = 0x00000002,
            TTC_PRODUCTID_KEYBOARD = 0x00000003,
            TTC_PRODUCTID_GAMEPAD = 0x00000004,
            TTC_PRODUCTID_MOUSEREL = 0x00000005
        }

        //Virtual Keyboard structure
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SetFeatureKeyboard
        {
            public byte ReportID;
            public byte CommandCode;
            public uint Timeout;
            public byte Modifier;
            public byte Padding;
            public byte Key0;
            public byte Key1;
            public byte Key2;
            public byte Key3;
            public byte Key4;
            public byte Key5;
        }

        //Virtual Mouse Abstract structure
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SetFeatureMouseAbs
        {
            public byte ReportID;
            public byte CommandCode;
            public byte Buttons;
            public ushort X;
            public ushort Y;
            public byte VWheelPosition;
            public byte HWheelPosition;
        }

        //Virtual Mouse Relative structure
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SetFeatureMouseRel
        {
            public byte ReportID;
            public byte CommandCode;
            public byte Buttons;
            public sbyte X;
            public sbyte Y;
            public byte VWheelPosition;
            public byte HWheelPosition;
        }
    }
}