using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static LibraryUsb.NativeMethods_Bth;
using static LibraryUsb.NativeMethods_File;

namespace LibraryUsb
{
    public partial class BthDevice
    {
        public static BLUETOOTH_ADDRESS? GetLocalBluetoothMacAddress()
        {
            IntPtr radioHandle = IntPtr.Zero;
            IntPtr bluetoothHandle = IntPtr.Zero;
            try
            {
                BLUETOOTH_FIND_RADIO_PARAMS radioFindParams = new BLUETOOTH_FIND_RADIO_PARAMS();
                radioFindParams.dwSize = Marshal.SizeOf(radioFindParams);
                radioHandle = BluetoothFindFirstRadio(ref radioFindParams, ref bluetoothHandle);
                if (radioHandle == IntPtr.Zero)
                {
                    Debug.WriteLine("No bluetooth radio found to get mac address for.");
                    return null;
                }

                BLUETOOTH_RADIO_INFO radioInfo = new BLUETOOTH_RADIO_INFO();
                radioInfo.dwSize = Marshal.SizeOf(radioInfo);
                if (BluetoothGetRadioInfo(radioHandle, ref radioInfo))
                {
                    Debug.WriteLine("Bluetooth local mac address: " + radioInfo.address.byte1 + ":" + radioInfo.address.byte2 + ":" + radioInfo.address.byte3 + ":" + radioInfo.address.byte4 + ":" + radioInfo.address.byte5 + ":" + radioInfo.address.byte6);
                    return radioInfo.address;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get local bluetooth mac address: " + ex.Message);
                return null;
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