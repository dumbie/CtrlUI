using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LibraryUsb
{
    public partial class FakerInputDevice
    {
        public bool MousePress(MouseButtons mouseButton)
        {
            try
            {
                FAKERINPUT_CONTROL_REPORT_HEADER structHeader = new FAKERINPUT_CONTROL_REPORT_HEADER();
                structHeader.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_CONTROL;
                structHeader.ReportLength = (byte)Marshal.SizeOf(typeof(FAKERINPUT_RELATIVE_MOUSE_REPORT));
                byte[] headerArray = ConvertToByteArray(structHeader);

                FAKERINPUT_RELATIVE_MOUSE_REPORT structInput = new FAKERINPUT_RELATIVE_MOUSE_REPORT();
                structInput.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_RELATIVE_MOUSE;
                structInput.Button = (byte)mouseButton;
                byte[] inputArray = ConvertToByteArray(structInput);

                return WriteBytesFile(MergeHeaderInputByteArray(CONTROL_REPORT_SIZE, headerArray, inputArray));
            }
            catch
            {
                Debug.WriteLine("Failed to set press mouse button.");
                return false;
            }
        }

        public bool MouseScroll(int verticalScroll, int horizontalScroll)
        {
            try
            {
                FAKERINPUT_CONTROL_REPORT_HEADER structHeader = new FAKERINPUT_CONTROL_REPORT_HEADER();
                structHeader.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_CONTROL;
                structHeader.ReportLength = (byte)Marshal.SizeOf(typeof(FAKERINPUT_RELATIVE_MOUSE_REPORT));
                byte[] headerArray = ConvertToByteArray(structHeader);

                FAKERINPUT_RELATIVE_MOUSE_REPORT structInput = new FAKERINPUT_RELATIVE_MOUSE_REPORT();
                structInput.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_RELATIVE_MOUSE;
                structInput.VWheelPosition = (byte)verticalScroll;
                structInput.HWheelPosition = (byte)horizontalScroll;
                byte[] inputArray = ConvertToByteArray(structInput);

                return WriteBytesFile(MergeHeaderInputByteArray(CONTROL_REPORT_SIZE, headerArray, inputArray));
            }
            catch
            {
                Debug.WriteLine("Failed to press mouse button.");
                return false;
            }
        }

        public bool MouseMoveRelative(short moveX, short moveY)
        {
            try
            {
                FAKERINPUT_CONTROL_REPORT_HEADER structHeader = new FAKERINPUT_CONTROL_REPORT_HEADER();
                structHeader.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_CONTROL;
                structHeader.ReportLength = (byte)Marshal.SizeOf(typeof(FAKERINPUT_RELATIVE_MOUSE_REPORT));
                byte[] headerArray = ConvertToByteArray(structHeader);

                FAKERINPUT_RELATIVE_MOUSE_REPORT structInput = new FAKERINPUT_RELATIVE_MOUSE_REPORT();
                structInput.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_RELATIVE_MOUSE;
                structInput.XValue = moveX;
                structInput.YValue = moveY;
                byte[] inputArray = ConvertToByteArray(structInput);

                return WriteBytesFile(MergeHeaderInputByteArray(CONTROL_REPORT_SIZE, headerArray, inputArray));
            }
            catch
            {
                Debug.WriteLine("Failed to set relative mouse input.");
                return false;
            }
        }

        public bool MouseMoveAbsolute(double moveX, double moveY)
        {
            try
            {
                //Screen screen = Screen.FromPoint(Cursor.Position);
                //int screenWidth = screen.Bounds.Width;
                //int screenHeight = screen.Bounds.Height;

                FAKERINPUT_CONTROL_REPORT_HEADER structHeader = new FAKERINPUT_CONTROL_REPORT_HEADER();
                structHeader.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_CONTROL;
                structHeader.ReportLength = (byte)Marshal.SizeOf(typeof(FAKERINPUT_ABSOLUTE_MOUSE_REPORT));
                byte[] headerArray = ConvertToByteArray(structHeader);

                FAKERINPUT_ABSOLUTE_MOUSE_REPORT structInput = new FAKERINPUT_ABSOLUTE_MOUSE_REPORT();
                structInput.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_ABSOLUTE_MOUSE;
                structInput.XValue = (ushort)(moveX * 32767.5); //0.0 to 1.0
                structInput.YValue = (ushort)(moveY * 32767.5); //0.0 to 1.0
                byte[] inputArray = ConvertToByteArray(structInput);

                return WriteBytesFile(MergeHeaderInputByteArray(CONTROL_REPORT_SIZE, headerArray, inputArray));
            }
            catch
            {
                Debug.WriteLine("Failed to set relative mouse input.");
                return false;
            }
        }

        public bool MouseReset()
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
                Debug.WriteLine("Failed to reset mouse input.");
                return false;
            }
        }
    }
}