using System.Collections.Generic;

namespace CtrlUI
{
    public partial class Classes
    {
        public class NetmarbleApps
        {
            public string AppDrive { get; set; }
            public Dictionary<string, NetmarbleGame> game { get; set; }
        }

        public class NetmarbleGame
        {
            public NetmarbleBuild build { get; set; }
        }

        public class NetmarbleBuild
        {
            public string name { get; set; }
            public NetmarbleBuildDownload download { get; set; }
            public string id { get; set; }
            public string installPath { get; set; }
        }

        public class NetmarbleBuildDownload
        {
            public string buildCode { get; set; }
            public string executeFileName { get; set; }
        }
    }
}