using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace LibraryUsb
{
    public class NativeMethods_IoControl
    {
        public enum IoControlCodes : uint
        {
            IOCTL_DEVICE_CONNECT = 0x2A4000,
            IOCTL_DEVICE_DISCONNECT = 0x2A4004,
            IOCTL_DEVICE_SENDDATA = 0x2A400C,
            IOCTL_STORAGE_EJECT_MEDIA = 0x2D4808,
            IOCTL_STORAGE_MEDIA_REMOVAL = 0x002D4804,
            IOCTL_BTH_DISCONNECT_DEVICE = 0x41000C,
            IOCTL_HID_ACTIVATE_DEVICE = 0xB001F,
            IOCTL_HID_DEACTIVATE_DEVICE = 0xB0023
        }

        [DllImport("kernel32.dll")]
        public static extern bool DeviceIoControl(IntPtr hDevice, IoControlCodes dwIoControlCode, byte[] lpInBuffer, int nInBufferSize, byte[] lpOutBuffer, int nOutBufferSize, out int lpBytesReturned, IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern bool DeviceIoControl(SafeFileHandle hDevice, IoControlCodes dwIoControlCode, byte[] lpInBuffer, int nInBufferSize, byte[] lpOutBuffer, int nOutBufferSize, out int lpBytesReturned, IntPtr lpOverlapped);
    }
}