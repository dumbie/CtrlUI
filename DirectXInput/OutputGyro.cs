using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Send gyro
        async Task LoopOutputGyro(ControllerStatus Controller)
        {
            try
            {
                Debug.WriteLine("Send gyro for: " + Controller.Details.DisplayName);

                //Send gyro to dsu client
                while (!Controller.OutputGyroTask.TaskStopRequest && Controller.Connected())
                {
                    try
                    {
                        //Delay task to prevent high cpu usage
                        TaskDelayMs(1);

                        //Send gyro motion to the dsu client
                        await SendGyroMotion(Controller);
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}