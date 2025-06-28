namespace CtrlUI
{
    public partial class Classes
    {
        //Arrays
        public static string[] vViveAppIdBlacklist = { "65d81211-14f6-4eb1-9a92-5346fe6bf572", "4f5f140a-0928-4fcb-8023-93dc212eac17" };

        //Classes
        public class ViveApps
        {
            public string appId { get; set; }
            public string title { get; set; }
            public string imageUri { get; set; }
            public string path { get; set; }
            public string uri { get; set; }
        }
    }
}