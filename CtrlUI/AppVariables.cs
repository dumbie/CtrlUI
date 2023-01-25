using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVSearch;
using static ArnoldVinkCode.AVSettings;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    public class AppVariables
    {
        //Application Variables
        readonly public static bool vAdministratorPermission = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        public static CultureInfo vAppCultureInfo = CultureInfo.InvariantCulture;
        public static Assembly vAppAssembly = Assembly.GetExecutingAssembly();
        public static Configuration vConfigurationCtrlUI = SettingLoadConfig("CtrlUI.exe.csettings");
        public static Configuration vConfigurationDirectXInput = SettingLoadConfig("DirectXInput.exe.csettings");

        //Api Variables
        public static string vApiIGDBClientID = "pf1397qtj00w9z55vmwbp7lzf557ja"; //Yes, I know I didn't remove the api key.
        public static string vApiIGDBAuthorization = "3ofmaqyzmxs2kz3yniey9kim2y253s"; //Yes, I know I didn't remove the api key.
        public static string vApiIGDBTokenCache = string.Empty;
        public static DateTime? vApiIGDBTokenExpire = null;

        //Interaction Variables
        public static bool vSingleTappedEvent = true;
        public static bool vMousePressDownLeftClick = false;
        public static bool vMousePressDownRightClick = false;
        public static bool vMousePressDownXButton1 = false;
        public static string[] vSelectNearCharacterLists = { "lb_Games", "lb_Apps", "lb_Emulators", "lb_Launchers", "lb_Shortcuts", "lb_Processes", "lb_Search", "lb_FilePicker" };
        public static string[] vLoopTargetListsFirstLastColumn = { "lb_Games", "lb_Apps", "lb_Emulators", "lb_Launchers", "lb_Shortcuts", "lb_Processes" };
        public static string[] vLoopTargetListsFirstLastItem = { "listbox_MainMenu", "lb_MessageBox" };
        public static string[] vTabTargetListsSingle = { "lb_Manage_AddAppCategory", "lb_Manage_AddEmulatorCategory" };
        public static string[] vTabTargetListsFirstLastColumn = { "lb_Search" };
        public static string[] vTabTargetListsFirstLastItem = { "lb_FilePicker", "lb_ProfileManager" };
        public static string[] vTabTargetButtonsDown = { "btn_Monitor_Switch_Primary", "grid_Popup_TextInput_button_ConfirmText", "btn_Manage_SaveEditApp" };
        public static string[] vTabTargetButtonsUp = { };

        //Dispatcher Timers
        public static DispatcherTimer vDispatcherTimerOverlay = new DispatcherTimer();

        //Search Variables
        public static SearchSource[] vImageSourceFolders =
        {
            new SearchSource() { SearchPath = "Assets/User/Apps", SearchPatterns = new [] {"*.png", "*.jpg"}, SearchOption = SearchOption.AllDirectories },
            new SearchSource() { SearchPath = "Assets/Default/Apps", SearchPatterns = new [] {"*.png", "*.jpg"}, SearchOption = SearchOption.AllDirectories },
            new SearchSource() { SearchPath = "Assets/User/Games", SearchPatterns = new [] {"*.png", "*.jpg"}, SearchOption = SearchOption.AllDirectories },
            new SearchSource() { SearchPath = "Assets/Default/Games", SearchPatterns = new [] {"*.png", "*.jpg"}, SearchOption = SearchOption.AllDirectories },
            new SearchSource() { SearchPath = "Assets/User/Emulators", SearchPatterns = new [] {"*.png", "*.jpg"}, SearchOption = SearchOption.AllDirectories },
            new SearchSource() { SearchPath = "Assets/Default/Emulators", SearchPatterns = new [] {"*.png", "*.jpg"}, SearchOption = SearchOption.AllDirectories }
        };
        public static SearchSource[] vImageSourceFoldersUser =
        {
            new SearchSource() { SearchPath = "Assets/User/Apps", SearchPatterns = new [] {"*.png", "*.jpg"}, SearchOption = SearchOption.AllDirectories },
            new SearchSource() { SearchPath = "Assets/User/Games", SearchPatterns = new [] {"*.png", "*.jpg"}, SearchOption = SearchOption.AllDirectories },
            new SearchSource() { SearchPath = "Assets/User/Emulators", SearchPatterns = new [] {"*.png", "*.jpg"}, SearchOption = SearchOption.AllDirectories },
        };

        //Image Variables
        public static int vImageLoadSize = 150;
        public static string vImageBackupSource = "Assets/Default/Apps/Unknown.png";
        public static BitmapImage vImagePreloadSteam = FileToBitmapImage(new string[] { "Steam" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadUbisoft = FileToBitmapImage(new string[] { "Ubisoft" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadEADesktop = FileToBitmapImage(new string[] { "EA Desktop" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadGoG = FileToBitmapImage(new string[] { "GoG" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadEpic = FileToBitmapImage(new string[] { "Epic" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadBattleNet = FileToBitmapImage(new string[] { "Battle.net" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadRockstar = FileToBitmapImage(new string[] { "Rockstar" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadDiscord = FileToBitmapImage(new string[] { "Discord" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadMicrosoft = FileToBitmapImage(new string[] { "Microsoft" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadAmazon = FileToBitmapImage(new string[] { "Amazon" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadConsole = FileToBitmapImage(new string[] { "Assets/Default/Icons/Console.png" }, null, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadHandheld = FileToBitmapImage(new string[] { "Assets/Default/Icons/Handheld.png" }, null, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadComputer = FileToBitmapImage(new string[] { "Assets/Default/Icons/Computer.png" }, null, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadPong = FileToBitmapImage(new string[] { "Assets/Default/Icons/Pong.png" }, null, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadVirtualReality = FileToBitmapImage(new string[] { "Assets/Default/Icons/VirtualReality.png" }, null, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadOperatingSystem = FileToBitmapImage(new string[] { "Assets/Default/Icons/OperatingSystem.png" }, null, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadArcade = FileToBitmapImage(new string[] { "Assets/Default/Icons/Arcade.png" }, null, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadPinball = FileToBitmapImage(new string[] { "Assets/Default/Icons/Pinball.png" }, null, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadChess = FileToBitmapImage(new string[] { "Assets/Default/Icons/Chess.png" }, null, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadApp = FileToBitmapImage(new string[] { "Assets/Default/Icons/App.png" }, null, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadGame = FileToBitmapImage(new string[] { "Assets/Default/Icons/Game.png" }, null, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadEmulator = FileToBitmapImage(new string[] { "Assets/Default/Icons/Emulator.png" }, null, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadProcess = FileToBitmapImage(new string[] { "Assets/Default/Icons/Process.png" }, null, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadShortcut = FileToBitmapImage(new string[] { "Assets/Default/Icons/Shortcut.png" }, null, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadUnknownApp = FileToBitmapImage(new string[] { "Assets/Default/Apps/Unknown.png" }, null, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadHelp = FileToBitmapImage(new string[] { "Assets/Default/Icons/Help.png" }, null, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);

        //Busy Variables
        public static bool vBusyChangingWindow = false;
        public static bool vBusyCheckingForUpdate = false;
        public static bool vBusyRefreshingProcesses = false;
        public static bool vBusyRefreshingShortcuts = false;
        public static bool vBusyRefreshingLaunchers = false;

        //Process Variables
        public static Process vProcessCurrent = Process.GetCurrentProcess();
        public static Process vProcessDirectXInput = null;

        //App Status Variables
        public static bool vAppMinimized = false;
        public static bool vAppActivated = true;

        //Popup Variables
        public static bool vPopupOpen = false;
        public static FrameworkElement vPopupElementTarget = null;
        public static FrameworkElementFocus vPopupElementFocus = new FrameworkElementFocus();

        //ColorPicker Variables
        public static bool vColorPickerOpen = false;
        public static FrameworkElementFocus vColorPickerElementFocus = new FrameworkElementFocus();

        //Text Input Variables
        public static bool vTextInputOpen = false;
        public static bool vTextInputCancelled = false;
        public static string vTextInputResult = string.Empty;
        public static FrameworkElementFocus vTextInputElementFocus = new FrameworkElementFocus();

        //MainMenu Variables
        public static bool vMainMenuOpen = false;
        public static FrameworkElementFocus vMainMenuElementFocus = new FrameworkElementFocus();

        //Sort Variables
        public static SortingType vSortType = SortingType.Number;

        //HowLongToBeat Variables
        public static bool vHowLongToBeatOpen = false;
        public static FrameworkElementFocus vHowLongToBeatElementFocus = new FrameworkElementFocus();

        //MessageBox Variables
        public static bool vMessageBoxOpen = false;
        public static bool vMessageBoxCancelled = false;
        public static DataBindString vMessageBoxResult = null;
        public static FrameworkElementFocus vMessageBoxElementFocus = new FrameworkElementFocus();

        //File Picker Variables
        public static bool vFilePickerOpen = false;
        public static bool vFilePickerCancelled = false;
        public static bool vFilePickerCompleted = false;
        public static bool vFilePickerLoadBusy = false;
        public static bool vFilePickerLoadCancel = false;
        public static bool vFilePickerFolderSelectMode = false;
        public static DataBindFile vFilePickerResult = null;
        public static FilePickerSettings vFilePickerSettings = new FilePickerSettings();
        public static FrameworkElementFocus vFilePickerElementFocus = new FrameworkElementFocus();
        public static List<PickerNavigation> vFilePickerNavigationHistory = new List<PickerNavigation>();
        public static SortingType vFilePickerSortingType = SortingType.Name;
        public static string vFilePickerSourcePath = string.Empty;
        public static string vFilePickerCurrentPath = string.Empty;
        public static string vFilePickerPreviousPath = string.Empty;

        //Profile Manager Variables
        public static string vProfileManagerName = "CtrlIgnoreProcessName";
        public static ObservableCollection<ProfileShared> vProfileManagerListShared = null;

        //Clipboard Variables
        public static List<DataBindFile> vClipboardFiles = new List<DataBindFile>();

        //Manage Variables
        public static DataBindApp vEditAppDataBind = null;
        public static DataBindApp vMoveAppDataBind = null;
        public static AppCategory vEditAppDataBindCategory = AppCategory.App;

        //Controller Variables
        public static int vControllerActiveId = 0;
        public static ControllerStatusDetails vController0 = new ControllerStatusDetails(0);
        public static ControllerStatusDetails vController1 = new ControllerStatusDetails(1);
        public static ControllerStatusDetails vController2 = new ControllerStatusDetails(2);
        public static ControllerStatusDetails vController3 = new ControllerStatusDetails(3);
        public static bool vControllerAnyConnected()
        {
            return vController0.Connected || vController1.Connected || vController2.Connected || vController3.Connected;
        }

        public static bool vControllerBusy = false;
        public static int vControllerDelayTicks10 = 10;
        public static int vControllerDelayTicks125 = 125;
        public static int vControllerDelayTicks250 = 250;
        public static int vControllerDelayTicks500 = 500;
        public static int vControllerDelayTicks750 = 750;
        public static int vControllerDelayTicks1000 = 1000;
        public static long vControllerDelay_DPad = 0;
        public static long vControllerDelay_Stick = 0;
        public static long vControllerDelay_Trigger = 0;
        public static long vControllerDelay_Button = 0;
        public static long vControllerDelay_Activate = 0;

        //Sockets Variables
        public static ArnoldVinkSockets vArnoldVinkSockets = null;

        //Json Lists
        public static List<ApiIGDBGenres> vApiIGDBGenres = new List<ApiIGDBGenres>();
        public static List<ApiIGDBPlatforms> vApiIGDBPlatforms = new List<ApiIGDBPlatforms>();

        //Load Status
        public static bool vListLoadedApplications = false;
        public static bool vListLoadedLaunchers = false;
        public static bool vListLoadedShortcuts = false;
        public static bool vListLoadedProcesses = false;
        public static bool vAppsLoaded()
        {
            return vListLoadedApplications && vListLoadedLaunchers && vListLoadedShortcuts && vListLoadedProcesses;
        }

        //Application Lists
        public static ObservableCollection<ProfileShared> vCtrlHDRProcessName = new ObservableCollection<ProfileShared>();
        public static ObservableCollection<ProfileShared> vCtrlIgnoreProcessName = new ObservableCollection<ProfileShared>();
        public static ObservableCollection<ProfileShared> vCtrlIgnoreLauncherName = new ObservableCollection<ProfileShared>();
        public static ObservableCollection<ProfileShared> vCtrlIgnoreShortcutName = new ObservableCollection<ProfileShared>();
        public static ObservableCollection<ProfileShared> vCtrlIgnoreShortcutUri = new ObservableCollection<ProfileShared>();
        public static ObservableCollection<ProfileShared> vCtrlKeyboardExtensionName = new ObservableCollection<ProfileShared>();
        public static ObservableCollection<ProfileShared> vCtrlKeyboardProcessName = new ObservableCollection<ProfileShared>();
        public static ObservableCollection<ProfileShared> vCtrlChromiumBrowsers = new ObservableCollection<ProfileShared>();
        public static ObservableCollection<ProfileShared> vCtrlCloseLaunchers = new ObservableCollection<ProfileShared>();
        public static ObservableCollection<ProfileShared> vCtrlLocationsFile = new ObservableCollection<ProfileShared>();
        public static ObservableCollection<ProfileShared> vCtrlLocationsShortcut = new ObservableCollection<ProfileShared>();
        public static ObservableCollection<DataBindApp> List_Games = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<DataBindApp> List_Launchers = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<DataBindApp> List_Apps = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<DataBindApp> List_Emulators = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<DataBindApp> List_Shortcuts = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<DataBindApp> List_Processes = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<DataBindApp> List_Search = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<DataBindFile> List_FilePicker = new ObservableCollection<DataBindFile>();
        public static ObservableCollection<SolidColorBrush> List_ColorPicker = new ObservableCollection<SolidColorBrush>();
        public static ObservableCollection<DataBindString> List_MainMenu = new ObservableCollection<DataBindString>();
        public static List<string> vLauncherAppAvailableCheck = new List<string>();
    }
}