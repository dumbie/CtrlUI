using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static LibraryUsb.NativeMethods_File;
using static LibraryUsb.NativeMethods_IoControl;

namespace LibraryUsb
{
    public static class Events
    {
        public static bool SafeCloseMarshal(IntPtr hGlobal)
        {
            try
            {
                if (hGlobal != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(hGlobal);
                    //Debug.WriteLine("Marshal freed: " + hGlobal);
                }
                else
                {
                    Debug.WriteLine("Marshal is already free: " + hGlobal);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to free marshal: " + ex.Message);
                return false;
            }
        }

        public static bool SafeCloseHandle(IntPtr hHandle)
        {
            try
            {
                if (hHandle != IntPtr.Zero)
                {
                    CloseHandle(hHandle);
                    //Debug.WriteLine("Closed the handle: " + hHandle);
                }
                else
                {
                    Debug.WriteLine("Handle is already closed: " + hHandle);
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
                SafeCloseHandle(hEvent);
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