using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInteropDll;
using static LibraryShared.Classes;
using static LibraryUsb.NativeMethods_IoControl;

namespace LibraryUsb
{
    public partial class VigemBusDevice : WinUsbDevice
    {
        public VigemBusDevice(Guid deviceGuid, bool initialize, bool closeDevice) : base(deviceGuid, initialize, closeDevice) { }
        public VigemBusDevice(string devicePath, string deviceInstanceId, bool initialize, bool closeDevice) : base(devicePath, deviceInstanceId, initialize, closeDevice) { }

        public async Task<bool> VirtualPlugin(int controllerNumber)
        {
            try
            {
                if (!Connected) { return false; }

                //Set buffer header
                byte[] writeBuffer = new byte[(int)ByteArraySizes.Plugin];
                writeBuffer[0] = (byte)ByteArraySizes.Plugin; //Size
                writeBuffer[4] = (byte)(controllerNumber + 1); //SerialNo
                writeBuffer[8] = (byte)VIGEM_TARGET_TYPE.Xbox360Wired; //TargetType

                //Send device control code
                return DeviceIoControl(FileHandle, (uint)IoControlCodesVirtual.VIGEM_PLUGIN, writeBuffer, writeBuffer.Length, null, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
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

                //Send device control code
                return DeviceIoControl(FileHandle, (uint)IoControlCodesVirtual.VIGEM_UNPLUG, writeBuffer, writeBuffer.Length, null, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
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
                bool unplug0 = await VirtualUnplug(0 + VirtualIdOffset);
                bool unplug1 = await VirtualUnplug(1 + VirtualIdOffset);
                bool unplug2 = await VirtualUnplug(2 + VirtualIdOffset);
                bool unplug3 = await VirtualUnplug(3 + VirtualIdOffset);
                return unplug0 && unplug1 && unplug2 && unplug3;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to unplug all devices: " + ex.Message);
                return false;
            }
        }

        public bool VirtualInput(ref ControllerStatus controller)
        {
            IntPtr createEvent = CreateEvent(IntPtr.Zero, true, false, null);
            try
            {
                if (!Connected) { return false; }

                //Create native overlapped
                NativeOverlapped nativeOverlapped = new NativeOverlapped();
                nativeOverlapped.EventHandle = createEvent;

                //Send device control code
                bool iocontrol = DeviceIoControl(FileHandle, (uint)IoControlCodesVirtual.VIGEM_INPUT, controller.VirtualDataInput, controller.VirtualDataInput.Length, null, 0, out int bytesWritten, ref nativeOverlapped);

                //Get overlapped result
                if (!iocontrol && Marshal.GetLastWin32Error() == (int)IoErrorCodes.ERROR_IO_PENDING)
                {
                    if (WaitForSingleObject(nativeOverlapped.EventHandle, INFINITE) == WaitObjectResult.WAIT_OBJECT_0)
                    {
                        return GetOverlappedResult(FileHandle, ref nativeOverlapped, out int bytesTransferred, false);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to send virtual bus input: " + ex.Message);
                return false;
            }
            finally
            {
                SafeCloseHandle(ref createEvent);
            }
        }

        public bool VirtualOutput(ref ControllerStatus controller)
        {
            IntPtr createEvent = CreateEvent(IntPtr.Zero, true, false, null);
            try
            {
                //Check if controller is connected
                if (!controller.Connected())
                {
                    Debug.WriteLine("Virtual output controller is not connected: " + controller.NumberId);
                    return false;
                }

                //Check if virtual bus is connected
                if (!Connected)
                {
                    Debug.WriteLine("Virtual output bus is not connected: " + controller.NumberId);
                    return false;
                }

                //Create native overlapped
                NativeOverlapped nativeOverlapped = new NativeOverlapped();
                nativeOverlapped.EventHandle = createEvent;

                //Send device control code
                bool iocontrol = DeviceIoControl(FileHandle, (uint)IoControlCodesVirtual.VIGEM_OUTPUT, controller.VirtualDataInput, controller.VirtualDataInput.Length, controller.VirtualDataOutput, controller.VirtualDataOutput.Length, out int bytesWritten, ref nativeOverlapped);

                //Get overlapped result
                if (!iocontrol && Marshal.GetLastWin32Error() == (int)IoErrorCodes.ERROR_IO_PENDING)
                {
                    if (WaitForSingleObject(nativeOverlapped.EventHandle, INFINITE) == WaitObjectResult.WAIT_OBJECT_0)
                    {
                        return GetOverlappedResult(FileHandle, ref nativeOverlapped, out int bytesTransferred, false);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to read virtual bus output: " + ex.Message);
                return false;
            }
            finally
            {
                SafeCloseHandle(ref createEvent);
            }
        }
    }
}