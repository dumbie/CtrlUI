using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.IO;
using static LibraryUsb.NativeMethods_File;

namespace LibraryUsb
{
    public partial class HidDevice
    {
        public bool Connected;
        public bool Exclusive;
        public string DevicePath;
        public string DeviceInstanceId;
        private SafeFileHandle FileHandle;
        private FileStream FileStream;
        public HidDeviceAttributes Attributes;
        public HidDeviceCapabilities Capabilities;

        public HidDevice(string devicePath, string deviceInstanceId, bool initialize, bool closeDevice)
        {
            try
            {
                DevicePath = devicePath.ToLower();
                DeviceInstanceId = deviceInstanceId.ToLower();

                if (initialize)
                {
                    DisableDevice();
                    EnableDevice();
                }

                if (OpenDevice())
                {
                    if (initialize)
                    {
                        OpenFileStream();
                    }
                    GetDeviceAttributes();
                    GetDeviceCapabilities();
                    //GetDeviceButtonStatus();
                    //GetDeviceValueStatus();
                    GetProductName();
                    GetVendorName();
                    GetSerialNumber();
                    if (closeDevice)
                    {
                        CloseDevice();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to create hid device: " + ex.Message);
            }
        }

        private bool OpenDevice()
        {
            try
            {
                FileShareMode shareModeExclusive = FileShareMode.FILE_SHARE_NONE;
                FileShareMode shareModeNormal = FileShareMode.FILE_SHARE_READ | FileShareMode.FILE_SHARE_WRITE;
                FileDesiredAccess desiredAccess = FileDesiredAccess.GENERIC_READ | FileDesiredAccess.GENERIC_WRITE;
                FileCreationDisposition creationDisposition = FileCreationDisposition.OPEN_EXISTING;
                FileFlagsAndAttributes flagsAttributes = FileFlagsAndAttributes.FILE_FLAG_NORMAL | FileFlagsAndAttributes.FILE_FLAG_OVERLAPPED | FileFlagsAndAttributes.FILE_FLAG_NO_BUFFERING | FileFlagsAndAttributes.FILE_FLAG_WRITE_THROUGH;

                //Try to open the device exclusively
                FileHandle = CreateFile(DevicePath, desiredAccess, shareModeExclusive, IntPtr.Zero, creationDisposition, flagsAttributes, IntPtr.Zero);
                Exclusive = true;

                //Try to open the device normally
                if (FileHandle == null || FileHandle.IsInvalid || FileHandle.IsClosed)
                {
                    //Debug.WriteLine("Failed to open device exclusively, opening normally.");
                    FileHandle = CreateFile(DevicePath, desiredAccess, shareModeNormal, IntPtr.Zero, creationDisposition, flagsAttributes, IntPtr.Zero);
                    Exclusive = false;
                }

                //Check if the device is opened
                if (FileHandle == null || FileHandle.IsInvalid || FileHandle.IsClosed)
                {
                    //Debug.WriteLine("Failed to open hid device: " + DevicePath);
                    Connected = false;
                    return false;
                }
                else
                {
                    //Debug.WriteLine("Opened hid device: " + DevicePath + ", exclusively: " + Exclusive);
                    Connected = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to open hid device: " + ex.Message);
                Connected = false;
                return false;
            }
        }

        private bool OpenFileStream()
        {
            try
            {
                FileStream = new FileStream(FileHandle, FileAccess.ReadWrite, 1, true);
                if (FileStream.CanTimeout)
                {
                    FileStream.ReadTimeout = 2000;
                    FileStream.WriteTimeout = 2000;
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to open hid file stream: " + ex.Message);
                return false;
            }
        }

        public bool CloseDevice()
        {
            try
            {
                if (FileStream != null)
                {
                    FileStream.Dispose();
                    FileStream = null;
                }
                if (FileHandle != null)
                {
                    FileHandle.Dispose();
                    FileHandle = null;
                }
                Connected = false;
                Exclusive = false;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to close hid device: " + ex.Message);
                return false;
            }
        }
    }
}