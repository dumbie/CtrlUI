using System.Diagnostics;
using static ArnoldVinkCode.AVFiles;

namespace LibraryShared
{
    public partial class AppStartupCheck
    {
        public static void Application_UpdateCheck()
        {
            try
            {
                Debug.WriteLine("Checking application update.");

                //Remove old unused files
                File_Delete("Resources/LibraryUsb.dll");

                //Move old profiles
                File_Move("Profiles/CtrlApplications.json", "Profiles/User/CtrlApplications.json", true);
                File_Move("Profiles/CtrlHDRProcessName.json", "Profiles/User/CtrlHDRProcessName.json", true);
                File_Move("Profiles/CtrlIgnoreProcessName.json", "Profiles/User/CtrlIgnoreProcessName.json", true);
                File_Move("Profiles/CtrlIgnoreLauncherName.json", "Profiles/User/CtrlIgnoreLauncherName.json", true);
                File_Move("Profiles/CtrlIgnoreShortcutName.json", "Profiles/User/CtrlIgnoreShortcutName.json", true);
                File_Move("Profiles/CtrlIgnoreShortcutUri.json", "Profiles/User/CtrlIgnoreShortcutUri.json", true);
                File_Move("Profiles/CtrlKeyboardExtensionName.json", "Profiles/User/CtrlKeyboardExtensionName.json", true);
                File_Move("Profiles/CtrlKeyboardProcessName.json", "Profiles/User/CtrlKeyboardProcessName.json", true);
                File_Move("Profiles/CtrlLocationsFile.json", "Profiles/User/CtrlLocationsFile.json", true);
                File_Move("Profiles/CtrlLocationsShortcut.json", "Profiles/User/CtrlLocationsShortcut.json", true);
                File_Move("Profiles/FpsPositionProcessName.json", "Profiles/User/FpsPositionProcessName.json", true);
                File_Move("Profiles/DirectKeyboardTextList.json", "Profiles/User/DirectKeyboardTextList.json", true);
                File_Move("Profiles/DirectControllersIgnored.json", "Profiles/User/DirectControllersIgnored.json", true);

                //Rename old folder names
                Directory_Move("Assets/Roms", "Assets/User/Games", true);

                //Check - If updater has been updated
                File_Move("UpdaterNew.exe", "Updater.exe", true);
                File_Move("Resources/UpdaterReplace.exe", "Updater.exe", true);

                //Check - If updater failed to cleanup
                File_Delete("Resources/AppUpdate.zip");
            }
            catch { }
        }
    }
}