using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Security.Principal;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static ArnoldVinkCode.AVFocus;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVJsonFunctions;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkCode.AVSearch;
using static ArnoldVinkCode.AVSettings;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    public class AppVariables
    {
        //Application Windows
        public static WindowMain vWindowMain = new WindowMain();

        //Application Variables
        readonly public static bool vAdministratorPermission = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        public static CultureInfo vAppCultureInfo = CultureInfo.InvariantCulture;
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
        public static string[] vTabTargetListsSingleColumn = { "lb_Manage_AddAppCategory", "lb_Manage_AddEmulatorCategory" };
        public static string[] vTabTargetListsFirstLastColumn = { };
        public static string[] vTabTargetListsFirstLastItem = { };

        //Dispatcher Timers
        public static DispatcherTimer vDispatcherTimerOverlay = new DispatcherTimer();

        //Search Variables
        public static SearchSource[] vImageSourceFoldersEmulatorsCombined =
        {
            new SearchSource() { SearchPath = "Assets/User/Emulators", SearchPatterns = new [] {"*.png", "*.jpg"}, SearchOption = SearchOption.AllDirectories },
            new SearchSource() { SearchPath = "Assets/Default/Emulators", SearchPatterns = new [] {"*.png", "*.jpg"}, SearchOption = SearchOption.AllDirectories }
        };
        public static SearchSource[] vImageSourceFoldersEmulatorsUser =
        {
            new SearchSource() { SearchPath = "Assets/User/Emulators", SearchPatterns = new [] {"*.png", "*.jpg"}, SearchOption = SearchOption.AllDirectories }
        };
        public static SearchSource[] vImageSourceFoldersAppsCombined =
        {
            new SearchSource() { SearchPath = "Assets/User/Apps", SearchPatterns = new [] {"*.png", "*.jpg"}, SearchOption = SearchOption.AllDirectories },
            new SearchSource() { SearchPath = "Assets/Default/Apps", SearchPatterns = new [] {"*.png", "*.jpg"}, SearchOption = SearchOption.AllDirectories },
            new SearchSource() { SearchPath = "Assets/User/Games", SearchPatterns = new [] {"*.png", "*.jpg"}, SearchOption = SearchOption.AllDirectories },
            new SearchSource() { SearchPath = "Assets/Default/Games", SearchPatterns = new [] {"*.png", "*.jpg"}, SearchOption = SearchOption.AllDirectories }
        };
        public static SearchSource[] vImageSourceFoldersAppsUser =
        {
            new SearchSource() { SearchPath = "Assets/User/Apps", SearchPatterns = new [] {"*.png", "*.jpg"}, SearchOption = SearchOption.AllDirectories },
            new SearchSource() { SearchPath = "Assets/User/Games", SearchPatterns = new [] {"*.png", "*.jpg"}, SearchOption = SearchOption.AllDirectories }
        };

        //Image Variables
        public static int vImageLoadSize = 180;
        public static string vImageBackupSource = "Assets/Default/Apps/Unknown.png";
        public static BitmapImage vImagePreloadSteam = FileToBitmapImage(new string[] { "Steam" }, vImageSourceFoldersAppsCombined, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadUbisoft = FileToBitmapImage(new string[] { "Ubisoft" }, vImageSourceFoldersAppsCombined, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadEADesktop = FileToBitmapImage(new string[] { "EA Desktop" }, vImageSourceFoldersAppsCombined, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadGoG = FileToBitmapImage(new string[] { "GoG" }, vImageSourceFoldersAppsCombined, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadEpic = FileToBitmapImage(new string[] { "Epic" }, vImageSourceFoldersAppsCombined, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadBattleNet = FileToBitmapImage(new string[] { "Battle.net" }, vImageSourceFoldersAppsCombined, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadRockstar = FileToBitmapImage(new string[] { "Rockstar" }, vImageSourceFoldersAppsCombined, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadDiscord = FileToBitmapImage(new string[] { "Discord" }, vImageSourceFoldersAppsCombined, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadMicrosoft = FileToBitmapImage(new string[] { "Microsoft" }, vImageSourceFoldersAppsCombined, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadAmazon = FileToBitmapImage(new string[] { "Amazon" }, vImageSourceFoldersAppsCombined, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadIndieGala = FileToBitmapImage(new string[] { "IndieGala" }, vImageSourceFoldersAppsCombined, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
        public static BitmapImage vImagePreloadItchIO = FileToBitmapImage(new string[] { "ItchIO" }, vImageSourceFoldersAppsCombined, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
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
        public static bool vBusyCheckingForUpdate = false;
        public static bool vBusyRefreshingProcesses = false;
        public static bool vBusyRefreshingShortcuts = false;
        public static bool vBusyRefreshingLaunchers = false;
        public static int vBusyRefreshingCount()
        {
            int refreshCount = 0;
            try
            {
                if (vBusyRefreshingShortcuts) { refreshCount++; }
                if (vBusyRefreshingLaunchers) { refreshCount++; }
            }
            catch { }
            return refreshCount;
        }

        //Process Variables
        public static ProcessMulti vProcessCurrent = Get_ProcessMultiCurrent();
        public static ProcessMulti vProcessDirectXInput = null;

        //App Status Variables
        public static bool vAppMinimized = false;
        public static bool vAppActivated = true;

        //Popup Variables
        public static bool vPopupOpen = false;
        public static FrameworkElement vPopupElementTarget = null;
        public static AVFocusDetails vPopupElementFocus = new AVFocusDetails();

        //ColorPicker Variables
        public static bool vColorPickerOpen = false;
        public static AVFocusDetails vColorPickerElementFocus = new AVFocusDetails();

        //Text Input Variables
        public static bool vTextInputOpen = false;
        public static bool vTextInputCancelled = false;
        public static string vTextInputResult = string.Empty;
        public static AVFocusDetails vTextInputElementFocus = new AVFocusDetails();

        //MainMenu Variables
        public static bool vMainMenuOpen = false;
        public static AVFocusDetails vMainMenuElementFocus = new AVFocusDetails();

        //Category Variables
        public static ListCategory vCurrentListCategory = ListCategory.App;

        //Sort Variables
        public static SortingType vSortType = SortingType.Number;

        //HowLongToBeat Variables
        public static bool vHowLongToBeatOpen = false;
        public static AVFocusDetails vHowLongToBeatElementFocus = new AVFocusDetails();

        //MessageBox Variables
        public static bool vMessageBoxOpen = false;
        public static bool vMessageBoxCancelled = false;
        public static DataBindString vMessageBoxResult = null;
        public static AVFocusDetails vMessageBoxElementFocus = new AVFocusDetails();

        //File Picker Variables
        public static bool vFilePickerOpen = false;
        public static bool vFilePickerCancelled = false;
        public static bool vFilePickerCompleted = false;
        public static bool vFilePickerLoadBusy = false;
        public static bool vFilePickerLoadCancel = false;
        public static bool vFilePickerFolderSelectMode = false;
        public static DataBindFile vFilePickerResult = null;
        public static FilePickerSettings vFilePickerSettings = new FilePickerSettings();
        public static AVFocusDetails vFilePickerElementFocus = new AVFocusDetails();
        public static List<PickerNavigation> vFilePickerNavigationHistory = new List<PickerNavigation>();
        public static SortingType vFilePickerSortingType = SortingType.Name;
        public static string vFilePickerSourcePath = string.Empty;
        public static string vFilePickerCurrentPath = string.Empty;
        public static string vFilePickerPreviousPath = string.Empty;

        //Profile Manager Variables
        public static string vProfileManagerName = "CtrlLocationsShortcut";
        public static ObservableCollection<ProfileShared> vProfileManagerListShared = null;

        //Clipboard Variables
        public static List<DataBindFile> vClipboardFiles = new List<DataBindFile>();

        //Manage Variables
        public static DataBindApp vEditAppDataBind = null;
        public static DataBindApp vMoveAppDataBind = null;
        public static AppCategory vEditAppDataBindCategory = AppCategory.App;

        //Controller Variables
        public static bool vControllerBusy = false;
        public static int vControllerActiveId = 0;
        public static ControllerStatusDetails vController0 = new ControllerStatusDetails(0);
        public static ControllerStatusDetails vController1 = new ControllerStatusDetails(1);
        public static ControllerStatusDetails vController2 = new ControllerStatusDetails(2);
        public static ControllerStatusDetails vController3 = new ControllerStatusDetails(3);
        public static bool vControllerAnyConnected()
        {
            return vController0.Connected || vController1.Connected || vController2.Connected || vController3.Connected;
        }

        //Sockets Variables
        public static ArnoldVinkSockets vArnoldVinkSockets = null;

        //Load Status
        public static bool vListLoadedApplications = false;
        public static bool vListLoadedLaunchers = false;
        public static bool vListLoadedShortcuts = false;
        public static bool vListLoadedProcesses = false;
        public static bool vAppsLoaded()
        {
            return vListLoadedApplications && vListLoadedLaunchers && vListLoadedShortcuts && vListLoadedProcesses;
        }

        //Json Lists
        public static List<ApiIGDBGenres> vApiIGDBGenres = JsonLoadFile<List<ApiIGDBGenres>>(@"Resources\ApiIGDB\Genres.json");
        public static List<ApiIGDBPlatforms> vApiIGDBPlatforms = JsonLoadFile<List<ApiIGDBPlatforms>>(@"Resources\ApiIGDB\Platforms.json");

        //Application Lists
        public static ObservableCollection<ProfileShared> vCtrlIgnoreProcessName = JsonLoadFile<ObservableCollection<ProfileShared>>(@"Profiles\Default\CtrlIgnoreProcessName.json");
        public static ObservableCollection<ProfileShared> vCtrlIgnoreLauncherName = JsonLoadFile<ObservableCollection<ProfileShared>>(@"Profiles\User\CtrlIgnoreLauncherName.json");
        public static ObservableCollection<ProfileShared> vCtrlIgnoreShortcutName = JsonLoadFile<ObservableCollection<ProfileShared>>(@"Profiles\User\CtrlIgnoreShortcutName.json");
        public static ObservableCollection<ProfileShared> vCtrlKeyboardExtensionName = JsonLoadFile<ObservableCollection<ProfileShared>>(@"Profiles\User\CtrlKeyboardExtensionName.json");
        public static ObservableCollection<ProfileShared> vCtrlKeyboardProcessName = JsonLoadFile<ObservableCollection<ProfileShared>>(@"Profiles\User\CtrlKeyboardProcessName.json");
        public static ObservableCollection<ProfileShared> vCtrlChromiumBrowsers = JsonLoadFile<ObservableCollection<ProfileShared>>(@"Profiles\Default\CtrlChromiumBrowsers.json");
        public static ObservableCollection<ProfileShared> vCtrlCloseLaunchers = JsonLoadFile<ObservableCollection<ProfileShared>>(@"Profiles\Default\CtrlCloseLaunchers.json");
        public static ObservableCollection<ProfileShared> vCtrlLocationsFile = JsonLoadFile<ObservableCollection<ProfileShared>>(@"Profiles\User\CtrlLocationsFile.json");
        public static ObservableCollection<ProfileShared> vCtrlLocationsShortcut = JsonLoadFile<ObservableCollection<ProfileShared>>(@"Profiles\User\CtrlLocationsShortcut.json");
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