using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static LibraryShared.Classes;
using static LibraryUsb.NativeMethods_IoControl;

namespace LibraryUsb
{
    public partial class WinUsbDevice
    {
        public enum IO_FUNCTION : uint
        {
            IOCTL_BUSENUM_BASE = 0x801,
            IOCTL_BUSENUM_PLUGIN_HARDWARE = IOCTL_BUSENUM_BASE + 0x0,
            IOCTL_BUSENUM_UNPLUG_HARDWARE = IOCTL_BUSENUM_BASE + 0x1,
            IOCTL_XUSB_REQUEST_NOTIFICATION = IOCTL_BUSENUM_BASE + 0x200,
            IOCTL_XUSB_SUBMIT_REPORT = IOCTL_BUSENUM_BASE + 0x201
        }

        public enum IO_METHOD : uint
        {
            METHOD_BUFFERED = 0,
            METHOD_IN_DIRECT = 1,
            METHOD_OUT_DIRECT = 2,
            METHOD_NEITHER = 3
        }

        public enum FILE_DEVICE_TYPE : uint
        {
            FILE_DEVICE_BUS_EXTENDER = 0x0000002A
        }

        public enum FILE_ACCESS_DATA : uint
        {
            FILE_READ_DATA = 0x0001,
            FILE_WRITE_DATA = 0x0002,
            FILE_APPEND_DATA = 0x0004
        }

        public uint CTL_CODE(FILE_DEVICE_TYPE DeviceType, FILE_ACCESS_DATA Access, IO_FUNCTION Function, IO_METHOD Method)
        {
            return (((uint)DeviceType) << 16) | (((uint)Access) << 14) | (((uint)Function) << 2) | ((uint)Method);
        }

        public enum VIGEM_TARGET_TYPE : uint
        {
            Xbox360Wired = 0,
            XboxOneWired = 1,
            DualShock4Wired = 2
        }

        public enum XUSB_BUTTON : ushort
        {
            XUSB_GAMEPAD_DPAD_UP = 0x0001,
            XUSB_GAMEPAD_DPAD_DOWN = 0x0002,
            XUSB_GAMEPAD_DPAD_LEFT = 0x0004,
            XUSB_GAMEPAD_DPAD_RIGHT = 0x0008,
            XUSB_GAMEPAD_START = 0x0010,
            XUSB_GAMEPAD_BACK = 0x0020,
            XUSB_GAMEPAD_LEFT_THUMB = 0x0040,
            XUSB_GAMEPAD_RIGHT_THUMB = 0x0080,
            XUSB_GAMEPAD_LEFT_SHOULDER = 0x0100,
            XUSB_GAMEPAD_RIGHT_SHOULDER = 0x0200,
            XUSB_GAMEPAD_GUIDE = 0x0400,
            XUSB_GAMEPAD_A = 0x1000,
            XUSB_GAMEPAD_B = 0x2000,
            XUSB_GAMEPAD_X = 0x4000,
            XUSB_GAMEPAD_Y = 0x8000
        }

        public struct BUSENUM_PLUGIN_HARDWARE
        {
            public int Size;
            public int SerialNo;
            public VIGEM_TARGET_TYPE TargetType;
            public ushort VendorId;
            public ushort ProductId;
        }

        public struct BUSENUM_UNPLUG_HARDWARE
        {
            public int Size;
            public int SerialNo;
        }

        public struct XUSB_INPUT_REPORT
        {
            public int Size;
            public int SerialNo;
            public XUSB_REPORT Report;
        }

        public struct XUSB_REPORT
        {
            public XUSB_BUTTON wButtons;
            public byte bLeftTrigger;
            public byte bRightTrigger;
            public short sThumbLX;
            public short sThumbLY;
            public short sThumbRX;
            public short sThumbRY;
        }

        public struct XUSB_OUTPUT_REPORT
        {
            public int Size;
            public int SerialNo;
            public byte RumbleHeavy;
            public byte RumbleLight;
            public byte LedNumber;
        }

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
                if (controlIntPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(controlIntPtr);
                }
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
                if (controlIntPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(controlIntPtr);
                }
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

                //Prepare marshal structure
                controlIntPtr = Marshal.AllocHGlobal(controller.XInputData.Size);
                Marshal.StructureToPtr(controller.XInputData, controlIntPtr, false);

                //Get device control code
                uint controlCode = CTL_CODE(FILE_DEVICE_TYPE.FILE_DEVICE_BUS_EXTENDER, FILE_ACCESS_DATA.FILE_WRITE_DATA, IO_FUNCTION.IOCTL_XUSB_SUBMIT_REPORT, IO_METHOD.METHOD_BUFFERED);

                //Send marshal structure
                bool iocontrol = DeviceIoControl(FileHandle, controlCode, controlIntPtr, controller.XInputData.Size, controlIntPtr, controller.XInputData.Size, out int bytesWritten, controller.InputVirtualOverlapped);

                //Get overlapped result
                if (!iocontrol && Marshal.GetLastWin32Error() == (int)IoErrorCodes.ERROR_IO_PENDING)
                {
                    WaitForSingleObject(controller.InputVirtualOverlapped.EventHandle, INFINITE);
                    return GetOverlappedResult(FileHandle, ref controller.InputVirtualOverlapped, out int bytesTransferred, false);
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
                if (controlIntPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(controlIntPtr);
                }
            }
        }

        public bool VirtualOutput(ref ControllerStatus controller)
        {
            IntPtr controlIntPtr = IntPtr.Zero;
            try
            {
                if (!Connected) { return false; }

                //Prepare marshal structure
                controlIntPtr = Marshal.AllocHGlobal(controller.XOutputData.Size);
                Marshal.StructureToPtr(controller.XOutputData, controlIntPtr, false);

                //Get device control code
                uint controlCode = CTL_CODE(FILE_DEVICE_TYPE.FILE_DEVICE_BUS_EXTENDER, FILE_ACCESS_DATA.FILE_READ_DATA | FILE_ACCESS_DATA.FILE_WRITE_DATA, IO_FUNCTION.IOCTL_XUSB_REQUEST_NOTIFICATION, IO_METHOD.METHOD_BUFFERED);

                //Send marshal structure
                bool iocontrol = DeviceIoControl(FileHandle, controlCode, controlIntPtr, controller.XOutputData.Size, controlIntPtr, controller.XOutputData.Size, out int bytesWritten, controller.OutputVirtualOverlapped);

                //Get overlapped result
                if (!iocontrol && Marshal.GetLastWin32Error() == (int)IoErrorCodes.ERROR_IO_PENDING)
                {
                    WaitForSingleObject(controller.OutputVirtualOverlapped.EventHandle, INFINITE);
                    if (GetOverlappedResult(FileHandle, ref controller.OutputVirtualOverlapped, out int bytesTransferred, false))
                    {
                        //Set marshal structure
                        controller.XOutputData = Marshal.PtrToStructure<XUSB_OUTPUT_REPORT>(controlIntPtr);
                        controller.XOutputCurrentRumbleHeavy = controller.XOutputData.RumbleHeavy;
                        controller.XOutputCurrentRumbleLight = controller.XOutputData.RumbleLight;
                        return true;
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
                if (controlIntPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(controlIntPtr);
                }
            }
        }
    }
}