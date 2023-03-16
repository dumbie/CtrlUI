using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static LibraryUsb.Events;
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
                Debug.WriteLine("HidHide toggle hide: " + enableHide);
                return DeviceIoControl(FileHandle, controlCode, controlIntPtr, controlLength, IntPtr.Zero, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("HidHide failed to toggle hide: " + ex.Message);
                return false;
            }
            finally
            {
                SafeCloseMarshal(controlIntPtr);
            }
        }

        public List<string> ListDeviceGet()
        {
            IntPtr controlIntPtr = IntPtr.Zero;
            try
            {
                if (!Connected) { return null; }

                //Get device control code
                uint controlCode = CTL_CODE(FILE_DEVICE_TYPE.DEVICE_TYPE_HIDHIDE, FILE_ACCESS_DATA.FILE_READ_DATA, IO_FUNCTION.IOCTL_GET_BLACKLIST, IO_METHOD.METHOD_BUFFERED);

                //Send marshal structure (list size)
                DeviceIoControl(FileHandle, controlCode, IntPtr.Zero, 0, IntPtr.Zero, 0, out int bytesWrittenSize, IntPtr.Zero);

                //Check string array size
                if (bytesWrittenSize < 10)
                {
                    //Debug.WriteLine("HidHide device blacklist is empty.");
                    return new List<string>();
                }

                //Set marshal structure
                controlIntPtr = Marshal.AllocHGlobal(bytesWrittenSize);

                //Send marshal structure (read list)
                DeviceIoControl(FileHandle, controlCode, IntPtr.Zero, 0, controlIntPtr, bytesWrittenSize, out int bytesWrittenList, IntPtr.Zero);

                //Convert pointer to string array
                return MultiSzPointerToStringArray(controlIntPtr, bytesWrittenSize).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("HidHide failed to get device blacklist: " + ex.Message);
                return new List<string>();
            }
            finally
            {
                SafeCloseMarshal(controlIntPtr);
            }
        }

        public bool ListDeviceReset()
        {
            IntPtr controlIntPtr = IntPtr.Zero;
            try
            {
                if (!Connected) { return false; }

                //Set blacklist
                List<string> listStrings = new List<string>();

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
                SafeCloseMarshal(controlIntPtr);
            }
        }

        public async Task<bool> ListDeviceAdd(string pathString)
        {
            IntPtr controlIntPtr = IntPtr.Zero;
            try
            {
                if (!Connected) { return false; }

                //Get current blacklist
                List<string> deviceStrings = ListDeviceGet();

                //Check blacklist
                if (deviceStrings.Contains(pathString))
                {
                    Debug.WriteLine("HidHide device already exists: " + pathString);
                    return true;
                }

                //Add to blacklist
                deviceStrings.Add(pathString);

                //Set marshal structure
                controlIntPtr = StringArrayToMultiSzPointer(deviceStrings, out int controlLength);

                //Get device control code
                uint controlCode = CTL_CODE(FILE_DEVICE_TYPE.DEVICE_TYPE_HIDHIDE, FILE_ACCESS_DATA.FILE_READ_DATA, IO_FUNCTION.IOCTL_SET_BLACKLIST, IO_METHOD.METHOD_BUFFERED);

                //Send marshal structure
                Debug.WriteLine("HidHide hiding device: " + pathString);
                bool hideResult = DeviceIoControl(FileHandle, controlCode, controlIntPtr, controlLength, IntPtr.Zero, 0, out int bytesWritten, IntPtr.Zero) && bytesWritten > 0;

                //Wait for device is hidden
                if (hideResult)
                {
                    await Task.Delay(500);
                }

                return hideResult;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("HidHide failed hiding device: " + ex.Message);
                return false;
            }
            finally
            {
                SafeCloseMarshal(controlIntPtr);
            }
        }

        public bool ListApplicationReset()
        {
            IntPtr controlIntPtr = IntPtr.Zero;
            try
            {
                if (!Connected) { return false; }

                //Set whitelist
                List<string> listStrings = new List<string>();

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
                Debug.WriteLine("HidHide failed to reset application whitelist: " + ex.Message);
                return false;
            }
            finally
            {
                SafeCloseMarshal(controlIntPtr);
            }
        }

        public bool ListApplicationAdd(string pathString)
        {
            IntPtr controlIntPtr = IntPtr.Zero;
            try
            {
                if (!Connected) { return false; }

                //Set whitelist
                List<string> listStrings = new List<string>();
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
                Debug.WriteLine("HidHide failed to add application whitelist: " + ex.Message);
                return false;
            }
            finally
            {
                SafeCloseMarshal(controlIntPtr);
            }
        }
    }
}