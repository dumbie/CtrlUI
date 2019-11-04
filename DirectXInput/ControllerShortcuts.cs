using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessWin32Functions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

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
                        Debug.WriteLine("Guide status:" + GuideCurrent);

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
                    if (Controller.InputCurrent.ButtonGuideShort && vProcessKeyboardController == null)
                    {
                        Debug.WriteLine("Guide short press, showing ctrlui.");
                        LaunchCtrlUI();

                        ControllerUsed = true;
                        ControllerDelayLong = true;
                    }
                    else if (Controller.InputCurrent.ButtonGuideLong)
                    {
                        Debug.WriteLine("Guide long press showing keyboard controller.");
                        LaunchKeyboardController();

                        ControllerUsed = true;
                        ControllerDelayLong = true;
                    }
                    //Disconnect controller from Bluetooth
                    else if ((Controller.InputCurrent.ButtonGuide || Controller.InputCurrent.ButtonGuideShort) && Controller.InputCurrent.ButtonStart)
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

        //Launch keyboard controller when not running
        void LaunchKeyboardController()
        {
            try
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutLaunchKeyboardController"]))
                {
                    Debug.WriteLine("Shortcut launch keyboard controller has been pressed.");

                    if (!CheckRunningProcessByName("KeyboardController", false))
                    {
                        ProcessLauncherWin32("KeyboardController-Admin.exe", "", "", false, false);
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

                    if (!CheckRunningProcessByName("CtrlUI", false))
                    {
                        ProcessLauncherWin32("CtrlUI-Admin.exe", "", "", false, false);
                    }
                }
            }
            catch { }
        }
    }
}