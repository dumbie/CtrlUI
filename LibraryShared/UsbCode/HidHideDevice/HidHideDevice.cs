using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using static LibraryUsb.NativeMethods_File;

namespace LibraryUsb
{
    public partial class HidHideDevice
    {
        public bool Connected;
        public bool Installed;
        public bool Exclusive;
        private SafeFileHandle FileHandle;

        public HidHideDevice()
        {
            try
            {
                OpenDevice();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to create hid hide device: " + ex.Message);
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
                FileFlagsAndAttributes flagsAttributes = FileFlagsAndAttributes.FILE_FLAG_NORMAL | FileFlagsAndAttributes.FILE_FLAG_OVERLAPPED | FileFlagsAndAttributes.FILE_FLAG_NO_BUFFERING;

                //Try to open the device exclusively
                FileHandle = CreateFile("\\\\.\\HidHide", desiredAccess, shareModeExclusive, IntPtr.Zero, creationDisposition, flagsAttributes, IntPtr.Zero);
                Exclusive = true;

                //Try to open the device normally
                if (FileHandle == null || FileHandle.IsInvalid || FileHandle.IsClosed)
                {
                    //Debug.WriteLine("Failed to open device exclusively, opening normally.");
                    FileHandle = CreateFile("\\\\.\\HidHide", desiredAccess, shareModeNormal, IntPtr.Zero, creationDisposition, flagsAttributes, IntPtr.Zero);
                    Exclusive = false;
                }

                //Check if the device is opened
                if (FileHandle == null || FileHandle.IsInvalid || FileHandle.IsClosed)
                {
                    //Debug.WriteLine("Failed to open hid hide device: " + DevicePath);
                    Connected = false;
                    Installed = false;
                    return false;
                }
                else
                {
                    //Debug.WriteLine("Opened hid hide device: " + DevicePath + ", exclusively: " + Exclusive);
                    Connected = true;
                    Installed = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to open hid hide device: " + ex.Message);
                Connected = false;
                Installed = false;
                return false;
            }
        }

        public bool CloseDevice()
        {
            try
            {
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
                Debug.WriteLine("Failed to close hid hide device: " + ex.Message);
                return false;
            }
        }
    }
}