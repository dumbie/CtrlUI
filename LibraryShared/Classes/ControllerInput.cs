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

            //Raw Touchpad
            public byte TouchpadActive = 0;
            public byte TouchpadId = 0;
            public int TouchpadX = 0;
            public int TouchpadY = 0;

            //Raw Gyroscope
            public float GyroPitch = 0;
            public float GyroYaw = 0;
            public float GyroRoll = 0;

            //Raw Accelerometer
            public float AccelX = 0;
            public float AccelY = 0;
            public float AccelZ = 0;

            //Raw DPad
            public ControllerButtonDetails DPadUp = new ControllerButtonDetails();
            public ControllerButtonDetails DPadDown = new ControllerButtonDetails();
            public ControllerButtonDetails DPadLeft = new ControllerButtonDetails();
            public ControllerButtonDetails DPadRight = new ControllerButtonDetails();

            //Raw Buttons
            public ControllerButtonDetails ButtonA = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonB = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonX = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonY = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonBack = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonStart = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonGuide = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonTouchpad = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonMedia = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonShoulderLeft = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonShoulderRight = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonThumbLeft = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonThumbLeftLeft = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonThumbLeftUp = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonThumbLeftRight = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonThumbLeftDown = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonThumbRight = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonThumbRightLeft = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonThumbRightUp = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonThumbRightRight = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonThumbRightDown = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonTriggerLeft = new ControllerButtonDetails();
            public ControllerButtonDetails ButtonTriggerRight = new ControllerButtonDetails();
            public bool[] ButtonPressStatus = new bool[300];
        }
    }
}