namespace LibraryShared
{
    public partial class Classes
    {
        public class ApiIGDBPlatforms
        {
            public int id { get; set; }
            public string abbreviation { get; set; }
            public string alternative_name { get; set; }
            public int category { get; set; }
            public int created_at { get; set; }
            public int? generation { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public string summary { get; set; }
            public int updated_at { get; set; }
            public string url { get; set; }
            public ApiIGDBImages platform_logo { get; set; }
            public ApiIGDBPlatformsFamily platform_family { get; set; }
            public ApiIGDBPlatformsVersions[] versions { get; set; }
            public ApiIGDBWebsites[] websites { get; set; }
        }
    }
}