using LibraryUsb;
using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVActions;

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
            public int BatteryPercentageCurrent = -1; //-1 Unknown, -2 Charging
            public int BatteryPercentagePrevious = -1; //-1 Unknown, -2 Charging

            //Controller Details
            public long LastReadTicks = 0;
            public int LastActiveTicks = 0;
            public int MilliSecondsAllowReadWrite = 2000;
            public int MilliSecondsTimeout = 4000;
            public ControllerDetails Details = null;
            public bool Connected { get { return Details != null; } }
            public bool BlockOutput = false;

            //Controller Task
            public AVTaskDetails InputTask = new AVTaskDetails();

            //WinUsb Device Variables
            public WinUsbDevice WinUsbDevice = null;

            //Hid Device Variables
            public HidDevice HidDevice = null;

            //Device In and Output
            public bool InputHeaderOffsetFinished = false;
            public int InputHeaderOffsetByte = 0;
            public bool InputButtonOffsetFinished = false;
            public int InputButtonOffsetByte = 0;
            public byte[] InputReport = null;
            public byte[] OutputReport = null;
            public byte[] XInputData = new byte[28];
            public byte[] XOutputData = new byte[8];

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
                    BatteryPercentageCurrent = -1; //-1 Unknown, -2 Charging
                    BatteryPercentagePrevious = -1; //-1 Unknown, -2 Charging

                    //Controller Details
                    LastReadTicks = 0;
                    LastActiveTicks = 0;
                    BlockOutput = false;

                    //Controller Task
                    InputTask = new AVTaskDetails();

                    //Device In and Output
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