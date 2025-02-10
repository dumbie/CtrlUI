namespace LibraryShared
{
    public partial class Classes
    {
        public class ApiIGDBGamesMultiplayerModes
        {
            public int id { get; set; }
            public bool campaigncoop { get; set; }
            public bool dropin { get; set; }
            public int game { get; set; }
            public bool lancoop { get; set; }
            public bool offlinecoop { get; set; }
            public int offlinecoopmax { get; set; }
            public int offlinemax { get; set; }
            public bool onlinecoop { get; set; }
            public int onlinecoopmax { get; set; }
            public int onlinemax { get; set; }
            public int platform { get; set; }
            public bool splitscreen { get; set; }
        }
    }
}