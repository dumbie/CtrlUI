using System.Collections.Generic;

namespace CtrlUI
{
    public partial class Classes
    {
        public class HikariFieldApps
        {
            public Dictionary<string, HikariFieldInstall> installs { get; set; }
        }

        public partial class HikariFieldInstall
        {
            public int build_id { get; set; }
            public string version { get; set; }
            public string depot { get; set; }
            public string installed_path { get; set; }
            public string exec_file { get; set; }
        }
    }
}