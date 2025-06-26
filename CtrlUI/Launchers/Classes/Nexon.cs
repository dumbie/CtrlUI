using System.Collections.Generic;

namespace CtrlUI
{
    public partial class Classes
    {
        //Nexon application
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

        //Nexon data
        public class NexonData
        {
            public int productId { get; set; }
            public string shortCutName { get; set; }
            public string directoryName { get; set; }
            public string directory { get; set; }
            public string executablePath { get; set; }
            public string executablePathBit64 { get; set; }
            public string workingDirectory { get; set; }
            public string requiredDiskSpace { get; set; }
        }
    }
}