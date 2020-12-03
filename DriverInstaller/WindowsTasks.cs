using Microsoft.Win32.TaskScheduler;
using System;
using System.Diagnostics;

namespace DriverInstaller
{
    public partial class WindowMain
    {
        //Remove service task
        void RemoveServiceTask(string taskName)
        {
            try
            {
                using (TaskService taskService = new TaskService())
                {
                    taskService.RootFolder.DeleteTask(taskName);
                    Debug.WriteLine("Removed service task: " + taskName);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to remove service task: " + ex.Message);
            }
        }
    }
}