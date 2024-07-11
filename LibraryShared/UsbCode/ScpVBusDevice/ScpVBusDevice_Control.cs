using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static LibraryShared.Classes;
using static LibraryUsb.NativeMethods_IoControl;

namespace LibraryUsb
{
    public partial class ScpVBusDevice : WinUsbDevice
    {
        public ScpVBusDevice(Guid deviceGuid, bool initialize, bool closeDevice) : base(deviceGuid, initialize, closeDevice) { }
        public ScpVBusDevice(string devicePath, string deviceInstanceId, bool initialize, bool closeDevice) : base(devicePath, deviceInstanceId, initialize, closeDevice) { }

        public async Task<bool> VirtualPlugin(int controllerNumber)
        {
            try
            {
                if (!Connected) { return false; }

                //Set buffer header
                byte[] writeBuffer = new byte[(int)ByteArraySizes.Plugin];
                writeBuffer[0] = (byte)ByteArraySizes.Plugin; //Size
                writeBuffer[4] = (byte)(controllerNumber + 1); //SerialNo

                //Send device control code
                return DeviceIoControl(FileHandle, (uint)IoControlCodesVirtual.SCP_PLUGIN, writeBuffer, writeBuffer.Length, null, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to plugin device: " + ex.Message);
                return false;
            }
            finally
            {
                await Task.Delay(100);
            }
        }

        public async Task<bool> VirtualUnplug(int controllerNumber)
        {
            try
            {
                if (!Connected) { return false; }

                //Set buffer header
                byte[] writeBuffer = new byte[(int)ByteArraySizes.Unplug];
                writeBuffer[0] = (byte)ByteArraySizes.Unplug; //Size
                writeBuffer[4] = (byte)(controllerNumber + 1); //SerialNo
                writeBuffer[8] = 0x0001; //FlagForce

                //Send device control code
                return DeviceIoControl(FileHandle, (uint)IoControlCodesVirtual.SCP_UNPLUG, writeBuffer, writeBuffer.Length, null, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to unplug device: " + ex.Message);
                return false;
            }
            finally
            {
                await Task.Delay(100);
            }
        }

        public async Task<bool> VirtualUnplugAll()
        {
            try
            {
                if (!Connected) { return false; }

                //Set buffer header
                byte[] writeBuffer = new byte[16];
                writeBuffer[0] = 0x10; //Size
                writeBuffer[8] = 0x0001; //FlagForce

                //Send device control code
                return DeviceIoControl(FileHandle, (uint)IoControlCodesVirtual.SCP_UNPLUG, writeBuffer, writeBuffer.Length, null, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to unplug all devices: " + ex.Message);
                return false;
            }
            finally
            {
                await Task.Delay(100);
            }
        }

        public bool VirtualReadWrite(ref ControllerStatus controller)
        {
            try
            {
                if (!Connected) { return false; }

                //Send device control code
                return DeviceIoControl(FileHandle, (uint)IoControlCodesVirtual.SCP_REPORT, controller.VirtualDataInput, controller.VirtualDataInput.Length, controller.VirtualDataOutput, controller.VirtualDataOutput.Length, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to read write virtual bus: " + ex.Message);
                return false;
            }
        }
    }
}