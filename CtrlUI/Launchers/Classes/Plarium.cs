using System.Collections.Generic;

namespace CtrlUI
{
    public partial class Classes
    {
        public class PlariumApps
        {
            public Dictionary<string, InstalledGame> InstalledGames { get; set; }
        }

        public partial class InstalledGame
        {
            public int Id { get; set; }
            public int IntegrationType { get; set; }
            public string InstallationPath { get; set; }
            public Dictionary<string, string> InsalledGames { get; set; }
        }
    }
}