using System;
using System.Runtime.InteropServices;

namespace LibraryUsb
{
    public static class NativeMethods_DeviceManager
    {
        [DllImport("kernel32.dll")]
        public static extern bool DeviceIoControl(IntPtr DeviceHandle, int IoControlCode, byte[] InBuffer, int InBufferSize, byte[] OutBuffer, int OutBufferSize, ref int BytesReturned, IntPtr Overlapped);
    }
}