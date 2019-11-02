using System;
using System.Runtime.InteropServices;

namespace LibraryUsb
{
    internal static class NativeMethods_DevMan
    {
        [DllImport("kernel32.dll")]
        internal static extern bool DeviceIoControl(IntPtr DeviceHandle, int IoControlCode, byte[] InBuffer, int InBufferSize, byte[] OutBuffer, int OutBufferSize, ref int BytesReturned, IntPtr Overlapped);
    }
}