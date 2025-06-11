using System.Collections.Generic;

namespace LibraryShared
{
    public partial class Classes
    {
        public class EpicInstalledManifest
        {
            public string LaunchExecutable;
            public string AppName;
            public string DisplayName;
        }

        public class EpicLauncherInstalled
        {
            public List<InstalledApp> InstallationList;
            public class InstalledApp
            {
                public string InstallLocation;
                public string AppName;
                public string AppVersion;
            }
        }
    }
}