﻿using LibraryUsb;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Media;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVInteropDll;
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

            //Color Status
            public Color? Color = null;
            public bool ColorLedBlink = false;
            public byte ColorLedCurrentR = 0;
            public byte ColorLedPreviousR = 0;
            public byte ColorLedCurrentG = 0;
            public byte ColorLedPreviousG = 0;
            public byte ColorLedCurrentB = 0;
            public byte ColorLedPreviousB = 0;

            //Battery Status
            public ControllerBattery BatteryCurrent = new ControllerBattery();
            public ControllerBattery BatteryPrevious = new ControllerBattery();

            //Timeout Variables
            public bool TimeoutIgnore = false;
            public long TicksInputLast = 0;
            public long TicksInputPrev = 0;
            public long TicksActiveLast = 0;
            public int TicksTargetTimeout = 3000;

            //Controller Details
            public ControllerDetails Details = null;
            public bool Disconnecting = false;
            public bool Connected()
            {
                try
                {
                    if (HidDevice != null && !HidDevice.Connected) { return false; }
                    else if (WinUsbDevice != null && !WinUsbDevice.Connected) { return false; }
                    else if (Details == null) { return false; }
                    else if (Disconnecting) { return false; }
                }
                catch { }
                return true;
            }

            //Controller Tasks
            public NativeOverlapped InputVirtualOverlapped = new NativeOverlapped() { EventHandle = CreateEvent(IntPtr.Zero, true, false, null) };
            public NativeOverlapped OutputVirtualOverlapped = new NativeOverlapped() { EventHandle = CreateEvent(IntPtr.Zero, true, false, null) };
            public AVTaskDetails InputControllerTask = new AVTaskDetails("InputControllerTask");
            public AVTaskDetails OutputControllerTask = new AVTaskDetails("OutputControllerTask");
            public AVTaskDetails OutputVirtualTask = new AVTaskDetails("OutputVirtualTask");
            public AVTaskDetails OutputGyroTask = new AVTaskDetails("OutputGyroTask");

            //WinUsb Device Variables
            public WinUsbDevice WinUsbDevice = null;

            //Hid Device Variables
            public HidDevice HidDevice = null;

            //Gyro Dsu Client Variables
            public uint GyroDsuClientPacketNumber = 0;
            public UdpEndPointDetails GyroDsuClientEndPoint = null;

            //Device In and Output
            public byte[] InputReport = null;
            public byte[] OutputReport = null;
            public XUSB_INPUT_REPORT XInputData = new XUSB_INPUT_REPORT();
            public XUSB_OUTPUT_REPORT XOutputData = new XUSB_OUTPUT_REPORT();
            public byte XOutputCurrentRumbleHeavy = 0;
            public byte XOutputCurrentRumbleLight = 0;
            public byte XOutputPreviousRumbleHeavy = 0;
            public byte XOutputPreviousRumbleLight = 0;

            //Controller Input
            public long Delay_CtrlUIOutput = GetSystemTicksMs();
            public long Delay_ControllerShortcut = GetSystemTicksMs();
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

                    //Timeout Variables
                    TimeoutIgnore = false;
                    TicksInputPrev = 0;
                    TicksInputLast = 0;
                    TicksActiveLast = 0;

                    //Controller Details
                    Details = null;
                    Disconnecting = false;

                    //Controller Tasks
                    InputVirtualOverlapped = new NativeOverlapped() { EventHandle = CreateEvent(IntPtr.Zero, true, false, null) };
                    OutputVirtualOverlapped = new NativeOverlapped() { EventHandle = CreateEvent(IntPtr.Zero, true, false, null) };
                    InputControllerTask = new AVTaskDetails("InputControllerTask");
                    OutputControllerTask = new AVTaskDetails("OutputControllerTask");
                    OutputVirtualTask = new AVTaskDetails("OutputVirtualTask");
                    OutputGyroTask = new AVTaskDetails("OutputGyroTask");

                    //WinUsb Device Variables
                    WinUsbDevice = null;

                    //Hid Device Variables
                    HidDevice = null;

                    //Gyro Dsu Client Variables
                    GyroDsuClientPacketNumber = 0;
                    GyroDsuClientEndPoint = null;

                    //Device In and Output
                    InputReport = null;
                    OutputReport = null;
                    XInputData = new XUSB_INPUT_REPORT();
                    XOutputData = new XUSB_OUTPUT_REPORT();
                    XOutputCurrentRumbleHeavy = 0;
                    XOutputCurrentRumbleLight = 0;
                    XOutputPreviousRumbleHeavy = 0;
                    XOutputPreviousRumbleLight = 0;

                    //Controller Input
                    InputCurrent = new ControllerInput();
                    SupportedCurrent = new ControllerSupported();
                }
                catch { }
            }
        }
    }
}