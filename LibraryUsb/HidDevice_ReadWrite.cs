using System;
using System.Threading.Tasks;

namespace LibraryUsb
{
    public partial class HidDevice
    {
        //Read file with timeout
        public async Task<bool> ReadFileTimeout(byte[] lpBuffer, uint nNumberOfBytesToRead, IntPtr lpOverlapped, int readTimeOut)
        {
            try
            {
                Task<bool> readTask = Task.Run(delegate
                {
                    return NativeMethods_Hid.ReadFile(DeviceHandle, lpBuffer, nNumberOfBytesToRead, out uint lpNumberOfBytesRead, lpOverlapped);
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