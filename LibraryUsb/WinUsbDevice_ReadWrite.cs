using System;
using static LibraryUsb.NativeMethods_DeviceManager;
using static LibraryUsb.NativeMethods_WinUsb;

namespace LibraryUsb
{
    public partial class WinUsbDevice
    {
        public bool ReadIntPipe(byte[] inputBuffer)
        {
            try
            {
                if (!IsActive)
                {
                    return false;
                }

                int Transferred = 0;
                bool Readed = WinUsb_ReadPipe(WinUsbHandle, IntIn, inputBuffer, inputBuffer.Length, ref Transferred, IntPtr.Zero);
                if (Readed && Transferred > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch { }
            return false;
        }

        public bool ReadBulkPipe(byte[] inputBuffer)
        {
            try
            {
                if (!IsActive)
                {
                    return false;
                }

                int Transferred = 0;
                bool Readed = WinUsb_ReadPipe(WinUsbHandle, BulkIn, inputBuffer, inputBuffer.Length, ref Transferred, IntPtr.Zero);
                if (Readed && Transferred > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch { }
            return false;
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

        public bool WriteControlTransfer(byte RequestType, byte Request, ushort Value, byte[] Buffer, ref int Transferred)
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

        public bool WriteDeviceIO(byte[] Input, byte[] Output)
        {
            if (!IsActive)
            {
                return false;
            }

            return DeviceIoControl(FileHandle, IoControlCodes.IOCTL_DEVICE_SENDDATA, Input, Input.Length, Output, Output.Length, out uint Transferred, IntPtr.Zero) && Transferred > 0;
        }
    }
}