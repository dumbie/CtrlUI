using System.Collections.Generic;

namespace CtrlUI
{
    public partial class Classes
    {
        public class AsobimoApps
        {
            public string forced_update_version { get; set; }
            public string force_update_url { get; set; }
            public string home_url { get; set; }
            public string footbg_url { get; set; }
            public List<AsobimoTitleData> title_data { get; set; }
        }

        public partial class AsobimoTitleData
        {
            public string titlename { get; set; }
            public string titletext { get; set; }
            public string gameid { get; set; }
            public string titleconfigurl { get; set; }
            public string fullexefilename { get; set; }
            public string internalgamefoldername { get; set; }
            public string status { get; set; }
            public string closebuttonname { get; set; }
            public string closebuttonname_webview { get; set; }
            public string bgname { get; set; }
        }
    }
}