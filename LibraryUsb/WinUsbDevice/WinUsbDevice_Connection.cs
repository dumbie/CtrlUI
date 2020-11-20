using System;
using System.Diagnostics;
using static LibraryUsb.NativeMethods_IoControl;

namespace LibraryUsb
{
    public partial class WinUsbDevice
    {
        public bool Plugin(int number)
        {
            try
            {
                if (!Connected) { return false; }
                byte[] outputBuffer = new byte[16];
                outputBuffer[0] = 0x10;
                outputBuffer[1] = 0x00;
                outputBuffer[2] = 0x00;
                outputBuffer[3] = 0x00;
                number++;
                outputBuffer[4] = (byte)((number >> 0) & 0xFF);
                outputBuffer[5] = (byte)((number >> 8) & 0xFF);
                outputBuffer[6] = (byte)((number >> 16) & 0xFF);
                outputBuffer[7] = (byte)((number >> 24) & 0xFF);
                return DeviceIoControl(FileHandle, IoControlCodes.IOCTL_DEVICE_CONNECT, outputBuffer, outputBuffer.Length, null, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to plugin device: " + ex.Message);
                return false;
            }
        }

        public bool Unplug(int number)
        {
            try
            {
                if (!Connected) { return false; }
                byte[] outputBuffer = new byte[16];
                outputBuffer[0] = 0x10;
                outputBuffer[1] = 0x00;
                outputBuffer[2] = 0x00;
                outputBuffer[3] = 0x00;
                number++;
                outputBuffer[4] = (byte)((number >> 0) & 0xFF);
                outputBuffer[5] = (byte)((number >> 8) & 0xFF);
                outputBuffer[6] = (byte)((number >> 16) & 0xFF);
                outputBuffer[7] = (byte)((number >> 24) & 0xFF);
                return DeviceIoControl(FileHandle, IoControlCodes.IOCTL_DEVICE_DISCONNECT, outputBuffer, outputBuffer.Length, null, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to unplug device: " + ex.Message);
                return false;
            }
        }

        public bool UnplugAll()
        {
            try
            {
                if (!Connected) { return false; }
                byte[] outputBuffer = new byte[16];
                outputBuffer[0] = 0x10;
                outputBuffer[1] = 0x00;
                outputBuffer[2] = 0x00;
                outputBuffer[3] = 0x00;
                return DeviceIoControl(FileHandle, IoControlCodes.IOCTL_DEVICE_DISCONNECT, outputBuffer, outputBuffer.Length, null, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to unplug all devices: " + ex.Message);
                return false;
            }
        }
    }
}