using System.Collections.Generic;

namespace CtrlUI
{
    public partial class Classes
    {
        public class AnkamaInstall
        {
            public string id { get; set; }
            public string gameUid { get; set; }
            public int gameId { get; set; }
            public string gameName { get; set; }
            public string name { get; set; }
            public string location { get; set; }
            public string version { get; set; }
            public string repositoryVersion { get; set; }
            public List<string> installedFragments { get; set; }
        }

        public class AnkamaData
        {
            public string displayName { get; set; }
        }
    }
}