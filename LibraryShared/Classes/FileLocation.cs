using System;

namespace LibraryShared
{
    public partial class Classes
    {
        [Serializable]
        public class FileLocation
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }
    }
}