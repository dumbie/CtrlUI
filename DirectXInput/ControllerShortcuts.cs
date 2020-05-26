using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessWin32Functions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Check if controller shortcut is pressed
        async Task<bool> ControllerShortcut(ControllerStatus Controller)
        {
            bool ControllerUsed = false;
            bool ControllerDelayShort = false;
            bool ControllerDelayLong = false;
            try
            {
                if (Environment.TickCount >= Controller.Delay_ControllerShortcut)
                {
                    //Activate the controller
                    if (Controller.InputCurrent.ButtonGuide.PressedShort)
                    {
                        Debug.WriteLine("Shortcut activate controller has been pressed.");
                        bool controllerActivated = ControllerActivate(Controller);

                        ControllerUsed = true;
                        ControllerDelayShort = true;

                        if (controllerActivated) { return ControllerUsed; }
                    }

                    //Show CtrlUI application
                    if (Controller.InputCurrent.ButtonGuide.PressedShort && !App.vWindowKeyboard.vWindowVisible && vProcessCtrlUI != null)
                    {
                        Debug.WriteLine("Guide short press showing CtrlUI.");
                        await ShowCtrlUI();

                        ControllerUsed = true;
                        ControllerDelayLong = true;
                    }
                    //Launch CtrlUI application
                    else if (Controller.InputCurrent.ButtonGuide.PressedShort && !App.vWindowKeyboard.vWindowVisible && vProcessCtrlUI == null)
                    {
                        await LaunchCtrlUI();

                        ControllerUsed = true;
                        ControllerDelayLong = true;
                    }
                    //Hide the keyboard controller
                    else if (Controller.InputCurrent.ButtonGuide.PressedShort && App.vWindowKeyboard.vWindowVisible)
                    {
                        await KeyboardControllerHideShow(false);

                        ControllerUsed = true;
                        ControllerDelayLong = true;
                    }
                    //Show the keyboard controller
                    else if (Controller.InputCurrent.ButtonGuide.PressedLong && !App.vWindowKeyboard.vWindowVisible)
                    {
                        await KeyboardControllerHideShow(true);

                        ControllerUsed = true;
                        ControllerDelayLong = true;
                    }
                    //Press Alt+Enter
                    else if (Controller.InputCurrent.ButtonStart.PressedRaw && Controller.InputCurrent.ButtonShoulderRight.PressedRaw)
                    {
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutAltEnter"]))
                        {
                            Debug.WriteLine("Button Global - Alt+Enter");

                            NotificationDetails notificationDetails = new NotificationDetails();
                            notificationDetails.Icon = "AppMiniMaxi";
                            notificationDetails.Text = "Pressing Alt+Enter";
                            App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                            await KeyPressCombo((byte)KeysVirtual.Menu, (byte)KeysVirtual.Return, false);

                            ControllerUsed = true;
                            ControllerDelayLong = true;
                        }
                    }
                    //Press Alt+F4
                    else if (Controller.InputCurrent.ButtonStart.PressedRaw && Controller.InputCurrent.ButtonShoulderLeft.PressedRaw)
                    {
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutAltF4"]))
                        {
                            Debug.WriteLine("Button Global - Alt+F4");

                            NotificationDetails notificationDetails = new NotificationDetails();
                            notificationDetails.Icon = "AppClose";
                            notificationDetails.Text = "Pressing Alt+F4";
                            App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                            await KeyPressCombo((byte)KeysVirtual.Menu, (byte)KeysVirtual.F4, false);

                            ControllerUsed = true;
                            ControllerDelayLong = true;
                        }
                    }
                    //Press Alt+Tab or Win+Tab
                    else if (Controller.InputCurrent.ButtonBack.PressedRaw && Controller.InputCurrent.ButtonShoulderRight.PressedRaw)
                    {
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutWinTab"]))
                        {
                            Debug.WriteLine("Button Global - Win+Tab");

                            NotificationDetails notificationDetails = new NotificationDetails();
                            notificationDetails.Icon = "AppMiniMaxi";
                            notificationDetails.Text = "Pressing Win+Tab";
                            App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                            await KeyPressCombo((byte)KeysVirtual.LeftWindows, (byte)KeysVirtual.Tab, false);

                            ControllerUsed = true;
                            ControllerDelayLong = true;
                        }
                        else if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutAltTab"]))
                        {
                            Debug.WriteLine("Button Global - Alt+Tab");

                            NotificationDetails notificationDetails = new NotificationDetails();
                            notificationDetails.Icon = "AppMiniMaxi";
                            notificationDetails.Text = "Pressing Alt+Tab";
                            App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                            await KeyPressCombo((byte)KeysVirtual.Menu, (byte)KeysVirtual.Tab, false);

                            ControllerUsed = true;
                            ControllerDelayLong = true;
                        }
                    }
                    //Make screenshot
                    else if (Controller.InputCurrent.ButtonBack.PressedRaw && Controller.InputCurrent.ButtonShoulderLeft.PressedRaw)
                    {
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutScreenshot"]))
                        {
                            Debug.WriteLine("Button Global - Screenshot");
                            PlayInterfaceSound(vConfigurationCtrlUI, "Screenshot", true);
                            await KeyPressCombo((byte)KeysVirtual.LeftWindows, (byte)KeysVirtual.Snapshot, false);

                            ControllerUsed = true;
                            ControllerDelayLong = true;
                        }
                    }
                    //Disconnect controller from Bluetooth
                    else if (Controller.InputCurrent.ButtonStart.PressedRaw && Controller.InputCurrent.ButtonGuide.PressedRaw)
                    {
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutDisconnectBluetooth"]))
                        {
                            Debug.WriteLine("Shortcut disconnect Bluetooth has been pressed.");
                            StopControllerTask(Controller, false);

                            ControllerUsed = true;
                            ControllerDelayLong = true;
                        }
                    }

                    if (ControllerDelayShort)
                    {
                        Controller.Delay_ControllerShortcut = Environment.TickCount + vControllerDelayShortTicks;
                    }
                    else if (ControllerDelayLong)
                    {
                        Controller.Delay_ControllerShortcut = Environment.TickCount + vControllerDelayLongTicks;
                    }
                }
            }
            catch { }
            return ControllerUsed;
        }

        //Hide or show the keyboard controller
        async Task KeyboardControllerHideShow(bool forceShow)
        {
            try
            {
                Debug.WriteLine("Shortcut keyboard has been pressed.");
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    if (forceShow || !App.vWindowKeyboard.vWindowVisible)
                    {
                        await App.vWindowKeyboard.Show();
                    }
                    else
                    {
                        App.vWindowKeyboard.Hide();
                    }
                });
            }
            catch { }
        }

        //Launch CtrlUI when not running
        async Task LaunchCtrlUI()
        {
            try
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutLaunchCtrlUI"]))
                {
                    Debug.WriteLine("Shortcut launch CtrlUI has been pressed.");

                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "AppLaunch";
                    notificationDetails.Text = "Launching CtrlUI";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                    if (!CheckRunningProcessByNameOrTitle("CtrlUI", false))
                    {
                        await ProcessLauncherWin32Async("CtrlUI-Admin.exe", "", "", true, false);
                    }
                }
            }
            catch { }
        }

        //Show CtrlUI when not focused
        async Task ShowCtrlUI()
        {
            try
            {
                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vArnoldVinkSockets.vTcpListenerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vTcpListenerPort;
                socketSend.Object = "AppWindowHideShow";
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                TcpClient tcpClient = await vArnoldVinkSockets.TcpClientCheckCreateConnect(vArnoldVinkSockets.vTcpListenerIp, vArnoldVinkSockets.vTcpListenerPort - 1, vArnoldVinkSockets.vTcpClientTimeout);
                await vArnoldVinkSockets.TcpClientSendBytes(tcpClient, SerializedData, vArnoldVinkSockets.vTcpClientTimeout, false);
            }
            catch { }
        }
    }
}