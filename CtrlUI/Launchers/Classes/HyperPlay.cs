using System.Collections.Generic;

namespace CtrlUI
{
    public partial class Classes
    {
        public class HyperPlayLibrary
        {
            public List<HyperPlayGame> games { get; set; }
        }

        public partial class HyperPlayGame
        {
            public string app_name { get; set; }
            public string title { get; set; }
            public string art_square { get; set; }
            public string art_cover { get; set; }
            public string type { get; set; }
            public bool is_installed { get; set; }
            public HyperPlayInstall install { get; set; }
        }

        public partial class HyperPlayInstall
        {
            public string platform { get; set; }
            public string channelName { get; set; }
            public string appName { get; set; }
            public string install_path { get; set; }
            public string executable { get; set; }
            public string install_size { get; set; }
            public bool is_dlc { get; set; }
            public string version { get; set; }
        }
    }
}