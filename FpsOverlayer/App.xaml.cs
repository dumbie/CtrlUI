using System.Windows;

namespace FpsOverlayer
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