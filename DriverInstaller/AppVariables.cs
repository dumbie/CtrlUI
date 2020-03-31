using System;
using System.Collections.Generic;
using static LibraryShared.Classes;

namespace DriverInstaller
{
    public partial class AppVariables
    {
        //Application Variables
        public static Guid vClassGuid_Hid = new Guid("745A17A0-74D3-11D0-B6FE-00A0C90F57DA");
        public static Guid vClassGuid_System = new Guid("4D36E97D-E325-11CE-BFC1-08002BE10318");
        public static bool vRebootRequired = false;
        public static bool vDirectXInputRunning = false;

        //Application Lists
        public static List<ProfileShared> vDirectCloseTools = new List<ProfileShared>();
    }
}