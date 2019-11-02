using ArnoldVinkCode;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Threading;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public class AppVariables
    {
        //Application Variables
        public static Configuration vConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        public static int vCurrentProcessId = Process.GetCurrentProcess().Id;
        public static bool vAdministratorPermission = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        public static string[] vAppsOtherTools = new string[] { };

        //Interaction Variables
        public static bool vSingleTappedEvent = true;
        public static DispatcherTimer vDispatcherTimer = new DispatcherTimer();

        //Message Box Variables
        public static bool vMessageBoxOpen = false;
        public static bool vMessageBoxPopupCancelled = false;
        public static int vMessageBoxPopupResult = 0;

        //App Status Variables
        public static Process vProcessCtrlUI = null;
        public static bool vProcessCtrlUIActivated = false;
        public static Process vProcessKeyboardController = null;
        public static bool vAppMaximized = false;
        public static bool vAppMinimized = false;
        public static bool vAppActivated = true;
        public static int vPrevComboboxIndex = -1;

        //Controller Variables
        public static bool vControllerBusy = false;
        public static int vControllerDelayPollingTicks = 25;
        public static int vControllerDelayMicroTicks = 75;
        public static int vControllerDelayShortTicks = 150;
        public static int vControllerDelayMediumTicks = 300;
        public static int vControllerDelayLongTicks = 750;
        public static bool vControllerRumbleTest = false;
        public static List<string> vControllerBlockedPaths = new List<string>();
        public static ControllerStatus vController0 = new ControllerStatus();
        public static ControllerStatus vController1 = new ControllerStatus();
        public static ControllerStatus vController2 = new ControllerStatus();
        public static ControllerStatus vController3 = new ControllerStatus();

        //Socket Variables
        public static ArnoldVinkSocketServer vSocketServer = new ArnoldVinkSocketServer();
        public static ArnoldVinkSocketClient vSocketClient = new ArnoldVinkSocketClient();

        //Application Lists
        public static ObservableCollection<ControllerProfile> List_ControllerProfile = new ObservableCollection<ControllerProfile>();
        public static ObservableCollection<ControllerSupported> List_ControllerSupported = new ObservableCollection<ControllerSupported>();
    }
}