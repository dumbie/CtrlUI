using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static LibraryUsb.NativeMethods_Hid;

namespace LibraryUsb
{
    public partial class HidDevice
    {
        public string DevicePath { get; }
        public IntPtr DeviceHandle { get; set; }
        public string Description { get; }
        public string HardwareId { get; }
        public HidDeviceCapabilities Capabilities { get; }
        public HidDeviceAttributes Attributes { get; }

        public HidDevice(string devicePath, string description, string hardwareId)
        {
            try
            {
                DevicePath = devicePath;
                Description = description;
                HardwareId = hardwareId;

                OpenDeviceExclusively();
                CheckConnectionState();
                Attributes = GetDeviceAttributes();
                Capabilities = GetDeviceCapabilities();
                CloseDevice();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error querying HID device: " + DevicePath + " / " + ex.Message);
            }
        }

        //Use GetFeature to timeout disconnected device
        public void CheckConnectionState()
        {
            try
            {
                byte[] FeatureBytes = new byte[64];
                FeatureBytes[0] = 0x02;
                HidD_GetFeature(DeviceHandle, FeatureBytes, FeatureBytes.Length);
            }
            catch { }
        }

        public bool GetProductDescription(out byte[] data)
        {
            data = new byte[254];
            try
            {
                return HidD_GetProductString(DeviceHandle, ref data[0], data.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error accessing HID device: " + DevicePath + " / " + ex.Message);
                return false;
            }
        }

        public bool GetManufacturer(out byte[] data)
        {
            data = new byte[254];
            try
            {
                return HidD_GetManufacturerString(DeviceHandle, ref data[0], data.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error accessing HID device: " + DevicePath + " / " + ex.Message);
                return false;
            }
        }

        public bool GetSerialNumber(out byte[] data)
        {
            data = new byte[254];
            try
            {
                return HidD_GetSerialNumberString(DeviceHandle, ref data[0], data.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error accessing HID device: " + DevicePath + " / " + ex.Message);
                return false;
            }
        }

        private HidDeviceAttributes GetDeviceAttributes()
        {
            try
            {
                HIDD_ATTRIBUTES hiddDeviceAttributes = new HIDD_ATTRIBUTES();
                hiddDeviceAttributes.Size = Marshal.SizeOf(hiddDeviceAttributes);
                HidD_GetAttributes(DeviceHandle, ref hiddDeviceAttributes);
                HidDeviceAttributes returnDeviceAttributes = new HidDeviceAttributes(hiddDeviceAttributes);

                //Get the product name
                GetProductDescription(out byte[] productNameBytes);
                if (productNameBytes != null)
                {
                    string productNameString = productNameBytes.ToUTF16String().Replace("\0", "");
                    if (!string.IsNullOrWhiteSpace(productNameString))
                    {
                        returnDeviceAttributes.ProductName = productNameString;
                    }
                    else
                    {
                        returnDeviceAttributes.ProductName = returnDeviceAttributes.ProductHexId + " Unknown";
                    }
                }
                else
                {
                    returnDeviceAttributes.ProductName = returnDeviceAttributes.ProductHexId + " Unknown";
                }

                //Get the vendor name
                GetManufacturer(out byte[] vendorNameBytes);
                if (vendorNameBytes != null)
                {
                    string vendorNameString = vendorNameBytes.ToUTF16String().Replace("\0", "");
                    if (!string.IsNullOrWhiteSpace(vendorNameString))
                    {
                        returnDeviceAttributes.VendorName = vendorNameString;
                    }
                    else
                    {
                        returnDeviceAttributes.VendorName = returnDeviceAttributes.VendorHexId + " Unknown";
                    }
                }
                else
                {
                    returnDeviceAttributes.VendorName = returnDeviceAttributes.VendorHexId + " Unknown";
                }

                //Get the serial number
                GetSerialNumber(out byte[] SerialNumberBytes);
                if (SerialNumberBytes != null)
                {
                    returnDeviceAttributes.SerialNumber = SerialNumberBytes.ToUTF16String().Replace("\0", "");
                }
                else
                {
                    returnDeviceAttributes.SerialNumber = string.Empty;
                }

                return returnDeviceAttributes;
            }
            catch
            {
                return null;
            }
        }

        private HidDeviceCapabilities GetDeviceCapabilities()
        {
            try
            {
                IntPtr preparsedDataPointer = IntPtr.Zero;
                HIDP_CAPS deviceCapabilities = new HIDP_CAPS();

                if (HidD_GetPreparsedData(DeviceHandle, ref preparsedDataPointer))
                {
                    HidP_GetCaps(preparsedDataPointer, ref deviceCapabilities);
                    HidD_FreePreparsedData(preparsedDataPointer);
                }

                return new HidDeviceCapabilities(deviceCapabilities);
            }
            catch
            {
                return null;
            }
        }

        private IntPtr DeviceOpen(string devicePath, uint shareMode)
        {
            IntPtr CreateResult = IntPtr.Zero;
            try
            {
                SECURITY_ATTRIBUTES security = new SECURITY_ATTRIBUTES();
                security.lpSecurityDescriptor = IntPtr.Zero;
                security.bInheritHandle = true;
                security.nLength = Marshal.SizeOf(security);

                uint fileAttributes = (uint)FILE_ATTRIBUTE.FILE_ATTRIBUTE_NORMAL | (uint)FILE_FLAG.FILE_FLAG_NORMAL;
                uint desiredAccess = (uint)GENERIC_MODE.GENERIC_WRITE | (uint)GENERIC_MODE.GENERIC_READ;

                CreateResult = CreateFile(devicePath, desiredAccess, shareMode, ref security, OPEN_EXISTING, fileAttributes, 0);
            }
            catch { }
            return CreateResult;
        }

        public void CloseDevice()
        {
            try
            {
                CancelIoEx(DeviceHandle, IntPtr.Zero);
                CloseHandle(DeviceHandle);
            }
            catch { }
        }
    }
}