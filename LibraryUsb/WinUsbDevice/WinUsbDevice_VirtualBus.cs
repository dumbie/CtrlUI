using System;
using System.Diagnostics;
using static LibraryUsb.NativeMethods_IoControl;

namespace LibraryUsb
{
    public partial class WinUsbDevice
    {
        public enum IoControlCodesVirtual : uint
        {
            SCP_PLUGIN = 0x2A4000,
            SCP_UNPLUG = 0x2A4004,
            SCP_REPORT = 0x2A400C
        }

        public bool VirtualPlugin(int controllerNumber)
        {
            try
            {
                if (!Connected) { return false; }
                byte[] outputBuffer = new byte[16];
                outputBuffer[0] = 0x10;
                outputBuffer[4] = (byte)((controllerNumber + 1 >> 0) & 0xFF);
                outputBuffer[5] = (byte)((controllerNumber + 1 >> 8) & 0xFF);
                outputBuffer[6] = (byte)((controllerNumber + 1 >> 16) & 0xFF);
                outputBuffer[7] = (byte)((controllerNumber + 1 >> 24) & 0xFF);
                return DeviceIoControl(FileHandle, (uint)IoControlCodesVirtual.SCP_PLUGIN, outputBuffer, outputBuffer.Length, null, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to plugin device: " + ex.Message);
                return false;
            }
        }

        public bool VirtualUnplug(int controllerNumber)
        {
            try
            {
                if (!Connected) { return false; }
                byte[] outputBuffer = new byte[16];
                outputBuffer[0] = 0x10;
                outputBuffer[4] = (byte)((controllerNumber + 1 >> 0) & 0xFF);
                outputBuffer[5] = (byte)((controllerNumber + 1 >> 8) & 0xFF);
                outputBuffer[6] = (byte)((controllerNumber + 1 >> 16) & 0xFF);
                outputBuffer[7] = (byte)((controllerNumber + 1 >> 24) & 0xFF);
                return DeviceIoControl(FileHandle, (uint)IoControlCodesVirtual.SCP_UNPLUG, outputBuffer, outputBuffer.Length, null, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to unplug device: " + ex.Message);
                return false;
            }
        }

        public bool VirtualUnplugAll()
        {
            try
            {
                if (!Connected) { return false; }
                byte[] outputBuffer = new byte[16];
                outputBuffer[0] = 0x10;
                return DeviceIoControl(FileHandle, (uint)IoControlCodesVirtual.SCP_UNPLUG, outputBuffer, outputBuffer.Length, null, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to unplug all devices: " + ex.Message);
                return false;
            }
        }

        public bool VirtualReadWrite(byte[] inputBuffer, byte[] outputBuffer)
        {
            try
            {
                if (!Connected) { return false; }
                return DeviceIoControl(FileHandle, (uint)IoControlCodesVirtual.SCP_REPORT, inputBuffer, inputBuffer.Length, outputBuffer, outputBuffer.Length, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to write scp bytes: " + ex.Message);
                return false;
            }
        }
    }
}