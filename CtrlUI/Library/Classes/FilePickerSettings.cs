using System.Collections.Generic;

namespace LibraryShared
{
    public partial class Classes
    {
        public class FilePickerSettings
        {
            public List<string> FilterIn { get; set; } = new List<string>();
            public List<string> FilterOut { get; set; } = new List<string>();
            public string Title { get; set; } = "File Browser";
            public string Description { get; set; } = "Please select a file, folder or disk:";
            public string RootPath { get; set; } = string.Empty;
            public bool ShowLaunchWithoutFile { get; set; } = false;
            public bool ShowEmulatorInterface { get; set; } = false;
            public bool ShowEmulatorImages { get; set; } = false;
            public bool ShowFiles { get; set; } = true;
            public bool ShowDirectories { get; set; } = true;
            public DataBindApp SourceDataBindApp { get; set; } = null;
        }
    }
}