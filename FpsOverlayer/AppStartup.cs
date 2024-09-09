using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInteropDll;
using static FpsOverlayer.AppBackup;
using static FpsOverlayer.AppVariables;
using static LibraryShared.AppUpdate;

namespace FpsOverlayer
{
    public class AppStartup
    {
        public async static Task Startup()
        {
            try
            {
                Debug.WriteLine("Welcome to application.");

                //Setup application defaults
                AVStartup.SetupDefaults(ProcessPriority.High, true);

                //Application update checks
                await UpdateCheck();

                //Backup Notes
                BackupNotes();

                //Open the application window
                vWindowMain.Show();
            }
            catch { }
        }
    }
}