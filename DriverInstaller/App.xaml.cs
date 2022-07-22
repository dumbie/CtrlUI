using System.Diagnostics;
using System.Windows;
using static LibraryShared.AppCheck;
using static LibraryShared.AppUpdate;

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
                //Application startup checks
                await StartupCheck("Driver Installer", ProcessPriorityClass.Normal);

                //Application update checks
                await UpdateCheck();

                //Open the application window
                vWindowMain.Show();
            }
            catch { }
        }
    }
}