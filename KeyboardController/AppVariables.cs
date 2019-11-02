using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Threading;

namespace KeyboardController
{
    partial class AppVariables
    {
        //Variables
        public static Configuration vConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        public static int vCurrentProcessId = Process.GetCurrentProcess().Id;

        //Interaction Variables
        public static DispatcherTimer vDispatcherTimer = new DispatcherTimer();

        //Keyboard Variables
        public static bool vCapsEnabled = false;
        public static bool vKeysEnabled = true;

        //Mouse Variables
        public static int vMouseClicks = 0;
        public static bool vMouseHoldingLeft = false;

        //Socket Variables
        public static ArnoldVinkSocketServer vSocketServer = new ArnoldVinkSocketServer();

        //Controller Variables
        public static bool vControllerBusy = false;
        public static int vControllerDelayPollingTicks = 25;
        public static int vControllerDelayMicroTicks = 75;
        public static int vControllerDelayShortTicks = 150;
        public static int vControllerDelayMediumTicks = 300;
        public static int vControllerDelayLongTicks = 750;
        public static int vControllerDelay_Keyboard = Environment.TickCount;
        public static int vControllerDelay_Mouse = Environment.TickCount;
    }
}