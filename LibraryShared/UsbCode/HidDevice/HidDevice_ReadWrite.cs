using System;
using System.Diagnostics;
using System.Threading;
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
                FileStream.Write(outputBuffer, 0, outputBuffer.Length);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to write file bytes: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> WriteBytesFileTimeOut(byte[] outputBuffer, int writeTimeOut)
        {
            try
            {
                if (!Connected) { return false; }
                using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
                {
                    cancellationTokenSource.CancelAfter(writeTimeOut);
                    await FileStream.WriteAsync(outputBuffer, 0, outputBuffer.Length, cancellationTokenSource.Token);
                    return true;
                }
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

        public async Task<bool> ReadBytesFileTimeOut(byte[] inputBuffer, int readTimeOut)
        {
            try
            {
                if (!Connected) { return false; }
                using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
                {
                    cancellationTokenSource.CancelAfter(readTimeOut);
                    return await FileStream.ReadAsync(inputBuffer, 0, inputBuffer.Length, cancellationTokenSource.Token) > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to read file bytes: " + ex.Message);
                return false;
            }
        }
    }
}