using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static LibraryUsb.NativeMethods_Hid;

namespace LibraryUsb
{
    public partial class HidDevice
    {
        public bool WriteBytesOutputReport(byte[] outputBuffer)
        {
            try
            {
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
                return WriteFile(FileHandle, outputBuffer, (uint)outputBuffer.Length, out uint bytesWritten, IntPtr.Zero);
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
                IntPtr lpOverlapped = IntPtr.Zero;
                return ReadFile(FileHandle, inputBuffer, (uint)inputBuffer.Length, out uint lpNumberOfBytesRead, lpOverlapped);
            }
            catch { }
            return false;
        }

        public async Task<bool> ReadBytesFileTimeout(byte[] inputBuffer, int readTimeOut)
        {
            try
            {
                Task<bool> readTask = Task.Run(delegate
                {
                    IntPtr lpOverlapped = IntPtr.Zero;
                    return ReadFile(FileHandle, inputBuffer, (uint)inputBuffer.Length, out uint lpNumberOfBytesRead, lpOverlapped);
                });

                Task delayTask = Task.Delay(readTimeOut);
                Task timeoutTask = await Task.WhenAny(readTask, delayTask);
                if (timeoutTask == readTask)
                {
                    return readTask.Result;
                }
            }
            catch { }
            return false;
        }
    }
}