using System.Diagnostics;
using System.Windows;
using static LibraryShared.AppLaunchCheck;

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
                //Check application status
                Application_LaunchCheck("DirectXInput", "DirectXInput", ProcessPriorityClass.RealTime, false);

                await vWindowMain.Startup();
            }
            catch { }
        }
    }
}