using System.Diagnostics;
using static ArnoldVinkCode.AVFiles;

namespace LibraryShared
{
    public partial class AppLaunchCheck
    {
        public static void Application_UpdateCheck()
        {
            try
            {
                Debug.WriteLine("Checking application update.");

                //Remove old unused files
                File_Remove("Profiles/AppsBlacklistProcess.json");
                File_Remove("Profiles/AppsBlacklistShortcut.json");
                File_Remove("Profiles/AppsBlacklistShortcutUri.json");
                File_Remove("Profiles/AppsCloseLaunchers.json");
                File_Remove("Profiles/AppsCloseTools.json");
                File_Remove("Profiles/ControllersSupported.json");
                File_Remove("Profiles/FileLocations.json");
                File_Remove("Profiles/FpsBlacklistProcess.json");
                File_Remove("Profiles/FpsPositionProcess.json");
                File_Remove("Profiles/ShortcutLocations.json");

                //Rename old file names
                File_Rename("Profiles/Apps.json", "Profiles/CtrlApplications.json", true);
                File_Rename("Profiles/Controllers.json", "Profiles/DirectControllersProfile.json", true);

                //Check - If updater has been updated
                File_Rename("UpdaterNew.exe", "Updater.exe", true);

                //Check - If updater failed to cleanup
                File_Remove("AppUpdate.zip");
            }
            catch { }
        }
    }
}