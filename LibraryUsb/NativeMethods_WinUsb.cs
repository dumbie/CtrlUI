using System;
using System.Runtime.InteropServices;

namespace LibraryUsb
{
    public static class NativeMethods_WinUsb
    {
        public enum USBD_PIPE_TYPE
        {
            UsbdPipeTypeControl = 0,
            UsbdPipeTypeIsochronous = 1,
            UsbdPipeTypeBulk = 2,
            UsbdPipeTypeInterrupt = 3
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct USB_INTERFACE_DESCRIPTOR
        {
            public byte bLength;
            public byte bDescriptorType;
            public byte bInterfaceNumber;
            public byte bAlternateSetting;
            public byte bNumEndpoints;
            public byte bInterfaceClass;
            public byte bInterfaceSubClass;
            public byte bInterfaceProtocol;
            public byte iInterface;
        }

        [StructLayout(LayoutKind.Explicit, Size = 18, CharSet = CharSet.Auto)]
        public struct USB_DEVICE_DESCRIPTOR
        {
            [FieldOffset(0)] public byte bLength;
            [FieldOffset(1)] public byte bDescriptorType;
            [FieldOffset(2)] public ushort bcdUSB;
            [FieldOffset(4)] public byte bDeviceClass;
            [FieldOffset(5)] public byte bDeviceSubClass;
            [FieldOffset(6)] public byte bDeviceProtocol;
            [FieldOffset(7)] public byte bMaxPacketSize0;
            [FieldOffset(8)] public ushort idVendor;
            [FieldOffset(10)] public ushort idProduct;
            [FieldOffset(12)] public ushort bcdDevice;
            [FieldOffset(14)] public byte iVendor;
            [FieldOffset(15)] public byte iProduct;
            [FieldOffset(16)] public byte iSerialNumber;
            [FieldOffset(17)] public byte bNumConfigurations;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINUSB_PIPE_INFORMATION
        {
            public USBD_PIPE_TYPE PipeType;
            public byte PipeId;
            public ushort MaximumPacketSize;
            public byte Interval;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct WINUSB_SETUP_PACKET
        {
            public byte RequestType;
            public byte Request;
            public ushort Value;
            public ushort Index;
            public ushort Length;
        }

        [DllImport("winusb.dll")]
        public static extern bool WinUsb_Initialize(IntPtr DeviceHandle, ref IntPtr InterfaceHandle);

        [DllImport("winusb.dll")]
        public static extern bool WinUsb_GetDescriptor(IntPtr InterfaceHandle, byte DescriptorType, byte Index, ushort LanguageID, ref USB_DEVICE_DESCRIPTOR UsbAltDeviceDescriptor, int BufferLength, ref int LengthTransferred);

        [DllImport("winusb.dll")]
        public static extern bool WinUsb_QueryInterfaceSettings(IntPtr InterfaceHandle, byte AlternateInterfaceNumber, ref USB_INTERFACE_DESCRIPTOR UsbAltInterfaceDescriptor);

        [DllImport("winusb.dll")]
        public static extern bool WinUsb_QueryPipe(IntPtr InterfaceHandle, byte AlternateInterfaceNumber, byte PipeIndex, ref WINUSB_PIPE_INFORMATION PipeInformation);

        [DllImport("winusb.dll")]
        public static extern bool WinUsb_AbortPipe(IntPtr InterfaceHandle, byte PipeID);

        [DllImport("winusb.dll")]
        public static extern bool WinUsb_FlushPipe(IntPtr InterfaceHandle, byte PipeID);

        [DllImport("winusb.dll")]
        public static extern bool WinUsb_ControlTransfer(IntPtr InterfaceHandle, WINUSB_SETUP_PACKET SetupPacket, byte[] Buffer, int BufferLength, ref int LengthTransferred, IntPtr Overlapped);

        [DllImport("winusb.dll")]
        public static extern bool WinUsb_ReadPipe(IntPtr InterfaceHandle, byte PipeID, byte[] Buffer, int BufferLength, ref int LengthTransferred, IntPtr Overlapped);

        [DllImport("winusb.dll")]
        public static extern bool WinUsb_WritePipe(IntPtr InterfaceHandle, byte PipeID, byte[] Buffer, int BufferLength, ref int LengthTransferred, IntPtr Overlapped);

        [DllImport("winusb.dll")]
        public static extern bool WinUsb_Free(IntPtr InterfaceHandle);
    }
}