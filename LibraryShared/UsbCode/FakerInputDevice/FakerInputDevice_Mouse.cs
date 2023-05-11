using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static ArnoldVinkCode.AVInputOutputClass;

namespace LibraryUsb
{
    public partial class FakerInputDevice
    {
        public bool MouseRelative(MouseHidAction mouseAction)
        {
            try
            {
                FAKERINPUT_CONTROL_REPORT_HEADER structHeader = new FAKERINPUT_CONTROL_REPORT_HEADER();
                structHeader.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_CONTROL;
                structHeader.ReportLength = (byte)Marshal.SizeOf(typeof(FAKERINPUT_RELATIVE_MOUSE_REPORT));
                byte[] headerArray = ConvertToByteArray(structHeader);

                FAKERINPUT_RELATIVE_MOUSE_REPORT structInput = new FAKERINPUT_RELATIVE_MOUSE_REPORT();
                structInput.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_RELATIVE_MOUSE;
                structInput.XValue = (short)mouseAction.MoveHorizontal;
                structInput.YValue = (short)mouseAction.MoveVertical;
                structInput.VWheelPosition = (byte)mouseAction.ScrollVertical;
                structInput.HWheelPosition = (byte)mouseAction.ScrollHorizontal;
                structInput.Button = (byte)mouseAction.Button;
                byte[] inputArray = ConvertToByteArray(structInput);

                return WriteBytesFile(MergeHeaderInputByteArray(CONTROL_REPORT_SIZE, headerArray, inputArray));
            }
            catch
            {
                Debug.WriteLine("Failed to set relative mouse input.");
                return false;
            }
        }

        public bool MouseResetRelative()
        {
            try
            {
                FAKERINPUT_CONTROL_REPORT_HEADER structHeader = new FAKERINPUT_CONTROL_REPORT_HEADER();
                structHeader.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_CONTROL;
                structHeader.ReportLength = (byte)Marshal.SizeOf(typeof(FAKERINPUT_RELATIVE_MOUSE_REPORT));
                byte[] headerArray = ConvertToByteArray(structHeader);

                FAKERINPUT_RELATIVE_MOUSE_REPORT structInput = new FAKERINPUT_RELATIVE_MOUSE_REPORT();
                structInput.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_RELATIVE_MOUSE;
                byte[] inputArray = ConvertToByteArray(structInput);

                return WriteBytesFile(MergeHeaderInputByteArray(CONTROL_REPORT_SIZE, headerArray, inputArray));
            }
            catch
            {
                Debug.WriteLine("Failed to reset relative mouse input.");
                return false;
            }
        }

        public bool MouseAbsolute(MouseHidAction mouseAction)
        {
            try
            {
                Screen screen = Screen.FromPoint(Cursor.Position);
                double maxValue = 32767.5;

                double targetWidth = (double)mouseAction.MoveHorizontal / screen.Bounds.Width;
                targetWidth *= maxValue;
                if (targetWidth > maxValue) { targetWidth = maxValue; }

                double targetHeight = (double)mouseAction.MoveVertical / screen.Bounds.Height;
                targetHeight *= maxValue;
                if (targetHeight > maxValue) { targetHeight = maxValue; }

                FAKERINPUT_CONTROL_REPORT_HEADER structHeader = new FAKERINPUT_CONTROL_REPORT_HEADER();
                structHeader.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_CONTROL;
                structHeader.ReportLength = (byte)Marshal.SizeOf(typeof(FAKERINPUT_ABSOLUTE_MOUSE_REPORT));
                byte[] headerArray = ConvertToByteArray(structHeader);

                FAKERINPUT_ABSOLUTE_MOUSE_REPORT structInput = new FAKERINPUT_ABSOLUTE_MOUSE_REPORT();
                structInput.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_ABSOLUTE_MOUSE;
                structInput.XValue = (ushort)targetWidth;
                structInput.YValue = (ushort)targetHeight;
                structInput.VWheelPosition = (byte)mouseAction.ScrollVertical;
                structInput.HWheelPosition = (byte)mouseAction.ScrollHorizontal;
                structInput.Button = (byte)mouseAction.Button;
                byte[] inputArray = ConvertToByteArray(structInput);

                return WriteBytesFile(MergeHeaderInputByteArray(CONTROL_REPORT_SIZE, headerArray, inputArray));
            }
            catch
            {
                Debug.WriteLine("Failed to set absolute mouse input.");
                return false;
            }
        }

        public bool MouseResetAbsolute()
        {
            try
            {
                FAKERINPUT_CONTROL_REPORT_HEADER structHeader = new FAKERINPUT_CONTROL_REPORT_HEADER();
                structHeader.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_CONTROL;
                structHeader.ReportLength = (byte)Marshal.SizeOf(typeof(FAKERINPUT_ABSOLUTE_MOUSE_REPORT));
                byte[] headerArray = ConvertToByteArray(structHeader);

                FAKERINPUT_ABSOLUTE_MOUSE_REPORT structInput = new FAKERINPUT_ABSOLUTE_MOUSE_REPORT();
                structInput.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_ABSOLUTE_MOUSE;
                byte[] inputArray = ConvertToByteArray(structInput);

                return WriteBytesFile(MergeHeaderInputByteArray(CONTROL_REPORT_SIZE, headerArray, inputArray));
            }
            catch
            {
                Debug.WriteLine("Failed to reset absolute mouse input.");
                return false;
            }
        }
    }
}