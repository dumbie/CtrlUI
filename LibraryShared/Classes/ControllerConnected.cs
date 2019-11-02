namespace LibraryShared
{
    public partial class Classes
    {
        public class ControllerConnected
        {
            public string DisplayName { get; set; }
            public string Path { get; set; }
            public string HardwareId { get; set; }
            public string Type { get; set; }
            public bool Wireless { get; set; }
            public ControllerProfile Profile { get; set; }
        }
    }
}