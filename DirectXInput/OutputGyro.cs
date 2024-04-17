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
                while (TaskCheckLoop(Controller.OutputGyroTask) && Controller.Connected())
                {
                    try
                    {
                        //Send gyro motion to the dsu client
                        await SendGyroMotionController(Controller);
                    }
                    catch { }
                    finally
                    {
                        //Delay task to prevent high cpu usage
                        AVHighResDelay.Delay(1);
                    }
                }
            }
            catch { }
        }
    }
}