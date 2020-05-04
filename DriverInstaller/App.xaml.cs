using System.Diagnostics;
using System.Windows;
using static LibraryShared.AppStartupCheck;

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
                //Check the application status
                await Application_LaunchCheck("Driver Installer", ProcessPriorityClass.Normal, false, true);

                //Check the application update
                Application_UpdateCheck();

                //Open the application window
                vWindowMain.Show();
            }
            catch { }
        }
    }
}