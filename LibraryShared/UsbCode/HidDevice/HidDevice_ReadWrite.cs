using System;
using System.Diagnostics;
using static LibraryUsb.NativeMethods_Hid;

namespace LibraryUsb
{
    public partial class HidDevice
    {
        public bool WriteBytesOutputReport(byte[] outputBuffer)
        {
            try
            {
                if (!Connected) { return false; }
                return HidD_SetOutputReport(FileHandle, outputBuffer, outputBuffer.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to write output report bytes: " + ex.Message);
                return false;
            }
        }

        public bool WriteBytesFile(byte[] outputBuffer)
        {
            try
            {
                if (!Connected) { return false; }
                FileStream.Write(outputBuffer, 0, outputBuffer.Length);
                return true;
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
                return FileStream.Read(inputBuffer, 0, inputBuffer.Length) > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to read file bytes: " + ex.Message);
                return false;
            }
        }
    }
}