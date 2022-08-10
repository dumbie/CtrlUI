using ArnoldVinkCode;
using LibreHardwareMonitor.Hardware;
using Microsoft.Diagnostics.Tracing.Session;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using static ArnoldVinkCode.AVTaskbarInformation;
using static ArnoldVinkCode.ProcessClasses;
using static LibraryShared.Classes;
using static LibraryShared.Settings;

namespace FpsOverlayer
{
    public class AppVariables
    {
        //Application Variables
        public static Configuration vConfigurationCtrlUI = Settings_Load_CtrlUI();
        public static Configuration vConfigurationFpsOverlayer = Settings_Load_FpsOverlayer();
        public static ProcessMulti vTargetProcess = new ProcessMulti();
        public static bool vManualHidden = false;

        //Process Variables
        public static Process vProcessCurrent = Process.GetCurrentProcess();

        //Margin Variables
        public static int vKeypadAdjustMargin = 0;
        public static int vTaskBarAdjustMargin = 0;
        public static AppBarPosition vTaskBarPosition = AppBarPosition.ABE_BOTTOM;

        //Frames per second
        public static long vLastFrameTimeAdded = 0;
        public static double vLastFrameTimeStamp = 0;
        public static List<double> vListFrameTime = new List<double>();

        //Strings
        public static string vTitleGPU = "GPU";
        public static string vTitleCPU = "CPU";
        public static string vTitleMEM = "MEM";
        public static string vTitleNET = "NET";
        public static string vTitleMON = "MON";
        public static string vTitleFPS = "FPS";
        public static string vTitleBAT = "BAT";

        //Keyboard Variables
        public static AVInputOutputHotKey vAVInputOutputHotKey = new AVInputOutputHotKey();

        //Hardware
        public static Computer vHardwareComputer = null;

        //Trace Events
        public static TraceEventSession vTraceEventSession;

        //Trace Events - Event IDs
        public const int vEventID_DxgKrnlPresent = 184;

        //Trace Events - Provider Guids
        public static Guid vProvider_DxgKrnl = Guid.Parse("{802ec45a-1e99-4b83-9920-87c98277ba9d}");

        //Sockets Variables
        public static ArnoldVinkSockets vArnoldVinkSockets = null;

        //Application Lists
        public static ObservableCollection<ProfileShared> vFpsPositionProcessName = new ObservableCollection<ProfileShared>();
    }
}