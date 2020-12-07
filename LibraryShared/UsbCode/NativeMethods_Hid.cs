using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using static LibraryUsb.HidDeviceAttributes;
using static LibraryUsb.HidDeviceButtonValues;
using static LibraryUsb.HidDeviceCapabilities;

namespace LibraryUsb
{
    public class NativeMethods_Hid
    {
        public enum HIDP_REPORT_TYPE : int
        {
            HidP_Input = 0,
            HidP_Output = 1,
            HidP_Feature = 2
        }

        public enum HID_USAGE_PAGE : byte
        {
            HID_USAGE_PAGE_UNDEFINED = 0x00,
            HID_USAGE_PAGE_GENERIC = 0x01,
            HID_USAGE_PAGE_SIMULATION = 0x02,
            HID_USAGE_PAGE_VR = 0x03,
            HID_USAGE_PAGE_SPORT = 0x04,
            HID_USAGE_PAGE_GAME = 0x05,
            HID_USAGE_PAGE_GENERIC_DEVICE = 0x06,
            HID_USAGE_PAGE_KEYBOARD = 0x07,
            HID_USAGE_PAGE_LED = 0x08,
            HID_USAGE_PAGE_BUTTON = 0x09
        }

        public enum HID_USAGE_GENERIC_DESKTOP_PAGE : byte
        {
            HID_USAGE_GENERIC_UNDEFINED = 0x00,
            HID_USAGE_GENERIC_POINTER = 0x01,
            HID_USAGE_GENERIC_MOUSE = 0x02,
            HID_USAGE_GENERIC_JOYSTICK = 0x04,
            HID_USAGE_GENERIC_GAMEPAD = 0x05,
            HID_USAGE_GENERIC_KEYBOARD = 0x06,
            HID_USAGE_GENERIC_KEYPAD = 0x07,
            HID_USAGE_GENERIC_MULTI_AXIS_CONTROLLER = 0x08,
            HID_USAGE_GENERIC_LX = 0x30,
            HID_USAGE_GENERIC_LY = 0x31,
            HID_USAGE_GENERIC_LZ = 0x32,
            HID_USAGE_GENERIC_RX = 0x33,
            HID_USAGE_GENERIC_RY = 0x34,
            HID_USAGE_GENERIC_RZ = 0x35,
            HID_USAGE_GENERIC_HATSWITCH = 0x39
        }

        [DllImport("hid.dll")]
        public static extern bool HidD_GetAttributes(SafeFileHandle hidDeviceObject, ref HIDD_ATTRIBUTES attributes);

        [DllImport("hid.dll")]
        public static extern bool HidD_GetFeature(SafeFileHandle hidDeviceObject, byte[] lpReportBuffer, int reportBufferLength);

        [DllImport("hid.dll")]
        public static extern bool HidD_SetFeature(SafeFileHandle hidDeviceObject, byte[] lpReportBuffer, int reportBufferLength);

        [DllImport("hid.dll")]
        public static extern bool HidP_GetCaps(IntPtr preparsedData, ref HIDP_CAPS capabilities);

        [DllImport("hid.dll")]
        public static extern bool HidP_GetButtonCaps(HIDP_REPORT_TYPE reportType, [In, Out] ButtonValueCaps[] buttonCaps, ref int buttonCapsLength, IntPtr preparsedData);

        [DllImport("hid.dll")]
        public static extern bool HidP_GetValueCaps(HIDP_REPORT_TYPE reportType, [In, Out] ButtonValueCaps[] valueCaps, ref int valueCapsLength, IntPtr preparsedData);

        [DllImport("hid.dll")]
        public static extern bool HidP_GetUsages(HIDP_REPORT_TYPE reportType, ushort usagePage, ushort linkCollection, ushort[] usageList, ref int usageLength, IntPtr preparsedData, byte[] report, int reportLength);

        [DllImport("hid.dll")]
        public static extern bool HidP_GetUsageValue(HIDP_REPORT_TYPE reportType, ushort usagePage, ushort linkCollection, ushort usage, out uint usageValue, IntPtr preparsedData, byte[] report, int reportLength);

        [DllImport("hid.dll")]
        public static extern bool HidD_GetInputReport(SafeFileHandle hidDeviceObject, byte[] buffer, int bufferLength);

        [DllImport("hid.dll", CharSet = CharSet.Unicode)]
        public static extern bool HidD_GetProductString(SafeFileHandle hidDeviceObject, ref byte lpReportBuffer, int ReportBufferLength);

        [DllImport("hid.dll", CharSet = CharSet.Unicode)]
        public static extern bool HidD_GetManufacturerString(SafeFileHandle hidDeviceObject, ref byte lpReportBuffer, int ReportBufferLength);

        [DllImport("hid.dll", CharSet = CharSet.Unicode)]
        public static extern bool HidD_GetSerialNumberString(SafeFileHandle hidDeviceObject, ref byte lpReportBuffer, int reportBufferLength);

        [DllImport("hid.dll")]
        public static extern bool HidD_GetPreparsedData(SafeFileHandle hidDeviceObject, ref IntPtr preparsedData);

        [DllImport("hid.dll")]
        public static extern bool HidD_FreePreparsedData(IntPtr preparsedData);

        [DllImport("hid.dll")]
        public static extern bool HidD_SetOutputReport(SafeFileHandle hidDeviceObject, byte[] lpReportBuffer, int reportBufferLength);
    }
}