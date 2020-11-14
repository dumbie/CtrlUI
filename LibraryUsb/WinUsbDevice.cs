using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static LibraryUsb.NativeMethods_DeviceManager;
using static LibraryUsb.NativeMethods_Flags;
using static LibraryUsb.NativeMethods_SetupApi;
using static LibraryUsb.NativeMethods_WinUsb;

namespace LibraryUsb
{
    public partial class WinUsbDevice
    {
        public WinUsbDevice() { }
        public WinUsbDevice(string Class)
        {
            GuidClass = new Guid(Class);
        }

        public Guid GuidClass = Guid.Empty;
        public string DevicePath { get; set; }
        public bool Connected { get; set; }
        public bool IsActive = false;

        public IntPtr FileHandle = IntPtr.Zero;
        public IntPtr WinUsbHandle = (IntPtr)INVALID_HANDLE_VALUE;

        public byte IntIn = 0xFF;
        public byte IntOut = 0xFF;
        public byte BulkIn = 0xFF;
        public byte BulkOut = 0xFF;

        public bool OpenDeviceClass(bool Initialize)
        {
            string DevicePath = string.Empty;
            if (Find(GuidClass, ref DevicePath))
            {
                return OpenDevicePath(DevicePath, Initialize);
            }
            return false;
        }

        public bool OpenDevicePath(string devicePath, bool Initialize)
        {
            DevicePath = devicePath.ToUpper();
            if (OpenDevice())
            {
                if (Initialize)
                {
                    if (WinUsb_Initialize(FileHandle, ref WinUsbHandle))
                    {
                        if (InitializeDevice())
                        {
                            IsActive = true;
                        }
                        else
                        {
                            WinUsb_Free(WinUsbHandle);
                            WinUsbHandle = (IntPtr)INVALID_HANDLE_VALUE;
                        }
                    }
                    else
                    {
                        CloseHandle(FileHandle);
                    }
                }
                else
                {
                    IsActive = true;
                }
            }

            return IsActive;
        }

        public bool CloseDevice()
        {
            try
            {
                IsActive = false;

                if (WinUsbHandle != (IntPtr)INVALID_HANDLE_VALUE)
                {
                    WinUsb_AbortPipe(WinUsbHandle, IntIn);
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

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to close win device: " + ex.Message);
                return false;
            }
        }

        public bool Plugin(int Serial)
        {
            if (IsActive)
            {
                byte[] Buffer = new byte[16];

                Buffer[0] = 0x10;
                Buffer[1] = 0x00;
                Buffer[2] = 0x00;
                Buffer[3] = 0x00;

                Serial++;
                Buffer[4] = (byte)((Serial >> 0) & 0xFF);
                Buffer[5] = (byte)((Serial >> 8) & 0xFF);
                Buffer[6] = (byte)((Serial >> 16) & 0xFF);
                Buffer[7] = (byte)((Serial >> 24) & 0xFF);

                return DeviceIoControl(FileHandle, IoControlCodes.IOCTL_DEVICE_CONNECT, Buffer, Buffer.Length, null, 0, out uint Transferred, IntPtr.Zero);
            }
            return false;
        }

        public bool Unplug(int Serial)
        {
            if (IsActive)
            {
                byte[] Buffer = new byte[16];

                Buffer[0] = 0x10;
                Buffer[1] = 0x00;
                Buffer[2] = 0x00;
                Buffer[3] = 0x00;

                Serial++;
                Buffer[4] = (byte)((Serial >> 0) & 0xFF);
                Buffer[5] = (byte)((Serial >> 8) & 0xFF);
                Buffer[6] = (byte)((Serial >> 16) & 0xFF);
                Buffer[7] = (byte)((Serial >> 24) & 0xFF);

                return DeviceIoControl(FileHandle, IoControlCodes.IOCTL_DEVICE_DISCONNECT, Buffer, Buffer.Length, null, 0, out uint Transferred, IntPtr.Zero);
            }
            return false;
        }

        public bool UnplugAll()
        {
            if (IsActive)
            {
                byte[] Buffer = new byte[16];

                Buffer[0] = 0x10;
                Buffer[1] = 0x00;
                Buffer[2] = 0x00;
                Buffer[3] = 0x00;

                return DeviceIoControl(FileHandle, IoControlCodes.IOCTL_DEVICE_DISCONNECT, Buffer, Buffer.Length, null, 0, out uint Transfered, IntPtr.Zero);
            }
            return false;
        }

        private bool Find(Guid Target, ref string Path)
        {
            IntPtr detailDataBuffer = IntPtr.Zero;
            IntPtr deviceInfoSet = IntPtr.Zero;

            try
            {
                SP_DEVICE_INTERFACE_DATA DeviceInterfaceData = new SP_DEVICE_INTERFACE_DATA(), da = new SP_DEVICE_INTERFACE_DATA();
                int bufferSize = 0;
                int memberIndex = 0;

                deviceInfoSet = SetupDiGetClassDevs(ref Target, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);

                DeviceInterfaceData.cbSize = da.cbSize = Marshal.SizeOf(DeviceInterfaceData);

                while (SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref Target, memberIndex, ref DeviceInterfaceData))
                {
                    SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref DeviceInterfaceData, IntPtr.Zero, 0, ref bufferSize, ref da);
                    {
                        detailDataBuffer = Marshal.AllocHGlobal(bufferSize);

                        Marshal.WriteInt32(detailDataBuffer, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);

                        if (SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref DeviceInterfaceData, detailDataBuffer, bufferSize, ref bufferSize, ref da))
                        {
                            IntPtr pDevicePathName = detailDataBuffer + 4;

                            Path = Marshal.PtrToStringAuto(pDevicePathName).ToUpper();
                            Marshal.FreeHGlobal(detailDataBuffer);

                            if (memberIndex == 0)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            Marshal.FreeHGlobal(detailDataBuffer);
                        }
                    }

                    memberIndex++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.HelpLink + " / " + ex.Message);
            }

            if (deviceInfoSet != IntPtr.Zero)
            {
                SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }

            if (detailDataBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(detailDataBuffer);
            }

            return false;
        }

        private bool GetDeviceInstance(ref string Instance)
        {
            IntPtr detailDataBuffer = IntPtr.Zero;
            IntPtr deviceInfoSet = IntPtr.Zero;

            try
            {
                SP_DEVICE_INTERFACE_DATA DeviceInterfaceData = new SP_DEVICE_INTERFACE_DATA(), da = new SP_DEVICE_INTERFACE_DATA();
                int bufferSize = 0;
                int memberIndex = 0;

                deviceInfoSet = SetupDiGetClassDevs(ref GuidClass, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);

                DeviceInterfaceData.cbSize = da.cbSize = Marshal.SizeOf(DeviceInterfaceData);

                while (SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref GuidClass, memberIndex, ref DeviceInterfaceData))
                {
                    SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref DeviceInterfaceData, IntPtr.Zero, 0, ref bufferSize, ref da);
                    {
                        detailDataBuffer = Marshal.AllocHGlobal(bufferSize);

                        Marshal.WriteInt32(detailDataBuffer, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);

                        if (SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref DeviceInterfaceData, detailDataBuffer, bufferSize, ref bufferSize, ref da))
                        {
                            IntPtr pDevicePathName = detailDataBuffer + 4;

                            string Current = Marshal.PtrToStringAuto(pDevicePathName).ToUpper();
                            Marshal.FreeHGlobal(detailDataBuffer);

                            if (Current == DevicePath)
                            {
                                int nBytes = 256;
                                IntPtr ptrInstanceBuf = Marshal.AllocHGlobal(nBytes);

                                CM_Get_Device_ID(da.Flags, ptrInstanceBuf, nBytes, 0);
                                Instance = Marshal.PtrToStringAuto(ptrInstanceBuf).ToUpper();

                                Marshal.FreeHGlobal(ptrInstanceBuf);
                                return true;
                            }
                        }
                        else
                        {
                            Marshal.FreeHGlobal(detailDataBuffer);
                        }
                    }

                    memberIndex++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.HelpLink + " / " + ex.Message);
            }

            if (deviceInfoSet != IntPtr.Zero)
            {
                SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }

            if (detailDataBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(detailDataBuffer);
            }

            return false;
        }

        private bool OpenDevice()
        {
            try
            {
                uint shareMode = (uint)FILE_SHARE.FILE_SHARE_READ | (uint)FILE_SHARE.FILE_SHARE_WRITE;
                uint desiredAccess = (uint)GENERIC_MODE.GENERIC_READ | (uint)GENERIC_MODE.GENERIC_WRITE;
                uint fileAttributes = (uint)FILE_ATTRIBUTE.FILE_ATTRIBUTE_NORMAL | (uint)FILE_FLAG.FILE_FLAG_OVERLAPPED;
                FileHandle = CreateFile(DevicePath, desiredAccess, shareMode, IntPtr.Zero, OPEN_EXISTING, fileAttributes, 0);
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

        private bool UsbEndpointDirectionIn(int addr)
        {
            return (addr & 0x80) == 0x80;
        }

        private bool UsbEndpointDirectionOut(int addr)
        {
            return (addr & 0x80) == 0x00;
        }

        private bool InitializeDevice()
        {
            try
            {
                USB_INTERFACE_DESCRIPTOR ifaceDescriptor = new USB_INTERFACE_DESCRIPTOR();
                WINUSB_PIPE_INFORMATION pipeInfo = new WINUSB_PIPE_INFORMATION();

                if (WinUsb_QueryInterfaceSettings(WinUsbHandle, 0, ref ifaceDescriptor))
                {
                    for (int i = 0; i < ifaceDescriptor.bNumEndpoints; i++)
                    {
                        WinUsb_QueryPipe(WinUsbHandle, 0, Convert.ToByte(i), ref pipeInfo);

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
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.HelpLink + " / " + ex.Message);
            }
            return false;
        }

        private bool RestartDevice(string InstanceId)
        {
            IntPtr deviceInfoSet = IntPtr.Zero;

            try
            {
                SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();

                deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);
                deviceInfoSet = SetupDiGetClassDevs(ref GuidClass, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);

                if (SetupDiOpenDeviceInfo(deviceInfoSet, InstanceId, IntPtr.Zero, 0, ref deviceInterfaceData))
                {
                    SP_PROPCHANGE_PARAMS props = new SP_PROPCHANGE_PARAMS();

                    props.classInstallHeader = new SP_CLASSINSTALL_HEADER();
                    props.classInstallHeader.cbSize = Marshal.SizeOf(props.classInstallHeader);
                    props.classInstallHeader.installFunction = DIF_PROPERTYCHANGE;

                    props.Scope = DICS_FLAG_GLOBAL;
                    props.stateChange = DICS_PROPCHANGE;
                    props.hwProfile = 0x00;

                    if (SetupDiSetClassInstallParams(deviceInfoSet, ref deviceInterfaceData, ref props, Marshal.SizeOf(props)))
                    {
                        return SetupDiChangeState(deviceInfoSet, ref deviceInterfaceData);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.HelpLink + " / " + ex.Message);
            }

            if (deviceInfoSet != IntPtr.Zero)
            {
                SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }

            return false;
        }
    }
}