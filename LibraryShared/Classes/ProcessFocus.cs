using System;
using System.Diagnostics;

namespace LibraryShared
{
    public partial class Classes
    {
        public class ProcessFocus
        {
            public string Title = "Unknown";
            public IntPtr WindowHandle = IntPtr.Zero;
            public Process Process = null;
        }
    }
}