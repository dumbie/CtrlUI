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
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessClasses;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.Settings;

namespace CtrlUI
{
    public class AppVariables
    {
        //Application Variables
        readonly public static bool vAdministratorPermission = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        public static CultureInfo vAppCultureInfo = new CultureInfo("en-US");
        public static Assembly vAppAssembly = Assembly.GetExecutingAssembly();
        public static Configuration vConfigurationCtrlUI = Settings_Load_CtrlUI();
        public static Configuration vConfigurationDirectXInput = Settings_Load_DirectXInput();

        //Api Variables
        public static string vApiIGDBClientID = "pf1397qtj00w9z55vmwbp7lzf557ja"; //Yes, I know I didn't remove the api key.
        public static string vApiIGDBAuthorization = "3ofmaqyzmxs2kz3yniey9kim2y253s"; //Yes, I know I didn't remove the api key.
        public static string vApiIGDBTokenCache = string.Empty;
        public static DateTime? vApiIGDBTokenExpire = null;

        //Interaction Variables
        public static long vMouseLastInteraction = GetSystemTicksMs();
        public static PointWin vMousePreviousPosition = new PointWin();
        public static bool vSingleTappedEvent = true;
        public static bool vMousePressDownLeftClick = false;
        public static bool vMousePressDownRightClick = false;
        public static bool vMousePressDownXButton1 = false;
        public static string[] vLoopTargetListsColumn = { "listbox_MainMenu", "lb_MessageBox" };
        public static string[] vSelectTargetLists = { "lb_Games", "lb_Apps", "lb_Emulators", "lb_Launchers", "lb_Shortcuts", "lb_Processes", "lb_FilePicker" };
        public static string[] vTabTargetListsSingleColumn = { "lb_Games", "lb_Apps", "lb_Emulators", "lb_Launchers", "lb_Shortcuts", "lb_Processes", "lb_Manage_AddAppCategory" };
        public static string[] vTabTargetListsFirstLastColumn = { "lb_Search" };
        public static string[] vTabTargetListsFirstLastItem = { "lb_FilePicker", "lb_ProfileManager" };
        public static string[] vTabTargetButtons = { "grid_Popup_TextInput_button_ConfirmText" };

        //Dispatcher Timers
        public static DispatcherTimer vDispatcherTimerOverlay = new DispatcherTimer();

        //Image Variables
        public static ImageSourceFolders[] vImageSourceFolders =
        {
            new ImageSourceFolders() { SourcePath = "Assets/User/Apps", SearchOption = SearchOption.AllDirectories },
            new ImageSourceFolders() { SourcePath = "Assets/Default/Apps", SearchOption = SearchOption.AllDirectories },
            new ImageSourceFolders() { SourcePath = "Assets/User/Games", SearchOption = SearchOption.AllDirectories },
            new ImageSourceFolders() { SourcePath = "Assets/Default/Games", SearchOption = SearchOption.AllDirectories }
        };
        public static string vImageBackupSource = "Assets/Default/Apps/Unknown.png";
        public static BitmapImage vImagePreloadSteam = FileToBitmapImage(new string[] { "Steam" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 100, 0);
        public static BitmapImage vImagePreloadUbisoft = FileToBitmapImage(new string[] { "Ubisoft" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 100, 0);
        public static BitmapImage vImagePreloadEADesktop = FileToBitmapImage(new string[] { "EA Desktop" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 100, 0);
        public static BitmapImage vImagePreloadGoG = FileToBitmapImage(new string[] { "GoG" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 100, 0);
        public static BitmapImage vImagePreloadBethesda = FileToBitmapImage(new string[] { "Bethesda" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 100, 0);
        public static BitmapImage vImagePreloadEpic = FileToBitmapImage(new string[] { "Epic" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 100, 0);
        public static BitmapImage vImagePreloadBattleNet = FileToBitmapImage(new string[] { "Battle.net" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 100, 0);
        public static BitmapImage vImagePreloadRockstar = FileToBitmapImage(new string[] { "Rockstar" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 100, 0);
        public static BitmapImage vImagePreloadDiscord = FileToBitmapImage(new string[] { "Discord" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 100, 0);
        public static BitmapImage vImagePreloadMicrosoft = FileToBitmapImage(new string[] { "Microsoft" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 100, 0);

        //Busy Variables
        public static bool vBusyCheckingForUpdate = false;
        public static bool vBusyRefreshingProcesses = false;
        public static bool vBusyRefreshingShortcuts = false;
        public static bool vBusyRefreshingLaunchers = false;

        //Process Variables
        public static Process vProcessCurrent = Process.GetCurrentProcess();
        public static Process vProcessDirectXInput = null;

        //App Status Variables
        public static ProcessMulti vPrevFocusedProcess = null;
        public static bool vChangingWindow = false;
        public static bool vAppMaximized = false;
        public static bool vAppMinimized = false;
        public static bool vAppActivated = true;

        //Popup Variables
        public static bool vPopupOpen = false;
        public static FrameworkElement vPopupElementTarget = null;
        public static FrameworkElementFocus vPopupElementFocus = new FrameworkElementFocus();

        //ColorPicker Variables
        public static bool vColorPickerOpen = false;
        public static FrameworkElementFocus vColorPickerElementFocus = new FrameworkElementFocus();

        //Search Variables
        public static bool vSearchOpen = false;
        public static FrameworkElementFocus vSearchElementFocus = new FrameworkElementFocus();

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

        //MessageBox Variables
        public static bool vMessageBoxOpen = false;
        public static bool vMessageBoxCancelled = false;
        public static DataBindString vMessageBoxResult = null;
        public static FrameworkElementFocus vMessageBoxElementFocus = new FrameworkElementFocus();

        //File Picker Variables
        public static bool vFilePickerOpen = false;
        public static bool vFilePickerCancelled = false;
        public static bool vFilePickerCompleted = false;
        public static DataBindFile vFilePickerResult = null;
        public static FrameworkElementFocus vFilePickerElementFocus = new FrameworkElementFocus();
        public static List<int> vFilePickerNavigateIndexes = new List<int>();
        public static string vFilePickerCurrentPath = string.Empty;
        public static string vFilePickerPreviousPath = string.Empty;
        public static List<string> vFilePickerFilterIn = new List<string>();
        public static List<string> vFilePickerFilterOut = new List<string>();
        public static string vFilePickerTitle = "File Browser";
        public static string vFilePickerDescription = "Please select a file, folder or disk:";
        public static bool vFilePickerShowNoFile = false;
        public static bool vFilePickerShowRoms = false;
        public static bool vFilePickerShowFiles = true;
        public static bool vFilePickerShowDirectories = true;
        public static SortingType vFilePickerSortType = SortingType.Name;

        //Profile Manager Variables
        public static string vProfileManagerName = "CtrlIgnoreProcessName";
        public static ObservableCollection<ProfileShared> vProfileManagerListShared = null;

        //Clipboard Variables
        public static List<DataBindFile> vClipboardFiles = new List<DataBindFile>();

        //Manage Variables
        public static DataBindApp vEditAppDataBind = null;
        public static DataBindApp vMoveAppDataBind = null;
        public static AppCategory vEditAppCategoryPrevious = AppCategory.App;

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
        public static int vControllerDelayNanoTicks = 10;
        public static int vControllerDelayMicroTicks = 75;
        public static int vControllerDelayShortTicks = 130;
        public static int vControllerDelayMediumTicks = 250;
        public static int vControllerDelayLongTicks = 500;
        public static int vControllerDelayLongerTicks = 750;
        public static long vControllerDelay_DPad = GetSystemTicksMs();
        public static long vControllerDelay_Stick = GetSystemTicksMs();
        public static long vControllerDelay_Trigger = GetSystemTicksMs();
        public static long vControllerDelay_Button = GetSystemTicksMs();
        public static long vControllerDelay_Activate = GetSystemTicksMs();

        //Sockets Variables
        public static ArnoldVinkSockets vArnoldVinkSockets = null;

        //Json Lists
        public static List<ApiIGDBGenres> vApiIGDBGenres = new List<ApiIGDBGenres>();
        public static List<ApiIGDBPlatforms> vApiIGDBPlatforms = new List<ApiIGDBPlatforms>();

        //Application Lists
        public static ObservableCollection<ProfileShared> vCtrlIgnoreProcessName = new ObservableCollection<ProfileShared>();
        public static ObservableCollection<ProfileShared> vCtrlIgnoreLauncherName = new ObservableCollection<ProfileShared>();
        public static ObservableCollection<ProfileShared> vCtrlIgnoreShortcutName = new ObservableCollection<ProfileShared>();
        public static ObservableCollection<ProfileShared> vCtrlIgnoreShortcutUri = new ObservableCollection<ProfileShared>();
        public static ObservableCollection<ProfileShared> vCtrlKeyboardExtensionName = new ObservableCollection<ProfileShared>();
        public static ObservableCollection<ProfileShared> vCtrlKeyboardProcessName = new ObservableCollection<ProfileShared>();
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