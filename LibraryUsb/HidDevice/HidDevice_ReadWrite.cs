using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static LibraryUsb.NativeMethods_File;
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
                Debug.WriteLine("Failed to write outputreport bytes: " + ex.Message);
                return false;
            }
        }

        public bool WriteBytesFile(byte[] outputBuffer)
        {
            try
            {
                if (!Connected) { return false; }
                return WriteFile(FileHandle, outputBuffer, outputBuffer.Length, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
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
                return ReadFile(FileHandle, inputBuffer, inputBuffer.Length, out int bytesRead, IntPtr.Zero) && bytesRead > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to read file bytes: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> ReadBytesFileTimeout(byte[] inputBuffer, int readTimeOut)
        {
            try
            {
                if (!Connected) { return false; }
                Task<bool> readTask = Task.Run(delegate
                {
                    return ReadFile(FileHandle, inputBuffer, inputBuffer.Length, out int bytesRead, IntPtr.Zero) && bytesRead > 0;
                });

                Task delayTask = Task.Delay(readTimeOut);
                Task timeoutTask = await Task.WhenAny(readTask, delayTask);
                if (timeoutTask == readTask)
                {
                    return readTask.Result;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to read file timeout bytes: " + ex.Message);
                return false;
            }
        }

        public bool GetFeature(HID_USAGE_GENERIC usageGeneric)
        {
            try
            {
                int featureLength = Capabilities.FeatureReportByteLength;
                if (featureLength <= 0) { featureLength = 64; }
                byte[] data = new byte[featureLength];
                data[0] = (byte)usageGeneric;
                return HidD_GetFeature(FileHandle, data, data.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get feature: " + ex.Message);
                return false;
            }
        }

        public bool SetFeature(HID_USAGE_GENERIC usageGeneric)
        {
            try
            {
                int featureLength = Capabilities.FeatureReportByteLength;
                if (featureLength <= 0) { featureLength = 64; }
                byte[] data = new byte[featureLength];
                data[0] = (byte)usageGeneric;
                return HidD_SetFeature(FileHandle, data, data.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to set feature: " + ex.Message);
                return false;
            }
        }
    }
}