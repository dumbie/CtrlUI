namespace LibraryShared
{
    public partial class Classes
    {
        public class ApiIGDBWebsites
        {
            public int id { get; set; }
            public int category { get; set; }
            public int? game { get; set; }
            public bool trusted { get; set; }
            public string url { get; set; }
        }
    }
}