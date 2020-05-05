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
        //Update the button press times
        void UpdateButtonPressTimes(ControllerButtonDetails buttonDetails)
        {
            try
            {
                if (buttonDetails.PressedRaw)
                {
                    if (buttonDetails.PressTimeCurrent == 0)
                    {
                        //Debug.WriteLine("Starting button hold.");
                        buttonDetails.PressTimeCurrent = Environment.TickCount;
                    }
                }
                else
                {
                    if (buttonDetails.PressTimeDone)
                    {
                        buttonDetails.PressTimePrevious = 0;
                        //Debug.WriteLine("Releasing button hold: 0");
                    }
                    else if (buttonDetails.PressTimeCurrent > 0)
                    {
                        buttonDetails.PressTimePrevious = Environment.TickCount - buttonDetails.PressTimeCurrent;
                        //Debug.WriteLine("Releasing button hold: " + buttonDetails.PressTimePrevious);
                    }

                    buttonDetails.PressTimeDone = false;
                    buttonDetails.PressTimeCurrent = 0;
                }
            }
            catch { }
        }

        //Check the button press times
        void CheckButtonPressTimes(ControllerButtonDetails buttonDetails)
        {
            try
            {
                buttonDetails.PressedShort = false;
                buttonDetails.PressedLong = false;

                if (!buttonDetails.PressTimeDone)
                {
                    if (buttonDetails.PressTimePrevious > 0)
                    {
                        if (AVFunctions.BetweenNumbers(buttonDetails.PressTimePrevious, 1, 500, true))
                        {
                            buttonDetails.PressedShort = true;
                            buttonDetails.PressTimeDone = true;
                            return;
                        }
                    }

                    if (buttonDetails.PressTimeCurrent > 0)
                    {
                        int GuideCurrent = Environment.TickCount - buttonDetails.PressTimeCurrent;
                        //Debug.WriteLine("Button status: " + GuideCurrent);
                        if (GuideCurrent > 850)
                        {
                            buttonDetails.PressedLong = true;
                            buttonDetails.PressTimeDone = true;
                            return;
                        }
                    }
                }
            }
            catch { }
        }

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
                    //Show CtrlUI application
                    if (Controller.InputCurrent.ButtonGuide.PressedShort && vProcessKeyboardController == null && vProcessCtrlUI != null)
                    {
                        Debug.WriteLine("Guide short press showing CtrlUI.");
                        await ShowCtrlUI();

                        ControllerUsed = true;
                        ControllerDelayLong = true;
                    }
                    //Launch CtrlUI application
                    else if (Controller.InputCurrent.ButtonGuide.PressedShort && vProcessKeyboardController == null && vProcessCtrlUI == null)
                    {
                        await LaunchCtrlUI();

                        ControllerUsed = true;
                        ControllerDelayLong = true;
                    }
                    //Close the keyboard controller
                    else if ((Controller.InputCurrent.ButtonGuide.PressedShort || Controller.InputCurrent.ButtonGuide.PressedLong) && vProcessKeyboardController != null)
                    {
                        Debug.WriteLine("Guide press closing keyboard controller.");
                        await CloseKeyboardController();

                        ControllerUsed = true;
                        ControllerDelayLong = true;
                    }
                    //Launch the keyboard controller
                    else if (Controller.InputCurrent.ButtonGuide.PressedLong && vProcessKeyboardController == null)
                    {
                        await LaunchKeyboardController();

                        ControllerUsed = true;
                        ControllerDelayLong = true;
                    }
                    //Press Alt+Enter
                    else if (Controller.InputCurrent.ButtonStart.PressedRaw && Controller.InputCurrent.ButtonShoulderRight.PressedRaw)
                    {
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutAltEnter"]))
                        {
                            Debug.WriteLine("Button Global - Alt+Enter");
                            App.vWindowOverlay.Notification_Show_Status("MiniMaxi", "Pressing Alt+Enter");
                            KeyPressCombo((byte)KeysVirtual.Menu, (byte)KeysVirtual.Return, false);

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
                            App.vWindowOverlay.Notification_Show_Status("Closing", "Pressing Alt+F4");
                            KeyPressCombo((byte)KeysVirtual.Menu, (byte)KeysVirtual.F4, false);

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
                            App.vWindowOverlay.Notification_Show_Status("MiniMaxi", "Pressing Win+Tab");
                            KeyPressCombo((byte)KeysVirtual.LeftWindows, (byte)KeysVirtual.Tab, false);

                            ControllerUsed = true;
                            ControllerDelayLong = true;
                        }
                        else if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutAltTab"]))
                        {
                            Debug.WriteLine("Button Global - Alt+Tab");
                            App.vWindowOverlay.Notification_Show_Status("MiniMaxi", "Pressing Alt+Tab");
                            KeyPressCombo((byte)KeysVirtual.Menu, (byte)KeysVirtual.Tab, false);

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
                            PlayInterfaceSound("Screenshot", true);
                            KeyPressCombo((byte)KeysVirtual.LeftWindows, (byte)KeysVirtual.Snapshot, false);

                            ControllerUsed = true;
                            ControllerDelayLong = true;
                        }
                    }
                    //Disconnect controller from Bluetooth
                    else if ((Controller.InputCurrent.ButtonGuide.PressedRaw || Controller.InputCurrent.ButtonGuide.PressedShort || Controller.InputCurrent.ButtonGuide.PressedLong) && Controller.InputCurrent.ButtonStart.PressedRaw)
                    {
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutDisconnectBluetooth"]))
                        {
                            Debug.WriteLine("Shortcut Disconnect Bluetooth has been pressed.");
                            await StopController(Controller, false);

                            ControllerUsed = true;
                            ControllerDelayLong = true;
                        }
                    }
                    //Set controller as manage controller
                    else if (Controller.InputCurrent.ButtonStart.PressedRaw)
                    {
                        Debug.WriteLine("Shortcut set manage controller has been pressed.");
                        SetManageController(Controller);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
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

        //Launch the keyboard controller
        async Task LaunchKeyboardController()
        {
            try
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutLaunchKeyboardController"]))
                {
                    Debug.WriteLine("Shortcut launch keyboard controller has been pressed.");
                    App.vWindowOverlay.Notification_Show_Status("Keyboard", "Showing Keyboard");

                    if (!CheckRunningProcessByNameOrTitle("KeyboardController", false))
                    {
                        await ProcessLauncherWin32Async("KeyboardController-Admin.exe", "", "", true, false);
                    }
                }
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
                    App.vWindowOverlay.Notification_Show_Status("App", "Launching CtrlUI");

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

        //Close Keyboard Controller
        async Task CloseKeyboardController()
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
                socketSend.Object = "ApplicationExit";
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                TcpClient tcpClient = await vArnoldVinkSockets.TcpClientCheckCreateConnect(vArnoldVinkSockets.vTcpListenerIp, vArnoldVinkSockets.vTcpListenerPort + 1, vArnoldVinkSockets.vTcpClientTimeout);
                await vArnoldVinkSockets.TcpClientSendBytes(tcpClient, SerializedData, vArnoldVinkSockets.vTcpClientTimeout, false);
            }
            catch { }
        }
    }
}