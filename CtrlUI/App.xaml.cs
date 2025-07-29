using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    public partial class App : Application
    {
        //Initialize
        public App()
        {
            InitializeComponent();
            this.UnhandledException += (sender, e) =>
            {
                e.Handled = true;
                Debug.WriteLine("App unhandled exception: " + e.Message);
            };
        }

        //Startup
        public static async Task Main(string[] args)
        {
            try
            {
                //Create app in main thread
                vApp = new App();

                //App startup
                await AppStartup.Startup(args);
            }
            catch { }
        }
    }
}