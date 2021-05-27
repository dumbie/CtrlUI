namespace LibraryShared
{
    public partial class Classes
    {
        public class ControllerSupported
        {
            public string CodeName { get; set; } = "Unsupported";
            public string[] ProductIDs { get; set; }
            public string VendorID { get; set; }
            public bool HasTouchpad { get; set; }
            public bool HasGyroscope { get; set; }
            public bool HasAccelerometer { get; set; }
            public bool HasRumbleTrigger { get; set; }
            public Offset OffsetUsb { get; set; } = new Offset();
            public Offset OffsetWireless { get; set; } = new Offset();

            public class Offset
            {
                public int BeginOffset { get; set; } = 0;
                public int ThumbLeftX { get; set; } = 0;
                public int ThumbLeftY { get; set; } = 1;
                public int ThumbRightX { get; set; } = 2;
                public int ThumbRightY { get; set; } = 3;
                public int ButtonsGroup1 { get; set; } = 4;
                public int ButtonsGroup2 { get; set; } = 5;
                public int ButtonsGroup3 { get; set; } = 6;
                public int TriggerLeft { get; set; } = 7;
                public int TriggerRight { get; set; } = 8;
                public int Touchpad { get; set; } = 34;
                public int Gyroscope { get; set; } = 12;
                public int Accelerometer { get; set; } = 18;
            }
        }
    }
}