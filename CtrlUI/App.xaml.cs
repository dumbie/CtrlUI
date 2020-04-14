using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVFirewall;
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
                //Restart wait fix
                await Task.Delay(2000);

                //Get previous focused application
                vPrevFocusedProcess = GetFocusedProcess();

                //Check the application update
                Application_UpdateCheck();

                //Check the application status
                await Application_LaunchCheck("CtrlUI", ProcessPriorityClass.High, false, true);

                //Allow application in firewall
                string appFilePath = Assembly.GetEntryAssembly().Location;
                Firewall_ExecutableAllow("CtrlUI", appFilePath, true);

                //Open the application window
                vWindowMain.Show();
            }
            catch { }
        }
    }
}