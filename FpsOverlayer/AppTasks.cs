using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace FpsOverlayer
{
    public class AppTasks
    {
        public static AVTaskDetails vTask_TraceEventOutput = new AVTaskDetails();
        public static AVTaskDetails vTask_MonitorHardware = new AVTaskDetails();
        public static AVTaskDetails vTask_MonitorProcess = new AVTaskDetails();
        public static AVTaskDetails vTask_MonitorTaskbar = new AVTaskDetails();

        //Stop all the background tasks
        public static async Task TasksBackgroundStop()
        {
            try
            {
                await TaskStopLoop(vTask_TraceEventOutput);
                await TaskStopLoop(vTask_MonitorHardware);
                await TaskStopLoop(vTask_MonitorProcess);
                await TaskStopLoop(vTask_MonitorTaskbar);
            }
            catch { }
        }
    }
}