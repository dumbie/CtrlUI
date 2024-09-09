using ArnoldVinkCode;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVProcess;
using static CtrlUI.AppBackup;
using static CtrlUI.AppVariables;
using static LibraryShared.AppUpdate;

namespace CtrlUI
{
    public partial class AppStartup
    {
        public static async Task Startup(StartupEventArgs e)
        {
            try
            {
                Debug.WriteLine("Welcome to application.");

                //Application restart delay
                await RestartDelay(e);

                //Setup application defaults
                AVStartup.SetupDefaults(ProcessPriority.High, true);

                //Application update checks
                await UpdateCheck();

                //Backup Json profiles
                BackupJsonProfiles();

                //Open the application window
                vWindowMain.Show();
            }
            catch { }
        }

        //Application restart delay
        private static async Task RestartDelay(StartupEventArgs e)
        {
            try
            {
                if (e.Args != null && e.Args.Contains("-restart"))
                {
                    //Get current process information
                    ProcessMulti currentProcess = Get_ProcessMultiCurrent();

                    //Check if application is already running
                    while (Get_ProcessesMultiByName(currentProcess.ExeNameNoExt, true).Count > 1)
                    {
                        await Task.Delay(500);
                    }
                }
            }
            catch { }
        }
    }
}