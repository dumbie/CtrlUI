using ArnoldVinkCode;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.Styles.MainColors;
using static FpsOverlayer.AppVariables;
using static LibraryShared.Classes;

namespace FpsOverlayer
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
                        else if (receivedString == "SwitchFpsOverlayVisibility")
                        {
                            SwitchFpsOverlayVisibility();
                        }
                        else if (receivedString == "SwitchBrowserOverlayVisibility")
                        {
                            vWindowBrowser.Browser_Switch_Visibility();
                        }
                        else if (receivedString == "SwitchCrosshairOverlayVisibility")
                        {
                            SwitchCrosshairVisibility();
                        }
                        else if (receivedString == "ChangeFpsOverlayPosition")
                        {
                            ChangeFpsOverlayPosition();
                        }
                        else if (receivedString == "SettingChangedColorAccentLight")
                        {
                            vConfigurationCtrlUI = SettingLoadConfig("CtrlUI.exe.csettings");
                            string colorLightHex = SettingLoad(vConfigurationCtrlUI, "ColorAccentLight", typeof(string));
                            ChangeApplicationAccentColor(colorLightHex);
                        }
                        else if (receivedString == "SettingChangedDisplayMonitor")
                        {
                            vConfigurationCtrlUI = SettingLoadConfig("CtrlUI.exe.csettings");
                            UpdateWindowPosition();
                        }
                    }
                    else if (deserializedBytes.Object is KeypadSize)
                    {
                        KeypadSize receivedKeypadSize = (KeypadSize)deserializedBytes.Object;

                        //Set the window keypad margin
                        vKeypadAdjustMargin = receivedKeypadSize.Height;

                        //Update fps overlay position and visibility
                        UpdateFpsOverlayPositionVisibility(vTargetProcess.ExeNameNoExt);
                    }
                }
            }
            catch { }
        }
    }
}