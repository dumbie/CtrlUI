using System;
using System.Windows;
using static ArnoldVinkCode.AVAssembly;

namespace ScreenCapture
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                //Resolve missing assembly dll files
                AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveFile;

                await AppStartup.Application_Startup(e.Args);
            }
            catch { }
        }
    }
}