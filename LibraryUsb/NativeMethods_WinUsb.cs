using System;
using System.Runtime.InteropServices;

namespace LibraryUsb
{
    internal static class NativeMethods_WinUsb
    {
        internal const int FILE_ATTRIBUTE_NORMAL = 0x80;

        internal enum USBD_PIPE_TYPE
        {
            UsbdPipeTypeControl = 0,
            UsbdPipeTypeIsochronous = 1,
            UsbdPipeTypeBulk = 2,
            UsbdPipeTypeInterrupt = 3,
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct USB_INTERFACE_DESCRIPTOR
        {
            internal byte bLength;
            internal byte bDescriptorType;
            internal byte bInterfaceNumber;
            internal byte bAlternateSetting;
            internal byte bNumEndpoints;
            internal byte bInterfaceClass;
            internal byte bInterfaceSubClass;
            internal byte bInterfaceProtocol;
            internal byte iInterface;
        }

        [StructLayout(LayoutKind.Explicit, Size = 18, CharSet = CharSet.Auto)]
        internal struct USB_DEVICE_DESCRIPTOR
        {
            [FieldOffset(0)] internal byte bLength;
            [FieldOffset(1)] internal byte bDescriptorType;
            [FieldOffset(2)] internal ushort bcdUSB;
            [FieldOffset(4)] internal byte bDeviceClass;
            [FieldOffset(5)] internal byte bDeviceSubClass;
            [FieldOffset(6)] internal byte bDeviceProtocol;
            [FieldOffset(7)] internal byte bMaxPacketSize0;
            [FieldOffset(8)] internal ushort idVendor;
            [FieldOffset(10)] internal ushort idProduct;
            [FieldOffset(12)] internal ushort bcdDevice;
            [FieldOffset(14)] internal byte iVendor;
            [FieldOffset(15)] internal byte iProduct;
            [FieldOffset(16)] internal byte iSerialNumber;
            [FieldOffset(17)] internal byte bNumConfigurations;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WINUSB_PIPE_INFORMATION
        {
            internal USBD_PIPE_TYPE PipeType;
            internal byte PipeId;
            internal ushort MaximumPacketSize;
            internal byte Interval;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct WINUSB_SETUP_PACKET
        {
            internal byte RequestType;
            internal byte Request;
            internal ushort Value;
            internal ushort Index;
            internal ushort Length;
        }

        [DllImport("winusb.dll")]
        internal static extern bool WinUsb_Initialize(IntPtr DeviceHandle, ref IntPtr InterfaceHandle);

        [DllImport("winusb.dll")]
        internal static extern bool WinUsb_GetDescriptor(IntPtr InterfaceHandle, byte DescriptorType, byte Index, ushort LanguageID, ref USB_DEVICE_DESCRIPTOR UsbAltDeviceDescriptor, int BufferLength, ref int LengthTransferred);

        [DllImport("winusb.dll")]
        internal static extern bool WinUsb_QueryInterfaceSettings(IntPtr InterfaceHandle, byte AlternateInterfaceNumber, ref USB_INTERFACE_DESCRIPTOR UsbAltInterfaceDescriptor);

        [DllImport("winusb.dll")]
        internal static extern bool WinUsb_QueryPipe(IntPtr InterfaceHandle, byte AlternateInterfaceNumber, byte PipeIndex, ref WINUSB_PIPE_INFORMATION PipeInformation);

        [DllImport("winusb.dll")]
        internal static extern bool WinUsb_AbortPipe(IntPtr InterfaceHandle, byte PipeID);

        [DllImport("winusb.dll")]
        internal static extern bool WinUsb_FlushPipe(IntPtr InterfaceHandle, byte PipeID);

        [DllImport("winusb.dll")]
        internal static extern bool WinUsb_ControlTransfer(IntPtr InterfaceHandle, WINUSB_SETUP_PACKET SetupPacket, byte[] Buffer, int BufferLength, ref int LengthTransferred, IntPtr Overlapped);

        [DllImport("winusb.dll")]
        internal static extern bool WinUsb_ReadPipe(IntPtr InterfaceHandle, byte PipeID, byte[] Buffer, int BufferLength, ref int LengthTransferred, IntPtr Overlapped);

        [DllImport("winusb.dll")]
        internal static extern bool WinUsb_WritePipe(IntPtr InterfaceHandle, byte PipeID, byte[] Buffer, int BufferLength, ref int LengthTransferred, IntPtr Overlapped);

        [DllImport("winusb.dll")]
        internal static extern bool WinUsb_Free(IntPtr InterfaceHandle);
    }
}