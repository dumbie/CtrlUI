using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LibraryUsb
{
    public partial class FakerInputDevice
    {
        public bool MultimediaPress(MultimediaKeys keyMultimedia)
        {
            try
            {
                FAKERINPUT_CONTROL_REPORT_HEADER structHeader = new FAKERINPUT_CONTROL_REPORT_HEADER();
                structHeader.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_CONTROL;
                structHeader.ReportLength = (byte)Marshal.SizeOf(typeof(FAKERINPUT_MULTIMEDIA_REPORT));
                byte[] headerArray = ConvertToByteArray(structHeader);

                FAKERINPUT_MULTIMEDIA_REPORT structInput = new FAKERINPUT_MULTIMEDIA_REPORT();
                structInput.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_MULTIMEDIA;
                structInput.MultimediaKey0 = (byte)keyMultimedia;
                byte[] inputArray = ConvertToByteArray(structInput);

                return WriteBytesFile(MergeHeaderInputByteArray(CONTROL_REPORT_SIZE, headerArray, inputArray));
            }
            catch
            {
                Debug.WriteLine("Failed to press multimedia key.");
                return false;
            }
        }

        public bool MultimediaReset()
        {
            try
            {
                FAKERINPUT_CONTROL_REPORT_HEADER structHeader = new FAKERINPUT_CONTROL_REPORT_HEADER();
                structHeader.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_CONTROL;
                structHeader.ReportLength = (byte)Marshal.SizeOf(typeof(FAKERINPUT_MULTIMEDIA_REPORT));
                byte[] headerArray = ConvertToByteArray(structHeader);

                FAKERINPUT_MULTIMEDIA_REPORT structInput = new FAKERINPUT_MULTIMEDIA_REPORT();
                structInput.ReportID = (byte)FAKERINPUT_REPORT_ID.REPORTID_MULTIMEDIA;
                byte[] inputArray = ConvertToByteArray(structInput);

                return WriteBytesFile(MergeHeaderInputByteArray(CONTROL_REPORT_SIZE, headerArray, inputArray));
            }
            catch
            {
                Debug.WriteLine("Failed to reset multimedia input.");
                return false;
            }
        }
    }
}