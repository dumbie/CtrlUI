namespace LibraryShared
{
    public partial class Classes
    {
        public class ApiHltbSearchResultApi
        {
            public int count { get; set; }
            public Data[] data { get; set; }
        }

        public class Data
        {
            public string id { get; set; }
            public string name { get; set; }
            public string image { get; set; }
            public string platforms { get; set; }
            public string itch_id { get; set; }
            public string steam_id { get; set; }
            public string xbox_id { get; set; }
            public Review review { get; set; }
            public List list { get; set; }
            public Time time { get; set; }
        }

        public class Review
        {
            public string score { get; set; }
            public string count { get; set; }
        }

        public class List
        {
            public string playing { get; set; }
            public string backlogged { get; set; }
            public string replay { get; set; }
            public string custom { get; set; }
            public string completed { get; set; }
            public string retired { get; set; }
        }

        public class Time
        {
            public string main { get; set; }
            public string main_count { get; set; }
            public string main_plus { get; set; }
            public string main_plus_count { get; set; }
            public string completionist { get; set; }
            public string completionist_count { get; set; }
            public string combined { get; set; }
            public string combined_count { get; set; }
            public bool is_reliable { get; set; }
        }
    }
}