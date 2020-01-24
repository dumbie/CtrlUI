using AVForms;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Windows;
using static ArnoldVinkCode.ProcessWin32Functions;
using static LibraryShared.AppLaunchCheck;

namespace AdminLauncher
{
    public partial class App : Application
    {
        //Application Variables
        bool vAdministratorPermission = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        string SchTask_Author = string.Empty;
        string SchTask_Name = string.Empty;
        string SchTask_Description = string.Empty;
        string SchTask_FilePath = string.Empty;
        string SchTask_WorkingPath = string.Empty;

        //Application Startup
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                //Set the application details
                SchTask_Author = "Arnold Vink";
                SchTask_Name = "ArnoldVink_DirectXInput";
                SchTask_Description = "DirectXInput Administrator Helper";
                SchTask_FilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\DirectXInput.exe";
                SchTask_WorkingPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                //Check the application status
                Application_LaunchCheck(SchTask_Description, "DirectXInput-Admin", ProcessPriorityClass.Normal, false);

                //Check if the task already exists
                int ResultCheckTask = CheckTask();
                if (ResultCheckTask == 1)
                {
                    RunTask();
                }
                else if (ResultCheckTask == 2)
                {
                    await CheckAdmin();
                    CreateTask();
                    RunTask();
                }
                else if (ResultCheckTask == 3)
                {
                    await CheckAdmin();
                    CreateTask();
                    RunTask();
                }

                Debug.WriteLine("Admin launcher finished.");
                Environment.Exit(0);
                return;
            }
            catch { }
        }

        //Check admin priviliges
        async System.Threading.Tasks.Task CheckAdmin()
        {
            try
            {
                if (!vAdministratorPermission)
                {
                    int messageResult = await AVMessageBox.MessageBoxPopup(SchTask_Description, "It seems like this is the first time you are using the helper or the application path has changed so you will have to accept the upcoming administrator prompt, after that you will be able to run this helper without the administrator prompt.", "Continue", "Cancel", "", "");
                    if (messageResult == 1)
                    {
                        await ProcessLauncherWin32Async(Assembly.GetEntryAssembly().Location, Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "", true, false);
                        Environment.Exit(0);
                        return;
                    }
                    else
                    {
                        Debug.WriteLine("Admin launcher cancelled.");
                        Environment.Exit(0);
                        return;
                    }
                }
            }
            catch { }
        }

        //Run the admin schtask
        void RunTask()
        {
            try
            {
                Debug.WriteLine("Running the admin task.");
                using (TaskService taskService = new TaskService())
                {
                    using (Microsoft.Win32.TaskScheduler.Task task = taskService.GetTask(SchTask_Name))
                    {
                        task.Run();
                    }
                }
            }
            catch { }
        }

        //Check if schtask exists
        //0Unknown/1Exists/2DoesNotExist/3Invalid
        int CheckTask()
        {
            try
            {
                Debug.WriteLine("Checking the admin task.");
                using (TaskService taskService = new TaskService())
                {
                    using (Microsoft.Win32.TaskScheduler.Task task = taskService.GetTask(SchTask_Name))
                    {
                        if (task == null)
                        {
                            Debug.WriteLine("The task does not exist.");
                            return 2;
                        }
                        else
                        {
                            //Check if the application path has changed
                            if (!task.Definition.Actions.ToString().Contains(SchTask_FilePath))
                            {
                                Debug.WriteLine("Application path has changed.");
                                return 3;
                            }
                            else
                            {
                                Debug.WriteLine("The task should be working.");
                                return 1;
                            }
                        }
                    }
                }
            }
            catch { return 0; }
        }

        //Create admin run task
        void CreateTask()
        {
            try
            {
                Debug.WriteLine("Creating the admin task.");
                using (TaskService taskService = new TaskService())
                {
                    using (TaskDefinition taskDefinition = taskService.NewTask())
                    {
                        taskDefinition.RegistrationInfo.Description = SchTask_Description;
                        taskDefinition.RegistrationInfo.Author = SchTask_Author;
                        taskDefinition.Settings.RunOnlyIfIdle = false;
                        taskDefinition.Settings.AllowDemandStart = true;
                        taskDefinition.Settings.DisallowStartIfOnBatteries = false;
                        taskDefinition.Settings.StopIfGoingOnBatteries = false;
                        taskDefinition.Settings.ExecutionTimeLimit = new TimeSpan();
                        taskDefinition.Settings.IdleSettings.StopOnIdleEnd = false;
                        taskDefinition.Settings.AllowHardTerminate = false;
                        taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;
                        taskDefinition.Actions.Add(new ExecAction(SchTask_FilePath, null, SchTask_WorkingPath));
                        taskService.RootFolder.RegisterTaskDefinition(SchTask_Name, taskDefinition);
                    }
                }
            }
            catch { }
        }
    }
}