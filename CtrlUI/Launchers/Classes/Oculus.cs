namespace CtrlUI
{
    public partial class Classes
    {
        public class OculusJsonApp
        {
            public string appId { get; set; }
            public string canonicalName { get; set; }
            public string launchFile { get; set; }
            public string launchParameters { get; set; }
            public string launchFile2D { get; set; }
            public string launchParameters2D { get; set; }
        }

        public class OculusDatabaseApp
        {
            public string canonical_name { get; set; }
            public string display_name { get; set; }
            public string display_short_description { get; set; }
            public string organization_name { get; set; }
            public string genres { get; set; }
        }
    }
}