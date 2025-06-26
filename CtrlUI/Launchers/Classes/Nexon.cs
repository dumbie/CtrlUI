using System.Collections.Generic;

namespace CtrlUI
{
    public partial class Classes
    {
        public class NexonApps
        {
            public Dictionary<string, NexonAppDetails> installedApps { get; set; }
        }

        public class NexonAppDetails
        {
            public string name { get; set; }
            public string installPath { get; set; }
            public string installRootPath { get; set; }
            public string localManifest { get; set; }
        }
    }
}