using ArnoldVinkCode;
using System.Diagnostics;
using System.IO;

namespace LibraryShared
{
    public partial class Processes
    {
        //Launch a win32 application manually
        public static void ProcessLauncherWin32(string PathExe, string PathLaunch, string Argument)
        {
            try
            {
                //Check if the application exe file exists
                if (!File.Exists(PathExe))
                {
                    Debug.WriteLine("Launch executable not found.");
                    return;
                }

                //Show launching message
                Debug.WriteLine("Launching Win32: " + Path.GetFileNameWithoutExtension(PathExe));

                //Check the working path
                if (string.IsNullOrWhiteSpace(PathLaunch)) { PathLaunch = Path.GetDirectoryName(PathExe); }

                //Prepare the launching task
                void TaskAction()
                {
                    try
                    {
                        Process LaunchProcess = new Process();
                        LaunchProcess.StartInfo.FileName = PathExe;
                        LaunchProcess.StartInfo.WorkingDirectory = PathLaunch;
                        LaunchProcess.StartInfo.Arguments = Argument;
                        LaunchProcess.Start();
                    }
                    catch { }
                }

                //Launch the application
                AVActions.TaskStart(TaskAction, null);
            }
            catch
            {
                Debug.WriteLine("Failed launching Win32: " + Path.GetFileNameWithoutExtension(PathExe));
            }
        }

        private static void LaunchProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                Debug.WriteLine(e.Data);
            }
            catch { }
        }
    }
}