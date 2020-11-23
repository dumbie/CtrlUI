using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static LibraryUsb.HidDeviceAttributes;
using static LibraryUsb.HidDeviceCapabilities;
using static LibraryUsb.NativeMethods_Hid;

namespace LibraryUsb
{
    public partial class HidDevice
    {
        public byte[] GetFeature(byte featureByte)
        {
            try
            {
                int featureLength = Capabilities.FeatureReportByteLength;
                if (featureLength <= 0) { featureLength = 64; }
                byte[] featureData = new byte[featureLength];
                featureData[0] = featureByte;

                if (HidD_GetFeature(FileHandle, featureData, featureData.Length))
                {
                    return featureData;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get feature: " + ex.Message);
                return null;
            }
        }

        private bool GetDeviceAttributes()
        {
            try
            {
                HIDD_ATTRIBUTES hiddDeviceAttributes = new HIDD_ATTRIBUTES();
                hiddDeviceAttributes.Size = Marshal.SizeOf(hiddDeviceAttributes);
                if (HidD_GetAttributes(FileHandle, ref hiddDeviceAttributes))
                {
                    Attributes = new HidDeviceAttributes(hiddDeviceAttributes);
                    return true;
                }
                else
                {
                    Debug.WriteLine("Failed to get device attributes.");
                    return false;
                }
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
                if (HidD_GetPreparsedData(FileHandle, ref preparsedDataPointer))
                {
                    HIDP_CAPS deviceCapabilities = new HIDP_CAPS();
                    HidP_GetCaps(preparsedDataPointer, ref deviceCapabilities);
                    Capabilities = new HidDeviceCapabilities(deviceCapabilities);
                    return true;
                }
                else
                {
                    Debug.WriteLine("Failed to get device capabilities.");
                    return false;
                }
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
                    return true;
                }
                else
                {
                    Attributes.ProductName = Attributes.ProductHexId + " Unknown";
                    return false;
                }
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
                    return true;
                }
                else
                {
                    Attributes.VendorName = Attributes.VendorHexId + " Unknown";
                    return false;
                }
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
                string serialNumberString = data.ToUTF16String().Replace("\0", string.Empty);
                if (!string.IsNullOrWhiteSpace(serialNumberString))
                {
                    Attributes.SerialNumber = serialNumberString;
                    return true;
                }
                else
                {
                    Attributes.SerialNumber = string.Empty;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get serial number: " + ex.Message);
                return false;
            }
        }
    }
}