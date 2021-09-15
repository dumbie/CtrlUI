using ArnoldVinkCode;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LibraryUsb
{
    public partial class FakerInputDevice
    {
        public bool KeyboardPressRelease(KeyboardModifiers keyModifier, KeyboardKeys key0, KeyboardKeys key1, KeyboardKeys key2, KeyboardKeys key3, KeyboardKeys key4, KeyboardKeys key5)
        {
            try
            {
                KeyboardPress(keyModifier, key0, key1, key2, key3, key4, key5);
                AVActions.TaskDelayMs(50);
                KeyboardReset();
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to press and release keyboard keys.");
                return false;
            }
        }

        public bool KeyboardPress(KeyboardModifiers keyModifier, KeyboardKeys key0, KeyboardKeys key1, KeyboardKeys key2, KeyboardKeys key3, KeyboardKeys key4, KeyboardKeys key5)
        {
            try
            {
                FAKERINPUT_CONTROL_REPORT_HEADER structHeader = new FAKERINPUT_CONTROL_REPORT_HEADER();
                structHeader.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_CONTROL;
                structHeader.ReportLength = (byte)Marshal.SizeOf(typeof(FAKERINPUT_KEYBOARD_REPORT));
                byte[] headerArray = ConvertToByteArray(structHeader);

                FAKERINPUT_KEYBOARD_REPORT structInput = new FAKERINPUT_KEYBOARD_REPORT();
                structInput.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_KEYBOARD;
                structInput.ModifierCode = (byte)keyModifier;
                structInput.KeyCodes = new byte[] { (byte)key0, (byte)key1, (byte)key2, (byte)key3, (byte)key4, (byte)key5 };
                byte[] inputArray = ConvertToByteArray(structInput);

                return WriteBytesFile(MergeHeaderInputByteArray(CONTROL_REPORT_SIZE, headerArray, inputArray));
            }
            catch
            {
                Debug.WriteLine("Failed to press keyboard key.");
                return false;
            }
        }

        public bool KeyboardReset()
        {
            try
            {
                FAKERINPUT_CONTROL_REPORT_HEADER structHeader = new FAKERINPUT_CONTROL_REPORT_HEADER();
                structHeader.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_CONTROL;
                structHeader.ReportLength = (byte)Marshal.SizeOf(typeof(FAKERINPUT_KEYBOARD_REPORT));
                byte[] headerArray = ConvertToByteArray(structHeader);

                FAKERINPUT_KEYBOARD_REPORT structInput = new FAKERINPUT_KEYBOARD_REPORT();
                structInput.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_KEYBOARD;
                byte[] inputArray = ConvertToByteArray(structInput);

                return WriteBytesFile(MergeHeaderInputByteArray(CONTROL_REPORT_SIZE, headerArray, inputArray));
            }
            catch
            {
                Debug.WriteLine("Failed to reset keyboard input.");
                return false;
            }
        }
    }
}