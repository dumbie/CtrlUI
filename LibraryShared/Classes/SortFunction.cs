using System;

namespace LibraryShared
{
    public partial class Classes
    {
        public class SortFunction<T>
        {
            public Func<T, object> function = null;
            public bool ascending = true;
        }
    }
}