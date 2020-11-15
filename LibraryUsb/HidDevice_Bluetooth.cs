using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static LibraryUsb.NativeMethods_Bluetooth;
using static LibraryUsb.NativeMethods_DeviceManager;

namespace LibraryUsb
{
    public partial class HidDevice
    {
        //Disconnect the device from bluetooth
        public bool DisconnectBluetooth()
        {
            IntPtr radioHandle = IntPtr.Zero;
            IntPtr bluetoothHandle = IntPtr.Zero;
            try
            {
                Debug.WriteLine("Attempting to disconnect bluetooth device.");

                //Get and parse the mac address
                string macAddressRaw = Attributes.SerialNumber;
                byte[] macAddressBytes = new byte[8];
                string[] macAddressSplit = { $"{macAddressRaw[0]}{macAddressRaw[1]}", $"{macAddressRaw[2]}{macAddressRaw[3]}", $"{macAddressRaw[4]}{macAddressRaw[5]}", $"{macAddressRaw[6]}{macAddressRaw[7]}", $"{macAddressRaw[8]}{macAddressRaw[9]}", $"{macAddressRaw[10]}{macAddressRaw[11]}" };
                for (int i = 0; i < 6; i++)
                {
                    macAddressBytes[5 - i] = Convert.ToByte(macAddressSplit[i], 16);
                }

                Debug.WriteLine("Disconnecting bluetooth device: " + macAddressRaw);

                //Disconnect the device from bluetooth
                BLUETOOTH_FIND_RADIO_PARAMS radioFindParams = new BLUETOOTH_FIND_RADIO_PARAMS();
                radioFindParams.dwSize = Marshal.SizeOf(radioFindParams);
                radioHandle = BluetoothFindFirstRadio(ref radioFindParams, ref bluetoothHandle);

                bool bluetoothDisconnected = false;
                while (!bluetoothDisconnected)
                {
                    bluetoothDisconnected = DeviceIoControl(bluetoothHandle, IoControlCodes.IOCTL_BTH_DISCONNECT_DEVICE, macAddressBytes, macAddressBytes.Length, null, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
                    if (!bluetoothDisconnected)
                    {
                        if (!BluetoothFindNextRadio(radioHandle, ref bluetoothHandle))
                        {
                            bluetoothDisconnected = true;
                        }
                    }
                }

                Debug.WriteLine("Succesfully disconnected bluetooth: " + bluetoothDisconnected);
                return bluetoothDisconnected;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed disconnecting bluetooth: " + ex.Message);
                return false;
            }
            finally
            {
                if (radioHandle != IntPtr.Zero)
                {
                    BluetoothFindRadioClose(radioHandle);
                }
                if (bluetoothHandle != IntPtr.Zero)
                {
                    CloseHandle(bluetoothHandle);
                }
            }
        }
    }
}