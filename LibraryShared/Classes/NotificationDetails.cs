using System;
using System.Windows.Media;

namespace LibraryShared
{
    public partial class Classes
    {
        [Serializable]
        public class NotificationDetails
        {
            public string Icon { get; set; } = string.Empty;
            public string Text { get; set; } = string.Empty;
            public Color? Color { get; set; } = null;
        }
    }
}