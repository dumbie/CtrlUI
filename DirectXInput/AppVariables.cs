using ArnoldVinkCode;
using LibraryUsb;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.ProcessClasses;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.Settings;

namespace DirectXInput
{
    public class AppVariables
    {
        //Application Variables
        readonly public static bool vAdministratorPermission = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        public static Configuration vConfigurationCtrlUI = Settings_Load_CtrlUI();
        public static Configuration vConfigurationDirectXInput = Settings_Load_DirectXInput();
        public static Configuration vConfigurationFpsOverlayer = Settings_Load_FpsOverlayer();

        //Image Variables
        public static ImageSourceFolders[] vImageSourceFolders =
        {
            new ImageSourceFolders() { SourcePath = "Assets/User/Apps", SearchOption = SearchOption.AllDirectories },
            new ImageSourceFolders() { SourcePath = "Assets/Default/Apps", SearchOption = SearchOption.AllDirectories },
            new ImageSourceFolders() { SourcePath = "Assets/User/Games", SearchOption = SearchOption.AllDirectories },
            new ImageSourceFolders() { SourcePath = "Assets/Default/Games", SearchOption = SearchOption.AllDirectories }
        };
        public static string vImageBackupSource = "Assets/Default/Apps/Unknown.png";
        public static BitmapImage vImagePreloadIconVolumeDown = FileToBitmapImage(new string[] { "Assets/Default/Icons/VolumeDown.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 100, 0);
        public static BitmapImage vImagePreloadIconVolumeMute = FileToBitmapImage(new string[] { "Assets/Default/Icons/VolumeMute.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 100, 0);
        public static BitmapImage vImagePreloadIconKeyboardMove = FileToBitmapImage(new string[] { "Assets/Default/Icons/KeyboardMove.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 100, 0);
        public static BitmapImage vImagePreloadIconKeyboardScroll = FileToBitmapImage(new string[] { "Assets/Default/Icons/KeyboardScroll.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 100, 0);
        public static BitmapImage vImagePreloadIconControllerAccent = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller-Accent.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 100, 0);
        public static BitmapImage vImagePreloadIconControllerDark = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller-Dark.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 100, 0);

        //Interaction Variables
        public static bool vSingleTappedEvent = true;

        //Emoji Variables
        public static Button vEmojiFocusedButtonOpen = null;
        public static Button vEmojiFocusedButtonClose = null;

        //Dispatcher Timers
        public static DispatcherTimer vDispatcherTimerOverlay = new DispatcherTimer();

        //Mapping Variables
        public static DispatcherTimer vMappingControllerTimer = new DispatcherTimer();
        public static MappingStatus vMappingControllerStatus = MappingStatus.Done;
        public static Button vMappingControllerButton = null;
        public static DispatcherTimer vMappingKeypadTimer = new DispatcherTimer();
        public static MappingStatus vMappingKeypadStatus = MappingStatus.Done;
        public static Button vMappingKeypadButton = null;

        //MessageBox Variables
        public static bool vMessageBoxOpen = false;
        public static bool vMessageBoxPopupCancelled = false;
        public static int vMessageBoxPopupResult = 0;

        //Process Variables
        public static Process vProcessCurrent = Process.GetCurrentProcess();
        public static Process vProcessCtrlUI = null;
        public static bool vProcessCtrlUIActivated = false;
        public static Process vProcessFpsOverlayer = null;
        public static ProcessMulti vProcessForeground = null;

        //App Status Variables
        public static bool vAppMaximized = false;
        public static bool vAppMinimized = false;
        public static bool vAppActivated = true;
        public static int vPrevComboboxIndex = -1;

        //Keyboard Variables
        public static bool vCapsEnabled = false;
        public static bool vKeysEnabled = true;
        public static bool vMouseDownStatus = false;

        //Keypad Variables
        public static KeypadMapping vKeypadMappingProfile = new KeypadMapping();
        public static KeypadStatus vKeypadDownStatus = new KeypadStatus();
        public static string vKeypadPreviousProcessName = string.Empty;
        public static string vKeypadPreviousProcessTitle = string.Empty;

        //Controller Variables
        public static WinUsbDevice vVirtualBusDevice = null;
        public static bool vControllerBusy = false;
        public static bool vControllerMuteLed = false;
        public static int vControllerOffsetSmall = 2500;
        public static int vControllerOffsetMedium = 7500;
        public static int vControllerOffsetNormal = 15000;
        public static int vControllerDelayNanoTicks = 10;
        public static int vControllerDelayMicroTicks = 75;
        public static int vControllerDelayShortTicks = 130;
        public static int vControllerDelayMediumTicks = 250;
        public static int vControllerDelayLongTicks = 500;
        public static int vControllerDelayLongerTicks = 750;
        public static int vControllerDelay_Keypad = Environment.TickCount;
        public static int vControllerDelay_Keyboard = Environment.TickCount;
        public static int vControllerDelay_Media = Environment.TickCount;
        public static int vControllerDelay_Mouse = Environment.TickCount;
        public static bool vControllerRumbleTest = false;
        public static DateTime vControllerLastDisconnect = new DateTime();
        public static List<string> vControllerTempBlockPaths = new List<string>();
        public static ControllerStatus vController0 = new ControllerStatus(0);
        public static ControllerStatus vController1 = new ControllerStatus(1);
        public static ControllerStatus vController2 = new ControllerStatus(2);
        public static ControllerStatus vController3 = new ControllerStatus(3);

        //Returns if a controller is connected
        public static bool vControllerAnyConnected()
        {
            return vController0.Connected || vController1.Connected || vController2.Connected || vController3.Connected;
        }

        //Returns the active controllerstatus
        public static ControllerStatus vActiveController()
        {
            try
            {
                if (vController0.Activated) { return vController0; }
                else if (vController1.Activated) { return vController1; }
                else if (vController2.Activated) { return vController2; }
                else if (vController3.Activated) { return vController3; }
            }
            catch { }
            return null;
        }

        //Sockets Variables
        public static ArnoldVinkSockets vArnoldVinkSockets = null;

        //Application Lists
        public static List<ProfileShared> vDirectCloseTools = new List<ProfileShared>();
        public static List<ControllerSupported> vDirectControllersSupported = new List<ControllerSupported>();
        public static List<ControllerSupported> vDirectControllersIgnored = new List<ControllerSupported>();
        public static ObservableCollection<KeypadMapping> vDirectKeypadMapping = new ObservableCollection<KeypadMapping>();
        public static ObservableCollection<ControllerProfile> vDirectControllersProfile = new ObservableCollection<ControllerProfile>();
    }
}