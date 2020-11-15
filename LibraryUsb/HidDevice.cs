using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static LibraryUsb.HidDeviceAttributes;
using static LibraryUsb.HidDeviceCapabilities;
using static LibraryUsb.NativeMethods_Hid;
using static LibraryUsb.NativeMethods_Variables;

namespace LibraryUsb
{
    public partial class HidDevice
    {
        public bool Connected;
        public string DevicePath;
        public string HardwareId;
        private IntPtr FileHandle;
        public HidDeviceAttributes Attributes;
        public HidDeviceCapabilities Capabilities;

        public HidDevice(string devicePath, string hardwareId, bool initialize, bool closeDevice)
        {
            try
            {
                DevicePath = devicePath;
                HardwareId = hardwareId;

                if (initialize)
                {
                    DisableDevice();
                    EnableDevice();
                }

                if (OpenDevice())
                {
                    GetDeviceAttributes();
                    GetDeviceCapabilities();
                    GetFeature();
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
                uint shareMode = (uint)FILE_SHARE.FILE_SHARE_READ | (uint)FILE_SHARE.FILE_SHARE_WRITE;
                uint desiredAccess = (uint)GENERIC_MODE.GENERIC_READ | (uint)GENERIC_MODE.GENERIC_WRITE;
                uint fileAttributes = (uint)FILE_ATTRIBUTE.FILE_ATTRIBUTE_NORMAL | (uint)FILE_FLAG.FILE_FLAG_NORMAL;
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
                Debug.WriteLine("Failed to open hid device: " + ex.Message);
                Connected = false;
                return false;
            }
        }

        public bool CloseDevice()
        {
            try
            {
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
                Debug.WriteLine("Failed to close hid device: " + ex.Message);
                return false;
            }
        }

        private bool GetFeature()
        {
            try
            {
                int featureLength = Capabilities.FeatureReportByteLength;
                if (featureLength <= 0) { featureLength = 64; }

                byte[] data = new byte[featureLength];
                data[0] = 0x05;
                return HidD_GetFeature(FileHandle, data, data.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get feature: " + ex.Message);
                return false;
            }
        }

        private bool GetDeviceAttributes()
        {
            try
            {
                HIDD_ATTRIBUTES hiddDeviceAttributes = new HIDD_ATTRIBUTES();
                hiddDeviceAttributes.Size = Marshal.SizeOf(hiddDeviceAttributes);
                HidD_GetAttributes(FileHandle, ref hiddDeviceAttributes);
                Attributes = new HidDeviceAttributes(hiddDeviceAttributes);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get device attributes: " + ex.Message);
                return false;
            }
        }

        private bool GetDeviceCapabilities()
        {
            IntPtr preparsedDataPointer = IntPtr.Zero;
            try
            {
                HIDP_CAPS deviceCapabilities = new HIDP_CAPS();
                if (HidD_GetPreparsedData(FileHandle, ref preparsedDataPointer))
                {
                    HidP_GetCaps(preparsedDataPointer, ref deviceCapabilities);
                }
                Capabilities = new HidDeviceCapabilities(deviceCapabilities);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get device capabilities: " + ex.Message);
                return false;
            }
            finally
            {
                if (preparsedDataPointer != IntPtr.Zero)
                {
                    HidD_FreePreparsedData(preparsedDataPointer);
                }
            }
        }

        private bool GetProductName()
        {
            try
            {
                byte[] data = new byte[254];
                HidD_GetProductString(FileHandle, ref data[0], data.Length);
                string productNameString = data.ToUTF16String().Replace("\0", string.Empty);
                if (!string.IsNullOrWhiteSpace(productNameString))
                {
                    Attributes.ProductName = productNameString;
                }
                else
                {
                    Attributes.ProductName = Attributes.ProductHexId + " Unknown";
                }
                return true;
            }
            catch (Exception ex)
            {
                Attributes.ProductName = Attributes.ProductHexId + " Unknown";
                Debug.WriteLine("Failed to get product name: " + ex.Message);
                return false;
            }
        }

        public bool GetVendorName()
        {
            try
            {
                byte[] data = new byte[254];
                HidD_GetManufacturerString(FileHandle, ref data[0], data.Length);
                string vendorNameString = data.ToUTF16String().Replace("\0", string.Empty);
                if (!string.IsNullOrWhiteSpace(vendorNameString))
                {
                    Attributes.VendorName = vendorNameString;
                }
                else
                {
                    Attributes.VendorName = Attributes.VendorHexId + " Unknown";
                }
                return true;
            }
            catch (Exception ex)
            {
                Attributes.VendorName = Attributes.VendorHexId + " Unknown";
                Debug.WriteLine("Failed to get vendor name: " + ex.Message);
                return false;
            }
        }

        public bool GetSerialNumber()
        {
            try
            {
                byte[] data = new byte[254];
                HidD_GetSerialNumberString(FileHandle, ref data[0], data.Length);
                string serialNumberString = data.ToUTF16String().Replace("\0", "");
                if (!string.IsNullOrWhiteSpace(serialNumberString))
                {
                    Attributes.SerialNumber = serialNumberString;
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get serial number: " + ex.Message);
                return false;
            }
        }
    }
}