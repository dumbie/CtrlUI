using LibraryUsb;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryShared
{
    public partial class Classes
    {
        public class ControllerStatus
        {
            //Controller Status
            public int NumberId = -1;
            public bool Manage = false;

            //Battery Status
            public int BatteryPercentageCurrent = -1; //-1 Unknown, -2 Charging
            public int BatteryPercentagePrevious = -1; //-1 Unknown, -2 Charging

            //Controller Connected
            public int LastActive = 0;
            public int TimeoutMilliSeconds = 4000;
            public ControllerConnected Connected = null;

            //Controller Task
            public Task InputTask = null;
            public CancellationTokenSource InputTaskToken = null;

            //Controller Mapping
            public string[] Mapping = new string[] { "Done", "None" }; //Map, Done, Cancel

            //WinUsb Device Variables
            public WinUsbDevice WinUsbDevice = null;

            //X360 Device Variables
            public WinUsbDevice X360Device = null;

            //Hid Device Variables
            public HidDevice HidDevice = null;

            //Device In and Output
            public int InputHeaderByteOffset = 0;
            public int InputButtonByteOffset = 0;
            public byte[] InputReport = null;
            public byte[] OutputReport = null;
            public byte[] XInputData = new byte[28];
            public byte[] XOutputData = new byte[8];

            //Controller Input
            public int Delay_CtrlUIOutput = Environment.TickCount;
            public int Delay_KeyboardControllerShortcut = Environment.TickCount;
            public int Delay_MouseOutput = Environment.TickCount;
            public int Delay_ControllerShortcut = Environment.TickCount;
            public ControllerInput InputCurrent = new ControllerInput();

            //Reset the Controller Status to defaults
            public void ResetControllerStatus()
            {
                try
                {
                    Debug.WriteLine("Reset the controller status for controller: " + NumberId);

                    //Controller Status
                    Manage = false;

                    //Battery Status
                    BatteryPercentageCurrent = -1; //-1 Unknown, -2 Charging
                    BatteryPercentagePrevious = -1; //-1 Unknown, -2 Charging

                    //Controller Connected
                    LastActive = 0;

                    //Controller Mapping
                    Mapping = new string[] { "Done", "None" }; //Map, Done, Cancel

                    //Device In and Output
                    InputHeaderByteOffset = 0;
                    InputButtonByteOffset = 0;
                    InputReport = null;
                    OutputReport = null;

                    //Controller Input
                    InputCurrent = new ControllerInput();
                }
                catch { }
            }
        }
    }
}