using System.Windows;

namespace DriverInstaller
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                await AppStartup.Startup();
            }
            catch { }
        }
    }
}