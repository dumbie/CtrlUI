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
        readonly public static bool vAdministratorPermission = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        //Interaction Variables
        public static bool vSingleTappedEvent = true;

        //Dispatcher Timers
        public static DispatcherTimer vDispatcherTimerMapping = new DispatcherTimer();
        public static DispatcherTimer vDispatcherTimerOverlay = new DispatcherTimer();

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
        public static int vControllerDelayShortTicks = 130;
        public static int vControllerDelayMediumTicks = 250;
        public static int vControllerDelayLongTicks = 750;
        public static bool vControllerRumbleTest = false;
        public static List<string> vControllerTempBlockPaths = new List<string>();
        public static ControllerStatus vController0 = new ControllerStatus(0);
        public static ControllerStatus vController1 = new ControllerStatus(1);
        public static ControllerStatus vController2 = new ControllerStatus(2);
        public static ControllerStatus vController3 = new ControllerStatus(3);
        public static bool vControllerAnyConnected()
        {
            return vController0.Connected() || vController1.Connected() || vController2.Connected() || vController3.Connected();
        }

        //Sockets Variables
        public static ArnoldVinkSockets vArnoldVinkSockets = null;

        //Application Lists
        public static List<ProfileShared> vDirectCloseTools = new List<ProfileShared>();
        public static List<ControllerSupported> vDirectControllersSupported = new List<ControllerSupported>();
        public static List<ControllerSupported> vDirectControllersIgnored = new List<ControllerSupported>();
        public static ObservableCollection<ControllerProfile> vDirectControllersProfile = new ObservableCollection<ControllerProfile>();
    }
}