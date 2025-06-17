using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.Styles.MainColors;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    public partial class WindowMain : Window
    {
        //Window Initialize
        public WindowMain() { InitializeComponent(); }

        //Window Variables
        private IntPtr vInteropWindowHandle = IntPtr.Zero;

        //Window Initialized
        protected override async void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Get interop window handle
                vInteropWindowHandle = new WindowInteropHelper(this).EnsureHandle();

                //Register filter message
                ComponentDispatcher.ThreadFilterMessage += ReceivedFilterMessage;

                //Check application settings
                Folders_Check();
                Settings_Check();
                await Settings_Load();
                Settings_Save();

                //Check if resolution has changed
                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

                //Change application accent color
                string colorLightHex = SettingLoad(vConfigurationCtrlUI, "ColorAccentLight", typeof(string));
                ChangeApplicationAccentColor(colorLightHex);

                //Set the application clock style
                UpdateClockStyle();

                //Set content and resource images with Cache OnLoad
                SetContentResourceXamlImages();

                //Adjust the application font family
                AdjustApplicationFontStyle();

                //Adjust the application font size
                AdjustApplicationFontSize();

                //Adjust the application image size
                AdjustApplicationImageSize();

                //Check if application has launched as admin
                if (vAdministratorPermission)
                {
                    this.Title += " (Admin)";
                }

                //Check settings if need to minimize or focus window
                if (SettingLoad(vConfigurationCtrlUI, "LaunchMinimized", typeof(bool)))
                {
                    //Minimize CtrlUI window
                    await AppWindowMinimize(false, true);
                }
                else
                {
                    //Prevent or allow monitor sleep
                    UpdateMonitorSleepAuto();

                    //Focus on CtrlUI window
                    await AppWindowShow(true, true);
                }

                //Workaround for 64bit Windows problems with System32
                Wow64DisableWow64FsRedirection(IntPtr.Zero);

                //Registry enable linked connections
                RegistryEnableLinkedConnections();

                //Update the clock time
                UpdateClockTime();

                //Load the help text
                LoadHelp();

                //Add main menu items
                MainMenuAddItems();

                //Add categories to edit interface
                ManageInterface_AddCategories();

                //Register Interface Handlers
                RegisterInterfaceHandlers();

                //Bind all the lists to ListBox
                ListBoxBindLists();

                //Select the first ListBox item
                ListBoxResetIndexes();

                //Load Json stored apps
                await JsonLoadList_Applications();

                //Start the background tasks
                TasksBackgroundStart();

                //Check settings if DirectXInput launches on start
                if (SettingLoad(vConfigurationCtrlUI, "LaunchDirectXInput", typeof(bool)))
                {
                    await LaunchDirectXInput(true);
                }

                //Check settings if Fps Overlayer launches on start
                if (SettingLoad(vConfigurationCtrlUI, "LaunchFpsOverlayer", typeof(bool)))
                {
                    await LaunchFpsOverlayer(true);
                }

                //Check settings if Screen Capture Tool launches on start
                if (SettingLoad(vConfigurationCtrlUI, "LaunchScreenCaptureTool", typeof(bool)))
                {
                    await LaunchScreenCaptureTool(true);
                }

                //Check settings if this is the first application launch
                if (SettingLoad(vConfigurationCtrlUI, "AppFirstLaunch", typeof(bool)))
                {
                    await FirstLaunchAddApps();
                }

                //Update controller help
                UpdateControllerHelp();

                //Update controller color
                UpdateControllerColor();

                //Enable the socket server
                await EnableSocketServer();

                //Change listbox category to default
                await CategoryListChange(vCurrentListCategory);

                //Clean application update files
                await UpdateCleanup();

                //Check for available application update
                await UpdateCheck(true);
            }
            catch { }
        }

        //Enable the socket server
        private async Task EnableSocketServer()
        {
            try
            {
                int socketServerPort = SettingLoad(vConfigurationCtrlUI, "ServerPort", typeof(int));
                vArnoldVinkSockets = new ArnoldVinkSockets("127.0.0.1", socketServerPort, false, true);
                vArnoldVinkSockets.vSocketTimeout = 250;
                vArnoldVinkSockets.EventBytesReceived += ReceivedSocketHandler;
                await vArnoldVinkSockets.SocketServerEnable();
            }
            catch { }
        }

        //Application Close Handler
        protected async override void OnClosing(CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                await AppExit.Exit_Prompt();
            }
            catch { }
        }
    }
}