using ArnoldVinkCode;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace LibraryShared
{
    public partial class Processes
    {
        //Launch a win32 application manually
        public static void ProcessLauncherWin32(string PathExe, string PathLaunch, string Argument, bool RunAsAdmin, bool CreateNoWindow)
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

                        if (CreateNoWindow)
                        {
                            LaunchProcess.StartInfo.UseShellExecute = false;
                            LaunchProcess.StartInfo.CreateNoWindow = true;
                        }

                        if (RunAsAdmin)
                        {
                            LaunchProcess.StartInfo.Verb = "runas";
                        }

                        LaunchProcess.Start();
                    }
                    catch
                    {
                        Debug.WriteLine("Failed launching Win32: " + Path.GetFileNameWithoutExtension(PathExe));
                    }
                }

                //Launch the application
                AVActions.TaskStart(TaskAction, null);
            }
            catch
            {
                Debug.WriteLine("Failed launching Win32: " + Path.GetFileNameWithoutExtension(PathExe));
            }
        }

        //Launch a win32 application manually async
        public static async Task<int> ProcessLauncherWin32Async(string PathExe, string PathLaunch, string Argument, bool RunAsAdmin, bool CreateNoWindow)
        {
            try
            {
                //Check if the application exe file exists
                if (!File.Exists(PathExe))
                {
                    Debug.WriteLine("Launch executable not found.");
                    return -1;
                }

                //Show launching message
                Debug.WriteLine("Launching Win32: " + Path.GetFileNameWithoutExtension(PathExe));

                //Check the working path
                if (string.IsNullOrWhiteSpace(PathLaunch)) { PathLaunch = Path.GetDirectoryName(PathExe); }

                //Prepare the launching task
                int TaskAction()
                {
                    try
                    {
                        Process LaunchProcess = new Process();
                        LaunchProcess.StartInfo.FileName = PathExe;
                        LaunchProcess.StartInfo.WorkingDirectory = PathLaunch;
                        LaunchProcess.StartInfo.Arguments = Argument;

                        if (CreateNoWindow)
                        {
                            LaunchProcess.StartInfo.UseShellExecute = false;
                            LaunchProcess.StartInfo.CreateNoWindow = true;
                        }

                        if (RunAsAdmin)
                        {
                            LaunchProcess.StartInfo.Verb = "runas";
                        }

                        LaunchProcess.Start();
                        return LaunchProcess.Id;
                    }
                    catch
                    {
                        Debug.WriteLine("Failed launching Win32: " + Path.GetFileNameWithoutExtension(PathExe));
                        return -1;
                    }
                }

                //Launch the application
                return await AVActions.TaskStartReturn(TaskAction, null);
            }
            catch
            {
                Debug.WriteLine("Failed launching Win32: " + Path.GetFileNameWithoutExtension(PathExe));
                return -1;
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