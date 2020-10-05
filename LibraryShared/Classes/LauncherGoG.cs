using System.Collections.Generic;

namespace LibraryShared
{
    partial class Classes
    {
        public enum GoGAppCategory
        {
            unknown,
            game,
            tool,
            launcher,
            document
        }

        public class GoGConfig
        {
            public string libraryPath { get; set; }
        }

        public class GoGPlayTasks
        {
            public bool isPrimary { get; set; }
            public GoGAppCategory category { get; set; } = GoGAppCategory.unknown;
            public string type { get; set; }
            public string icon { get; set; }
            public string name { get; set; }
            public string path { get; set; }
            public string workingDir { get; set; }
            public string arguments { get; set; }
        }

        public class GoGGameInfo
        {
            public string gameId { get; set; }
            public string rootGameId { get; set; }
            public bool standalone { get; set; }
            public string dependencyGameId { get; set; }
            public string language { get; set; }
            public string name { get; set; }
            public List<GoGPlayTasks> playTasks { get; set; }
        }
    }
}