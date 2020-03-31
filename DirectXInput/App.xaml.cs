using System.Diagnostics;
using System.Reflection;
using System.Windows;
using static ArnoldVinkCode.AVFirewall;
using static LibraryShared.AppStartupCheck;

namespace DirectXInput
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
                //Check the application update
                Application_UpdateCheck();

                //Check the application status
                Application_LaunchCheck("DirectXInput", "DirectXInput", ProcessPriorityClass.High, false);

                //Allow application in firewall
                string appFilePath = Assembly.GetEntryAssembly().Location;
                Firewall_ExecutableAllow("DirectXInput", appFilePath, true);

                //Run application startup code
                await vWindowMain.Startup();
            }
            catch { }
        }
    }
}