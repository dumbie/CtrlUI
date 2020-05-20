using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVFirewall;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessFunctions;
using static CtrlUI.AppVariables;
using static LibraryShared.AppStartupCheck;

namespace CtrlUI
{
    public partial class App : Application
    {
        //Application Windows
        public static WindowMain vWindowMain = new WindowMain();

        //Application Startup
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                //Application restart delay
                await Application_RestartDelay(e);

                //Get previous focused application
                vPrevFocusedProcess = GetProcessMultiFromWindowHandle(GetForegroundWindow());

                //Check the application status
                await Application_LaunchCheck("CtrlUI", ProcessPriorityClass.High, false, true);

                //Check the application update
                Application_UpdateCheck();

                //Allow application in firewall
                string appFilePath = Assembly.GetEntryAssembly().Location;
                Firewall_ExecutableAllow("CtrlUI", appFilePath, true);

                //Open the application window
                vWindowMain.Show();
            }
            catch { }
        }

        //Application restart delay
        private async Task Application_RestartDelay(StartupEventArgs e)
        {
            try
            {
                if (e.Args != null && e.Args.Contains("-restart"))
                {
                    Process currentProcess = Process.GetCurrentProcess();
                    string processName = currentProcess.ProcessName;
                    while (Process.GetProcessesByName(processName).Length > 1)
                    {
                        await Task.Delay(500);
                    }
                }
            }
            catch { }
        }
    }
}