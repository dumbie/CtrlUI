using LibraryUsb;
using System;
using System.Diagnostics;
using System.Threading;
using static ArnoldVinkCode.AVActions;
using static LibraryUsb.NativeMethods_IoControl;
using static LibraryUsb.WinUsbDevice;

namespace LibraryShared
{
    public partial class Classes
    {
        public class ControllerStatus
        {
            //Controller Status
            public int NumberId = -1;
            public bool Activated = false;

            //Battery Status
            public ControllerBattery BatteryCurrent = new ControllerBattery();
            public ControllerBattery BatteryPrevious = new ControllerBattery();

            //Controller Details
            public long LastReadTicks = 0;
            public int LastActiveTicks = 0;
            public int MilliSecondsTimeout = 4000;
            public int MilliSecondsAllowReadWrite = 2000;
            public ControllerDetails Details = null;
            public bool BlockOutput = false;
            public bool Connected()
            {
                try
                {
                    if (HidDevice != null && !HidDevice.Connected) { return false; }
                    else if (WinUsbDevice != null && !WinUsbDevice.Connected) { return false; }
                    else if (Details == null) { return false; }
                }
                catch { }
                return true;
            }

            //Controller Task
            public AVTaskDetails InputTask = new AVTaskDetails();
            public NativeOverlapped InputOverlapped = new NativeOverlapped() { EventHandle = CreateEvent(IntPtr.Zero, true, false, null) };
            public AVTaskDetails OutputTask = new AVTaskDetails();
            public NativeOverlapped OutputOverlapped = new NativeOverlapped() { EventHandle = CreateEvent(IntPtr.Zero, true, false, null) };

            //WinUsb Device Variables
            public WinUsbDevice WinUsbDevice = null;

            //Hid Device Variables
            public HidDevice HidDevice = null;

            //Device In and Output
            public int InputButtonCountLoop1 = 0;
            public int InputButtonCountTotal1 = 80;
            public int InputButtonCountLoop2 = 0;
            public int InputButtonCountTotal2 = 80;
            public int InputButtonCountLoop3 = 0;
            public int InputButtonCountTotal3 = 80;
            public bool InputHeaderOffsetFinished = false;
            public int InputHeaderOffsetByte = 0;
            public bool InputButtonOffsetFinished = false;
            public int InputButtonOffsetByte = 0;
            public byte[] InputReport = null;
            public byte[] OutputReport = null;
            public XUSB_INPUT_REPORT XInputData = new XUSB_INPUT_REPORT();
            public XUSB_OUTPUT_REPORT XOutputData = new XUSB_OUTPUT_REPORT();

            //Controller Input
            public int Delay_CtrlUIOutput = Environment.TickCount;
            public int Delay_ControllerShortcut = Environment.TickCount;
            public ControllerInput InputCurrent = new ControllerInput();
            public ControllerSupported SupportedCurrent = new ControllerSupported();

            //Set used controller number
            public ControllerStatus(int numberId)
            {
                NumberId = numberId;
            }

            //Reset controller status to defaults
            public void ResetControllerStatus()
            {
                try
                {
                    Debug.WriteLine("Reset the controller status for controller: " + NumberId);

                    //Controller Status
                    Activated = false;

                    //Battery Status
                    BatteryCurrent = new ControllerBattery();
                    BatteryPrevious = new ControllerBattery();

                    //Controller Details
                    LastReadTicks = 0;
                    LastActiveTicks = 0;
                    Details = null;
                    BlockOutput = false;

                    //Controller Task
                    InputTask = new AVTaskDetails();
                    InputOverlapped = new NativeOverlapped() { EventHandle = CreateEvent(IntPtr.Zero, true, false, null) };
                    OutputTask = new AVTaskDetails();
                    OutputOverlapped = new NativeOverlapped() { EventHandle = CreateEvent(IntPtr.Zero, true, false, null) };

                    //WinUsb Device Variables
                    WinUsbDevice = null;

                    //Hid Device Variables
                    HidDevice = null;

                    //Device In and Output
                    InputButtonCountLoop1 = 0;
                    InputButtonCountTotal1 = 80;
                    InputButtonCountLoop2 = 0;
                    InputButtonCountTotal2 = 80;
                    InputButtonCountLoop3 = 0;
                    InputButtonCountTotal3 = 80;
                    InputHeaderOffsetFinished = false;
                    InputHeaderOffsetByte = 0;
                    InputButtonOffsetFinished = false;
                    InputButtonOffsetByte = 0;
                    InputReport = null;
                    OutputReport = null;
                    XInputData = new XUSB_INPUT_REPORT();
                    XOutputData = new XUSB_OUTPUT_REPORT();

                    //Controller Input
                    InputCurrent = new ControllerInput();
                    SupportedCurrent = new ControllerSupported();
                }
                catch { }
            }
        }
    }
}