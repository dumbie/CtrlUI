using System.Windows;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                await vWindowMain.Application_Startup();
            }
            catch { }
        }
    }
}