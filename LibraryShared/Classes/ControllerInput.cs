using System;

namespace LibraryShared
{
    public partial class Classes
    {
        [Serializable]
        public class ControllerInput
        {
            //Raw Thumbs
            public int ThumbLeftX = 0;
            public int ThumbLeftY = 0;
            public int ThumbRightX = 0;
            public int ThumbRightY = 0;

            //Raw Triggers
            public byte TriggerLeft = 0;
            public byte TriggerRight = 0;

            //Raw D-Pad
            public bool DPadUp = false;
            public bool DPadDown = false;
            public bool DPadLeft = false;
            public bool DPadRight = false;

            //Raw Buttons
            public bool ButtonA = false;
            public bool ButtonB = false;
            public bool ButtonX = false;
            public bool ButtonY = false;

            public bool ButtonBack = false;
            public bool ButtonStart = false;

            public bool ButtonGuide = false;
            public bool ButtonGuideShort = false;
            public bool ButtonGuideLong = false;
            public bool ButtonGuidePressTimeDone = false;
            public int ButtonGuidePressTimeCurrent = 0;
            public int ButtonGuidePressTimePrevious = 0;

            public bool ButtonShoulderLeft = false;
            public bool ButtonShoulderRight = false;
            public bool ButtonThumbLeft = false;
            public bool ButtonThumbRight = false;
            public bool ButtonTriggerLeft = false;
            public bool ButtonTriggerRight = false;

            //Raw Bytes
            public bool[] RawBytes = new bool[128];
        }
    }
}