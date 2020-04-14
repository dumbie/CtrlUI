using System.Diagnostics;
using System.Windows;
using static LibraryShared.AppStartupCheck;

namespace FpsOverlayer
{
    public partial class App : Application
    {
        //Application Windows
        public static WindowMain vWindowMain = new WindowMain();
        public static WindowSettings vWindowSettings = new WindowSettings();
        public static WindowApplications vWindowApplications = new WindowApplications();

        //Application Startup
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                //Check the application update
                Application_UpdateCheck();

                //Check the application status
                await Application_LaunchCheck("Fps Overlayer", ProcessPriorityClass.High, false, false);

                //Open the application window
                vWindowMain.Show();
            }
            catch { }
        }
    }
}