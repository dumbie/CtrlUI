using System.Collections.Generic;

namespace CtrlUI
{
    public partial class Classes
    {
        public class GameJoltApps
        {
            public Dictionary<string, GameJoltObject> objects { get; set; }
        }

        public partial class GameJoltObject
        {
            public int id { get; set; }
            public string install_dir { get; set; }
            public string title { get; set; }
            public List<GameJoltLaunchOptions> launch_options { get; set; }
        }

        public partial class GameJoltLaunchOptions
        {
            public string executable_path { get; set; }
        }
    }
}