using System;

namespace LibraryShared
{
    public partial class Classes
    {
        [Serializable]
        public class ControllerButtonDetails
        {
            public bool PressedRaw { get; set; } = false;
            public bool PressedShort { get; set; } = false;
            public bool PressedLong { get; set; } = false;
            public bool PressTimeDone { get; set; } = false;
            public int PressTimeCurrent { get; set; } = 0;
            public int PressTimePrevious { get; set; } = 0;
        }
    }
}