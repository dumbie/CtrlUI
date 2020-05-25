using System;
using System.Runtime.InteropServices;

namespace LibraryUsb
{
    public static class NativeMethods_DeviceManager
    {
        [DllImport("kernel32.dll")]
        public static extern bool DeviceIoControl(IntPtr hDevice, IoControlCodes dwIoControlCode, IntPtr lpInBuffer, int nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern bool DeviceIoControl(IntPtr hDevice, IoControlCodes dwIoControlCode, byte[] lpInBuffer, int nInBufferSize, byte[] lpOutBuffer, int nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

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
    }
}