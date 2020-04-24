using LibreHardwareMonitor.Hardware;
using Microsoft.Diagnostics.Tracing.Session;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using static ArnoldVinkCode.ProcessClasses;
using static LibraryShared.Classes;

namespace FpsOverlayer
{
    public class AppVariables
    {
        //Application Variables
        public static Configuration vConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        public static Configuration vConfigurationCtrlUI = null;
        public static ProcessMulti vTargetProcess = new ProcessMulti();
        public static bool vManualHidden = false;

        //Process Variables
        public static Process vProcessCurrent = Process.GetCurrentProcess();

        //Frames per second
        public static int vLastFrameTimeAdded = 0;
        public static double vLastFrameTimeStamp = 0;
        public static List<double> vListFrameTime = new List<double>();

        //Strings
        public static string vTitleGPU = "GPU";
        public static string vTitleCPU = "CPU";
        public static string vTitleMEM = "MEM";
        public static string vTitleNET = "NET";

        //Hardware
        public static Computer vHardwareComputer = null;

        //Trace Events
        public static TraceEventSession vTraceEventSession;

        //Trace Events - Event IDs
        public const int vEventID_DxgKrnlPresent = 184;

        //Trace Events - Provider Guids
        public static Guid vProvider_DxgKrnl = Guid.Parse("{802ec45a-1e99-4b83-9920-87c98277ba9d}");

        //Application Lists
        public static ObservableCollection<ProfileShared> vFpsPositionProcessName = new ObservableCollection<ProfileShared>();
    }
}