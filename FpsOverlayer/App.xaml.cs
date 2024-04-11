using System;
using System.Windows;
using static ArnoldVinkCode.AVAssembly;
using static ArnoldVinkCode.AVInteropDll;
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
                //Resolve missing assembly dll files
                AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveFile;

                //Application startup checks
                await StartupCheck("Fps Overlayer", ProcessPriority.High);

                //Application update checks
                await UpdateCheck();

                //Open the application window
                vWindowMain.Show();
            }
            catch { }
        }
    }
}