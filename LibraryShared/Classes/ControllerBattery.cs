using System;
using static LibraryShared.Enums;

namespace LibraryShared
{
    public partial class Classes
    {
        [Serializable]
        public class ControllerBattery
        {
            public BatteryStatus BatteryStatus = BatteryStatus.Unknown;
            public int BatteryPercentage = -1;
        }
    }
}