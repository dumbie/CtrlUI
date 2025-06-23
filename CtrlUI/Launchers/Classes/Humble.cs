using Newtonsoft.Json;
using System.Collections.Generic;

namespace CtrlUI
{
    public partial class Classes
    {
        public class HumbleAppJson
        {
            public Settings settings { get; set; }
            [JsonProperty("game-collection-4")]
            public List<GameCollection4> gamecollection4 { get; set; }
        }

        public class Settings
        {
            public string downloadLocation { get; set; }
        }

        public class GameCollection4
        {
            public string gameName { get; set; }
            public string status { get; set; }
            public string downloadMachineName { get; set; }
            public string filePath { get; set; }
            public string executablePath { get; set; }
        }
    }
}