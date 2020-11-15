using System;
using System.Diagnostics;
using static LibraryUsb.DeviceManager;
using static LibraryUsb.NativeMethods_Variables;
using static LibraryUsb.NativeMethods_WinUsb;

namespace LibraryUsb
{
    public partial class WinUsbDevice
    {
        public bool Connected;
        public string DevicePath;
        public Guid DeviceGuid = Guid.Empty;
        private IntPtr FileHandle = IntPtr.Zero;
        private IntPtr WinUsbHandle = (IntPtr)INVALID_HANDLE_VALUE;
        public byte IntIn = 0xFF;
        public byte IntOut = 0xFF;
        public byte BulkIn = 0xFF;
        public byte BulkOut = 0xFF;

        public WinUsbDevice(Guid deviceGuid, string devicePath, bool initialize, bool closeDevice)
        {
            try
            {
                DeviceGuid = deviceGuid;
                DevicePath = devicePath;
                if (DeviceGuid != Guid.Empty && string.IsNullOrWhiteSpace(DevicePath))
                {
                    if (!DeviceFind(DeviceGuid, ref DevicePath, 0))
                    {
                        Debug.WriteLine("Create win device not found.");
                        return;
                    }
                }

                if (OpenDevice())
                {
                    if (initialize)
                    {
                        InitializeDevice();
                    }
                    if (closeDevice)
                    {
                        CloseDevice();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to create win device: " + ex.Message);
            }
        }

        private bool OpenDevice()
        {
            try
            {
                uint shareMode = (uint)FILE_SHARE.FILE_SHARE_READ | (uint)FILE_SHARE.FILE_SHARE_WRITE;
                uint desiredAccess = (uint)GENERIC_MODE.GENERIC_READ | (uint)GENERIC_MODE.GENERIC_WRITE;
                uint fileAttributes = (uint)FILE_ATTRIBUTE.FILE_ATTRIBUTE_NORMAL | (uint)FILE_FLAG.FILE_FLAG_OVERLAPPED;
                FileHandle = CreateFile(DevicePath, desiredAccess, shareMode, IntPtr.Zero, CREATION_FLAG.OPEN_EXISTING, fileAttributes, 0);
                if (FileHandle == IntPtr.Zero || FileHandle == (IntPtr)INVALID_HANDLE_VALUE)
                {
                    Connected = false;
                    return false;
                }
                else
                {
                    Connected = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to open win device: " + ex.Message);
                Connected = false;
                return false;
            }
        }

        public bool CloseDevice()
        {
            try
            {
                if (WinUsbHandle != (IntPtr)INVALID_HANDLE_VALUE)
                {
                    WinUsb_AbortPipe(WinUsbHandle, IntIn);
                    WinUsb_AbortPipe(WinUsbHandle, IntOut);
                    WinUsb_AbortPipe(WinUsbHandle, BulkIn);
                    WinUsb_AbortPipe(WinUsbHandle, BulkOut);
                    WinUsb_Free(WinUsbHandle);
                    WinUsbHandle = (IntPtr)INVALID_HANDLE_VALUE;
                }
                if (FileHandle != IntPtr.Zero)
                {
                    CloseHandle(FileHandle);
                    FileHandle = IntPtr.Zero;
                }
                Connected = false;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to close win device: " + ex.Message);
                return false;
            }
        }

        private bool InitializeDevice()
        {
            try
            {
                if (!WinUsb_Initialize(FileHandle, ref WinUsbHandle))
                {
                    Debug.WriteLine("Failed to initialize win device.");
                    return false;
                }

                WINUSB_PIPE_INFORMATION pipeInfo = new WINUSB_PIPE_INFORMATION();
                USB_INTERFACE_DESCRIPTOR interfaceDescriptor = new USB_INTERFACE_DESCRIPTOR();

                if (WinUsb_QueryInterfaceSettings(WinUsbHandle, 0, ref interfaceDescriptor))
                {
                    for (byte i = 0; i < interfaceDescriptor.bNumEndpoints; i++)
                    {
                        WinUsb_QueryPipe(WinUsbHandle, 0, i, ref pipeInfo);
                        if ((pipeInfo.PipeType == USBD_PIPE_TYPE.UsbdPipeTypeBulk) & UsbEndpointDirectionIn(pipeInfo.PipeId))
                        {
                            BulkIn = pipeInfo.PipeId;
                            WinUsb_FlushPipe(WinUsbHandle, BulkIn);
                        }
                        else if ((pipeInfo.PipeType == USBD_PIPE_TYPE.UsbdPipeTypeBulk) & UsbEndpointDirectionOut(pipeInfo.PipeId))
                        {
                            BulkOut = pipeInfo.PipeId;
                            WinUsb_FlushPipe(WinUsbHandle, BulkOut);
                        }
                        else if ((pipeInfo.PipeType == USBD_PIPE_TYPE.UsbdPipeTypeInterrupt) & UsbEndpointDirectionIn(pipeInfo.PipeId))
                        {
                            IntIn = pipeInfo.PipeId;
                            WinUsb_FlushPipe(WinUsbHandle, IntIn);
                        }
                        else if ((pipeInfo.PipeType == USBD_PIPE_TYPE.UsbdPipeTypeInterrupt) & UsbEndpointDirectionOut(pipeInfo.PipeId))
                        {
                            IntOut = pipeInfo.PipeId;
                            WinUsb_FlushPipe(WinUsbHandle, IntOut);
                        }
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to initialize win device: " + ex.Message);
                return false;
            }
        }

        private bool UsbEndpointDirectionIn(int addr)
        {
            return (addr & 0x80) == 0x80;
        }

        private bool UsbEndpointDirectionOut(int addr)
        {
            return (addr & 0x80) == 0x00;
        }
    }
}