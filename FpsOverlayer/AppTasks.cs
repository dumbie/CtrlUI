using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace FpsOverlayer
{
    public class AppTasks
    {
        public static AVTaskDetails vTask_UpdateStatsFps = new AVTaskDetails("vTask_UpdateStatsFps");
        public static AVTaskDetails vTask_MonitorHardware = new AVTaskDetails("vTask_MonitorHardware");
        public static AVTaskDetails vTask_MonitorProcess = new AVTaskDetails("vTask_MonitorProcess");
        public static AVTaskDetails vTask_MonitorTaskbar = new AVTaskDetails("vTask_MonitorTaskbar");

        //Stop all the background tasks
        public static async Task TasksBackgroundStop()
        {
            try
            {
                await TaskStopLoop(vTask_UpdateStatsFps, 5000);
                await TaskStopLoop(vTask_MonitorHardware, 5000);
                await TaskStopLoop(vTask_MonitorProcess, 5000);
                await TaskStopLoop(vTask_MonitorTaskbar, 5000);
            }
            catch { }
        }
    }
}