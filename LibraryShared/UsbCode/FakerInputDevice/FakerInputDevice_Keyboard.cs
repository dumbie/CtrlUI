using ArnoldVinkCode;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInputOutputClass;

namespace LibraryUsb
{
    public partial class FakerInputDevice
    {
        public bool KeyboardPressRelease(KeysHidAction keyboardAction)
        {
            try
            {
                KeyboardPress(keyboardAction);
                AVActions.TaskDelayHighRes(50);
                KeyboardReset();
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to press and release keyboard keys.");
                return false;
            }
        }

        public bool KeyboardPress(KeysHidAction keyboardAction)
        {
            try
            {
                FAKERINPUT_CONTROL_REPORT_HEADER structHeader = new FAKERINPUT_CONTROL_REPORT_HEADER();
                structHeader.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_CONTROL;
                structHeader.ReportLength = (byte)Marshal.SizeOf(typeof(FAKERINPUT_KEYBOARD_REPORT));
                byte[] headerArray = ConvertToByteArray(structHeader);

                FAKERINPUT_KEYBOARD_REPORT structInput = new FAKERINPUT_KEYBOARD_REPORT();
                structInput.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_KEYBOARD;
                structInput.ModifierCodes = (byte)keyboardAction.Modifiers;
                structInput.KeyCodes = new byte[] { (byte)keyboardAction.Key0, (byte)keyboardAction.Key1, (byte)keyboardAction.Key2, (byte)keyboardAction.Key3, (byte)keyboardAction.Key4, (byte)keyboardAction.Key5, (byte)keyboardAction.Key6, (byte)keyboardAction.Key7 };
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