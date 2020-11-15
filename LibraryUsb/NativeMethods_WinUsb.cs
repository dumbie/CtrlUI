using System;
using System.Runtime.InteropServices;
using static LibraryUsb.NativeMethods_Variables;

namespace LibraryUsb
{
    public class NativeMethods_WinUsb
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

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, CREATION_FLAG dwCreationDisposition, uint dwFlagsAndAttributes, uint hTemplateFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr hObject);
    }
}