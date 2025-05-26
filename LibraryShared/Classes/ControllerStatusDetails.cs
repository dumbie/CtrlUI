using System;
using static LibraryUsb.VigemBusDevice;

namespace LibraryShared
{
    public partial class Classes
    {
        [Serializable]
        public class ControllerStatusDetails
        {
            public int NumberId { get; set; } = -1;
            public int NumberDisplay() { return NumberId + 1; }
            public int NumberVirtual() { return NumberId + VirtualIdOffset; }
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