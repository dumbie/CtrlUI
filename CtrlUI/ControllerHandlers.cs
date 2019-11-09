using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.OutputKeyboard;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Process controller input
        public async Task ControllerInteraction(ControllerInput ControllerInput)
        {
            try
            {
                if (await Controller_GlobalButtonPress(ControllerInput))
                {
                    return;
                }

                if (!vAppMinimized && vAppActivated)
                {
                    Controller_DPadPress(ControllerInput);
                    Controller_StickMovement(ControllerInput);
                    await Controller_ButtonPress(ControllerInput);
                    await Controller_TriggerPress(ControllerInput);
                }
            }
            catch { }
        }

        //Process global controller buttons
        async Task<bool> Controller_GlobalButtonPress(ControllerInput ControllerInput)
        {
            bool ControllerUsed = false;
            bool ControllerDelayShort = false;
            bool ControllerDelayLong = false;
            try
            {
                if (Environment.TickCount >= vControllerDelay_Global)
                {
                    if (ControllerInput.ButtonBack && ControllerInput.ButtonShoulderLeft)
                    {
                        Debug.WriteLine("Button Global - Screenshot");
                        if (ConfigurationManager.AppSettings["ShortcutScreenshot"] == "True")
                        {
                            PlayInterfaceSound("Screenshot", true);
                            KeyPressCombo((byte)KeysVirtual.LeftWindows, (byte)KeysVirtual.Snapshot, false);

                            ControllerUsed = true;
                            ControllerDelayLong = true;
                        }
                    }
                    else if (ControllerInput.ButtonBack && ControllerInput.ButtonShoulderRight)
                    {
                        Debug.WriteLine("Button Global - Alt/Win+Tab");
                        if (ConfigurationManager.AppSettings["ShortcutWinTab"] == "True")
                        {
                            Popup_Show_Status("MiniMaxi", "Pressing shortcut Win+Tab");
                            KeyPressCombo((byte)KeysVirtual.LeftWindows, (byte)KeysVirtual.Tab, false);

                            ControllerUsed = true;
                            ControllerDelayLong = true;
                        }
                        else if (ConfigurationManager.AppSettings["ShortcutAltTab"] == "True")
                        {
                            Popup_Show_Status("MiniMaxi", "Pressing shortcut Alt+Tab");
                            KeyPressCombo((byte)KeysVirtual.Menu, (byte)KeysVirtual.Tab, false);

                            ControllerUsed = true;
                            ControllerDelayLong = true;
                        }
                    }
                    else if (ControllerInput.ButtonStart && ControllerInput.ButtonShoulderLeft)
                    {
                        Debug.WriteLine("Button Global - Alt+F4");
                        if (ConfigurationManager.AppSettings["ShortcutAltF4"] == "True")
                        {
                            Popup_Show_Status("Closing", "Pressing shortcut Alt+F4");
                            KeyPressCombo((byte)KeysVirtual.Menu, (byte)KeysVirtual.F4, false);

                            ControllerUsed = true;
                            ControllerDelayLong = true;
                        }
                    }
                    else if (ControllerInput.ButtonStart && ControllerInput.ButtonShoulderRight)
                    {
                        Debug.WriteLine("Button Global - Alt+Enter");
                        if (ConfigurationManager.AppSettings["ShortcutAltEnter"] == "True")
                        {
                            Popup_Show_Status("MiniMaxi", "Pressing shortcut Alt+Enter");
                            KeyPressCombo((byte)KeysVirtual.Menu, (byte)KeysVirtual.Return, false);

                            ControllerUsed = true;
                            ControllerDelayLong = true;
                        }
                    }
                    else if (ControllerInput.ButtonGuideShort)
                    {
                        Debug.WriteLine("Button Global - Guide Short - Switch apps");
                        await AVActions.ActionDispatcherInvokeAsync(async delegate { await AppWindow_HideShow(); });

                        ControllerUsed = true;
                        ControllerDelayLong = true;
                    }

                    if (ControllerDelayShort)
                    {
                        vControllerDelay_DPad = Environment.TickCount + vControllerDelayShortTicks;
                        vControllerDelay_Stick = Environment.TickCount + vControllerDelayShortTicks;
                        vControllerDelay_Trigger = Environment.TickCount + vControllerDelayShortTicks;
                        vControllerDelay_Button = Environment.TickCount + vControllerDelayShortTicks;
                        vControllerDelay_Activate = Environment.TickCount + vControllerDelayShortTicks;
                        vControllerDelay_Global = Environment.TickCount + vControllerDelayShortTicks;
                    }
                    else if (ControllerDelayLong)
                    {
                        vControllerDelay_DPad = Environment.TickCount + vControllerDelayLongTicks;
                        vControllerDelay_Stick = Environment.TickCount + vControllerDelayLongTicks;
                        vControllerDelay_Trigger = Environment.TickCount + vControllerDelayLongTicks;
                        vControllerDelay_Button = Environment.TickCount + vControllerDelayLongTicks;
                        vControllerDelay_Activate = Environment.TickCount + vControllerDelayLongTicks;
                        vControllerDelay_Global = Environment.TickCount + vControllerDelayLongTicks;
                    }
                }
            }
            catch { }
            return ControllerUsed;
        }

        //Process active XInput controller buttons
        async Task<bool> Controller_ButtonPress(ControllerInput ControllerInput)
        {
            bool ControllerUsed = false;
            bool ControllerDelayShort = false;
            bool ControllerDelayMedium = false;
            bool ControllerDelayLong = false;
            try
            {
                if (Environment.TickCount >= vControllerDelay_Button)
                {
                    if (ControllerInput.ButtonA)
                    {
                        Debug.WriteLine("Button: APressed");

                        KeySendSingle((byte)KeysVirtual.Space, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayMedium = true;
                    }
                    else if (ControllerInput.ButtonB)
                    {
                        Debug.WriteLine("Button: BPressed");

                        if (Popup_Any_Open())
                        {
                            KeySendSingle((byte)KeysVirtual.Escape, vProcessCurrent.MainWindowHandle);
                        }
                        else
                        {
                            await RefreshApplicationLists(true, true, false, false);
                        }

                        ControllerUsed = true;
                        ControllerDelayMedium = true;
                    }
                    else if (ControllerInput.ButtonY)
                    {
                        Debug.WriteLine("Button: YPressed");

                        KeySendSingle((byte)KeysVirtual.Back, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayMedium = true;
                    }
                    else if (ControllerInput.ButtonX)
                    {
                        Debug.WriteLine("Button: XPressed");

                        if (vFilePickerOpen)
                        {
                            KeySendSingle((byte)KeysVirtual.Delete, vProcessCurrent.MainWindowHandle);
                        }
                        else if (vSearchOpen)
                        {
                            await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Reset_Search(true); });
                        }
                        else
                        {
                            await AVActions.ActionDispatcherInvokeAsync(async delegate { await QuickActionPrompt(); });
                        }

                        ControllerUsed = true;
                        ControllerDelayMedium = true;
                    }
                    else if (ControllerInput.ButtonShoulderLeft)
                    {
                        Debug.WriteLine("Button: ShoulderLeftPressed");
                        PlayInterfaceSound("ClickRight", false);

                        //Improve: KeySendCombo((byte)KeysVirtual.Shift, (byte)KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                        KeyPressCombo((byte)KeysVirtual.Shift, (byte)KeysVirtual.Tab, false);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ButtonShoulderRight)
                    {
                        Debug.WriteLine("Button: ShoulderRightPressed");
                        PlayInterfaceSound("ClickRight", false);

                        KeySendSingle((byte)KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ButtonBack)
                    {
                        Debug.WriteLine("Button: BackPressed / Show hide menu");
                        await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_ShowHide_MainMenu(false); });

                        ControllerUsed = true;
                        ControllerDelayLong = true;
                    }
                    else if (ControllerInput.ButtonStart)
                    {
                        Debug.WriteLine("Button: StartPressed / Show hide search");
                        await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_ShowHide_Search(false); });

                        ControllerUsed = true;
                        ControllerDelayLong = true;
                    }
                    else if (ControllerInput.ButtonThumbLeft)
                    {
                        Debug.WriteLine("Button: ThumbLeftPressed");
                        PlayInterfaceSound("ClickRight", false);

                        KeySendSingle((byte)KeysVirtual.Home, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ButtonThumbRight)
                    {
                        Debug.WriteLine("Button: ThumbRightPressed");
                        PlayInterfaceSound("ClickRight", false);

                        KeySendSingle((byte)KeysVirtual.End, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }

                    if (ControllerDelayShort)
                    {
                        vControllerDelay_Button = Environment.TickCount + vControllerDelayShortTicks;
                    }
                    else if (ControllerDelayMedium)
                    {
                        vControllerDelay_Button = Environment.TickCount + vControllerDelayMediumTicks;
                    }
                    else if (ControllerDelayLong)
                    {
                        vControllerDelay_Button = Environment.TickCount + vControllerDelayLongTicks;
                    }
                }
            }
            catch { }
            return ControllerUsed;
        }

        //Process XInput controller D-Pad
        bool Controller_DPadPress(ControllerInput ControllerInput)
        {
            bool ControllerUsed = false;
            bool ControllerDelayShort = false;
            bool ControllerDelayLong = false;
            try
            {
                if (Environment.TickCount >= vControllerDelay_DPad)
                {
                    if (ControllerInput.DPadLeft)
                    {
                        Debug.WriteLine("Button: DPadLeftPressed");

                        KeySendSingle((byte)KeysVirtual.Left, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.DPadUp)
                    {
                        Debug.WriteLine("Button: DPadUpPressed");

                        KeySendSingle((byte)KeysVirtual.Up, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.DPadRight)
                    {
                        Debug.WriteLine("Button: DPadRightPressed");

                        KeySendSingle((byte)KeysVirtual.Right, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.DPadDown)
                    {
                        Debug.WriteLine("Button: DPadDownPressed");

                        KeySendSingle((byte)KeysVirtual.Down, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }

                    if (ControllerDelayShort)
                    {
                        vControllerDelay_DPad = Environment.TickCount + vControllerDelayShortTicks;
                    }
                    else if (ControllerDelayLong)
                    {
                        vControllerDelay_DPad = Environment.TickCount + vControllerDelayLongTicks;
                    }
                }
            }
            catch { }
            return ControllerUsed;
        }

        //Process XInput controller triggers
        async Task<bool> Controller_TriggerPress(ControllerInput ControllerInput)
        {
            bool ControllerUsed = false;
            bool ControllerDelayShort = false;
            bool ControllerDelayLong = false;
            try
            {
                if (Environment.TickCount >= vControllerDelay_Trigger)
                {
                    //Check if the media popup is opened
                    bool MediaPopupOpen = false;
                    await AVActions.ActionDispatcherInvokeAsync(delegate { MediaPopupOpen = grid_Popup_Media.Visibility == Visibility.Visible; });

                    //Control the volume with controller triggers
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutVolume"]) || MediaPopupOpen)
                    {
                        if (ControllerInput.TriggerLeft > 0 && ControllerInput.TriggerRight > 0)
                        {
                            KeyPressSingle((byte)KeysVirtual.VolumeMute, false);

                            ControllerUsed = true;
                            ControllerDelayShort = true;
                        }
                        else if (ControllerInput.TriggerLeft > 0)
                        {
                            KeyPressSingle((byte)KeysVirtual.VolumeDown, false);

                            ControllerUsed = true;
                            ControllerDelayShort = true;
                        }
                        else if (ControllerInput.TriggerRight > 0)
                        {
                            KeyPressSingle((byte)KeysVirtual.VolumeUp, false);

                            ControllerUsed = true;
                            ControllerDelayShort = true;
                        }
                    }

                    if (ControllerDelayShort)
                    {
                        vControllerDelay_Trigger = Environment.TickCount + vControllerDelayShortTicks;
                    }
                    else if (ControllerDelayLong)
                    {
                        vControllerDelay_Trigger = Environment.TickCount + vControllerDelayLongTicks;
                    }
                }
            }
            catch { }
            return ControllerUsed;
        }

        //Process XInput controller sticks
        bool Controller_StickMovement(ControllerInput ControllerInput)
        {
            bool ControllerUsed = false;
            bool ControllerDelayShort = false;
            bool ControllerDelayLong = false;
            try
            {
                if (Environment.TickCount >= vControllerDelay_Stick)
                {
                    //Left stick movement
                    if (ControllerInput.ThumbLeftX < -10000 && Math.Abs(ControllerInput.ThumbLeftY) < 13000)
                    {
                        KeySendSingle((byte)KeysVirtual.Left, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ThumbLeftY > 10000 && Math.Abs(ControllerInput.ThumbLeftX) < 13000)
                    {
                        KeySendSingle((byte)KeysVirtual.Up, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ThumbLeftX > 10000 && Math.Abs(ControllerInput.ThumbLeftY) < 13000)
                    {
                        KeySendSingle((byte)KeysVirtual.Right, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ThumbLeftY < -10000 && Math.Abs(ControllerInput.ThumbLeftX) < 13000)
                    {
                        KeySendSingle((byte)KeysVirtual.Down, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }

                    //Right stick movement
                    if (ControllerInput.ThumbRightX < -10000 && Math.Abs(ControllerInput.ThumbRightY) < 13000)
                    {
                        PlayInterfaceSound("ClickRight", false);

                        KeySendSingle((byte)KeysVirtual.Home, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ThumbRightY > 10000 && Math.Abs(ControllerInput.ThumbRightX) < 13000)
                    {
                        PlayInterfaceSound("ClickRight", false);

                        KeySendSingle((byte)KeysVirtual.Prior, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ThumbRightX > 10000 && Math.Abs(ControllerInput.ThumbRightY) < 13000)
                    {
                        PlayInterfaceSound("ClickRight", false);

                        KeySendSingle((byte)KeysVirtual.End, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ThumbRightY < -10000 && Math.Abs(ControllerInput.ThumbRightX) < 13000)
                    {
                        PlayInterfaceSound("ClickRight", false);

                        KeySendSingle((byte)KeysVirtual.Next, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }

                    if (ControllerDelayShort)
                    {
                        vControllerDelay_Stick = Environment.TickCount + vControllerDelayShortTicks;
                    }
                    else if (ControllerDelayLong)
                    {
                        vControllerDelay_Stick = Environment.TickCount + vControllerDelayLongTicks;
                    }
                }
            }
            catch { }
            return ControllerUsed;
        }
    }
}