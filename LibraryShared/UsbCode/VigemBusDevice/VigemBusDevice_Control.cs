using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropDll;
using static LibraryShared.Classes;
using static LibraryUsb.NativeMethods_IoControl;

namespace LibraryUsb
{
    public partial class VigemBusDevice : WinUsbDevice
    {
        public VigemBusDevice(Guid deviceGuid, bool initialize, bool closeDevice) : base(deviceGuid, initialize, closeDevice) { }
        public VigemBusDevice(string devicePath, string deviceInstanceId, bool initialize, bool closeDevice) : base(devicePath, deviceInstanceId, initialize, closeDevice) { }

        public bool VirtualPlugin(int controllerNumber)
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
        }

        public bool VirtualUnplug(int controllerNumber)
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
        }

        public bool VirtualUnplugAll()
        {
            try
            {
                bool unplug0 = VirtualUnplug(0);
                bool unplug1 = VirtualUnplug(1);
                bool unplug2 = VirtualUnplug(2);
                bool unplug3 = VirtualUnplug(3);
                return unplug0 && unplug1 && unplug2 && unplug3;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to unplug all devices: " + ex.Message);
                return false;
            }
        }

        public bool VirtualInput(ControllerStatus controller)
        {
            try
            {
                if (!Connected) { return false; }

                ////Send device control code
                //bool iocontrol = DeviceIoControl(FileHandle, (uint)IoControlCodesVirtual.VIGEM_INPUT, controller.VirtualDataInput, controller.VirtualDataInput.Length, null, 0, out int bytesWritten, controller.InputVirtualOverlapped);

                ////Get overlapped result
                //if (!iocontrol && Marshal.GetLastWin32Error() == (int)IoErrorCodes.ERROR_IO_PENDING)
                //{
                //    if (WaitForSingleObject(controller.InputVirtualOverlapped.EventHandle, INFINITE) == WaitObjectResult.WAIT_OBJECT_0)
                //    {
                //        return GetOverlappedResult(FileHandle, ref controller.InputVirtualOverlapped, out int bytesTransferred, false);
                //    }
                //}
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to send virtual bus input: " + ex.Message);
                return false;
            }
        }

        public bool VirtualOutput(ref ControllerStatus controller)
        {
            try
            {
                if (!Connected) { return false; }

                ////Send device control code
                //bool iocontrol = DeviceIoControl(FileHandle, (uint)IoControlCodesVirtual.VIGEM_OUTPUT, controller.VirtualDataInput, controller.VirtualDataInput.Length, controller.VirtualDataOutput, controller.VirtualDataOutput.Length, out int bytesWritten, controller.OutputVirtualOverlapped);

                ////Get overlapped result
                //if (!iocontrol && Marshal.GetLastWin32Error() == (int)IoErrorCodes.ERROR_IO_PENDING)
                //{
                //    //.NET8 crashes here with ExecutionEngineException
                //    if (WaitForSingleObject(controller.OutputVirtualOverlapped.EventHandle, INFINITE) == WaitObjectResult.WAIT_OBJECT_0)
                //    {
                //        return GetOverlappedResult(FileHandle, ref controller.OutputVirtualOverlapped, out int bytesTransferred, false);
                //    }
                //}
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to read virtual bus output: " + ex.Message);
                return false;
            }
        }
    }
}