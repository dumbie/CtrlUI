﻿using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Linq;
using static ArnoldVinkCode.AVDevices.Enumerate;
using static ArnoldVinkCode.AVDevices.Interop;
using static LibraryUsb.NativeMethods_File;
using static LibraryUsb.NativeMethods_WinUsb;

namespace LibraryUsb
{
    public partial class WinUsbDevice
    {
        public bool Connected;
        public bool Initialized;
        public string DevicePath;
        public string DeviceInstanceId;
        public SafeFileHandle FileHandle;
        private IntPtr WinUsbHandle;
        public byte IntIn = 0xFF;
        public byte IntOut = 0xFF;
        public byte BulkIn = 0xFF;
        public byte BulkOut = 0xFF;

        public WinUsbDevice(Guid deviceGuid, bool initialize, bool closeDevice)
        {
            try
            {
                EnumerateInfo enumerateInfo = EnumerateDevicesSetupApi(deviceGuid, true).FirstOrDefault();
                if (enumerateInfo != null)
                {
                    DevicePath = enumerateInfo.DevicePath.ToLower();
                    DeviceInstanceId = enumerateInfo.DeviceInstanceId.ToLower();
                }
                else
                {
                    Debug.WriteLine("Create winusb device not found.");
                    return;
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

        public WinUsbDevice(string devicePath, string deviceInstanceId, bool initialize, bool closeDevice)
        {
            try
            {
                DevicePath = devicePath.ToLower();
                DeviceInstanceId = deviceInstanceId.ToLower();

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
                FileShareMode shareModeNormal = FileShareMode.FILE_SHARE_READ | FileShareMode.FILE_SHARE_WRITE;
                FileDesiredAccess desiredAccess = FileDesiredAccess.GENERIC_READ | FileDesiredAccess.GENERIC_WRITE;
                FileCreationDisposition creationDisposition = FileCreationDisposition.OPEN_EXISTING;
                FileFlagsAndAttributes flagsAttributes = FileFlagsAndAttributes.FILE_FLAG_NORMAL | FileFlagsAndAttributes.FILE_FLAG_OVERLAPPED | FileFlagsAndAttributes.FILE_FLAG_NO_BUFFERING | FileFlagsAndAttributes.FILE_FLAG_WRITE_THROUGH;

                //Try to open the device normally
                FileHandle = CreateFile(DevicePath, desiredAccess, shareModeNormal, IntPtr.Zero, creationDisposition, flagsAttributes, IntPtr.Zero);

                //Check if the device is opened
                if (FileHandle == null || FileHandle.IsInvalid || FileHandle.IsClosed)
                {
                    //Debug.WriteLine("Failed to open winusb device: " + DevicePath);
                    Connected = false;
                    return false;
                }
                else
                {
                    //Debug.WriteLine("Opened winusb device: " + DevicePath);
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

        private bool InitializeDevice()
        {
            try
            {
                //Try to initialize the device
                if (!WinUsb_Initialize(FileHandle, out WinUsbHandle))
                {
                    Debug.WriteLine("Failed to initialize winusb device: " + DevicePath);
                    Initialized = false;
                    return false;
                }

                WINUSB_PIPE_INFORMATION pipeInformation = new WINUSB_PIPE_INFORMATION();
                USB_INTERFACE_DESCRIPTOR interfaceDescriptor = new USB_INTERFACE_DESCRIPTOR();

                if (WinUsb_QueryInterfaceSettings(WinUsbHandle, 0, ref interfaceDescriptor))
                {
                    for (byte i = 0; i < interfaceDescriptor.bNumEndpoints; i++)
                    {
                        WinUsb_QueryPipe(WinUsbHandle, 0, i, ref pipeInformation);
                        if (pipeInformation.PipeType == USBD_PIPE_TYPE.Bulk && UsbEndpointDirectionIn(pipeInformation.PipeId))
                        {
                            BulkIn = pipeInformation.PipeId;
                            WinUsb_FlushPipe(WinUsbHandle, BulkIn);
                        }
                        else if (pipeInformation.PipeType == USBD_PIPE_TYPE.Bulk && UsbEndpointDirectionOut(pipeInformation.PipeId))
                        {
                            BulkOut = pipeInformation.PipeId;
                            WinUsb_FlushPipe(WinUsbHandle, BulkOut);
                        }
                        else if (pipeInformation.PipeType == USBD_PIPE_TYPE.Interrupt && UsbEndpointDirectionIn(pipeInformation.PipeId))
                        {
                            IntIn = pipeInformation.PipeId;
                            WinUsb_FlushPipe(WinUsbHandle, IntIn);
                        }
                        else if (pipeInformation.PipeType == USBD_PIPE_TYPE.Interrupt && UsbEndpointDirectionOut(pipeInformation.PipeId))
                        {
                            IntOut = pipeInformation.PipeId;
                            WinUsb_FlushPipe(WinUsbHandle, IntOut);
                        }
                    }
                    //Debug.WriteLine("Initialized winusb device: " + DevicePath);
                    Initialized = true;
                    return true;
                }
                //Debug.WriteLine("Failed to initialize winusb device: " + DevicePath);
                Initialized = false;
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to initialize winusb device: " + ex.Message);
                Initialized = false;
                return false;
            }
        }

        public bool CloseDevice()
        {
            try
            {
                if (WinUsbHandle != IntPtr.Zero)
                {
                    WinUsb_AbortPipe(WinUsbHandle, IntIn);
                    WinUsb_AbortPipe(WinUsbHandle, IntOut);
                    WinUsb_AbortPipe(WinUsbHandle, BulkIn);
                    WinUsb_AbortPipe(WinUsbHandle, BulkOut);
                    WinUsb_Free(WinUsbHandle);
                    WinUsbHandle = IntPtr.Zero;
                }
                if (FileHandle != null)
                {
                    FileHandle.Dispose();
                    FileHandle = null;
                }
                Connected = false;
                Initialized = false;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to close winusb device: " + ex.Message);
                return false;
            }
        }
    }
}