using ArnoldVinkCode;
using System;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static CtrlUI.AppVariables;
using static LibraryShared.Settings;

namespace CtrlUI
{
    public partial class WindowMain
    {
        public static AVTaskDetails vTask_UpdateClock = new AVTaskDetails();
        public static AVTaskDetails vTask_UpdateWindowStatus = new AVTaskDetails();
        public static AVTaskDetails vTask_ControllerConnected = new AVTaskDetails();
        public static AVTaskDetails vTask_UpdateProcesses = new AVTaskDetails();
        public static AVTaskDetails vTask_UpdateLaunchers = new AVTaskDetails();
        public static AVTaskDetails vTask_UpdateShortcuts = new AVTaskDetails();
        public static AVTaskDetails vTask_UpdateListStatus = new AVTaskDetails();
        public static AVTaskDetails vTask_UpdateAppRunningTime = new AVTaskDetails();
        public static AVTaskDetails vTask_UpdateMediaInformation = new AVTaskDetails();
        public static AVTaskDetails vTask_ShowHideMouse = new AVTaskDetails();

        //Start all the background tasks
        void TasksBackgroundStart()
        {
            try
            {
                AVActions.TaskStartLoop(vTaskLoop_UpdateClock, vTask_UpdateClock);
                AVActions.TaskStartLoop(vTaskLoop_UpdateWindowStatus, vTask_UpdateWindowStatus);
                AVActions.TaskStartLoop(vTaskLoop_ControllerConnected, vTask_ControllerConnected);
                AVActions.TaskStartLoop(vTaskLoop_UpdateProcesses, vTask_UpdateProcesses);
                AVActions.TaskStartLoop(vTaskLoop_UpdateLaunchers, vTask_UpdateLaunchers);
                AVActions.TaskStartLoop(vTaskLoop_UpdateShortcuts, vTask_UpdateShortcuts);
                AVActions.TaskStartLoop(vTaskLoop_UpdateListStatus, vTask_UpdateListStatus);
                AVActions.TaskStartLoop(vTaskLoop_UpdateAppRunningTime, vTask_UpdateAppRunningTime);
                AVActions.TaskStartLoop(vTaskLoop_UpdateMediaInformation, vTask_UpdateMediaInformation);
                TaskStart_ShowHideMouseCursor();
            }
            catch { }
        }

        //Stop all the background tasks
        public static async Task TasksBackgroundStop()
        {
            try
            {
                await AVActions.TaskStopLoop(vTask_UpdateClock);
                await AVActions.TaskStopLoop(vTask_UpdateWindowStatus);
                await AVActions.TaskStopLoop(vTask_ControllerConnected);
                await AVActions.TaskStopLoop(vTask_UpdateProcesses);
                await AVActions.TaskStopLoop(vTask_UpdateShortcuts);
                await AVActions.TaskStopLoop(vTask_UpdateListStatus);
                await AVActions.TaskStopLoop(vTask_UpdateAppRunningTime);
                await AVActions.TaskStopLoop(vTask_UpdateMediaInformation);
                await AVActions.TaskStopLoop(vTask_ShowHideMouse);
            }
            catch { }
        }

        void TaskStart_ShowHideMouseCursor()
        {
            try
            {
                if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "HideMouseCursor")))
                {
                    AVActions.TaskStartLoop(vTaskLoop_ShowHideMouse, vTask_ShowHideMouse);
                }
            }
            catch { }
        }
    }
}