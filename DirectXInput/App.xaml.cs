using DirectXInput.KeyboardCode;
using DirectXInput.KeypadCode;
using DirectXInput.OverlayCode;
using System.Windows;

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
                await vWindowMain.Application_Startup();
            }
            catch { }
        }
    }
}