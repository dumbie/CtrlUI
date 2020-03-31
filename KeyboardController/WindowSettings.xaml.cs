using System;
using System.ComponentModel;
using System.Windows;

namespace KeyboardController
{
    public partial class WindowSettings : Window
    {
        //Window Initialize
        public WindowSettings() { InitializeComponent(); }

        //Window Initialized
        protected override void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Check application settings
                Settings_Load();
                Settings_Save();
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