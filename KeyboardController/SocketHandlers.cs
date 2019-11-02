using ArnoldVinkCode;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSocketClass;
using static KeyboardController.AppVariables;
using static LibraryShared.ClassConverters;
using static LibraryShared.Classes;

namespace KeyboardController
{
    partial class WindowMain
    {
        //Handle received socket data
        public async Task ReceivedSocketHandler(byte[] receivedBytes)
        {
            try
            {
                async void TaskAction()
                {
                    try
                    {
                        await ReceivedSocketHandlerThread(receivedBytes);
                    }
                    catch { }
                }
                await AVActions.TaskStart(TaskAction, null);
            }
            catch { }
        }

        async Task ReceivedSocketHandlerThread(byte[] receivedBytes)
        {
            try
            {
                //Deserialize the received bytes
                SocketSendContainer DeserializedBytes = DeserializeBytesToClass<SocketSendContainer>(receivedBytes);

                //Get the source server ip and port
                //Debug.WriteLine("Received socket from: " + DeserializedBytes.SourceIp + ":" + DeserializedBytes.SourcePort);

                //Check what kind of object was received
                if (DeserializedBytes.Object is ControllerInput)
                {
                    if (!vControllerBusy)
                    {
                        vControllerBusy = true;

                        ControllerInput receivedControllerInput = (ControllerInput)DeserializedBytes.Object;
                        ControllerInteractionMouse(receivedControllerInput);
                        await ControllerInteractionKeyboard(receivedControllerInput);

                        vControllerBusy = false;
                    }
                }
            }
            catch { }
        }
    }
}