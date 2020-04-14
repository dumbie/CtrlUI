using System.Diagnostics;
using System.Windows;
using static LibraryShared.AppStartupCheck;

namespace Updater
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
                await Application_LaunchCheck("Application Updater", ProcessPriorityClass.Normal, true, true);

                //Open the application window
                vWindowMain.Show();
            }
            catch { }
        }
    }
}