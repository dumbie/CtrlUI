using ArnoldVinkCode;
using ArnoldVinkStyles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Security.Principal;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkCode.AVJsonFunctions;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkCode.AVSearch;
using static ArnoldVinkStyles.AVFocus;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    public class AppVariables
    {
        //Application Windows
        public static App vApp = null;
        public static AVWindow vWindowMain = null;

        //Application Variables
        readonly public static bool vAdministratorPermission = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        public static CultureInfo vAppCultureInfo = CultureInfo.InvariantCulture;
        public static AVSettingsConfig vSettings = new AVSettingsConfig("CtrlUI.exe.csettings");

        //Api Variables
        public static string vApiHltbAuthKey = string.Empty;
        public static DateTime? vApiHltbAuthDateTime = null;
        public static string vApiHltbSearchName = string.Empty;
        public static string vApiIGDBClientID = "pf1397qtj00w9z55vmwbp7lzf557ja"; //Yes, I know I didn't remove the api key.
        public static string vApiIGDBAuthorization = "3ofmaqyzmxs2kz3yniey9kim2y253s"; //Yes, I know I didn't remove the api key.
        public static string vApiIGDBTokenCache = string.Empty;
        public static DateTime? vApiIGDBTokenExpire = null;

        //Interaction Variables
        public static bool vMousePressDownLeft = false;
        public static bool vMousePressDownRight = false;
        public static bool vMousePressDownMiddle = false;
        public static bool vMousePressDownXButton1 = false;
        public static bool vMousePressDownXButton2 = false;
        public static string[] vSelectNearCharacterLists = { "listView_Games", "listView_Apps", "listView_Emulators", "listView_Launchers", "listView_Shortcuts", "listView_Processes", "listView_Gallery", "listView_Search", "listView_FilePicker" };
        public static string[] vTabTargetListsSingleColumn = { "listView_Manage_AddAppCategory", "listView_Manage_AddEmulatorCategory" };
        public static string[] vTabTargetListsFirstLastColumn = { };
        public static string[] vTabTargetListsFirstLastItem = { "listView_FilePicker", "listView_ProfileManager", "listView_Sorting" };

        //Timers
        public static AVHighResTimer vAVTimerOverlayCharacter = new AVHighResTimer();
        public static AVHighResTimer vAVTimerOverlayNotification = new AVHighResTimer();
        public static AVHighResTimer vAVTimerDelayGallery = new AVHighResTimer();

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
        };
        public static SearchSource[] vImageSourceFoldersAppsUser =
        {
            new SearchSource() { SearchPath = "Assets/User/Apps", SearchPatterns = new [] {"*.png", "*.jpg"}, SearchOption = SearchOption.AllDirectories },
        };

        //Image Variables
        public static int vImageLoadSizeGallery = 384;
        public static int vImageLoadSizeFilePicker = 128;
        public static int vImageLoadSizeApplication = 192;
        public static string vImageBackupSource = "Assets/Default/Icons/Unknown.png";

        //Update Variables
        public static long vLastUpdateGallery = 0;
        public static long vLastUpdateShortcuts = 0;
        public static long vLastUpdateLaunchers = 0;

        //Busy Variables
        public static bool vBusyRefreshingProcesses = false;
        public static bool vBusyRefreshingGallery = false;
        public static bool vBusyRefreshingShortcuts = false;
        public static bool vBusyRefreshingLaunchers = false;
        public static int vBusyRefreshingCount()
        {
            int refreshCount = 0;
            try
            {
                if (vBusyRefreshingGallery) { refreshCount++; }
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

        //Sorting Variables
        public static bool vSortingOpen = false;
        public static AVFocusDetails vSortingElementFocus = new AVFocusDetails();

        //HowLongToBeat Variables
        public static bool vHowLongToBeatOpen = false;
        public static AVFocusDetails vHowLongToBeatElementFocus = new AVFocusDetails();

        //Information Variables
        public static bool vContentInformationOpen = false;
        public static AVFocusDetails vContentInformationElementFocus = new AVFocusDetails();
        public static object vContentInformationDataBind = null;
        public static byte[] vContentInformationImageBytes = [];

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
        public static string vFilePickerSourcePath = string.Empty;
        public static string vFilePickerCurrentPath = string.Empty;
        public static string vFilePickerPreviousPath = string.Empty;

        //Profile Manager Variables
        public static string vProfileManagerName = string.Empty;
        public static ObservableCollection<ProfileShared> vProfileManagerListShared = null;

        //Clipboard Variables
        public static List<DataBindFile> vClipboardFiles = new List<DataBindFile>();

        //Manage Variables
        public static AppCategory vEditAppDataBindCategory = AppCategory.App;
        public static DataBindApp vEditAppDataBind = null;
        public static DataBindApp vMoveAppDataBind = null;
        public static ListView vMoveAppListView = null;

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
        public static bool vListLoadedGallery = false;
        public static bool vAppsLoaded()
        {
            return vListLoadedApplications && vListLoadedLaunchers && vListLoadedShortcuts && vListLoadedProcesses && vListLoadedGallery;
        }

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
        public static ObservableCollection<ProfileShared> vCtrlLocationsGallery = JsonLoadFile<ObservableCollection<ProfileShared>>(@"Profiles\User\CtrlLocationsGallery.json");
        public static ObservableCollection<DataBindApp> List_Games = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<DataBindApp> List_Launchers = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<DataBindApp> List_Apps = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<DataBindApp> List_Emulators = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<DataBindApp> List_Shortcuts = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<DataBindApp> List_Processes = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<DataBindApp> List_Gallery = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<DataBindApp> List_Search = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<DataBindFile> List_FilePicker = new ObservableCollection<DataBindFile>();
        public static ObservableCollection<SolidColorBrush> List_ColorPicker = new ObservableCollection<SolidColorBrush>();
        public static ObservableCollection<DataBindString> List_MainMenu = new ObservableCollection<DataBindString>();
        public static List<string> vLauncherAppAvailableCheck = new List<string>();
    }
}