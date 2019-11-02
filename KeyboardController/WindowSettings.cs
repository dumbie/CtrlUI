using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace KeyboardController
{
    public partial class WindowSettings : Window
    {
        //Application Launch
        public WindowSettings()
        {
            try
            {
                //Initialize Component
                InitializeComponent();

                //Start loading the application
                Loaded += Application_Loaded;
            }
            catch { }
        }

        //Application Loading
        void Application_Loaded(object sender, RoutedEventArgs args)
        {
            try
            {
                Settings_Load();
                Settings_Save();

                Debug.WriteLine("Loaded application.");
            }
            catch { }
        }

        //Application Close Handler
        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                this.Hide();
            }
            catch { }
        }
    }
}