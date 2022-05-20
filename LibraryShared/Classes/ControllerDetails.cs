using static LibraryShared.Enums;

namespace LibraryShared
{
    public partial class Classes
    {
        public class ControllerDetails
        {
            public string DisplayName { get; set; }
            public string Path { get; set; }
            public string ModelId { get; set; }
            public ControllerType Type { get; set; }
            public bool Wireless { get; set; }
            public ControllerProfile Profile { get; set; }
        }
    }
}