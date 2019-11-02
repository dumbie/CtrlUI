using System.Windows;
using static LibraryShared.AppLaunchCheck;

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
                Application_LaunchCheck("CtrlUI Updater", "Updater", false);

                //Open the window main from application
                vWindowMain.Show();
                await vWindowMain.Startup();
            }
            catch { }
        }
    }
}