using LibraryUsb;
using System;
using System.Diagnostics;
using System.Threading;
using static ArnoldVinkCode.AVActions;
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
            public bool Connected { get { return Details != null; } }
            public bool BlockOutput = false;

            //Controller Task
            public AVTaskDetails InputTask = new AVTaskDetails();
            public ManualResetEvent ManualResetEventInput = new ManualResetEvent(false);
            public AVTaskDetails OutputTask = new AVTaskDetails();
            public ManualResetEvent ManualResetEventOutput = new ManualResetEvent(false);

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
                    BlockOutput = false;

                    //Controller Task
                    InputTask = new AVTaskDetails();
                    ManualResetEventInput = new ManualResetEvent(false);
                    OutputTask = new AVTaskDetails();
                    ManualResetEventOutput = new ManualResetEvent(false);

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

                    //Controller Input
                    InputCurrent = new ControllerInput();
                    SupportedCurrent = new ControllerSupported();
                }
                catch { }
            }
        }
    }
}