using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static LibraryUsb.NativeMethods_WinUsb;

namespace LibraryUsb
{
    public partial class WinUsbDevice
    {
        private bool UsbEndpointDirectionIn(int addr)
        {
            return (addr & 0x80) == 0x80;
        }

        private bool UsbEndpointDirectionOut(int addr)
        {
            return (addr & 0x80) == 0x00;
        }

        public USB_DEVICE_DESCRIPTOR? ReadDeviceDescriptor()
        {
            try
            {
                //Check if device is initialized
                if (!Initialized)
                {
                    Debug.WriteLine("Device needs to be initialized to read the descriptor.");
                    return null;
                }

                USB_DEVICE_DESCRIPTOR USB_DEVICE_DESCRIPTOR = new USB_DEVICE_DESCRIPTOR();
                int descriptorSize = Marshal.SizeOf(USB_DEVICE_DESCRIPTOR);
                bool readed = WinUsb_GetDescriptor(WinUsbHandle, DESCRIPTOR_TYPE.USB_DEVICE_DESCRIPTOR_TYPE, 0, 0, ref USB_DEVICE_DESCRIPTOR, descriptorSize, out int bytesRead) && bytesRead > 0;
                if (readed)
                {
                    return USB_DEVICE_DESCRIPTOR;
                }
                else
                {
                    Debug.WriteLine("Failed to read device descriptor.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to read device descriptor: " + ex.Message);
                return null;
            }
        }
    }
}