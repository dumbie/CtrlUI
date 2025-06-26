using System.Collections.Generic;

namespace CtrlUI
{
    public partial class Classes
    {
        public class ItchIOVerdict
        {
            public class Candidate
            {
                public string path { get; set; }
                public int depth { get; set; }
                public string flavor { get; set; }
                public string arch { get; set; }
                public int size { get; set; }
                public WindowsInfo windowsInfo { get; set; }
            }

            public class WindowsInfo
            {
                public bool gui { get; set; }
            }

            public string basePath { get; set; }
            public int totalSize { get; set; }
            public List<Candidate> candidates { get; set; }
        }

        public class ItchIOApp
        {
            public long Identifier { get; set; }
            public string Title { get; set; }
            public string ExecutablePath { get; set; }
            public string VerdictJson { get; set; }
        }
    }
}