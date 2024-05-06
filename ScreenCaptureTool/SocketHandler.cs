using ArnoldVinkCode;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;

namespace ScreenCapture
{
    partial class SocketServer
    {
        //Handle received pipe string
        public static void ReceivedPipesHandler(string receivedString)
        {
            try
            {
                async void TaskAction()
                {
                    try
                    {
                        Debug.WriteLine("Received pipe string: " + receivedString);
                        if (receivedString == "-videostop")
                        {
                            await AppClose.Application_Exit();
                        }
                    }
                    catch { }
                }
                AVActions.TaskStartBackground(TaskAction);
            }
            catch { }
        }

        //Handle received socket data
        public static void ReceivedSocketHandler(TcpClient tcpClient, UdpEndPointDetails endPoint, byte[] receivedBytes)
        {
            try
            {
                async Task TaskAction()
                {
                    try
                    {
                        if (tcpClient != null)
                        {
                            //await ReceivedTcpSocketHandlerThread(tcpClient, receivedBytes);
                        }
                        else
                        {
                            await ReceivedUdpSocketHandlerThread(endPoint, receivedBytes);
                        }
                    }
                    catch { }
                }
                AVActions.TaskStartBackground(TaskAction);
            }
            catch { }
        }

        public static async Task ReceivedUdpSocketHandlerThread(UdpEndPointDetails endPoint, byte[] receivedBytes)
        {
            try
            {
                //Get the source server ip and port
                Debug.WriteLine("Received udp socket from: " + endPoint.IPEndPoint.Address.ToString() + ":" + endPoint.IPEndPoint.Port + "/" + receivedBytes.Length + "bytes");

                //Deserialize the received bytes
                if (DeserializeBytesToObject(receivedBytes, out string deserializedBytes))
                {
                    Debug.WriteLine("Received socket string: " + deserializedBytes);
                    if (deserializedBytes == "CaptureImage")
                    {
                        await CaptureScreen.CaptureImageProcess(0);
                    }
                    else if (deserializedBytes == "CaptureVideo")
                    {
                        await CaptureScreen.CaptureVideoProcess(0);
                    }
                }
            }
            catch { }
        }
    }
}