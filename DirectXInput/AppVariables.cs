using ArnoldVinkCode;
using LibraryUsb;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Security.Principal;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.Media.Control;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkCode.AVSettings;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace DirectXInput
{
    public class AppVariables
    {
        //Application Variables
        readonly public static bool vAdministratorPermission = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        public static Configuration vConfigurationCtrlUI = SettingLoadConfig("CtrlUI.exe.csettings");
        public static Configuration vConfigurationDirectXInput = SettingLoadConfig("DirectXInput.exe.csettings");

        //Image Variables
        public static string vImageBackupSource = "Assets/Default/Apps/Unknown.png";
        public static BitmapImage vImagePreloadIconControllerAccent = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller-Accent.png" }, null, vImageBackupSource, IntPtr.Zero, 100, 0);
        public static BitmapImage vImagePreloadIconControllerDark = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller-Dark.png" }, null, vImageBackupSource, IntPtr.Zero, 100, 0);

        //Interaction Variables
        public static bool vSingleTappedEvent = true;
        public static string[] vVerticalLoopTargetLists = { "listbox_TextList", "listbox_ShortcutList" };
        public static bool vShowDebugInformation = false;

        //Dispatcher Timers
        public static DispatcherTimer vDispatcherTimerOverlay = new DispatcherTimer();

        //Mapping Variables
        public static DispatcherTimer vMappingControllerTimer = new DispatcherTimer();
        public static MappingStatus vMappingControllerStatus = MappingStatus.Done;
        public static Button vMappingControllerButton = null;
        public static Button vMappingKeypadButton = null;

        //MessageBox Variables
        public static bool vMessageBoxOpen = false;
        public static bool vMessageBoxPopupCancelled = false;
        public static int vMessageBoxPopupResult = 0;

        //Process Variables
        public static ProcessMulti vProcessCurrent = Get_ProcessMultiCurrent();
        public static ProcessMulti vProcessCtrlUI = null;
        public static ProcessMulti vProcessFpsOverlayer = null;
        public static ProcessMulti vProcessForeground = null;
        public static bool vProcessCtrlUIActivated = false;

        //App Status Variables
        public static bool vAppMaximized = false;
        public static bool vAppMinimized = false;
        public static bool vAppActivated = true;
        public static bool vComboboxSaveEnabled = true;
        public static int vComboboxIndexPrev = -1;

        //Emoji and text list Variables
        public static int vLastPopupListTextIndex = 0;
        public static int vLastPopupListEmojiIndex = 0;
        public static int vLastPopupListShortcutIndex = 0;
        public static string vLastPopupListType = "Emoji";
        public static FrameworkElementFocus vFocusedButtonKeyboard = new FrameworkElementFocus();
        public static FrameworkElementFocus vFocusedButtonText = new FrameworkElementFocus();
        public static FrameworkElementFocus vFocusedButtonEmoji = new FrameworkElementFocus();
        public static FrameworkElementFocus vFocusedButtonShortcut = new FrameworkElementFocus();
        public static int vDirectKeyboardEmojiIndexActivity = 0;
        public static int vDirectKeyboardEmojiIndexNature = 0;
        public static int vDirectKeyboardEmojiIndexFood = 0;
        public static int vDirectKeyboardEmojiIndexOther = 0;
        public static int vDirectKeyboardEmojiIndexPeople = 0;
        public static int vDirectKeyboardEmojiIndexSmiley = 0;
        public static int vDirectKeyboardEmojiIndexSymbol = 0;
        public static int vDirectKeyboardEmojiIndexTravel = 0;

        //Color Variables
        public static SolidColorBrush vKeypadNormalBrush = null;
        public static SolidColorBrush vApplicationAccentLightBrush = null;

        //Keyboard Variables
        public static GlobalSystemMediaTransportControlsSessionManager vSmtcSessionManager = null;
        public static string vKeyboardKeypadLastActive = "Keyboard";
        public static bool vCapsEnabled = false;
        public static bool vKeysEnabled = true;
        public static bool vMouseLeftDownStatus = false;
        public static bool vMouseRightDownStatus = false;

        //Keypad Variables
        public static KeypadMapping vKeypadMappingProfile = new KeypadMapping();
        public static string vKeypadPreviousProcessName = string.Empty;
        public static string vKeypadPreviousProcessTitle = string.Empty;
        public static double vKeypadImageHeight = 240;

        //Virtual Variables
        public static FakerInputDevice vFakerInputDevice = null;

        //Controller Variables
        public static HidHideDevice vHidHideDevice = null;
        public static WinUsbDevice vVirtualBusDevice = null;
        public static bool vControllerBusy = false;
        public static bool vControllerMuteLedCurrent = false;
        public static bool vControllerMuteLedPrevious = false;
        public static int vControllerThumbOffset2500 = 2500;
        public static int vControllerThumbOffset7500 = 7500;
        public static int vControllerThumbOffset10000 = 10000;
        public static int vControllerThumbOffset15000 = 15000;
        public static int vControllerDelayTicks10 = 10;
        public static int vControllerDelayTicks125 = 125;
        public static int vControllerDelayTicks250 = 250;
        public static int vControllerDelayTicks500 = 500;
        public static int vControllerDelayTicks750 = 750;
        public static int vControllerDelayTicks1000 = 1000;
        public static long vControllerDelay_KeypadControl = 0;
        public static long vControllerDelay_KeypadPreview = 0;
        public static long vControllerDelay_Keyboard = 0;
        public static long vControllerDelay_Media = 0;
        public static long vControllerDelay_Mouse = 0;
        public static bool vControllerRumbleTest = false;
        public static ControllerStatus vController0 = new ControllerStatus(0);
        public static ControllerStatus vController1 = new ControllerStatus(1);
        public static ControllerStatus vController2 = new ControllerStatus(2);
        public static ControllerStatus vController3 = new ControllerStatus(3);

        //Returns if a controller is connected
        public static bool vControllerAnyConnected()
        {
            return vController0.Connected() || vController1.Connected() || vController2.Connected() || vController3.Connected();
        }

        //Returns if a controller is disconnecting
        public static bool vControllerAnyDisconnecting()
        {
            return vController0.Disconnecting || vController1.Disconnecting || vController2.Disconnecting || vController3.Disconnecting;
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
        public static List<ControllerIgnored> vDirectControllersIgnored = new List<ControllerIgnored>();
        public static List<ProfileShared> vDirectKeyboardEmojiListActivity = new List<ProfileShared>();
        public static List<ProfileShared> vDirectKeyboardEmojiListNature = new List<ProfileShared>();
        public static List<ProfileShared> vDirectKeyboardEmojiListFood = new List<ProfileShared>();
        public static List<ProfileShared> vDirectKeyboardEmojiListOther = new List<ProfileShared>();
        public static List<ProfileShared> vDirectKeyboardEmojiListPeople = new List<ProfileShared>();
        public static List<ProfileShared> vDirectKeyboardEmojiListSmiley = new List<ProfileShared>();
        public static List<ProfileShared> vDirectKeyboardEmojiListSymbol = new List<ProfileShared>();
        public static List<ProfileShared> vDirectKeyboardEmojiListTravel = new List<ProfileShared>();
        public static ObservableCollection<ProfileShared> vDirectKeyboardToolList = new ObservableCollection<ProfileShared>();
        public static ObservableCollection<ProfileShared> vDirectKeyboardTextList = new ObservableCollection<ProfileShared>();
        public static ObservableCollection<ProfileShared> vDirectKeyboardShortcutList = new ObservableCollection<ProfileShared>();
        public static ObservableCollection<KeypadMapping> vDirectKeypadMapping = new ObservableCollection<KeypadMapping>();
        public static ObservableCollection<ControllerProfile> vDirectControllersProfile = new ObservableCollection<ControllerProfile>();
        public static ObservableCollection<ProfileShared> vControllerDebugInput = new ObservableCollection<ProfileShared>(new ProfileShared[180]);
    }
}