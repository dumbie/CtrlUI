using System.Windows;
using static ArnoldVinkCode.ArnoldVinkProcesses;
using static CtrlUI.AppVariables;
using static LibraryShared.AppLaunchCheck;

namespace CtrlUI
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
                //Get previous focused application
                vPrevFocusedProcess = GetFocusedProcess();

                //Check the application status
                Application_LaunchCheck("CtrlUI", "CtrlUI", false);

                //Open the window main from application
                vWindowMain.Show();
                await vWindowMain.Startup();
            }
            catch { }
        }
    }
}