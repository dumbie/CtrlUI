using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LibraryUsb
{
    public class HidDeviceButtonValues
    {
        private static string GetUsagePageName(ushort usagePage)
        {
            return DictionaryUsagePageName.ContainsKey(usagePage) ? DictionaryUsagePageName[usagePage] : $"{usagePage:X4}";
        }

        private static string GetUsageGenericName(ushort usagePage, ushort usageGeneric)
        {
            uint extendedUsageGeneric = (uint)(usagePage << 16) | usageGeneric;
            return DictionaryUsageGenericName.ContainsKey(extendedUsageGeneric) ? DictionaryUsageGenericName[extendedUsageGeneric] : $"{usagePage:X4} {usageGeneric:X4}";
        }

        private static readonly Dictionary<ushort, string> DictionaryUsagePageName = new Dictionary<ushort, string>()
        {
            { 0x0000, "Undefined Page" },
            { 0x0001, "Generic Page" },
            { 0x0002, "Simulation Page" },
            { 0x0003, "VR Page" },
            { 0x0004, "Sport Page" },
            { 0x0005, "Game Page" },
            { 0x0006, "Generic Device Page" },
            { 0x0007, "Keyboard Page" },
            { 0x0008, "LED Page" },
            { 0x0009, "Button Page" }
        };

        private static readonly Dictionary<uint, string> DictionaryUsageGenericName = new Dictionary<uint, string>()
        {
            { 0x0001_0000, "Undefined" },
            { 0x0001_0001, "Pointer" },
            { 0x0001_0002, "Mouse" },
            { 0x0001_0004, "Joystick" },
            { 0x0001_0005, "Game Pad" },
            { 0x0001_0006, "Keyboard" },
            { 0x0001_0007, "Keypad" },
            { 0x0001_0008, "Multi-axis Controller" },
            { 0x0001_0009, "Tablet PC System Controls" },
            { 0x0001_0030, "LX" },
            { 0x0001_0031, "LY" },
            { 0x0001_0032, "LZ" },
            { 0x0001_0033, "RX" },
            { 0x0001_0034, "RY" },
            { 0x0001_0035, "RZ" },
            { 0x0001_0039, "Hat Switch" }
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct ButtonValueCaps
        {
            [FieldOffset(0)]
            public ushort UsagePage;
            public string UsagePageName => GetUsagePageName(this.UsagePage);
            [FieldOffset(2)]
            public byte ReportID;
            [FieldOffset(3), MarshalAs(UnmanagedType.U1)]
            public bool IsAlias;
            [FieldOffset(4)]
            public ushort BitField;
            [FieldOffset(6)]
            public ushort LinkCollection;
            [FieldOffset(8)]
            public ushort LinkUsage;
            [FieldOffset(10)]
            public ushort LinkUsagePage;
            public string LinkUsageName => GetUsageGenericName(this.LinkUsagePage, this.LinkUsage);
            [FieldOffset(12), MarshalAs(UnmanagedType.U1)]
            public bool IsRange;
            [FieldOffset(13), MarshalAs(UnmanagedType.U1)]
            public bool IsStringRange;
            [FieldOffset(14), MarshalAs(UnmanagedType.U1)]
            public bool IsDesignatorRange;
            [FieldOffset(15), MarshalAs(UnmanagedType.U1)]
            public bool IsAbsolute;
            [FieldOffset(16), MarshalAs(UnmanagedType.U1)]
            public bool HasNull;
            [FieldOffset(17)]
            public byte Reserved;
            [FieldOffset(18)]
            public ushort BitSize;
            [FieldOffset(20)]
            public ushort ReportCount;
            [FieldOffset(32)]
            public uint UnitsExp;
            [FieldOffset(36)]
            public uint Units;
            [FieldOffset(40)]
            public int LogicalMin;
            [FieldOffset(44)]
            public int LogicalMax;
            [FieldOffset(48)]
            public int PhysicalMin;
            [FieldOffset(52)]
            public int PhysicalMax;
            [FieldOffset(56)]
            public ButtonValueCapsRange Range;
            [FieldOffset(56)]
            public ButtonValueCapsNotRange NotRange;
            public string NotRangeUsageName => this.IsRange ? string.Empty : GetUsageGenericName(this.UsagePage, this.NotRange.UsageGeneric);
            public ushort UsageMin => this.IsRange ? this.Range.UsageMin : this.NotRange.UsageGeneric;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ButtonValueCapsRange
        {
            public ushort UsageMin;
            public ushort UsageMax;
            public ushort StringMin;
            public ushort StringMax;
            public ushort DesignatorMin;
            public ushort DesignatorMax;
            public ushort DataIndexMin;
            public ushort DataIndexMax;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ButtonValueCapsNotRange
        {
            public ushort UsageGeneric;
            public ushort Reserved1;
            public ushort StringIndex;
            public ushort Reserved2;
            public ushort DesignatorIndex;
            public ushort Reserved3;
            public ushort DataIndex;
            public ushort Reserved4;
        }
    }
}