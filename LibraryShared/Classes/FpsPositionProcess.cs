using System;

namespace LibraryShared
{
    public partial class Classes
    {
        [Serializable]
        public class FpsPositionProcess
        {
            public string Process { get; set; }
            public int Position { get; set; }
        }
    }
}