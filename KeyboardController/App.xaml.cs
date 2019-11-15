using System.Diagnostics;
using System.Windows;
using static LibraryShared.AppLaunchCheck;

namespace KeyboardController
{
    public partial class App : Application
    {
        //Application Windows
        public static WindowMain vWindowMain = new WindowMain();
        public static WindowSettings vWindowSettings = new WindowSettings();

        //Application Startup
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                //Check the application status
                Application_LaunchCheck("Keyboard Controller", "KeyboardController", ProcessPriorityClass.High, false);

                //Open the window main from application
                vWindowMain.Show();
                await vWindowMain.Startup();
            }
            catch { }
        }
    }
}