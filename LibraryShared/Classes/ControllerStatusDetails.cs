using System;

namespace LibraryShared
{
    public partial class Classes
    {
        [Serializable]
        public class ControllerStatusDetails
        {
            public int NumberId { get; set; } = -1;
            public bool Activated { get; set; } = false;
            public bool Connected { get; set; } = false;
            public int BatteryPercentageCurrent { get; set; } = -1;

            //Set used controller number
            public ControllerStatusDetails(int numberId)
            {
                NumberId = numberId;
            }
        }
    }
}