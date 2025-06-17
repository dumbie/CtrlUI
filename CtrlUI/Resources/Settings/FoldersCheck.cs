using ArnoldVinkCode;
using System;
using System.Diagnostics;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check application user folders
        public void Folders_Check()
        {
            try
            {
                AVFiles.Directory_Create(@"Assets\User\Apps", false);
                AVFiles.Directory_Create(@"Assets\User\Clocks", false);
                AVFiles.Directory_Create(@"Assets\User\Emulators", false);
                AVFiles.Directory_Create(@"Assets\User\Fonts", false);
                AVFiles.Directory_Create(@"Assets\User\Icons", false);
                AVFiles.Directory_Create(@"Assets\User\Sounds", false);

                Debug.WriteLine("Checked application user folders.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed checking application user folders: " + ex.Message);
            }
        }
    }
}