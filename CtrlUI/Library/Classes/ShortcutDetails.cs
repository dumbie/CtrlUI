using System;

namespace LibraryShared
{
    public partial class Classes
    {
        public enum ShortcutType : int
        {
            Execute = 0,
            UrlProtocol = 1,
            UWP = 2
        }

        public class ShortcutDetails
        {
            public ShortcutType Type { get; set; } = ShortcutType.Execute;
            public string Title { get; set; }
            public string NameExe { get; set; }
            public string TargetPath { get; set; }
            public string WorkingPath { get; set; }
            public int IconIndex { get; set; }
            public string IconPath { get; set; }
            public string ShortcutPath { get; set; }
            public string Argument { get; set; }
            public string Comment { get; set; }
            public DateTime DateModified { get; set; }
        }
    }
}