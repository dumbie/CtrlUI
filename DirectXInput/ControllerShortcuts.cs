using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVAudioDevice;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessWin32Functions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Settings;
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
            bool ControllerDelayLonger = false;
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

                    //Hide the keyboard, keypad or media controller controller
                    if (Controller.InputCurrent.ButtonGuide.PressedShort && (App.vWindowKeyboard.vWindowVisible || App.vWindowKeypad.vWindowVisible || App.vWindowMedia.vWindowVisible))
                    {
                        await HideOpenPopups();

                        ControllerUsed = true;
                        ControllerDelayLonger = true;
                    }
                    //Show CtrlUI application
                    else if (Controller.InputCurrent.ButtonGuide.PressedShort && vProcessCtrlUI != null)
                    {
                        Debug.WriteLine("Guide short press showing CtrlUI.");
                        await ShowCtrlUI();

                        ControllerUsed = true;
                        ControllerDelayLonger = true;
                    }
                    //Launch CtrlUI application
                    else if (Controller.InputCurrent.ButtonGuide.PressedShort && vProcessCtrlUI == null)
                    {
                        await LaunchCtrlUI();

                        ControllerUsed = true;
                        ControllerDelayLonger = true;
                    }
                    //Show the keyboard controller
                    else if (Controller.InputCurrent.ButtonGuide.PressedLong && !App.vWindowKeyboard.vWindowVisible && !App.vWindowKeypad.vWindowVisible)
                    {
                        await KeyboardControllerHideShow(true);

                        ControllerUsed = true;
                        ControllerDelayLonger = true;
                    }
                    //Switch between keyboard and Keypad
                    else if (Controller.InputCurrent.ButtonGuide.PressedLong && (App.vWindowKeyboard.vWindowVisible || App.vWindowKeypad.vWindowVisible))
                    {
                        await KeyboardKeypadSwitch();

                        ControllerUsed = true;
                        ControllerDelayLonger = true;
                    }
                    //Show or hide the media controller
                    else if (Controller.InputCurrent.ButtonTouchpad.PressedRaw)
                    {
                        await MediaControllerHideShow(false);

                        ControllerUsed = true;
                        ControllerDelayLonger = true;
                    }
                    //Mute or unmute the input/microphone
                    else if (Controller.InputCurrent.ButtonMedia.PressedRaw)
                    {
                        App.vWindowOverlay.Notification_Show_Status("MicrophoneMute", "Toggling input mute");
                        AudioMuteSwitch(true);

                        ControllerUsed = true;
                        ControllerDelayLonger = true;
                    }
                    //Press Alt+Enter
                    else if (Controller.InputCurrent.ButtonStart.PressedRaw && Controller.InputCurrent.ButtonShoulderRight.PressedRaw)
                    {
                        if (Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutAltEnter")))
                        {
                            Debug.WriteLine("Button Global - Alt+Enter");

                            NotificationDetails notificationDetails = new NotificationDetails();
                            notificationDetails.Icon = "AppMiniMaxi";
                            notificationDetails.Text = "Pressing Alt+Enter";
                            App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                            await KeyPressComboAuto(KeysVirtual.Alt, KeysVirtual.Enter);

                            ControllerUsed = true;
                            ControllerDelayLonger = true;
                        }
                    }
                    //Press Alt+F4
                    else if (Controller.InputCurrent.ButtonStart.PressedRaw && Controller.InputCurrent.ButtonShoulderLeft.PressedRaw)
                    {
                        if (Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutAltF4")))
                        {
                            Debug.WriteLine("Button Global - Alt+F4");

                            NotificationDetails notificationDetails = new NotificationDetails();
                            notificationDetails.Icon = "AppClose";
                            notificationDetails.Text = "Pressing Alt+F4";
                            App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                            await KeyPressComboAuto(KeysVirtual.Alt, KeysVirtual.F4);

                            ControllerUsed = true;
                            ControllerDelayLonger = true;
                        }
                    }
                    //Press Alt+Tab or Win+Tab
                    else if (Controller.InputCurrent.ButtonBack.PressedRaw && Controller.InputCurrent.ButtonShoulderRight.PressedRaw)
                    {
                        if (Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutWinTab")))
                        {
                            Debug.WriteLine("Button Global - Win+Tab");

                            NotificationDetails notificationDetails = new NotificationDetails();
                            notificationDetails.Icon = "AppMiniMaxi";
                            notificationDetails.Text = "Pressing Win+Tab";
                            App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                            await KeyPressComboAuto(KeysVirtual.LeftWindows, KeysVirtual.Tab);

                            ControllerUsed = true;
                            ControllerDelayLonger = true;
                        }
                        else if (Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutAltTab")))
                        {
                            Debug.WriteLine("Button Global - Alt+Tab");

                            NotificationDetails notificationDetails = new NotificationDetails();
                            notificationDetails.Icon = "AppMiniMaxi";
                            notificationDetails.Text = "Pressing Alt+Tab";
                            App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                            await KeyPressComboAuto(KeysVirtual.Alt, KeysVirtual.Tab);

                            ControllerUsed = true;
                            ControllerDelayLonger = true;
                        }
                    }
                    //Make screenshot
                    else if (Controller.InputCurrent.ButtonBack.PressedRaw && Controller.InputCurrent.ButtonShoulderLeft.PressedRaw)
                    {
                        if (Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutScreenshot")))
                        {
                            Debug.WriteLine("Button Global - Screenshot");
                            PlayInterfaceSound(vConfigurationCtrlUI, "Screenshot", true);

                            await KeyPressComboAuto(KeysVirtual.LeftWindows, KeysVirtual.Snapshot);

                            ControllerUsed = true;
                            ControllerDelayLonger = true;
                        }
                    }
                    //Disconnect controller from Bluetooth
                    else if (Controller.InputCurrent.ButtonStart.PressedRaw && Controller.InputCurrent.ButtonGuide.PressedRaw)
                    {
                        if (Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutDisconnectBluetooth")))
                        {
                            Debug.WriteLine("Shortcut disconnect Bluetooth has been pressed.");
                            StopControllerTask(Controller, "manually");

                            ControllerUsed = true;
                            ControllerDelayLonger = true;
                        }
                    }

                    if (ControllerDelayShort)
                    {
                        Controller.Delay_ControllerShortcut = Environment.TickCount + vControllerDelayShortTicks;
                    }
                    else if (ControllerDelayLonger)
                    {
                        Controller.Delay_ControllerShortcut = Environment.TickCount + vControllerDelayLongerTicks;
                    }
                }
            }
            catch { }
            return ControllerUsed;
        }

        //Switch between keyboard and keypad
        async Task KeyboardKeypadSwitch()
        {
            try
            {
                Debug.WriteLine("Switching between keyboard and keypad");
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    if (App.vWindowKeyboard.vWindowVisible)
                    {
                        App.vWindowKeyboard.Hide();
                        await App.vWindowKeypad.Show();
                    }
                    else
                    {
                        await App.vWindowKeypad.Hide();
                        await App.vWindowKeyboard.Show();
                    }
                });
            }
            catch { }
        }

        //Hide or show the keyboard controller
        async Task KeyboardControllerHideShow(bool forceShow)
        {
            try
            {
                Debug.WriteLine("Shortcut keyboard has been pressed.");
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    if (!App.vWindowKeyboard.vWindowVisible && !App.vWindowKeypad.vWindowVisible)
                    {
                        await App.vWindowKeyboard.Show();
                    }
                    else if (!forceShow)
                    {
                        App.vWindowKeyboard.Hide();
                        await App.vWindowKeypad.Hide();
                    }
                });
            }
            catch { }
        }

        //Hide all opened popups
        async Task HideOpenPopups()
        {
            try
            {
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    App.vWindowKeyboard.Hide();
                    await App.vWindowKeypad.Hide();
                    await App.vWindowMedia.Hide();
                });
            }
            catch { }
        }

        //Hide or show the media controller
        async Task MediaControllerHideShow(bool forceShow)
        {
            try
            {
                Debug.WriteLine("Shortcut media has been pressed.");
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    if (!App.vWindowMedia.vWindowVisible)
                    {
                        await App.vWindowMedia.Show();
                    }
                    else if (!forceShow)
                    {
                        await App.vWindowMedia.Hide();
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
                if (Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutLaunchCtrlUI")))
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