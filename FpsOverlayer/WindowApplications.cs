using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using static LibraryShared.Classes;

namespace FpsOverlayer
{
    public partial class WindowApplications : Window
    {
        //Application Launch
        public WindowApplications()
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
                //Register Interface Handlers
                RegisterInterfaceHandlers();

                //Load position processes
                LoadPositionProcesses();

                Debug.WriteLine("Loaded application.");
            }
            catch { }
        }

        //Register Interface Handlers
        void RegisterInterfaceHandlers()
        {
            try
            {
                button_AddApp.Click += Button_AddApp_Click;
            }
            catch { }
        }

        //Add application to the list
        void Button_AddApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string processName = textbox_AddApp.Text;
                Debug.WriteLine("Adding new process: " + processName);

                //Color brushes
                BrushConverter BrushConvert = new BrushConverter();
                Brush BrushInvalid = BrushConvert.ConvertFromString("#CD1A2B") as Brush;
                Brush BrushValid = BrushConvert.ConvertFromString("#1DB954") as Brush;

                //Check if the name is empty
                if (string.IsNullOrWhiteSpace(processName))
                {
                    textbox_AddApp.BorderBrush = BrushInvalid;
                    Debug.WriteLine("Please enter an application process.");
                    return;
                }

                //Check if the name is place holder
                if (processName == "Process name")
                {
                    textbox_AddApp.BorderBrush = BrushInvalid;
                    Debug.WriteLine("Please enter an application process.");
                    return;
                }

                //Check if process already exists
                if (AppVariables.vFpsPositionProcess.Any(x => x.Process.ToLower() == processName.ToLower()))
                {
                    textbox_AddApp.BorderBrush = BrushInvalid;
                    Debug.WriteLine("Application process already exists.");
                    return;
                }

                //Clear name from the textbox
                textbox_AddApp.Text = "Process name";

                FpsPositionProcess fpsPositionProcess = new FpsPositionProcess();
                fpsPositionProcess.Process = processName;
                fpsPositionProcess.Position = 0;

                AppVariables.vFpsPositionProcess.Add(fpsPositionProcess);
                JsonFunctions.JsonSaveObject(AppVariables.vFpsPositionProcess, "FpsPositionProcess");

                textbox_AddApp.BorderBrush = BrushValid;
            }
            catch { }
        }

        //Load position processes
        void LoadPositionProcesses()
        {
            try
            {
                listbox_Apps.ItemsSource = AppVariables.vFpsPositionProcess;
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