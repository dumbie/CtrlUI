using System;
using System.Diagnostics;

namespace LibraryShared
{
    public partial class Classes
    {
        public class ProcessMultipleCheck
        {
            public string Status = string.Empty;
            public Process Process = null;
            public ProcessUwp ProcessUwp = null;
        }

        public class ProcessUwp
        {
            public string AppUserModelId = string.Empty;
            public IntPtr WindowHandle = IntPtr.Zero;
            public int ProcessId = -1;
        }
    }
}