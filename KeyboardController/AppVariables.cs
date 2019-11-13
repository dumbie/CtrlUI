﻿using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Threading;

namespace KeyboardController
{
    partial class AppVariables
    {
        //Application Variables
        public static Configuration vConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        //Process Variables
        public static Process vProcessCurrent = Process.GetCurrentProcess();

        //Interaction Variables
        public static DispatcherTimer vDispatcherTimer = new DispatcherTimer();

        //Keyboard Variables
        public static bool vCapsEnabled = false;
        public static bool vKeysEnabled = true;

        //Mouse Variables
        public static int vMouseClicks = 0;
        public static bool vMouseHoldingLeft = false;

        //Sockets Variables
        public static ArnoldVinkSockets vArnoldVinkSockets = null;

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