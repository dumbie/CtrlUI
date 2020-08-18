using System.Collections.Generic;

namespace LibraryShared
{
    partial class Classes
    {
        public class GoGConfig
        {
            public string libraryPath;
        }

        public class GoGPlayTasks
        {
            public bool isPrimary;
            public string type;
            public string icon;
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