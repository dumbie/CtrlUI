using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVFirewall;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessFunctions;
using static CtrlUI.AppVariables;
using static LibraryShared.AppCheck;
using static LibraryShared.AppUpdate;

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

                //Application startup checks
                await StartupCheck("CtrlUI", ProcessPriorityClass.High);

                //Application update checks
                await UpdateCheck();

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