using System;
using System.Diagnostics;
using static LibraryUsb.NativeMethods_File;
using static LibraryUsb.NativeMethods_IoControl;

namespace LibraryUsb
{
    public static class Events
    {
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