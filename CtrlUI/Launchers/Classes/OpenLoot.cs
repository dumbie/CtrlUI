using Newtonsoft.Json.Linq;

namespace CtrlUI
{
    public partial class Classes
    {
        public class OpenLootSettings
        {
            public JToken apps { get; set; }
        }

        public class OpenLootAppData
        {
            public bool is_installed { get; set; }
            public string version { get; set; }
            public string start_exe_path { get; set; }
            public string start_exe_args { get; set; }
        }
    }
}