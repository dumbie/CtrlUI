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
        public static Configuration vConfigurationCtrlUI = null;
        public static bool vAdministratorPermission = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        public static string[] vAppsOtherTools = new string[] { };
        public static double vInterfaceSoundVolume = 0.80;

        //Interaction Variables
        public static bool vSingleTappedEvent = true;
        public static DispatcherTimer vDispatcherTimer = new DispatcherTimer();

        //Message Box Variables
        public static bool vMessageBoxOpen = false;
        public static bool vMessageBoxPopupCancelled = false;
        public static int vMessageBoxPopupResult = 0;

        //Process Variables
        public static Process vProcessCurrent = Process.GetCurrentProcess();
        public static Process vProcessCtrlUI = null;
        public static bool vProcessCtrlUIActivated = false;
        public static Process vProcessKeyboardController = null;

        //App Status Variables
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
        public static ControllerStatus vController0 = new ControllerStatus(0);
        public static ControllerStatus vController1 = new ControllerStatus(1);
        public static ControllerStatus vController2 = new ControllerStatus(2);
        public static ControllerStatus vController3 = new ControllerStatus(3);

        //Sockets Variables
        public static ArnoldVinkSockets vArnoldVinkSockets = null;

        //Application Lists
        public static ObservableCollection<ControllerProfile> List_ControllerProfile = new ObservableCollection<ControllerProfile>();
        public static ObservableCollection<ControllerSupported> List_ControllerSupported = new ObservableCollection<ControllerSupported>();
    }
}