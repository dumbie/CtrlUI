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
            IntPtr controlIntPtr = IntPtr.Zero;
            try
            {
                if (!Connected) { return false; }

                //Set marshal structure
                BUSENUM_PLUGIN_HARDWARE busenumPlugin = new BUSENUM_PLUGIN_HARDWARE();
                busenumPlugin.Size = Marshal.SizeOf(busenumPlugin);
                busenumPlugin.SerialNo = controllerNumber + 1;
                busenumPlugin.TargetType = VIGEM_TARGET_TYPE.Xbox360Wired;

                //Prepare marshal structure
                controlIntPtr = Marshal.AllocHGlobal(busenumPlugin.Size);
                Marshal.StructureToPtr(busenumPlugin, controlIntPtr, false);

                //Get device control code
                uint controlCode = CTL_CODE(FILE_DEVICE_TYPE.FILE_DEVICE_BUS_EXTENDER, FILE_ACCESS_DATA.FILE_WRITE_DATA, IO_FUNCTION.IOCTL_BUSENUM_PLUGIN_HARDWARE, IO_METHOD.METHOD_BUFFERED);

                //Send marshal structure
                return DeviceIoControl(FileHandle, controlCode, controlIntPtr, busenumPlugin.Size, IntPtr.Zero, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to plugin device: " + ex.Message);
                return false;
            }
            finally
            {
                SafeCloseMarshal(controlIntPtr);
            }
        }

        public bool VirtualUnplug(int controllerNumber)
        {
            IntPtr controlIntPtr = IntPtr.Zero;
            try
            {
                if (!Connected) { return false; }

                //Set marshal structure
                BUSENUM_UNPLUG_HARDWARE busenumUnplug = new BUSENUM_UNPLUG_HARDWARE();
                busenumUnplug.Size = Marshal.SizeOf(busenumUnplug);
                busenumUnplug.SerialNo = controllerNumber + 1;

                //Prepare marshal structure
                controlIntPtr = Marshal.AllocHGlobal(busenumUnplug.Size);
                Marshal.StructureToPtr(busenumUnplug, controlIntPtr, false);

                //Get device control code
                uint controlCode = CTL_CODE(FILE_DEVICE_TYPE.FILE_DEVICE_BUS_EXTENDER, FILE_ACCESS_DATA.FILE_WRITE_DATA, IO_FUNCTION.IOCTL_BUSENUM_UNPLUG_HARDWARE, IO_METHOD.METHOD_BUFFERED);

                //Send marshal structure
                return DeviceIoControl(FileHandle, controlCode, controlIntPtr, busenumUnplug.Size, IntPtr.Zero, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to unplug device: " + ex.Message);
                return false;
            }
            finally
            {
                SafeCloseMarshal(controlIntPtr);
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

        public bool VirtualInput(ref ControllerStatus controller)
        {
            IntPtr controlIntPtr = IntPtr.Zero;
            try
            {
                if (!Connected) { return false; }

                ////Prepare marshal structure
                //controlIntPtr = Marshal.AllocHGlobal(controller.XInputData.Size);
                //Marshal.StructureToPtr(controller.XInputData, controlIntPtr, false);

                ////Get device control code
                //uint controlCode = CTL_CODE(FILE_DEVICE_TYPE.FILE_DEVICE_BUS_EXTENDER, FILE_ACCESS_DATA.FILE_WRITE_DATA, IO_FUNCTION.IOCTL_XUSB_SUBMIT_REPORT, IO_METHOD.METHOD_BUFFERED);

                ////Send marshal structure
                //bool iocontrol = DeviceIoControl(FileHandle, controlCode, controlIntPtr, controller.XInputData.Size, controlIntPtr, controller.XInputData.Size, out int bytesWritten, controller.InputVirtualOverlapped);

                ////Get overlapped result
                //if (!iocontrol && Marshal.GetLastWin32Error() == (int)IoErrorCodes.ERROR_IO_PENDING)
                //{
                //    WaitForSingleObject(controller.InputVirtualOverlapped.EventHandle, INFINITE);
                //    return GetOverlappedResult(FileHandle, ref controller.InputVirtualOverlapped, out int bytesTransferred, false);
                //}
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to send virtual bus input: " + ex.Message);
                return false;
            }
            finally
            {
                SafeCloseMarshal(controlIntPtr);
            }
        }

        public bool VirtualOutput(ref ControllerStatus controller)
        {
            IntPtr controlIntPtr = IntPtr.Zero;
            try
            {
                if (!Connected) { return false; }

                ////Prepare marshal structure
                //controlIntPtr = Marshal.AllocHGlobal(controller.XOutputData.Size);
                //Marshal.StructureToPtr(controller.XOutputData, controlIntPtr, false);

                ////Get device control code
                //uint controlCode = CTL_CODE(FILE_DEVICE_TYPE.FILE_DEVICE_BUS_EXTENDER, FILE_ACCESS_DATA.FILE_READ_DATA | FILE_ACCESS_DATA.FILE_WRITE_DATA, IO_FUNCTION.IOCTL_XUSB_REQUEST_NOTIFICATION, IO_METHOD.METHOD_BUFFERED);

                ////Send marshal structure
                //bool iocontrol = DeviceIoControl(FileHandle, controlCode, controlIntPtr, controller.XOutputData.Size, controlIntPtr, controller.XOutputData.Size, out int bytesWritten, controller.OutputVirtualOverlapped);

                ////Get overlapped result
                //if (!iocontrol && Marshal.GetLastWin32Error() == (int)IoErrorCodes.ERROR_IO_PENDING)
                //{
                //    WaitForSingleObject(controller.OutputVirtualOverlapped.EventHandle, INFINITE); //NET8 crash
                //    if (GetOverlappedResult(FileHandle, ref controller.OutputVirtualOverlapped, out int bytesTransferred, false))
                //    {
                //        //Set marshal structure
                //        controller.XOutputData = Marshal.PtrToStructure<XUSB_OUTPUT_REPORT>(controlIntPtr);
                //        controller.RumbleCurrentHeavy = controller.XOutputData.RumbleHeavy;
                //        controller.RumbleCurrentLight = controller.XOutputData.RumbleLight;
                //        return true;
                //    }
                //}
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to read virtual bus output: " + ex.Message);
                return false;
            }
            finally
            {
                SafeCloseMarshal(controlIntPtr);
            }
        }
    }
}