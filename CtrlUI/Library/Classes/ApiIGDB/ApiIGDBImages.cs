namespace LibraryShared
{
    public partial class Classes
    {
        public class ApiIGDBImages
        {
            public int id { get; set; }
            public bool alpha_channel { get; set; }
            public bool animated { get; set; }
            public string image_id { get; set; }
            public string url { get; set; }
            public int height { get; set; }
            public int width { get; set; }
            public int? game { get; set; }
        }
    }
}