using System.Collections.Generic;

namespace LibraryShared
{
    partial class Classes
    {
        public enum GoGAppCategory
        {
            game,
            tool,
            launcher,
            document
        }

        public class GoGConfig
        {
            public string libraryPath;
        }

        public class GoGPlayTasks
        {
            public bool isPrimary;
            public GoGAppCategory category;
            public string type;
            public string icon;
            public string name;
            public string path;
            public string workingDir;
            public string arguments;
        }

        public class GoGGameInfo
        {
            public string gameId;
            public string rootGameId;
            public bool standalone;
            public string dependencyGameId;
            public string language;
            public string name;
            public List<GoGPlayTasks> playTasks;
        }
    }
}