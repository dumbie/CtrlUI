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
            public ControllerBattery BatteryCurrent { get; set; } = new ControllerBattery();

            //Set used controller number
            public ControllerStatusDetails(int numberId)
            {
                NumberId = numberId;
            }
        }
    }
}