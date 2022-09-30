﻿using ArnoldVinkCode;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Media;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static FpsOverlayer.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Settings;

namespace FpsOverlayer
{
    partial class WindowMain
    {
        //Handle received socket data
        public async Task ReceivedSocketHandler(TcpClient tcpClient, UdpEndPointDetails endPoint, byte[] receivedBytes)
        {
            try
            {
                async void TaskAction()
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
                await AVActions.TaskStart(TaskAction);
            }
            catch { }
        }

        async Task ReceivedUdpSocketHandlerThread(UdpEndPointDetails endPoint, byte[] receivedBytes)
        {
            try
            {
                //Get the source server ip and port
                //Debug.WriteLine("Received udp socket from: " + endPoint.IPEndPoint.Address.ToString() + ":" + endPoint.IPEndPoint.Port);

                //Deserialize the received bytes
                if (DeserializeBytesToObject(receivedBytes, out SocketSendContainer deserializedBytes))
                {
                    if (deserializedBytes.Object is string)
                    {
                        string receivedString = (string)deserializedBytes.Object;
                        Debug.WriteLine("Received socket string: " + receivedString);
                        if (receivedString == "ApplicationExit")
                        {
                            await Application_Exit();
                        }
                        else if (receivedString == "SettingChangedColorAccentLight")
                        {
                            vConfigurationCtrlUI = Settings_Load_CtrlUI();
                            Settings_Load_AccentColor(vConfigurationCtrlUI);
                        }
                        else if (receivedString == "SettingChangedDisplayMonitor")
                        {
                            vConfigurationCtrlUI = Settings_Load_CtrlUI();
                            UpdateWindowPosition();
                        }
                    }
                    else if (deserializedBytes.Object is KeypadSize)
                    {
                        KeypadSize receivedKeypadSize = (KeypadSize)deserializedBytes.Object;

                        //Set the window keypad margin
                        vKeypadAdjustMargin = receivedKeypadSize.Height;

                        //Update the fps overlay position
                        UpdateFpsOverlayPosition(vTargetProcess.Name);
                    }
                }
            }
            catch { }
        }
    }
}