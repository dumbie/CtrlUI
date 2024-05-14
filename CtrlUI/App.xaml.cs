using System.Windows;

namespace CtrlUI
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                await AppStartup.Startup(e);
            }
            catch { }
        }
    }
}