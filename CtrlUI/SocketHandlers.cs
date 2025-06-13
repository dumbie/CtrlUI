using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle received socket data
        public void ReceivedSocketHandler(TcpClient tcpClient, UdpEndPointDetails endPoint, byte[] receivedBytes)
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

        async Task ReceivedUdpSocketHandlerThread(UdpEndPointDetails endPoint, byte[] receivedBytes)
        {
            try
            {
                //Get the source server ip and port
                //Debug.WriteLine("Received udp socket from: " + endPoint.IPEndPoint.Address.ToString() + ":" + endPoint.IPEndPoint.Port + "/" + receivedBytes.Length + "bytes");

                //Deserialize the received bytes
                if (DeserializeBytesToObject(receivedBytes, out SocketSendContainer deserializedBytes))
                {
                    Type objectType = Type.GetType(deserializedBytes.SendType);
                    if (objectType == typeof(ControllerInput))
                    {
                        if (!vControllerBusy)
                        {
                            vControllerBusy = true;

                            ControllerInput receivedControllerInput = deserializedBytes.GetObjectAsType<ControllerInput>();
                            await ControllerInteraction(receivedControllerInput);

                            vControllerBusy = false;
                        }
                    }
                    else if (objectType == typeof(List<ControllerStatusDetails>))
                    {
                        List<ControllerStatusDetails> controllerStatusSummaryList = deserializedBytes.GetObjectAsType<List<ControllerStatusDetails>>();
                        UpdateControllerStatus(controllerStatusSummaryList);
                    }
                    else if (objectType == typeof(string))
                    {
                        string receivedString = (string)deserializedBytes.SendObject;
                        Debug.WriteLine("Received socket string: " + receivedString);
                        if (receivedString == "AppWindowHideShow")
                        {
                            await AVActions.DispatcherInvoke(async delegate { await AppWindow_HideShow(); });
                        }
                    }
                }
            }
            catch { }
        }
    }
}