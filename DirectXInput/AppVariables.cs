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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static ArnoldVinkCode.AVActions;
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
        public static string[] vVerticalLoopTargetLists = { "listbox_TextList" };
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
        public static Process vProcessCurrent = Process.GetCurrentProcess();
        public static Process vProcessCtrlUI = null;
        public static bool vProcessCtrlUIActivated = false;
        public static Process vProcessFpsOverlayer = null;
        public static ProcessMulti vProcessForeground = null;

        //App Status Variables
        public static bool vAppMaximized = false;
        public static bool vAppMinimized = false;
        public static bool vAppActivated = true;
        public static bool vComboboxSaveEnabled = true;
        public static int vComboboxIndexPrev = -1;

        //Emoji and text list Variables
        public static int vLastPopupListTextIndex = 0;
        public static int vLastPopupListEmojiIndex = 0;
        public static string vLastPopupListType = "Emoji";
        public static FrameworkElementFocus vEmojiFocusedButtonOpen = new FrameworkElementFocus();
        public static FrameworkElementFocus vEmojiFocusedButtonClose = new FrameworkElementFocus();
        public static FrameworkElementFocus vTextFocusedButtonOpen = new FrameworkElementFocus();
        public static FrameworkElementFocus vTextFocusedButtonClose = new FrameworkElementFocus();
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
        public static string vKeyboardKeypadLastActive = "Keyboard";
        public static bool vCapsEnabled = false;
        public static bool vKeysEnabled = true;
        public static bool vMouseLeftDownStatus = false;
        public static bool vMouseRightDownStatus = false;

        //Keypad Variables
        public static KeypadMapping vKeypadMappingProfile = new KeypadMapping();
        public static string vKeypadPreviousProcessName = string.Empty;
        public static string vKeypadPreviousProcessTitle = string.Empty;

        //Virtual Variables
        public static FakerInputDevice vFakerInputDevice = null;

        //Controller Variables
        public static HidHideDevice vHidHideDevice = null;
        public static WinUsbDevice vVirtualBusDevice = null;
        public static bool vControllerBusy = false;
        public static bool vControllerMuteLedCurrent = false;
        public static bool vControllerMuteLedPrevious = false;
        public static int vControllerThumbOffsetSmall = 2500;
        public static int vControllerThumbOffsetMedium = 7500;
        public static int vControllerThumbOffsetNormal = 10000;
        public static int vControllerThumbOffsetLarge = 15000;
        public static int vControllerDelayNanoTicks = 10;
        public static int vControllerDelayMicroTicks = 75;
        public static int vControllerDelayMacroTicks = 100;
        public static int vControllerDelayShortTicks = 125;
        public static int vControllerDelayMediumTicks = 250;
        public static int vControllerDelayLongTicks = 500;
        public static int vControllerDelayLongerTicks = 750;
        public static int vControllerDelayLongestTicks = 1000;
        public static long vControllerDelay_KeypadProfile = GetSystemTicksMs();
        public static long vControllerDelay_KeypadPreview = GetSystemTicksMs();
        public static long vControllerDelay_Keyboard = GetSystemTicksMs();
        public static long vControllerDelay_Media = GetSystemTicksMs();
        public static long vControllerDelay_Mouse = GetSystemTicksMs();
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
            return vController0.Connected() || vController1.Connected() || vController2.Connected() || vController3.Connected();
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
        public static List<ControllerIgnored> vDirectControllersIgnoredUser = new List<ControllerIgnored>();
        public static List<ControllerIgnored> vDirectControllersIgnoredDefault = new List<ControllerIgnored>();
        public static List<ProfileShared> vDirectKeyboardEmojiListActivity = new List<ProfileShared>();
        public static List<ProfileShared> vDirectKeyboardEmojiListNature = new List<ProfileShared>();
        public static List<ProfileShared> vDirectKeyboardEmojiListFood = new List<ProfileShared>();
        public static List<ProfileShared> vDirectKeyboardEmojiListOther = new List<ProfileShared>();
        public static List<ProfileShared> vDirectKeyboardEmojiListPeople = new List<ProfileShared>();
        public static List<ProfileShared> vDirectKeyboardEmojiListSmiley = new List<ProfileShared>();
        public static List<ProfileShared> vDirectKeyboardEmojiListSymbol = new List<ProfileShared>();
        public static List<ProfileShared> vDirectKeyboardEmojiListTravel = new List<ProfileShared>();
        public static ObservableCollection<ProfileShared> vDirectKeyboardTextList = new ObservableCollection<ProfileShared>();
        public static ObservableCollection<KeypadMapping> vDirectKeypadMapping = new ObservableCollection<KeypadMapping>();
        public static ObservableCollection<ControllerProfile> vDirectControllersProfile = new ObservableCollection<ControllerProfile>();
    }
}