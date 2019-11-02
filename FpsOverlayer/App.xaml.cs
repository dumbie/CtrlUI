using System.Windows;
using static LibraryShared.AppLaunchCheck;

namespace FpsOverlayer
{
    public partial class App : Application
    {
        //Application Windows
        public static WindowMain vWindowMain = new WindowMain();
        public static WindowSettings vWindowSettings = new WindowSettings();
        public static WindowApplications vWindowApplications = new WindowApplications();

        //Application Startup
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                //Check the application status
                Application_LaunchCheck("Fps Overlayer", "FpsOverlayer", false);

                //Open the window main from application
                vWindowMain.Show();
                vWindowMain.Startup();
            }
            catch { }
        }
    }
}