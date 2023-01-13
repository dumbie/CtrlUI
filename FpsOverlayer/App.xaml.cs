using System.Diagnostics;
using System.Reflection;
using System.Windows;
using static ArnoldVinkCode.AVFirewall;
using static FpsOverlayer.AppVariables;
using static LibraryShared.AppCheck;
using static LibraryShared.AppUpdate;

namespace FpsOverlayer
{
    public partial class App : Application
    {
        //Application Startup
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                //Application startup checks
                await StartupCheck("Fps Overlayer", ProcessPriorityClass.High);

                //Application update checks
                await UpdateCheck();

                //Allow application in firewall
                string appFilePath = Assembly.GetEntryAssembly().Location;
                Firewall_ExecutableAllow("Fps Overlayer", appFilePath, true);

                //Open the application window
                vWindowMain.Show();
            }
            catch { }
        }
    }
}