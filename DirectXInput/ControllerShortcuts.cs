using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVAudioDevice;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessWin32Functions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Settings;
using static LibraryShared.SoundPlayer;
using static LibraryUsb.FakerInputDevice;

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
                if (GetSystemTicksMs() >= Controller.Delay_ControllerShortcut)
                {
                    //Activate the controller
                    if (Controller.InputCurrent.ButtonGuide.PressedShort)
                    {
                        Debug.WriteLine("Shortcut activate controller has been pressed.");
                        bool controllerActivated = await ControllerActivate(Controller);

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
                    //Show the keyboard or keypad
                    else if (Controller.InputCurrent.ButtonGuide.PressedLong)
                    {
                        if (!App.vWindowKeyboard.vWindowVisible && !App.vWindowKeypad.vWindowVisible)
                        {
                            if (vKeyboardKeypadLastActive == "Keyboard")
                            {
                                await KeyboardPopupHideShow(true);
                            }
                            else
                            {
                                await KeypadPopupHideShow(true);
                            }
                        }
                        else
                        {
                            await KeyboardKeypadPopupSwitch();
                        }

                        ControllerUsed = true;
                        ControllerDelayLonger = true;
                    }
                    //Show or hide the media controller
                    else if (Controller.InputCurrent.ButtonTouchpad.PressedRaw)
                    {
                        if (Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutMediaPopup")))
                        {
                            await MediaPopupHideShow(false);

                            ControllerUsed = true;
                            ControllerDelayLonger = true;
                        }
                    }
                    //Mute or unmute the input/microphone
                    else if (Controller.InputCurrent.ButtonMedia.PressedRaw)
                    {
                        int muteFunction = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "ShortcutMuteFunction"));
                        if (muteFunction == 0)
                        {
                            await App.vWindowOverlay.Notification_Show_Status("MicrophoneMute", "Toggling input mute");
                            AudioMuteSwitch(true);
                        }
                        else
                        {
                            await App.vWindowOverlay.Notification_Show_Status("VolumeMute", "Toggling output mute");
                            AudioMuteSwitch(false);
                        }

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
                            await App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                            vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.AltLeft, KeyboardKeys.Enter, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);

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
                            await App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                            vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.AltLeft, KeyboardKeys.F4, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);

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
                            await App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                            vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.WindowsLeft, KeyboardKeys.Tab, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);

                            ControllerUsed = true;
                            ControllerDelayLonger = true;
                        }
                        else if (Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutAltTab")))
                        {
                            Debug.WriteLine("Button Global - Alt+Tab");

                            NotificationDetails notificationDetails = new NotificationDetails();
                            notificationDetails.Icon = "AppMiniMaxi";
                            notificationDetails.Text = "Pressing Alt+Tab";
                            await App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                            vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.AltLeft, KeyboardKeys.Tab, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);

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

                            vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.WindowsLeft, KeyboardKeys.PrintScreen, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);

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
                            StopControllerTask(Controller, "manually", string.Empty);

                            ControllerUsed = true;
                            ControllerDelayLonger = true;
                        }
                    }

                    if (ControllerDelayShort)
                    {
                        Controller.Delay_ControllerShortcut = GetSystemTicksMs() + vControllerDelayShortTicks;
                    }
                    else if (ControllerDelayLonger)
                    {
                        Controller.Delay_ControllerShortcut = GetSystemTicksMs() + vControllerDelayLongerTicks;
                    }
                }
            }
            catch { }
            return ControllerUsed;
        }

        //Switch between keyboard and keypad
        async Task KeyboardKeypadPopupSwitch()
        {
            try
            {
                Debug.WriteLine("Switching between keyboard and keypad");
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    if (App.vWindowKeyboard.vWindowVisible)
                    {
                        await App.vWindowKeyboard.Hide();
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

        //Hide or show the keypad
        async Task KeypadPopupHideShow(bool forceShow)
        {
            try
            {
                Debug.WriteLine("Shortcut keypad has been pressed.");
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    if (!App.vWindowKeyboard.vWindowVisible && !App.vWindowKeypad.vWindowVisible)
                    {
                        await App.vWindowKeypad.Show();
                    }
                    else if (!forceShow)
                    {
                        await App.vWindowKeyboard.Hide();
                        await App.vWindowKeypad.Hide();
                    }
                });
            }
            catch { }
        }

        //Hide or show the keyboard
        async Task KeyboardPopupHideShow(bool forceShow)
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
                        await App.vWindowKeyboard.Hide();
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
                    await App.vWindowKeyboard.Hide();
                    await App.vWindowKeypad.Hide();
                    await App.vWindowMedia.Hide();
                });
            }
            catch { }
        }

        //Hide or show the media controller
        async Task MediaPopupHideShow(bool forceShow)
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
                    await App.vWindowOverlay.Notification_Show_Status(notificationDetails);

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
                socketSend.SourceIp = vArnoldVinkSockets.vSocketServerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vSocketServerPort;
                socketSend.Object = "AppWindowHideShow";
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                TcpClient tcpClient = await vArnoldVinkSockets.TcpClientCheckCreateConnect(vArnoldVinkSockets.vSocketServerIp, vArnoldVinkSockets.vSocketServerPort - 1, vArnoldVinkSockets.vSocketTimeout);
                await vArnoldVinkSockets.TcpClientSendBytes(tcpClient, SerializedData, vArnoldVinkSockets.vSocketTimeout, false);
            }
            catch { }
        }
    }
}