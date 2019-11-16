using System.Windows;
using System.Windows.Controls;

namespace LibraryShared
{
    public partial class Classes
    {
        public class FrameworkElementFocus
        {
            public int FocusIndex { get; set; } = 0;
            public ListBox FocusListBox { get; set; } = null;
            public FrameworkElement FocusTarget { get; set; } = null;
            public FrameworkElement FocusPrevious { get; set; } = null;

            public void Reset()
            {
                FocusIndex = 0;
                FocusListBox = null;
                FocusTarget = null;
                FocusPrevious = null;
            }
        }
    }
}