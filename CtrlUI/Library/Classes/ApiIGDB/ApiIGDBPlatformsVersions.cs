namespace LibraryShared
{
    public partial class Classes
    {
        public class ApiIGDBPlatformsVersions
        {
            public int id { get; set; }
            public ApiIGDBCompanies[] companies { get; set; }
            public string connectivity { get; set; }
            public string cpu { get; set; }
            public string graphics { get; set; }
            public int? main_manufacturer { get; set; }
            public string media { get; set; }
            public string memory { get; set; }
            public string name { get; set; }
            public string os { get; set; }
            public string online { get; set; }
            public string output { get; set; }
            public ApiIGDBImages platform_logo { get; set; }
            public ApiIGDBReleaseDates[] platform_version_release_dates { get; set; }
            public string resolutions { get; set; }
            public string slug { get; set; }
            public string sound { get; set; }
            public string storage { get; set; }
            public string summary { get; set; }
            public string url { get; set; }
        }
    }
}