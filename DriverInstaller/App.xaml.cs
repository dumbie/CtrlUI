using System.Windows;
using static LibraryShared.AppLaunchCheck;

namespace DriverInstaller
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
                Application_LaunchCheck("Driver Installer", "DriverInstaller", false, false);

                //Open the window main from application
                vWindowMain.Show();
                await vWindowMain.Startup();
            }
            catch { }
        }
    }
}