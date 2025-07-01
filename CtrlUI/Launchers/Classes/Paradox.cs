using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace CtrlUI
{
    public partial class Classes
    {
        public class ParadoxLauncherSettings
        {
            public string gameId { get; set; }
            public string exePath { get; set; }
            public List<string> exeArgs { get; set; }
            public string gameDataPath { get; set; }
        }

        public class ParadoxUserSettings
        {
            public JArray gameLibraryPaths { get; set; }
        }

        public class ParadoxGameLibraryPath
        {
            public string gameId { get; set; }
            public string repositoryPath { get; set; }
            public string repositoryType { get; set; }
            public string installationPath { get; set; }
            public string launcherSettingsDirPath { get; set; }
        }

        public class ParadoxGameMetadata
        {
            public ParadoxGameMetadataData data { get; set; }
        }

        public class ParadoxGameMetadataData
        {
            public List<ParadoxGameMetadataGame> games { get; set; }
        }

        public class ParadoxGameMetadataGame
        {
            public string id { get; set; }
            public string name { get; set; }
            public List<string> exeArgs { get; set; }
            public string exePath { get; set; }
        }
    }
}