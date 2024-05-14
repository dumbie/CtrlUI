using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVProcess;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Close single process
        async Task<bool> CloseSingleProcessAuto(ProcessMulti processMulti, DataBindApp dataBindApp, bool resetProcess, bool removeProcess)
        {
            try
            {
                await Notification_Send_Status("AppClose", "Closing " + dataBindApp.Name);
                Debug.WriteLine("Closing process: " + dataBindApp.Name);

                //Close the process
                bool closedProcess = false;
                if (processMulti.Identifier > 0)
                {
                    closedProcess = AVProcess.Close_ProcessTreeByProcessId(processMulti.Identifier);
                }
                else if (!string.IsNullOrWhiteSpace(processMulti.AppUserModelId))
                {
                    closedProcess = AVProcess.Close_ProcessesByAppUserModelId(processMulti.AppUserModelId);
                }
                else if (!string.IsNullOrWhiteSpace(processMulti.ExeName))
                {
                    closedProcess = AVProcess.Close_ProcessesByName(processMulti.ExeName, true);
                }
                else if (!string.IsNullOrWhiteSpace(processMulti.ExePath))
                {
                    closedProcess = AVProcess.Close_ProcessesByExecutablePath(processMulti.ExePath);
                }

                //Check if process closed
                if (closedProcess)
                {
                    await Notification_Send_Status("AppClose", "Closed " + dataBindApp.Name);
                    Debug.WriteLine("Closed process: " + dataBindApp.Name);

                    //Reset the process running status
                    if (resetProcess)
                    {
                        dataBindApp.ResetStatus(false);
                    }

                    //Remove the process from the list
                    if (removeProcess)
                    {
                        await RemoveAppFromList(dataBindApp, false, false, true);
                    }

                    return true;
                }
                else
                {
                    await Notification_Send_Status("AppClose", "Failed to close application");
                    Debug.WriteLine("Failed to close the application.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to close the application: " + ex.Message);
                return false;
            }
        }

        //Close all processes
        async Task<bool> CloseAllProcessesAuto(DataBindApp dataBindApp, bool resetProcess, bool removeProcess)
        {
            try
            {
                await Notification_Send_Status("AppClose", "Closing all " + dataBindApp.Name);
                Debug.WriteLine("Closing all processes: " + dataBindApp.Name);

                //Close the processes
                bool closedProcess = false;
                foreach (ProcessMulti processMulti in dataBindApp.ProcessMulti)
                {
                    try
                    {
                        if (processMulti.Identifier > 0)
                        {
                            closedProcess = AVProcess.Close_ProcessTreeByProcessId(processMulti.Identifier);
                        }
                        else if (!string.IsNullOrWhiteSpace(processMulti.AppUserModelId))
                        {
                            closedProcess = AVProcess.Close_ProcessesByAppUserModelId(processMulti.AppUserModelId);
                        }
                        else if (!string.IsNullOrWhiteSpace(processMulti.ExeName))
                        {
                            closedProcess = AVProcess.Close_ProcessesByName(processMulti.ExeName, true);
                        }
                        else if (!string.IsNullOrWhiteSpace(processMulti.ExePath))
                        {
                            closedProcess = AVProcess.Close_ProcessesByExecutablePath(processMulti.ExePath);
                        }
                    }
                    catch { }
                }

                //Check if process closed
                if (closedProcess)
                {
                    await Notification_Send_Status("AppClose", "Closed all " + dataBindApp.Name);
                    Debug.WriteLine("Closed all processes: " + dataBindApp.Name);

                    //Reset the process running status
                    if (resetProcess)
                    {
                        dataBindApp.ResetStatus(false);
                    }

                    //Remove the process from the list
                    if (removeProcess)
                    {
                        await RemoveAppFromList(dataBindApp, false, false, true);
                    }

                    return true;
                }
                else
                {
                    await Notification_Send_Status("AppClose", "Failed to close application");
                    Debug.WriteLine("Failed to close the application.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to close the application: " + ex.Message);
                return false;
            }
        }

        //Close other running launchers
        async Task CloseLaunchers()
        {
            try
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString AnswerCloseLaunchers = new DataBindString();
                AnswerCloseLaunchers.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppClose.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerCloseLaunchers.Name = "Close launchers";
                Answers.Add(AnswerCloseLaunchers);

                DataBindString messageResult = await Popup_Show_MessageBox("Do you want to close other running launchers?", "You can edit the launchers that are closing in the profile manager.", "This includes launchers like Steam, EA Desktop, Ubisoft, GoG, Battle.net and Epic.", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerCloseLaunchers)
                    {
                        await Notification_Send_Status("AppClose", "Closing other launchers");

                        //Close all known other launchers
                        foreach (ProfileShared closeLauncher in vCtrlCloseLaunchers)
                        {
                            try
                            {
                                AVProcess.Close_ProcessesByName(closeLauncher.String1, true);
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }
        }

        //Close remote streamers
        async Task CloseStreamers()
        {
            try
            {
                //Ask if the user really wants to disconnect remote streams
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString AnswerDisconnectStreams = new DataBindString();
                AnswerDisconnectStreams.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Stream.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerDisconnectStreams.Name = "Disconnect streams";
                Answers.Add(AnswerDisconnectStreams);

                DataBindString messageResult = await Popup_Show_MessageBox("Do you want to disconnect remote streams?", "", "This includes streams from GeForce Experience, Parsec and Steam In-Home Streaming.", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerDisconnectStreams)
                    {
                        await Notification_Send_Status("Stream", "Disconnecting remote streams");

                        //Disconnect Steam Streaming
                        AVProcess.Close_ProcessesByName("steam.exe", true);

                        //Disconnect GeForce Experience
                        AVProcess.Close_ProcessesByName("nvstreamer.exe", true);

                        //Disconnect Parsec Streaming
                        KeyPressReleaseCombo(KeysVirtual.CtrlLeft, KeysVirtual.F3);

                        //Disconnect Remote Desktop
                        //LaunchProcess(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\tsdiscon.exe", "", "", "");
                    }
                }
            }
            catch { }
        }

        //Close Fps Overlayer
        async Task CloseFpsOverlayer()
        {
            try
            {
                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                Debug.WriteLine("Hiding Fps Overlayer");

                //Show notification
                await Notification_Send_Status("Fps", "Hiding Fps Overlayer");

                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vArnoldVinkSockets.vSocketServerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vSocketServerPort;
                socketSend.Object = "ApplicationExit";
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(vArnoldVinkSockets.vSocketServerIp), vArnoldVinkSockets.vSocketServerPort + 2);
                await vArnoldVinkSockets.UdpClientSendBytesServer(ipEndPoint, SerializedData, vArnoldVinkSockets.vSocketTimeout);
            }
            catch { }
        }
    }
}