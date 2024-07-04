using System;
using System.Linq;
using static ArnoldVinkCode.AVInputOutputClass;

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
            public byte Touchpad1Active = 0;
            public byte Touchpad2Active = 0;
            public byte Touchpad1Id = 0;
            public byte Touchpad2Id = 0;
            public int Touchpad1X = 0;
            public int Touchpad2X = 0;
            public int Touchpad1Y = 0;
            public int Touchpad2Y = 0;

            //Raw Gyroscope
            public float GyroPitch = 0;
            public float GyroYaw = 0;
            public float GyroRoll = 0;

            //Raw Accelerometer
            public float AccelX = 0;
            public float AccelY = 0;
            public float AccelZ = 0;

            //Raw Buttons
            public ControllerButtonDetails[] Buttons = Enumerable.Range(0, Enum.GetNames(typeof(ControllerButtons)).Length).Select(x => new ControllerButtonDetails()).ToArray();
        }
    }
}