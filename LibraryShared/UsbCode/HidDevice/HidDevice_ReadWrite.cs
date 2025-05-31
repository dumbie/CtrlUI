using System;
using System.Diagnostics;
using static LibraryUsb.NativeMethods_File;

namespace LibraryUsb
{
    public partial class HidDevice
    {
        public bool WriteBytesFile(byte[] outputBuffer)
        {
            try
            {
                if (!Connected) { return false; }
                WriteFile(FileHandle, outputBuffer, outputBuffer.Length, out int lpNumberOfBytesWritten, IntPtr.Zero);
                return lpNumberOfBytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to write file bytes: " + ex.Message);
                return false;
            }
        }

        public bool ReadBytesFile(byte[] inputBuffer)
        {
            try
            {
                if (!Connected) { return false; }
                ReadFile(FileHandle, inputBuffer, inputBuffer.Length, out int lpNumberOfBytesRead, IntPtr.Zero);
                return lpNumberOfBytesRead > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to read file bytes: " + ex.Message);
                return false;
            }
        }
    }
}