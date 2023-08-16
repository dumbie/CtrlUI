using static LibraryShared.Enums;

namespace LibraryShared
{
    public partial class Classes
    {
        public class ControllerDetails
        {
            public string DisplayName { get; set; }
            public string DevicePath { get; set; }
            public string DeviceInstanceId { get; set; }
            public bool Wireless { get; set; }
            public ControllerType Type { get; set; }
            public ControllerProfile Profile { get; set; }
        }
    }
}