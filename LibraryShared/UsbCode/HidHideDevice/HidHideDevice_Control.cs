using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using static LibraryUsb.NativeMethods_File;
using static LibraryUsb.NativeMethods_IoControl;

namespace LibraryUsb
{
    public partial class HidHideDevice
    {
        public enum IO_FUNCTION : uint
        {
            IOCTL_GET_WHITELIST = 2048,
            IOCTL_SET_WHITELIST = 2049,
            IOCTL_GET_BLACKLIST = 2050,
            IOCTL_SET_BLACKLIST = 2051,
            IOCTL_GET_ACTIVE = 2052,
            IOCTL_SET_ACTIVE = 2053
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
            DEVICE_TYPE_HIDHIDE = 32769
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

        private string DosPathToDevicePath(string dosPath)
        {
            try
            {
                //Get drive letter
                string driveLetter = Path.GetPathRoot(dosPath).Replace("\\", string.Empty);

                //Get device path
                StringBuilder pathInformation = new StringBuilder(1024);
                QueryDosDevice(driveLetter, pathInformation, 1024);

                //Combine device path
                return dosPath.Replace(driveLetter, pathInformation.ToString());
            }
            catch { }
            return dosPath;
        }

        public bool DeviceHideToggle(bool enableHide)
        {
            int controlLength = sizeof(bool);
            IntPtr controlIntPtr = Marshal.AllocHGlobal(controlLength);
            try
            {
                if (!Connected) { return false; }

                //Set marshal structure
                if (enableHide)
                {
                    Marshal.WriteByte(controlIntPtr, 1);
                }
                else
                {
                    Marshal.WriteByte(controlIntPtr, 0);
                }

                //Get device control code
                uint controlCode = CTL_CODE(FILE_DEVICE_TYPE.DEVICE_TYPE_HIDHIDE, FILE_ACCESS_DATA.FILE_READ_DATA, IO_FUNCTION.IOCTL_SET_ACTIVE, IO_METHOD.METHOD_BUFFERED);

                //Send marshal structure
                Debug.WriteLine("HidHide toggling hide: " + enableHide);
                return DeviceIoControl(FileHandle, controlCode, controlIntPtr, controlLength, IntPtr.Zero, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to toggle hidhide device: " + ex.Message);
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

        public IEnumerable<string> ListDeviceGet()
        {
            IntPtr controlIntPtr = IntPtr.Zero;
            try
            {
                if (!Connected) { return null; }

                //Set blacklist
                IList<string> listStrings = new List<string>();

                //Get device control code
                uint controlCode = CTL_CODE(FILE_DEVICE_TYPE.DEVICE_TYPE_HIDHIDE, FILE_ACCESS_DATA.FILE_READ_DATA, IO_FUNCTION.IOCTL_GET_BLACKLIST, IO_METHOD.METHOD_BUFFERED);

                //Send marshal structure (check size)
                DeviceIoControl(FileHandle, controlCode, IntPtr.Zero, 0, IntPtr.Zero, 0, out int bytesWrittenSize, IntPtr.Zero);

                //Set marshal structure
                controlIntPtr = Marshal.AllocHGlobal(bytesWrittenSize);

                //Send marshal structure (read list)
                DeviceIoControl(FileHandle, controlCode, IntPtr.Zero, 0, controlIntPtr, bytesWrittenSize, out int bytesWrittenList, IntPtr.Zero);

                //Convert pointer to string array
                IEnumerable<string> deviceStrings = MultiSzPointerToStringArray(controlIntPtr, bytesWrittenSize);

                foreach (string deviceString in deviceStrings)
                {
                    Debug.WriteLine(deviceString);
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get device blacklist: " + ex.Message);
                return null;
            }
            finally
            {
                if (controlIntPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(controlIntPtr);
                }
            }
        }

        public bool ListDeviceReset()
        {
            IntPtr controlIntPtr = IntPtr.Zero;
            try
            {
                if (!Connected) { return false; }

                //Set blacklist
                IList<string> listStrings = new List<string>();

                //Set marshal structure
                controlIntPtr = StringArrayToMultiSzPointer(listStrings, out int controlLength);

                //Get device control code
                uint controlCode = CTL_CODE(FILE_DEVICE_TYPE.DEVICE_TYPE_HIDHIDE, FILE_ACCESS_DATA.FILE_READ_DATA, IO_FUNCTION.IOCTL_SET_BLACKLIST, IO_METHOD.METHOD_BUFFERED);

                //Send marshal structure
                Debug.WriteLine("HidHide resetting device blacklist.");
                return DeviceIoControl(FileHandle, controlCode, controlIntPtr, controlLength, IntPtr.Zero, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to reset device blacklist: " + ex.Message);
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

        public bool ListDeviceAdd(string pathString)
        {
            IntPtr controlIntPtr = IntPtr.Zero;
            try
            {
                if (!Connected) { return false; }

                //Set blacklist
                IList<string> listStrings = new List<string>();
                listStrings.Add(pathString);

                //Set marshal structure
                controlIntPtr = StringArrayToMultiSzPointer(listStrings, out int controlLength);

                //Get device control code
                uint controlCode = CTL_CODE(FILE_DEVICE_TYPE.DEVICE_TYPE_HIDHIDE, FILE_ACCESS_DATA.FILE_READ_DATA, IO_FUNCTION.IOCTL_SET_BLACKLIST, IO_METHOD.METHOD_BUFFERED);

                //Send marshal structure
                Debug.WriteLine("HidHide hiding device: " + pathString);
                return DeviceIoControl(FileHandle, controlCode, controlIntPtr, controlLength, IntPtr.Zero, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to add device blacklist: " + ex.Message);
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

        public bool ListApplicationReset()
        {
            IntPtr controlIntPtr = IntPtr.Zero;
            try
            {
                if (!Connected) { return false; }

                //Set whitelist
                IList<string> listStrings = new List<string>();

                //Set marshal structure
                controlIntPtr = StringArrayToMultiSzPointer(listStrings, out int controlLength);

                //Get device control code
                uint controlCode = CTL_CODE(FILE_DEVICE_TYPE.DEVICE_TYPE_HIDHIDE, FILE_ACCESS_DATA.FILE_READ_DATA, IO_FUNCTION.IOCTL_SET_WHITELIST, IO_METHOD.METHOD_BUFFERED);

                //Send marshal structure
                Debug.WriteLine("HidHide resetting application whitelist.");
                return DeviceIoControl(FileHandle, controlCode, controlIntPtr, controlLength, IntPtr.Zero, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to reset application whitelist: " + ex.Message);
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

        public bool ListApplicationAdd(string pathString)
        {
            IntPtr controlIntPtr = IntPtr.Zero;
            try
            {
                if (!Connected) { return false; }

                //Set whitelist
                IList<string> listStrings = new List<string>();
                listStrings.Add(DosPathToDevicePath(pathString));

                //Set marshal structure
                controlIntPtr = StringArrayToMultiSzPointer(listStrings, out int controlLength);

                //Get device control code
                uint controlCode = CTL_CODE(FILE_DEVICE_TYPE.DEVICE_TYPE_HIDHIDE, FILE_ACCESS_DATA.FILE_READ_DATA, IO_FUNCTION.IOCTL_SET_WHITELIST, IO_METHOD.METHOD_BUFFERED);

                //Send marshal structure
                Debug.WriteLine("HidHide allowing application: " + pathString);
                return DeviceIoControl(FileHandle, controlCode, controlIntPtr, controlLength, IntPtr.Zero, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to add application whitelist: " + ex.Message);
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