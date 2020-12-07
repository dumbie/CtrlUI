using System;
using System.Diagnostics;
using static LibraryUsb.NativeMethods_WinUsb;

namespace LibraryUsb
{
    public partial class WinUsbDevice
    {
        public bool WriteBytesIntPipe(byte[] outputBuffer)
        {
            try
            {
                if (!Connected) { return false; }
                return WinUsb_WritePipe(WinUsbHandle, IntOut, outputBuffer, outputBuffer.Length, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to write intpipe bytes: " + ex.Message);
                return false;
            }
        }

        public bool WriteBytesBulkPipe(byte[] outputBuffer)
        {
            try
            {
                if (!Connected) { return false; }
                return WinUsb_WritePipe(WinUsbHandle, BulkOut, outputBuffer, outputBuffer.Length, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to write bulkpipe bytes: " + ex.Message);
                return false;
            }
        }

        public bool WriteBytesTransfer(byte requestType, byte request, ushort value, byte[] outputBuffer)
        {
            try
            {
                if (!Connected) { return false; }
                WINUSB_SETUP_PACKET setupPacket = new WINUSB_SETUP_PACKET();
                setupPacket.RequestType = requestType;
                setupPacket.Request = request;
                setupPacket.Value = value;
                setupPacket.Index = 0;
                setupPacket.Length = (ushort)outputBuffer.Length;
                return WinUsb_ControlTransfer(WinUsbHandle, setupPacket, outputBuffer, outputBuffer.Length, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to write transfer bytes: " + ex.Message);
                return false;
            }
        }

        public bool ReadBytesIntPipe(byte[] inputBuffer)
        {
            try
            {
                if (!Connected) { return false; }
                return WinUsb_ReadPipe(WinUsbHandle, IntIn, inputBuffer, inputBuffer.Length, out int bytesRead, IntPtr.Zero) && bytesRead > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to read intpipe bytes: " + ex.Message);
                return false;
            }
        }

        public bool ReadBytesBulkPipe(byte[] inputBuffer)
        {
            try
            {
                if (!Connected) { return false; }
                return WinUsb_ReadPipe(WinUsbHandle, BulkIn, inputBuffer, inputBuffer.Length, out int bytesRead, IntPtr.Zero) && bytesRead > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to read bulkpipe bytes: " + ex.Message);
                return false;
            }
        }
    }
}