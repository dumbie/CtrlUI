﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVAssembly;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVProcess;
using static LibraryShared.AppCheck;
using static LibraryShared.AppUpdate;

namespace CtrlUI
{
    public partial class App : Application
    {
        //Application Windows
        public static WindowMain vWindowMain = new WindowMain();

        //Application Startup
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                //Resolve missing assembly dll files
                AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveFile;

                //Application restart delay
                await Application_RestartDelay(e);

                //Application startup checks
                await StartupCheck("CtrlUI", ProcessPriority.High);

                //Application update checks
                await UpdateCheck();

                //Open the application window
                vWindowMain.Show();
            }
            catch { }
        }

        //Application restart delay
        private async Task Application_RestartDelay(StartupEventArgs e)
        {
            try
            {
                if (e.Args != null && e.Args.Contains("-restart"))
                {
                    //Get current process information
                    ProcessMulti currentProcess = Get_ProcessMultiCurrent();

                    //Check if application is already running
                    while (Get_ProcessesMultiByName(currentProcess.ExeNameNoExt, true).Count > 1)
                    {
                        await Task.Delay(500);
                    }
                }
            }
            catch { }
        }
    }
}