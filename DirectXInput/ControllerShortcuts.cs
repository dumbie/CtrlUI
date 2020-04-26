using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
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
        void UpdateButtonPressTimes(ControllerStatus Controller)
        {
            try
            {
                if (Controller.InputCurrent.ButtonGuide)
                {
                    if (Controller.InputCurrent.ButtonGuidePressTimeCurrent == 0)
                    {
                        //Debug.WriteLine("Starting guide hold.");
                        Controller.InputCurrent.ButtonGuidePressTimeCurrent = Environment.TickCount;
                    }
                }
                else
                {
                    if (Controller.InputCurrent.ButtonGuidePressTimeDone)
                    {
                        Controller.InputCurrent.ButtonGuidePressTimePrevious = 0;
                        //Debug.WriteLine("Releasing guide hold: 0");
                    }
                    else if (Controller.InputCurrent.ButtonGuidePressTimeCurrent > 0)
                    {
                        Controller.InputCurrent.ButtonGuidePressTimePrevious = Environment.TickCount - Controller.InputCurrent.ButtonGuidePressTimeCurrent;
                        //Debug.WriteLine("Releasing guide hold: " + Controller.InputCurrent.ButtonGuidePressTimePrevious);
                    }

                    Controller.InputCurrent.ButtonGuidePressTimeDone = false;
                    Controller.InputCurrent.ButtonGuidePressTimeCurrent = 0;
                }
            }
            catch { }
        }

        //Check the guide button press times
        void CheckButtonPressTimeGuide(ControllerStatus Controller)
        {
            try
            {
                Controller.InputCurrent.ButtonGuideShort = false;
                Controller.InputCurrent.ButtonGuideLong = false;

                if (!Controller.InputCurrent.ButtonGuidePressTimeDone)
                {
                    if (Controller.InputCurrent.ButtonGuidePressTimePrevious > 0)
                    {
                        if (AVFunctions.BetweenNumbers(Controller.InputCurrent.ButtonGuidePressTimePrevious, 1, 500, true))
                        {
                            Controller.InputCurrent.ButtonGuideShort = true;
                            Controller.InputCurrent.ButtonGuidePressTimeDone = true;
                            return;
                        }
                    }

                    if (Controller.InputCurrent.ButtonGuidePressTimeCurrent > 0)
                    {
                        int GuideCurrent = Environment.TickCount - Controller.InputCurrent.ButtonGuidePressTimeCurrent;
                        //Debug.WriteLine("Guide status: " + GuideCurrent);
                        if (GuideCurrent > 850)
                        {
                            Controller.InputCurrent.ButtonGuideLong = true;
                            Controller.InputCurrent.ButtonGuidePressTimeDone = true;
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
                    //Launch CtrlUI application
                    if (Controller.InputCurrent.ButtonGuideShort && vProcessKeyboardController == null && vProcessCtrlUI == null)
                    {
                        Debug.WriteLine("Guide short press showing CtrlUI.");
                        LaunchCtrlUI();

                        ControllerUsed = true;
                        ControllerDelayLong = true;
                    }
                    //Launch the keyboard controller
                    else if (Controller.InputCurrent.ButtonGuideLong && vProcessKeyboardController == null)
                    {
                        Debug.WriteLine("Guide long press showing keyboard controller.");
                        LaunchKeyboardController();

                        ControllerUsed = true;
                        ControllerDelayLong = true;
                    }
                    //Press Alt+Enter
                    else if (Controller.InputCurrent.ButtonStart && Controller.InputCurrent.ButtonShoulderRight)
                    {
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutAltEnter"]))
                        {
                            Debug.WriteLine("Button Global - Alt+Enter");
                            App.vWindowOverlay.Overlay_Show_Status("MiniMaxi", "Pressing Alt+Enter");
                            KeyPressCombo((byte)KeysVirtual.Menu, (byte)KeysVirtual.Return, false);

                            ControllerUsed = true;
                            ControllerDelayLong = true;
                        }
                    }
                    //Press Alt+F4
                    else if (Controller.InputCurrent.ButtonStart && Controller.InputCurrent.ButtonShoulderLeft)
                    {
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutAltF4"]))
                        {
                            Debug.WriteLine("Button Global - Alt+F4");
                            App.vWindowOverlay.Overlay_Show_Status("Closing", "Pressing Alt+F4");
                            KeyPressCombo((byte)KeysVirtual.Menu, (byte)KeysVirtual.F4, false);

                            ControllerUsed = true;
                            ControllerDelayLong = true;
                        }
                    }
                    //Press Alt+Tab or Win+Tab
                    else if (Controller.InputCurrent.ButtonBack && Controller.InputCurrent.ButtonShoulderRight)
                    {
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutWinTab"]))
                        {
                            Debug.WriteLine("Button Global - Win+Tab");
                            App.vWindowOverlay.Overlay_Show_Status("MiniMaxi", "Pressing Win+Tab");
                            KeyPressCombo((byte)KeysVirtual.LeftWindows, (byte)KeysVirtual.Tab, false);

                            ControllerUsed = true;
                            ControllerDelayLong = true;
                        }
                        else if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutAltTab"]))
                        {
                            Debug.WriteLine("Button Global - Alt+Tab");
                            App.vWindowOverlay.Overlay_Show_Status("MiniMaxi", "Pressing Alt+Tab");
                            KeyPressCombo((byte)KeysVirtual.Menu, (byte)KeysVirtual.Tab, false);

                            ControllerUsed = true;
                            ControllerDelayLong = true;
                        }
                    }
                    //Make screenshot
                    else if (Controller.InputCurrent.ButtonBack && Controller.InputCurrent.ButtonShoulderLeft)
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
                    else if ((Controller.InputCurrent.ButtonGuide || Controller.InputCurrent.ButtonGuideShort || Controller.InputCurrent.ButtonGuideLong) && Controller.InputCurrent.ButtonStart)
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
                    else if (Controller.InputCurrent.ButtonStart)
                    {
                        Debug.WriteLine("Shortcut set manage controller has been pressed.");
                        SetManageController(Controller);

                        ControllerUsed = true;
                        ControllerDelayLong = true;
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
        void LaunchKeyboardController()
        {
            try
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutLaunchKeyboardController"]))
                {
                    Debug.WriteLine("Shortcut launch keyboard controller has been pressed.");

                    if (!CheckRunningProcessByNameOrTitle("KeyboardController", false))
                    {
                        ProcessLauncherWin32("KeyboardController-Admin.exe", "", "", true, false);
                    }
                }
            }
            catch { }
        }

        //Launch CtrlUI when not running
        void LaunchCtrlUI()
        {
            try
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutLaunchCtrlUI"]))
                {
                    Debug.WriteLine("Shortcut launch CtrlUI has been pressed.");

                    if (!CheckRunningProcessByNameOrTitle("CtrlUI", false))
                    {
                        App.vWindowOverlay.Overlay_Show_Status("App", "Launching CtrlUI");
                        ProcessLauncherWin32("CtrlUI-Admin.exe", "", "", true, false);
                    }
                    else
                    {
                        App.vWindowOverlay.Overlay_Show_Status("MiniMaxi", "Showing CtrlUI");
                        //Fix move this check
                    }
                }
            }
            catch { }
        }
    }
}