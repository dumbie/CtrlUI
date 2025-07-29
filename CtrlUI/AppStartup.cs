using ArnoldVinkCode;
using ArnoldVinkStyles;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkStyles.AVWindow;
using static CtrlUI.AppBackup;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    public partial class AppStartup
    {
        public static async Task Startup(string[] startupArguments)
        {
            try
            {
                Debug.WriteLine("Welcome to application.");

                //Application restart delay
                await RestartDelay(startupArguments);

                //Setup application defaults
                AVStartup.SetupDefaults(ProcessPriorityClasses.HIGH_PRIORITY_CLASS, true);

                //Backup Json profiles
                BackupJsonProfiles();

                //Set window title
                string windowTitle = "CtrlUI";

                //Check if application has launched as admin
                if (vAdministratorPermission)
                {
                    windowTitle += " (Admin)";
                }

                //Open application window
                AVWindowDetails windowDetails = new AVWindowDetails()
                {
                    Type = typeof(WindowMain),
                    Title = windowTitle,
                    IconPath = "CtrlUI.Assets.AppIcon.ico",
                    Width = 1280,
                    Height = 720,
                    TopMost = true,
                    NoBorder = true,
                    Transparency = true
                };
                AppVariables.vWindowMain = new AVWindow(windowDetails);
            }
            catch { }
        }

        //Application restart delay
        private static async Task RestartDelay(string[] startupArguments)
        {
            try
            {
                if (startupArguments != null && startupArguments.Contains("-restart"))
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