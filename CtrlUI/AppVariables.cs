using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessClasses;
using static LibraryShared.Classes;

namespace CtrlUI
{
    public class AppVariables
    {
        //Application Variables
        public static int vAppCurrentMonitor = 0;
        public static CultureInfo vAppCultureInfo = new CultureInfo("en-US");
        public static Configuration vConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        public static bool vAdministratorPermission = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        public static string[] vAppsBlacklistShortcut = new string[] { };
        public static string[] vAppsBlacklistShortcutUri = new string[] { };
        public static string[] vAppsOtherLaunchers = new string[] { };
        public static List<FileLocation> vFileLocations = new List<FileLocation>();
        public static string[] vAppsBlacklistProcess = new string[] { };
        public static string[] vAppsShortcutProtocol = new string[] { };

        //Interaction Variables
        public static int vMouseLastInteraction = Environment.TickCount;
        public static PointWin vMousePreviousPosition = new PointWin();
        public static bool vSingleTappedEvent = true;
        public static bool vMousePressDownLeftClick = false;
        public static bool vMousePressDownRightClick = false;
        public static bool vMousePressDownXButton1 = false;
        public static DispatcherTimer vDispatcherTimer = new DispatcherTimer();
        public static string[] vListTabTarget = { "lb_Games", "lb_Apps", "lb_Emulators", "lb_Shortcuts", "lb_Processes" };

        //Update Variables
        public static bool vCheckingForUpdate = false;

        //Busy Variables
        public static bool vBusyRefreshingApps = false;

        //Process Variables
        public static Process vProcessCurrent = Process.GetCurrentProcess();
        public static Process vProcessDirectXInput = null;
        public static Process vProcessKeyboardController = null;

        //App Status Variables
        public static ProcessFocus vPrevFocusedProcess = null;
        public static WindowState vAppPrevWindowState = WindowState.Normal;
        public static bool vChangingWindow = false;
        public static bool vAppMaximized = false;
        public static bool vAppMinimized = false;
        public static bool vAppActivated = true;

        //Popup Variables
        public static bool vPopupOpen = false;
        public static FrameworkElement vPopupTargetElement = null;
        public static FrameworkElement vPopupPreviousFocus = null;

        //ColorPicker Variables
        public static bool vColorPickerOpen = false;
        public static FrameworkElement vColorPickerPreviousFocus = null;

        //Search Variables
        public static bool vSearchOpen = false;
        public static FrameworkElement vSearchPreviousFocus = null;

        //MainMenu Variables
        public static bool vMainMenuOpen = false;
        public static FrameworkElement vMainMenuPreviousFocus = null;

        //Sort Variables
        public static string vSortType = "Number"; //Number/Name

        //MessageBox Variables
        public static bool vMessageBoxOpen = false;
        public static bool vMessageBoxCancelled = false;
        public static DataBindString vMessageBoxResult = null;
        public static FrameworkElement vMessageBoxPreviousFocus = null;

        //File Picker Variables
        public static bool vFilePickerOpen = false;
        public static bool vFilePickerCancelled = false;
        public static bool vFilePickerCompleted = false;
        public static DataBindFile vFilePickerResult = null;
        public static FrameworkElement vFilePickerPreviousFocus = null;
        public static List<int> vFilePickerNavigateIndexes = new List<int>();
        public static string vFilePickerCurrentPath = string.Empty;
        public static string vFilePickerPreviousPath = string.Empty;
        public static string[] vFilePickerFilterIn = new string[] { };
        public static string[] vFilePickerFilterOut = new string[] { };
        public static string[][] vFilePickerStrings = new string[][] { };
        public static string vFilePickerTitle = "File Browser";
        public static string vFilePickerDescription = "Please select a file, folder or disk:";
        public static bool vFilePickerShowNoFile = false;
        public static bool vFilePickerShowRoms = false;
        public static bool vFilePickerShowFiles = true;
        public static bool vFilePickerShowDirectories = true;
        public static bool vFilePickerSortByName = true;

        //Manage Variables
        public static ListBox vEditAppListBox = null;
        public static DataBindApp vEditAppDataBind = null;
        public static string vEditAppCategoryPrevious = string.Empty;

        //Controller Variables
        public static string vControllerActiveId = "0";
        public static ControllerStatusSend vController0 = new ControllerStatusSend();
        public static ControllerStatusSend vController1 = new ControllerStatusSend();
        public static ControllerStatusSend vController2 = new ControllerStatusSend();
        public static ControllerStatusSend vController3 = new ControllerStatusSend();
        public static bool vControllerAnyConnected()
        {
            return vController0.Connected || vController1.Connected || vController2.Connected || vController3.Connected;
        }

        public static bool vControllerBusy = false;
        public static int vControllerDelayPollingTicks = 25;
        public static int vControllerDelayMicroTicks = 75;
        public static int vControllerDelayShortTicks = 150;
        public static int vControllerDelayMediumTicks = 300;
        public static int vControllerDelayLongTicks = 750;
        public static int vControllerDelay_DPad = Environment.TickCount;
        public static int vControllerDelay_Stick = Environment.TickCount;
        public static int vControllerDelay_Trigger = Environment.TickCount;
        public static int vControllerDelay_Button = Environment.TickCount;
        public static int vControllerDelay_Activate = Environment.TickCount;
        public static int vControllerDelay_Global = Environment.TickCount;

        //Sockets Variables
        public static ArnoldVinkSockets vArnoldVinkSockets = null;

        //Application Lists
        public static ObservableCollection<DataBindApp> List_Games = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<DataBindApp> List_Apps = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<DataBindApp> List_Emulators = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<DataBindApp> List_Shortcuts = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<DataBindApp> List_Processes = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<DataBindApp> List_Search = new ObservableCollection<DataBindApp>();
        public static ObservableCollection<SolidColorBrush> List_ColorPicker = new ObservableCollection<SolidColorBrush>();
        public static ObservableCollection<DataBindFile> List_FilePicker = new ObservableCollection<DataBindFile>();
    }
}