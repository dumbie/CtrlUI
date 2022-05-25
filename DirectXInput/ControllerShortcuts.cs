using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVAudioDevice;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Settings;
using static LibraryUsb.FakerInputDevice;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Check if controller shortcut is pressed
        async Task<bool> ControllerShortcut(ControllerStatus Controller)
        {
            bool ControllerUsed = false;
            bool ControllerDelay125 = false;
            bool ControllerDelay750 = false;
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
                        ControllerDelay125 = true;

                        if (controllerActivated) { return ControllerUsed; }
                    }

                    //Hide popups with guide button
                    if (Controller.InputCurrent.ButtonGuide.PressedShort && (App.vWindowKeyboard.vWindowVisible || App.vWindowKeypad.vWindowVisible))
                    {
                        await HideOpenPopups();

                        ControllerUsed = true;
                        ControllerDelay750 = true;
                    }
                    //Show or launch CtrlUI application
                    else if (Controller.InputCurrent.ButtonGuide.PressedShort)
                    {
                        if (Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutLaunchCtrlUI")))
                        {
                            Debug.WriteLine("Shortcut launch CtrlUI has been pressed.");
                            Debug.WriteLine("Guide short press showing CtrlUI.");
                            await ProcessFunctions.LaunchShowCtrlUI();

                            ControllerUsed = true;
                            ControllerDelay750 = true;
                        }
                    }
                    //Show the keyboard or keypad
                    else if (Controller.InputCurrent.ButtonGuide.PressedLong)
                    {
                        if (!App.vWindowKeyboard.vWindowVisible && !App.vWindowKeypad.vWindowVisible)
                        {
                            if (vKeyboardKeypadLastActive == "Keyboard")
                            {
                                await KeyboardPopupHideShow(false);
                            }
                            else
                            {
                                await KeypadPopupHideShow(false);
                            }
                        }
                        else
                        {
                            await KeyboardKeypadPopupSwitch();
                        }

                        ControllerUsed = true;
                        ControllerDelay750 = true;
                    }
                    //Mute or unmute the input/microphone
                    else if (Controller.InputCurrent.ButtonMedia.PressedRaw)
                    {
                        int muteFunction = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "ShortcutMuteFunction"));
                        if (muteFunction == 0)
                        {
                            if (AudioMuteSwitch(true))
                            {
                                await App.vWindowOverlay.Notification_Show_Status("MicrophoneMute", "Input muted");
                            }
                            else
                            {
                                await App.vWindowOverlay.Notification_Show_Status("MicrophoneMute", "Input unmuted");
                            }
                        }
                        else
                        {
                            if (AudioMuteSwitch(false))
                            {
                                await App.vWindowOverlay.Notification_Show_Status("VolumeMute", "Output muted");
                            }
                            else
                            {
                                await App.vWindowOverlay.Notification_Show_Status("VolumeMute", "Output unmuted");
                            }
                        }

                        ControllerUsed = true;
                        ControllerDelay750 = true;
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

                            vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.AltLeft, KeyboardModifiers.None, KeyboardKeys.Enter, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);

                            ControllerUsed = true;
                            ControllerDelay750 = true;
                        }
                    }
                    //Press Alt+Tab
                    else if (Controller.InputCurrent.ButtonStart.PressedRaw && Controller.InputCurrent.ButtonShoulderLeft.PressedRaw)
                    {
                        if (Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutAltTab")))
                        {
                            Debug.WriteLine("Button Global - Alt+Tab");

                            NotificationDetails notificationDetails = new NotificationDetails();
                            notificationDetails.Icon = "AppMiniMaxi";
                            notificationDetails.Text = "Pressing Alt+Tab";
                            await App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                            vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.AltLeft, KeyboardModifiers.None, KeyboardKeys.Tab, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);

                            ControllerUsed = true;
                            ControllerDelay750 = true;
                        }
                    }
                    //Make screenshot
                    else if (Controller.InputCurrent.ButtonTouchpad.PressedRaw)
                    {
                        if (Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutScreenshot")))
                        {
                            Debug.WriteLine("Button Global - Screenshot");
                            await CaptureScreen.CaptureScreenToFile();

                            ControllerUsed = true;
                            ControllerDelay750 = true;
                        }
                    }
                    //Disconnect controller from Bluetooth
                    else if (Controller.InputCurrent.ButtonStart.PressedRaw && Controller.InputCurrent.ButtonGuide.PressedRaw && Controller.Details.Wireless)
                    {
                        if (Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutDisconnectBluetooth")))
                        {
                            Debug.WriteLine("Shortcut disconnect Bluetooth has been pressed.");
                            StopControllerTask(Controller, "manually", string.Empty);

                            ControllerUsed = true;
                            ControllerDelay750 = true;
                        }
                    }

                    if (ControllerDelay125)
                    {
                        Controller.Delay_ControllerShortcut = GetSystemTicksMs() + vControllerDelayTicks125;
                    }
                    else if (ControllerDelay750)
                    {
                        Controller.Delay_ControllerShortcut = GetSystemTicksMs() + vControllerDelayTicks750;
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
                        if (forceShow || Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutKeyboardPopup")))
                        {
                            await App.vWindowKeypad.Show();
                        }
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
                        if (forceShow || Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "ShortcutKeyboardPopup")))
                        {
                            await App.vWindowKeyboard.Show();
                        }
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
                });
            }
            catch { }
        }
    }
}