using System;
using System.Diagnostics;
using static LibraryShared.CRC32;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Add CRC32 hash to bytes array
        private byte[] ByteArrayAddCRC32(byte[] outputReport, int crcStartIndex)
        {
            try
            {
                //Compute CRC32 hash
                byte[] checksum = ComputeHashCRC32(outputReport, false);

                //Add CRC32 hash bytes
                byte[] outputReportCRC32 = new byte[outputReport.Length + 4];
                Array.Copy(outputReport, 1, outputReportCRC32, 0, outputReport.Length - 1);
                outputReportCRC32[crcStartIndex] = checksum[0];
                outputReportCRC32[crcStartIndex + 1] = checksum[1];
                outputReportCRC32[crcStartIndex + 2] = checksum[2];
                outputReportCRC32[crcStartIndex + 3] = checksum[3];
                return outputReportCRC32;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to add CRC32 bytes to the array: " + ex.Message);
                return outputReport;
            }
        }
    }
}