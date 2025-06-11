namespace LibraryShared
{
    public partial class Classes
    {
        public class ApiIGDBGamesEngines
        {
            public int id { get; set; }
            public int[] companies { get; set; }
            public int created_at { get; set; }
            public string description { get; set; }
            public int logo { get; set; }
            public string name { get; set; }
            public int[] platforms { get; set; }
            public string slug { get; set; }
            public int updated_at { get; set; }
            public string url { get; set; }
        }
    }
}