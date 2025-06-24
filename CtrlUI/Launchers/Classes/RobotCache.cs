using System.Collections.Generic;

namespace CtrlUI
{
    public partial class Classes
    {
        //Config
        public class RobotCacheConfig
        {
            public List<string> libraries { get; set; }
        }

        //App
        public class RobotCacheApp
        {
            public List<RobotCacheExeInfo> exeInfos { get; set; }
        }

        public class RobotCacheExeInfo
        {
            public RobotCacheDepotInfo depotInfo { get; set; }
            public string path { get; set; }
            public string title { get; set; }
        }

        public class RobotCacheDepotInfo
        {
            public int GameId { get; set; }
        }
    }
}