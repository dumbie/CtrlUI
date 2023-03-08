using DirectXInput.KeyboardCode;
using DirectXInput.KeypadCode;
using DirectXInput.OverlayCode;
using System.Reflection;
using System.Windows;
using static ArnoldVinkCode.AVFirewall;
using static ArnoldVinkCode.AVInteropDll;
using static LibraryShared.AppCheck;
using static LibraryShared.AppUpdate;

namespace DirectXInput
{
    public partial class App : Application
    {
        //Application Windows
        public static WindowMain vWindowMain = new WindowMain();
        public static WindowOverlay vWindowOverlay = new WindowOverlay();
        public static WindowKeyboard vWindowKeyboard = new WindowKeyboard();
        public static WindowKeypad vWindowKeypad = new WindowKeypad();

        //Application Startup
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                //Application startup checks
                await StartupCheck("DirectXInput", ProcessPriority.High);

                //Application update checks
                await UpdateCheck();

                //Allow application in firewall
                string appFilePath = Assembly.GetEntryAssembly().Location;
                Firewall_ExecutableAllow("DirectXInput", appFilePath, true);

                //Run application startup code
                await vWindowMain.Startup();
            }
            catch { }
        }
    }
}