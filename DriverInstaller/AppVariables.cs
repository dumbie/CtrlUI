using System.Collections.Generic;
using static ArnoldVinkCode.AVJsonFunctions;
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
        public static List<ProfileShared> vCtrlCloseLaunchers = JsonLoadFile<List<ProfileShared>>(@"Profiles\Default\CtrlCloseLaunchers.json");
        public static List<ProfileShared> vDirectCloseTools = JsonLoadFile<List<ProfileShared>>(@"Profiles\Default\DirectCloseTools.json");
    }
}