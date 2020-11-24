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
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            try
            {
                if (!Connected) { return false; }

                Task<int> readTask = FileStream.ReadAsync(inputBuffer, 0, inputBuffer.Length, cancellationTokenSource.Token);
                Task delayTask = Task.Delay(readTimeOut);
                Task timeoutTask = await Task.WhenAny(readTask, delayTask);
                if (timeoutTask == readTask)
                {
                    return readTask.Result > 0;
                }

                Debug.WriteLine("Failed to read file bytes, timeout.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to read file bytes: " + ex.Message);
                return false;
            }
            finally
            {
                if (cancellationTokenSource.Token.CanBeCanceled) { cancellationTokenSource.Cancel(); }
                cancellationTokenSource.Dispose();
            }
        }
    }
}