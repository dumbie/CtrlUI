using System;
using System.Diagnostics;

namespace DriverInstaller
{
    class AppExit
    {
        public static void Exit()
        {
            try
            {
                Debug.WriteLine("Exiting application.");

                //Exit application
                Environment.Exit(0);
            }
            catch { }
        }
    }
}