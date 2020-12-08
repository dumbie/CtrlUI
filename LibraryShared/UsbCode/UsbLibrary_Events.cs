using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static LibraryUsb.NativeMethods_File;
using static LibraryUsb.NativeMethods_IoControl;

namespace LibraryUsb
{
    public static class Events
    {
        public static bool SafeFreeMarshal(IntPtr hglobal)
        {
            try
            {
                if (hglobal != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(hglobal);
                    Debug.WriteLine("Marshal freed: " + hglobal);
                }
                else
                {
                    Debug.WriteLine("Marshal is already free.");
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to free marshal: " + ex.Message);
                return false;
            }
        }

        public static bool SafeCloseHandle(IntPtr handle)
        {
            try
            {
                if (handle != IntPtr.Zero)
                {
                    CloseHandle(handle);
                    Marshal.FreeHGlobal(handle);
                    Debug.WriteLine("Closed the handle: " + handle);
                }
                else
                {
                    Debug.WriteLine("Handle is already closed.");
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to close handle: " + ex.Message);
                return false;
            }
        }

        public static bool SetAndCloseEvent(IntPtr hEvent)
        {
            try
            {
                SetEvent(hEvent);
                CloseHandle(hEvent);
                Debug.WriteLine("Set and closed the event: " + hEvent);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to set and close event: " + ex.Message);
                return false;
            }
        }
    }
}