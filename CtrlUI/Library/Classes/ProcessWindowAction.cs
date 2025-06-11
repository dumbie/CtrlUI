using System;
using System.Collections.Generic;

namespace LibraryShared
{
    public partial class Enums
    {
        //Enumerators
        public enum ProcessWindowActions
        {
            Single,
            Multiple,
            NoAction,
            Cancel
        }

        //Classes
        public class ProcessWindowAction
        {
            public ProcessWindowActions Action { get; set; } = ProcessWindowActions.NoAction;
            public IntPtr WindowHandle { get; set; } = IntPtr.Zero;
            public List<IntPtr> WindowHandles { get; set; } = null;
        }
    }
}