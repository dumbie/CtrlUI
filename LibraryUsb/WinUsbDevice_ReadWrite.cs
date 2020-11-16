using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static LibraryUsb.NativeMethods_IoControl;
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

        public bool WriteBytesDeviceIO(byte[] inputBuffer, byte[] outputBuffer)
        {
            try
            {
                if (!Connected) { return false; }
                return DeviceIoControl(FileHandle, IoControlCodes.IOCTL_DEVICE_SENDDATA, inputBuffer, inputBuffer.Length, outputBuffer, outputBuffer.Length, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to write deviceio bytes: " + ex.Message);
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

        public USB_DEVICE_DESCRIPTOR? ReadDeviceDescriptor()
        {
            try
            {
                USB_DEVICE_DESCRIPTOR USB_DEVICE_DESCRIPTOR = new USB_DEVICE_DESCRIPTOR();
                int descriptorSize = Marshal.SizeOf(USB_DEVICE_DESCRIPTOR);
                bool readed = WinUsb_GetDescriptor(WinUsbHandle, DESCRIPTOR_TYPE.USB_DEVICE_DESCRIPTOR_TYPE, 0, 0, ref USB_DEVICE_DESCRIPTOR, descriptorSize, out int bytesRead) && bytesRead > 0;
                if (readed)
                {
                    return USB_DEVICE_DESCRIPTOR;
                }
                else
                {
                    Debug.WriteLine("Failed to read device descriptor.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to read device descriptor: " + ex.Message);
                return null;
            }
        }
    }
}