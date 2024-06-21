using System;
using System.Windows;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVStartup;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                //Setup application defaults
                SetupDefaults(ProcessPriority.High, true);

                //Run application startup code
                await vWindowMain.Application_Startup();
            }
            catch { }
        }
    }
}