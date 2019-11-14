using System;

namespace LibraryShared
{
    public partial class Classes
    {
        [Serializable]
        public class ControllerStatusSummary
        {
            public ControllerStatusSummary(int numberId)
            {
                NumberId = numberId;
            }

            public int NumberId { get; set; } = -1;
            public bool Manage { get; set; } = false;
            public bool Connected { get; set; } = false;
            public int BatteryPercentageCurrent { get; set; } = -1;
        }
    }
}