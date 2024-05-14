using System.Collections.Generic;
using static LibraryShared.Classes;

namespace DriverInstaller
{
    public partial class AppVariables
    {
        //Application Windows
        public static WindowMain vWindowMain = new WindowMain();

        //Application Variables
        public static bool vRebootRequired = false;
        public static bool vDirectXInputRunning = false;

        //Application Lists
        public static List<ProfileShared> vCtrlCloseLaunchers = new List<ProfileShared>();
        public static List<ProfileShared> vDirectCloseTools = new List<ProfileShared>();
    }
}