using System;
using System.Diagnostics;
using System.Linq;
using static LibraryShared.CRC32;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Replace CRC32 hash in bytes array
        private bool ByteArrayCRC32Replace(ref byte[] outputReport, uint crcSeed, int crcIndexSkip, int crcIndexStart)
        {
            try
            {
                //Compute CRC32 hash
                byte[] checksum = ComputeHashCRC32(crcSeed, outputReport.Take(crcIndexSkip + crcIndexStart).ToArray(), false);

                //Skip header and take hashed bytes
                outputReport = outputReport.Skip(crcIndexSkip).Take(crcIndexStart + 4).ToArray();

                //Add CRC32 hash bytes
                outputReport[crcIndexStart] = checksum[0];
                outputReport[crcIndexStart + 1] = checksum[1];
                outputReport[crcIndexStart + 2] = checksum[2];
                outputReport[crcIndexStart + 3] = checksum[3];

                //Return result
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to replace CRC32 bytes in array: " + ex.Message);
                return false;
            }
        }
    }
}