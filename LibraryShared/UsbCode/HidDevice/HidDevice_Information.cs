using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVDevices.Extensions;
using static LibraryUsb.HidDeviceAttributes;
using static LibraryUsb.HidDeviceButtonValues;
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

        public bool SetFeature(byte[] featureByte)
        {
            try
            {
                return HidD_SetFeature(FileHandle, featureByte, featureByte.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to set feature: " + ex.Message);
                return false;
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

        private List<int> GetDeviceButtonStatus()
        {
            //Sony PlayStation DualSense 5 only reports 14 out of 15 buttons
            List<int> buttonStatus = new List<int>();
            IntPtr preparsedDataPointer = IntPtr.Zero;
            try
            {
                Debug.WriteLine("Getting device button status.");
                if (HidD_GetPreparsedData(FileHandle, ref preparsedDataPointer))
                {
                    int buttonCapsLength = Capabilities.NumberInputButtonCaps;
                    if (buttonCapsLength > 0)
                    {
                        ButtonValueCaps[] buttonCaps = new ButtonValueCaps[buttonCapsLength];
                        HidP_GetButtonCaps(HIDP_REPORT_TYPE.HidP_Input, buttonCaps, ref buttonCapsLength, preparsedDataPointer);
                        foreach (ButtonValueCaps buttonCap in buttonCaps)
                        {
                            //Get input report
                            byte[] reportBuffer = new byte[Capabilities.InputReportByteLength];
                            reportBuffer[0] = buttonCap.ReportID;
                            HidD_GetInputReport(FileHandle, reportBuffer, reportBuffer.Length);

                            //Get number of buttons
                            int numberOfButtons = buttonCap.Range.UsageMax - buttonCap.Range.UsageMin + 1;

                            //Check pressed buttons
                            ushort[] usageList = new ushort[numberOfButtons];
                            HidP_GetUsages(HIDP_REPORT_TYPE.HidP_Input, buttonCap.UsagePage, 0, usageList, ref numberOfButtons, preparsedDataPointer, reportBuffer, reportBuffer.Length);

                            //Add pressed buttons
                            for (int i = 0; i < numberOfButtons; i++)
                            {
                                int pressedButton = usageList[i] - buttonCap.Range.UsageMin;
                                Debug.WriteLine("Button " + pressedButton + " is pressed.");
                                buttonStatus.Add(pressedButton);
                            }
                        }
                    }
                    return buttonStatus;
                }
                else
                {
                    Debug.WriteLine("Failed to get device buttons.");
                    return buttonStatus;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get device buttons: " + ex.Message);
                return buttonStatus;
            }
            finally
            {
                if (preparsedDataPointer != IntPtr.Zero)
                {
                    HidD_FreePreparsedData(preparsedDataPointer);
                }
            }
        }

        private List<int> GetDeviceValueStatus()
        {
            List<int> valueStatus = new List<int>();
            IntPtr preparsedDataPointer = IntPtr.Zero;
            try
            {
                Debug.WriteLine("Getting device value status.");
                if (HidD_GetPreparsedData(FileHandle, ref preparsedDataPointer))
                {
                    int valueCapsLength = Capabilities.NumberInputValueCaps;
                    if (valueCapsLength > 0)
                    {
                        ButtonValueCaps[] valueCaps = new ButtonValueCaps[valueCapsLength];
                        HidP_GetValueCaps(HIDP_REPORT_TYPE.HidP_Input, valueCaps, ref valueCapsLength, preparsedDataPointer);
                        foreach (ButtonValueCaps valueCap in valueCaps)
                        {
                            //Get input report
                            byte[] reportBuffer = new byte[Capabilities.InputReportByteLength];
                            reportBuffer[0] = valueCap.ReportID;
                            HidD_GetInputReport(FileHandle, reportBuffer, reportBuffer.Length);

                            //Get usage value
                            HidP_GetUsageValue(HIDP_REPORT_TYPE.HidP_Input, valueCap.UsagePage, 0, valueCap.UsageMin, out uint usageValue, preparsedDataPointer, reportBuffer, reportBuffer.Length);

                            //Show value in console
                            Debug.WriteLine(valueCap.NotRangeUsageName + "/" + usageValue);
                        }
                    }
                    return valueStatus;
                }
                else
                {
                    Debug.WriteLine("Failed to get device values.");
                    return valueStatus;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get device values: " + ex.Message);
                return valueStatus;
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
                //Check if device has attributes
                if (Attributes == null) { return false; }

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
                //Check if device has attributes
                if (Attributes == null) { return false; }

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
                //Check if device has attributes
                if (Attributes == null) { return false; }

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