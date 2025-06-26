using System.Collections.Generic;

namespace CtrlUI
{
    public partial class Classes
    {
        public class ElixirApps
        {
            public Dictionary<string, ElixirAppDetails> GameRegistry { get; set; }
        }

        public class ElixirAppDetails
        {
            public string activeBranch { get; set; }
            public ElixirMaster MASTER { get; set; }
        }

        public partial class ElixirMaster
        {
            public string version { get; set; }
            public string build { get; set; }
            public string installPath { get; set; }
        }
    }
}