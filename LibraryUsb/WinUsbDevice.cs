using System;
using System.Diagnostics;
using static LibraryUsb.DeviceManager;
using static LibraryUsb.NativeMethods_File;
using static LibraryUsb.NativeMethods_WinUsb;

namespace LibraryUsb
{
    public partial class WinUsbDevice
    {
        public bool Connected;
        public string DevicePath;
        public Guid DeviceGuid;
        private IntPtr FileHandle;
        private IntPtr WinUsbHandle = INVALID_HANDLE_VALUE;
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
                        Debug.WriteLine("Create winusb device not found.");
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
                Debug.WriteLine("Failed to create winusb device: " + ex.Message);
            }
        }

        private bool OpenDevice()
        {
            try
            {
                FileShareMode shareMode = FileShareMode.FILE_SHARE_READ | FileShareMode.FILE_SHARE_WRITE;
                FileDesiredAccess desiredAccess = FileDesiredAccess.GENERIC_READ | FileDesiredAccess.GENERIC_WRITE;
                FileCreationDisposition creationDisposition = FileCreationDisposition.OPEN_EXISTING;
                FileFlagsAndAttributes flagsAttributes = FileFlagsAndAttributes.FILE_FLAG_NORMAL | FileFlagsAndAttributes.FILE_ATTRIBUTE_NORMAL | FileFlagsAndAttributes.FILE_FLAG_OVERLAPPED;
                FileHandle = CreateFile(DevicePath, desiredAccess, shareMode, IntPtr.Zero, creationDisposition, flagsAttributes, 0);
                if (FileHandle == IntPtr.Zero || FileHandle == INVALID_HANDLE_VALUE)
                {
                    Debug.WriteLine("Failed to open winusb device.");
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
                Debug.WriteLine("Failed to open winusb device: " + ex.Message);
                Connected = false;
                return false;
            }
        }

        public bool CloseDevice()
        {
            try
            {
                if (WinUsbHandle != INVALID_HANDLE_VALUE)
                {
                    WinUsb_AbortPipe(WinUsbHandle, IntIn);
                    WinUsb_AbortPipe(WinUsbHandle, IntOut);
                    WinUsb_AbortPipe(WinUsbHandle, BulkIn);
                    WinUsb_AbortPipe(WinUsbHandle, BulkOut);
                    WinUsb_Free(WinUsbHandle);
                    WinUsbHandle = INVALID_HANDLE_VALUE;
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
                Debug.WriteLine("Failed to close winusb device: " + ex.Message);
                return false;
            }
        }

        private bool InitializeDevice()
        {
            try
            {
                if (!WinUsb_Initialize(FileHandle, ref WinUsbHandle))
                {
                    Debug.WriteLine("Failed to initialize winusb device.");
                    return false;
                }

                WINUSB_PIPE_INFORMATION pipeInfo = new WINUSB_PIPE_INFORMATION();
                USB_INTERFACE_DESCRIPTOR interfaceDescriptor = new USB_INTERFACE_DESCRIPTOR();

                if (WinUsb_QueryInterfaceSettings(WinUsbHandle, 0, ref interfaceDescriptor))
                {
                    for (byte i = 0; i < interfaceDescriptor.bNumEndpoints; i++)
                    {
                        WinUsb_QueryPipe(WinUsbHandle, 0, i, ref pipeInfo);
                        if (pipeInfo.PipeType == USBD_PIPE_TYPE.Bulk && UsbEndpointDirectionIn(pipeInfo.PipeId))
                        {
                            BulkIn = pipeInfo.PipeId;
                            WinUsb_FlushPipe(WinUsbHandle, BulkIn);
                        }
                        else if (pipeInfo.PipeType == USBD_PIPE_TYPE.Bulk && UsbEndpointDirectionOut(pipeInfo.PipeId))
                        {
                            BulkOut = pipeInfo.PipeId;
                            WinUsb_FlushPipe(WinUsbHandle, BulkOut);
                        }
                        else if (pipeInfo.PipeType == USBD_PIPE_TYPE.Interrupt && UsbEndpointDirectionIn(pipeInfo.PipeId))
                        {
                            IntIn = pipeInfo.PipeId;
                            WinUsb_FlushPipe(WinUsbHandle, IntIn);
                        }
                        else if (pipeInfo.PipeType == USBD_PIPE_TYPE.Interrupt && UsbEndpointDirectionOut(pipeInfo.PipeId))
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
                Debug.WriteLine("Failed to initialize winusb device: " + ex.Message);
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