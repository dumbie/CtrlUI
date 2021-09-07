using System.Diagnostics;
using System.Reflection;
using System.Windows;
using static ArnoldVinkCode.AVFirewall;
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
                //Check the application status
                await Application_LaunchCheck("Fps Overlayer", ProcessPriorityClass.High, false, false);

                //Check the application update
                Application_UpdateCheck();

                //Allow application in firewall
                string appFilePath = Assembly.GetEntryAssembly().Location;
                Firewall_ExecutableAllow("Fps Overlayer", appFilePath, true);

                //Open the application window
                await vWindowMain.Show();
            }
            catch { }
        }
    }
}