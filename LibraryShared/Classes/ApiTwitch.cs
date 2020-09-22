namespace LibraryShared
{
    public partial class Classes
    {
        public class ApiTwitchOauth2
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
            public int status { get; set; }
            public string message { get; set; }
        }
    }
}