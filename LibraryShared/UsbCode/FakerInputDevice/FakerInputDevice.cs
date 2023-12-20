using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static ArnoldVinkCode.AVDevices.Enumerate;
using static ArnoldVinkCode.AVDevices.Interop;
using static LibraryUsb.NativeMethods_File;
using static LibraryUsb.NativeMethods_Guid;

namespace LibraryUsb
{
    public partial class FakerInputDevice
    {
        public bool Connected;
        public bool Exclusive;
        public string DevicePath;
        private SafeFileHandle FileHandle;
        private const byte KBD_KEY_CODES = 6;
        private const int CONTROL_REPORT_SIZE = 65;
        private string FAKER_VENDOR_HEXID = "0xFE0F";
        private string FAKER_PRODUCT_HEXID = "0x00FF";

        public FakerInputDevice()
        {
            try
            {
                FindDevice();
                OpenDevice();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to create FakerInput device: " + ex.Message);
            }
        }

        private void FindDevice()
        {
            try
            {
                //Find the device path
                IEnumerable<EnumerateInfo> SelectedHidDevice = EnumerateDevicesSetupApi(GuidClassHidDevice, true);
                foreach (EnumerateInfo EnumDevice in SelectedHidDevice)
                {
                    try
                    {
                        //Read information from the device
                        HidDevice foundHidDevice = new HidDevice(EnumDevice.DevicePath, EnumDevice.DeviceInstanceId, false, true);

                        //Check if device has attributes
                        if (foundHidDevice.Attributes == null) { continue; }

                        //Check if device has capabilities
                        if (foundHidDevice.Capabilities == null) { continue; }

                        //Check if device is FakerInput
                        if (foundHidDevice.Attributes.ProductHexId == FAKER_PRODUCT_HEXID && foundHidDevice.Attributes.VendorHexId == FAKER_VENDOR_HEXID)
                        {
                            if (foundHidDevice.Capabilities.UsagePage == 65280 && foundHidDevice.Capabilities.UsageGeneric == 1)
                            {
                                DevicePath = EnumDevice.DevicePath;
                                break;
                            }
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to find FakerInput device: " + ex.Message);
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
                FileFlagsAndAttributes flagsAttributes = FileFlagsAndAttributes.FILE_FLAG_NORMAL | FileFlagsAndAttributes.FILE_FLAG_NO_BUFFERING | FileFlagsAndAttributes.FILE_FLAG_WRITE_THROUGH;

                //Try to open the device exclusively
                FileHandle = CreateFile(DevicePath, desiredAccess, shareModeExclusive, IntPtr.Zero, creationDisposition, flagsAttributes, IntPtr.Zero);
                Exclusive = true;

                //Try to open the device normally
                if (FileHandle == null || FileHandle.IsInvalid || FileHandle.IsClosed)
                {
                    //Debug.WriteLine("Failed to open FakerInput device exclusively, opening normally.");
                    FileHandle = CreateFile(DevicePath, desiredAccess, shareModeNormal, IntPtr.Zero, creationDisposition, flagsAttributes, IntPtr.Zero);
                    Exclusive = false;
                }

                //Check if the device is opened
                if (FileHandle == null || FileHandle.IsInvalid || FileHandle.IsClosed)
                {
                    //Debug.WriteLine("Failed to open FakerInput device: " + DevicePath);
                    Connected = false;
                    return false;
                }
                else
                {
                    //Debug.WriteLine("Opened FakerInput device: " + DevicePath + ", exclusively: " + Exclusive);
                    Connected = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to open FakerInput device: " + ex.Message);
                Connected = false;
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
                Debug.WriteLine("Failed to close FakerInput device: " + ex.Message);
                return false;
            }
        }
    }
}