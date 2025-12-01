using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkStyles.MainColors;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    public partial class WindowMain : Page
    {
        //Window Initialize
        public WindowMain()
        {
            InitializeComponent();
            Loaded += async delegate { await WindowMain_Loaded(); };
        }

        //Window Initialized
        private async Task WindowMain_Loaded()
        {
            try
            {
                //Register filter message
                vWindowMain.ForwardMessage += ReceivedFilterMessage;
                vWindowMain.CloseRequested += WindowMain_CloseRequested;

                //Check application settings
                Folders_Check();
                Settings_Check();
                await Settings_Items();
                await Settings_Load();
                Settings_Save();

                //Check if resolution has changed
                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

                //Change application accent color
                string colorLightHex = vSettings.Load("ColorAccentLight", typeof(string));
                ChangeApplicationAccentColor(colorLightHex);

                //Set the application clock style
                await UpdateClockStyle();

                //Set content and resource images with Cache OnLoad
                await SetContentResourceXamlImages();

                //Adjust the application font family
                AdjustApplicationFontStyle();

                //Adjust the application font size
                AdjustApplicationFontSize();

                //Adjust the application image size
                AdjustApplicationImageSize();

                //Check settings if need to minimize or focus window
                if (vSettings.Load("LaunchMinimized", typeof(bool)))
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

                //Update clock time
                UpdateClockTime();

                //Load help text
                LoadHelp();

                //Add main menu items
                await MainMenuAddItems();

                //Add categories to edit interface
                await ManageInterface_AddCategories();

                //Register Interface Handlers
                RegisterInterfaceHandlers();

                //Bind all lists to ListView
                ListViewBindLists();

                //Select the first ListView item
                ListViewResetIndexes();

                //Load Json stored apps
                await JsonLoadList_Applications();

                //Start background tasks
                TasksBackgroundStart();

                //Check settings if DirectXInput launches on start
                if (vSettings.Load("LaunchDirectXInput", typeof(bool)))
                {
                    LaunchDirectXInput(true);
                }

                //Check settings if FpsOverlayer launches on start
                if (vSettings.Load("LaunchFpsOverlayer", typeof(bool)))
                {
                    LaunchFpsOverlayer(true);
                }

                //Check settings if ScreenCapy launches on start
                if (vSettings.Load("LaunchScreenCapy", typeof(bool)))
                {
                    LaunchScreenCapy(true);
                }

                //Check settings if this is the first application launch
                if (vSettings.Load("AppFirstLaunch", typeof(bool)))
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
                int socketServerPort = vSettings.Load("ServerPort", typeof(int));
                vArnoldVinkSockets = new ArnoldVinkSockets("127.0.0.1", socketServerPort, false, true);
                vArnoldVinkSockets.vSocketTimeout = 250;
                vArnoldVinkSockets.EventBytesReceived += ReceivedSocketHandler;
                await vArnoldVinkSockets.SocketServerEnable();
            }
            catch { }
        }

        //Application Close Handler
        private async void WindowMain_CloseRequested()
        {
            try
            {
                await Exit_Prompt();
            }
            catch { }
        }
    }
}