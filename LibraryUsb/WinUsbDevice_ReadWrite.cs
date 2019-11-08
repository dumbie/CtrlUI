using System;
using System.Runtime.InteropServices;
using static LibraryUsb.NativeMethods_DeviceManager;
using static LibraryUsb.NativeMethods_WinUsb;

namespace LibraryUsb
{
    public partial class WinUsbDevice
    {
        public USB_DEVICE_DESCRIPTOR ReadDescriptor(ref int Transferred)
        {
            USB_DEVICE_DESCRIPTOR USB_DEVICE_DESCRIPTOR = new USB_DEVICE_DESCRIPTOR();
            int USB_DEVICE_DESCRIPTOR_SIZE = Marshal.SizeOf(USB_DEVICE_DESCRIPTOR);
            WinUsb_GetDescriptor(WinUsbHandle, 0x01, 0, 0, ref USB_DEVICE_DESCRIPTOR, USB_DEVICE_DESCRIPTOR_SIZE, ref Transferred);
            return USB_DEVICE_DESCRIPTOR;
        }

        public bool ReadIntPipe(byte[] Buffer, int Length, ref int Transferred)
        {
            if (!IsActive)
            {
                return false;
            }

            return WinUsb_ReadPipe(WinUsbHandle, IntIn, Buffer, Length, ref Transferred, IntPtr.Zero);
        }

        public bool ReadBulkPipe(byte[] Buffer, int Length, ref int Transferred)
        {
            if (!IsActive)
            {
                return false;
            }

            return WinUsb_ReadPipe(WinUsbHandle, BulkIn, Buffer, Length, ref Transferred, IntPtr.Zero);
        }

        public bool WriteIntPipe(byte[] Buffer, int Length, ref int Transferred)
        {
            if (!IsActive)
            {
                return false;
            }

            return WinUsb_WritePipe(WinUsbHandle, IntOut, Buffer, Length, ref Transferred, IntPtr.Zero);
        }

        public bool WriteBulkPipe(byte[] Buffer, int Length, ref int Transferred)
        {
            if (!IsActive)
            {
                return false;
            }

            return WinUsb_WritePipe(WinUsbHandle, BulkOut, Buffer, Length, ref Transferred, IntPtr.Zero);
        }

        public bool SendTransfer(byte RequestType, byte Request, ushort Value, byte[] Buffer, ref int Transferred)
        {
            if (!IsActive)
            {
                return false;
            }

            WINUSB_SETUP_PACKET Setup = new WINUSB_SETUP_PACKET();
            Setup.RequestType = RequestType;
            Setup.Request = Request;
            Setup.Value = Value;
            Setup.Index = 0;
            Setup.Length = (ushort)Buffer.Length;

            return WinUsb_ControlTransfer(WinUsbHandle, Setup, Buffer, Buffer.Length, ref Transferred, IntPtr.Zero);
        }

        public bool Report(byte[] Input, byte[] Output)
        {
            if (!IsActive)
            {
                return false;
            }

            int Transferred = 0;
            return DeviceIoControl(FileHandle, 0x2A400C, Input, Input.Length, Output, Output.Length, ref Transferred, IntPtr.Zero) && Transferred > 0;
        }
    }
}