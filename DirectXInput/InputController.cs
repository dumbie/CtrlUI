using System;
using System.Runtime.InteropServices;
using static LibraryShared.Classes;
using static LibraryUsb.WinUsbDevice;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //65535 = 0xFFFF Hex = 255 * 257 Int
        byte TranslateByte_0xFF(int rawOffset, int rawState) { return Convert.ToByte((rawState >> rawOffset) & 0xFF); }
        byte TranslateByte_0x0F(int rawOffset, int rawState) { return Convert.ToByte((rawState >> rawOffset) & 0x0F); }
        byte TranslateByte_0x10(int rawOffset, int rawState) { return Convert.ToByte((rawState >> rawOffset) & 0x10); }

        //Lookup button group offset
        int LookupButtonGroupOffset(int targetGroup, ControllerSupported controllerSupported)
        {
            try
            {
                switch (targetGroup)
                {
                    case 1: return (int)controllerSupported.OffsetHeader.ButtonsGroup1;
                    case 2: return (int)controllerSupported.OffsetHeader.ButtonsGroup2;
                    case 3: return (int)controllerSupported.OffsetHeader.ButtonsGroup3;
                }
            }
            catch { }
            return 0;
        }

        //Update and prepare XInput byte data
        void PrepareXInputDataEmpty(ControllerStatus Controller)
        {
            try
            {
                //Set report structure
                XUSB_REPORT usbReport = new XUSB_REPORT();

                //Set submit structure
                Controller.XInputData = new XUSB_INPUT_REPORT();
                Controller.XInputData.Size = Marshal.SizeOf(Controller.XInputData);
                Controller.XInputData.SerialNo = Controller.NumberId + 1;
                Controller.XInputData.Report = usbReport;
            }
            catch { }
        }

        //Update and prepare XInput byte data
        void PrepareXInputDataCurrent(ControllerStatus Controller)
        {
            try
            {
                //Set report structure
                XUSB_REPORT usbReport = new XUSB_REPORT();

                //Set current input
                //Thumbs
                usbReport.sThumbLX = (short)Controller.InputCurrent.ThumbLeftX;
                usbReport.sThumbLY = (short)Controller.InputCurrent.ThumbLeftY;
                usbReport.sThumbRX = (short)Controller.InputCurrent.ThumbRightX;
                usbReport.sThumbRY = (short)Controller.InputCurrent.ThumbRightY;

                //Triggers
                if (!Controller.Details.Profile.UseButtonTriggers)
                {
                    usbReport.bLeftTrigger = Controller.InputCurrent.TriggerLeft;
                    usbReport.bRightTrigger = Controller.InputCurrent.TriggerRight;
                }
                else
                {
                    if (Controller.InputCurrent.ButtonTriggerLeft.PressedRaw) { usbReport.bLeftTrigger = 255; }
                    if (Controller.InputCurrent.ButtonTriggerRight.PressedRaw) { usbReport.bRightTrigger = 255; }
                }

                //DPad
                if (Controller.InputCurrent.DPadLeft.PressedRaw) { usbReport.wButtons |= XUSB_BUTTON.XUSB_GAMEPAD_DPAD_LEFT; }
                if (Controller.InputCurrent.DPadUp.PressedRaw) { usbReport.wButtons |= XUSB_BUTTON.XUSB_GAMEPAD_DPAD_UP; }
                if (Controller.InputCurrent.DPadRight.PressedRaw) { usbReport.wButtons |= XUSB_BUTTON.XUSB_GAMEPAD_DPAD_RIGHT; }
                if (Controller.InputCurrent.DPadDown.PressedRaw) { usbReport.wButtons |= XUSB_BUTTON.XUSB_GAMEPAD_DPAD_DOWN; }

                //Buttons
                if (Controller.InputCurrent.ButtonA.PressedRaw) { usbReport.wButtons |= XUSB_BUTTON.XUSB_GAMEPAD_A; }
                if (Controller.InputCurrent.ButtonB.PressedRaw) { usbReport.wButtons |= XUSB_BUTTON.XUSB_GAMEPAD_B; }
                if (Controller.InputCurrent.ButtonX.PressedRaw) { usbReport.wButtons |= XUSB_BUTTON.XUSB_GAMEPAD_X; }
                if (Controller.InputCurrent.ButtonY.PressedRaw) { usbReport.wButtons |= XUSB_BUTTON.XUSB_GAMEPAD_Y; }
                if (Controller.InputCurrent.ButtonBack.PressedRaw) { usbReport.wButtons |= XUSB_BUTTON.XUSB_GAMEPAD_BACK; }
                if (Controller.InputCurrent.ButtonStart.PressedRaw) { usbReport.wButtons |= XUSB_BUTTON.XUSB_GAMEPAD_START; }
                if (Controller.InputCurrent.ButtonGuide.PressedRaw) { usbReport.wButtons |= XUSB_BUTTON.XUSB_GAMEPAD_GUIDE; }
                if (Controller.InputCurrent.ButtonShoulderLeft.PressedRaw) { usbReport.wButtons |= XUSB_BUTTON.XUSB_GAMEPAD_LEFT_SHOULDER; }
                if (Controller.InputCurrent.ButtonShoulderRight.PressedRaw) { usbReport.wButtons |= XUSB_BUTTON.XUSB_GAMEPAD_RIGHT_SHOULDER; }
                if (Controller.InputCurrent.ButtonThumbLeft.PressedRaw) { usbReport.wButtons |= XUSB_BUTTON.XUSB_GAMEPAD_LEFT_THUMB; }
                if (Controller.InputCurrent.ButtonThumbRight.PressedRaw) { usbReport.wButtons |= XUSB_BUTTON.XUSB_GAMEPAD_RIGHT_THUMB; }

                //Set submit structure
                Controller.XInputData = new XUSB_INPUT_REPORT();
                Controller.XInputData.Size = Marshal.SizeOf(Controller.XInputData);
                Controller.XInputData.SerialNo = Controller.NumberId + 1;
                Controller.XInputData.Report = usbReport;
            }
            catch { }
        }
    }
}