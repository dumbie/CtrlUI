using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVProcess;

namespace LibraryShared
{
    public partial class AppCheck
    {
        public static async Task StartupCheck(string appName, ProcessPriority priorityLevel)
        {
            try
            {
                Debug.WriteLine("Checking application status.");

                //Get current process information
                ProcessMulti currentProcess = Get_ProcessMultiCurrent();
                List<ProcessMulti> activeProcesses = Get_ProcessesMultiByName(currentProcess.ExeNameNoExt, true);

                //Check if application is already running
                if (activeProcesses.Count > 1)
                {
                    Debug.WriteLine("Application " + appName + " is already running, closing the process");
                    Environment.Exit(0);
                    return;
                }

                //Set the working directory to executable directory
                AVFunctions.ApplicationUpdateWorkingPath();

                //Set the application priority level
                try
                {
                    currentProcess.Priority = priorityLevel;
                }
                catch { }

                //Check - Windows version
                if (AVFunctions.DevOsVersion() < 10)
                {
                    List<string> messageAnswers = new List<string>();
                    messageAnswers.Add("Ok");
                    await new AVMessageBox().Popup(null, "Windows 10 required", appName + " only supports Windows 10 or newer.", messageAnswers);

                    //Close the application
                    Environment.Exit(0);
                    return;
                }

                //Check for missing user folders
                AVFiles.Directory_Create(@"Assets\User\Apps", false);
                AVFiles.Directory_Create(@"Assets\User\Emulators", false);
                AVFiles.Directory_Create(@"Assets\User\Clocks", false);
                AVFiles.Directory_Create(@"Assets\User\Fonts", false);
                AVFiles.Directory_Create(@"Assets\User\Sounds", false);
            }
            catch { }
        }
    }
}