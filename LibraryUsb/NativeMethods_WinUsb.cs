using System;
using System.Runtime.InteropServices;

namespace LibraryUsb
{
    public class NativeMethods_WinUsb
    {
        public enum USBD_PIPE_TYPE : int
        {
            Control = 0,
            Isochronous = 1,
            Bulk = 2,
            Interrupt = 3
        }

        public enum DESCRIPTOR_TYPE : byte
        {
            USB_DEVICE_DESCRIPTOR_TYPE = 0x01,
            USB_CONFIGURATION_DESCRIPTOR_TYPE = 0x02,
            USB_STRING_DESCRIPTOR_TYPE = 0x03
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct USB_DEVICE_DESCRIPTOR
        {
            public byte bLength;
            public byte bDescriptorType;
            public ushort bcdUSB;
            public byte bDeviceClass;
            public byte bDeviceSubClass;
            public byte bDeviceProtocol;
            public byte bMaxPacketSize0;
            public ushort idVendor;
            public ushort idProduct;
            public ushort bcdDevice;
            public byte iManufacturer;
            public byte iProduct;
            public byte iSerialNumber;
            public byte bNumConfigurations;
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

        [StructLayout(LayoutKind.Sequential)]
        public struct WINUSB_PIPE_INFORMATION
        {
            public USBD_PIPE_TYPE PipeType;
            public byte PipeId;
            public ushort MaximumPacketSize;
            public byte Interval;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINUSB_SETUP_PACKET
        {
            public byte RequestType;
            public byte Request;
            public ushort Value;
            public ushort Index;
            public ushort Length;
        }

        [DllImport("winusb.dll")]
        public static extern bool WinUsb_Initialize(IntPtr deviceHandle, ref IntPtr interfaceHandle);

        [DllImport("winusb.dll")]
        public static extern bool WinUsb_GetDescriptor(IntPtr InterfaceHandle, DESCRIPTOR_TYPE DescriptorType, byte Index, ushort LanguageID, ref USB_DEVICE_DESCRIPTOR deviceDesc, int BufferLength, out int LengthTransfered);

        [DllImport("winusb.dll")]
        public static extern bool WinUsb_QueryInterfaceSettings(IntPtr interfaceHandle, byte alternateInterfaceNumber, ref USB_INTERFACE_DESCRIPTOR usbAltInterfaceDescriptor);

        [DllImport("winusb.dll")]
        public static extern bool WinUsb_QueryPipe(IntPtr interfaceHandle, byte alternateInterfaceNumber, byte PipeIndex, ref WINUSB_PIPE_INFORMATION pipeInformation);

        [DllImport("winusb.dll")]
        public static extern bool WinUsb_AbortPipe(IntPtr interfaceHandle, byte pipeID);

        [DllImport("winusb.dll")]
        public static extern bool WinUsb_FlushPipe(IntPtr interfaceHandle, byte pipeID);

        [DllImport("winusb.dll")]
        public static extern bool WinUsb_ControlTransfer(IntPtr interfaceHandle, WINUSB_SETUP_PACKET setupPacket, byte[] buffer, int bufferLength, out int lengthTransferred, IntPtr overlapped);

        [DllImport("winusb.dll")]
        public static extern bool WinUsb_WritePipe(IntPtr interfaceHandle, byte pipeID, byte[] buffer, int bufferLength, out int lengthTransferred, IntPtr overlapped);

        [DllImport("winusb.dll")]
        public static extern bool WinUsb_ReadPipe(IntPtr interfaceHandle, byte pipeID, byte[] buffer, int bufferLength, out int lengthTransferred, IntPtr overlapped);

        [DllImport("winusb.dll")]
        public static extern bool WinUsb_Free(IntPtr interfaceHandle);
    }
}