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
        public void DisconnectBluetooth()
        {
            try
            {
                Debug.WriteLine("Attempting to disconnect bluetooth device.");

                //Get the device serial number
                string MacAddressRaw = Attributes.SerialNumber;

                //Set and parse the mac address
                byte[] MacAddressBytes = new byte[8];
                string[] MacAddressSplit = { $"{MacAddressRaw[0]}{MacAddressRaw[1]}", $"{MacAddressRaw[2]}{MacAddressRaw[3]}", $"{MacAddressRaw[4]}{MacAddressRaw[5]}", $"{MacAddressRaw[6]}{MacAddressRaw[7]}", $"{MacAddressRaw[8]}{MacAddressRaw[9]}", $"{MacAddressRaw[10]}{MacAddressRaw[11]}" };
                for (int i = 0; i < 6; i++)
                {
                    MacAddressBytes[5 - i] = Convert.ToByte(MacAddressSplit[i], 16);
                }

                Debug.WriteLine("Disconnecting bluetooth device: " + MacAddressRaw);

                //Disconnect the device from bluetooth
                BLUETOOTH_FIND_RADIO_PARAMS RadioFindParams = new BLUETOOTH_FIND_RADIO_PARAMS();
                RadioFindParams.dwSize = Marshal.SizeOf(RadioFindParams);

                IntPtr BluetoothHandle = IntPtr.Zero;
                IntPtr RadioHandle = BluetoothFindFirstRadio(ref RadioFindParams, ref BluetoothHandle);

                bool ControllerDisconnected = false;
                while (!ControllerDisconnected)
                {
                    ControllerDisconnected = DeviceIoControl(BluetoothHandle, IoControlCodes.IOCTL_BTH_DISCONNECT_DEVICE, MacAddressBytes, MacAddressBytes.Length, null, 0, out uint Transferred, IntPtr.Zero);
                    CloseHandle(BluetoothHandle);
                    if (!ControllerDisconnected)
                    {
                        if (!BluetoothFindNextRadio(RadioHandle, ref BluetoothHandle))
                        {
                            ControllerDisconnected = true;
                        }
                    }
                }

                BluetoothFindRadioClose(RadioHandle);
                Debug.WriteLine("Bluetooth disconnected succesfully: " + ControllerDisconnected);
            }
            catch { }
        }
    }
}